using SasaLib;
using SasaLib.PIPE;
using SasaLibDummy;
using SharedClassLibrary;
using StageServerRemote;
using STAGINGSYSTEM_COMMANDS;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;
using ToyoMcMfg.Staging.DataBaseConfig;
using ToyoStageService;

namespace RemoteClient
{
    [SupportedOSPlatform("windows")]

    public class RemoteClientServerControl : RMCsupport
    {

        private readonly string pipename;

        #region ●プロパティ
        /// <summary>
        ///
        /// </summary>
        internal string DomainName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        internal string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        internal string UserPassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        internal bool ClsLogon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        internal string Hostname { get; set; }

        /// <summary>
        /// 接続タイムアウト
        /// </summary>
        public int ClientTimeOut { get; set; } = 20000;

        public int ReadStreamStringTimeOut { get; set; } = 20000;

        public int ReadHandShakeStreamStringTimeOut { get; set; } = 10000;

        /// <summary>
        /// PIPE接続のステータス
        /// </summary>
        public bool PipeConnectionStatus { get; private set; }

        /// <summary>
        /// 実行結果メッセージ
        /// </summary>
        internal string AnserMessage { get; private set; }
        #endregion

        #region ●コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="DomainName"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="ClsLogonDummy"></param>
        /// <param name="PipeServerName"></param>
        /// <param name="PipeName"></param>
        public RemoteClientServerControl(string DomainName,
            string UserName,
            string UserPassword,
            bool ClsLogonDummy,
            string PipeServerName,
            string PipeName)
        {
            this.DomainName = DomainName;
            this.UserName = UserName;
            this.UserPassword = UserPassword;
            this.ClsLogon = ClsLogonDummy;

            this.Hostname = PipeServerName;
            this.pipename = PipeName;
        }
        #endregion


        /// <summary>
        /// ■StageServerConfig.XMLをリロードする "ServerControl" "RELOAD_STAGESERVERCONFIG"
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string RELOAD_STAGESERVERCONFIG(string Hostname, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientServerControl:RELOAD_STAGESERVERCONFIG
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;

                        delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                        return "Error";
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    // サーバーからのサーバ識別文字列を受け取ります。
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return null;
                    }
                    delegateWriteLine($"サーバーから返信:{input0}");

                    if (CheckFirstMessage(input0))
                    {
                        string token2 = CMDS.DC_DR_SW_ServerControl;
                        delegateWriteLine($"サーバーへ返信:{token2}");
                        stst.WriteString(token2);

                        string token3 = CMDS.DC_ServerControl_RELOAD_STAGESERVERCONFIG;
                        delegateWriteLine($"サーバーへ返信:{token3}");
                        stst.WriteString(token3);

                        pipeCltStream.Close();
                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    GlovalValues.Mylog.LogRotateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■StampConf.XMLをリロードする "ServerControl" "RELOAD_STAMPCONF"
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string RELOAD_STAMPCONF(string Hostname, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientServerControl:RELOAD_STAMPCONF
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;

                        delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                        return "Error";
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    // サーバーからのサーバ識別文字列を受け取ります。
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return null;
                    }
                    delegateWriteLine($"サーバーから返信:{input0}");

                    if (CheckFirstMessage(input0))
                    {
                        string token2 = CMDS.DC_DR_SW_ServerControl;
                        delegateWriteLine($"サーバーへ返信:{token2}");
                        stst.WriteString(token2);

                        string token3 = CMDS.DC_ServerControl_RELOAD_STAMPCONF;
                        delegateWriteLine($"サーバーへ返信:{token3}");
                        stst.WriteString(token3);

                        pipeCltStream.Close();
                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    GlovalValues.Mylog.LogRotateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■BarcodeConf.XMLをリロードする "ServerControl" "RELOAD_STAMPCONF"
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string RELOAD_BARCODECONF(string Hostname, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientServerControl:RELOAD_BARCODECONF
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;

                        delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                        return "Error";
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    // サーバーからのサーバ識別文字列を受け取ります。
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return null;
                    }
                    delegateWriteLine($"サーバーから返信:{input0}");

                    if (CheckFirstMessage(input0))
                    {
                        string token2 = CMDS.DC_DR_SW_ServerControl;
                        delegateWriteLine($"サーバーへ返信:{token2}");
                        stst.WriteString(token2);

                        string token3 = CMDS.DC_ServerContorl_RELOAD_BARCODECONF;
                        delegateWriteLine($"サーバーへ返信:{token3}");
                        stst.WriteString(token3);

                        pipeCltStream.Close();
                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    GlovalValues.Mylog.LogRotateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string SAVE_STAGESERVERCONFIG(string Hostname, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientServerControl;SAVE_STAGESERVERCONFIG
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;

                        delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                        return "Error";
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    // サーバーからのサーバ識別文字列を受け取ります。
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return null;
                    }
                    delegateWriteLine($"サーバーから返信:{input0}");

                    if (CheckFirstMessage(input0))
                    {
                        string token2 = CMDS.DC_DR_SW_ServerControl;
                        delegateWriteLine($"サーバーへ返信:{token2}");
                        stst.WriteString(token2);

                        string token3 = CMDS.DC_DR_SW_ServerControl_SAVE_STAGESERVERCONFIG;
                        delegateWriteLine($"サーバーへ返信:{token3}");
                        stst.WriteString(token3);

                        pipeCltStream.Close();
                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    GlovalValues.Mylog.LogRotateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string SAVE_STAGESERVERDATABASECONFIG(string Hostname, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientServerControl:SAVE_STAGESERVERDATABASECONFIG
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;

                        delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                        return "Error";
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    // サーバーからのサーバ識別文字列を受け取ります。
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return null;
                    }
                    delegateWriteLine($"サーバーから返信:{input0}");

                    if (CheckFirstMessage(input0))
                    {
                        string token2 = CMDS.DC_DR_SW_ServerControl;
                        delegateWriteLine($"サーバーへ返信:{token2}");
                        stst.WriteString(token2);

                        string token3 = CMDS.DC_SW_ServerContorol_SAVE_STAGESERVERDATABASECONFIG;
                        delegateWriteLine($"サーバーへ返信:{token3}");
                        stst.WriteString(token3);

                        pipeCltStream.Close();
                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    GlovalValues.Mylog.LogRotateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■サーバーモードを得る "Status" "CURRENT MODE"
        /// </summary>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string Check_Status_CurrentMode(SasaLibDelegateWriteLine delegateWriteLine = null, bool IsOutMsg = true)
        {
            if (delegateWriteLine == null) { delegateWriteLine = Console.WriteLine; IsOutMsg = false; }

            DateTime dt1 = DateTime.Now;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientServerControl:Check_Status_CurrentMode
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon, debugConsoleMsg: false))
            {

                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);

                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);

                    }
                    catch (IOException ioex)
                    {
                        PipeConnectionStatus = false;
                        GlovalValues.Mylog.LogRotateWriteLine($"Check_Status_CurrentMode(...) \nPIPEサーバー接続エラー {Hostname}\n {ioex.Message}");
                        delegateWriteLine($"Check_Status_CurrentMode(...) \nPIPEサーバー接続エラー  PipeServerName={Hostname} IsConnected={pipeCltStream.IsConnected} ClientTImeOut {ClientTimeOut}\n {ioex.Message}");
                        pipeCltStream.Close();
                        return null;

                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        GlovalValues.Mylog.LogRotateWriteLine($"Check_Status_CurrentMode(...) \nPIPEサーバー接続エラー {Hostname}\n {ex.Message}");
                        delegateWriteLine($"Check_Status_CurrentMode(...) \nPIPEサーバー接続エラー  PipeServerName={Hostname} IsConnected={pipeCltStream.IsConnected} ClientTImeOut {ClientTimeOut}\n {ex.Message}");
                        pipeCltStream.Close();
                        return "Error";
                    }

                    // サーバーからのサーバ識別文字列を受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);

                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return null;
                    }

                    if (CheckFirstMessage(input0))
                    {
                        if (IsOutMsg)
                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"■Check_Status_CurrentMode():{dt1}:サーバーからの接続文字列{input0}は期待値です");

                        int writeResult = stst.WriteString(CMDS.DC_DR_Status);

                        stst.WriteString(CMDS.DC_DR_Status_CURRENT_MODE);

                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
                        if (IsOutMsg)
                            delegateWriteLine($"{DateTime.Now.ToString()} Check_Status_CurrentMode() Server:{Hostname} PipeName:{pipename}【{AnserMessage}】を受信しました");

                        pipeCltStream.Close();

                        return AnserMessage;
                    }
                    else
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"■Check_Status_CurrentMode():{dt1}:サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    GlovalValues.Mylog.LogRotateWriteLine($"Check_Status_CurrentMode(...) PIPEサーバー接続エラー {ex.Message}");
                    delegateWriteLine($"Check_Status_CurrentMode(...) PIPEサーバー接続エラー {ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■処理完了に時間のかかるコマンドを疑似実行する ストレステスト
        /// </summary>
        /// <param name="resultMessage"></param>
        /// <param name="wait_minitus_timestr">パイプコマンドが終了までかかる時間を文字列で分指定</param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public bool StressTest(int readWrteStringTimeOut, out string resultMessage, string wait_minitus_timestr = "1", SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

            resultMessage = null;

            //using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            //{
            //    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
            //    // 待機中のサーバーへ接続
            //    try
            //    {
            //        pipeCltStream.Connect(ClientTimeOut);
            //        // サーバーからの書き込みを受け取ります。
            //        StreamString stst = new StreamString(pipeCltStream);
            //        bool OperationCanceledException;
            //        bool AggregateException;

            //        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
            //        if (OperationCanceledException || AggregateException)
            //        {
            //            delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
            //            return false;
            //        }
            //        if (CheckFirstMessage(input0, delegateWriteLine))
            //        {
            //            var receved = SendPipeCommandAndReceveMessage(stst, CMDS.DC_DR_StressTest, 5000);

            //            delegateWriteLine($"RemoteClientServerControl.StressTest(..)  コマンド名 \"StressTest\" 送信\r\n\tサーバーからの送信を受信:”{receved}”");

            //            stst.WriteString(wait_minitus_timestr);

            //            resultMessage = stst.ReadString(readWrteStringTimeOut);
            //            delegateWriteLine($"RemoteClientServerControl.StressTest(..)  コマンド負荷時間 \"{wait_minitus_timestr}\" 分  を送信\r\n\tサーバーからの送信を受信:”{resultMessage}”");

            //            resultMessage = stst.ReadString(-1);
            //            delegateWriteLine($"RemoteClientServerControl.StressTest(..) コマンド終了 結果を受信\r\n\tサーバーからの送信を受信:”{resultMessage}”");
            //        }
            //        else
            //        {
            //            delegateWriteLine($"▲ CheckFirstMessage(input0) の戻り値が false");
            //            return false;
            //        }
            //        ///
            //        pipeCltStream.Close();
            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        delegateWriteLine($"RemoteClientServerControl.StressTest(..)\n" +
            //            $"Hostname:{Hostname}, ClsLogonDummy:{ClsLogonDummy}, DomainName:{DomainName}, UserName:{UserName}, UserPassword:{UserPassword}" +
            //            $" PIEP接続失敗\n{ex.Message}\n{ex.InnerException}");
            //        pipeCltStream.Close();
            //        return false;
            //    }

            //}

            string serverResultTesttMsg = null;
            bool anser = false;

            // TODO: ClsLogonDummy を 書き換えた RemoteClientServerControl:StressTest
            new WithFakeAccount(DomainName, UserName, UserPassword, ClsLogon, () =>
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        anser = false;
                        return;
                    }
                    if (CheckFirstMessage(input0, delegateWriteLine))
                    {
                        var receved = SendPipeCommandAndReceveMessage(stst, CMDS.DC_DR_StressTest, 5000);

                        delegateWriteLine($"RemoteClientServerControl.StressTest(..)  コマンド名 \"StressTest\" 送信\r\n\tサーバーからの送信を受信:”{receved}”");

                        stst.WriteString(wait_minitus_timestr);

                        string resultMessage1 = stst.ReadString(readWrteStringTimeOut);
                        delegateWriteLine($"RemoteClientServerControl.StressTest(..)  コマンド負荷時間 \"{wait_minitus_timestr}\" 分  を送信\r\n\tサーバーからの送信を受信:”{resultMessage1}”");

                        serverResultTesttMsg = stst.ReadString(-1);
                        delegateWriteLine($"RemoteClientServerControl.StressTest(..) コマンド終了 結果を受信\r\n\tサーバーからの送信を受信:”{serverResultTesttMsg}”");
                    }
                    else
                    {
                        delegateWriteLine($"▲ CheckFirstMessage(input0) の戻り値が false");
                        anser = false;
                        return;
                    }
                    ///
                    pipeCltStream.Close();
                    anser = true;
                    return;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"RemoteClientServerControl.StressTest(..)\n" +
                        $"Hostname:{Hostname}, ClsLogonDummy:{ClsLogon}, DomainName:{DomainName}, UserName:{UserName}, UserPassword:{UserPassword}" +
                        $" PIEP接続失敗\n{ex.Message}\n{ex.InnerException}");
                    pipeCltStream.Close();
                    anser = false;
                    return;
                }
            });

            resultMessage = serverResultTesttMsg;
            return anser;
        }

        /// <summary>
        /// ■DR パイプコマンド "GetPipeServerJobList" DRAWREGISTサービスが保持している List<AcceptPipeCommand> ActiveCommandJobList を得ます
        /// </summary>
        /// <param name="connectClients"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public bool GetActiveSessionCommandList(out List<AcceptPipeCommand> connectClients, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

            connectClients = new List<AcceptPipeCommand>();


            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientServerControl:GetActiveSessionCommandList
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return false;
                    }
                    if (CheckFirstMessage(input0))
                    {
                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_GetActiveSessionCommandList);
                        if (writeResult == -1)
                            throw new Exception("PIPEコマンドを送信できませんでした");

                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            connectClients = reader.ReadObject<List<AcceptPipeCommand>>(); //④read
                        }

                        foreach (var a in connectClients)
                        {
                            delegateWriteLine($"■全ジョブ数 {connectClients.Count},  ServerId : {a.ServerId} , ClientUser : {a.ClientUser}  : {a.CommandName}");
                        }
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        return false;
                    }
                    ///
                    pipeCltStream.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"RemoteClientDRAWCAPTURE.GetPipeServerJobList(..)\n" +
                        $"Hostname:{Hostname}, ClsLogonDummy:{ClsLogon}, DomainName:{DomainName}, UserName:{UserName}, UserPassword:{UserPassword}" +
                        $" PIEP接続失敗\n{ex.Message}\n{ex.InnerException}");
                    pipeCltStream.Close();
                    return false;
                }

            }
        }
    }
}


