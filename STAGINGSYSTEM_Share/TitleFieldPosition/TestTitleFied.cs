using SasaLib;
using SasaLib.PrintConfig;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace TitleFieldPosition
{
    public class TestTitleFied
    {
        public MemoryStream CreateTestDrawing(CommonPaperSize cp, float Dpi, string FullFileName, string titleImage = "TestTitleFieldBase.bmp",float titleDpi = 400)
        {

            var baseimage = CreaateImage(cp, Dpi);
            

            string f1 = System.IO.Path.GetDirectoryName(FullFileName);
            string s1 = System.IO.Path.GetFileNameWithoutExtension(FullFileName);
            string se1 = System.IO.Path.GetExtension(FullFileName);

            string orgimage = System.IO.Path.Combine(f1, $"{s1}_Original"+se1);
            baseimage.Save(orgimage);

            //表題欄右下の座標をゲット

            var anser1 = TitleFieldConfig.Config.TitleFieldPosition.Find(x => x.CommonPapserSize == cp);
            var X = anser1.BottomRightBasePositionX;
            var Y = anser1.BottomRightBasePositionY;

            //自分自身の実行ファイルのパスを取得する
            string AppPath = SasaLib.FileFolder.GetFolderName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string titleImageFullFileName = Path.Combine(AppPath, titleImage);

            baseimage = AddTitledField(baseimage, titleImageFullFileName, titleDpi,(float)X, (float)Y);

            string stampedimage = System.IO.Path.Combine(f1, $"{s1}_Originalスタンプ後" + ".png");
            baseimage.Save(stampedimage);

            string stampedimagetif = System.IO.Path.Combine(f1, $"{s1}_Originalスタンプ後2" + ".tif");
            bool ans = ImageUtil.SaveImageToFile(baseimage, "image/tiff", EncoderValue.CompressionCCITT4, stampedimagetif);

            var sw = GetMemoryStreamFromImage(baseimage);

            // TIFFメモリストリームをファイルに保存
            StreamExtensions.StreamToFile(sw, FullFileName);

            return sw;
        }

        System.Drawing.Image CreaateImage(CommonPaperSize cp, float Dpi)
        {
            System.Drawing.Size size = PaperCheck.GetCommonPaperSize(CommonPaperSize.A4P);

            //int width = size.Width * (int)(Dpi / 25.4);
            //int height = size.Height * (int)(Dpi / 25.4);
            int width = 3304;
            int height = 4677;

            System.Drawing.Bitmap bitmap = new Bitmap(width, height);

            bitmap.SetResolution(Dpi, Dpi);
            System.Drawing.Image newImage = bitmap;

            // ImageオブジェクトからGraphicsオブジェクトを生成
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(newImage);

            SolidBrush brush = new SolidBrush(Color.White);
            gr.FillRectangle(brush, 0, 0, width, height);

            int A = 0;
            int W = 1;
            Pen p = new Pen(Color.Black,W);
            gr.DrawRectangle(p, 0+A, 0+A, width - A*2, height - A*2);

            gr.Dispose();
            return newImage;
        }

        System.Drawing.Image AddTitledField(System.Drawing.Image baseImage, string stampFullfilename,float stampDpi,  float X, float Y)
        {
            System.Drawing.Bitmap loadimg = new System.Drawing.Bitmap(stampFullfilename);
            MemoryStream ms = new MemoryStream();
            loadimg.Save(ms, ImageFormat.Bmp);

            Bitmap addImage = new Bitmap(ms);

            addImage.MakeTransparent(Color.Red); // 透過色

            Bitmap addBitmap = new Bitmap(addImage);
            addBitmap.SetResolution(stampDpi, stampDpi);

            // StampBitmap を OriginalImageに上書きする
            ImageUtil.OverwritingImage(baseImage, addBitmap,  X, Y);

            return baseImage;
        }

        MemoryStream GetMemoryStreamFromImage(System.Drawing.Image image)
        {
            // メモリストリームを用意し、TIFF CCITT4圧縮のストリームデータへ変換します。
            MemoryStream tiffStream = new MemoryStream();
            ImageUtil.ImageToTIFF1bppCCITT4Stream(image, tiffStream);

            return tiffStream;

        }


    }
}
