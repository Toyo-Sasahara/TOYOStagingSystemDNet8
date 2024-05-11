using SasaLib;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using STAGINGSYSTEM_COMMANDS;
using SasaLibDummy;

namespace ServerControlCenterApplication
{
    internal class RemotePipeClient : RMCsupport
    {
        private readonly string pipeName;

        public delegate bool delegate_CommandHandShakeStart(NamedPipeClientStream pipeCltStream);

        bool pingOK;

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
        internal string serverHostname { get; set; }

        //bool PipeConnectionStatus;

        /// <summary>
        /// 接続タイムアウト
        /// </summary>
        public int ClientTimeOut { get; set; } = 100;

        /// <summary>
        /// PIPE接続のステータス
        /// </summary>
        public int ReadStreamStringTimeOut { get; set; } = 20000;

        public int ReadHandShakeStreamStringTimeOut { get; set; } = 10000;

        #endregion

        #region ●コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="DomainName"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="ClsLogon"></param>
        /// <param name="PipeServerName"></param>
        /// <param name="PipeName"></param>
        public RemotePipeClient(string DomainName, string UserName, string UserPassword, bool ClsLogon, string PipeServerName, string PipeName)
        {
            this.DomainName = DomainName;
            this.UserName = UserName;
            this.UserPassword = UserPassword;
            this.ClsLogon = ClsLogon;
            this.serverHostname = PipeServerName;
            this.pipeName = PipeName;


            if (CheckPing(PipeServerName) == false)
            {
                DebugConsole.Write($"Ping応答なし:{PipeServerName}\n");
                pingOK = false;
            }
            else
            {
                pingOK = true;
            }
        }
        #endregion


        ///---------------------------------------------------------------------------------------

        /// <summary>
        /// ハンドシェイクとコマンド開始
        /// </summary>
        /// <param name="CommandName"></param>
        /// <param name="delegate_ExecuteCommandMessageSendFunc"></param>
        /// <param name="WriteLine"></param>
        /// <returns></returns>
        public bool Command_ConnnectStart(string CommandName, delegate_CommandHandShakeStart delegate_ExecuteCommandMessageSendFunc, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = DebugConsole.WriteLine;

            if (pingOK == false)
                return false;
            DebugConsole.WriteLine($"接続先 {serverHostname} {pipeName} 接続文字列:{CMDS.ConnectKeyword}");

            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(serverHostname, pipeName);
                try
                {
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        ////PipeConnectionStatus = false;
                        DebugConsole.WriteLine($"※Command_ConnnectStart(..) PIPEｻｰﾊﾞｰ:{serverHostname} PIPE名:{pipeName} 実行コマンド:{CommandName} NamedPipeClientStream.Connect(..)にて例外検知 {ex.Message}");
                        return false;
                    }
                    // サーバーからのサーバ識別文字列を受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6004, $"RemotePipeClient.Command_ConnnectStart(..) PIPE接続失敗。ハンドシェイクでタイムアウト");
                        return false;
                    }
                    if (CheckFirstMessage(input0))
                    {
                        DebugConsole.WriteLine($"PIPEｻｰﾊﾞｰ:{serverHostname}PIPE名:{pipeName} からの接続文字列{input0}は期待値です");
                        stst.WriteString(CommandName);

                        //WriteLine($"≫[{DateTime.Now}]:[{serverHostname}:{pipeName}]:ｺﾏﾝﾄﾞ:{CommandName} 接続しました。回答待ち・・・");

                        bool result = delegate_ExecuteCommandMessageSendFunc(pipeCltStream);

                        //DebugConsole.WriteLine($"※PIPEｻｰﾊﾞｰ:{serverHostname} PIPE名:{pipeName} コマンド:{CommandName} 処理関数の結果 {result} です");
                    }
                    else
                    {
                        WriteLine($"※PIPEｻｰﾊﾞｰ:{serverHostname}PIPE名:{pipeName} ステージサーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return false;
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    //PipeConnectionStatus = false;

                    if (pipeCltStream.IsConnected == false)
                    {
                        WriteLine($"※PIPEｻｰﾊﾞｰ:{serverHostname}PIPE名:{pipeName} コマンド:{CommandName} 、サーバーから途中で切断されました");
                    }
                    else
                    {
                        SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6002, $"RemotePipeClient.Command_ConnnectStart(..) 、PIPEｻｰﾊﾞｰ:{serverHostname}PIPE名:{pipeName} 例外発生 {ex.Message} ");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        private bool CheckPing(string address, int Count = 1)
        {
            bool ans = false;
            Ping sender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            for (int i = 0; i < Count; i++)
            {
                try
                {
                    int timeout = 120;
                    string data = "aaaaaaaaaaaaaa";
                    byte[] buffer = Encoding.ASCII.GetBytes(data);
                    PingReply reply;
                    reply = sender.Send(address, timeout, buffer, options);

                    if (reply.Status == IPStatus.Success)
                    {
                        // Console.WriteLine("Reply from {0}: bytes={1} TTL={3}", reply.Address, reply.Buffer.Length,  reply.RoundtripTime);
                        ans = true;
                    }
                    else
                    {
                        ans = false;
                    }

                }
                catch (PingException pingex)
                {
                    var a = pingex;

                    Console.WriteLine($"CheckPingで例外発生(宛先:{address}, Count変数={Count})：" + a.InnerException);
                }

                // ping送信の間隔を取る
                if (i < Count + 1)
                {
                    System.Threading.Thread.Sleep(50);
                }
            }
            return ans;
        }

    }
}
