using SasaLib;
using SasaLib.PIPE;
using SasaLibDummy;
using STAGINGSYSTEM_COMMANDS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using ToyoMcMfg.Staging.RemoteObjects;

namespace StageServerRemote
{

    /// <summary>
    /// DRAWCAPTUEserviceとのリモート接続クラス
    /// </summary>
    public class RemoteClientSYSTEMWATCH : RMCsupport
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
        internal bool ClsLogonDummy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        internal string PipeServerName { get; set; }

        /// <summary>
        /// 接続タイムアウト
        /// </summary>
        public int ClientTimeOut { get; set; } = 10000;

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
        public RemoteClientSYSTEMWATCH(string DomainName, string UserName, string UserPassword, bool ClsLogonDummy, string PipeServerName, string PipeName)
        {
            this.DomainName = DomainName;
            this.UserName = UserName;
            this.UserPassword = UserPassword;
            this.ClsLogonDummy = ClsLogonDummy;
            this.PipeServerName = PipeServerName;
            this.pipename = PipeName;
        }
        #endregion

        /// <summary>
        /// ■STAGINGSYSTEMwatchのPIPEサーバーのアセンブリバージョンを得る
        /// </summary>
        /// <returns></returns>
        public string GetSYSTEMWATCHserviceVersion(string Hostname, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
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
                        delegateWriteLine($"※RemoteClientDrawDRAWWATCH.GetSYSTEMWATCHserviceVersion(..)にて例外発生 {ex.Message}");
                        return "Error";
                    }
                    // サーバーからのサーバ識別文字列を受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(60 * 1000 * 2, out OperationCanceledException, out AggregateException, null);
                    delegateWriteLine($"サーバー識別文字列 {input0} を受信しました");

                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return null;
                    }
                    if (CheckFirstMessage(input0))
                    {
                        delegateWriteLine($"サーバー識別文字列 {input0} はただし値です。コマンドを送信します");
                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_GetVersion);
                        delegateWriteLine($"コマンドGetVersionを送信しました。");

                        delegateWriteLine($"サーバーからのバージョン情報を待っています");
                        string ServerVersion = stst.ReadString(ReadStreamStringTimeOut, null);

                        pipeCltStream.Close();

                        return ServerVersion;
                    }
                    else
                    {
                        delegateWriteLine($"※RemoteClientDrawDRAWWATCH.GetSYSTEMWATCHserviceVersion(..) ステージサーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;

                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6002, $"PIPEサーバー接続エラー\n{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■ﾌﾟﾘﾝﾀｷｭｰを表示
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="printerName"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string ShowPrinterQueue(string Hostname, string printerName, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
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
                        delegateWriteLine($"※RemoteClientDrawDRAWWATCH.GetSYSTEMWATCHserviceVersion(..)にて例外発生 {ex.Message}");
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
                        int writeResult = stst.WriteString(CMDS.SW_ShowPrinterQueue);

                        stst.WriteString(printerName);

                        string result = stst.ReadString(ReadStreamStringTimeOut, null);

                        pipeCltStream.Close();

                        return result;
                    }
                    else
                    {
                        delegateWriteLine($"※RemoteClientDrawDRAWWATCH.GetSYSTEMWATCHserviceVersion(..) ステージサーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;

                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6002, $"PIPEサーバー接続エラー\n{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■指定したホストから指定した名前のメモリマップドファイルを読み込む
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="Label"></param>
        /// <param name="typestr"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public object GetSYSTEMWATCHserviceMmapvalue(string Hostname, string Label, string typestr, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
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

                        delegateWriteLine($"※RemoteClientDrawDRAWWATCH.GetSYSTEMWATCHserviceMmapvalue(..)にて例外発生 {ex.Message}");
                        return null;
                    }
                    // サーバーからのサーバ識別文字列を受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    string input0 = stst.ReadString(ReadStreamStringTimeOut, null);
                    if (CheckFirstMessage(input0))
                    {
                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ステージサーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.SW_GetMemMapdFile);

                        stst.WriteString(Label);
                        stst.WriteString(typestr);

                        switch (typestr)
                        {
                            case "string":
                                var resultStr = stst.ReadString(ReadStreamStringTimeOut, null);
                                pipeCltStream.Close();
                                return resultStr;

                            case "bool":
                                using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                                {
                                    var resultBool = reader.ReadObject<bool>();
                                    pipeCltStream.Close();
                                    return resultBool;
                                }

                            case "int":
                                using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                                {
                                    var resultInt = reader.ReadObject<int>();
                                    pipeCltStream.Close();
                                    return resultInt;
                                }

                            default:
                                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAGINGSYSTEMwatch", EventLogEntryType.Error, 6002, $"エラー　型名\"{typestr}\"は設定されていない", false);
                                pipeCltStream.Close();
                                return null;
                        }
                    }
                    else
                    {
                        delegateWriteLine($"※RemoteClientDrawDRAWWATCH.GetSYSTEMWATCHserviceMmapvalue(..) ステージサーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return null;
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;

                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6002, $"PIPEサーバー接続エラー\n{ex.Message}");
                }
                return null;
            }
        }

        /// <summary>
        /// ■指定したホストの指定した名前のメモリマップドファイルに値を書き込む
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="Label"></param>
        /// <param name="typestr"></param>
        /// <param name="Value"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public bool SetDRAWWATCHserviceMmapvalue(string Hostname, string Label, string typestr, object Value, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
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

                        delegateWriteLine($"※RemoteClientDrawDRAWWATCH.SetDRAWWATCHserviceMmapvalue(..)にて例外発生 {ex.Message}");
                        return false;
                    }
                    // サーバーからのサーバ識別文字列を受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    string input0 = stst.ReadString(ReadStreamStringTimeOut, null);
                    if (CheckFirstMessage(input0))
                    {
                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ステージサーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.SW_SetMemMapdFile);

                        stst.WriteString(Label);
                        stst.WriteString(typestr);

                        if (typestr == "string")
                        {
                            stst.WriteString((string)Value);
                        }
                        else
                        {
                            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                            {
                                // ④結果をクライアントに送出
                                writer.WriteObject(Value);
                            }

                        }

                        pipeCltStream.Close();

                        return true;
                    }
                    else
                    {
                        delegateWriteLine($"※RemoteClientDrawDRAWWATCH.SetDRAWWATCHserviceMmapvalue(..) ステージサーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return false;
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;

                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6002, $"PIPEサーバー接続エラー\n{ex.Message}");
                    return false;
                }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendTestMsg"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public bool ConnectTest(string sendTestMsg, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);


                    // 待機中のサーバーへ接続
                    pipeCltStream.Connect(ClientTimeOut);

                    delegateWriteLine($"待機中のサーバーへ接続しました {PipeServerName} {pipename}");

                    // サーバーからのサーバ識別文字列を受け取ります。
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
                        delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");

                        string command = CMDS.DC_DR_SW_ConnectTest;
                        delegateWriteLine($"【{command}】を送信");
                        stst.WriteString(command);


                        string str = sendTestMsg;
                        delegateWriteLine($"【{str}】を送信");
                        stst.WriteString(str);

                        /// サーバーから結果情報を取得
                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
                        delegateWriteLine($"ConnectTest()【{AnserMessage}】を受信しました");

                        if (str == AnserMessage)
                        {
                            delegateWriteLine($"送信内容と受信内容が一致したので、接続テストは問題ありません");
                            pipeCltStream.Close();

                            delegateWriteLine($"ConnectTestt({sendTestMsg})正常終了");
                            return true;
                        }
                        else
                        {
                            delegateWriteLine($"送信内容と受信内容が不一致！！、接続テスト失敗");
                            pipeCltStream.Close();

                            delegateWriteLine($"ConnectTestt({sendTestMsg})エラー終了");
                            return false;
                        }
                    }
                    else
                    {
                        delegateWriteLine($"ConnectTestt({sendTestMsg})エラー終了。サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return false;
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"ｽﾃｰｼﾞﾝｸﾞｻｰﾊﾞｰ{PipeServerName} が応答しません。\n {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverSaveFullfileName"></param>
        /// <param name="clientSaveFullFileName"></param>
        /// <param name="paperSize"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public bool TitleFieldTest(string serverSaveFullfileName, string clientSaveFullFileName, SasaLib.PrintConfig.CommonPaperSize paperSize = SasaLib.PrintConfig.CommonPaperSize.A4P, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);

                    // 待機中のサーバーへ接続
                    pipeCltStream.Connect(ClientTimeOut);

                    delegateWriteLine($"待機中のサーバーへ接続しました {PipeServerName} {pipename}");

                    StreamString stst = new StreamString(pipeCltStream);

                    bool OperationCanceledException;
                    bool AggregateException;

                    // ①サーバーからのサーバ識別文字列を受け取ります。
                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return false;
                    }

                    if (CheckFirstMessage(input0))
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");

                        string command = CMDS.SW_UnknownCommand;
                        delegateWriteLine($"【{command}】を送信");

                        // ②コマンド名送信
                        stst.WriteString(command);

                        // ③サーバに保存するファイル名を送信
                        stst.WriteString(serverSaveFullfileName, 100000, out OperationCanceledException, out AggregateException);

                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            // ④用紙サイズ送信
                            writer.WriteObject(paperSize);
                        }

                        float dpi = 400.0f;
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            // ⑤DPI送信
                            writer.WriteObject(dpi);
                        }

                        MemoryStream ms = new MemoryStream();
                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            /// ⑥サーバーから結果情報を取得
                            ms = reader.ReadObject<MemoryStream>();

                            // TIFFメモリストリームをファイルに保存
                            StreamExtensions.StreamToFile(ms, clientSaveFullFileName);

                        }
                        ms.Close();

                        return true;

                    }
                    else
                    {
                        delegateWriteLine($"ConnectTestt({serverSaveFullfileName})エラー終了。サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return false;
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"ｽﾃｰｼﾞﾝｸﾞｻｰﾊﾞｰ{PipeServerName} が応答しません。\n {ex.Message}");
                    return false;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string GetSTAGINGSYSTEMwatchLogLevel(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (TimeoutException ex) //指定した timeout 期間内に、サーバーに接続できませんでした
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        return null;

                    }
                    catch (InvalidOperationException ex) //クライアントが既に接続されています。
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        return null;

                    }
                    catch (IOException ex) //サーバーが別のクライアントに接続されており、タイムアウト期間が期限切れです
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー サーバーが別のクライアントに接続されており、タイムアウト期間が期限切れです {ex.Message}");
                        return null;

                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        return null;
                    }
                    // サーバーからのサーバ識別文字列を受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    //string input0 = ss.ReadString();
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
                        // SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl);
                        stst.WriteString(CMDS.DC_DR_SW_SS_ServerControl_CONSOLE_LOGLEVEL_GET);

                        //string currentLogLevel = ss.ReadString();
                        string currentLogLevel = stst.ReadString(ReadStreamStringTimeOut, null);

                        pipeCltStream.Close();

                        return currentLogLevel;

                    }
                    else
                    {
                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();

                        return $"tDRAWCAPTUREserviceからログレベルの取得に失敗！！";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;

                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string SetSTAGINGSYSTEMwatchLogLevel(int Level, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (TimeoutException ex) //指定した timeout 期間内に、サーバーに接続できませんでした
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        return null;

                    }
                    catch (InvalidOperationException ex) //クライアントが既に接続されています。
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        return null;

                    }
                    catch (IOException ex) //サーバーが別のクライアントに接続されており、タイムアウト期間が期限切れです
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー サーバーが別のクライアントに接続されており、タイムアウト期間が期限切れです {ex.Message}");
                        return null;

                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        return null;
                    }
                    // サーバーからのサーバ識別文字列を受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    //string input0 = ss.ReadString();
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
                        // SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl);
                        stst.WriteString(CMDS.DC_DR_SW_SS_ServerCOntorl_CONSOLE_LOGLEVEL_SET);

                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(Level); //④send
                        }
                        pipeCltStream.Close();
                        return AnserMessage;
                    }
                    else
                    {
                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;

                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"PIPEサーバー接続エラー\n{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableMemory"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public bool GetAvailableMemory(out float availableMemory, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            availableMemory = 0f;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    using (NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename))
                    {
                        // 待機中のサーバーへ接続
                        pipeCltStream.Connect(ClientTimeOut);

                        delegateWriteLine($"待機中のサーバーへ接続しました {PipeServerName} {pipename}");

                        StreamString stst = new StreamString(pipeCltStream);
                        bool OperationCanceledException;
                        bool AggregateException;

                        // ①サーバーからのサーバ識別文字列を受け取ります。
                        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                        if (OperationCanceledException || AggregateException)
                        {
                            delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                            return false;
                        }

                        if (CheckFirstMessage(input0))
                        {
                            delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");

                            string command = CMDS.SW_GetAvailableMemory;
                            delegateWriteLine($"【{command}】を送信");

                            // ②コマンド名送信
                            stst.WriteString(command);

                            using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                /// ⑥サーバーから結果情報を取得
                                availableMemory = reader.ReadObject<float>();

                                delegateWriteLine($"サーバーから結果を受信しました。利用可能なメモリーは {availableMemory} MB です");

                                reader.Close();
                            }

                            return true;
                        }
                        else
                        {
                            delegateWriteLine($"{CMDS.SW_GetAvailableMemory} エラー終了。サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return false;
                        }
                        // Give the client process some time to display results before exiting.
                    }
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"ｽﾃｰｼﾞﾝｸﾞｻｰﾊﾞｰ{PipeServerName} が応答しません。\n {ex.Message}");
                    return false;
                }

            }
        }

        public bool GetUsedMemory(string asmName, out long availableMemory, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            availableMemory = 0;

            // TODO: ClsLogonDummy を 書き換える必要
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    using (NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename))
                    {
                        // 待機中のサーバーへ接続
                        pipeCltStream.Connect(ClientTimeOut);

                        delegateWriteLine($"待機中のサーバーへ接続しました {PipeServerName} {pipename}");

                        StreamString stst = new StreamString(pipeCltStream);
                        bool OperationCanceledException;
                        bool AggregateException;

                        // ①サーバーからのサーバ識別文字列を受け取ります。
                        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                        if (OperationCanceledException || AggregateException)
                        {
                            delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                            return false;
                        }

                        if (CheckFirstMessage(input0))
                        {
                            delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");

                            string command = CMDS.SW_GetMemoryUsageWorkingSetSize;
                            delegateWriteLine($"【{command}】を送信");

                            // ②コマンド名送信
                            stst.WriteString(command);

                            stst.WriteString(asmName);

                            using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                /// ⑥サーバーから結果情報を取得
                                availableMemory = reader.ReadObject<long>();

                                if (availableMemory != 0f)
                                {
                                    delegateWriteLine($"サーバーから結果を受信しました。アセンブリ {asmName} の使用中メモリーは {availableMemory:n0} バイト です");
                                }
                                else
                                {
                                    delegateWriteLine($"サーバーから結果を受信しました。アセンブリ {asmName} は見つかりません");
                                }

                                reader.Close();
                            }

                            return true;
                        }
                        else
                        {
                            delegateWriteLine($"{CMDS.SW_GetAvailableMemory} エラー終了。サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return false;
                        }
                        // Give the client process some time to display results before exiting.
                    }
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"ｽﾃｰｼﾞﾝｸﾞｻｰﾊﾞｰ{PipeServerName} が応答しません。\n {ex.Message}");
                    return false;
                }

            }
        }

    }
}
