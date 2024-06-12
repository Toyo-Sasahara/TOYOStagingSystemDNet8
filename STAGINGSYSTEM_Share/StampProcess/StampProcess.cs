
using MailNotice;
using SasaLib;
using SasaLib.PrintConfig;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using TitleFieldPosition;

namespace ToyoStageService
{
    /// <summary>
    /// 承認印捺印処理
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class StampProcess
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        static MailAccount mailAccount = new MailAccount(
            StageServerConfig.Config.EMAILSERVER,
            StageServerConfig.Config.EMAILSENDPORT,
            StageServerConfig.Config.EMAILNOTICE_SendToADDR,
            StageServerConfig.Config.SMTPAUTHUSER,
            StageServerConfig.Config.SMTPAUTHPASS_SasaLibEncryptionType,
            StageServerConfig.Config.SMTPAUTHPASS
        );

        /// <summary>
        /// イメージファイル
        /// </summary>
        private string CurOrgFilePath { get; set; }

        /// <summary>
        /// イメージオブジェクト
        /// </summary>
        private System.Drawing.Image OriginalImage;

        /// <summary>
        /// イメージオブジェクトの用紙サイズ・向き
        /// </summary>
        private readonly CommonPaperSize curPaperSize;

        /// <summary>
        /// ■コンストラクタ スタンプを上書きする元のイメージを指定
        /// </summary>
        /// <param name="ImageFilePath"></param>
        public StampProcess(string ImageFilePath)
        {
            CurOrgFilePath = ImageFilePath;

            bool ans = LoadImage(ImageFilePath);
            if (ans)
            {
                // ペーパーサーズを取得
                curPaperSize = ImageUtil.GetPaparSize(OriginalImage).CommonPaperSize;
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"スタンプ元イメージ {ImageFilePath} を読み込みました");
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"このファイルの用紙サイズは {curPaperSize} と判断します");
            }
            else
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001, $"※{AssemblyInternalName}\nStampProcess(...) スタンプ元イメージ読込失敗 {ImageFilePath}");
                MailNotice.SendEmailFromCommonLibrary("ToyoSTAMPprocess", "重大エラー",
                    $"※{AssemblyInternalName}\nStampProcess(...) スタンプ元イメージ読込失敗 {ImageFilePath}", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
            }
        }

        /// <summary>
        /// ■スタンプ用イメージ（白紙）テンプレートファイルを作成 
        /// </summary>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <param name="resolution"></param>
        public static void MakeStampTemplate(float W = 19f, float H = 16f, float resolution = 400)
        {

            long x = ImageUtil.MilliToPixel(W, resolution);
            long y = ImageUtil.MilliToPixel(H, resolution);

            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"スタンプサイズドットX,Y={x},{y}");
            SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001, $"●{AssemblyInternalName}\nMakeStampTemplate(...)実行します\nスタンプサイズドットX,Y={x},{y}");

            Bitmap bmporg = new Bitmap((int)x, (int)y);
            Graphics flagGraphics = Graphics.FromImage(bmporg);

            flagGraphics.FillRectangle(Brushes.Black, 0, 0, x, y);

            string stampTemplateFullFileName = StageServerConfigWork.ConfigFolder + System.IO.Path.DirectorySeparatorChar + @"StampBaseTemplate.bmp";
            bmporg.Save(stampTemplateFullFileName);

            SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001, $"●{AssemblyInternalName}\nMakeStampTemplate(...)実行完了。{stampTemplateFullFileName}を作成しました");
        }

        /// <summary>
        /// ■スタンプ押印実行
        /// </summary>
        /// <param name="baseImageFullFileName">スタンプ先のイメージファイルパス</param>
        /// <param name="Line2">日付</param>
        /// <param name="Line1">苗字</param>
        /// <param name="Line3">名前</param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public static bool StampingGo(string baseImageFullFileName, string Line2, string Line1, string Line3, int X, int Y, double scale = 1.2, string StampTemplate = "StampBase.bmp", float StampTemplateDPI = 400f, SasaLibDelegateWriteLine WriteLine = null)
        {
            LockFileTable.WaitLoop(baseImageFullFileName);

            if (scale <= 0)
                throw new ArgumentException("StampProcess.StampingGo(..) 引数 scale は 0より大きい必要がある");

            try
            {
                LockFileTable.Add(baseImageFullFileName);

                string ImageFile = FileFolder.GetFileName(baseImageFullFileName);

                SharedClassLibrary.DebugClass.ConsoleDebugOut(3, $"\t▼StampingGo({ImageFile},{Line2},{Line1},{Line3},{X},{Y}) 開始");

                StampProcess stampProcess = new StampProcess(baseImageFullFileName);

                // スタンプイメージを生成
                var stampImage = stampProcess.GetStampImg(StampTemplate, StampTemplateDPI,
                    Line1str: Line1, Line2str: Line2, Line3str: Line3,
                    Line1_FontSize: StampConfig.Config.Line1_FontSize,
                    Line2_FontSize: StampConfig.Config.Line2_FontSize,
                    Line3_FontSize: StampConfig.Config.Line3_FontSize,
                    Scale: scale,
                    Line1_X: StampConfig.Config.Line1_X,
                    Line1_Y: StampConfig.Config.Line1_Y,
                    Line1_Xe: StampConfig.Config.Line1_Xe,

                    Line2_X: StampConfig.Config.Line2_X,
                    Line2_Y: StampConfig.Config.Line2_Y,
                    Line2_Xe: StampConfig.Config.Line2_Xe,

                    Line3_X: StampConfig.Config.Line3_X,
                    Line3_Y: StampConfig.Config.Line3_Y,
                    Line3_Xe: StampConfig.Config.Line3_Xe,

                    centering1: true, centering2: false, centering3: true,
                    debugMode: StampConfig.Config.DebugMode
                    );

                stampProcess.StampAdd(stampImage, X, Y);

                if (WriteLine != null)
                    WriteLine($"●{AssemblyInternalName} StampProcess.StampingGo(...)実行. ImageFile=押印先ファイル:{baseImageFullFileName},押印日;{Line2},所属部署:{Line1},押印者名:{Line3},押印座標:{X},{Y} , スタンプテンプレートファイル；{StampTemplate}");

                LockFileTable.Remove(baseImageFullFileName);

                return true;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001, $"※{AssemblyInternalName}\nStampProcess.StampingGo(...)にて例外発生\n" +
                    $"ImageFile=押印先ファイル:{baseImageFullFileName},押印日;{Line2},所属部署:{Line1},押印者名:{Line3},押印座標:{X},{Y}\n" +
                    $"スタンプテンプレートファイル；{StampTemplate}" +
                    $"{ex.Message}");
                LockFileTable.Remove(baseImageFullFileName);

                MailNotice.SendEmailFromCommonLibrary("ToyoSTAMPprocess", "重大エラー",
                    $"※{AssemblyInternalName}\nStampProcess.StampingGo(...)にて例外発生\n" +
                    $"ImageFile=押印先ファイル:{baseImageFullFileName},押印日;{Line2},所属部署:{Line1},押印者名:{Line3},押印座標:{X},{Y}\n" +
                    $"スタンプテンプレートファイル；{StampTemplate}" +
                    $"{ex.Message}", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);

                return false;
            }

        }

        /// <summary>
        /// ■スタンプ削除イメージを押印
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static bool StampingEraseGo(string ImagePath, int X, int Y, double scale = 1.2, SasaLibDelegateWriteLine WriteLine = null)
        {
            try
            {
                LockFileTable.WaitLoop(ImagePath);

                string ImageFile = FileFolder.GetFileName(ImagePath);

                SharedClassLibrary.DebugClass.ConsoleDebugOut(3, $"●StampingEraceGo({ImageFile},{X},{Y}) 開始");

                StampProcess stampProcess = new StampProcess(ImagePath);

                // スタンプ削除イメージを生成
                var image = stampProcess.GetEraceStampImg("StampEraceBase.bmp", 400f);

                bool ans = stampProcess.StampAdd(image, X, Y);
                if (ans == false)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001, $"●{AssemblyInternalName}\nstampProcess.StampAdd(image, X, Y) 書き込み失敗\n" +
                        $"ImageFile=押印先ファイル:{ImagePath},スタンプ削除イメージを使用,押印座標:{X},{Y}", false);

                    return false;
                }

                if (WriteLine != null)
                    WriteLine($"●{AssemblyInternalName}\nStampProcess.StampingEraceGo(...)実行, ImageFile=押印先ファイル:{ImagePath},スタンプ削除イメージを使用,押印座標:{X},{Y}");

                LockFileTable.Remove(ImagePath);

                return true;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001,
                    $"※{AssemblyInternalName}\nStampProcess.StampingEraceGo(...)にて例外発生\n" +
                    $"ImageFile=押印先ファイル:{ImagePath},押印座標:{X},{Y}\n" +
                    $"{ex.Message}");

                MailNotice.SendEmailFromCommonLibrary("ToyoSTAMPprocess", "重大エラー",
                    $"※{AssemblyInternalName}\nStampProcess.StampingEraceGo(...)にて例外発生\n" +
                    $"ImageFile=押印先ファイル:{ImagePath},押印座標:{X},{Y}\n" +
                    $"{ex.Message}", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);

                return false;
            }

        }

        /// <summary>
        /// イメージを読込
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private bool LoadImage(string soucefullPath)
        {
            string distFullPath = null;
            try
            {
                var tempFilenameWithoutExtension = "[" + System.IO.Path.GetFileNameWithoutExtension(soucefullPath) + "]" + "_" + Path.GetRandomFileName();

                distFullPath = SasaLib.FileFolder.ChangeExtension(tempFilenameWithoutExtension, "TMP");

                File.Copy(soucefullPath, distFullPath, true);
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001, $"※イメージファイル\"{soucefullPath}\"読込およびテンポラリファイル\"{distFullPath}\"作成失敗 \nIOException.Message={ex.Message}");
                return false;
            }

            try
            {
                System.Drawing.Image orgImage = System.Drawing.Image.FromFile(distFullPath);
                MemoryStream ms = new MemoryStream();
                ImageUtil.ImageToTIFF1bppCCITT4Stream(orgImage, ms);

                orgImage.Dispose();

                OriginalImage = ImageUtil.TiffStreamToImage(ms);

                ms.Dispose();

                SasaLib.FileFolder.RemoveFile(distFullPath);

                return true;

            }
            catch (IOException ioex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001, $"※イメージファイル\"{distFullPath}\"読込失敗 \nIOException.Message={ioex.Message} {ioex.InnerException}");
                return false;
            }
        }

        /// <summary>
        /// 指定した場所にスタンプを押印
        /// 
        /// </summary>
        /// <param name="StampImage"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        private bool StampAdd(Image StampImage, double X, double Y)
        {
            if (OriginalImage == null) { SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "curOrgImage がnull"); return false; }

            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"\t●StampAdd(..)　①curOrgImage Resolution H,V = {OriginalImage.HorizontalResolution},{OriginalImage.VerticalResolution}");
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"\t●StampAdd(..)  ②stampImg Width,Height = {StampImage.Width},{StampImage.Height}");

            Bitmap StampBitmap = new Bitmap(StampImage);
            float hdpi = StampImage.HorizontalResolution;
            float vdpi = StampImage.VerticalResolution;
            StampBitmap.SetResolution(hdpi, vdpi); // ImageのれぞリューションをBitmapに適用

            //用紙サイズを判定
            var commonPaperSize = ImageUtil.GetPaparSize(OriginalImage).CommonPaperSize;
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"\t●StampAdd(..) ③押印先イメージの認定サイズ：commonPaperSize={commonPaperSize}");

            //表題欄右下の座標をゲット
            var zero = GetTitleOffset(commonPaperSize);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"{commonPaperSize}の表題欄右下の座標はイメージデータ右下からオフセット({zero.X},{zero.Y})です");

            // StampBitmap を OriginalImageに上書きする
            ImageUtil.OverwritingImage(OriginalImage, StampBitmap, (float)(X + zero.X), (float)(Y + zero.Y));
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"\t●StampAdd(..) ④StampBitmapをOriginalImageに上書きしたはず");

            #region
            //bool ans = ImageUtil.SaveImageToFile(OriginalImage, "image/tiff", EncoderValue.CompressionCCITT4, CurOrgFilePath);
            //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"\t●StampAdd(..) ⑤OriginalImageを{CurOrgFilePath}に書き戻したはず。メソッド終了");
            #endregion

            #region Imageオブジェクトをファイルへ保存。繰り返しあり
            bool ans;
            int count = 0;
            do
            {
                count++;
                ans = ImageUtil.SaveImageToFile(OriginalImage, "image/tiff", EncoderValue.CompressionCCITT4, CurOrgFilePath);
                if (count > 10)
                {
                    Thread.Sleep(500);
                    SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001, $"※ImageUtil.SaveImageToFile()再試行{count}回");

                    return false;
                }
            }
            while (ans == false);
            #endregion
            return ans;
        }

        /// <summary>
        /// CommonPaperSize毎に設定した表題欄の右下座標mmを得る
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        private System.Windows.Point GetTitleOffset(CommonPaperSize cp)
        {
            foreach (var a in TitleFieldConfig.Config.TitleFieldPosition)
            {
                if (a.CommonPapserSize == cp)
                {
                    return new System.Windows.Point(a.BottomRightBasePositionX, a.BottomRightBasePositionY);
                }
            }
            return new System.Windows.Point(0, 0);
        }


        /// <summary>
        /// スタンプイメージを生成
        /// </summary>
        /// <param name="StampBaseTemplate">スタンプのテンプレート</param>
        /// <param name="TemplateDPI"></param>
        /// <param name="Line1str">1行目</param>
        /// <param name="Line2str">2行目</param>
        /// <param name="Line3str">3行目</param>
        /// <param name="Line1_FontSize">1行目フォントサイズ</param>
        /// <param name="Line2_FontSize">2行目フォントサイズ</param>
        /// <param name="Line3_FontSize">3行目フォントサイズ</param>
        /// <param name="Scale">拡大率</param>
        /// <param name="Line1_X">1行目X開始位置</param>
        /// <param name="Line1_Y">1行目Y開始位置</param>
        /// <param name="Line1_Xe">1行目X終了位置</param>
        /// <param name="Line2_X"><2行目X開始位置/param>
        /// <param name="Line2_Y">2行目Y開始位置</param>
        /// <param name="Line2_Xe">2行目X終了位置</param>
        /// <param name="Line3_X">3行目X開始位置</param>
        /// <param name="Line3_Y">3行目Y開始位置</param>
        /// <param name="Line3_Xe">3行目X終了位置</param>
        /// <param name="centering1">1行目をセンタリングする</param>
        /// <param name="centering2">2行目をセンタリングする</param>
        /// <param name="centering3">3行目をセンタリングする</param>
        /// <param name="StampdImageFullPath">デバッグ用。スタンプをファイルとして保存する場合のフルパス</param>
        /// <returns></returns>
        private Image GetStampImg(
            string StampBaseTemplate,
            float TemplateDPI,
            string Line1str,
            string Line2str,
            string Line3str,
            int Line1_FontSize,
            int Line2_FontSize,
            int Line3_FontSize,
            double Scale,
            int Line1_X, int Line1_Y, int Line1_Xe,
            int Line2_X, int Line2_Y, int Line2_Xe,
            int Line3_X, int Line3_Y, int Line3_Xe,
            bool centering1 = false, bool centering2 = false, bool centering3 = false,
            bool debugMode = false,
            string StampdImageFullPath = "")
        {
            // template の大きさは、　400dpi で 24bit 透明箇所はredとする W=267 H=220
            if (Line1str == "")
                Line1str = "";
            if (Line2str == "")
                Line2str = "1971-04-02";
            if (Line3str == "")
                Line3str = "テスト";


            //自分自身の実行ファイルのパスを取得する
            string AppPath = SasaLib.FileFolder.GetFolderName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string StampBaseTemplateFullPath = Path.Combine(AppPath, StampBaseTemplate);

            if (SasaLib.FileFolder.FileExists(StampBaseTemplateFullPath) != true)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001,
                    $"※東陽機械技術部 ステージングサーバー\nGetStampImg( {Line1str},{Line2str},{Line3str})\n{StampBaseTemplateFullPath}がありません。サービスを終了します");
                MailNotice.SendEmailFromCommonLibrary("ToyoSTAMPprocess", "重大エラー",
                    $"※GetStampImg(..) にて必須ファイル不足\n" +
                    $"{StampBaseTemplateFullPath}がありません。サービスを終了します\n" +
                    $"", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
                Environment.Exit(1);
                //アプリケーションを強制終了します
                Environment.Exit(1);
            }

            try
            {
                System.Drawing.Bitmap loadimg = new System.Drawing.Bitmap(StampBaseTemplateFullPath);
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"GetStampImg(..) スタンプテンプレート：{StampBaseTemplateFullPath}ロードしました");
                loadimg.SetResolution(TemplateDPI, TemplateDPI);

                MemoryStream ms = new MemoryStream();
                loadimg.Save(ms, ImageFormat.Bmp);

                Bitmap newImage = new Bitmap(ms);

                newImage.MakeTransparent(Color.Red);
                // ImageオブジェクトからGraphicsオブジェクトを生成
                System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(newImage);

                if (centering1)
                {

                    ImageUtil.DrawFontToImage(newImage, Line1str, Line1_X, Line1_Y, StampConfig.Config.Line1_Font_Name, Line1_FontSize, centering: true, Line1_Xe, debugmode: debugMode);
                }
                else
                    ImageUtil.DrawFontToImage(newImage, Line1str, Line1_X, Line1_Y, StampConfig.Config.Line1_Font_Name, Line1_FontSize, centering: false, Line1_Xe, debugmode: debugMode);

                if (centering2)
                {

                    ImageUtil.DrawFontToImage(newImage, Line2str, Line2_X, Line2_Y, StampConfig.Config.Line2_Font_Name, Line2_FontSize, centering: true, Line2_Xe, debugmode: debugMode);
                }
                else
                    ImageUtil.DrawFontToImage(newImage, Line2str, Line2_X, Line2_Y, StampConfig.Config.Line2_Font_Name, Line2_FontSize, centering: false, Line2_Xe, debugmode: debugMode);

                if (centering3)
                {

                    ImageUtil.DrawFontToImage(newImage, Line3str, Line3_X, Line3_Y, StampConfig.Config.Line3_Font_Name, Line3_FontSize, centering: true, Line3_Xe, debugmode: debugMode);
                }
                else
                    ImageUtil.DrawFontToImage(newImage, Line3str, Line3_X, Line3_Y, StampConfig.Config.Line3_Font_Name, Line3_FontSize, centering: false, Line3_Xe, debugmode: debugMode);


                /// スケール・テスト中
                if (Scale != 1.0)
                {
                    int W = (int)((double)newImage.Width * Scale);
                    int H = (int)((double)newImage.Height * Scale);

                    newImage = ImageUtil.Myresize(newImage, W, H);
                }

                newImage.SetResolution(loadimg.HorizontalResolution, loadimg.VerticalResolution); // Dpiを適用

                //デバッグ用
                if (String.IsNullOrWhiteSpace(StampdImageFullPath) != true)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Information, 5001, $"GetStampImg(..)　デバッグ用としてスタンプを次のファイル\n" +
                            $"{StampdImageFullPath}\n" +
                            $"に保存しました");
                    newImage.Save(StampdImageFullPath);
                }

                //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"\tGetStampImg({Line1},{Line2},{Line3})　終了");
                // Imageを返す
                return newImage;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001,
                    $"※GetStampImg(..) にて例外発生\n" +
                    $"StampBaseTemplateFullPath:{StampBaseTemplateFullPath}\n" +
                    $"{ex.Message}");

                MailNotice.SendEmailFromCommonLibrary("ToyoSTAMPprocess", "重大エラー",
                    $"※GetStampImg(..) にて例外発生\n" +
                    $"StampBaseTemplateFullPath:{StampBaseTemplateFullPath}\n" +
                    $"{ex.Message}", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);

                return null;
            }

        }

        /// <summary>
        /// 削除用スタンプイメージを生成
        /// </summary>
        /// <param name="Scale"></param>
        /// <param name="StampBaseTemplate"></param>
        /// <param name="StampdImageFullPath"></param>
        /// <returns></returns>
        private Image GetEraceStampImg(
                string StampBaseTemplate,
                float TemplateDPI,
                double Scale = 1.0,
                string StampdImageFullPath = "")
        {
            // template の大きさは、　400dpi で 24bit 透明箇所はredとする W=267 H=220


            //自分自身の実行ファイルのパスを取得する
            string AppPath = SasaLib.FileFolder.GetFolderName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string StampBaseTemplateFullPath = Path.Combine(AppPath, StampBaseTemplate);

            if (SasaLib.FileFolder.FileExists(StampBaseTemplateFullPath) != true)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001,
                    $"▼東陽機械技術部 ステージングサーバー\nGetEraceStampImg(...)\n{StampBaseTemplateFullPath}がありません。サービスを終了します");
                MailNotice.SendEmailFromCommonLibrary("ToyoSTAMPprocess", "重大エラー",
                    $"GetStampImg(..) にて必須ファイル不足\n" +
                    $"{StampBaseTemplateFullPath}がありません。サービスを終了します\n" +
                    $"", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
                //アプリケーションを強制終了します
                Environment.Exit(1);
            }

            try
            {
                System.Drawing.Bitmap loadimg = new System.Drawing.Bitmap(StampBaseTemplateFullPath);
                //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"GetEraceStampImg(..) スタンプテンプレート：{StampBaseTemplateFullPath}ロードしました");

                loadimg.SetResolution(TemplateDPI, TemplateDPI);

                MemoryStream ms = new MemoryStream();
                loadimg.Save(ms, ImageFormat.Bmp);

                Bitmap newImage = new Bitmap(ms);

                newImage.MakeTransparent(Color.Red);
                // ImageオブジェクトからGraphicsオブジェクトを生成
                System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(newImage);



                /// スケール・テスト中
                if (Scale != 1.0)
                {
                    int W = (int)((double)newImage.Width * Scale);
                    int H = (int)((double)newImage.Height * Scale);

                    newImage = ImageUtil.Myresize(newImage, W, H);
                }

                newImage.SetResolution(loadimg.HorizontalResolution, loadimg.VerticalResolution); // Dpiを適用

                //デバッグ用
                if (String.IsNullOrWhiteSpace(StampdImageFullPath) != true)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Information, 5001, $"GetEraceStampImg(..)　デバッグ用としてスタンプを次のファイル\n" +
                            $"{StampdImageFullPath}\n" +
                            $"に保存しました");
                    newImage.Save(StampdImageFullPath);
                }

                //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"\tGetStampImg({Line1},{Line2},{Line3})　終了");
                // Imageを返す
                return newImage;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPprocess", EventLogEntryType.Error, 5001, $"GetEraceStampImg(..) にて例外発生\n" +
                    $"StampBaseTemplateFullPath:{StampBaseTemplateFullPath}\n" +
                    $"{ex.Message}");
                return null;
            }

        }

    }
}
