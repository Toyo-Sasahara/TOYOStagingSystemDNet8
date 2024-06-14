//using SasaLib;
//using SasaLib.PIPE;
//using SasaLibDummy;
//using StageServerRemote;
//using STAGINGSYSTEM_COMMANDS;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Configuration;
//using System.Diagnostics;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.IO.Pipes;
//using System.Linq;
//using System.Reflection.Emit;
//using System.Runtime.Versioning;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Xml.Serialization;
//using ToyoMcMfg.Staging.DataBaseConfig;
//using ToyoStageService;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

//namespace StageServerRemote
//{
//    [SupportedOSPlatform("windows")]

//    public class RemoteClientClientPreInputTICKET : RMCsupport
//    {
//        private readonly string pipename;
//        #region ●プロパティ
//        /// <summary>
//        ///
//        /// </summary>
//        internal string DomainName { get; set; }
//        /// <summary>
//        /// 
//        /// </summary>
//        internal string UserName { get; set; }
//        /// <summary>
//        /// 
//        /// </summary>
//        internal string UserPassword { get; set; }
//        /// <summary>
//        /// 
//        /// </summary>
//        internal bool ClsLogon { get; set; }
//        /// <summary>
//        /// 
//        /// </summary>
//        internal string PipeServerName { get; set; }

//        /// <summary>
//        /// 接続タイムアウト
//        /// </summary>
//        public int ClientTimeOut { get; set; } = 9500;

//        public int ReadStreamStringTimeOut { get; set; } = 10000;

//        /// <summary>
//        /// サーバーからのサーバ識別文字列を受け取るReadStremのタイムアウト
//        /// </summary>
//        public int ReadHandShakeStreamStringTimeOut { get; set; } = 10000;

//        /// <summary>
//        /// PIPE接続のステータス
//        /// </summary>
//        internal bool PipeConnectionStatus { get; private set; }

//        /// <summary>
//        /// 実行結果メッセージ
//        /// </summary>
//        internal string AnserMessage { get; private set; }
//        #endregion

//        #region ●コンストラクタ
//        /// <summary>
//        /// コンストラクタ PIPEサーバーへの接続準備も行う
//        /// </summary>
//        /// <param name="DomainName">接続強制ユーザーのドメイン</param>
//        /// <param name="UserName">接続強制ユーザー名</param>
//        /// <param name="UserPassword">接続強制ユーザーパスワード</param>
//        /// <param name="ClsLogon">接続強制を許可するスイッチこのスイッチがfalseの場合接続強制アカウントは使用されない</param>
//        /// <param name="PipeName">接続先パイプ名</param>
//        public RemoteClientClientPreInputTICKET(string DomainName,
//            string UserName,
//            string UserPassword,
//            bool ClsLogon,
//            string PipeServerName,
//            string PipeName)
//        {
//            this.DomainName = DomainName;
//            this.UserName = UserName;
//            this.UserPassword = UserPassword;
//            this.ClsLogon = ClsLogon;

//            this.PipeServerName = PipeServerName;
//            this.pipename = PipeName;

//        }
//        #endregion

//        /// <summary>
//        /// サーバーに初期入力中のチケットコードを送信する
//        /// </summary>
//        /// <param name="pipeName"></param>
//        /// <param name="ticket"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public List<ClientPreInputTICKET> AddClientPreInputTICKETCODE(ClientPreInputTICKET ticket, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null)
//                delegateWriteLine = Console.WriteLine;

//            //List<ClientPreInputTICKET> preInputTickets;

//            //using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
//            //{
//            //    try
//            //    {
//            //        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
//            //        // 待機中のサーバーへ接続
//            //        try
//            //        {
//            //            pipeCltStream.Connect(ClientTimeOut);
//            //        }
//            //        catch (Exception ex)
//            //        {
//            //            PipeConnectionStatus = false;

//            //            delegateWriteLine($"PIPEサーバーへの接続エラー {ex.Message}");
//            //            return null;
//            //        }
//            //        // サーバーからのサーバ識別文字列を受け取ります。
//            //        StreamString stst = new StreamString(pipeCltStream);
//            //        bool OperationCanceledException;
//            //        bool AggregateException;

//            //        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//            //        if (OperationCanceledException || AggregateException)
//            //        {
//            //            delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//            //            return null;
//            //        }
//            //        if (CheckFirstMessage(input0))
//            //        {
//            //            //delegateWriteLine($"ステージサーバーからの接続文字列{input0}は期待値です");
//            //            int writeResult = stst.WriteString(CMDS.DR_AddClientPreInputTICKETCODE);

//            //            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
//            //            {
//            //                writer.WriteObject(ticket);
//            //            }

//            //            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//            //            {
//            //                preInputTickets = reader.ReadObject<List<ClientPreInputTICKET>>(); // ｻｰﾊﾞｰからオブジェクト受信
//            //            }

//            //            pipeCltStream.Close();

//            //            return preInputTickets;
//            //        }
//            //        else
//            //        {
//            //            delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//            //            pipeCltStream.Close();
//            //            return null;
//            //        }
//            //        // Give the client process some time to display results before exiting.
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        PipeConnectionStatus = false;

//            //        delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//            //        //SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6001, $"PIPEサーバー接続エラー\n{ex.Message}");
//            //    }
//            //    return null;
//            //}

//            // TODO: ClsLogonDummy を 書き換えた AddClientPreInputTICKETCODE
//            List<ClientPreInputTICKET> result = null;
//            new WithFakeAccount(DomainName, UserName, UserPassword, ClsLogon, () =>
//            {
//                try
//                {
//                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
//                    // 待機中のサーバーへ接続
//                    try
//                    {
//                        pipeCltStream.Connect(ClientTimeOut);
//                    }
//                    catch (Exception ex)
//                    {
//                        PipeConnectionStatus = false;

//                        delegateWriteLine($"PIPEサーバーへの接続エラー {ex.Message}");
//                        result = null;
//                        return;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        result = null;
//                        return;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        //delegateWriteLine($"ステージサーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DR_AddClientPreInputTICKETCODE);

//                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            writer.WriteObject(ticket);


//                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                            {
//                                result = reader.ReadObject<List<ClientPreInputTICKET>>(); // ｻｰﾊﾞｰからオブジェクト受信
//                            }
//                            //pipeCltStream.Close();
//                        }
//                    }
//                    else
//                    {
//                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//                    //SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6001, $"PIPEサーバー接続エラー\n{ex.Message}");
//                }
//            });

//            return result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="TICKETCODE"></param>
//        /// <returns></returns>
//        public bool RemoveCommonApprovalWaitingTicketList(string TICKETCODE)
//        {
//            List<string> sucessRmoveTICKETs = RemovePreInputTIKECTCODEs(new List<string>() { TICKETCODE });
//            if (sucessRmoveTICKETs != null)
//            {
//                if (sucessRmoveTICKETs.Count > 0)
//                    return true;
//                else
//                    return false;
//            }
//            else
//                return false;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="TICKETCODEs"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public List<string> RemovePreInputTIKECTCODEs(List<string> TICKETCODEs, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null)
//                delegateWriteLine = Console.WriteLine;


//            try
//            {
//                if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//                //using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
//                //{
//                //    try
//                //    {
//                //        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(this.PipeServerName, pipename);
//                //        // 待機中のサーバーへ接続
//                //        try
//                //        {
//                //            pipeCltStream.Connect(ClientTimeOut);
//                //        }
//                //        catch (Exception ex)
//                //        {
//                //            PipeConnectionStatus = false;

//                //            delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
//                //            return null;
//                //        }
//                //        // サーバーからのサーバ識別文字列を受け取ります。
//                //        StreamString stst = new StreamString(pipeCltStream);
//                //        bool OperationCanceledException;
//                //        bool AggregateException;

//                //        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                //        if (OperationCanceledException || AggregateException)
//                //        {
//                //            delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                //            return null;
//                //        }
//                //        if (CheckFirstMessage(input0))
//                //        {
//                //            int writeResult = stst.WriteString(CMDS.DR_RemovePreInputTIKECTCODEs);
//                //            if (writeResult == -1)
//                //                throw new Exception("PIPEコマンドを送信できませんでした");

//                //            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
//                //            {
//                //                // サーバーに送出
//                //                writer.WriteObject(TICKETCODEs);
//                //            }

//                //            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                //            {
//                //                result = reader.ReadObject<List<string>>(); // ｻｰﾊﾞｰからオブジェクト受信
//                //            }

//                //            pipeCltStream.Close();

//                //            return result;
//                //        }
//                //        else
//                //        {
//                //            delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
//                //            pipeCltStream.Close();
//                //            return null;
//                //        }
//                //        // Give the client process some time to display results before exiting.
//                //    }
//                //    catch (Exception ex)
//                //    {
//                //        PipeConnectionStatus = false;
//                //        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
//                //    }
//                //    return null;
//                //}

//                List<string> result = null;
//                // TODO: ClsLogonDummy を 書き換えた RemoteClientClientPreInputTICKET:RemovePreInputTIKECTCODEs
//                new WithFakeAccount(DomainName, UserName, UserPassword, ClsLogon, () =>
//                {
//                    try
//                    {
//                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(this.PipeServerName, pipename);
//                        // 待機中のサーバーへ接続
//                        try
//                        {
//                            pipeCltStream.Connect(ClientTimeOut);
//                        }
//                        catch (Exception ex)
//                        {
//                            PipeConnectionStatus = false;

//                            delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
//                            result = null;
//                            return;
//                        }
//                        // サーバーからのサーバ識別文字列を受け取ります。
//                        StreamString stst = new StreamString(pipeCltStream);
//                        bool OperationCanceledException;
//                        bool AggregateException;

//                        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                        if (OperationCanceledException || AggregateException)
//                        {
//                            delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                            result = null;
//                            return;
//                        }
//                        if (CheckFirstMessage(input0))
//                        {
//                            int writeResult = stst.WriteString(CMDS.DR_RemovePreInputTIKECTCODEs);
//                            if (writeResult == -1)
//                                throw new Exception("PIPEコマンドを送信できませんでした");

//                            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
//                            {
//                                // サーバーに送出
//                                writer.WriteObject(TICKETCODEs);
//                            }

//                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                            {
//                                result = reader.ReadObject<List<string>>(); // ｻｰﾊﾞｰからオブジェクト受信
//                            }

//                            pipeCltStream.Close();

//                        }
//                        else
//                        {
//                            delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
//                            pipeCltStream.Close();
//                            result = null;
//                            return;
//                        }
//                        // Give the client process some time to display results before exiting.
//                    }
//                    catch (Exception ex)
//                    {
//                        PipeConnectionStatus = false;
//                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
//                    }
//                    result = null;
//                    return;
//                });

//                return result;
//            }
//            catch (Exception ex)
//            {
//                delegateWriteLine(ex.Message);
//                return null;
//            }
//        }


//    }
//}
