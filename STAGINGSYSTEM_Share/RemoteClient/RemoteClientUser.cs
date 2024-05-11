using System;
using System.Collections.Generic;
using System.IO;
using SasaLib;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using SasaLib.PIPE;
using ToyoMcMfg.Staging.DataBaseConfig;
using STAGINGSYSTEM_COMMANDS;
using SasaLibDummy;

namespace StageServerRemote
{
    /// <summary>
    /// ●リモートにてユーザーに関する操作を実行するクラス
    /// </summary>
    public class RemoteClientUser : RMCsupport
    {

        /// <summary>
        /// 接続アカウントをセット
        /// </summary>
        readonly string DomainName;
        readonly string UserName;
        readonly string UserPassword;
        readonly bool ClsLogonDummy;
        readonly string PipeServerName;
        readonly string PipeName;

        /// <summary>
        /// 接続タイムアウト
        /// </summary>
        public int ClientTimeOut { get; set; } = 10000;

        //public int ReadStreamStringTimeOut { get; set; } = 20000;

        public int ReadHandShakeStreamStringTimeOut { get; set; } = 10000;

        /// <summary>
        /// データベース検索結果（複数）
        /// </summary>
        public List<FieldValueSet> DBresultList { get; private set; }

        /// <summary>
        /// PIPE接続のステータス
        /// </summary>
        private bool PipeConnectionStatus { get;  set; }

        /// <summary>
        /// 実行結果メッセージ
        /// </summary>
        private string AnserMessage { get;  set; }

        /// <summary>
        /// 検索結果個数
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ForcedDomainName"></param>
        /// <param name="ForcedUserName"></param>
        /// <param name="ForcedUserPassword"></param>
        /// <param name="ForcedAccountFlag"></param>
        /// <param name="PipeName"></param>
        public RemoteClientUser(string ForcedDomainName, 
            string ForcedUserName,
            string ForcedUserPassword,
            bool ForcedAccountFlag,
            string PipeServerName,
            string PipeName)
        {
            DomainName = ForcedDomainName;
            UserName = ForcedUserName;
            UserPassword = ForcedUserPassword;
            ClsLogonDummy = ForcedAccountFlag;
            this.PipeServerName = PipeServerName;
            this.PipeName = PipeName;
        }

        /// <summary>
        /// ユーザーデータベース検索
        /// </summary>
        /// <param name="SEARCHKey"></param>
        /// <param name="DBValue"></param>
        /// <param name="SelectKeys"></param>
        public void Search(string SEARCHKey, string DBValue, List<string> SelectKeys)
        {
            if (string.IsNullOrWhiteSpace(DBValue))
            {
                return;
            }

            string keys = string.Join(",", SelectKeys);

            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, PipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        AnserMessage = $"PIPE接続失敗 {ex.Message}";
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6005, "東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                        return;
                    }

                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);

                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6004, $"RemoteClientUser.Search(..) PIPE接続失敗。ハンドシェイクでタイムアウト");
                        return;
                    }

                    if (CheckFirstMessage(input0))
                    {
                        int writeResult = stst.WriteString(CMDS.DR_UserSearch); //send
                        if (writeResult == -1)
                            throw new Exception("PIPEコマンドを送信できませんでした");

                        stst.WriteString("USERSTORE"); //send 検索するテーブル名

                        stst.WriteString(SEARCHKey); //send

                        stst.WriteString(DBValue); //send

                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(SelectKeys); //④send
                        }

                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            DBresultList = reader.ReadObject<List<FieldValueSet>>(); //⑤read
                            Count = DBresultList.Count;
                        }
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
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"パイプサーバー接続エラー 例外:{ex.Message}");
                }
            }
        }
    }

}
