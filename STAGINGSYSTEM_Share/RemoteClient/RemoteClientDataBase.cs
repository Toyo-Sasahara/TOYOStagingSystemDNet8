using System;
using System.Collections.Generic;
using System.IO;
using SasaLib;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using SasaLib.PIPE;
using ToyoMcMfg.Staging.DataBaseConfig;
using ToyoMcMfg.Staging.RemoteObjects;
using System.Data;
using STAGINGSYSTEM_COMMANDS;
using SasaLibDummy;
using SharedClassLibrary;
using System.Runtime.Versioning;

namespace StageServerRemote
{
    [SupportedOSPlatform("windows")]

    /// <summary>
    /// リモートにてデータベースに関する操作を行うクラス
    /// </summary>
    public class RemoteClientDataBase : RMCsupport
    {
        private SasaLibDelegateWriteLine delegateWriteLine;

        /// <summary>
        /// 接続アカウントをセット
        /// </summary>
        private readonly string DomainName;
        private readonly string UserName;
        private readonly string UserPassword;
        private readonly bool ClsLogonDummy;
        private readonly string PipeServerName;
        private readonly string PipeName;

        #region ●プロパティ

        /// <summary>
        /// 接続タイムアウト
        /// </summary>
        public int ClientTimeOut { get; set; } = 20000;

        public int ReadStreamStringTimeOut { get; set; } = 30000;

        public int ReadHandShakeStreamStringTimeOut { get; set; } = 20000;

        /// <summary>
        /// PIPE接続のステータス
        /// </summary>
        internal bool PipeConnectionStatus { get; private set; }

        /// <summary>
        /// 実行結果
        /// </summary>
        public bool AnserSucess { get; private set; }

        /// <summary>
        /// 実行結果メッセージ
        /// </summary>
        public string AnserMessage { get; private set; }

        /// <summary>
        /// 検索結果
        /// </summary>
        public List<FieldValueSet> DBresultList { get; private set; }


        /// <summary>
        /// 検索結果個数
        /// </summary>
        internal int Count { get; private set; }

        #endregion ●プロパティ

        #region ●コンストラクタ
        /// <summary>
        ///  ■リモートにてデータベースに関する操作を行うクラスのコンストラクタ
        /// </summary>
        /// <param name="ForcedDomainName"></param>
        /// <param name="ForcedUserName"></param>
        /// <param name="ForcedUserPassword"></param>
        /// <param name="ForcedAccountFlag">このフラグがtrueの時、ForcedDomainName、ForcedUserName、ForcedUserPasswordを使用する</param>
        /// <param name="PipeName">接続先のパイプ名</param>
        public RemoteClientDataBase(string ForcedDomainName, string ForcedUserName, string ForcedUserPassword, bool ForcedAccountFlag, string PipeServerName, string PipeName, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                this.delegateWriteLine = Console.WriteLine;
            else
                this.delegateWriteLine = delegateWriteLine;
            DomainName = ForcedDomainName;
            UserName = ForcedUserName;
            UserPassword = ForcedUserPassword;
            ClsLogonDummy = ForcedAccountFlag;
            this.PipeServerName = PipeServerName;
            this.PipeName = PipeName;
        }
        #endregion ●コンストラクタ

        #region ●インスタンス メソッド(public)
        /// <summary>
        /// ■StageDataBaseを検索する 検索条件は１つ
        /// 2022/08/24時点使用中確認
        /// </summary>
        /// <param name="findKeyValue">検索するフィールド名と値、型の構造体</param>
        /// <param name="AnsLines">検索結果</param>
        /// <returns></returns>
        public List<FieldValueSet> DataBaseSearch3(SqlFieldValue findKeyValue, int AnsLines = 9999)
        {
            InSearchWorking = true;

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
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, $"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}");

                        InSearchWorking = false;
                        return null;
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    //① サーバーとのハンドシェイクをチェック。
                    if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                    {
                        // ②コマンド送信
                        int writeResult = stst.WriteString(CMDS.DR_DataBaseSearch3); //send

                        // ③対象テーブル名送信
                        stst.WriteString("FILESTORE"); //send

                        // ④検索キーフィールドと検索値　findKeyValueを送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(findKeyValue); //④send
                        }

                        // ⑤ 検索結果に使用する FieldListを送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(new List<string>() { "*" }); //④send
                        }

                        // ⑥ 追加オプションを送信
                        stst.WriteString($"TOP ({AnsLines.ToString()})"); //send

                        // ⑦検索結果を受信

                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            DBresultList = reader.ReadObject<List<FieldValueSet>>(); //⑤read
                            Count = DBresultList.Count;
                        }
                    }
                    else
                    {
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "サーバとのハンドシェイクに失敗");
                    }

                    pipeCltStream.Close();

                    InSearchWorking = false;
                    return DBresultList;
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    Console.WriteLine($"PIPEサーバー接続エラー 例外 {ex.Message}");

                    InSearchWorking = false;
                    return null;
                }
            }
        }

        /// <summary>
        /// ■StageDataBaseを検索する 検索条件(複数可)を文字列で指定
        /// 2022/08/24時点・クライアントから未使用・サーバーから未使用
        /// </summary>
        /// <param name="sqlSearchFieldValues"></param>
        /// <param name="AnsLines"></param>
        /// <returns></returns>
        public List<FieldValueSet> DataBaseSearch4(List<SqlSearchStringValue> sqlSearchFieldValues, int AnsLines = 65535)
        {
            InSearchWorking = true;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:RemoteClientDataBase:DataBaseSearch4
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
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, $"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}");

                        InSearchWorking = false;
                        return null;
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    //① サーバーとのハンドシェイクをチェック。
                    if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                    {
                        // ②コマンド送信
                        int writeResult = stst.WriteString(CMDS.DR_DataBaseSearch4); //send

                        // ③対象テーブル名送信
                        stst.WriteString("FILESTORE"); //send

                        // ④検索キーフィールドと検索値　findKeyValueを送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(sqlSearchFieldValues); //④send
                        }

                        // ⑤ 検索結果に使用する FieldListを送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(new List<string>() { "*" }); //④send
                        }

                        // ⑥ 追加オプションを送信
                        stst.WriteString($"TOP ({AnsLines.ToString()})"); //send

                        // ⑦検索結果を受信

                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            DBresultList = reader.ReadObject<List<FieldValueSet>>(); //⑤read
                            Count = DBresultList.Count;
                        }
                    }
                    else
                    {
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "サーバとのハンドシェイクに失敗");
                    }

                    pipeCltStream.Close();

                    InSearchWorking = false;
                    return DBresultList;
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバー接続エラー {ex.Message}");

                    InSearchWorking = false;
                    return null;
                }
            }
        }

        /// <summary>
        /// ■StageDataBaseを検索する 検索条件(複数可)を文字列で指定。ORDER BYを指定可能
        /// 2022/08/24時点使用中確認
        /// </summary>
        /// <param name="sqlSearchFieldValues"></param>
        /// <param name="AnsLines"></param>
        /// <param name="ORDERBYSTR"></param>
        /// <returns></returns>
        public List<FieldValueSet> DataBaseSearch4a(List<SqlSearchStringValue> sqlSearchFieldValues, int AnsLines, string ORDERBYSTR)
        {
            InSearchWorking = true;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:RemoteClientDataBase:DataBaseSearch4a
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
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, $"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}");

                        InSearchWorking = false;
                        return null;
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    //① サーバーとのハンドシェイクをチェック。
                    if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                    {
                        // ②コマンド送信
                        int writeResult = stst.WriteString(CMDS.DR_DataBaseSearch4a); //send

                        // ③対象テーブル名送信
                        stst.WriteString("FILESTORE"); //send

                        // ④検索キーフィールドと検索値　findKeyValueを送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(sqlSearchFieldValues); //④send
                        }

                        // ⑤ 検索結果に使用する FieldListを送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(new List<string>() { "*" }); //④send
                        }

                        // ⑥ 追加オプションを送信
                        stst.WriteString($"TOP ({AnsLines.ToString()})"); //send
                        // ⑦ORDER BY オプション送信
                        stst.WriteString($"{ORDERBYSTR}"); //send

                        // ⑦検索結果を受信

                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            DBresultList = reader.ReadObject<List<FieldValueSet>>(WriteLine:DebugConsole.WriteLine ,Verbose:true); //⑤read
                            Count = DBresultList.Count;
                        }
                    }
                    else
                    {
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "サーバとのハンドシェイクに失敗");
                    }

                    pipeCltStream.Close();

                    InSearchWorking = false;
                    return DBresultList;
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    Console.WriteLine($"PIPEサーバー接続エラー 例外:{ex.Message}");

                    InSearchWorking = false;
                    return null;
                }
            }
        }

        /// <summary>
        /// ■アークスイート登録待ち一覧を取得するだけのメソッド
        /// </summary>
        /// <param name="sqlSearchFieldValues"></param>
        /// <param name="AnsLines"></param>
        /// <param name="ORDERBYSTR"></param>
        /// <returns></returns>
        public List<FieldValueSet> GetArcSuiteAwaitingRegist(List<SqlSearchStringValue> sqlSearchFieldValues, string toyoUSERID, int AnsLines, string ORDERBYSTR)
        {
            InSearchWorking = true;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:GetArcSuiteAwaitingRegist
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy,debugConsoleMsg:false))
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
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, $"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}");

                        InSearchWorking = false;
                        return null;
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    //① サーバーとのハンドシェイクをチェック。
                    if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                    {
                        // ②コマンド送信
                        int writeResult = stst.WriteString(CMDS.DR_GetArcSuiteAwaitingRegist); //send

                        // ③従業員ＩＤ
                        stst.WriteString(toyoUSERID);

                        // ④対象テーブル名送信
                        stst.WriteString("FILESTORE"); //send

                        // ⑤検索キーフィールドと検索値　findKeyValueを送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(sqlSearchFieldValues); //④send
                        }

                        // ⑥ 検索結果に使用する FieldListを送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(new List<string>() { "*" }); //④send
                        }

                        // ⑦ 追加オプションを送信
                        stst.WriteString($"TOP ({AnsLines.ToString()})"); //send
                        // ⑧ORDER BY オプション送信
                        stst.WriteString($"{ORDERBYSTR}"); //send

                        // ⑨検索結果を受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            DBresultList = reader.ReadObject<List<FieldValueSet>>(); //⑤read
                            Count = DBresultList.Count;
                        }
                    }
                    else
                    {
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "サーバとのハンドシェイクに失敗");
                    }

                    pipeCltStream.Close();

                    InSearchWorking = false;
                    return DBresultList;
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    Console.WriteLine($"GetArcSuiteAwaitingRegist PIPEサーバー接続エラー 例外:{ex.Message}");

                    InSearchWorking = false;
                    return null;
                }
            }
        }

        /// <summary>
        /// ■DB更新
        /// 2022/08/24時点使用中確認
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <param name="keyValueSet"></param>
        /// <returns></returns>
        public int Update(string GUIDBASE64, FieldValueSet keyValueSet)
        {
            int count = -1;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:Update
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
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, "東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                        return -1;
                    }

                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);

                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return -1;
                    }

                    if (CheckFirstMessage(input0))
                    {
                        int writeResult = stst.WriteString(CMDS.DR_DataBaseUpdate); //①send

                        stst.WriteString("GUIDBASE64"); //②send

                        stst.WriteString(GUIDBASE64); //②send

                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(keyValueSet); //③send
                        }

                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            count = reader.ReadObject<int>(); //④read

                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"変更した行：{count} 件");
                        }
                    }
                    else
                    {
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "Server could not be verified.");
                    }
                    pipeCltStream.Close();

                    return count;
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバー接続エラー {ex.Message}");
                }
            }

            return count;
        }

        /// <summary>
        /// ■ApprovedCancel2の処理が終った時に発呼されるイベントハンドラ
        /// </summary>
        public event EventHandler<bool> OnApprovedCancel2Result;

        public event EventHandler <List<string>> OnApprovedCancel3Result;

        /// <summary>
        /// ■押印クリア・データベース更新（GUIDBASE64コードのList形式にて指定） (RMCmaintenance.ApprovedCancel(..)より呼び出し)
        /// </summary>
        /// <param name="GUIDBASE64s"></param>
        /// <returns></returns>
        internal bool ApprovedCancels2(List<string> GUIDBASE64s, SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;


            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:RemoteClientDataBase:ApprovedCancels2
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, PipeName);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                        DelegateWriteLine($"ApprovedCancels2(...) pipeClientst.InBufferSize={pipeCltStream.InBufferSize}, pipeClientst.OutBufferSize={pipeCltStream.OutBufferSize}");
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        AnserMessage = $"PIPE接続失敗 {ex.Message}";
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, "東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                        return false;
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    // サーバーとハンドシェイク
                    if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                    {
                        // ①コマンド送信
                        int writeResult = stst.WriteString(CMDS.DR_ApprovedCancels2);
                        if (writeResult == -1)
                            throw new Exception("PIPEコマンドを送信できませんでした");

                        // ②キャンセル対象を送信　List<string>　の GUIDBASE64
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(GUIDBASE64s); //②send
                        }

                        // ③サーバーから結果を取得
                        List<string> CancelErrorGUIDBASE64List = new List<string>();
                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            CancelErrorGUIDBASE64List = reader.ReadObject<List<string>>();   //
                        }
                        DelegateWriteLine($"CancelErrorGUIDBASE64List.Count = {CancelErrorGUIDBASE64List.Count}");
                        if (CancelErrorGUIDBASE64List.Count == 0)
                        {
                            AnserSucess = true;
                        }
                        else
                        {
                            AnserSucess = false;

                        }
                    }
                    else
                    {
                        DelegateWriteLine("サーバーとハンドシェイクに失敗");
                    }
                    pipeCltStream.Close();
                    // Give the client process some time to display results before exiting.

                    OnApprovedCancel2Result?.Invoke(this, AnserSucess); // 承認処理結果を含むイベントを発火

                }
                catch (Exception ex)
                {
                    AnserSucess = false;

                    PipeConnectionStatus = false;
                    DelegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                }
            }

            return AnserSucess;
        }

        /// <summary>
        /// ■押印クリア・データベース更新(ApprovedCancelオブジェクトのList形式にて指定)
        /// 2022/08/24 時点 クライアントから呼び出しを確認
        /// 2022/08/24 時点使用中確認 リモートコマンド"ApprovedCancels3" を呼び出し
        /// </summary>
        /// <param name="CancelList"></param>
        /// <returns></returns>
        public bool ApprovedCancels3(List<ApprovedCancel> CancelList)
        {
            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:RemoteClientDataBase:ApprovedCancels3
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, PipeName);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ApprovedCancels(...) pipeClientst.InBufferSize={pipeCltStream.InBufferSize}, pipeClientst.OutBufferSize={pipeCltStream.OutBufferSize}");
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        AnserMessage = $"PIPE接続失敗 {ex.Message}";
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, "東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                        return false;
                    }
                    StreamString stst = new StreamString(pipeCltStream);
                    // サーバーとハンドシェイク
                    if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                    {
                        // ①コマンド送信
                        int writeResult = stst.WriteString(CMDS.DR_ApprovedCancels3);

                        // ②サーバー状態把握
                        string anserback = stst.ReadString(ReadStreamStringTimeOut, null);

                        if (anserback == "OK")
                        {
                            // ③キャンセル対象送信 List<ApprovedCancel>
                            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                            {
                                writer.WriteObject(CancelList); //②send
                            }
                            // ④サーバーから結果を取得
                            List<string> CancelErrorGUIDBASE64List = new List<string>();
                            using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                CancelErrorGUIDBASE64List = reader.ReadObject<List<string>>();   //
                            }
                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"CancelErrorGUIDBASE64List.Count = {CancelErrorGUIDBASE64List.Count}");

                            OnApprovedCancel3Result?.Invoke(this, CancelErrorGUIDBASE64List); // 承認処理結果を含むイベントを発火

                            if (CancelErrorGUIDBASE64List.Count == 0)
                            {
                                AnserSucess = true;
                            }
                            else
                            {
                                AnserSucess = false;
                            }
                        }
                        else if (anserback == "BUSY")
                        {
                            Debug.WriteLine("■■■■アークスイートサーバー・登録中。キャンセル動作停止");
                        }

                    }
                    else
                    {
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "サーバーとハンドシェイクに失敗");
                    }
                    pipeCltStream.Close();
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    AnserSucess = false;

                    PipeConnectionStatus = false;
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバー接続エラー 例外:{ex.Message}");
                }
            }

            return AnserSucess;
        }


        /// <summary>
        /// ■【2019-06-29】承認ロジックスタート(押印のみ、アークスイートへ登録指示なし。ステージサーバーへの登録実行フラグも立てない）
        /// </summary>
        /// <param name="GUIDBASE64">押印先を指定・GUIDBASE64</param>
        /// <param name="USERID">押印する従業員番号</param>
        /// <param name="StampTemplate">押印時に使用するスタンプの枠</param>
        public bool Approved2b(string GUIDBASE64, string USERID, out string ApprovedMessage, string StampTemplate = "StampBase.bmp")
        {
            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:RemoteClientDataBase:Approved2b
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                bool result = false;
                ApprovedMessage = null;

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
                    SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, "東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                    return false;
                }

                // サーバーからの書き込みを受け取ります。
                StreamString stst = new StreamString(pipeCltStream);

                // ① サーバーメッセージを受信、② サーバーメッセージをチェック
                if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                {
                    // ③コマンド送信
                    int writeResult = stst.WriteString(CMDS.DR_Approved2b);
                    // ④GUIDBASE64コードを送信
                    stst.WriteString(GUIDBASE64);
                    // ⑤承認実行者のID 例 0123 を送信
                    stst.WriteString(USERID);
                    // ⑤'スタンプベースイメージ名送信
                    stst.WriteString(StampTemplate);

                    ApprovedStatus Anser = new ApprovedStatus();
                    using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                    {
                        // ⑥サーバーから結果情報を取得
                        Anser = reader.ReadObject<ApprovedStatus>();
                        result = false;
                    }

                    /// サーバーから結果情報を評価
                    if (Anser.ApprovedSucess == false)
                    {
                        //MessageBox.Show($"承認サーバーからエラーが返されました。\n{Anser.ApprovedMessage}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ApprovedMessage = Anser.ApprovedMessage;
                        result = false;
                    }

                    GlovalValues.Mylog.LogRotateWriteLine($"◆承認サーバーからのメッセージ：Approved2b(...)実行完了。" +
                        $" 結果： 承認作業結果={Anser.ApprovedSucess},承認作業メッセージ={Anser.ApprovedMessage} StagingServerからの検索成功= {Anser.DataBaseGetSucess}\n" +
                        $"製図者：{Anser.AUTHOR} ,設計者： {Anser.DESIGNER} ,承認者：{Anser.APPROVEDUSER} ,ArcSuiteオブジェクトID： {Anser.ARCSUITEID},REGISTEDUSER:{Anser.REGISTEDUSER}" +
                        $"承認サーバーからのメッセージ終了"
                        );
                    GlovalValues.Mylog.Flash();

                    AnserSucess = Anser.ApprovedSucess;
                    AnserMessage = Anser.ApprovedMessage;

                    result = true;
                }
                else
                {
                    GlovalValues.Mylog.LogRotateWriteLine($"サーバーとのハンドシェイクに失敗しました");
                    result = false;
                }

                pipeCltStream.Close();
                return result;
            }
        }

        /// <summary>
        /// ■承認ロジックスタート(押印のみ、アークスイートへ登録指示なし。ステージサーバーへの登録実行フラグも立てない）
        /// </summary>
        /// <param name="GUIDBASE64">承認するチケットコード</param>
        /// <param name="USERID">承認作業を行うUSERID</param>
        /// <param name="Anser"></param>
        /// <param name="StampTemplate">押印に使うスタンプテンプレートイメージ</param>
        /// <param name="DateString">押印日を指定　null の場合は本日とする</param>
        /// <returns></returns>
        public bool Approved2c(string GUIDBASE64, string USERID, ref ApprovedStatus Anser, string StampTemplate = "StampBase.bmp", string DateString = null)
        {
            GlovalValues.Mylog.LogRotateWriteLine($"Approved2c()開始します ClientTimeOut:{ClientTimeOut}, ReadStreamStringTimeOut:{ReadStreamStringTimeOut}, ReadHandShakeStreamStringTimeOut:{ReadHandShakeStreamStringTimeOut}");

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:RemoteClientDataBase:Approved2c
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                bool result = false;

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
                    SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, $"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}");
                    return false;
                }

                // サーバーからの書き込みを受け取ります。
                StreamString stst = new StreamString(pipeCltStream);

                // ① サーバーメッセージを受信、② サーバーメッセージをチェック
                if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                {
                    // ③コマンド送信
                    int writeResult = stst.WriteString(CMDS.DR_Approved2c);
                    // ④GUIDBASE64コードを送信
                    stst.WriteString(GUIDBASE64);
                    // ⑤承認実行者のID 例 0123 を送信
                    stst.WriteString(USERID);
                    // ⑤'スタンプベースイメージ名送信
                    stst.WriteString(StampTemplate);

                    if (DateString == null)
                        DateString = "";

                    //⑥日付情報を送信
                    stst.WriteString(DateString);


                    //ApprovedStatus Anser = new ApprovedStatus();
                    using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                    {
                        // ⑦サーバーから結果情報を取得
                        Anser = reader.ReadObject<ApprovedStatus>();
                    }

                    /// サーバーから結果情報を評価
                    if (Anser.ApprovedSucess == false)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }

                    GlovalValues.Mylog.LogRotateWriteLine($"◆設計者承認 DB上の承認プロセス：Approved2c(...)完了。サーバーからのメッセージ：" +
                        $"対象：TICKETCODE=【{Anser.TICKETCODE}】, 図面番号=【{Anser.PARTNUMBER}】, サーバーでの検索結果 = {Anser.DataBaseGetSucess}," +
                        $"承認作業結果={Anser.ApprovedSucess}, 結果メッセージ=\"{Anser.ApprovedMessage}\"," +
                        $"製図者：【{Anser.AUTHOR}】 , 設計者：【{Anser.DESIGNER}】 , 承認者：{Anser.APPROVEDUSER} , 承認日時：{Anser.APPROVEDDATE} , ArcSuiteオブジェクトID：\"{Anser.ARCSUITEID},REGISTEDUSER:{Anser.REGISTEDUSER}\""
                        );
                    GlovalValues.Mylog.Flash();

                    AnserSucess = Anser.ApprovedSucess;
                    AnserMessage = Anser.ApprovedMessage;
                }
                else
                {
                    GlovalValues.Mylog.LogRotateWriteLine($"サーバーとのハンドシェイクに失敗しました");
                    result = false;
                }

                pipeCltStream.Close();
                return result;
            }
        }

        public FieldValueSet MergeApprovedStatus(ApprovedStatus approvedStatus, FieldValueSet fieldValueSet)
        {
            {
                int i = fieldValueSet.Params.FindIndex(x => x.Field == "DESIGNER");
                SqlDbType sqlDbType = fieldValueSet.Params[i].SqlDBType;
                fieldValueSet.Params.RemoveAt(i);
                fieldValueSet.Params.Add(new FieldValueSet.Param() { Field = "DESIGNER", Value = approvedStatus.DESIGNER, SqlDBType = sqlDbType });
            }

            {
                int i = fieldValueSet.Params.FindIndex(x => x.Field == "CHECKDATE");
                SqlDbType sqlDbType = fieldValueSet.Params[i].SqlDBType;
                fieldValueSet.Params.RemoveAt(i);
                fieldValueSet.Params.Add(new FieldValueSet.Param() { Field = "CHECKDATE", Value = approvedStatus.CHECKDATE, SqlDBType = sqlDbType });
            }
            return fieldValueSet;
        }

        #endregion ●インスタンス メソッド(public)

        #region ●インスタンス メソッド(internal)

        /// <summary>
        /// ■StageDataBaseを検索
        /// </summary>
        /// <param name="SearchField">検索条件の対象フィールド名</param>
        /// <param name="SearchValue">検索条件のフィールドの値</param>
        /// <param name="FieldList">検索結果を得るフィールドのリスト</param>
        /// <param name="memberName">このメソッドの呼び出し元(デバッグ用)</param>
        /// <param name="sourceFilePath">このメソッドの呼び出し元ソースファイル(デバッグ用)</param>
        /// <param name="sourceLineNumber">ソースファイルの行番号</param>
        internal List<FieldValueSet> DataBaseSearch(string SearchField, string SearchValue, List<string> FieldList, int AnsLines = 65535, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"◆RMdataBaseSearch.Search(...)呼び出し元{SasaLib.FileFolder.GetFileName(sourceFilePath)},{sourceLineNumber}行・・・メソッド:{memberName}");

            //空白かNULL文字の時は終了
            if (String.IsNullOrWhiteSpace(SearchValue) == true)
            {
                return null;
            }

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:RemoteClientDataBase:DataBaseSearch
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"　HostName:{PipeServerName}, PipeName:{PipeName}, SearchField:{SearchField}, SearchValue:{SearchValue}");

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
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, $"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}");
                        return null;
                    }

                    StreamString stst = new StreamString(pipeCltStream);

                    //① サーバーとのハンドシェイクをチェック。
                    if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                    {
                        // ②コマンド送信
                        int writeResult = stst.WriteString(CMDS.DR_DataBaseSearch);
                        // ③検索対象テーブル名送信
                        stst.WriteString("FILESTORE");
                        // ④検索対象のフィールド名送信
                        stst.WriteString(SearchField);
                        // ⑤検索するフィールドの内容を送信
                        stst.WriteString(SearchValue);
                        // ⑥検索結果に組み込むFieldListを送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(FieldList);
                        }
                        // ⑦検索結果の最大値を設定
                        stst.WriteString($"TOP ({AnsLines.ToString()})");

                        // ⑧結果を DataBaseSearch オブジェクトにて受信
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

                    return DBresultList;
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"PIPEサーバー接続エラー 例外：{ex.Message}");
                    return null;
                }
            }
        }

        /// <summary>
        /// ■Null値を検索するメソッド
        /// </summary>
        /// <param name="SearchField">検索する対象フィールド名</param>
        /// <param name="FieldList">検索結果を得るフィールドのリスト</param>
        internal void SearchNull(string SearchField, List<string> FieldList)
        {
            string keys = string.Join(",", FieldList);

            // TODO: RemoteClientDataBase:ClsLogonDummy を 書き換える必要 SearchNull
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
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, "東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                        return;
                    }

                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);

                    if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                    {
                        int writeResult = stst.WriteString(CMDS.DR_DataBaseSearchNull); //①send

                        stst.WriteString("FILESTORE"); //②send

                        stst.WriteString(SearchField); //②send

                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(FieldList); //③send
                        }

                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            DBresultList = reader.ReadObject<List<FieldValueSet>>(); //④read
                        }

                        Console.WriteLine("検索結果：{0} 件", DBresultList.Count);
                    }
                    else
                    {
                        Console.WriteLine("Server could not be verified.");
                    }
                    pipeCltStream.Close();
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine($"{ex.Message}", "PIPEサーバー接続エラー");
                }
            }
        }

        /// <summary>
        /// ■削除実行
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        internal bool RecordAndEntityfileDelete(string GUIDBASE64, SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;

            // TODO: ClsLogonDummy を 書き換える必要 RemoteClientDataBase:RecordAndEntityfileDelete
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
                        SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 6003, "東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                        return false;
                    }


                    StreamString stst = new StreamString(pipeCltStream);

                    // サーバーとハンドシェイク
                    if (CheckFirstMessage(stst.ReadString(ReadStreamStringTimeOut, null)))
                    {
                        // ①コマンド送信
                        int writeResult = stst.WriteString(CMDS.DR_RecordAndEntityfileDelete);

                        // ②削除対象送信
                        stst.WriteString(GUIDBASE64);

                        // ③サーバーから結果文字列を取得
                        AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);

                        DelegateWriteLine($"削除に対するサーバーからの情報 {AnserMessage}");
                    }
                    else
                    {
                        DelegateWriteLine("サーバーとハンドシェイクに失敗");
                    }
                    pipeCltStream.Close();
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    DelegateWriteLine(ex.Message);
                    DelegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                }
            }
            if (AnserMessage == "Sucess")
            {
                AnserSucess = true;
            }
            return AnserSucess;
        }

        #endregion ●インスタンス メソッド(internal)
    }
}
