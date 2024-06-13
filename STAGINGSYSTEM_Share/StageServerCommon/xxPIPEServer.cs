//using SasaLib.PIPE;
//using SasaLib;
//using SharedClassLibrary;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO.Pipes;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Drawing;
//using System.Security.AccessControl;
//using System.Security.Principal;
//using System.Windows.Forms;
//using System.Reflection;
//using Microsoft.Win32.SafeHandles;
//using System.Runtime.Versioning;

//namespace ToyoStageService
//{
//    /// <summary>
//    /// PIPEｻｰﾊﾞｰ構築ｸﾗｽ
//    /// </summary>
//    [SupportedOSPlatform("windows")]
//    public class PIPEServer
//    {
//        /// <summary>
//        /// パイプサーバータスク
//        /// </summary>
//        private static List<Task<int>> serverTasks;

//        readonly object serverTasks_LockHandler = new object();
//        readonly object activeCommanList_LockHandler = new object();

//        /// <summary>
//        /// 現在実行中のPIPEセッション 
//        /// </summary>
//        private static List<AcceptPipeCommand> activeSessionCommandList = new List<AcceptPipeCommand>();

//        /// <summary>
//        /// コマンド解析・実行メソッドへのデリゲート宣言
//        /// </summary>
//        /// <param name="command"></param>
//        /// <param name="pipeSrvStream"></param>
//        /// <param name="serverId"></param>
//        /// <param name="BUSYFLAG"></param>
//        /// <returns></returns>
//        public delegate bool delegate_CommandAnalysisAndExecute(int serverId, string objectID, int taskID, NamedPipeServerStream pipeSrvStream, string command);

//        /// <summary>
//        /// デリゲート先を保持
//        /// </summary>
//        private delegate_CommandAnalysisAndExecute commandAnalysisAndExecute;

//        /// <summary>
//        /// イベントビューアーのソース名
//        /// </summary>      
//        public string EventViewSourceName { get; private set; }

//        /// <summary>
//        /// ロギングメッセージでPIPEサーバを識別する識別名
//        /// </summary>
//        public string LogMsg_ServiceName { get; }

//        /// <summary>
//        /// PIPEサーバーのPIPE名
//        /// </summary>
//        public string PIPEName { get; }

//        /// <summary>
//        /// PIPEコマンド接続開始 ハンドシェイク文字列
//        /// </summary>
//        public string ServerHandshakeStartMSG { get; }

//        /// <summary>
//        /// IPEコマンド接続開始 ハンドシェイク文字列ストリームタイムアウト
//        /// </summary>
//        public int StartHandShakeReadWriteTimeOut { get; }

//        private int ReadStreamStringTimeOut { get; } = 20000;

//        /// <summary>
//        /// <summary>
//        /// このPIPEｻｰﾊﾞｰがハンドシェイク実行時にBUSYを返した数
//        /// </summary>
//        internal int PipeServer_BUSY_Count { get; private set; }

//        /// <summary>
//        /// 正常実行時のPIPEコマンドに関するログを記録するか否か
//        /// </summary>
//        public bool IsLoggingPIPE_NormalStatus { get; set; }

//        /// <summary>
//        /// コンストラクタ
//        /// </summary>
//        /// <param name="eventViewSourceName"></param>
//        /// <param name="pipeName"></param>
//        /// <param name="serverHandshakeStartMSG"></param>
//        /// <param name="startHandShakeReadWriteTimeOut"></param>
//        /// <param name="commandAnalysisAndExecute"></param>
//        public PIPEServer(string eventViewSourceName, string pipeName, string serverHandshakeStartMSG, string logMsg_ServiceName, int startHandShakeReadWriteTimeOut = 9000 , delegate_CommandAnalysisAndExecute commandAnalysisAndExecute = null)
//        {
//            var methodname = "PIPEServer.PIPEServer()";
            

//            ServerLog.PiperServerLogging.LogRotateWriteLine(
//                $"\n\nStart {methodname} pipeName:{pipeName} , startHandShakeReadWriteTimeOut:{startHandShakeReadWriteTimeOut}", FlashSync: true);

//            EventViewSourceName = eventViewSourceName;
//            LogMsg_ServiceName = logMsg_ServiceName;
//            PIPEName = pipeName;
//            ServerHandshakeStartMSG = serverHandshakeStartMSG;
//            StartHandShakeReadWriteTimeOut = startHandShakeReadWriteTimeOut;

//            if (commandAnalysisAndExecute == null)
//            {
//                ServerLog.PiperServerLogging.LogRotateWriteLine(
//                    $"commandAnalysisAndExecute == null のため デリゲート dummyCommandAnalysisAndExecute()を呼び出します"
//                    , FlashSync: true);
//                commandAnalysisAndExecute = dummyCommandAnalysisAndExecute;
//            }
//            else
//            {
//                ServerLog.PiperServerLogging.LogRotateWriteLine(
//                    $"{pipeName} PIPEコマンドアナライザー デリゲート commandAnalysisAndExecute()を呼び出します"
//                    , FlashSync: true);

//                this.commandAnalysisAndExecute = commandAnalysisAndExecute; // デリゲート先を呼び出す
//            }

//            ServerLog.PiperServerLogging.LogRotateWriteLine(
//                $"{methodname} 終了"
//                , FlashSync: true);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="func"></param>
//        //public void AddCommandAnalysisAndExecuteDelegate(delegate_CommandAnalysisAndExecute func)
//        //{
//        //    commandAnalysisAndExecute += func;

//        //}

//        /// <summary>
//        /// ■パイプサーバー生成および実行
//        /// </summary>
//        /// <param name="numberOfTasks"></param>
//        public void CreateTasks(int numberOfTasks, int checkPipeCommandRunningCount = 10)
//        {
//            var methodname = "PIPEServer.CreateTasks()";

//            if (numberOfTasks == 0)
//                numberOfTasks = 1;

//            if (checkPipeCommandRunningCount == 0)
//                checkPipeCommandRunningCount = 10;

//            try
//            {
//                serverTasks = new List<Task<int>>();

//                // タスクをデリゲートとして定義します
//                Func<object, int> action = (object obj) =>
//                {
//                    int serverId = (int)obj;

//                    Task task = PIPEserverGeneration(serverId, numberOfTasks);

//                    CheckPipeCommandRunningCount(checkPipeCommandRunningCount);

//                    int tickCount = Environment.TickCount;
//                    return tickCount;
//                };

//                SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.Information, 4005, $"■PIPEサーバ生成 開始 合計 {numberOfTasks} セッション");

//                // 開始したタスクの構築
//                for (int i = 0; i < numberOfTasks; i++)
//                {
//                    int index = i;
//                    serverTasks.Add(Task<int>.Factory.StartNew(action, index));
//                }


//                try
//                {
//                    ServerLog.PiperServerLogging.LogRotateWriteLine($"{methodname} Task.WaitAll():を呼び出します", FlashSync: true);

//                    // すべてのタスクが終了するのを待ちます。
//                    Task.WaitAll(serverTasks.ToArray());

//                    ServerLog.PiperServerLogging.LogRotateWriteLine($"{methodname} Task.WaitAll():から戻りました", FlashSync: true);

//                    SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.Information, 4005, $"■PIPEサーバを生成 しました 合計 {serverTasks.Count} セッション");
//                }
//                catch (AggregateException e)
//                {
//                    StringBuilder sb = new StringBuilder();
//                    for (int j = 0; j < e.InnerExceptions.Count; j++)
//                    {
//                        sb.Append($"{e.InnerExceptions[j]}");
//                    }

//                    SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.Error, 4005, $"{LogMsg_ServiceName} Task.WaitAll(...)で例外キャッチ：続行\n {e.Message}\n{sb.ToString()}");
//                }
//            }
//            catch (Exception ex)
//            {
//                SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.FailureAudit, 4005, $"{LogMsg_ServiceName} CreateTasks()で例外キャッチ：CreateTasks()を再実行します\n {ex.Message} {ex.StackTrace}");
//                ServerLog.PiperServerLogging.LogRotateWriteLine($"{methodname} ※再呼出し:CreateTasks() 開始", FlashSync: true);
//                CreateTasks(numberOfTasks, checkPipeCommandRunningCount);
//                ServerLog.PiperServerLogging.LogRotateWriteLine($"{methodname} ※再呼出し:CreateTasks() 終了", FlashSync: true);
//            }
//        }

//        /// <summary>
//        /// パイプサーバー生成
//        /// </summary>
//        /// <param name="serverId"></param>
//        /// <returns></returns>
//        private async Task PIPEserverGeneration(int serverId, int numberOfTask)
//        {
//            //GUID生成
//            GUIDExtensions gUIDExtensions = new GUIDExtensions(true);
//            string objectID = gUIDExtensions.B64String;

//            // ループスタート
//            while (true)
//            {
//                bool BUSYFLAG = false;
//                DateTime startTime = DateTime.Now;

//                StopWatch stopWatch;

//                // ｸﾗｲｱﾝﾄから送られてきたコマンド文字列
//                string command = null;

//                string clientinfo = null;

//                try
//                {
//                    // 名前付きパイプサーバーの生成 
//                    try
//                    {
//                        using (NamedPipeServerStream namedPipeSrvStream = Create(PIPEName, numberOfTask))
//                        {
//                            StreamString stst = new StreamString(namedPipeSrvStream);


//                            bool OperationCanceledException = false;

//                            bool AggregateException = false;


//                            // クライアントからの接続を待つ
//                            Task treslt = namedPipeSrvStream.WaitForConnectionAsync();

//                            if (IsLoggingPIPE_NormalStatus)
//                                ServerLog.PiperServerLogging.LogRotateWriteLine($"[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}] 接続待機します", FlashSync: true);

//                            await treslt; // ここで接続待機となります

//                            if (activeSessionCommandList.Count >= numberOfTask - 1)
//                            {
//                                BUSYFLAG = true;
//                                ServerLog.PiperServerLogging.LogRotateWriteLine($"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}] 名前付きパイプのコマンドセッション数が ({numberOfTask} -1) 以上です。BUFSYFLAG=true にしました", FlashSync: true);
//                            } // セッション数確認
//                            else
//                            {
//                                BUSYFLAG = false;
//                                //Console.WriteLine($"□{LogMsg_ServiceName} 名前付きパイプのコマンドセッション数が ({numberOfTask} -1)未満です。BUFSYFLAG=false にしました");
//                            } // ※エラー処理 （セッション数確認 ）

//                            clientinfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);

//                            if (BUSYFLAG == true)
//                            {
//                                // 受付不可キーワード送出
//                                stst.WriteString(@"BUSY");

//                                PipeServer_BUSY_Count++;

//                                SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.Warning, 2002, $"※{LogMsg_ServiceName} {clientinfo} TaskID:{treslt.Id} コマンドサーバー PIPEserverGenerationType(...) BUSYフラグtrueのため、受け付けません。(ロック開始:{startTime.ToShortTimeString()}), [PipeNameDR_BUSY_Count:{PipeServer_BUSY_Count}],  StartHandShakeReadWriteTimeOut:{StartHandShakeReadWriteTimeOut}");

//                                namedPipeSrvStream.Close();
//                                BUSYFLAG = false;

//                                ServerLog.PiperServerLogging.LogRotateWriteLine($"※パイプサーバー[ID:{serverId}:{objectID}], [Task.ID:{treslt.Id}] BUSYフラグ true (ロック開始:{startTime.ToShortTimeString()})、処理中です受け付けません. これまでの回数 [PipeNameDR_BUSY_Count:{PipeServer_BUSY_Count}]", FlashSync: true);

//                                RemoveSessionCommandList(serverId, treslt.Id);
//                                continue;
//                            } // ※エラー処理

//                            BUSYFLAG = true;
//                            startTime = DateTime.Now;


//                            try
//                            {
//                                //⓪ハンドシェイクスタートメッセージ送出
//                                // PIPEサーバー識別文字列を送信。-1なら失敗として continue                                                 
//                                int writeResult = stst.WriteString(ServerHandshakeStartMSG, StartHandShakeReadWriteTimeOut, out OperationCanceledException, out AggregateException, WriteNull);

//                                if (writeResult == -1)
//                                {
//                                    namedPipeSrvStream.Close();
//                                    BUSYFLAG = false;

//                                    SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.Warning, 2002,
//                                        $"※{LogMsg_ServiceName} {clientinfo} TaskID:{treslt.Id} PIPEサーバー {serverId} 識別文字列 \"{ServerHandshakeStartMSG}\" 送信失敗 , StartHandShakeReadWriteTimeOut:{StartHandShakeReadWriteTimeOut}");
//                                    ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                        $"※パイプサーバー[ID:{serverId}:{objectID}], [Task.ID:{treslt.Id}] 識別文字列 \"{ServerHandshakeStartMSG}\" 送信失敗 , StartHandShakeReadWriteTimeOut:{StartHandShakeReadWriteTimeOut}", FlashSync: true);

//                                    RemoveSessionCommandList(serverId, treslt.Id);
//                                    continue;
//                                } // ※エラー処理
//                            }
//                            catch (Exception ex)
//                            {
//                                namedPipeSrvStream.Close();
//                                BUSYFLAG = false;

//                                ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                   $"※パイプサーバー[ID:{serverId}:{objectID}], [Task.ID:{treslt.Id}] ハンドシェイクメッセージ送信にて例外 {ex.Message} {ex.StackTrace} ,StartHandShakeReadWriteTimeOut:{StartHandShakeReadWriteTimeOut} ", FlashSync: true);

//                                RemoveSessionCommandList(serverId, treslt.Id);
//                                continue;
//                            } // ハンドシェイク処理（ServerHandshakeStartMSG 送信処理）

//                            try
//                            {

//                                if (IsLoggingPIPE_NormalStatus)
//                                    ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                    $"[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo} 接続されました。クライアントからのコマンド名受信待機中 タイムアウト {StartHandShakeReadWriteTimeOut}", FlashSync: true);


//                                //①クライアントから最初に送られてきた文字列をコマンド名とする。


//                                //command = stst.ReadString();
//                                //command = stst.ReadString(ReadStreamStringTimeOut , null);
//                                command = stst.ReadString(StartHandShakeReadWriteTimeOut, out OperationCanceledException, out AggregateException, null);

//                                if (OperationCanceledException == true || AggregateException == true || string.IsNullOrEmpty(command) == true)
//                                {
//                                    if (OperationCanceledException == true)
//                                    {
//                                        ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                           $"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo} ServerHandshakeStartMSG送信後、OperationCanceledExceptionが発生 ﾀｲﾑｱｳﾄ {StartHandShakeReadWriteTimeOut} msecです", FlashSync: true);
//                                    }
//                                    if (AggregateException == true)
//                                    {
//                                        ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                           $"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo} ServerHandshakeStartMSG送信後、AggregateExceptionが発生　ﾀｲﾑｱｳﾄ{StartHandShakeReadWriteTimeOut}の受信待機中 に  を検知", FlashSync: true);

//                                    }
//                                    if (command == "")
//                                    {
//                                        ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                           $"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo} ServerHandshakeStartMSG送信後、次に送られてきた受信文字列が　\"\"(空文字) でした。", FlashSync: true);
//                                    }
//                                    if (command == null)
//                                    {
//                                        ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                           $"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo} ServerHandshakeStartMSG送信後、次にられてきたデータが null でした。", FlashSync: true);
//                                    }

//                                    ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                           $"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo} NamedPipeServerStreamを閉じて whileループを continue します. 現時点での実行中パイプコマンド数:{activeSessionCommandList.Count}", FlashSync: true);

//                                    namedPipeSrvStream.Close();
//                                    BUSYFLAG = false;
//                                    RemoveSessionCommandList(serverId, treslt.Id);
//                                    continue;

//                                }

//                                if (IsLoggingPIPE_NormalStatus)
//                                    ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                    $"[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo} クライアントから コマンド名【{command}】を受信しました ", FlashSync: true);
//                            }
//                            catch (Exception ex)
//                            {
//                                namedPipeSrvStream.Close();
//                                BUSYFLAG = false;

//                                ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                   $"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo}　コマンド受信にて例外 {ex.Message} {ex.StackTrace} , StartHandShakeReadWriteTimeOut:{StartHandShakeReadWriteTimeOut}", FlashSync: true);

//                                RemoveSessionCommandList(serverId, treslt.Id);
//                                continue;
//                            } // コマンド名受信

//                            stopWatch = new StopWatch(); // ストップウォッチ開始

//                            // 実行中のコマンドセッションリストを更新
//                            bool updateSessionlistResult = UpdateSessionCommandList(serverId, objectID, treslt.Id, NamedPipeClientInfo.GetClientComputerName(namedPipeSrvStream), namedPipeSrvStream.GetImpersonationUserName(), command);


//                            if (updateSessionlistResult == false)
//                                throw new Exception("UpdateSessionCommandList(..)の戻り値が false でした これは 同じserverID, TaskID で更新されたことを示します");

//                            if (namedPipeSrvStream.IsConnected == false)
//                            {
//                                namedPipeSrvStream.Close();
//                                BUSYFLAG = false;

//                                ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                   $"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo} PIPEserverGeneration(...) 内にて  コマンド実行直前に NamedPipeServerStreamIsConnected==false を検知しました。", FlashSync: true);

//                                RemoveSessionCommandList(serverId, treslt.Id);
//                                continue;
//                            } // ※エラー処理

//                            //if (OperationCanceledException == false && AggregateException == false)
//                            //{
//                            //■コマンドワード解析と実行
//                            bool result = commandAnalysisAndExecute(serverId, objectID, treslt.Id, namedPipeSrvStream, command);

//                            if (result == false)
//                            {

//                                SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.Error, 2002,
//                                    $"※{LogMsg_ServiceName} TaskID:{treslt.Id}  PIPEserverGenerationType(...) {clientinfo} ,delegate_CommandAnalysisAndExecute(..) コマンド実行ディスパッチャの戻り値が false でした, StartHandShakeReadWriteTimeOut:{StartHandShakeReadWriteTimeOut}");

//                                ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                   $"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo}  delegate_CommandAnalysisAndExecute(..) コマンド実行ディスパッチャの戻り値が false でした, StartHandShakeReadWriteTimeOut:{StartHandShakeReadWriteTimeOut}", FlashSync: true);
//                            }

//                            var Elapsed = stopWatch.Stop();

//                            bool removeSessionCommandListResult = RemoveSessionCommandList(serverId, treslt.Id);

//                            if (updateSessionlistResult == false)
//                                throw new Exception($"RemoveSessionCommandList(..)の戻り値が false でした これは 削除指定した serverID, TaskID がListに存在しないことを示します。現時点での実行中パイプコマンド数:{activeSessionCommandList.Count}");

//                            if (IsLoggingPIPE_NormalStatus)
//                                ServerLog.PiperServerLogging.LogRotateWriteLine(
//                                $"[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{treslt.Id}]{clientinfo} 呼び出されたコマンド[{command}]が終了 消費時間 [{Elapsed.TotalMilliseconds:F1} msec], 現時点での実行中パイプコマンド数:{activeSessionCommandList.Count}", FlashSync: true);

//                        } // using end
//                    }
//                    catch (Exception ex)
//                    {
//                        var errMsg = $"※{LogMsg_ServiceName}  パイプコマンド受信 using 内にて 例外検知 {ex.Message} {ex.StackTrace}.";
//                        SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.Error, 2002, errMsg, OutConsole: false);
//                    }

//                    BUSYFLAG = false;

//                    DebugClass.ConsoleDebugOut(5, $"■[ServerID:{serverId}] ---エンド--------------------------------", viewLvele: false);
//                }
//                catch (Exception ex)
//                {
//                    var errMsg = $"※{LogMsg_ServiceName} PIPEserverGenerationType(...) Whileループ内にて 例外検知  {ex.Message} {ex.StackTrace} {clientinfo}] ｺﾏﾝﾄﾞ{command}";
//                    SharedClassLibrary.DebugClass.ConsoleDebugOut(9, errMsg);
//                    SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.Error, 2002, errMsg, OutConsole: false);

//                    BUSYFLAG = false;
//                }

//                //Console.WriteLine($"■{LogMsg_ServiceName} ---エンド------- ActiveCommandJobList.Count:{activeSessionCommandList.Count}--------------------");

//            } // while ループエンド
//        }

//        private async void CheckPipeCommandRunningCount(int maxCount)
//        {

//            while (true)
//            {
//                if (activeSessionCommandList.Count > maxCount)
//                {
//                    SasaLib.Eventlog.Log.WriteEntry(EventViewSourceName, EventLogEntryType.Warning, 4005, $"※PIPEサーバー 【{PIPEName}】\n" +
//                        $"実行中のPIPEコマンドセッションが {maxCount} を超えました！！\n" +
//                        $"現在: {activeSessionCommandList.Count}");
//                }
//                await Task.Delay(1000);
//            } // while ループエンド

//        }

//        /// <summary>
//        /// 現在のセッションコマンドリストを更新
//        /// </summary>
//        /// <param name="serverId"></param>
//        /// <param name="clientUser"></param>
//        /// <param name="command"></param>
//        internal bool UpdateSessionCommandList(int serverId, string objectID, int TaskID, string clientHost, string clientUser, string command)
//        {
//            AcceptPipeCommand accceptPipeCommand = new AcceptPipeCommand()
//            {
//                ServerId = serverId,
//                TaskID = TaskID,
//                ObjectID = objectID,
//                ClientHost = clientHost,
//                ClientUser = clientUser,
//                CommandName = command,
//                Command_Accept_DateTime = DateTime.Now
//            };

//            lock (activeCommanList_LockHandler)
//            {
//                var index = activeSessionCommandList.FindIndex(a => a.ServerId == serverId && a.TaskID == TaskID);
//                if (index > -1)
//                {
//                    activeSessionCommandList.RemoveAt(index);
//                    activeSessionCommandList.Insert(index, accceptPipeCommand);
//                    return false;
//                }
//                else
//                {
//                    activeSessionCommandList.Add(accceptPipeCommand);
//                    return true;
//                }
//            }
//        }

//        /// <summary>
//        /// 現在のセッションコマンドリストから指定セッションを削除
//        /// </summary>
//        /// <param name="serverId"></param>
//        internal bool RemoveSessionCommandList(int serverId, int TaskID)
//        {
//            try
//            {
//                lock (activeCommanList_LockHandler)
//                {
//                    var index = activeSessionCommandList.FindIndex(a => a.ServerId == serverId && a.TaskID == TaskID);

//                    if (index > -1)
//                    {
//                        activeSessionCommandList.RemoveAt(index);
//                        return true;
//                    }
//                    else
//                    {
//                        return false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ServerLog.PiperServerLogging.LogRotateWriteLine(
//                    $"RemoveSessionCommandList()にて例外検知 {ex.Message}{ex.InnerException}"
//                , FlashSync: true);

//                return false;
//            }
//        }

//        /// <summary>
//        /// 実行中のセッションリストを返す
//        /// </summary>
//        /// <returns></returns>
//        public static List<AcceptPipeCommand> GetSessionCommandList()
//        {
//            return activeSessionCommandList;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="pipeName"></param>
//        /// <param name="maxInstances"></param>
//        /// <returns></returns>
//        private NamedPipeServerStream Create(string pipeName, int maxInstances = NamedPipeServerStream.MaxAllowedServerInstances)
//        {
//            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
//            PipeAccessRule par = new PipeAccessRule(sid, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);

//            PipeSecurity ps = new PipeSecurity();

//            ps.AddAccessRule(new PipeAccessRule("Users", PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow));
//            ps.AddAccessRule(new PipeAccessRule("CREATOR OWNER", PipeAccessRights.FullControl, AccessControlType.Allow));
//            ps.AddAccessRule(new PipeAccessRule("SYSTEM", PipeAccessRights.FullControl, AccessControlType.Allow));

//            ps.AddAccessRule(par);

//            //TODO NamedPipeServerStream の 仕様変更
//            //return new NamedPipeServerStream(PipeName, PipeDirection.InOut, maxInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 2048, 2048, ps);
//            var stream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, maxInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 2048, 2048);
//            stream.SetAccessControl(ps);
//            return stream;
//        }

//        /// <summary>
//        /// コマンド解析・実行メソッドのダミー（デリゲートに指定されなかった場合）
//        /// </summary>
//        /// <param name="command"></param>
//        /// <param name="pipeSrvStream"></param>
//        /// <param name="serverId"></param>
//        /// <param name="ImpersonationUserName"></param>
//        /// <param name="BUSYFLAG"></param>
//        /// <returns></returns>
//        private bool dummyCommandAnalysisAndExecute(int serverId, string objcetId, int taskId, NamedPipeServerStream namedPipeSrvStream, string command)
//        {
//            string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);
//            // 接続してきたクライアントのユーザ名
//            string ImpersonationUserName = namedPipeSrvStream.GetImpersonationUserName();


//            ServerLog.PiperServerLogging.LogRotateWriteLine(
//                $"■{LogMsg_ServiceName} {clientInfo} PIPEServer.dummyCommandAnalysisAndExecute(..)が呼ばれました "
//            , FlashSync: true);

//            return true;
//        }

//        private void WriteNull(string msg)
//        {
//            ;
//        }

//        string GenerateRandomString(int length)
//        {
//            // ランダムな文字列を生成するための文字のセット
//            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

//            // Randomオブジェクトを作成
//            Random random = new Random();

//            // StringBuilderを使用してランダムな文字列を構築
//            char[] stringChars = new char[length];
//            for (int i = 0; i < length; i++)
//            {
//                stringChars[i] = chars[random.Next(chars.Length)];
//            }

//            // 文字配列を文字列に変換して返す
//            return new string(stringChars);
//        }
//    }


//}
