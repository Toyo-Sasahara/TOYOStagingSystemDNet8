using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Drawing;
using System.Drawing.Imaging;

namespace SasaImagePrinter;

internal static class PdfImageConverter
{
    public static async Task<IReadOnlyList<string>> ConvertAsync(
        byte[] pdfBytes, string format, PrinterSettings settings, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(settings.OutputDirectory);
        string tempPath = Path.Combine(Path.GetTempPath(), $"SasaImagePrinter-{Guid.NewGuid():N}.pdf");
        await File.WriteAllBytesAsync(tempPath, pdfBytes, cancellationToken);
        var outputs = new List<string>();

        try
        {
            StorageFile pdfFile = await StorageFile.GetFileFromPathAsync(tempPath);
            PdfDocument document = await PdfDocument.LoadFromFileAsync(pdfFile);
            string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
            string jobId = Guid.NewGuid().ToString("N")[..8];
            Guid encoderId = format.Equals("JPEG", StringComparison.OrdinalIgnoreCase)
                ? BitmapEncoder.JpegEncoderId : BitmapEncoder.TiffEncoderId;
            string extension = format.Equals("JPEG", StringComparison.OrdinalIgnoreCase) ? "jpg" : "tif";

            for (uint index = 0; index < document.PageCount; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using PdfPage page = document.GetPage(index);
                uint width = Math.Max(1, (uint)Math.Round(page.Size.Width * settings.Dpi / 96.0));
                uint height = Math.Max(1, (uint)Math.Round(page.Size.Height * settings.Dpi / 96.0));
                using var rendered = new InMemoryRandomAccessStream();
                await page.RenderToStreamAsync(rendered, new PdfPageRenderOptions
                {
                    DestinationWidth = width,
                    DestinationHeight = height
                });

                string outputPath = Path.Combine(settings.OutputDirectory,
                    $"Print-{stamp}-{jobId}-p{index + 1:D3}.{extension}");

                if (format.Equals("TIFF", StringComparison.OrdinalIgnoreCase))
                {
                    rendered.Seek(0);
                    await SaveBinaryCcittGroup4TiffAsync(rendered, outputPath, settings.Dpi);
                    outputs.Add(outputPath);
                    continue;
                }

                rendered.Seek(0);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(rendered);
                SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync(
                    BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore);

                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(settings.OutputDirectory);
                StorageFile output = await folder.CreateFileAsync(
                    Path.GetFileName(outputPath), CreationCollisionOption.GenerateUniqueName);
                using IRandomAccessStream stream = await output.OpenAsync(FileAccessMode.ReadWrite);
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
                encoder.SetSoftwareBitmap(bitmap);
                await encoder.FlushAsync();
                bitmap.Dispose();
                outputs.Add(output.Path);
            }
        }
        finally
        {
            try { File.Delete(tempPath); } catch { }
        }
        return outputs;
    }

    private static async Task SaveBinaryCcittGroup4TiffAsync(
        IRandomAccessStream rendered, string outputPath, uint dpi)
    {
        using var reader = new DataReader(rendered.GetInputStreamAt(0));
        uint byteCount = checked((uint)rendered.Size);
        await reader.LoadAsync(byteCount);
        byte[] encodedPage = new byte[byteCount];
        reader.ReadBytes(encodedPage);

        using var sourceStream = new MemoryStream(encodedPage, writable: false);
        using var source = new Bitmap(sourceStream);
        using var binary = ConvertToOneBit(source, dpi);
        ImageCodecInfo tiffCodec = ImageCodecInfo.GetImageEncoders()
            .Single(codec => codec.FormatID == ImageFormat.Tiff.Guid);
        using var parameters = new EncoderParameters(1);
        parameters.Param[0] = new EncoderParameter(
            System.Drawing.Imaging.Encoder.Compression,
            (long)EncoderValue.CompressionCCITT4);
        binary.Save(outputPath, tiffCodec, parameters);
    }

    private static Bitmap ConvertToOneBit(Bitmap source, uint dpi)
    {
        using var rgb = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage(rgb))
        {
            graphics.Clear(Color.White);
            graphics.DrawImageUnscaled(source, 0, 0);
        }
        var result = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);
        result.SetResolution(dpi, dpi);
        Rectangle bounds = new(0, 0, source.Width, source.Height);
        BitmapData sourceData = rgb.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData targetData = result.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

        try
        {
            int sourceStride = Math.Abs(sourceData.Stride);
            int targetStride = Math.Abs(targetData.Stride);
            byte[] pixels = new byte[sourceStride * source.Height];
            byte[] packed = new byte[targetStride * source.Height];
            System.Runtime.InteropServices.Marshal.Copy(sourceData.Scan0, pixels, 0, pixels.Length);
            for (int y = 0; y < source.Height; y++)
            {
                int sourceRow = sourceData.Stride > 0 ? y * sourceStride : (source.Height - 1 - y) * sourceStride;
                int targetRow = targetData.Stride > 0 ? y * targetStride : (source.Height - 1 - y) * targetStride;
                for (int x = 0; x < source.Width; x++)
                {
                    int offset = sourceRow + x * 3;
                    int luminance = (pixels[offset + 2] * 299 + pixels[offset + 1] * 587 + pixels[offset] * 114) / 1000;
                    if (luminance >= 128)
                        packed[targetRow + (x >> 3)] |= (byte)(0x80 >> (x & 7));
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(packed, 0, targetData.Scan0, packed.Length);
        }
        finally
        {
            rgb.UnlockBits(sourceData);
            result.UnlockBits(targetData);
        }
        return result;
    }
}
