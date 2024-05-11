using MailNotice;
using SasaLib;
using SharedClassLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ToyoMcMfg.Staging.RemoteObjects;

namespace ToyoStageService
{
    /// <summary>
    /// 個別のプリンタステータスを保持するクラス
    /// </summary>
    public class PrinterStatus
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
        /// PrinterInfoクラスから
        /// </summary>
        public string PrinterName;

        /// <summary>
        /// PrinterInfoクラスから
        /// </summary>
        public int NumberOfQueuesToSendErrors;

        /// <summary>
        /// 印刷キューに並んでいるジョブの合計数を取得します。
        /// </summary>
        public int NumberOfJobs;

        /// <summary>
        /// 印刷キューが一時停止されているかどうかを示す値を取得します。
        /// </summary>
        public bool IsPaused;

        /// <summary>
        /// プリンターがオフラインであるかどうかを示す値を取得します。
        /// </summary>
        public bool IsOffline;

        /// <summary>
        /// プリンターやデバイスがエラー状態になっているかどうかを示す値を取得します。
        /// </summary>
        public bool IsInError;

        /// <summary>
        /// プリンターのドアが開いているかどうかを示す値を取得します。
        /// </summary>
        public bool IsDoorOpened;

        /// <summary>
        /// プリンターが使用可能かどうかを示す値を取得します。
        /// </summary>
        public bool IsNotAvailable;

        /// <summary>
        /// 現在のジョブに必要なサイズの用紙をプリンターに補充する必要があるかどうかを示す値を取得します。
        /// </summary>
        public bool IsOutOfPaper;

        /// <summary>
        /// 現在の印刷ジョブで、プリンターに手差しで給紙する必要があるかどうかを示す値を取得します
        /// </summary>
        public bool IsManualFeedRequired;

        /// <summary>
        /// ジョブが印刷中かどうかを示す値を取得します。
        /// </summary>
        public bool IsPrinting;

        /// <summary>
        /// プリンターが印刷ジョブを処理しているかどうかを示す値を取得します。
        /// </summary>
        public bool IsProcessing;

        /// <summary>
        /// プリンターがエラー状態になっているかどうかを示す値を取得します。
        /// </summary>
        public bool IsServerUnknown;

        /// <summary>
        /// プリンターが人の介入を必要とするかどうかを示す値を取得します。
        /// </summary>
        public bool NeedUserIntervention;

        /// <summary>
        /// プリンターで紙詰まりが発生しているかどうかを示す値を取得します。
        /// </summary>
        public bool IsPaperJammed;

        /// <summary>
        /// プリンタステータスが変更された時のイベントハンドラ
        /// </summary>
        public event EventHandler<PrinterQueueStatus> StatusChange;

        public event EventHandler<PrinterQueueNumberOfJobs> QueueNumberOfJobsWhenOver;

        /// <summary>
        /// ｺﾝｽﾄﾗｸﾀ
        /// </summary>
        public PrinterStatus(string PrinterName, int numberOfQueuesToSendErrors)
        {
            this.PrinterName = PrinterName;
            NumberOfQueuesToSendErrors = numberOfQueuesToSendErrors;
            RemainJobCountUpdate(true);
        }


        /// <summary>
        /// 
        /// </summary>
        public void RemainJobCountUpdate(bool init = false)
        {
            try
            {
                if (NumberOfJobs != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).NumberOfJobs)
                {
                    NumberOfJobs = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).NumberOfJobs;
                    if (init == false) OnStatusChange(PrinterName, "IsOffline", IsOffline);
                    OnQueueNumberOfJobsOver(PrinterName, NumberOfJobs, NumberOfQueuesToSendErrors);
                }


                if (IsOffline != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsOffline)
                {
                    IsOffline = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsOffline;
                    if (init == false) OnStatusChange(PrinterName, "IsOffline", IsOffline);
                }

                if (IsNotAvailable != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsNotAvailable)
                {
                    IsNotAvailable = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsNotAvailable;
                    if (init == false) OnStatusChange(PrinterName, "IsNotAvailable", IsNotAvailable);
                }

                if (IsInError != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsInError)
                {
                    IsInError = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsInError;
                    if (init == false) OnStatusChange(PrinterName, "IsInError", IsInError);
                }

                if (IsDoorOpened != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsDoorOpened)
                {
                    IsDoorOpened = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsDoorOpened;
                    if (init == false) OnStatusChange(PrinterName, "IsDoorOpened", IsDoorOpened);
                }


                if (IsOutOfPaper != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsOutOfPaper)
                {
                    IsOutOfPaper = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsOutOfPaper;
                    if (init == false) OnStatusChange(PrinterName, "IsOutOfPaper", IsOutOfPaper);
                }

                if (IsManualFeedRequired != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsManualFeedRequired)
                {
                    IsManualFeedRequired = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsManualFeedRequired;
                    if (init == false) OnStatusChange(PrinterName, "IsManualFeedRequired", IsManualFeedRequired);
                }

                if (IsPaused != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsPaused)
                {
                    IsPaused = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsPaused;
                    if (init == false) OnStatusChange(PrinterName, "IsPaused", IsPaused);
                }

                if (IsPrinting != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsPrinting)
                {
                    IsPrinting = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsPrinting;
                    if (init == false) OnStatusChange(PrinterName, "IsPrinting", IsPrinting);
                }

                if (IsProcessing != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsProcessing)
                {
                    IsProcessing = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsProcessing;
                    if (init == false) OnStatusChange(PrinterName, "IsProcessing", IsProcessing);
                }

                if (IsServerUnknown != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsServerUnknown)
                {
                    IsServerUnknown = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).IsServerUnknown;
                    if (init == false) OnStatusChange(PrinterName, "IsServerUnknown", IsServerUnknown);
                }

                if (NeedUserIntervention != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).NeedUserIntervention)
                {
                    NeedUserIntervention = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).NeedUserIntervention;
                    if (init == false) OnStatusChange(PrinterName, "NeedUserIntervention", NeedUserIntervention);
                }

                if (IsPaperJammed != SasaLib.PrinterStatus.GetPrintQueue(PrinterName).NeedUserIntervention)
                {
                    IsPaperJammed = SasaLib.PrinterStatus.GetPrintQueue(PrinterName).NeedUserIntervention;
                    if (init == false) OnStatusChange(PrinterName, "IsPaperJammed", IsPaperJammed);
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERstatus", EventLogEntryType.Error, 4006,
                        $"ToyoStageService.PrinterStatus.RemainJobCountUpdate()にて 例外発生 {ex.Message} , {ex.InnerException} , Printer.PrinterName:\"{PrinterName}\"");
            }
        }

        void OnStatusChange(string printername, string statusName, object value)
        {
            DebugClass.ConsoleDebugOut(0, $"■PrinterStatusクラス プリンタ {printername} {statusName} が {value} に変化しました");

            if (StatusChange != null)
            {
                var args = new PrinterQueueStatus(printername, statusName, value);
                StatusChange(this, args);
            }
        }

        void OnQueueNumberOfJobsOver(string printername, int numberOfJobs, int threshold)
        {
            if (threshold == 0)
                threshold = 5;

            if (numberOfJobs > threshold)
            {
                DebugClass.ConsoleDebugOut(0, $"■PrinterStatusクラス プリンタ {printername} キューの残数が閾値 {threshold} を超えました。 現在 ; {numberOfJobs}");

                if (QueueNumberOfJobsWhenOver != null)
                {
                    var args = new PrinterQueueNumberOfJobs(printername, numberOfJobs, threshold);
                    QueueNumberOfJobsWhenOver(this, args);
                }
            }

        }

    }

    /// <summary>
    /// 設定上の全ローカルプリンターのステータスを保持するクラス
    /// </summary>
    public class PrinterStatuses
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
        /// StageServerのプリンタ設定の一覧
        /// </summary>
        List<PrinterInfo> _printerInfos;

        /// <summary>
        /// プリンタ毎のステータスのリスト
        /// </summary>
        List<PrinterStatus> _Statuses = new List<PrinterStatus>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="printerInfos"></param>
        public PrinterStatuses(List<PrinterInfo> printerInfos)
        {
            _Statuses.Clear();

            _printerInfos = printerInfos;

            foreach (var printerInfo in _printerInfos)
            {
                if (printerInfo.Ready == true)
                {
                    var printerStatus = new PrinterStatus(printerInfo.PrinterName, printerInfo.NumberOfQueuesToSendErrors);
                    printerStatus.StatusChange += OnStatusChange;
                    printerStatus.QueueNumberOfJobsWhenOver += OnPprintQueueNumberOfJobsOver;
                    _Statuses.Add(printerStatus);
                }
            }
        }

        /// <summary>
        /// プリンタステータスが変更された時のイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnStatusChange(Object sender, PrinterQueueStatus args)
        {
            if (StageServerConfig.Config.PrinterSystemErrorReportMailSend)
            {
                if (args.statusName == "IsOffline")
                    return; // 通知除外ステータス
                if (args.statusName == "IsOutOfPaper")
                    return; // 通知除外ステータス

                MailNotice.SendEmailFromPrinterStatusSystem("情報報告", $"プリンタ{args.printerName} ステータス変更を検知 {args.statusName} : {args.value} ", mailAccount, StageServerConfig.Config.EMAILFROMPRINTERSTATUSADDR);
            }
            ServerLog.Logging.LogRotateWriteLine($"プリンタ{args.printerName} ステータス変更を検知 {args.statusName} : {args.value} "); ServerLog.Logging.Flash();
        }

        public void OnPprintQueueNumberOfJobsOver(Object sender, PrinterQueueNumberOfJobs args)
        {
            if (StageServerConfig.Config.PrinterSystemErrorReportMailSend)
                MailNotice.SendEmailFromPrinterStatusSystem("エラー", $"プリンタ{args.printerName} 閾値 {args.threshold} を超えました 現在 {args.NumberOfJobs}",mailAccount, StageServerConfig.Config.EMAILFROMPRINTERSTATUSADDR);

            ServerLog.Logging.LogRotateWriteLine($"プリンタ{args.printerName} 閾値 {args.threshold} を超えました 現在 {args.NumberOfJobs}"); ServerLog.Logging.Flash();
        }

        /// <summary>
        /// ローカルプリンタキュー状態のアップデート
        /// </summary>
        public void Update()
        {
            foreach (var Status in _Statuses)
            {
                Status.RemainJobCountUpdate();
            }
        }

        /// <summary>
        /// ローカルプリンタキュー状態を文字列で取得
        /// </summary>
        /// <returns></returns>
        public string GetRemainJobInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"■全ローカルプリンタのジョブ残数の合計： {RemainJobCount()}\n");

            foreach (var Status in _Statuses)
            {
                sb.Append($"■プリンタ名:{Status.PrinterName}・ジョブ残数:{Status.NumberOfJobs}\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 全ローカルプリンタの残りジョブ数の合計を得る
        /// </summary>
        /// <returns></returns>
        public int RemainJobCount()
        {
            return _Statuses.Sum(v => v.NumberOfJobs);
        }

        /// <summary>
        /// プリンタで一番残りジョブ数が多い残ジョブ数を得る
        /// </summary>
        /// <returns></returns>
        public int MaxRemainJobCount()
        {
            var ans = _Statuses.Max(value => value.NumberOfJobs);
            return ans;
        }
    }


    /// <summary>
    /// イベントハンドラ用の引数
    /// </summary>
    public class PrinterQueueStatus
    {
        public string printerName { get; set; }
        public string statusName { get; set; }
        public object value { get; set; }

        public PrinterQueueStatus(string printerName, string statusName, object vaule)
        {
            this.printerName = printerName;
            this.statusName = statusName;
            this.value = vaule;
        }
    }

    /// <summary>
    /// イベントハンドラ用の引数
    /// </summary>
    public class PrinterQueueNumberOfJobs
    {
        public string printerName { get; set; }
        public int NumberOfJobs { get; set; }
        public int threshold { get; set; }

        public PrinterQueueNumberOfJobs(string printerName, int NumberOfJobs, int threshold)
        {
            this.printerName = printerName;
            this.NumberOfJobs = NumberOfJobs;
            this.threshold = threshold;
        }
    }

}
