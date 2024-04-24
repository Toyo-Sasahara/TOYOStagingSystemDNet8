using SasaLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagingSystemTools
{
    public static class TestGenerateTIFF
    {

        public static void Test1_A0()
        {
            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(28087, 19866, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            newBitmap.SetResolution(600, 600);

            System.Drawing.Image ImageTiff = SasaLib.ImageUtil.ChangeResolution(newBitmap, 300,System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);
        }


        public static void Test2_A2()
        {
            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(28087, 19866, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            newBitmap.SetResolution(600F, 600F);
            newBitmap.Save(@"D:\org600F.bmp");

            newBitmap.SetResolution(96F, 96F);
            newBitmap.Save(@"D:\org96F.bmp");

            System.Drawing.Bitmap ImageTiff = SasaLib.ImageUtil.ChangeSize(newBitmap,18724,13244,System.Drawing.Drawing2D.InterpolationMode.Default);
            ImageTiff.SetResolution(400F, 400F);
            ImageTiff.Save(@"D:\org400F.bmp");

        }

        public static void Test2_A3()
        {

            //OpenFileDialogクラスのインスタンスを作成
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "*.bmp";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = @"D:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "イメージファイル(*.bmp;*.png;*.tif;*.tiff)|*.bmp;*.png;*.tif;*.tiff|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 1;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(28087, 19866, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                //newBitmap.SetResolution(600F, 600F);
                //newBitmap.Save(@"D:\org600F.bmp");
                string filename = ofd.FileName;

                System.Drawing.Image soruceImage = SasaLib.ImageUtil.CreateImageFromFile(filename);
                System.Drawing.Bitmap output = SasaLib.ImageUtil.ChangeResolution((Bitmap)soruceImage, 400f,400f);

            }

        }

        private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, System.Drawing.Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }
    }
}
