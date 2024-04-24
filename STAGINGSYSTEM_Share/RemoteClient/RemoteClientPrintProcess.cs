using System;
using SasaLib;
using System.Diagnostics;
using System.IO.Pipes;
using STAGINGSYSTEM_COMMANDS;
using SasaLibDummy;

namespace StageServerRemote
{
    /// <summary>
    /// ●リモートにて印刷させるクラス
    /// </summary>
    internal class RemoteClientPrintProcess : RMCsupport
    {

        /// <summary>
        /// 接続アカウントをセット
        /// </summary>
        string DomainName;
        string UserName;
        string UserPassword;
        bool ClsLogonDummy;
        string PipeServerName;
        string PipeName;

        /// <summary>
        /// 接続タイムアウト
        /// </summary>
        public int ClientTimeOut { get; set; } = 20000;

        public int ReadStreamStringTimeOut { get; set; } = 20000;

        public int ReadHandShakeStreamStringTimeOut { get; set; } = 10000;

        /// <summary>
        /// PIPE接続のステータス
        /// </summary>
        private bool PipeConnectionStatus { get; set; }

        /// <summary>
        /// 実行結果メッセージ
        /// </summary>
        private string AnserMessage { get;  set; }

        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="DomainName"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        internal RemoteClientPrintProcess(string ForcedDomainName, string ForcedUserName, string ForcedUserPassword, bool ForcedAccountFlag, string PipeServerName,string PipeName)
        {
            DomainName = ForcedDomainName;
            UserName = ForcedUserName;
            UserPassword = ForcedUserPassword;
            ClsLogonDummy = ForcedAccountFlag;
            this.PipeServerName = PipeServerName;
            this.PipeName = PipeName;
        }


        /// <summary>
        /// あいてサーバーの、TIFF絶対パスと印刷設定XMLを指定し印刷を行う。
        /// </summary>
        /// <param name="imageFilePath">相手サーバーのTIFFフルパス</param>
        /// <param name="PlotterSettingXML"></param>
        internal void PrintStart(string imageFilePath, string PlotterSettingXML = "")
        {
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, PipeName);

                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        AnserMessage = $"PIPE接続失敗 {ex.Message}";
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6004, $"RemoteClientPrintProcess.PrintStart(..) PIPE接続失敗。ハンドシェイクでタイムアウト {ex.Message}");
                        return;
                    }

                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);

                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6004, "最初のハンドシェイクにてタイムアウトが発生");
                        return;
                    }

                    if (CheckFirstMessage(input0))
                    {
                        int writeResult = stst.WriteString(CMDS.DR_PrintStart);
                        if (writeResult == -1)
                            throw new Exception("PIPEコマンドを送信できませんでした");

                        stst.WriteString(imageFilePath);

                        stst.WriteString(PlotterSettingXML);
                    }
                    else
                    {
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "Server could not be verified.");
                    }
                    pipeCltStream.Close();
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバー接続エラー 例外 {ex.Message}");
                }
            }
        }
    }
}