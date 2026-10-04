using MailNotice;
using SasaLib;
using Serilog.Core;
using SharedClassLibrary;
using StreamCommandExecutorClient;
using StreamCommandExecutorServer;
using System.Diagnostics;
using System.Reflection;
using ToyoStageService;
using ToyoStageService.StreamBasedServer;

namespace ToyoSTAGINGSYSTEMwatchDNet8
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        /// <summary>
        /// このアセンブリの内部名
        /// </summary>
        protected static string? AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        MailAccount? mailAccount;


        /// <summary>
        /// サーバーログシステムオブジェクト
        /// </summary>
        public SasaLib.Logging? Logging;

        /// <summary>
        /// □サーバー停止を知らせるフラグ
        /// </summary>
        internal volatile bool serviceStop = false;


        /// <summary>
        /// このアセンブリのバージョンを得る
        /// </summary>
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        ///// <summary>
        ///// コミットプリンター一覧
        ///// </summary>
        //internal List<PrinterInfo> printerInfos;

        /// <summary>
        /// 
        /// </summary>
        public LocalPrinterWatcher? localPrinterWatcher;

        /// <summary>
        /// このサービスのバージョン文字列を得る
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            string VersionText = $" Assembly Version {AssemblyVersion} (SasaLib {SasaLibInfo.GetAssemblyVersion()})";
            return VersionText;
        }

        /// <summary>
        /// このアセンブリを製品バージョンとして共用Staticアセンブりへ退避
        /// </summary>
        private void SetServerVersion()
        {
            string MainAssembly = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string SasaLibAssemblyVer = SasaLibInfo.GetAssemblyVersion();
            GlovalValues.ServerVersion = $"ToyoSTAGINGSYSTEMwatch ver {MainAssembly}|SasaLib Ver {SasaLibAssemblyVer}";
        }

        /// <summary>
        /// エラーをイベントビューアに記録する間隔(sec)
        /// </summary>
        internal int EventRecodingRemainingSec = 60;

        /// <summary>
        /// エラーをメールを再送する間隔(sec)
        /// </summary>
        internal int MailOutgoingRemainingSec = 300;



        /// <summary>
        /// メモリ不足状態なら true,それ以外はfalse
        /// </summary>
        public bool LowMemoryState { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private float _availableMemory;

        /// <summary>
        /// サーバーの使用可能メモリ
        /// </summary>
        public float AvailableMemory
        {
            get { return _availableMemory; }
            private set
            {
                if (_availableMemory != value)
                    _availableMemory = value;

                if (_availableMemory < StageServerConfig.Config.TriggerAlertRemaingMemoryMegaByte && LowMemoryState == false)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoSTAGINGSYSTEMwatch", EventLogEntryType.Warning, 0001,
                        $"※サーバーの使用可能メモリが設定値 {StageServerConfig.Config.TriggerAlertRemaingMemoryMegaByte} 未満に達しました。現在 {value} MByte" +
                        $""
                    , false, true);

                    AvailableMemoryLessThanTrigger?.Invoke(this, value);

                    LowMemoryState = true;
                }

                if (_availableMemory >= StageServerConfig.Config.TriggerAlertRemaingMemoryMegaByte && LowMemoryState == true)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoSTAGINGSYSTEMwatch", EventLogEntryType.Information, 0001,
                        $"■サーバーの使用可能メモリが設定値 {StageServerConfig.Config.TriggerAlertRemaingMemoryMegaByte} 以上に回復しました。現在 {value} MByte" +
                        $""
                    , false, true);

                    AvailableMemoryAboveTrigger?.Invoke(this, value);

                    LowMemoryState = false;
                }

            }
        }

        /// <summary>
        /// メモリ残量が最初に閾値未満に到達した時に発火するイベントハンドラ
        /// </summary>
        public event EventHandler<float>? AvailableMemoryLessThanTrigger;

        /// <summary>
        /// メモリ残量が最初に閾値以上に到達した時に発火するイベントハンドラ
        /// </summary>
        public event EventHandler<float>? AvailableMemoryAboveTrigger;

        /// <summary>
        /// 設定ファイル読込（起動時のみ）
        /// </summary>
        /// <param name="WriteEvent"></param>
        internal void InitReadConfig(bool WriteEvent)
        {
            string ServiceStartupDateTime = DateTime.Now.ToString("(yyyy-MM-dd HH,mm,ss)");

            // メインコンフィグフォルダ
            string mainConfFolder = ConfigFilesFullPath.ConfigFolder;

            /// StageServerの設定ファイル[StageServerConfig.XML]をロード
            StageServerConfigWork.ReadStageServerConfig(ConfigFilesFullPath.STSConfigFileFullpath, WriteEvent);

            /// DataBaseの設定ファイル[StageServerDatabaseConfig.XML]をロード
            string STDBConfigFileFullpath = System.IO.Path.Combine(mainConfFolder, @"StageServerDatabaseConfig.XML");
            SasaLib.NumberingSupport.StageServerDatabaseConfigWork.PreparationConfigData(STDBConfigFileFullpath, false);

            /// 表題欄位置定義ファイルの  [TitleFieldConf.XML]をロード
            bool Result = TitleFieldConfigWork.ReadTitleFieldConfig(ConfigFilesFullPath.STTitleFieldConfigFullpath, WriteEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WriteEvent"></param>
        void RepeatReloadConfig()
        {
            // メインコンフィグフォルダ
            string mainConfFolder = ConfigFilesFullPath.ConfigFolder;

            /// DataBaseの設定ファイル[StageServerDatabaseConfig.XML]をロード
            SasaLib.NumberingSupport.StageServerDatabaseConfigWork.PreparationConfigData(ConfigFilesFullPath.STDBConfigFileFullpath, false);

            /// 表題欄位置定義ファイルの  [TitleFieldConf.XML]をロード
            bool Result = TitleFieldConfigWork.ReadTitleFieldConfig(ConfigFilesFullPath.STTitleFieldConfigFullpath, true);

        }

        private void ToyoSTAGINGSYSTEMwatch_AvailableMemoryLessThanTriggerd(object sender, float e)
        {
            ToyoStageService.MailNotice.SendAlertEmailFromService("図面登録承認サービス(SW)", "障害報告", $"●{Environment.MachineName} ※サーバーの使用可能メモリが設定値({StageServerConfig.Config.TriggerAlertRemaingMemoryMegaByte} MByte)以下になりました (現在 {e} MByte)"
                , mailAccount, StageServerConfig.Config.EMAILFROMSYSWATCHADDR);
        }

        private void ToyoSTAGINGSYSTEMwatch_AvailableMemoryAboveTriggerd(object sender, float e)
        {
            ToyoStageService.MailNotice.SendAlertEmailFromService("図面登録承認サービス(SW)", "復旧報告", $"●{Environment.MachineName} ■サーバーの使用可能メモリが設定値({StageServerConfig.Config.TriggerAlertRemaingMemoryMegaByte} MByte)以上に復帰しました (現在 {e} MByte)"
                , mailAccount, StageServerConfig.Config.EMAILFROMSYSWATCHADDR);

        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            // ========================================
            // 起動時に1回だけ行う処理
            // ========================================
            InitializeService();


            try
            {
                // ========================================
                // 各常駐タスクを開始
                // ========================================

                Task[] tasks =
                [
                    StreamBasedServerLoopAsync(stoppingToken),
                    SharedConfigLoopAsync(stoppingToken),
                    ReadConfigLoopAsync(stoppingToken),
                    ReadPrinterConfigLoopAsync(stoppingToken),
                    CheckPrinterQueueLoopAsync(stoppingToken),
                    WatchDocDRServiceLoopAsync(stoppingToken),
                    WatchDocDCServiceLoopAsync(stoppingToken),
                    CheckMemoryLoopAsync(stoppingToken),
                ];

                await Task.WhenAll(tasks);

            }
            catch when (stoppingToken.IsCancellationRequested)
            {
                // Windowsサービスの正常な停止要求
                logger.LogInformation("サービス停止要求を受信しました");
            }
            finally
            {
                // ========================================
                // 全常駐処理終了後の後始末
                // ========================================

                logger.LogInformation("常駐処理が終了しました");

                serviceStop = true;
                SasaLib.Eventlog.Log.WriteEntry("ToyoSYSTEMWATCHservice", EventLogEntryType.Information, 4006, $"●{Environment.MachineName} 図面承認・登録システム 【ローカルプリンタ監視サービス】 サービス終了イベントを受信\n" +
                    $"SERVERMODE は {StageServerConfig.Config.SERVERMODE} でした。\n"
                    );

                ToyoStageService.MailNotice.SendInfomationEmailFromService("図面登録承認サービス(SW)", "終了報告", $"●{Environment.MachineName} ■図面承認・登録システム 【ローカルプリンタ監視サービス】{GetVersion()}\n" +
                    $"サービス終了イベントを受信\n" +
                    $"SERVERMODE は {StageServerConfig.Config.SERVERMODE} でした。", mailAccount, StageServerConfig.Config.EMAILFROMSYSWATCHADDR);

                ServerLog.MainLogging.LogRotateWriteLine($"ToyoSYSTEMWATCHservice OnStop()", FlashSync: true);
                ServerLog.OtherLogging.LogRotateWriteLine($"ToyoSYSTEMWATCHservice OnStop()", FlashSync: true);
                ServerLog.PipeLogging.LogRotateWriteLine($"ToyoSYSTEMWATCHservice OnStop()", FlashSync: true);
                ServerLog.DebugLogging.LogRotateWriteLine($"ToyoSYSTEMWATCHservice OnStop()", FlashSync: true);

            }

        }


        private void InitializeService()
        {
            /// 設定ファイル初読込
            InitReadConfig(true);

            mailAccount = new MailAccount(
               StageServerConfig.Config.EMAILSERVER,
               StageServerConfig.Config.EMAILSENDPORT,
               StageServerConfig.Config.EMAILNOTICE_SendToADDR,
               StageServerConfig.Config.EMAILINFOMAITON_SendToADDR,
               StageServerConfig.Config.SMTPAUTHUSER,
               StageServerConfig.Config.SMTPAUTHPASS_SasaLibEncryptionType,
               StageServerConfig.Config.SMTPAUTHPASS
           );


            SetServerVersion();

            string methodname = "ToyoSYSTEMWATCHservice.OnStart()";

            /// サーバーログシステム初期化その１（ｼｽﾃﾑ監視用）
            ServerLog.MainLogging = new Logging(StageServerConfig.Config.SERVERLOGFOLDER, "SW-Main.log");
            /// サーバーログシステム初期化その２（その他）
            ServerLog.OtherLogging = new Logging(StageServerConfig.Config.SERVERLOGFOLDER, "SW-Other.log");
            ///  サーバーログシステム初期化その3（Pipeサーバー関連）
            ServerLog.PipeLogging = new Logging(StageServerConfig.Config.SERVERLOGFOLDER, "SW-Pipe.log");
            ///  サーバーログシステム初期化その４（デバッグ用）
            ServerLog.DebugLogging = new Logging(StageServerConfig.Config.SERVERLOGFOLDER, "SW-Debug.log");

            ServerLog.MainLogging.LogRotateWriteLine($"\n■サービススタート ToyoSYSTEMWATCHservice ({GetVersion()}) OnStart() SERVERMODE:{StageServerConfig.Config.SERVERMODE}", FlashSync: true);
            ServerLog.OtherLogging.LogRotateWriteLine($"\n■サービススタート ToyoSYSTEMWATCHservice ({GetVersion()}) OnStart() SERVERMODE:{StageServerConfig.Config.SERVERMODE}", FlashSync: true);
            ServerLog.PipeLogging.LogRotateWriteLine($"\n■サービススタート oyoSYSTEMWATCHservice ({GetVersion()}) OnStart() SERVERMODE:{StageServerConfig.Config.SERVERMODE}", FlashSync: true);
            ServerLog.DebugLogging.LogRotateWriteLine($"\n■サービススタート ToyoSYSTEMWATCHservice ({GetVersion()}) OnStart() SERVERMODE:{StageServerConfig.Config.SERVERMODE}", FlashSync: true);

            /// エラーをイベントビューアに記録する間隔(sec)
            EventRecodingRemainingSec = StageServerConfig.Config.RecordOfErrEvent_ServerFailerSendRemainingSec;

            /// エラーをメールを再送する間隔(sec)
            MailOutgoingRemainingSec = StageServerConfig.Config.EMAILNOTICE_ServerFailerSendRemainingSec;

            /// 共有設定項目の準備
            SharedConfigValue.mmapedFile = new SharedConfigValue();

            // コンソールデバッグ出力モード
            GlovalValues.ConsoleWriteLevel = StageServerConfig.Config.DebugWriteLevel_SYSTEMWATCHservice;

            DebugClass.ConsoleDebugOut(0, $"●{Environment.MachineName} 図面承認・登録システム 【ローカルプリンタ監視サービス】 {AssemblyInternalName} 起動開始します。\n" +
                $"コンソールログレベル：{GlovalValues.ConsoleWriteLevel}\n");

            SasaLib.Eventlog.Log.WriteEntry("ToyoSTAGINGSYSTEMwatch", EventLogEntryType.Information, 4006,
                $"●{Environment.MachineName} 図面承認・登録システム 【ローカルプリンタ監視サービス】 {AssemblyInternalName}  起動開始します。" +
                $"コンソールログレベル：{GlovalValues.ConsoleWriteLevel}\n" +
                $"プリンタキュー監視タスク 注意喚起スプール残 ：{StageServerConfig.Config.LocalPrinterWaitJobThreshold} 件以上\n" +
                $"Switch.System.Runtime.Serialization.UseNewMaxArraySize は {DotNetFrameworkAppContextSetSwitch.GetCurrentStatus("Switch.System.Runtime.Serialization.UseNewMaxArraySize")} です。" +
                $"{GetVersion()}\n" +
                $"SERVERMODE は {StageServerConfig.Config.SERVERMODE} です。"
                , false, true);
            ToyoStageService.MailNotice.SendInfomationEmailFromService("図面登録承認サービス(SW)", "起動報告", $"●{Environment.MachineName} ■図面承認・登録システム 【ローカルプリンタ監視サービス】{GetVersion()}\n" +
                $"{AssemblyInternalName}  起動開始します\n" +
                $"プリンタキュー監視タスク 注意喚起スプール残数は ：{StageServerConfig.Config.LocalPrinterWaitJobThreshold} 件以上です\n" +
                $"SERVERMODE は {StageServerConfig.Config.SERVERMODE} です。", mailAccount, StageServerConfig.Config.EMAILFROMSYSWATCHADDR);

            AvailableMemoryLessThanTrigger += ToyoSTAGINGSYSTEMwatch_AvailableMemoryLessThanTriggerd;
            AvailableMemoryAboveTrigger += ToyoSTAGINGSYSTEMwatch_AvailableMemoryAboveTriggerd;


        }

        /// <summary>
        /// ■ストリームベースのサーバーを複数起動するタスク
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>

        private async Task StreamBasedServerLoopAsync(CancellationToken stoppingToken)
        {
            // デバッガのための待機
            int waitSec = 5;
            int loopWaitSec = 5;

            logger.LogInformation($"メモリマップドファイルタスクの実行まで{waitSec}秒待ちます");
            await Task.Delay(TimeSpan.FromSeconds(waitSec), stoppingToken);

            try
            {
                // 抽象化サーバータスクを生成
                await StreamBasedServerLauncher.StartMultipleServersAsync(
                    serverCount: 3,
                    baseTcpPort: StageServerConfig.Config.NewStreamMode_SW_TcpPort,
                    pipeName: StageServerConfig.Config.NewStreamMode_SW_PipeName,
                    maxTcpConnections: 5,
                    maxPipeInstances: 5,
                    serverHandshakeStartMSG: StageServerConfig.Config.NewStreamMode_HandShakeStr,
                    startHandShakeReadWriteTimeOut: StageServerConfig.Config.ReadWriteHandShakeStreamStringTimeOut,
                    commandHandler: CommandExecutor.ExecuteAsync,
                    WriteLine: LogRoateWriteLine,
                    verbose: StageServerConfig.Config.NewStreamMode_Verbose
                );

                void LogRoateWriteLine(string msg)
                {
                    ServerLog.PipeLogging.LogRotateWriteLine(msg, ConsoleWriteLineSwitch: true, FlashSync: true);
                }


                ServerLog.PipeLogging.LogRotateWriteLine($"■StreamBasedServerLauncher.StartMultipleServersAsync() 実行完了");
            }
            catch (Exception ex)
            {
                ServerLog.PipeLogging.LogRotateWriteLine($"RMcommandReceiver.TaskRun()の実行で例外発生\n{ex.Message}");
            }
        }


        /// <summary>
        /// ■メモリマップドファイルの特定ステータス情報を繰り返し読み戻す (5 秒)
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task SharedConfigLoopAsync(CancellationToken stoppingToken)
        {
            // デバッガのための待機
            int waitSec = 5;
            int loopWaitSec = 5;

            logger.LogInformation($"メモリマップドファイルタスクの実行まで{waitSec}秒待ちます");
            await Task.Delay(TimeSpan.FromSeconds(waitSec), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                SharedConfigValue.mmapedFile.ReadToStaticValues(this);

                logger.LogInformation($" SharedConfigValue.mmapedFile.ReadToStaticValues(this) を実行しました {loopWaitSec} 秒ごとに実行されます");

                await Task.Delay(TimeSpan.FromSeconds(loopWaitSec), stoppingToken);
            }
        }

        /// <summary>
        /// ■一部の設定ファイルは繰り返し再読み込み (10 秒)
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task ReadConfigLoopAsync(CancellationToken stoppingToken)
        {
            // デバッガのための待機
            int waitSec = 10;

            // 一部の設定ファイルは再読み込み
            int loopWaitSec = 20;

            logger.LogInformation($"タスクの実行まで{waitSec}秒待ちます");
            await Task.Delay(TimeSpan.FromSeconds(waitSec), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {

                /// ■一部の設定ファイルは再読み込み
                RepeatReloadConfig();

                logger.LogInformation($" RepeatReloadConfig() を実行しました {loopWaitSec} 秒ごとに実行されます");

                await Task.Delay(TimeSpan.FromSeconds(loopWaitSec), stoppingToken);
            }
        }

        /// <summary>
        ///  ■コミット用プリンタの設定情報更新タスク
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task ReadPrinterConfigLoopAsync(CancellationToken stoppingToken)
        {
            PrinterInfo printerInfo = new PrinterInfo();


            // 起動時にプリンター設定をすべて読込 かつ 指定間隔で再読み込み
            PrintProcess printProcess = new PrintProcess();

            // デバッガのための待機
            int waitSec = 10;

            // 一部の設定ファイルは再読み込み
            int loopWaitSec = StageServerConfig.Config.ReloadPrinterConfig_IntervalSecond;

            logger.LogInformation($"タスクの実行まで{waitSec}秒待ちます");
            await Task.Delay(TimeSpan.FromSeconds(waitSec), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {

                printProcess.GetPrinterInfos(StageServerConfig.Config.RecordOfNormaEvents_ReloadPrinters);

                logger.LogInformation($"■List<PrintersInfo> printerInfos 更新しました Count= {printerInfo.GetPrinterInfos().Count} 次回 {StageServerConfig.Config.ReloadPrinterConfig_IntervalSecond * 1000}秒後");

                ServerLog.OtherLogging.LogRotateWriteLine($"■CreatePrinterInfosTaskメソッド: List<PrintersInfo> printerInfos 更新しました Count= {printerInfo.GetPrinterInfos().Count} 次回 {StageServerConfig.Config.ReloadPrinterConfig_IntervalSecond * 1000}秒後"); ServerLog.MainLogging.Flash();

                localPrinterWatcher = new LocalPrinterWatcher(printerInfo.GetPrinterInfos(), StageServerConfig.Config.LocalPrinterWaitJobThreshold);

                logger.LogInformation($" localPrinterWatcher を更新しました {loopWaitSec} 秒ごとに実行されます");

                await Task.Delay(TimeSpan.FromSeconds(loopWaitSec), stoppingToken);
            }
        }

        /// <summary>
        /// ■プリンタキュー監視タスク
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task CheckPrinterQueueLoopAsync(CancellationToken stoppingToken)
        {
            // デバッガのための待機
            int waitSec = 5;
            int loopWaitSec = 1;

            PrinterInfo printerInfo = new PrinterInfo();

            logger.LogInformation($"プリンタキュー監視タスクの実行まで{waitSec}秒待ちます");
            await Task.Delay(TimeSpan.FromSeconds(waitSec), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (localPrinterWatcher != null)
                        localPrinterWatcher.Update(printerInfo.GetPrinterInfos());
                    else
                    {
                        ServerLog.MainLogging.LogRotateWriteLine($"※printerStatusesHelper が初期化されていません"); ServerLog.MainLogging.Flash();
                    }
                }
                catch (Exception ex)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoSTAGINGSYSTEMwatch", EventLogEntryType.Error, 4006, $"※ローカルプリンタ―ステータス取得メソッドの実行中に例外検知{ex.Message}");
                    ServerLog.MainLogging.LogRotateWriteLine($"※ローカルプリンタ―ステータス取得メソッドの実行中に例外検知{ex.Message}"); ServerLog.MainLogging.Flash();
                }

                //logger.LogInformation($"プリンタキュー監視タスクを実行しました {loopWaitSec} 秒ごとに実行されます");

                await Task.Delay(TimeSpan.FromSeconds(loopWaitSec), stoppingToken);
            }
        }

        /// <summary>
        /// ■生存監視タスク(DR)
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task WatchDocDRServiceLoopAsync(CancellationToken stoppingToken)
        {
            // デバッガのための待機
            int waitSec = 5;
            int loopWaitSec = StageServerConfig.Config.WatchDocTimeSec;

            /// エラーをイベントビューアーに記録するまでの残りカウント
            int eventRecodingCountDown = 0;
            ///エラーをメール送信するまでの残りカウント
            int mailSendCountDown = 0;

            /// 復帰後に一度だけメールを送るためのフラグ
            bool RecoveryMailFlag = false;

            logger.LogInformation($"生存監視タスク(DR)の実行まで{waitSec}秒待ちます");
            await Task.Delay(TimeSpan.FromSeconds(waitSec), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await CMD06_ConnectTest_Client.ExecuteAsync(
                    serverInfo: new StreamCommandExecutorClient.ServerInfo(
                    serverName: "127.0.0.1",
                    tcpPort: StageServerConfig.Config.NewStreamMode_DR_TcpPort),
                    testMessage: "ToyoSTAGINGSYSTEMwatchから実行(ストリーム接続)",
                    WriteLine: Console.WriteLine
                );

                logger.LogInformation($" CMD06_ConnectTest_Client.ExecuteAsync 実行結果 result.Sucess=true result.Message = {result.Message}");

                if (result.Sucess)
                {
                    if (RecoveryMailFlag == true)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("ToyoSTAGINGSYSTEMwatch", EventLogEntryType.Information, 1000,
                            $"復旧報告\nToyoDRAWREGISTserviceとのストリーム通信が再開されました."
                            );
                        ToyoStageService.MailNotice.SendAlertEmailFromService("図面登録承認サービス(SW)", "復旧報告", $"●{Environment.MachineName} ■ToyoDRAWREGISTserviceとのストリーム通信が再開されました."
                            , mailAccount, StageServerConfig.Config.EMAILFROMCAPTURESYSADDR);
                        /// 最初の一回目を送信したのでフラグを落とす
                        RecoveryMailFlag = false;
                        /// 回復したらカウントを初期値へ戻す。
                        eventRecodingCountDown = 0;
                        /// 回復したらカウントを初期値へ戻す。
                        mailSendCountDown = 0;
                    }
                }
                else
                {
                    ServerLog.MainLogging.LogRotateWriteLine($"※ToyoDRAWREGISTserviceとのストリーム通信ができません。現カウンタ値：eventRecodingCountDown={eventRecodingCountDown}, mailSendCountDown={mailSendCountDown}");

                    //初っ端からカウンタをマイナスにすることにより最初に１トリガを発生
                    eventRecodingCountDown--;
                    mailSendCountDown--;

                    if (eventRecodingCountDown < 1)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("ToyoSTAGINGSYSTEMwatch", EventLogEntryType.Error, 1000,
                            $"障害報告\n" +
                            $"ToyoDRAWREGISTserviceとのストリーム通信の接続ができません。" +
                            $"チェック間隔 {StageServerConfig.Config.WatchDocTimeSec}sec.\n" +
                            $"イベント通知間隔{EventRecodingRemainingSec}sec , メール通知間隔{MailOutgoingRemainingSec} sec\n" +
                            $"現カウンタ値：eventRecodingCountDown={eventRecodingCountDown}, mailSendCountDown={mailSendCountDown}");
                        /// 次回の送信までのカウント値を設定。
                        eventRecodingCountDown = EventRecodingRemainingSec / StageServerConfig.Config.WatchDocTimeSec;
                    }

                    if (mailSendCountDown < 1)
                    {
                        ToyoStageService.MailNotice.SendAlertEmailFromService("図面登録承認サービス(SW)", "障害報告", $"●{Environment.MachineName} ※ToyoDRAWREGISTserviceとのとのストリーム通信が無反応!!。チェック間隔 {StageServerConfig.Config.WatchDocTimeSec}sec.\n" +
                            $"イベント通知間隔{EventRecodingRemainingSec}sec , メール通知間隔{MailOutgoingRemainingSec} sec"
                            , mailAccount, StageServerConfig.Config.EMAILFROMCAPTURESYSADDR);
                        /// 次回の送信までのカウント値を設定。
                        mailSendCountDown = this.MailOutgoingRemainingSec / StageServerConfig.Config.WatchDocTimeSec;
                        /// 回復したらメールを送れるようにフラグ立て
                        RecoveryMailFlag = true;
                    }
                }


                logger.LogInformation($" を実行しました {loopWaitSec} 秒ごとに実行されます");

                await Task.Delay(TimeSpan.FromSeconds(loopWaitSec), stoppingToken);
            }
        }

        /// <summary>
        /// ■生存監視タスク(DC)
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task WatchDocDCServiceLoopAsync(CancellationToken stoppingToken)
        {
            // デバッガのための待機
            int waitSec = 5;
            int loopWaitSec = StageServerConfig.Config.WatchDocTimeSec;

            /// エラーをイベントビューアーに記録するまでの残りカウント
            int eventRecodingCountDown = 0;
            ///エラーをメール送信するまでの残りカウント
            int mailSendCountDown = 0;

            /// 復帰後に一度だけメールを送るためのフラグ
            bool RecoveryMailFlag = false;

            logger.LogInformation($"生存監視タスク(DC)の実行まで{waitSec}秒待ちます");
            await Task.Delay(TimeSpan.FromSeconds(waitSec), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await CMD06_ConnectTest_Client.ExecuteAsync(
                    serverInfo: new StreamCommandExecutorClient.ServerInfo(
                    serverName: "127.0.0.1",
                    tcpPort: StageServerConfig.Config.NewStreamMode_DC_TcpPort),
                    testMessage: "ToyoSTAGINGSYSTEMwatchから実行(ストリーム接続)",
                    WriteLine: Console.WriteLine
                );

                logger.LogInformation($" CMD06_ConnectTest_Client.ExecuteAsync 実行結果 result.Sucess=true result.Message = {result.Message}");

                if (result.Sucess)
                {
                    if (RecoveryMailFlag == true)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("ToyoSTAGINGSYSTEMwatch", EventLogEntryType.Information, 1000,
                            $"復旧報告\nToyoDRAWCAPTUREserviceとのストリーム通信が再開されました."
                            );
                        ToyoStageService.MailNotice.SendAlertEmailFromService("図面登録承認サービス(SW)", "復旧報告", $"●{Environment.MachineName} ■ToyoDRAWCAPTUREserviceとのストリーム通信が再開されました."
                            , mailAccount, StageServerConfig.Config.EMAILFROMCAPTURESYSADDR);
                        /// 最初の一回目を送信したのでフラグを落とす
                        RecoveryMailFlag = false;
                        /// 回復したらカウントを初期値へ戻す。
                        eventRecodingCountDown = 0;
                        /// 回復したらカウントを初期値へ戻す。
                        mailSendCountDown = 0;
                    }
                }
                else
                {
                    ServerLog.MainLogging.LogRotateWriteLine($"※ToyoDRAWCAPTUREserviceとのストリーム通信ができません。現カウンタ値：eventRecodingCountDown={eventRecodingCountDown}, mailSendCountDown={mailSendCountDown}");

                    //初っ端からカウンタをマイナスにすることにより最初に１トリガを発生
                    eventRecodingCountDown--;
                    mailSendCountDown--;

                    if (eventRecodingCountDown < 1)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("ToyoSTAGINGSYSTEMwatch", EventLogEntryType.Error, 1000,
                            $"障害報告\n" +
                            $"ToyoDRAWCAPTUREserviceとのストリーム通信の接続ができません。" +
                            $"チェック間隔 {StageServerConfig.Config.WatchDocTimeSec}sec.\n" +
                            $"イベント通知間隔{EventRecodingRemainingSec}sec , メール通知間隔{MailOutgoingRemainingSec} sec\n" +
                            $"現カウンタ値：eventRecodingCountDown={eventRecodingCountDown}, mailSendCountDown={mailSendCountDown}");
                        /// 次回の送信までのカウント値を設定。
                        eventRecodingCountDown = EventRecodingRemainingSec / StageServerConfig.Config.WatchDocTimeSec;
                    }

                    if (mailSendCountDown < 1)
                    {
                        ToyoStageService.MailNotice.SendAlertEmailFromService("図面登録承認サービス(SW)", "障害報告", $"●{Environment.MachineName} ※ToyoDRAWCAPTUREserviceとのとのストリーム通信が無反応!!。チェック間隔 {StageServerConfig.Config.WatchDocTimeSec}sec.\n" +
                            $"イベント通知間隔{EventRecodingRemainingSec}sec , メール通知間隔{MailOutgoingRemainingSec} sec"
                            , mailAccount, StageServerConfig.Config.EMAILFROMCAPTURESYSADDR);
                        /// 次回の送信までのカウント値を設定。
                        mailSendCountDown = this.MailOutgoingRemainingSec / StageServerConfig.Config.WatchDocTimeSec;
                        /// 回復したらメールを送れるようにフラグ立て
                        RecoveryMailFlag = true;
                    }
                }


                logger.LogInformation($" を実行しました {loopWaitSec} 秒ごとに実行されます");

                await Task.Delay(TimeSpan.FromSeconds(loopWaitSec), stoppingToken);
            }
        }

        /// <summary>
        /// 使用可能メモリ監視タスク
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        private async Task CheckMemoryLoopAsync(CancellationToken stoppingToken)
        {
            // "Memory" パフォーマンスカウンタを指定して PerformanceCounter インスタンスを作成
            PerformanceCounter memoryCounter = new PerformanceCounter("Memory", "Available MBytes");


            // デバッガのための待機
            int waitSec = 5;
            int loopWaitSec = 1;

            logger.LogInformation($" 使用可能メモリ監視タスク実行まで{waitSec}秒待ちます");
            await Task.Delay(TimeSpan.FromSeconds(waitSec), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                // 利用可能なメモリーの量を取得
                AvailableMemory = memoryCounter.NextValue();


                logger.LogInformation($" 利用可能なメモリーの量 {AvailableMemory} MB,  {loopWaitSec} 秒ごとに実行されます");

                await Task.Delay(TimeSpan.FromSeconds(loopWaitSec), stoppingToken);
            }
        }

    }
}