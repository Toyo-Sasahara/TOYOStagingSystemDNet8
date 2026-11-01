using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SasaPdfRelayPrinter;

internal static class PdfPrintDispatcher
{
    public static async Task<int> PrintAsync(byte[] pdfBytes, string printerName, uint dpi, CancellationToken cancellationToken)
    {
        List<Bitmap> pages = await RenderPagesAsync(pdfBytes, dpi, cancellationToken);
        try
        {
            await Task.Run(() => PrintBitmaps(pages, printerName), cancellationToken);
            return pages.Count;
        }
        finally
        {
            foreach (Bitmap bitmap in pages)
            {
                bitmap.Dispose();
            }
        }
    }

    private static async Task<List<Bitmap>> RenderPagesAsync(byte[] pdfBytes, uint dpi, CancellationToken cancellationToken)
    {
        string tempPath = Path.Combine(Path.GetTempPath(), $"SasaPdfRelay-{Guid.NewGuid():N}.pdf");
        await File.WriteAllBytesAsync(tempPath, pdfBytes, cancellationToken);
        var pages = new List<Bitmap>();

        try
        {
            StorageFile pdfFile = await StorageFile.GetFileFromPathAsync(tempPath);
            PdfDocument document = await PdfDocument.LoadFromFileAsync(pdfFile);
            for (uint index = 0; index < document.PageCount; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using PdfPage page = document.GetPage(index);
                pages.Add(await RenderPageAsync(page, dpi));
            }
        }
        finally
        {
            try
            {
                File.Delete(tempPath);
            }
            catch
            {
            }
        }

        return pages;
    }

    private static async Task<Bitmap> RenderPageAsync(PdfPage page, uint dpi)
    {
        uint width = Math.Max(1, (uint)Math.Round(page.Size.Width * dpi / 96.0));
        uint height = Math.Max(1, (uint)Math.Round(page.Size.Height * dpi / 96.0));
        using var rendered = new InMemoryRandomAccessStream();
        await page.RenderToStreamAsync(rendered, new PdfPageRenderOptions
        {
            DestinationWidth = width,
            DestinationHeight = height
        });

        rendered.Seek(0);
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(rendered);
        SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore);

        using var encoded = new InMemoryRandomAccessStream();
        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, encoded);
        encoder.SetSoftwareBitmap(softwareBitmap);
        await encoder.FlushAsync();
        softwareBitmap.Dispose();

        encoded.Seek(0);
        using var reader = new DataReader(encoded.GetInputStreamAt(0));
        uint bytesCount = checked((uint)encoded.Size);
        await reader.LoadAsync(bytesCount);
        byte[] bytes = new byte[bytesCount];
        reader.ReadBytes(bytes);

        using var memory = new MemoryStream(bytes, writable: false);
        using var source = new Bitmap(memory);
        var copy = new Bitmap(source);
        copy.SetResolution(dpi, dpi);
        return copy;
    }

    private static void PrintBitmaps(IReadOnlyList<Bitmap> pages, string printerName)
    {
        if (pages.Count == 0)
        {
            throw new InvalidOperationException("No printable pages were rendered.");
        }

        using var document = new PrintDocument();
        document.PrinterSettings.PrinterName = printerName;
        if (!document.PrinterSettings.IsValid)
        {
            throw new InvalidOperationException($"Printer '{printerName}' is not available.");
        }

        document.PrintController = new StandardPrintController();
        int pageIndex = 0;

        document.PrintPage += (_, e) =>
        {
            Bitmap page = pages[pageIndex];
            Rectangle bounds = e.MarginBounds;
            float ratio = Math.Min((float)bounds.Width / page.Width, (float)bounds.Height / page.Height);
            int width = Math.Max(1, (int)Math.Round(page.Width * ratio));
            int height = Math.Max(1, (int)Math.Round(page.Height * ratio));
            int left = bounds.Left + (bounds.Width - width) / 2;
            int top = bounds.Top + (bounds.Height - height) / 2;
            e.Graphics.DrawImage(page, new Rectangle(left, top, width, height));

            pageIndex++;
            e.HasMorePages = pageIndex < pages.Count;
        };

        document.Print();
    }
}
