//using SasaLib;
//using SasaLib.PIPE;
//using SasaLibDummy;
//using SharedClassLibrary;
//using STAGINGSYSTEM_COMMANDS;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Pipes;
//using System.Linq;
//using System.Runtime.Versioning;
//using System.Text;
//using System.Threading.Tasks;

//namespace ToyoStageService
//{
//    [SupportedOSPlatform("windows")]

//    /// <summary>
//    /// ステージサーバー間でPIPE接続を行うクライアント側クラス
//    /// </summary>
//    public class PIPEClient
//    {
//        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

//        /// <summary>
//        /// 接続ユーザーのドメイン名(ForcedAccountFlagがtrueの時に使われる)
//        /// </summary>
//        readonly string ForcedDomainName;
//        /// <summary>
//        /// 接続するユーザー名(ForcedAccountFlagがtrueの時に使われる)
//        /// </summary>
//        readonly string ForcedUserName;
//        /// <summary>
//        /// 接続するユーザーのパスワード(ForcedAccountFlagがtrueの時に使われる)
//        /// </summary>
//        readonly string ForcedUserPassword;
//        /// <summary>
//        /// trueの場合、指定したアカウントで偽装接続行う。
//        /// </summary>
//        readonly bool ForcedAccountFlag;
//        /// <summary>
//        /// 接続先サーバー名
//        /// </summary>
//        readonly string PipeServerName;
//        /// <summary>
//        /// 接続先PIPE名
//        /// </summary>
//        readonly string PipeName;

//        /// <summary>
//        /// PIPE接続を実行する。
//        /// </summary>
//        /// <param name="ForcedDomainName">接続ユーザーのドメイン名(ForcedAccountFlagがtrueのとき)</param>
//        /// <param name="ForcedUserName">接続ユーザー名(ForcedAccountFlagがtrueのとき)</param>
//        /// <param name="ForcedUserPassword">接続ユーザーのパスワード(ForcedAccountFlagがtrueのとき)</param>
//        /// <param name="ForcedAccountFlag">trueの時偽装ログオンを実行</param>
//        /// <param name="PipeServerName">接続先サーバー名</param>
//        /// <param name="PipeName">接続先PIPE名</param>
//        public PIPEClient(string ForcedDomainName, string ForcedUserName, string ForcedUserPassword, bool ForcedAccountFlag, string PipeServerName, string PipeName)
//        {
//            this.ForcedDomainName = ForcedDomainName;
//            this.ForcedUserName = ForcedUserName;
//            this.ForcedUserPassword = ForcedUserPassword;
//            this.ForcedAccountFlag = ForcedAccountFlag;
//            this.PipeServerName = PipeServerName;
//            this.PipeName = PipeName;
//        }

//        /// <summary>
//        /// ■サーバへの接続可否を確認する
//        /// </summary>
//        /// <returns></returns>
//        public bool ConnectTest(int clientimeOut, int readHandShakeStreamStringTimeOut)
//        {
//            DateTime dt1 = DateTime.Now;

//            using (new ClsLogonDummy(ForcedDomainName, ForcedUserName, ForcedUserPassword, ForcedAccountFlag))
//            {
//                try
//                {
//                    NamedPipeClientStream pipeClientst = new NamedPipeClientStream(PipeServerName, PipeName);

//                    // 待機中のサーバーへ接続
//                    pipeClientst.Connect(clientimeOut);

//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString ss = new StreamString(pipeClientst);

//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = ss.ReadString(readHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6004, $"PIPEClient.ConnectTest(..) ReadString(..)失敗。ReadHandShakeStreamStringTimeOut= {readHandShakeStreamStringTimeOut} msec , OperationCanceledException = {OperationCanceledException}, AggregateException = {AggregateException}");
//                        return false;
//                    }

//                    if (CheckFirstMessage(input0))
//                    {
//                        DebugClass.ConsoleDebugOut(9, $"■コマンドクライアント【ConnectTest】ハンドシェイク成功．接続先 \"\\\\{PipeServerName}\\pipe\\{PipeName}\"からの接続文字列{input0}は期待値です");

//                        string command = CMDS.DC_DR_SW_ConnectTest;
//                        DebugClass.ConsoleDebugOut(9, $"■コマンドクライアント【ConnectTest】接続先 \"\\\\{PipeServerName}\\pipe\\{PipeName}\" \"{command}\" を送信");
//                        ss.WriteString(command);

//                        string str = "接続テスト";
//                        DebugClass.ConsoleDebugOut(9, $"■コマンドクライアント【ConnectTest】接続先 \"\\\\{PipeServerName}\\pipe\\{PipeName}\" \"{str}\" を送信");
//                        ss.WriteString(str);

//                        /// サーバーから結果情報を取得
//                        string AnserMessage = ss.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut, null);
//                        DebugClass.ConsoleDebugOut(9, $"■コマンドクライアント【ConnectTest】接続先 \"\\\\{PipeServerName}\\pipe\\{PipeName}\" \"{AnserMessage}\" を受信");

//                        if (str == AnserMessage)
//                        {
//                            DebugClass.ConsoleDebugOut(8, $"■コマンドクライアント【ConnectTest】接続先 \"\\\\{PipeServerName}\\pipe\\{PipeName}\", 送信内容\"{str}\"と受信内容\"{AnserMessage}\"が一致したので接続テストは問題ありません");
//                            pipeClientst.Close();

//                            return true;
//                        }
//                        else
//                        {
//                            ServerLog.Logging.LogRotateWriteLine($"※コマンドクライアント【ConnectTest】接続先 \"\\\\{PipeServerName}\\pipe\\{PipeName}\", 送信内容\"{str}\"と受信内容\"{AnserMessage}\"が不一致！！接続テスト失敗", DebugWriteLineSwitch: true, FlashSync: true);
//                            pipeClientst.Close();

//                            return false;
//                        }
//                    }
//                    else
//                    {
//                        DebugClass.ConsoleDebugOut(0, $"※コマンドクライアント【ConnectTest】接続先 \"\\\\{PipeServerName}\\pipe\\{PipeName}\",サーバーからの接続文字列{input0}が期待と違います。サーバー:{PipeServerName} PIPE名:{PipeName}への接続テスト失敗");
//                        pipeClientst.Close();

//                        return false;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }

//                catch (IOException ioex)
//                {
//                    ServerLog.Logging.LogRotateWriteLine($"※コマンドクライアント【ConnectTest】PIPEClient.ConnectTest()にて例外検知 {ioex.Message} タイムアウト:{readHandShakeStreamStringTimeOut} 接続先 \"\\\\{PipeServerName}\\pipe\\{PipeName}\" に接続できません", DebugWriteLineSwitch: true, FlashSync: true);
//                    return false;
//                }
//                catch (Exception ex)
//                {
//                    ServerLog.Logging.LogRotateWriteLine($"※コマンドクライアント【ConnectTest】PIPEClient.ConnectTest()にて例外検知 {ex.Message} 接続先 \"\\\\{PipeServerName}\\pipe\\{PipeName}\" に接続できません",DebugWriteLineSwitch:true,FlashSync:true);
//                    return false;
//                }

//            }
//        }


//        /// <summary>
//        /// サーバーから返答された識別文字列をチェックする
//        /// </summary>
//        /// <param name="input0">サーバーからの文字列を指定</param>
//        /// <returns></returns>
//        private bool CheckFirstMessage(string input0)
//        {
//            if (input0 == StageServerConfig.Config.HandShakeKeyword)
//                return true;
//            else if (input0 == @"BUSY")
//            {
//                return false;
//            }
//            else
//            {
//                return false;
//            }
//        }


//        /// <summary>
//        /// サーバーのモードを確認
//        /// </summary>
//        /// <returns></returns>
//        public StageServerConfig.ServerMode Get_Status_Servemode(int clientimeOut,int readHandShakeStreamStringTimeOut, EventsSummary evt)
//        {
//            using (new ClsLogonDummy(ForcedDomainName, ForcedUserName, ForcedUserPassword, ForcedAccountFlag))
//            {
//                StageServerConfig.ServerMode serverMode = StageServerConfig.ServerMode.Unknown;
//                try
//                {
//                    NamedPipeClientStream pipeClientst = new NamedPipeClientStream(PipeServerName, PipeName);

//                    // 待機中のサーバーへ接続
//                    pipeClientst.Connect(clientimeOut);

//                    StreamString ss = new StreamString(pipeClientst);
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = ss.ReadString(readHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        evt.Add($"※PIPEClient.Get_Status_Servemode(..) PIPE接続失敗。ハンドシェイクでタイムアウト", OutConsole: true);

//                        return StageServerConfig.ServerMode.Unknown;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        //SasaLib.Eventlog.Log.WriteEntry("Server to Server PIPE", EventLogEntryType.Information, 5107,
//                        //    $"{Environment.MachineName} Get_Status_Servemode() 相手サーバー{PipeServerName}からの接続文字列{input0}は期待値です");

//                        string command = CMDS.DC_DR_Status;
//                        ss.WriteString(command);
//                        string str = CMDS.DC_Status_CheckSERVERMODE;
//                        ss.WriteString(str);

//                        evt.Add($"Get_Status_Servemode() 相手サーバー{PipeServerName}へCheckSERVERMODEを送信", OutConsole: true);

//                        /// サーバーから結果情報を取得

//                        using (BinaryReader reader = new BinaryReader(pipeClientst, Encoding.UTF8, true)) // 送信されるサイズを受信しバッファー準備
//                        {
//                            serverMode = reader.ReadObject<StageServerConfig.ServerMode>(); //
//                        }

//                        pipeClientst.Close();

//                        evt.Add($"Get_Status_Servemode() CheckFirstMessage({input0})実行。相手サーバー{PipeServerName}から StageServerConfig.ServerMode型 \"{serverMode}\" を受信しました", OutConsole: true);
//                    }
//                    else
//                    {
//                        pipeClientst.Close();

//                        evt.Add($"※Get_Status_Servemode() CheckFirstMessage({input0})実行。相手サーバー{PipeServerName}から {input0} が返されました", OutConsole: true);
//                    }

//                    return serverMode;
//                }
//                catch (Exception ex)
//                {
//                    DebugClass.ConsoleDebugOut(0, $"▲{AssemblyInternalName} StageServerConfig.ServerMode Get_Status_Servemode()にて例外検知 {ex.Message} サーバー名:{PipeServerName}/パイプ名:{PipeName} に接続できません");
//                    evt.Add($"▲{AssemblyInternalName} StageServerConfig.ServerMode Get_Status_Servemode()にて例外検知 {ex.Message} {ex.InnerException} サーバー名:{PipeServerName}/パイプ名:{PipeName} に接続できません", OutConsole: true);
//                    return StageServerConfig.ServerMode.Unknown;
//                }
//            }

//        }
//    }
//}
