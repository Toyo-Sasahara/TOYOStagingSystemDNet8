using SasaLib;
using System.Drawing.Printing;
using SasaLib.PrintConfig;
using System;
using System.Text;
using System.Diagnostics;
using MailNotice;
using System.Security.Principal;

namespace ToyoStageService
{
    /// <summary>
    /// 印刷を賄う基本クラス。１イメージにつき1インスタンスを作成
    /// </summary>
    public class PrinterService
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        MailAccount mailAccount = new MailAccount(
            StageServerConfig.Config.EMAILSERVER,
            StageServerConfig.Config.EMAILSENDPORT,
            StageServerConfig.Config.EMAILNOTICE_SendToADDR,
            StageServerConfig.Config.SMTPAUTHUSER,
            StageServerConfig.Config.SMTPAUTHPASS_SasaLibEncryptionType,
            StageServerConfig.Config.SMTPAUTHPASS
        );

        /// <summary>
        /// 印刷実行時のユーザーアカウント（ドメイン名）
        /// </summary>
        private string Print_Domain { get; set; } = null;

        /// <summary>
        /// 印刷実行時のユーザーアカウント（ユーユーザー名
        /// </summary>
        private string Print_User { get; set; } = null;

        /// <summary>
        /// 印刷実行時のユーザーのパスワード（平文）
        /// </summary>
        private string Print_UserPlanePassword { get; set; } = null;

        // プリンタは故障中ではないか
        private bool IsPrinterFailure = false;

        /// プリンタの準備ができたか
        private bool Ready = false;

        /// <summary>
        /// 実際に印刷するイメージファイル
        /// </summary>
        private string printingImageFullFilename;

        /// <summary>
        /// 印刷時にロックファイルとするフルファイル名
        /// </summary>
        private string printerLockFullFilename;

        /// <summary>
        ///  プリントファイルを保持
        /// </summary>
        private System.Drawing.Image currentImage;


        /// <summary>
        /// 標準用紙サイズ
        /// </summary>
        private CommonPaperSize currentPaperSize;

        /// <summary>
        /// 印刷に使うプリンタノコンフィグを保持
        /// </summary>
        PrinterConfigData curPrnConfData;

        /// <summary>
        /// 一括イベントビューアー処理のためのオブジェクト
        /// </summary>
        private EventsSummary evt;

        /// <summary>
        /// ★コンストラクタ。①TIFFファイル名と、出力先プリンタの設定ファイルを受け取る
        /// 
        /// </summary>
        /// <param name="PrintingImageFullFilename"></param>
        public PrinterService(string PrintingImageFullFilename, string PrinterConfigXmlFile, EventsSummary evt, out bool IsError)
        {
            IsError = true;

            this.evt = evt;

            /// PrinterConfigFileに記述されたプリント設定をオブジェクトへ確保
            this.evt.Add($"PrinterService({PrintingImageFullFilename},{PrinterConfigXmlFile}) 実行開始しました", true, true);

            //ファイル名をクラス内フィールドへ退避
            printingImageFullFilename = PrintingImageFullFilename;
            printerLockFullFilename = FileFolder.ChangeExtension(this.printingImageFullFilename, "PRLCK");


            /// イメージをオブジェクトへ確保（ファイルをロックしない）
            currentImage = ImageUtil.CreateImageFromFile(PrintingImageFullFilename);
            // サイズ
            var W = currentImage.Width;
            var H = currentImage.Height;
            // 解像度
            var resW = currentImage.VerticalResolution;
            var resH = currentImage.HorizontalResolution;
            // ピクセルフォーマット
            var pixelFormat = currentImage.PixelFormat;

            // 解像度をもとに用紙サイズ(mm)を取得
            PM size = ImageUtil.GetPaperSizeMillimeter(currentImage);
            /// イメージから用紙サイズと向きを推察
            currentPaperSize = PaperCheck.GetJISpaperSize(size.Width, size.Height, 5);

            /// PrinterConfigFileに記述されたプリント設定をオブジェクトへ確保
            curPrnConfData = new PrinterConfigData(PrinterConfigXmlFile);

            Print_Domain = curPrnConfData.Print_Domain;
            Print_User = curPrnConfData.Print_User;
            Print_UserPlanePassword = curPrnConfData.Print_UserPlanePassword;


            IsPrinterFailure = curPrnConfData.IsPrinterFailure;
            if (IsPrinterFailure == false)
            {
                Ready = curPrnConfData.Ready;
                this.evt.Add($"PrinterService(..) curPrnConfData.Readは{curPrnConfData.Ready}です", true, true);

                string eventMsg;

                if (Ready)
                {
                    if (string.IsNullOrWhiteSpace(curPrnConfData.PrinterName) == false)
                    {
                        StringBuilder sb = new StringBuilder();

                        try
                        {
                            bool errFlag = false;
                            sb.Append($"印刷諸元\n");
                            sb.Append($"①対象ファイル：{PrintingImageFullFilename}\n");
                            sb.Append($"②印刷設定ファイル：{PrinterConfigXmlFile}\n");
                            sb.Append($"③イメージサイズ・解像度,深さ：W={W}, H={H}, DPI(W)={resW}, DPI(H)={resH}, ﾋﾟｸｾﾙﾌｫｰﾏｯﾄ：{pixelFormat}\n");
                            sb.Append($"④イメージサイズから選択されたCommonPaperSize：{currentPaperSize.ToString()}\n");
                            sb.Append($"⑤使用するプリンタドライバ：{curPrnConfData.PrinterName}\n");

                            try { sb.Append($"⑥プリンタに指示する用紙設定：{curPrnConfData.GetPaperSize(currentPaperSize).PaperName}\n"); }
                            catch (Exception ex)
                            {
                                sb.Append($"⑥プリンタに指示する用紙設定：エラーです\n(指示値:{curPrnConfData.GetPaperSize(currentPaperSize).PaperName})\n例外:{ex.Message}");
                                errFlag = true;
                            }

                            try
                            {
                                sb.Append($"⑦プリンタに指示する用紙向き（LandScape値）：{curPrnConfData.GetLandScape(currentPaperSize).ToString()}\n");
                            }
                            catch (Exception ex)
                            {
                                sb.Append($"⑦プリンタに指示する用紙向き：エラーです\n(指示値:{curPrnConfData.GetLandScape(currentPaperSize).ToString()}\n例外:{ex.Message}");
                                errFlag = true;
                            }
                            string souceName = null;
                            try
                            {
                                souceName = curPrnConfData.GetPaperSource(currentPaperSize).SourceName;
                                if (souceName != null)
                                {
                                    sb.Append($"⑧プリンターに指示する用紙供給元：{souceName}\n");
                                }
                                else
                                {
                                    sb.Append($"⑧プリンターに指示する用紙供給元：SourceNameがnullでした\n");
                                }
                            }
                            catch (Exception ex)
                            {
                                sb.Append($"⑧プリンタに指示する用紙供給元：エラーです(指示値:\"{souceName}\")\n例外:{ex.Message}"); errFlag = true;
                            }

                            sb.Append($"⑨リンタに指示する印刷オフセット：X={curPrnConfData.GetOffset(currentPaperSize).X.ToString()},Y={curPrnConfData.GetOffset(currentPaperSize).Y.ToString()}\n");
                            sb.Append($"⑩プリンタ設定の準備が完了しているか？：curPrnConfData.Ready = {curPrnConfData.Ready}");

                            eventMsg = sb.ToString();

                            if (errFlag)
                            {
                                if (StageServerConfig.Config.PrinterSystemErrorReportMailSend)
                                    MailNotice.SendEmailFromCommonLibrary("PrintService", "エラー", $"{AssemblyInternalName}\n印刷エラー\n{eventMsg}", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
                            }

                        }
                        catch (Exception ex)
                        {
                            eventMsg = sb.ToString();
                            SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERservice", EventLogEntryType.Error, 7002, $"{AssemblyInternalName} PrinterService(...)にて例外発生 {ex.Message}\n {eventMsg}\n");
                            if (StageServerConfig.Config.PrinterSystemErrorReportMailSend)
                                MailNotice.SendEmailFromCommonLibrary("PrintService", "エラー", $"{AssemblyInternalName}\n印刷エラー\n{eventMsg}", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
                        }

                        ///
                        this.evt.Add($"eventMsg={eventMsg}", true, true);

                        IsError = false;
                    }
                    else
                    {
                        SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERservice", EventLogEntryType.Error, 7002, $"{AssemblyInternalName} PrinterService(...) PrinterConfigData PrinterService.PrinterName が null または 空文字です");
                    }
                }
                else
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERservice", EventLogEntryType.Error, 7002, $"プリンタ設定ﾌｧｲﾙ(.XML)の確認が必要です\ncurrentPrnConfig.PrinterName={curPrnConfData.PrinterName}");
                    if (StageServerConfig.Config.PrinterSystemErrorReportMailSend)
                        MailNotice.SendEmailFromCommonLibrary("PrintService", "エラー", $"プリンタ設定ﾌｧｲﾙ(.XML)の確認が必要です\ncurrentPrnConfig.PrinterName={curPrnConfData.PrinterName}", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
                }
            }
            else
            {
                this.evt.Add($"プリンタ \"{curPrnConfData.PrinterName}\" は故障中です", true, true);
                ServerLog.Logging.LogRotateWriteLine($"▲印刷処理 プリンタ \"{curPrnConfData.PrinterName}\" は PrinterConfigData.IsPrinterFailure = true. 故障中です"); ServerLog.Logging.Flash();
            }
        }

        /// <summary>
        /// 印刷実行
        /// </summary>
        /// <param name="PrintDocumentName"></param>
        /// <param name="BeforePrintImageFullPath"></param>
        public bool PrintExecute(string PrintDocumentName = "", string BeforePrintImageFullPath = null)
        {
            evt.Add($"印刷指示開始：PrintDocumentName(option)={PrintDocumentName}\ncurrentPrnConfig.PrinterName={curPrnConfData.PrinterName}", true, true);

            if (curPrnConfData.configobj.IsPrinterFailure)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERservice", EventLogEntryType.Error, 7002, $"プリンタ {curPrnConfData.PrinterName} は現在故障中です IsPrinterFailure = {curPrnConfData.configobj.IsPrinterFailure} ");
                return false;
            }


            if (this.Ready == false)
            {
                evt.Add($"▼PrintService.PrintExecute(..)は PrinterConfigData.Ready がfalseのため何もしません.PrintDocumentName={PrintDocumentName} currentPrnConfig.PrinterName={curPrnConfData.PrinterName} ", true, true);
                if (StageServerConfig.Config.PrinterSystemErrorReportMailSend)
                    MailNotice.SendEmailFromCommonLibrary("PrintService.PrintExecute(..)", "情報", $"{AssemblyInternalName}  PrintExecute(..)は currentPrnConfig.Ready が false のため何もしません.プリンタ設定ﾌｧｲﾙの確認が必要です\nPrintDocumentName={PrintDocumentName}\ncurrentPrnConfig.PrinterName={curPrnConfData.PrinterName}", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
                return true;
            }

            /// 印刷ロックファイル作成
            FileFolder.Touch(printerLockFullFilename);
            evt.Add($"印刷ロックファイル作成 {printerLockFullFilename}", true, true);

            // プリンタドライバ名
            string printerName = curPrnConfData.PrinterName;

            // プリンタドライバ用 ペーパーサイズの設定
            PaperSize paperSize = curPrnConfData.GetPaperSize(currentPaperSize);

            // プリンタドライバに送るイメージの向き
            bool landScape = curPrnConfData.GetLandScape(currentPaperSize);

            // プリンタドライバに送る用紙供給元
            PaperSource paperSource = curPrnConfData.GetPaperSource(currentPaperSize);

            // 印刷時のオフセット情報
            var ofs = curPrnConfData.GetOffset(currentPaperSize);

            Printing sasaLibPrintingObj
                 = new Printing(currentImage, // 印刷イメージ
                    System.Drawing.Imaging.PixelFormat.Format1bppIndexed, // ビット深度変換値
                    3, // レンダリング方式
                    ofs.X, ofs.Y)
                 {
                     BeforePrintingImageSaveFilepath = BeforePrintImageFullPath
                 };

            sasaLibPrintingObj.DocumentName = PrintDocumentName;


            if (string.IsNullOrWhiteSpace(Print_Domain) || string.IsNullOrWhiteSpace(Print_User) || string.IsNullOrWhiteSpace(Print_UserPlanePassword))
            {
                //プリント開始
                sasaLibPrintingObj.PrintImage(printerName, paperSize, landScape, paperSource);
            }
            else
            {
                // TODO: 偽装

                //using (var i = new ImpersonatedUser(Print_Domain, Print_User, Print_UserPlanePassword))
                //{
                //    //プリント開始
                //    sasaLibPrintingObj.PrintImage(printerName, paperSize, landScape, paperSource);
                //}

                new WithFakeAccount(Print_Domain, Print_User, Print_UserPlanePassword, true, () =>
                {
                    //プリント開始
                    sasaLibPrintingObj.PrintImage(printerName, paperSize, landScape, paperSource);
                });

            }

            evt.Add($"印刷指示完了. currentPaperSize={currentPaperSize} offset=({ofs.X},{ofs.Y})\nprinterName={printerName} paperSize={paperSize} landScape={landScape} paperSource={paperSource}", true, true);

            currentImage.Dispose();

            if (FileFolder.RemoveFile(printerLockFullFilename) == true)
            {
                evt.Add($"印刷ロックファイル削除完了 {printerLockFullFilename}", true, true);
                return true;
            }
            else
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERservice", EventLogEntryType.Error, 7002, $"{AssemblyInternalName} PrintExecute(...)印刷指示失敗：ロックファイル PrinterFile={printingImageFullFilename}のリネームを失敗しました");
                if (StageServerConfig.Config.PrinterSystemErrorReportMailSend)
                    MailNotice.SendEmailFromCommonLibrary("PrintService.PrintExecute(..)", "エラー", "{AssemblyInternalName} PrintExecute(...)印刷指示失敗：ロックファイル PrinterFile={printerFile}のリネームを失敗しました", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
                return false;
            }

        }
    }
}
