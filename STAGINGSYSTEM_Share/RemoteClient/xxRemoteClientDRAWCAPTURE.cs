//using SasaLib;
//using SasaLib.PIPE;
//using SasaLib.PrintConfig;
//using SasaLibDummy;
//using SharedClassLibrary;
//using STAGINGSYSTEM_COMMANDS;

////using SharedClassLibrary;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Pipes;
//using System.Runtime.Versioning;
//using System.Security.Principal;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using ToyoMcMfg.Staging.RemoteObjects;
//using ToyoStageService;

//namespace StageServerRemote
//{
//    [SupportedOSPlatform("windows")]

//    /// <summary>
//    /// DRAWCAPTUEserviceとのリモート接続クラス
//    /// </summary>
//    public class RemoteClientDRAWCAPTURE : RMCsupport
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
//        public int ClientTimeOut { get; set; } = 20000;

//        public int ReadStreamStringTimeOut { get; set; } = 20000;

//        public int ReadHandShakeStreamStringTimeOut { get; set; } = 30000;

//        /// <summary>
//        /// PIPE接続のステータス
//        /// </summary>
//        public bool PipeConnectionStatus { get; private set; }

//        /// <summary>
//        /// 実行結果メッセージ
//        /// </summary>
//        internal string AnserMessage { get; private set; }
//        #endregion

//        #region ●コンストラクタ
//        /// <summary>
//        /// コンストラクタ
//        /// </summary>
//        /// <param name="DomainName"></param>
//        /// <param name="UserName"></param>
//        /// <param name="UserPassword"></param>
//        /// <param name="ClsLogonDummy"></param>
//        /// <param name="PipeServerName"></param>
//        /// <param name="PipeName"></param>
//        public RemoteClientDRAWCAPTURE(string DomainName,
//            string UserName,
//            string UserPassword,
//            bool ClsLogonDummy,
//            string PipeServerName,
//            string PipeName)
//        {
//            this.DomainName = DomainName;
//            this.UserName = UserName;
//            this.UserPassword = UserPassword;
//            this.ClsLogon = ClsLogonDummy;

//            this.PipeServerName = PipeServerName;
//            this.pipename = PipeName;
//        }
//        #endregion

//        /// <summary>
//        /// GetCommitPrinterInfo
//        /// </summary>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public List<PrinterInfo> GetCommitPrinterInfo(SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:GetCommitPrinterInfo
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
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

//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(..)にて例外発生 {ex.Message}");
//                        return null;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        int writeResult = stst.WriteString(CMDS.DC_GetCommitPrinterInfo);

//                        // クライアントから送られてきた 検索結果で出力するカラム名のListを取得す
//                        object nameAndAlias;
//                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            nameAndAlias = reader.ReadObject<object>();
//                        }
//                        pipeCltStream.Close();

//                        return (List<PrinterInfo>)nameAndAlias;
//                    }
//                    else
//                    {
//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(..)　PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return null;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(..)　例外発生{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"RemoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(..)　例外発生 {ex.Message}");
//                }
//                return null;
//            }


//        }

//        /// <summary>
//        /// プリンタコンフィグファイルの<PrinterName>と対応する<PrinterShortCutName>を得る
//        /// </summary>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public List<KeyValuePair<string, string>> GetCommitPrinterShortCutName(SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWREGIST:GetCommitPrinterShortCutName
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
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

//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(..)にて例外発生 {ex.Message}");
//                        return null;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ステージサーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_GetCommitPrinterShortCutName);


//                        // クライアントから送られてきた 検索結果で出力するカラム名のListを取得す
//                        List<KeyValuePair<string, string>> nameAndAlias = new List<KeyValuePair<string, string>>();
//                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            nameAndAlias = reader.ReadObject<List<KeyValuePair<string, string>>>(Verbose: true);

//                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"printeInfos.Count ={nameAndAlias.Count}件あります");

//                            foreach (var x in nameAndAlias)
//                            {
//                                delegateWriteLine($"ｽﾃｰｼﾞｻｰﾊﾞｰ{PipeServerName}が準備しているプリンタ x ={x.Key} {x.Value}");
//                            }
//                        }
//                        pipeCltStream.Close();

//                        return nameAndAlias;
//                    }
//                    else
//                    {
//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(..)　PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return null;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(..)　例外発生{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"RemoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(..)　例外発生 {ex.Message}");
//                }
//                return null;
//            }


//        }

//        public List<KeyValuePair<string, string>> GetCommitPrinterNameAndAlias(SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:GetCommitPrinterNameAndAlias
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
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

//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(..)にて例外発生 {ex.Message}");
//                        return null;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ステージサーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_GetCommitPrinterNameAndAlias);


//                        // クライアントから送られてきた 検索結果で出力するカラム名のListを取得す
//                        List<KeyValuePair<string, string>> nameAndAlias = new List<KeyValuePair<string, string>>();
//                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            nameAndAlias = reader.ReadObject<List<KeyValuePair<string, string>>>();

//                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"printeInfos.Count ={nameAndAlias.Count}件あります");

//                            foreach (var x in nameAndAlias)
//                            {
//                                delegateWriteLine($"ｽﾃｰｼﾞｻｰﾊﾞｰ{PipeServerName}が準備しているプリンタ x ={x.Key} {x.Value}");
//                            }
//                        }
//                        pipeCltStream.Close();

//                        return nameAndAlias;
//                    }
//                    else
//                    {
//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(..)　PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return null;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(..)　例外発生{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"RemoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(..)　例外発生 {ex.Message}");
//                }
//                return null;
//            }


//        }

//        public List<KeyValuePair<string, bool>> GetCommitPrinterIsFailStatus(SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            //using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
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

//            //            delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(..)にて例外発生 {ex.Message}");
//            //            return null;
//            //        }
//            //        // サーバーからのサーバ識別文字列を受け取ります。
//            //        StreamString stst = new StreamString(pipeCltStream);
//            //        //string input0 = ss.ReadString();
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
//            //            //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ステージサーバーからの接続文字列{input0}は期待値です");
//            //            int writeResult = stst.WriteString(CMDS.DC_GetCommitPrinterIsFailStatus);


//            //            // クライアントから送られてきた 検索結果で出力するカラム名のListを取得す
//            //            List<KeyValuePair<string, bool>> nameAndAlias = new List<KeyValuePair<string, bool>>();
//            //            using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//            //            {
//            //                nameAndAlias = reader.ReadObject<List<KeyValuePair<string, bool>>>();

//            //                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"printeInfos.Count ={nameAndAlias.Count}件あります");

//            //                foreach (var x in nameAndAlias)
//            //                {
//            //                    delegateWriteLine($"ｽﾃｰｼﾞｻｰﾊﾞｰ{PipeServerName}が準備しているプリンタの故障状態 x ={x.Key} {x.Value}");
//            //                }
//            //            }
//            //            pipeCltStream.Close();

//            //            return nameAndAlias;
//            //        }
//            //        else
//            //        {
//            //            delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(..)　PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//            //            pipeCltStream.Close();
//            //            return null;
//            //        }
//            //        // Give the client process some time to display results before exiting.
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        PipeConnectionStatus = false;

//            //        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(..)　例外発生{ex.Message}");
//            //        SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"RemoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(..)　例外発生 {ex.Message}");
//            //    }
//            //    return null;
//            //}

//            //TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:GetCommitPrinterIsFailStatus
//            // クライアントから送られてきた 検索結果で出力するカラム名のListを取得す
//            List<KeyValuePair<string, bool>> nameAndAlias = new List<KeyValuePair<string, bool>>();

//            new WithFakeAccount(DomainName, UserName, UserPassword, ClsLogon, () =>
//            {
//                Console.WriteLine("During impersonation: " + WindowsIdentity.GetCurrent().Name);

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

//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(..)にて例外発生 {ex.Message}");
//                        nameAndAlias = null;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        nameAndAlias = null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ステージサーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_GetCommitPrinterIsFailStatus);


//                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            nameAndAlias = reader.ReadObject<List<KeyValuePair<string, bool>>>();

//                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"printeInfos.Count ={nameAndAlias.Count}件あります");

//                            foreach (var x in nameAndAlias)
//                            {
//                                delegateWriteLine($"ｽﾃｰｼﾞｻｰﾊﾞｰ{PipeServerName}が準備しているプリンタの故障状態 x ={x.Key} {x.Value}");
//                            }
//                        }
//                        pipeCltStream.Close();

//                    }
//                    else
//                    {
//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(..)　PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        nameAndAlias = null;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(..)　例外発生{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"RemoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(..)　例外発生 {ex.Message}");
//                }

//            });

//            return nameAndAlias;
//        }


//        public List<KeyValuePair<string, string>> GetCommitPrinterSettingFromPaperSize(SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:GetCommitPrinterSettingFromPaperSize
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
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

//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(..)にて例外発生 {ex.Message}");
//                        return null;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ステージサーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_GetCommitPrinterSettingFromPaperSize);


//                        // クライアントから送られてきた 検索結果で出力するカラム名のListを取得す
//                        List<KeyValuePair<string, string>> resultData = new List<KeyValuePair<string, string>>();
//                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            resultData = reader.ReadObject<List<KeyValuePair<string, string>>>();

//                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"printeInfos.Count ={resultData.Count}件あります");

//                            foreach (var x in resultData)
//                            {
//                                delegateWriteLine($"ｽﾃｰｼﾞｻｰﾊﾞｰ{PipeServerName}が準備しているプリンタ x ={x.Key} {x.Value}");
//                            }
//                        }
//                        pipeCltStream.Close();

//                        return resultData;
//                    }
//                    else
//                    {
//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(..)　PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return null;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(..)　例外発生{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"RemoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(..)　例外発生 {ex.Message}");
//                }
//                return null;
//            }


//        }

//        /// <summary>
//        /// ■図面番号から図面種類を問い合わせる "CHECK_DRAWING_TYPE"
//        /// </summary>
//        /// <param name="Hostname"></param>
//        /// <param name="PARTNUMBER"></param>
//        /// <returns></returns>
//        public string CHECK_DRAWING_TYPE(string Hostname, string PARTNUMBER, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:CHECK_DRAWING_TYPE
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
//            {
//                try
//                {
//                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
//                    // 待機中のサーバーへ接続
//                    try
//                    {
//                        pipeCltStream.Connect(ClientTimeOut);
//                    }
//                    catch (Exception ex)
//                    {
//                        PipeConnectionStatus = false;

//                        delegateWriteLine($"※CHECK_DRAWING_TYPE(..) PIPEサーバーへの接続エラー {ex.Message} {ex.InnerException}");
//                        return "Error";
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        delegateWriteLine($"ステージサーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_CHECK_DRAWING_TYPE);
//                        stst.WriteString(PARTNUMBER);

//                        //string DrawingType = ss.ReadString();
//                        string DrawingType = stst.ReadString(ReadStreamStringTimeOut, null);

//                        pipeCltStream.Close();

//                        return DrawingType;
//                    }
//                    else
//                    {
//                        delegateWriteLine($"※CHECK_DRAWING_TYPE(..) PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return "エラー";
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"※CHECK_DRAWING_TYPE(..) PIPEサーバー接続エラー{ex.Message}");
//                }
//                return "エラー";
//            }

//        }

//        /// <summary>
//        /// ■コミット受付サーバーの受付停止時のメッセージを得る "CommitRecepitonState"
//        /// </summary>
//        /// <param name="Hostname"></param>
//        /// <returns></returns>
//        public string CommitRecepitonState(string Hostname, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            //using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy, debugConsoleMsg: false))
//            //using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
//            //{
//            //    try
//            //    {
//            //        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
//            //        // 待機中のサーバーへ接続
//            //        try
//            //        {
//            //            pipeCltStream.Connect(ClientTimeOut);
//            //        }
//            //        catch (Exception ex)
//            //        {
//            //            PipeConnectionStatus = false;

//            //            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへの接続エラー {ex.Message}");
//            //            delegateWriteLine($"PIPEサーバーへの接続エラー {ex.Message} RemoteClientDRAWCAPTURE.CommitRecepitonState(..)");
//            //            string msg;
//            //            if (ClsLogonDummy)
//            //                msg = $"コミット受付サーバーは停止しているようです。\nサーバー{Hostname}の ネットワークパイプ:{pipename} に接続できません\nRemoteClientDRAWCAPTURE.CommitRecepitonState(..)\nClsLogonDummy:{ClsLogonDummy} DomainName:{DomainName} UserName:{UserName} UserPassword:{UserPassword} 例外:{ex.Message}";
//            //            else
//            //                msg = $"コミット受付サーバーは停止しているようです。\nサーバー{Hostname}の ネットワークパイプ:{pipename} に接続できません\nRemoteClientDRAWCAPTURE.CommitRecepitonState(..)\n例外:{ex.Message}";

//            //            return msg;
//            //        }
//            //        // サーバーからのサーバ識別文字列を受け取ります。
//            //        StreamString stst = new StreamString(pipeCltStream);
//            //        //string input0 = ss.ReadString();
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
//            //            //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ステージサーバーからの接続文字列{input0}は期待値です");


//            //            int writeResult = stst.WriteString(CMDS.DC_CommitRecepitonState,timeoutmsec:15000);

//            //            if (writeResult <= 0)
//            //                throw new Exception("PIPEコマンドを送信できませんでした");

//            //            //string CommitRecepitonState = ss.ReadString();
//            //            string CommitRecepitonState = stst.ReadString(ReadStreamStringTimeOut, null);

//            //            pipeCltStream.Close();


//            //            return CommitRecepitonState;
//            //        }
//            //        else
//            //        {
//            //            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//            //            delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//            //            pipeCltStream.Close();
//            //            return $"ステージサーバーからの接続文字列{input0}が期待と違います";
//            //        }
//            //        // Give the client process some time to display results before exiting.
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        PipeConnectionStatus = false;
//            //        delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//            //        SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");
//            //    }
//            //    return $"PIPEサーバー接続エラー";
//            //}

//            // TODO: ClsLogonDummy を 書き換えた CommitRecepitonState
//            string output = null;
//            new WithFakeAccount(DomainName, UserName, UserPassword, ClsLogon, () =>
//            {
//                output = _bbb();
//            });

//            return output;

//            string _bbb()
//            {
//                try
//                {
//                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
//                    // 待機中のサーバーへ接続
//                    try
//                    {
//                        pipeCltStream.Connect(ClientTimeOut);
//                    }
//                    catch (Exception ex)
//                    {
//                        PipeConnectionStatus = false;

//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへの接続エラー {ex.Message}");
//                        delegateWriteLine($"PIPEサーバーへの接続エラー {ex.Message} RemoteClientDRAWCAPTURE.CommitRecepitonState(..)");
//                        string msg;
//                        if (ClsLogon)
//                            msg = $"コミット受付サーバーは停止しているようです。\nサーバー{Hostname}の ネットワークパイプ:{pipename} に接続できません\nRemoteClientDRAWCAPTURE.CommitRecepitonState(..)\nClsLogonDummy:{ClsLogon} DomainName:{DomainName} UserName:{UserName} UserPassword:{UserPassword} 例外:{ex.Message}";
//                        else
//                            msg = $"コミット受付サーバーは停止しているようです。\nサーバー{Hostname}の ネットワークパイプ:{pipename} に接続できません\nRemoteClientDRAWCAPTURE.CommitRecepitonState(..)\n例外:{ex.Message}";

//                        return msg;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }

//                    if (CheckFirstMessage(input0))
//                    {
//                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ステージサーバーからの接続文字列{input0}は期待値です");


//                        int writeResult = stst.WriteString(CMDS.DC_CommitRecepitonState, timeoutmsec: 15000);

//                        if (writeResult <= 0)
//                            throw new Exception("PIPEコマンドを送信できませんでした");

//                        //string CommitRecepitonState = ss.ReadString();
//                        string CommitRecepitonState = stst.ReadString(ReadStreamStringTimeOut, null);

//                        pipeCltStream.Close();


//                        return CommitRecepitonState;
//                    }
//                    else
//                    {
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return $"ステージサーバーからの接続文字列{input0}が期待と違います";
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;
//                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");
//                }
//                return $"PIPEサーバー接続エラー";
//            }

//        }


//        /// <summary>
//        /// ■ファイルを送る
//        /// </summary>
//        /// <param name="ServerDistFullFileName"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public bool FileSend(string LocalSourceFullFileName, string ServerDistFullFileName, out string resultMsg, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            System.IO.FileStream sourceFs = null;

//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:FileSend
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
//            {
//                try
//                {
//                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);

//                    sourceFs = new FileStream(LocalSourceFullFileName, FileMode.Open, FileAccess.Read);

//                    // 待機中のサーバーへ接続
//                    try
//                    {
//                        pipeCltStream.Connect(ClientTimeOut);
//                    }
//                    catch (Exception ex)
//                    {
//                        PipeConnectionStatus = false;

//                        delegateWriteLine($"PIPEサーバーへの接続エラー {ex.Message} RemoteClientDRAWCAPTURE.FileSend(..)");
//                        resultMsg = $"コミット受付サーバーは停止しているようです。\nサーバー{PipeServerName}の ネットワークパイプ:{pipename} に接続できません\nRemoteClientDRAWCAPTURE.FileSend(..)";
//                        return false;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        resultMsg = $"最初のハンドシェイクにてタイムアウトが発生";
//                        return false;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        delegateWriteLine($"ステージサーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl); // [■#1 Client -> Server String]
//                        stst.WriteString(CMDS.DC_ServerControl_FileSend); // [■#2 Client -> Server String]

//                        //string result3 = ss.ReadString(); // [■#3 Client <- Server String]
//                        string result3 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#3 Client <- Server String]
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"result#3 {result3}"); delegateWriteLine($"result#3 {result3}");

//                        stst.WriteString(ServerDistFullFileName); // [■#4 Client->Server String] サーバー側で作成するFullFileName

//                        //string result5 = ss.ReadString(); // [■#5 Client <- Server String]
//                        string result5 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#5 Client <- Server String]
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"result#5 {result5}"); delegateWriteLine($"result#5 {result5}");

//                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            writer.WriteObject(sourceFs.Length); // [■#6 Client -> Server Data] ファイルデータ
//                        }

//                        //string result7 = ss.ReadString(); // [■#7 Client <- Server String] 
//                        string result7 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#7 Client <- Server String] 
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"result#7 {result7}"); delegateWriteLine($"result#7 {result7}");


//                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            byte[] bytearry = new byte[sourceFs.Length];
//                            bytearry = SasaLib.StreamExtensions.StreamToBytes(sourceFs);
//                            writer.WriteObject(bytearry); // [■#8 Client -> Server Data] 
//                        }

//                        //string result9 = ss.ReadString(); // [■#9 Client <- Server String] 
//                        string result9 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#9 Client <- Server String] 
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"result#9 {result9}"); delegateWriteLine($"result#9 {result9}");

//                        //string result10 = ss.ReadString();// [■#10 Client <- Server String] 
//                        string result10 = stst.ReadString(ReadStreamStringTimeOut, null);// [■#10 Client <- Server String] 
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"result#10 {result10}"); delegateWriteLine($"result#10 {result10}");

//                        //string result11 = ss.ReadString();// [■#11 Client <- Server String Last] 
//                        string result11 = stst.ReadString(ReadStreamStringTimeOut, null);// [■#11 Client <- Server String Last] 
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"result#11 {result11}"); delegateWriteLine($"result#11 {result11}");

//                        sourceFs.Close();

//                        pipeCltStream.Close();

//                        resultMsg = $"{result11}"; // [■#11 Client <- Server String Last] 
//                        return true;
//                    }
//                    else
//                    {
//                        sourceFs.Close();

//                        pipeCltStream.Close();

//                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        resultMsg = $"ステージサーバーからの接続文字列{input0}が期待と違います";
//                        return false;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    if (sourceFs != null)
//                        sourceFs.Close();


//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");

//                    resultMsg = $"PIPEサーバー接続エラー\n{ex.Message}";
//                    return false;
//                }
//            }

//        }

//        /// <summary>
//        /// ■ファイルを受信する
//        /// </summary>
//        /// <param name="ServerDistFullFileName"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public bool FileRecv(string ServerSourceFullFileName, string LocalDistFullFileName, out string resultMsg, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            System.IO.FileStream sourceFs = null;

//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:FileRecv
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
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

//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) PIPEサーバーへの接続エラー {ex.Message}");
//                        resultMsg = $"コミット受付サーバーは停止しているようです。\nサーバー{PipeServerName}の ネットワークパイプ:{pipename} に接続できません";
//                        return false;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    // string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        resultMsg = $"最初のハンドシェイクにてタイムアウトが発生";
//                        return false;
//                    }

//                    if (CheckFirstMessage(input0))
//                    {
//                        delegateWriteLine($"■RemoteClientDRAWCAPTURE.FileRecv(..) PIPEサーバーからの接続文字列 \"{input0}\" は期待値です");
//                        int writeResultCommandName1 = stst.WriteString(CMDS.DC_DR_SW_ServerControl); // [■#1 Client -> Server String]
//                        if (writeResultCommandName1 == -1)
//                            throw new Exception($"サーバーへ \"{CMDS.DC_DR_SW_ServerControl}\" の送信に失敗しました");

//                        int writeResultCommandName2 = stst.WriteString(CMDS.DC_ServerControl_FileRecv); // [■#2 Client -> Server String]
//                        if (writeResultCommandName2 == -1)
//                            throw new Exception($"サーバーへ \"{CMDS.DC_ServerControl_FileRecv}\" の送信に失敗しました");

//                        string result3 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#3 Client <- Server String]
//                        delegateWriteLine($"■RemoteClientDRAWCAPTURE.FileRecv(..) Server result #3 \"{result3}\"");

//                        int writeResultServerSourceFullFileName = stst.WriteString(ServerSourceFullFileName); // [■#4 Client->Server String] サーバーソースファイル名
//                        if (writeResultServerSourceFullFileName == -1)
//                            throw new Exception($"サーバーへ \"{ServerSourceFullFileName}\" の送信に失敗しました");

//                        //string result5 = ss.ReadString(); // [■#5 Client <- Server String]
//                        string result5 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#5 Client <- Server String]
//                        delegateWriteLine($"■RemoteClientDRAWCAPTURE.FileRecv(..) Server result #5 \"{result5}\"");

//                        long DataSize = 0;
//                        try
//                        {
//                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                            {
//                                DataSize = reader.ReadObject<long>(); // [■#6 Client <- Server Data] データサイズ受信

//                                if (DataSize > 0)
//                                {
//                                    delegateWriteLine($"■RemoteClientDRAWCAPTURE.FileRecv(..) Server result #6 データサイズ受信 \"{DataSize}\"");
//                                }
//                                else
//                                {
//                                    delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) Server result #6 データサイズ受信 \"{DataSize}\" のため、ファイルが存在しないか その他エラーの可能性があります。");
//                                }
//                            }

//                        }
//                        catch (Exception ex)
//                        {
//                            delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) 先方から切断されました.");
//                            SasaLib.Eventlog.Log.WriteEntry("ToyoRMDRAWCAPTUREserviceControl", EventLogEntryType.Error, 6000, $"【RemoteClientDRAWCAPTURE.FileRecv(..) 切断されました】 {ex.Message} ", false);
//                            resultMsg = "切断されました";
//                            return false;
//                        }

//                        int writeResultSendByData = stst.WriteString($"[3.OK Data Size:{DataSize} byte] Pleas Send Byte[] Data."); // [■#7 Client -> Server String]  データサイズ了解
//                        if (writeResultSendByData == -1)
//                        {
//                            throw new Exception($"サーバーからデータサイズ{DataSize}の受信を受け取った旨を送信しようとして失敗しました");
//                        }

//                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            byte[] buf = new byte[DataSize];
//                            buf = reader.ReadObject<byte[]>();  // [■#8 Client <- Server Data] サーバー側 ソースファイルバイトデータ


//                            if (buf != null)
//                            {
//                                stst.WriteString($"[4.OK. Recevice Data. {buf.Length} byte]"); // [■#9 Client-> Server String]

//                                try
//                                {
//                                    File.WriteAllBytes(LocalDistFullFileName, buf);

//                                    resultMsg = $"書出し完了 {LocalDistFullFileName} , {buf.Length}";


//                                    // result5内の文字列を使いファイルのﾀｲﾑｽﾀﾝﾌﾟを解析
//                                    DateTime createTime;
//                                    DateTime lastWrteTime;
//                                    if (GetTimeStampFromReceveMessageTime(result5, out createTime, out lastWrteTime))
//                                    {
//                                        try
//                                        {
//                                            System.IO.File.SetCreationTime(LocalDistFullFileName, createTime);
//                                            System.IO.File.SetLastWriteTime(LocalDistFullFileName, lastWrteTime);
//                                        }
//                                        catch (Exception ex)
//                                        {
//                                            SasaLib.Eventlog.Log.WriteEntry("ToyoRMDRAWCAPTUREserviceControl", EventLogEntryType.Error, 6000, $"【RemoteClientDRAWCAPTURE.FileRecv(..)】 ﾀｲﾑｽﾀﾝﾌﾟ設定失敗 \"{LocalDistFullFileName}\" , 作成日時:{createTime.ToString()} 更新日時:{lastWrteTime} {ex.Message.ToString()} ", false);
//                                        }
//                                    }

//                                    delegateWriteLine($"■RemoteClientDRAWCAPTURE.FileRecv(..) resultMsg {resultMsg}");

//                                    return true;
//                                }
//                                catch (Exception ex)
//                                {
//                                    resultMsg = $"{ex.Message}";
//                                    delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) resultMsg {resultMsg}");
//                                    SasaLib.Eventlog.Log.WriteEntry("ToyoRMDRAWCAPTUREserviceControl", EventLogEntryType.Error, 6000, $"【RemoteClientDRAWCAPTURE.FileRecv(..)】 {ex.Message} ", false);
//                                    return false;
//                                }
//                            }
//                            else
//                            {
//                                resultMsg = $"書出し失敗";
//                                delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) resultMsg {resultMsg}");
//                                SasaLib.Eventlog.Log.WriteEntry("ToyoRMDRAWCAPTUREserviceControl", EventLogEntryType.Error, 6000, $"【RemoteClientDRAWCAPTURE.FileRecv(..)】 {LocalDistFullFileName} を作成・更新に失敗した", false);
//                                return false;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        if (sourceFs != null)
//                            sourceFs.Close();

//                        pipeCltStream.Close();

//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) PIPEサーバーからの接続文字列 \"{input0}\" は期待と違います");
//                        resultMsg = $"ステージサーバーからの接続文字列{input0}が期待と違います";
//                        return false;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {

//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) PIPEサーバー接続エラー\n{ex.Message} {ex.InnerException}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"【RemoteClientDRAWCAPTURE.FileRecv(..)】PIPEサーバー接続エラー\n{ex.Message}");

//                    resultMsg = $"PIPEサーバー接続エラー\n{ex.Message}";
//                    return false;
//                }
//            }
//        }

//        /// <summary>
//        /// ■
//        /// </summary>
//        /// <param name="ServerSourceFolderNaem"></param>
//        /// <param name="SearchPath"></param>
//        /// <param name="files"></param>
//        /// <param name="resultMsg"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public bool GetFileList(string ServerSourceFolderNaem, string SearchPath, out List<string> files, out string resultMsg, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            System.IO.FileStream sourceFs = null;

//            //using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
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

//            //            delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetFiles(..) PIPEサーバーへの接続エラー {ex.Message} RemoteClientDRAWCAPTURE.GetFileList(..)");
//            //            resultMsg = $"コミット受付サーバーは停止しているようです。\nサーバー{PipeServerName}の ネットワークパイプ:{pipename} に接続できません\nRemoteClientDRAWCAPTURE.GetFileList(..)";
//            //            files = null;
//            //            return false;
//            //        }
//            //        // サーバーからのサーバ識別文字列を受け取ります。
//            //        StreamString stst = new StreamString(pipeCltStream);
//            //        //string input0 = ss.ReadString();
//            //        bool OperationCanceledException;
//            //        bool AggregateException;

//            //        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//            //        if (OperationCanceledException || AggregateException)
//            //        {
//            //            delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//            //            resultMsg = $"最初のハンドシェイクにてタイムアウトが発生";
//            //            files = null;
//            //            return false;
//            //        }
//            //        if (CheckFirstMessage(input0))
//            //        {
//            //            delegateWriteLine($"■RemoteClientDRAWCAPTURE.GetFileList(..) PIPEサーバーからの接続文字列 \"{input0}\" は期待値です");
//            //            int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl); // [■#1 Client -> Server String]
//            //            stst.WriteString(CMDS.DC_ServerControl_GetFileList); // [■#2 Client -> Server String]

//            //            //string result3 = ss.ReadString(); // [■#3 Client <- Server String]
//            //            string result3 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#3 Client <- Server String]
//            //            delegateWriteLine($"■RemoteClientDRAWCAPTURE.GetFileList(..) Server result #3 \"{result3}\"");

//            //            stst.WriteString(ServerSourceFolderNaem); // [■#4 Client->Server String] サーバーソースフォルダ名名

//            //            //string result5 = ss.ReadString(); // [■#5 Client <- Server String]
//            //            string result5 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#5 Client <- Server String]
//            //            delegateWriteLine($"■RemoteClientDRAWCAPTURE.GetFileList(..) Server result #5 \"{result5}\"");

//            //            stst.WriteString(SearchPath); // [■#4 Client->Server String] 検索パス

//            //            // 
//            //            List<string> filelists = new List<string>();

//            //            using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//            //            {
//            //                files = reader.ReadObject<List<string>>();

//            //                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"printeInfos.Count ={filelists.Count}件あります");

//            //                foreach (var x in filelists)
//            //                {
//            //                    delegateWriteLine($"{x}");
//            //                }
//            //            }
//            //        }
//            //        else if (input0 == null)
//            //        {
//            //            delegateWriteLine($"コミットサーバーに接続できませんでした(タイムアウト)");
//            //            resultMsg = $"コミットサーバーに接続できませんでした(タイムアウト)";
//            //            files = null;
//            //            return false;
//            //        }
//            //        else
//            //        {
//            //            sourceFs.Close();

//            //            pipeCltStream.Close();

//            //            delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) PIPEサーバーからの接続文字列 \"{input0}\" は期待と違います");
//            //            resultMsg = $"ステージサーバーからの接続文字列{input0}が期待と違います";
//            //            files = null;

//            //            return false;
//            //        }
//            //        // Give the client process some time to display results before exiting.
//            //    }
//            //    catch (Exception ex)
//            //    {

//            //        PipeConnectionStatus = false;

//            //        delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) PIPEサーバー接続エラー\n{ex.Message}");
//            //        SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"【RemoteClientDRAWCAPTURE.FileRecv(..)】PIPEサーバー接続エラー\n{ex.Message}");

//            //        files = null;

//            //        resultMsg = $"PIPEサーバー接続エラー\n{ex.Message}";
//            //        return false;
//            //    }
//            //}
//            //resultMsg = null;
//            //return true;

//            string resultMsg2 = null;
//            List<string> files2 = null;
//            bool anser = false;

//            // TODO: ClsLogonDummy を 書き換えた RemoteClientDRAWCAPTURE:GetFileList
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

//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.GetFiles(..) PIPEサーバーへの接続エラー {ex.Message} RemoteClientDRAWCAPTURE.GetFileList(..)");
//                        resultMsg2 = $"コミット受付サーバーは停止しているようです。\nサーバー{PipeServerName}の ネットワークパイプ:{pipename} に接続できません\nRemoteClientDRAWCAPTURE.GetFileList(..)";
//                        files2 = null;
//                        anser = false;
//                        return;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        resultMsg2 = $"最初のハンドシェイクにてタイムアウトが発生";
//                        files2 = null;
//                        anser = false;
//                        return;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        delegateWriteLine($"■RemoteClientDRAWCAPTURE.GetFileList(..) PIPEサーバーからの接続文字列 \"{input0}\" は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl); // [■#1 Client -> Server String]
//                        stst.WriteString(CMDS.DC_ServerControl_GetFileList); // [■#2 Client -> Server String]

//                        //string result3 = ss.ReadString(); // [■#3 Client <- Server String]
//                        string result3 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#3 Client <- Server String]
//                        delegateWriteLine($"■RemoteClientDRAWCAPTURE.GetFileList(..) Server result #3 \"{result3}\"");

//                        stst.WriteString(ServerSourceFolderNaem); // [■#4 Client->Server String] サーバーソースフォルダ名名

//                        //string result5 = ss.ReadString(); // [■#5 Client <- Server String]
//                        string result5 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#5 Client <- Server String]
//                        delegateWriteLine($"■RemoteClientDRAWCAPTURE.GetFileList(..) Server result #5 \"{result5}\"");

//                        stst.WriteString(SearchPath); // [■#4 Client->Server String] 検索パス

//                        // 
//                        List<string> filelists = new List<string>();

//                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            files2 = reader.ReadObject<List<string>>();

//                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"printeInfos.Count ={filelists.Count}件あります");

//                            foreach (var x in filelists)
//                            {
//                                delegateWriteLine($"{x}");
//                            }

//                            anser = true;
//                        }
//                    }
//                    else if (input0 == null)
//                    {
//                        delegateWriteLine($"コミットサーバーに接続できませんでした(タイムアウト)");
//                        resultMsg2 = $"コミットサーバーに接続できませんでした(タイムアウト)";
//                        files2 = null;
//                        anser = false;
//                        return;
//                    }
//                    else
//                    {
//                        sourceFs.Close();

//                        pipeCltStream.Close();

//                        delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) PIPEサーバーからの接続文字列 \"{input0}\" は期待と違います");
//                        resultMsg2 = $"ステージサーバーからの接続文字列{input0}が期待と違います";
//                        files2 = null;
//                        anser = false;
//                        return;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {

//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"※RemoteClientDRAWCAPTURE.FileRecv(..) PIPEサーバー接続エラー\n{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"【RemoteClientDRAWCAPTURE.FileRecv(..)】PIPEサーバー接続エラー\n{ex.Message}");

//                    files2 = null;
//                    resultMsg2 = $"PIPEサーバー接続エラー\n{ex.Message}";
//                    anser = false;

//                    return;
//                }
//            });

//            resultMsg = resultMsg2;
//            files = files2;

//            return anser;
//        }

//        /// <summary>
//        /// ■
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="createTime"></param>
//        /// <param name="lastWrteTIme"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        bool GetTimeStampFromReceveMessageTime(string msg, out DateTime createTime, out DateTime lastWrteTIme, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            string format = "yyyyMMdd_HHmmss";

//            createTime = DateTime.MinValue;
//            lastWrteTIme = DateTime.MinValue;

//            try
//            {
//                string strCtime = SasaLib.StringParsing.SpecifiedStringExtraction(msg, "CTime");
//                if (strCtime != null)
//                {
//                    createTime = DateTime.ParseExact(strCtime, format, null);
//                }
//                else
//                {
//                    return false;
//                }

//                string strMtime = SasaLib.StringParsing.SpecifiedStringExtraction(msg, "MTime");
//                if (strMtime != null)
//                {
//                    lastWrteTIme = DateTime.ParseExact(strMtime, format, null);
//                }
//                else
//                {
//                    return false;
//                }

//                return true;
//            }
//            catch (Exception ex)
//            {
//                delegateWriteLine($"{ex.Message}");
//                return false;
//            }
//        }


//        /// <summary>
//        /// ■バックアップ実行　"ServerControl" "DATABASE_BACKUP"
//        /// </summary>
//        /// <param name="DistnationFolder"></param>
//        /// <returns></returns>
//        public async Task<List<string>> DatabaseBackup(string DistnationFolder, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;
//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:DatabaseBackup
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
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

//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへの接続エラー {ex.Message}");
//                        delegateWriteLine($"PIPEサーバーへの接続エラー {ex.Message}");
//                        return null;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        // SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"サーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl);
//                        stst.WriteString(CMDS.DC_ServerControl_DATABASE_BACKUP);
//                        stst.WriteString(DistnationFolder); // 保存先フォルダ名送信。バックアップ開始

//                        string Anser1 = await Task.Factory.StartNew(() => stst.ReadString(ReadStreamStringTimeOut, null));

//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"■DBバックアップ結果表示 {Anser1}");

//                        string Anser2 = await Task.Factory.StartNew(() => stst.ReadString(ReadStreamStringTimeOut, null));

//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"■ファイルストアバックアップ結果表示 {Anser2}");


//                        pipeCltStream.Close();
//                        List<string> AnserMessage = new List<string>() { Anser1, Anser2 };

//                        return AnserMessage;
//                    }
//                    else
//                    {
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return (new List<string>() { "Error", "サーバーからの接続文字列{input0}が期待と違います" });
//                    }
//                }

//                // Give the client process some time to display results before exiting.
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");
//                }
//                return (new List<string>() { "Error", $"PIPEサーバー接続エラー" });
//            }

//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="DistnationFolder"></param>
//        /// <param name="createTime"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public async Task<List<string>> DatabaseRestore(string DistnationFolder, DateTime createTime, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;
//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:DatabaseRestore
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
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

//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへの接続エラー {ex.Message}");
//                        delegateWriteLine($"PIPEサーバーへの接続エラー {ex.Message}");
//                        return null;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        // SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"サーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl);
//                        stst.WriteString(CMDS.DC_ServerControl_DATABASE_RESTORE);
//                        stst.WriteString(DistnationFolder);
//                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            writer.WriteObject(createTime); //
//                        }

//                        string Anser1 = await Task.Factory.StartNew(() => stst.ReadString(ReadStreamStringTimeOut, null));

//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"■DBリストア結果表示 {Anser1}");

//                        string Anser2 = await Task.Factory.StartNew(() => stst.ReadString(ReadStreamStringTimeOut, null));

//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"■ファイルストアリストア結果表示 {Anser2}");


//                        pipeCltStream.Close();
//                        List<string> AnserMessage = new List<string>() { Anser1, Anser2 };

//                        return AnserMessage;
//                    }
//                    else
//                    {
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return (new List<string>() { "Error", "サーバーからの接続文字列{input0}が期待と違います" });
//                    }
//                }

//                // Give the client process some time to display results before exiting.
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");
//                }
//                return (new List<string>() { "Error", $"PIPEサーバー接続エラー" });
//            }

//        }

//        #region ●メソッド 関連サーバー状態チェック・ステータス取得系

//        /// <summary>
//        /// ■ToyoDRAWRCAPTUREserviceのログレベル変数をセット　"ServerControl" "CONSOLE_LOGLEVEL_SET"
//        /// </summary>
//        /// <param name="Level"></param>
//        /// <returns></returns>
//        public string SetDRAWCAPTUREserviceLogLevel(int Level, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null)
//                delegateWriteLine = Console.WriteLine;
//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:SetDRAWCAPTUREserviceLogLevel
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
//            {
//                try
//                {
//                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
//                    // 待機中のサーバーへ接続
//                    try
//                    {
//                        pipeCltStream.Connect(ClientTimeOut);
//                    }
//                    catch (TimeoutException ex) //指定した timeout 期間内に、サーバーに接続できませんでした
//                    {
//                        PipeConnectionStatus = false;
//                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");

//                    }
//                    catch (InvalidOperationException ex) //クライアントが既に接続されています。
//                    {
//                        PipeConnectionStatus = false;
//                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");

//                    }
//                    catch (IOException ex) //サーバーが別のクライアントに接続されており、タイムアウト期間が期限切れです
//                    {
//                        PipeConnectionStatus = false;
//                        delegateWriteLine($"PIPEサーバー接続エラー サーバーが別のクライアントに接続されており、タイムアウト期間が期限切れです {ex.Message}");

//                    }
//                    catch (Exception ex)
//                    {
//                        PipeConnectionStatus = false;
//                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
//                        return null;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);

//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, delegateWriteLine);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }

//                    if (CheckFirstMessage(input0))
//                    {
//                        // SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"サーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl);
//                        stst.WriteString(CMDS.DC_DR_SW_SS_ServerCOntorl_CONSOLE_LOGLEVEL_SET);

//                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
//                        {
//                            writer.WriteObject(Level); //④send
//                        }
//                        pipeCltStream.Close();
//                        return AnserMessage;
//                    }
//                    else
//                    {
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return "エラー";
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");
//                }
//                return "エラー";
//            }
//        }

//        /// <summary>
//        /// ■ToyoDRAWCAPTUREserviceのログレベル変数を取得 "ServerControl" "CONSOLE_LOGLEVEL_GET"
//        /// </summary>
//        /// <param name="Level"></param>
//        /// <returns></returns>
//        public string GetDRAWCAPTUREserviceLogLevel(SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null)
//                delegateWriteLine = Console.WriteLine;
//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:GetDRAWCAPTUREserviceLogLevel
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
//            {
//                try
//                {
//                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
//                    // 待機中のサーバーへ接続
//                    try
//                    {
//                        pipeCltStream.Connect(ClientTimeOut);
//                    }
//                    catch (TimeoutException ex) //指定した timeout 期間内に、サーバーに接続できませんでした
//                    {
//                        PipeConnectionStatus = false;
//                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
//                        return null;

//                    }
//                    catch (InvalidOperationException ex) //クライアントが既に接続されています。
//                    {
//                        PipeConnectionStatus = false;
//                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
//                        return null;

//                    }
//                    catch (IOException ex) //サーバーが別のクライアントに接続されており、タイムアウト期間が期限切れです
//                    {
//                        PipeConnectionStatus = false;
//                        delegateWriteLine($"PIPEサーバー接続エラー サーバーが別のクライアントに接続されており、タイムアウト期間が期限切れです {ex.Message}");
//                        return null;

//                    }
//                    catch (Exception ex)
//                    {
//                        PipeConnectionStatus = false;
//                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
//                        return null;
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        // SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"サーバーからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl);
//                        stst.WriteString(CMDS.DC_DR_SW_SS_ServerControl_CONSOLE_LOGLEVEL_GET);

//                        //string currentLogLevel = ss.ReadString();
//                        string currentLogLevel = stst.ReadString(ReadStreamStringTimeOut, null);

//                        pipeCltStream.Close();

//                        return currentLogLevel;

//                    }
//                    else
//                    {
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();

//                        return $"tDRAWCAPTUREserviceからログレベルの取得に失敗！！";
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;

//                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");
//                }
//                return "エラー";
//            }
//        }

//        /// <summary>
//        /// ■DRAWCAPTUEserviceのPIPEサーバーのアセンブリバージョンを得る "GetVersion"
//        /// </summary>
//        /// <returns></returns>
//        public string GetDRAWCAPTUREserviceVersion(string Hostname, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null)
//                delegateWriteLine = Console.WriteLine;

//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:GetDRAWCAPTUREserviceVersion
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
//            {
//                try
//                {
//                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(Hostname, pipename);
//                    // 待機中のサーバーへ接続
//                    try
//                    {
//                        pipeCltStream.Connect(ClientTimeOut);
//                    }
//                    catch (Exception ex)
//                    {
//                        PipeConnectionStatus = false;

//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへの接続エラー {ex.Message}");
//                        delegateWriteLine($"PIPEサーバーへの接続エラー {ex.Message}");
//                        return "Error";
//                    }
//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return null;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへのからの接続文字列{input0}は期待値です");
//                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_GetVersion);

//                        //string ServerVersion = ss.ReadString();
//                        string ServerVersion = stst.ReadString(ReadStreamStringTimeOut, null);

//                        pipeCltStream.Close();

//                        return ServerVersion;
//                    }
//                    else
//                    {
//                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        return "エラー";
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;
//                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
//                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");
//                }
//                return "エラー";
//            }


//        }

//        /// <summary>
//        /// ■ステージサーバー上のテキストファイルをPIPE経由でﾛｰｶﾙﾌｧｲﾙに保存します。
//        /// </summary>
//        /// <param name="SourceFullFileName"></param>
//        /// <param name="DistFullFleName"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public bool GetTextFileFromPIPE(string SourceFullFileName, string DistFullFleName, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

//            if (SourceFullFileName == null)
//                return false;

//            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDRAWCAPTURE:GetTextFileFromPIPE
//            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
//            {
//                //SasaLib.StopWatch stopWatch = new StopWatch($"GetImageFromPIPE");
//                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
//                // 待機中のサーバーへ接続
//                try
//                {
//                    pipeCltStream.Connect(ClientTimeOut);
//                    // サーバーからの書き込みを受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);
//                    //string input0 = ss.ReadString();
//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        return false;
//                    }
//                    if (CheckFirstMessage(input0))
//                    {
//                        int writeResult = stst.WriteString(CMDS.DC_LoadTextFile);
//                        if (writeResult == -1)
//                            throw new Exception("PIPEコマンドを送信できませんでした");

//                        stst.WriteString(SourceFullFileName);
//                        //string receved = ss.ReadString();
//                        string receved = stst.ReadString(ReadStreamStringTimeOut, null);
//                        File.WriteAllText(DistFullFleName, receved);
//                    }
//                    else
//                    {
//                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
//                        return false;
//                    }
//                    ///
//                    pipeCltStream.Close();
//                    //stopWatch.Stop("RMloadImage({GUIDBASE64}・・・)");
//                    return true;
//                }
//                catch (Exception ex)
//                {
//                    delegateWriteLine($"RemoteClientDRAWCAPTURE.GetTextFileFromPIPE(..)\n" +
//                        $"Hostname:{PipeServerName}, ClsLogonDummy:{ClsLogon}, DomainName:{DomainName}, UserName:{UserName}, UserPassword:{UserPassword}" +
//                        $" PIEP接続失敗\n{ex.Message}\n{ex.InnerException}");
//                    pipeCltStream.Close();
//                    return false;
//                }

//            }
//        }

//        /// <summary>
//        /// ■サーバへの接続可否を確認する
//        /// </summary>
//        /// <returns></returns>
//        public bool ConnectTest(string sendTestMsg, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null)
//                delegateWriteLine = Console.WriteLine;

//            //using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogon))
//            //{
//            //    try
//            //    {
//            //        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);


//            //        // 待機中のサーバーへ接続
//            //        pipeCltStream.Connect(ClientTimeOut);

//            //        delegateWriteLine($"待機中のサーバーへ接続しました {PipeServerName} {pipename}");

//            //        // サーバーからのサーバ識別文字列を受け取ります。
//            //        StreamString stst = new StreamString(pipeCltStream);

//            //        bool OperationCanceledException;
//            //        bool AggregateException;

//            //        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//            //        if (OperationCanceledException || AggregateException)
//            //        {
//            //            delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//            //            return false;
//            //        }

//            //        if (CheckFirstMessage(input0))
//            //        {
//            //            delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");

//            //            string command = CMDS.DC_DR_SW_ConnectTest;
//            //            delegateWriteLine($"【{command}】を送信");
//            //            stst.WriteString(command);


//            //            string str = sendTestMsg;
//            //            delegateWriteLine($"【{str}】を送信");
//            //            stst.WriteString(str);

//            //            /// サーバーから結果情報を取得
//            //            string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
//            //            delegateWriteLine($"ConnectTest()【{AnserMessage}】を受信しました");

//            //            if (str == AnserMessage)
//            //            {
//            //                delegateWriteLine($"送信内容と受信内容が一致したので、接続テストは問題ありません");
//            //                pipeCltStream.Close();

//            //                delegateWriteLine($"ConnectTestt({sendTestMsg})正常終了");
//            //                return true;
//            //            }
//            //            else
//            //            {
//            //                delegateWriteLine($"送信内容と受信内容が不一致！！、接続テスト失敗");
//            //                pipeCltStream.Close();

//            //                delegateWriteLine($"ConnectTestt({sendTestMsg})エラー終了");
//            //                return false;
//            //            }
//            //        }
//            //        else
//            //        {
//            //            delegateWriteLine($"ConnectTestt({sendTestMsg})エラー終了。サーバーからの接続文字列{input0}が期待と違います");
//            //            pipeCltStream.Close();
//            //            return false;
//            //        }
//            //        // Give the client process some time to display results before exiting.
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        PipeConnectionStatus = false;
//            //        delegateWriteLine($"ｽﾃｰｼﾞﾝｸﾞｻｰﾊﾞｰ{PipeServerName} が応答しません。\n {ex.Message}");
//            //        return false;
//            //    }
//            //}

//            bool result = false;
//            // TODO: ClsLogonDummy を 書き換えた RemoteClientDRAWCAPTURE:ConnectTest
//            new WithFakeAccount(DomainName, UserName, UserPassword, ClsLogon, () =>
//            {
//                try
//                {
//                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);


//                    // 待機中のサーバーへ接続
//                    pipeCltStream.Connect(ClientTimeOut);

//                    delegateWriteLine($"待機中のサーバーへ接続しました {PipeServerName} {pipename}");

//                    // サーバーからのサーバ識別文字列を受け取ります。
//                    StreamString stst = new StreamString(pipeCltStream);

//                    bool OperationCanceledException;
//                    bool AggregateException;

//                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
//                    if (OperationCanceledException || AggregateException)
//                    {
//                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
//                        result = false;
//                        return;
//                    }

//                    if (CheckFirstMessage(input0))
//                    {
//                        delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");

//                        string command = CMDS.DC_DR_SW_ConnectTest;
//                        delegateWriteLine($"【{command}】を送信");
//                        stst.WriteString(command);


//                        string str = sendTestMsg;
//                        delegateWriteLine($"【{str}】を送信");
//                        stst.WriteString(str);

//                        /// サーバーから結果情報を取得
//                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
//                        delegateWriteLine($"ConnectTest()【{AnserMessage}】を受信しました");

//                        if (str == AnserMessage)
//                        {
//                            delegateWriteLine($"送信内容と受信内容が一致したので、接続テストは問題ありません");
//                            pipeCltStream.Close();

//                            delegateWriteLine($"ConnectTestt({sendTestMsg})正常終了");
//                            result = true;
//                            return;
//                        }
//                        else
//                        {
//                            delegateWriteLine($"送信内容と受信内容が不一致！！、接続テスト失敗");
//                            pipeCltStream.Close();

//                            delegateWriteLine($"ConnectTestt({sendTestMsg})エラー終了");
//                            result = false;
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        delegateWriteLine($"ConnectTestt({sendTestMsg})エラー終了。サーバーからの接続文字列{input0}が期待と違います");
//                        pipeCltStream.Close();
//                        result = false;
//                        return;
//                    }
//                    // Give the client process some time to display results before exiting.
//                }
//                catch (Exception ex)
//                {
//                    PipeConnectionStatus = false;
//                    delegateWriteLine($"ｽﾃｰｼﾞﾝｸﾞｻｰﾊﾞｰ{PipeServerName} が応答しません。\n {ex.Message}");
//                    result = false;
//                    return;
//                }

//            });

//            return result;
//        }

//        #endregion
//    }
//}
