using SasaLib;
using SasaLib.PrintConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;



/// <summary>
/// TIFF CTTI4 図面を生成
/// </summary>
[SupportedOSPlatform("windows")]
public class GenerateTIFFdrawing
{
    static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

    public int DebugLevel { get; private set; }

    public GenerateTIFFdrawing(int debugLevel = 0)
    {
        this.DebugLevel = debugLevel;
    }

    /// <summary>
    /// ■プリンタドライバからの出力ファイル(*.pm)(TIFFデータ内包PJL言語) からTIFFイメージを抜き出して　白黒2値可, CompressionCCITT4 TIFFイメージファイルに変換します。ImagePositonConfig データによるオフセットが可能です。
    /// Inventor と Solidworks から使用
    /// </summary>
    /// <param name="importPrinterOutputFullpath"></param>
    /// <param name="exportTiffFullpath"></param>
    /// <param name="currentPaperSize"></param>
    /// <param name="LogWrite">Console(stirng msg)形式のメソッド名へログ出力をデリゲートします</param>
    /// <returns></returns>
    public bool ConvertPMfileToTIFFfile(string importPrinterOutputFullpath, string exportTiffFullpath, out CommonPaperSize currentPaperSize,
            float outputDpi = 400f,
            bool testfileSaveMode = false, SasaLibDelegateWriteLine LogWrite = null)
    {
        string saveDirectory = System.IO.Path.GetDirectoryName(exportTiffFullpath);
        string saveFilenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(exportTiffFullpath);

        // 
        string testMode_extractTiffFromPJLFullFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + " ①pmストリームから抽出直後.tif");

        //
        string testMode_afterChangeRezolutionFullFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + " ②解像度変更直後.png");

        //
        string testMode_afterBinaryImageFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + " ③2値化処理直後.png");

        string testMode_afterChekPaperSizeFailerFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + " ④用紙ｻｲｽﾞ判定失敗直後.png");

        string testMode_afterOffsetFullFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + " ⑤オフセット処置直後.png");

        //
        string testMode_afterRotaingFullFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + " ⑥回転処置直後.png");

        System.Drawing.Image ImageTiff;
        currentPaperSize = CommonPaperSize.Custom;

        bool ans = ConvertPMfileToImage(importPrinterOutputFullpath, out ImageTiff, LogWrite);

        if (ans == false)
        {
            if (LogWrite != null) LogWrite($"※GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) エラー:ConvertPMfileToImage()の結果がfalseです");

            return false;
        }

        if (testfileSaveMode)
            ImageTiff.Save(testMode_extractTiffFromPJLFullFileName); // pmファイルからTiffデータを取出し直後を保存

        float HorizontalResolution = ImageTiff.HorizontalResolution;
        float VerticalResolution = ImageTiff.VerticalResolution;

        if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName}  HorizontalResolution={HorizontalResolution},  VerticalResolution={VerticalResolution} ");
        if ((VerticalResolution > outputDpi) || (HorizontalResolution > outputDpi))
        {
            if (LogWrite != null) LogWrite($"■GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName}  解像度が{outputDpi}dpiを超えるｲﾒｰｼﾞは対応していません。{outputDpi}DPIにリサイズします");

            // 解像度確認と調整
            ImageTiff = SasaLib.ImageUtil.ChangeResolution((Bitmap)ImageTiff, outputDpi, outputDpi);


            if (ImageTiff != null)
            {
                if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName}  {outputDpi}DPIにリサイズ成功");

                if (testfileSaveMode)
                    ImageTiff.Save(testMode_afterChangeRezolutionFullFileName); // 解像度変更直後に保存
            }
            else
            {
                if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName}  {outputDpi}DPIにリサイズ失敗しました");
                return false;
            }
        }

        if (ImageTiff.PixelFormat != PixelFormat.Format1bppIndexed) //イメージのフォーマットを1bppIndexed 以外かをﾁｪｯｸ
        {
            // 【※】イメージのフォーマットを1bppIndexedへ変換します（2値化）
            ImageTiff = ImageUtil.HispeedImageBinarizer((System.Drawing.Bitmap)ImageTiff);
            ((Bitmap)ImageTiff).SetResolution(outputDpi, outputDpi);

            if (testfileSaveMode)
                ImageTiff.Save(testMode_afterBinaryImageFileName); // 2値化直後に保存
        }



        // ImageTiff 大きさをmm単位で取得
        var size = ImageUtil.GetPaperSizeMillimeter(ImageTiff);

        // Widht(mm), Height(mm)から 標準用紙サイズを判定
        currentPaperSize = PaperCheck.GetJISpaperSize(size, 5);
        if (currentPaperSize == CommonPaperSize.Custom)
        {
            if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName} 用紙サイズの判定に失敗しました。");

            MessageBox.Show($"用紙サイズの判定に失敗しました。");

            if (testfileSaveMode)
                ImageTiff.Save(testMode_afterChekPaperSizeFailerFileName); // 用紙サイズの判定の失敗直後に保存

            return false;
        }
        if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName} GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) Imageオブジェクトから用紙サイズの推定に成功しました currentPaperSize{currentPaperSize}");


        if (ImagePositonConfig.Config != null)
        {
            // 設定ファイルから用紙サイズ別のオフセット値を取得
            System.Drawing.Point offset_mm = ImagePositonConfig.Config.GetOffset(currentPaperSize);

            if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName}イメージポジションオフセット設定ファイル から用意サイズ別の位置オフセット情報を取得しました");
            if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName}  イメージポジションオフセット設定ファイル の Version :{ImagePositonConfig.Config.VERSION}");

            if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName} イメージの大きさと向き イメージサイズ(mm換算)：W={size.Width},H={size.Height} 用紙サイズ判定：{currentPaperSize.ToString()} オフセット X={offset_mm.X},Y={offset_mm.Y}");

            // イメージのオフセット移動開始。
            ImageTiff = ImageUtil.Move1bppImageMilli(ImageTiff, offset_mm.X, offset_mm.Y);
            if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName} イメージのオフセット移動を行いました");
            if (testfileSaveMode)
                ImageTiff.Save(testMode_afterOffsetFullFileName); // イメージオフセット直後に保存

            // イメージのローテ―ト開始
            RotateFlipType ImageRotation = ImagePositonConfig.Config.GetImageRotation(currentPaperSize);
            ImageTiff.RotateFlip(ImageRotation);
            if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName} RotateFlip = {ImageRotation} を実行しました");
            if (testfileSaveMode)
                ImageTiff.Save(testMode_afterRotaingFullFileName); // イメージ回転直後に保存
        }

        // 【※】メモリストリームを用意し、TIFF CCITT4圧縮のストリームデータへ変換します。
        MemoryStream tiffStream = new MemoryStream();
        ImageUtil.ImageToTIFF1bppCCITT4Stream(ImageTiff, tiffStream);
        if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName} GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) イメージをCCITT4ストリームへ変換しました");

        // TIFFメモリストリームをファイルに保存
        StreamExtensions.StreamToFile(tiffStream, exportTiffFullpath);
        if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName} GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) TIFFメモリストリームをファイルに保存しました");

        // 後始末
        ImageTiff.Dispose();
        tiffStream.Dispose();

        // PMファイルを削除
        SasaLib.FileFolder.RemoveFile(importPrinterOutputFullpath);


        if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) {AssemblyInternalName} GenerateTIFFdrawing.ConvertPMfileToTIFFfile(..) 強制ガベージコレクションを指示しました");
        System.GC.Collect(); // アクセス不可能なオブジェクトを除去
        System.GC.WaitForPendingFinalizers(); // ファイナライゼーションが終わるまでスレッド待機
        System.GC.Collect(); // ファイナライズされたばかりのオブジェクトに関連するメモリを開放

        return true;
    }

    /// <summary>
    /// 指定した印刷出力ファイル(PJL言語、TIFFデータ内包)からイメージを取得し System.Drawing.Imageに変換します。
    /// </summary>
    /// <param name="importPrinterOutputFullpath">プリンタ出力ファイル PM</param>
    /// <param name="image">変換後に生成されるSystem.Drawing.Image</param>
    /// <param name="LogWrite">Console(stirng msg)形式のメソッド名へログ出力をデリゲートします</param>
    /// <returns></returns>
    public bool ConvertPMfileToImage(string importPrinterOutputFullpath, out System.Drawing.Image image, SasaLibDelegateWriteLine LogWrite = null)
    {
        if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToImage(..){AssemblyInternalName}  プリンタドライバからの出力ファイルは {importPrinterOutputFullpath} とします");

        image = null;

        // PMファイルを読み込むメモリストリームの宣言とPMファイルの読み込み
        System.IO.MemoryStream PJLdataStream = SasaLib.StreamExtensions.StreamFromFile(importPrinterOutputFullpath);
        if (PJLdataStream == null)
        {
            if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToImage(..){AssemblyInternalName} 印刷用プロットファイル：{importPrinterOutputFullpath} の読み込みに失敗");

            MessageBox.Show($"{AssemblyInternalName} 印刷用プロットファイル：{importPrinterOutputFullpath} の読み込みに失敗");

            return false;
        }

        // TIFF用のメモリストリームを宣言
        System.IO.Stream TIFFnativeStream;

        // メモリストリーム PJLdataStream からTIFFファイル情報を TIFFnativeStream へ切り出す
        bool ansTiffPrintExecute = GetImageStreamFromPJLstream(PJLdataStream, out TIFFnativeStream, LogWrite);
        if (ansTiffPrintExecute == false)
        {
            if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToImage(..){AssemblyInternalName} 印刷用プロットファイル：{importPrinterOutputFullpath} からTIFFデータの切り出しに失敗");

            MessageBox.Show($"{AssemblyInternalName} 印刷用プロットファイル：{importPrinterOutputFullpath} からTIFFデータの切り出しに失敗");

            return false;
        }
        if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ExtractPJLstreamToTIFFfile(..) メモリストリーム上の印刷データ(PJL)からTIFFデータの取り出しに成功しました");
        PJLdataStream.Dispose();

        image = new Bitmap(TIFFnativeStream);

        if (image == null)
        {
            if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToImage(..){AssemblyInternalName}  メモリストリームからImageオブジェクトのデータの切り出しに失敗");

            MessageBox.Show($"{AssemblyInternalName} メモリストリームからImageオブジェクトのデータの切り出しに失敗");
            return false;
        }
        if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToImage(..){AssemblyInternalName}  メモリストリーム上のTIFFデータからImageオブジェクトの生成に成功しました");


        // PMファイルを削除
        SasaLib.FileFolder.RemoveFile(importPrinterOutputFullpath);


        if (LogWrite != null) LogWrite($"GenerateTIFFdrawing.ConvertPMfileToImage(..){AssemblyInternalName}  強制ガベージコレクションを指示しました");
        System.GC.Collect(); // アクセス不可能なオブジェクトを除去
        System.GC.WaitForPendingFinalizers(); // ファイナライゼーションが終わるまでスレッド待機
        System.GC.Collect(); // ファイナライズされたばかりのオブジェクトに関連するメモリを開放

        return true;
    }


    /// <summary>
    /// ■メモリストリーム上のプリンタドライバからの出力(*.pm)(TIFFデータ内包PJL言語) からTIFFイメージを抜き出してCompressionCCITT4 TIFFファイル化.
    /// AutoCAD側が使用
    /// </summary>
    /// <param name="msPJLrawdata">メモリストリーム上のプリンタドライバからの出力(*.pm)(TIFFデータ内包PJL言語)</param>
    /// <param name="saveDirectory">TIFFイメージを出力するフォルダ</param>
    /// <param name="saveFilenameWithoutExtension">出力するファイル名（拡張子なし。最終出力時に .TIF が付与されます）</param>
    /// <param name="outputDpi">出力するTIFFイメージの解像度</param>
    /// <param name="rotateFlipType">イメージの向き</param>
    /// <param name="LogWrite">デバッグ用ログ出力デリゲート</param>
    /// <param name="testfileSaveMode">true の場合 出力フォルダに処理中のイメージを書き出し</param>
    /// <param name="LogWrite"></param>
    /// <returns></returns>
    public bool ConvertPMstreamToTIFFfile(MemoryStream msPJLrawdata, string saveDirectory, string saveFilenameWithoutExtension,
            float outputDpi = 400f,
            System.Drawing.RotateFlipType rotateFlipType = System.Drawing.RotateFlipType.RotateNoneFlipNone,
            bool testfileSaveMode = false,
            SasaLibDelegateWriteLine LogWrite = null
           )
    {
        if (LogWrite == null)
            LogWrite = Console.WriteLine;


        // 最終保存フルファイル名
        //string lastSaveTiffFileName = System.IO.Path.Combine(saveDirectory, System.IO.Path.ChangeExtension(saveFilenameWithoutExtension, "TIF"));
        string lastSaveTiffFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + @".TIF");

        if (DebugLevel > 0)
            LogWrite($"ConvertPMstreamToTIFFfile(...)開始。出力先:{lastSaveTiffFileName},  RotateFlipType ={rotateFlipType}");

        // TIFF用のメモリストリームを宣言
        System.IO.Stream TIFFnativeStream;

        // メモリストリーム msPJLrawdata からTIFFファイル情報を TIFFnativeStream へ切り出す
        bool ansTiffPrintExecute = GetImageStreamFromPJLstream(msPJLrawdata, out TIFFnativeStream, LogWrite);
        if (ansTiffPrintExecute == false)
        {
            LogWrite($"PMメモリストリームからTIFFイメージ切り出し失敗");
            return false;
        }

        if (DebugLevel > 0)
            LogWrite($"PMメモリストリームからTIFFイメージ切り出し成功");

        msPJLrawdata.Dispose();

        System.Drawing.Image ImageTiff = new Bitmap(TIFFnativeStream);

        if (ImageTiff == null)
        {
            if (LogWrite != null) LogWrite($"{AssemblyInternalName} GenerateTIFFdrawing.ExtractPJLstreamToTIFFfile(..) メモリストリームからImageオブジェクトのデータの切り出しに失敗");
            return false;
        }

        if (DebugLevel > 0)
            if (LogWrite != null) LogWrite($"{AssemblyInternalName} GenerateTIFFdrawing.ExtractPJLstreamToTIFFfile(..) メモリストリーム上のTIFFデータからImageオブジェクトの生成に成功しました");

        int testNumber = 0;

        if (testfileSaveMode)
        {
            string testMode_extractTiffFromPJLFullFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + $"【{++testNumber}】pmストリームから抽出直後.tif");

            ImageTiff.Save(testMode_extractTiffFromPJLFullFileName); // プリントファイルpmからTIFFデータ取出し直後を保存
        }

        float HorizontalResolution = ImageTiff.HorizontalResolution;
        float VerticalResolution = ImageTiff.VerticalResolution;

        if (DebugLevel > 0)
            if (LogWrite != null) LogWrite($"HorizontalResolution={HorizontalResolution},  VerticalResolution={VerticalResolution} ");

        if ((VerticalResolution > outputDpi) || (HorizontalResolution > outputDpi))
        {
            if (LogWrite != null) LogWrite($"■ﾌﾟﾘﾝﾀ出力ﾌｧｲﾙの解像度は{VerticalResolution}dpi です。{outputDpi}DPIにリサイズします");

            // 解像度確認と調整
            System.Drawing.Drawing2D.InterpolationMode interpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            ImageTiff = SasaLib.ImageUtil.ChangeResolution((Bitmap)ImageTiff, outputDpi, outputDpi, interpolationMode);

            if (ImageTiff == null)
            {
                if (LogWrite != null) LogWrite($"{outputDpi}DPIにリサイズ失敗しました");
                return false;
            }

            if (testfileSaveMode)
            {
                string testMode_afterChangeRezolutionFullFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + $"【{++testNumber}】解像度変更直後.png");

                ImageTiff.Save(testMode_afterChangeRezolutionFullFileName); // 解像度が400dpi以外の場合は解像度変更直後に保存(PNG)
            }
        }

        if (ImageTiff.PixelFormat != PixelFormat.Format1bppIndexed) //イメージのフォーマットを1bppIndexed 以外かをﾁｪｯｸ
        {
            if (DebugLevel > 0)
                if (LogWrite != null) LogWrite($"イメージは {ImageTiff.PixelFormat} です。２値化を試みます");

            // 【※】イメージのフォーマットを1bppIndexedへ変換します（2値化）
            ImageTiff = ImageUtil.HispeedImageBinarizer((System.Drawing.Bitmap)ImageTiff);
            ((Bitmap)ImageTiff).SetResolution(outputDpi, outputDpi);

            if (ImageTiff == null)
            {
                if (LogWrite != null) LogWrite($"2値化に失敗しました");
                return false;
            }

            if (testfileSaveMode)
            {
                string testMode_afterBinaryImageFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + $"【{++testNumber}】2値化処理直後.png");

                ImageTiff.Save(testMode_afterBinaryImageFileName);
            }
        }

        /// 
        if (DebugLevel > 0)
            LogWrite($"msPJLrawdata.Dispose()を実行しました。次にイメージの回転と保存を行います");

        //////////////////////////////////////////////////////////////
        if (ImageTiff != null)
        {
            if (DebugLevel > 0)
                LogWrite($"ConvertPMstreamToTIFFfile(...) Pixcel Format = {ImageTiff.PixelFormat.ToString()}");
            ImageTiff.RotateFlip(rotateFlipType);
            if (DebugLevel > 0)
                LogWrite($"ConvertPMstreamToTIFFfile(...) RotateFlip = {rotateFlipType} に成功しました");

            if (testfileSaveMode)
            {
                string testMode_afterRotaingFullFileName = System.IO.Path.Combine(saveDirectory, saveFilenameWithoutExtension + $"【{++testNumber}】回転処置直後.png");

                ImageTiff.Save(testMode_afterRotaingFullFileName);// イメージ回転適用直後に保存(PNG)
            }

            // Tiffファイルの保存
            try
            {
                string mimeType = "image/tiff";
                System.Drawing.Imaging.EncoderValue EncoderValue = System.Drawing.Imaging.EncoderValue.CompressionCCITT4;

                ImageUtil.SaveImageToFile(ImageTiff, mimeType, EncoderValue, lastSaveTiffFileName);

                if (DebugLevel > 0)
                    LogWrite($"ConvertPMstreamToTIFFfile(...) ImageUtil.SaveImageToFile(...)  {lastSaveTiffFileName} を書き込みました");

            }
            catch (Exception ex)
            {
                LogWrite($"ConvertPMstreamToTIFFfile(...) ImageUtil.SaveImageToFile(...)  {lastSaveTiffFileName} の書き出しに失敗しました{ex.Message}");
                ImageTiff.Dispose();
                LogWrite($"ConvertPMstreamToTIFFfile(...) Img.Dispose()しました");
                return false;
            }

            ImageTiff.Dispose();
            if (DebugLevel > 0)
                LogWrite($"ConvertPMstreamToTIFFfile(...) Img.Dispose()しました。変換・書き出し成功です。メソッドを終了します");
            return true;
        }
        return false;

    }



    /// <summary>
    /// MemoryStreamの PJLデータからTIFFデータを（複数ある場合は最初のデータを）Streamにて取得
    /// </summary>
    /// <param name="msPJLrawdata"></param>
    /// <param name="tiffStream1st"></param>
    /// <returns></returns>
    private bool GetImageStreamFromPJLstream(MemoryStream msPJLrawdata, out Stream tiffStream1st, SasaLibDelegateWriteLine LogWrite = null)
    {
        // PJLdecodeオブジェクトを生成
        SasaLib.PJL.PJLdecode pJLdecode = new SasaLib.PJL.PJLdecode(msPJLrawdata);

        // PJLデータか検証後実行
        if (pJLdecode.IsPJL)
        {
            tiffStream1st = pJLdecode.imageFileDataStreams[0];

            if (DebugLevel > 0)
                if (LogWrite != null) LogWrite($"{AssemblyInternalName} GenerateTIFFdrawing.ExtractPJLstreamToTIFFfile(..) 終了");

            return true;
        }
        else
        {
            if (LogWrite != null) LogWrite($"{AssemblyInternalName} GenerateTIFFdrawing.ExtractPJLstreamToTIFFfile(..)　PJLではない");
            pJLdecode.Dispose();
            tiffStream1st = null;
            return false;
        }
    }

}
