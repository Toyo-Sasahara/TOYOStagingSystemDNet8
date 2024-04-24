using MailNotice;
using SasaLib;
using SasaLib.PrintConfig;
using SharedClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using ToyoMcMfg.Staging.RemoteObjects;

namespace ToyoStageService
{
    /// <summary>
    /// 印刷プロセス（メイン）
    /// </summary>
    public class PrintProcess
    {
        string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        public string PlotterSettingFiles { get; private set; }

        MailAccount mailAccount = new MailAccount(
            StageServerConfig.Config.EMAILSERVER,
            StageServerConfig.Config.EMAILSENDPORT,
            StageServerConfig.Config.EMAILNOTICE_SendToADDR,
            StageServerConfig.Config.SMTPAUTHUSER,
            StageServerConfig.Config.SMTPAUTHPASS_SasaLibEncryptionType,
            StageServerConfig.Config.SMTPAUTHPASS
        );

        public static List<PrinterInfo> PrinterInfos { get; private set; } = new List<PrinterInfo>(); 


        /// <summary>
        /// CommonPaperSizeから、StageSeverConfig.XMLに定義された印刷設定ファイル名を返す
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public string GetPlotterSettingFromPaperSize(CommonPaperSize cp)
        {
            switch (cp)
            {
                case CommonPaperSize.A0P:
                    return StageServerConfig.Config.PlotSetting_A0P;
                case CommonPaperSize.A0L:
                    return StageServerConfig.Config.PlotSetting_A0L;
                case CommonPaperSize.A1P:
                    return StageServerConfig.Config.PlotSetting_A1P;
                case CommonPaperSize.A1L:
                    return StageServerConfig.Config.PlotSetting_A1L;
                case CommonPaperSize.A2P:
                    return StageServerConfig.Config.PlotSetting_A2P;
                case CommonPaperSize.A2L:
                    return StageServerConfig.Config.PlotSetting_A2L;
                case CommonPaperSize.A3P:
                    return StageServerConfig.Config.PlotSetting_A3P;
                case CommonPaperSize.A3L:
                    return StageServerConfig.Config.PlotSetting_A3L;
                case CommonPaperSize.A4P:
                    return StageServerConfig.Config.PlotSetting_A4P;
                case CommonPaperSize.A4L:
                    return StageServerConfig.Config.PlotSetting_A4L;
                default:
                    return StageServerConfig.Config.PlotSetting_unknown;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PrintProcess()
        {
            PlotterSettingFiles = GetPrinterConfigFiles();
        }

        /// <summary>
        /// 印刷指示実行
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <param name="PlotterSetting"></param>
        /// <returns></returns>
        public bool PrintStart(string imageFilePath, EventsSummary evt, string PlotterSetting = "", string PrintDocumentName = "")
        {
            Image printImage = ImageUtil.CreateImageFromFile(imageFilePath);

            ImageUtil.PaperSizeCabinet pc = ImageUtil.GetPaparSize(printImage);

            if (string.IsNullOrWhiteSpace(PlotterSetting))
            {
                // PlotterSetting ="RICOH SPC830M-B.XML"など。指定なければ
                // StageSeverConfig.XML記載の、下記設定から用意サイズ別印刷設定ファイルを決定する
                // <PlotSetting_A0L> SII Teriostar LP-1030.XML </ PlotSetting_A0L>
                // <PlotSetting_A1P> SII Teriostar LP-1030.XML </ PlotSetting_A1P>
                //
                PlotterSetting = GetPlotterSettingFromPaperSize(pc.CommonPaperSize);
            }

            if (FileFolder.FileExists(System.IO.Path.Combine(StageServerConfig.Config.PrintersConfigFolder, PlotterSetting)) == true)               
            {
                evt.Add($"印刷指示実行={imageFilePath}\nPrintDocumenName={PrintDocumentName}\n用意サイズ認定={pc.PaperName}\n印刷設定ファイル={PlotterSetting}", true, true);

                bool IsError;

                PrinterService printService = new PrinterService(imageFilePath, System.IO.Path.Combine(StageServerConfig.Config.PrintersConfigFolder, PlotterSetting) , evt, out IsError);
                
                if (printService == null)
                {
                    evt.Add($"▲PrinterService（..） 初期化失敗 (PprintService == null) imageFilePath:{imageFilePath},{pc.PaperName}\n" +
                        $"PlotterSettingFiles\n{PlotterSettingFiles}"
                        , true, true);
                    MailNotice.SendEmailFromCommonLibrary("PrintService", "エラー報告", $"{AssemblyInternalName} PrintStart(...)印刷初期化失敗\n" +
                        $"imageFilePath={imageFilePath}\nPlotterSetting={PlotterSetting}\nimageFilePath={PrintDocumentName}\n" +
                        $"PlotterSettingFiles\n{PlotterSettingFiles}"
                        , mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
                    return false;
                }

                if (IsError)
                {
                    evt.Add($"▲PrinterService（..） 初期化失敗 (PrinterServiceクラス コンストラクタ初期化失敗 IsError={IsError}) imageFilePath:{imageFilePath},{pc.PaperName}\n" +
                        $"PlotterSettingFiles\n{PlotterSettingFiles}"
                        , true, true);
                    return false;
                }

                // プリント前イメージ保存オプションを調査
                if (string.IsNullOrWhiteSpace(StageServerConfig.Config.PrintDebug_BeforePrintingImageSaveFolder) == true)
                {
                    // プリント実行
                    bool prnResult = printService.PrintExecute(PrintDocumentName);

                    if (prnResult == true)
                    {
                        evt.Add($"PrintExecute(..) 実行完了={imageFilePath},{pc.PaperName}", true, true);
                    }
                    else
                    {
                        evt.Add($"▲PrintExecute(..) 実行失敗={imageFilePath},{pc.PaperName}", true, true);
                        return false;
                    }
                }
                else
                {
                    System.IO.Directory.CreateDirectory(StageServerConfig.Config.PrintDebug_BeforePrintingImageSaveFolder);
                    if (System.IO.Directory.Exists(StageServerConfig.Config.PrintDebug_BeforePrintingImageSaveFolder))
                    {
                        var outputImageFullfilename = System.IO.Path.Combine(StageServerConfig.Config.PrintDebug_BeforePrintingImageSaveFolder, PrintDocumentName + ".PNG");

                        bool prnResult = printService.PrintExecute(PrintDocumentName, outputImageFullfilename);

                        if (prnResult == true)
                        {
                            evt.Add($"デバッグモード・PrintExecute(..) 実行完了. ソースイメージ:{imageFilePath},{pc.PaperName}   {outputImageFullfilename} として保存  ", true, true);
                            ServerLog.Logging.LogRotateWriteLine($"デバッグモード・PrintExecute(..) 実行完了. ソースイメージ:{imageFilePath},{pc.PaperName}   {outputImageFullfilename} として保存  "); ServerLog.Logging.Flash();
                        }
                        else
                        {
                            evt.Add($"▲デバッグモード・PrintExecute(..) 実行失敗. ソースイメージ:{imageFilePath},{pc.PaperName}   {outputImageFullfilename} として保存  ", true, true);
                            ServerLog.Logging.LogRotateWriteLine($"▲デバッグモード・PrintExecute(..) 実行失敗. ソースイメージ:{imageFilePath},{pc.PaperName}   {outputImageFullfilename} として保存  "); ServerLog.Logging.Flash();
                        }
                    }
                    else
                    {
                        SasaLib.Eventlog.Log.WriteEntry(
                            "ToyoPRINTsystem", EventLogEntryType.Error, 7003,
                            $"{AssemblyInternalName} PrintStart(...)デバッグモード・印刷処理失敗　ソースイメージ={imageFilePath} 用意サイズ認定={pc.PaperName}\n" +
                            $"デバッグ書き出しフォルダ {StageServerConfig.Config.PrintDebug_BeforePrintingImageSaveFolder} が存在しません "
                            );
                        return false;
                    }
                }

                return true;
            }
            else
            {               
                SasaLib.Eventlog.Log.WriteEntry(
                    "ToyoPRINTsystem", EventLogEntryType.Error, 7003,
                    $"{AssemblyInternalName} PrintStart(...)印刷処理失敗　イメージ={imageFilePath} 用意サイズ認定={pc.PaperName}\n" +
                    $"\tプロッタ設定ファイル = \"{System.IO.Path.Combine(StageServerConfig.Config.PrintersConfigFolder, PlotterSetting)}\" が見つかりません\n" +
                    $"PlotterSettingFiles\n{PlotterSettingFiles}"
                    );
                return false;
            }
        }

        /// <summary>
        /// 準備されたプリンタ名一覧を取得
        /// </summary>
        public List<PrinterInfo> GetPrinterInfos(bool eventOut = false)
        {
            try
            {
                PrinterInfos.Clear();

                EventsSummary evt = new EventsSummary();

                evt.Add($"■■■{AssemblyInternalName} GetPrinterInfos()が呼びだされました", false, true);

                SharedClassLibrary.DebugClass.ConsoleDebugOut(8, $"■■■{AssemblyInternalName} GetPrinterInfos()が呼びだされた");
                //string[] PrinterConfigXmlFilES = SasaLib.FileFolder.FindFiles(StageServerConfig.Config.PrintersConfigFolder, "*.XML");
                string[] PrinterConfigXmlFilES = System.IO.Directory.GetFiles(StageServerConfig.Config.PrintersConfigFolder, @"*.XML");

                //PrinterInfos = new List<PrinterInfo>();

                foreach (var PrinterConfigXmlFile in PrinterConfigXmlFilES)
                {
                    evt.Add($"■■■{AssemblyInternalName} GetPrinterNames() {PrinterConfigXmlFile}をリロードします", false, true);
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(9, $"■■■{AssemblyInternalName} GetPrinterInfos() {PrinterConfigXmlFile}を調査します。");

                    PrinterConfigData PrnConfData = new PrinterConfigData(PrinterConfigXmlFile);
                    var PrinterName = PrnConfData.PrinterName;
                    var PrinterAliasName = PrnConfData.PrinterAliasName;
                    var PrinterShortCutName = PrnConfData.PrinterShortCutName;
                    var PrinterDescription = PrnConfData.PrinterDescription;
                    var PrinterXmlFileName = System.IO.Path.GetFileName(PrinterConfigXmlFile);
                    var Ready = PrnConfData.Ready;
                    var IsPrinterFailure = PrnConfData.IsPrinterFailure;
                    var getPrintQueue = SasaLib.PrinterStatus.GetPrintQueue(PrinterName, evt, false); // ドライバ情報を取得
                    var IsDriverPaused = getPrintQueue == null ? false : getPrintQueue.IsPaused; // ドライバ情報を取得

                    evt.Add(
                        $"■■■{AssemblyInternalName} GetPrinterNames()\n" +
                        $"PrinterName:{PrinterName}\n" +
                        $"PrinterAliasName:{PrinterAliasName}\n" +
                        $"PrinterShortCutName:{PrinterShortCutName}\n" +
                        $"PrinterDescription:{PrinterDescription}\n" +
                        $"PrinterXmlFileName:{PrinterXmlFileName}\n" +
                        $"Ready:{Ready}\n" +
                        $"IsPrinterFailure:{IsPrinterFailure}\n" +
                        $"IsDriverPaused:{IsDriverPaused}\n"
                        , false, true);

                    PrinterInfos.Add(new PrinterInfo
                    {
                        PrinterName = PrinterName,
                        PrinterAliasName = PrinterAliasName,
                        PrinterShortCutName = PrinterShortCutName,
                        PrinterDescription = PrinterDescription,
                        PrinterXmlFileName = PrinterXmlFileName,
                        Ready = Ready,
                        IsPrinterFailure = IsPrinterFailure,
                        IsDriverPaused = IsDriverPaused
                    }); ;
                }


                if (eventOut)
                    evt.SendEntry("ToyoPRINTsystem", EventLogEntryType.Information, 7003, "■プリンタ一覧が要求されました");

                return PrinterInfos;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry(
                   "ToyoPRINTsystem", EventLogEntryType.Error, 7003,
                   $"PrintProcess.GetPrinterInfos(..)にて 例外検知 {ex.Message} "
                   );
                return PrinterInfos;
            }
        }

        private string GetPrinterConfigFiles()
        {

            StringBuilder sb = new StringBuilder();

            if (System.IO.Directory.Exists(StageServerConfig.Config.PrintersConfigFolder))
            {
                Directory.GetFiles(StageServerConfig.Config.PrintersConfigFolder).ToList().ForEach(filePath =>
                {
                    sb.AppendLine(filePath);
                });

                var result = sb.ToString();

                return result;

            }
            else
            {
                SasaLib.Eventlog.Log.WriteEntry(
                    "ToyoPRINTsystem", EventLogEntryType.Error, 7003,
                        $"重大エラー: フォルダ \"{StageServerConfig.Config.PrintersConfigFolder}\"にアクセスできません"
                );

                return $"重大エラー:フォルダ \"{StageServerConfig.Config.PrintersConfigFolder}\"にアクセスできません";
            }
        }
    }

}
