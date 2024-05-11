// ステージングサーバーと通信を行うクラス
//
using System;
using System.Collections.Generic;
using System.IO;
using SasaLib;
using System.Diagnostics;
using System.IO.Pipes;
using System.Drawing;
using System.Text;
using System.Runtime.CompilerServices;
using SasaLib.PIPE;
using System.Collections;
using ToyoMcMfg.Staging.DataBaseConfig;
using System.Windows.Forms;
using ToyoStageService;
using System.Threading.Tasks;
using System.Threading;
using STAGINGSYSTEM_COMMANDS;
using SasaLibDummy;

namespace StageServerRemote
{
    /// <summary>
    /// ■【2019-06-29】リモート操作を集めたクラス
    /// </summary>
    public class RemoteClientDRAWREGIST : RMCsupport
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
        public int ClientTimeOut { get; set; } = 100000;

        public int ReadStreamStringTimeOut { get; set; } = 60000;

        public int ReadHandShakeStreamStringTimeOut { get; set; } = 10000;

        /// <summary>
        /// PIPE接続のステータス
        /// </summary>
        internal bool PipeConnectionStatus { get; private set; }

        /// <summary>
        /// 実行結果メッセージ
        /// </summary>
        internal string AnserMessage { get; private set; }
        #endregion

        #region ●コンストラクタ
        /// <summary>
        /// コンストラクタ PIPEサーバーへの接続準備も行う
        /// </summary>
        /// <param name="DomainName">接続強制ユーザーのドメイン</param>
        /// <param name="UserName">接続強制ユーザー名</param>
        /// <param name="UserPassword">接続強制ユーザーパスワード</param>
        /// <param name="ClsLogonDummy">接続強制を許可するスイッチこのスイッチがfalseの場合接続強制アカウントは使用されない</param>
        /// <param name="PipeName">接続先パイプ名</param>
        public RemoteClientDRAWREGIST(string DomainName,
            string UserName,
            string UserPassword,
            bool ClsLogonDummy,
            string PipeServerName,
            string PipeName)
        {
            this.DomainName = DomainName;
            this.UserName = UserName;
            this.UserPassword = UserPassword;
            this.ClsLogonDummy = ClsLogonDummy;

            this.PipeServerName = PipeServerName;
            this.pipename = PipeName;

        }
        #endregion

        #region ●メソッド ステージサーバー図面検索系

        /// <summary>
        /// ■PARTNUMBERをキーとしてデータベースを検索。RMCdatabase.DataBaseSearch(...)を呼び出す
        /// </summary>
        /// <param name="PARTNUMBER"></param>
        /// <returns></returns>
        public List<FieldValueSet> DBsearchFromPARTNUMBER(string PARTNUMBER, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

            if (PARTNUMBER != null)
            {
                string SearchKey = @"PARTNUMBER";
                string SearchValue = PARTNUMBER;
                List<string> Keys = new List<string>() { "*" };

                RemoteClientDataBase dbSearch = new RemoteClientDataBase(DomainName,
                    UserName,
                    UserPassword,
                    ClsLogonDummy,
                    PipeServerName, pipename);

                dbSearch.DataBaseSearch(SearchKey, SearchValue, Keys);
                return dbSearch.DBresultList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ■PAPERSIZEをキーとしてデータベースを検索。RMCdatabase.DataBaseSearch(...)を呼び出す
        /// </summary>
        /// <param name="PAPERSIZE"></param>
        /// <returns></returns>
        public List<FieldValueSet> DBsearchFromPAPERSIZE(string PAPERSIZE, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            if (PAPERSIZE != null)
            {
                string SearchKey = @"PAPERSIZE";
                string SearchValue = PAPERSIZE;
                List<string> Keys = new List<string>() { "*" };

                RemoteClientDataBase dbSearch = new RemoteClientDataBase(DomainName,
                    UserName,
                    UserPassword,
                    ClsLogonDummy,
                    PipeServerName,
                    pipename);

                dbSearch.DataBaseSearch(SearchKey, SearchValue, Keys);
                return dbSearch.DBresultList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ■TICKETCODEをキーとしてデータベースを検索。RMCdatabase.DataBaseSearch(...)を呼び出す
        /// </summary>
        /// <param name="TICKETCODE"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        public List<FieldValueSet> DBsearchFromTICKETCODE(string TICKETCODE, [CallerMemberName] string okonsmemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            if (String.IsNullOrWhiteSpace(TICKETCODE) == false)
            {
                string SearchField = @"TICKETCODE";
                string SearchValue = TICKETCODE;
                List<string> FieldList = new List<string>() { "*" };

                // 
                RemoteClientDataBase dbSearch = new RemoteClientDataBase(DomainName,
                    UserName,
                    UserPassword,
                    ClsLogonDummy,
                    PipeServerName,
                    pipename);

                dbSearch.DataBaseSearch(SearchField, SearchValue, FieldList);

                if (dbSearch.Count > 0)
                {
                    delegateWriteLine($"検索結果：{dbSearch.DBresultList.Count} 件");

                    return dbSearch.DBresultList;
                }
                else
                {
                    return dbSearch.DBresultList;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ■TICKETCODEをキーとしてデータベースから検索。（1件のみの該当レコードがあるもの）。RemoteClientDRAWREGIST.DBsearchFromTICKETCODE(...)を呼び出す
        /// </summary>
        /// <param name="TICKETCODE"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns>複数の結果がある場合はnullを返す</returns>
        public FieldValueSet DBsearchFromTICKETCODEsingleResult(string TICKETCODE, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            delegateWriteLine($"DBsearchFromTICKETCODEsingleResult(...) 呼び出し元 {sourceFilePath},{sourceLineNumber},{memberName}");

            var result = DBsearchFromTICKETCODE(TICKETCODE);
            if (result != null)
            {
                if (result.Count == 1)
                {
                    return result[0];
                }
                return null;
            }
            return null;
        }

        /// <summary>
        ///■ARCSUITEIDをキーとしてデータベースの値がNULLの行を検索。RMCdataBase.SearchNull(...)
        /// </summary>
        /// <returns></returns>
        public List<FieldValueSet> DBsearchARCSUITEIDisNULL(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            List<string> Keys = new List<string>() { "*" };

            RemoteClientDataBase dbSearch = new RemoteClientDataBase(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                pipename);

            dbSearch.SearchNull(@"ARCSUITEID", Keys);
            return dbSearch.DBresultList;
        }

        /// <summary>
        /// ■無条件にすべてを検索。RMCdatabase.DataBaseSearch(...)
        /// </summary>
        /// <returns></returns>
        public List<FieldValueSet> DBsearchAllRecord(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            string SearchKey = @"TICKETCODE";
            string SearchValue = "%";
            List<string> Keys = new List<string>() { "*" };

            RemoteClientDataBase dbSearch = new RemoteClientDataBase(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                pipename);

            dbSearch.DataBaseSearch(SearchKey, SearchValue, new List<string>() { "*" });

            return dbSearch.DBresultList;
        }

        #endregion

        #region ●ArcSuite 属性値検索系

        /// <summary>
        /// ■ArcSuiteに図面番号を指定して検索。結果で表示する属性はおまかせ "GetArcSuiteAtrtribute"
        /// </summary>
        /// <param name="ZUBANlist">検索する図面番号のコレクション</param>
        /// <returns>該当結果のコレクション</returns>
        public ArrayList GetArcSuiteAttribute(List<string> ZUBANlist, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);

                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);

                    ArrayList AnsArrayList = new ArrayList();

                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);

                    // ①接続文字の受信とチェック
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
                        // ②コマンド名送信
                        int writeResult = stst.WriteString(CMDS.DR_GetArcSuiteAtrtribute);

                        // ③検索図面番号送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            // サーバーに送出
                            writer.WriteObject(ZUBANlist);
                        }
                        // ④サーバーからArrayListを受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            AnsArrayList = reader.ReadObject<ArrayList>();
                        }
                    }
                    else if (input0 == null)
                    {
                        delegateWriteLine($"コミットサーバーに接続できませんでした(タイムアウト)");
                        return null;
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        pipeCltStream.Close();
                        return null;
                    }

                    pipeCltStream.Close();
                    //
                    return AnsArrayList;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    pipeCltStream.Close();
                    return null;
                }
            }
        }

        /// <summary>
        /// ■ArcSuiteに図面番号を1件のみ指定して検索。結果で表示する属性も指定する "GetArcSuiteAtrtribute1"
        /// </summary>
        /// <param name="ZUBAN"></param>
        /// <param name="Attrstr"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public ArrayList GetArcSuiteAttribute(string ZUBAN, string Attrstr, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    ArrayList AnsArrayList = new ArrayList();
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    // ①接続文字の受信とチェック
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
                        // ②コマンド名送信
                        int writeResult = stst.WriteString(CMDS.DR_GetArcSuiteAtrtribute1);
                        // ③検索図面番号送信
                        stst.WriteString(ZUBAN);
                        // ④検索結果表示属性設定CSVの記述を送信
                        stst.WriteString(Attrstr);
                        // ⑤サーバーからArrayListを受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            AnsArrayList = reader.ReadObject<ArrayList>();
                        }
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        pipeCltStream.Close();
                        return null;
                    }
                    //
                    pipeCltStream.Close();
                    return AnsArrayList;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    pipeCltStream.Close();
                    return null;
                }
            }

        }

        /// <summary>
        /// ■ArcSuiteに図面番号を指定して検索。結果で取得する属性も指定する　"GetArcSuiteAtrtribute2"
        /// </summary>
        /// <param name="ZUBANlist">図番のコレクション</param>
        /// <param name="Attrstr">表示する属性値のコレクション</param>
        /// <returns>該当結果のコレクション</returns>
        public ArrayList GetArcSuiteAttribute2(List<string> ZUBANlist, string Attrstr, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    ArrayList AnsArrayList = new ArrayList();
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    // ①接続文字の受信とチェック
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
                        // ②コマンド名送信
                        int writeResult = stst.WriteString(CMDS.DR_GetArcSuiteAtrtribute2);
                        // ③検索図面番号送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            // サーバーに送出
                            writer.WriteObject(ZUBANlist);
                        }
                        // ④検索結果表示属性設定CSVの記述を送信
                        stst.WriteString(Attrstr);
                        // ⑤サーバーからArrayListを受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            AnsArrayList = reader.ReadObject<ArrayList>();
                        }
                    }
                    else if (input0 == null)
                    {
                        delegateWriteLine($"コミットサーバーに接続できませんでした(タイムアウト)");

                        return null;
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        pipeCltStream.Close();
                        return null;
                    }
                    //
                    pipeCltStream.Close();
                    return AnsArrayList;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    pipeCltStream.Close();
                    return null;
                }
            }
        }

        /// <summary>
        /// ■ArcSuiteに図面番号を指定して検索。結果で取得する属性も指定する　"GetArcSuiteAtrtribute2"
        /// </summary>
        /// <param name="ZUBANlist">図番のコレクション</param>
        /// <param name="Attrstr">表示する属性値のコレクション</param>
        /// <returns>該当結果のコレクション</returns>
        public async Task<ArrayList> GetArcSuiteAttribute3(string cabinetID, List<string> ZUBANlist, string Attrstr, CancellationToken ct, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (ct.IsCancellationRequested) { return null; } // Cancel発行時は null を返す

            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    ArrayList AnsArrayList = new ArrayList();
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    // ①接続文字の受信とチェック
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
                        // ②コマンド名送信
                        int writeResult = stst.WriteString(CMDS.DR_GetArcSuiteAtrtribute3);

                        // ③キャビネットID送信
                        stst.WriteString(cabinetID);

                        // ④検索図面番号送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            // サーバーに送出
                            writer.WriteObject(ZUBANlist);
                        }
                        // ⑤検索結果表示属性設定CSVの記述を送信
                        stst.WriteString(Attrstr);
                        // ⑥サーバーからArrayListを受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            try
                            {
                                var readerRaedobject = reader.ReadObject<ArrayList>(WriteLine: delegateWriteLine, Verbose: false);
                                AnsArrayList = readerRaedobject;
                            }
                            catch (Exception ex)
                            {
                                delegateWriteLine($"※reader.ReadObject(..) 返り値の受取にて例外 {ex.Message} {ex.StackTrace}");
                            }
                        }
                    }
                    else if (input0 == null)
                    {
                        delegateWriteLine($"コミットサーバーに接続できませんでした(タイムアウト)");

                        return null;
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        pipeCltStream.Close();
                        return null;
                    }
                    //
                    pipeCltStream.Close();

                    await Task.Delay(1); // ダミー（ワーニング対策）

                    if (ct.IsCancellationRequested)
                    {
                        return null;
                    }
                    else
                    {
                        return AnsArrayList;
                    }
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    pipeCltStream.Close();

                    await Task.Delay(1); // ダミー（ワーニング対策）

                    return null;
                }
            }
        }

        /// <summary>
        /// ■ArcSuiteにオブジェクトIDを指定して検索。"GetArcSuiteAtrtributeFromObjectID"
        /// </summary>
        /// <param name="objectId">ArcSuiteオブジェクトID</param>
        /// <param name="selectAttr">表示する属性値のコレクション</param>
        /// <returns>>該当結果のコレクション</returns>
        public ArrayList GetArcSuiteAttributeFromObjectId(string objectId, string selectAttr = "user:zuban|system:editionNumber|system:latestEditionFlag|system:createdOn|user:torokubi|user:drawingrevision", SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    ArrayList AnsArrayList = new ArrayList();
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    // ①接続文字の受信とチェック
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
                        // ②コマンド名送信
                        int writeResult = stst.WriteString(CMDS.DR_GetArcSuiteAtrtributeFromObjectID);
                        // ③ObjectIDを送信
                        stst.WriteString(objectId);
                        //④検索で取得する属性値を送信
                        stst.WriteString(selectAttr);
                        // ⑤サーバーからArrayListを受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            AnsArrayList = reader.ReadObject<ArrayList>();
                        }
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        pipeCltStream.Close();
                        return null;
                    }
                    //
                    pipeCltStream.Close();
                    return AnsArrayList;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    pipeCltStream.Close();
                    return null;
                }
            }
        }

        #endregion

        #region ●ArcSuite 図面ダウンロード
        /// <summary>
        /// ■ArcSuiteから複数の図面を検索し指定フォルダ（サーバーから見たUNCフォルダ）へ一括書き出し　"GetArcSuiteContents"
        /// </summary>
        /// <param name="ZUBANlist"></param>
        /// <param name="exportUNCFfolder"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public List<string> GetArcSuiteContents(string target_ServiceID_CabinetID, List<string> ZUBANlist, string exportUNCFfolder, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    int writeLineTimeout = 10000;
                    int writeResult;

                    pipeCltStream.Connect(ClientTimeOut);
                    List<string> resultList;
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    // ①接続文字の受信とチェック
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
                        // ②コマンド名送信
                        writeResult = stst.WriteString(CMDS.DR_GetArcSuiteContents, writeLineTimeout);
                        // ③サービスＩＤとキャビネットＩＤを送信
                        writeResult = stst.WriteString(target_ServiceID_CabinetID, writeLineTimeout);
                        // ④検索図面番号送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            // サーバーに送出
                            writer.WriteObject(ZUBANlist);
                        }
                        // ⑤書出し先フォルダを創出
                        writeResult = stst.WriteString(exportUNCFfolder, writeLineTimeout);
                        // ⑥サーバーからArrayListを受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            resultList = reader.ReadObject<List<string>>();
                        }

                        if (writeResult == 0) { return null; } // タイムアウトならnullリターン
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        pipeCltStream.Close();
                        return null;
                    }

                    pipeCltStream.Close();
                    //
                    return resultList;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    pipeCltStream.Close();
                    return null;
                }
            }
        }

        /// <summary>
        /// ■ArcSuiteから図面を検索し見つかればSystem.Drawing.Imageオブジェクトとして取得する。（最初の1ページのみ）
        /// </summary>
        /// <param name="ZUBAN"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public System.Drawing.Image GetArcSuiteLatestDrawing(string ZUBAN, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    System.Drawing.Image result;
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    // ①接続文字の受信とチェック
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                        return null;
                    }
                    if (CheckFirstMessage(input0,delegateWriteLine))
                    {
                        // ②コマンド名送信
                        int writeResult = stst.WriteString(CMDS.DR_GetArcSuiteLatestDrawing);
                        delegateWriteLine($"PIPEコマンド名\"{CMDS.DR_GetArcSuiteLatestDrawing}\"を送信-> 戻り値 writeResult = {writeResult}");

                        // ③検索図面番号送信
                        int writeResult2 = stst.WriteString(ZUBAN);
                        delegateWriteLine($"検索する図面番号 \"{ZUBAN}\"を送信-> 戻り値 writeResult2 = {writeResult2}");

                        // ⑤サーバーからSystemDrawingImageを受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            result = reader.ReadObject<System.Drawing.Image>();

                            delegateWriteLine($"サーバーから Image オブジェクトを受信しました.サイズ：{result.Size}");
                        }
                    }
                    else if (input0 == null)
                    {
                        delegateWriteLine($"コミットサーバーに接続できませんでした(タイムアウト)");
                        return null;
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        pipeCltStream.Close();
                        return null;
                    }

                    pipeCltStream.Close();
                    //
                    return result;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    pipeCltStream.Close();
                    return null;
                }
            }

        }

        /// <summary>
        /// ■ArcSuiteから図面を検索し見つかればファイルとして取得する。
        /// </summary>
        /// <param name="ZUBAN"></param>
        /// <param name="LocalDistFullFileName"></param>
        /// <param name="resultMsg"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public bool GetArcSuiteLatestDrawingFile(string target_ServiceID_CabinetID, string ZUBAN, string LocalDistFullFileName, out string resultMsg, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

            SasaLib.EventsSummary evt = new EventsSummary("ToyoRMDRAWCAPTUREserviceControl", 2003);
            string msg1 = $"① GetArcSuiteLatestDrawingFile(...)実行開始";
            evt.Add(msg1, OutConsole: false);

            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    StreamString stst = new StreamString(pipeCltStream);

                    string token1 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#1 Client <- Server String] 接続文字
                    string msg2 = $"② サーバーから受信 [{token1}]";
                    evt.Add(msg2, OutConsole: false);
                    delegateWriteLine(msg2);

                    if (CheckFirstMessage(token1))
                    {
                        string token2 = CMDS.DR_GetArcSuiteLatestDrawingFile;
                        stst.WriteString(token2); // [■#2 Client -> Server String] コマンド名
                        string msg3 = $"③ サーバーへコマンド名を送信 [{token2}]";
                        evt.Add(msg3, OutConsole: false);
                        delegateWriteLine(msg3);

                        string token3 = target_ServiceID_CabinetID;
                        stst.WriteString(token3); // [■#4 Client -> Server String] 検索図面番号
                        string msg4 = $"④ サーバーへサービスＩＤキャビネットＩＤを送信 [{token3}]";
                        evt.Add(msg4, OutConsole: false);
                        delegateWriteLine(msg4);

                        string token4 = ZUBAN;
                        stst.WriteString(token4); // [■#4 Client -> Server String] 検索図面番号
                        string msg5 = $"⑤ サーバーへ検索図面番号を送信 [{token4}]";
                        evt.Add(msg5, OutConsole: false);
                        delegateWriteLine(msg5);

                        string token5 = stst.ReadString(ReadStreamStringTimeOut, null);  // [■#5 Client <- Server String] サーバー側のコンテンツ実体ファイル名
                        if (token5 == null)
                        {
                            string msg6 = $"⑥ サーバーからｺﾝﾃﾝﾂ実体ﾌｧｲﾙ名を受信結果が null。タイムアウトの可能性あり";
                            evt.Add(msg6, OutConsole: false);
                            delegateWriteLine(msg6);
                            resultMsg = msg6;
                            return false;
                        }
                        else
                        {
                            string msg6 = $"⑥ サーバーからｺﾝﾃﾝﾂ実体ﾌｧｲﾙ名を受信 [{token5}]";
                            evt.Add(msg6, OutConsole: false);
                            delegateWriteLine(msg6);
                        }

                        long DataSize;
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            DataSize = reader.ReadObject<long>(); // [■#6 Client <- Server Data] サーバーソースファイルのサイズ受信
                            string msg7 = $"⑦ サーバーから受信。 DataSize = {DataSize}";
                            evt.Add(msg7, OutConsole: false);
                            delegateWriteLine(msg7);
                        }

                        string token7 = $"[3.OK Data Size:{DataSize} byte] Pleas Send Byte[] Data.";
                        stst.WriteString(token7);// [■#7 Client -> Server String]  データサイズ了解
                        string msg8 = $"⑧ サーバーへ送信 [{token7}]";
                        evt.Add(msg8, OutConsole: false);
                        delegateWriteLine(msg8);

                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            byte[] buf = new byte[DataSize];
                            string msg9 = $"⑨ サーバーからファイル本体の受信を開始。 DataSize = {DataSize}";
                            evt.Add(msg9, OutConsole: false);
                            delegateWriteLine(msg9)
                                ;
                            buf = reader.ReadObject<byte[]>();  // [■#8 Client <- Server Data] サーバー側 ソースファイルバイトデータ
                            string msg10 = $"⑩ サーバーからファイル本体の受信を完了。 buf.Length = {buf.Length}byte";
                            evt.Add(msg10, OutConsole: false);
                            delegateWriteLine(msg10);


                            if (buf != null)
                            {
                                string token9 = $"[.OK. Recevice Data. {buf.Length} byte]";
                                stst.WriteString(token9); // [■#9 Client-> Server String]
                                string msg11 = $"⑪ サーバーへ送信 [{token9}]";
                                evt.Add(msg11, OutConsole: false);
                                delegateWriteLine(msg11);

                                try
                                {
                                    File.WriteAllBytes(LocalDistFullFileName, buf);
                                    resultMsg = $"受信データファイル保存完了 {LocalDistFullFileName} buf.Count()={buf.Length}バイト ";
                                    string msg12 = $"⑫ {resultMsg} メソッドをtrueで完了させます";
                                    evt.Add(msg12, OutConsole: false);
                                    delegateWriteLine(msg12);
                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    resultMsg = $"例外エラー発生：{ex.Message}";
                                    SasaLib.Eventlog.Log.WriteEntry("ToyoRMDRAWCAPTUREserviceControl", EventLogEntryType.Error, 6001, $"【GetArcSuiteLatestDrawingFile(..)】 {ex.Message} ", false);
                                    string msg13 = $"※GetArcSuiteLatestDrawingFile(..) {resultMsg} メソッドをfalseで完了させます";
                                    evt.Add(msg13, OutConsole: false);
                                    delegateWriteLine(msg13);
                                    return false;
                                }
                            }
                            else
                            {
                                resultMsg = $"受信データファイル保存失敗 {LocalDistFullFileName} buf.Count()={buf.Length}バイト ";
                                SasaLib.Eventlog.Log.WriteEntry("ToyoRMDRAWCAPTUREserviceControl", EventLogEntryType.Error, 6001, $"【FileRecv】 {LocalDistFullFileName} を作成・更新に失敗した", false);
                                string msg14 = $"※GetArcSuiteLatestDrawingFile(..) {resultMsg} メソッドをfalseで完了させます";
                                evt.Add(msg14, OutConsole: false);
                                delegateWriteLine(msg14);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        resultMsg = $"書出し失敗";
                        string msg15 = $"※GetArcSuiteLatestDrawingFile(..) {resultMsg} サーバーからの接続回答が期待したものと違います {token1}";
                        evt.Add(msg15, OutConsole: false);
                        delegateWriteLine(msg15);
                        pipeCltStream.Close();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    resultMsg = $"書出し失敗";
                    string msg16 = $"※GetArcSuiteLatestDrawingFile(..) {resultMsg} PIPEサーバー接続エラー{ex.Message}";
                    evt.Add(msg16, OutConsole: false);
                    delegateWriteLine(msg16);
                    pipeCltStream.Close();
                    return false;
                }
            }
        }

        /// <summary>
        /// ■ArcSuiteから図面を検索し見つかればファイルとして取得する。
        /// </summary>
        /// <param name="target_ServiceID_CabinetID"></param>
        /// <param name="ZUBANs"></param>
        /// <param name="LocalDistFolder"></param>
        /// <param name="resultMsg"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public bool GetArcSuiteLatestDrawingFiles(string target_ServiceID_CabinetID, List<string> ZUBANs, string LocalDistFolder, ref List<KeyValuePair<string, string>> fileLists, out string resultMsg, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            resultMsg = null;

            SasaLib.EventsSummary evt = new EventsSummary("ToyoRMDRAWCAPTUREserviceControl", 2003);
            string msg1 = $"① GetArcSuiteLatestDrawingFiles(...)実行開始";
            evt.Add(msg1, OutConsole: false);

            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    StreamString stst = new StreamString(pipeCltStream);

                    //
                    string tokenRead1 = stst.ReadString(ReadStreamStringTimeOut, null); // [■#1 Client <- Server String] 接続文字
                    string msg2 = $"②サーバーから受信 [{tokenRead1}]";
                    evt.Add(msg2, OutConsole: false);
                    delegateWriteLine(msg2);

                    if (CheckFirstMessage(tokenRead1))
                    {
                        //
                        string token2 = CMDS.DR_GetArcSuiteLatestDrawingFiles;
                        stst.WriteString(token2); // [■#2 Client -> Server String] コマンド名
                        string msg3 = $"③GetArcSuiteLatestDrawingFiles(..) token2 サーバーへコマンド名を送信 [{token2}]";
                        evt.Add(msg3, OutConsole: false);
                        delegateWriteLine(msg3);
                        //
                        string token3 = target_ServiceID_CabinetID;
                        stst.WriteString(token3); // [■#3 Client -> Server String] サービスＩＤとキャビネットＩＤ
                        string msg4 = $"④GetArcSuiteLatestDrawingFiles(..) token3 サーバーへサービスＩＤキャビネットＩＤを送信 [{token3}]";
                        delegateWriteLine(msg4);
                        //
                        List<string> token4 = ZUBANs;
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(ZUBANs); // [■#4 Client -> Server String] 検索図面番号リストを送信
                        }
                        string msg5 = $"⑤検索図面番号リストを送信しました";
                        evt.Add(msg5, OutConsole: false);
                        delegateWriteLine(msg5);
                        //
                        int streamCount;
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            int token5 = reader.ReadObject<int>();// [■5# Client -> Server int] ストリーム個数を受信
                            streamCount = token5;
                        }
                        string msg6 = $"⑥ストリーム個数を受信 [{streamCount}]";
                        evt.Add(msg6, OutConsole: false);
                        delegateWriteLine(msg6);
                        //
                        int token6 = streamCount;
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(token6); // [■#6 Client -> Server String] トリーム個数を送信
                        }
                        string msg7 = $"⑦サーバーへストリーム個数を送信 {streamCount}";
                        evt.Add(msg7, OutConsole: false);
                        delegateWriteLine(msg7);
                        //

                        for (int i = 0; i < token6; i++)
                        {
                            //
                            string token7 = stst.ReadString(ReadStreamStringTimeOut, null);  // [■#7 Client <- Server String] サーバーから図面番号を受信
                            string zuban = token7;
                            string msg8 = $"⑦-1 サーバーから図面番号を受信 [{zuban}]";
                            evt.Add(msg8, OutConsole: false);
                            delegateWriteLine(msg8);
                            //
                            string token8 = stst.ReadString(ReadStreamStringTimeOut, null);  // [■#8 Client <- Server String] サーバー側のコンテンツ実体ファイル名
                            string receveFileName = token8;
                            string msg9 = $"⑦-2 サーバーからｺﾝﾃﾝﾂ実体ﾌｧｲﾙ名を受信 [{receveFileName}]";
                            evt.Add(msg9, OutConsole: false);
                            delegateWriteLine(msg9);
                            string LocalDistFullFileName = System.IO.Path.Combine(LocalDistFolder, receveFileName);
                            //
                            long DataSize;
                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                DataSize = reader.ReadObject<long>(); // [■#9 Client <- Server Data] サーバーソースファイルのサイズ受信
                                delegateWriteLine($"■GetArcSuiteLatestDrawingFile(..) token9 サーバーから受信。 DataSize = {DataSize}");
                            }
                            string msg10 = $"⑦-3 サーバーからファイルサイズを受信 [{DataSize}]";
                            evt.Add(msg10, OutConsole: false);
                            delegateWriteLine(msg10);
                            //
                            string token10 = $"[3.OK Data Size:{DataSize} byte] Pleas Send Byte[] Data.";
                            stst.WriteString(token10);// [■#10 Client -> Server String]  データサイズ了解
                            string msg11 = $"⑦-4 サーバーへ送信 [{token10}]";
                            evt.Add(msg11, OutConsole: false);
                            delegateWriteLine(msg11);
                            //
                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                byte[] buf = new byte[DataSize];
                                string msg12 = $"⑦-5 サーバーからファイル本体の受信を開始。 DataSize = {DataSize}";
                                evt.Add(msg12, OutConsole: false);
                                delegateWriteLine(msg12);

                                buf = reader.ReadObject<byte[]>();  // [■#11 Client <- Server Data] サーバー側 ソースファイルバイトデータ

                                string msg13 = $"⑦-6 サーバーからファイル本体の受信を完了。 buf.Length = {buf.Length}byte";
                                evt.Add(msg13, OutConsole: false);
                                delegateWriteLine(msg13);


                                if (buf != null)
                                {
                                    string token12 = $"[.OK. Recevice Data. {buf.Length} byte ]";
                                    stst.WriteString(token12); // [■#12 Client-> Server String]
                                    string msg14 = $"⑦-7 サーバーへ送信 [{token12}]";
                                    evt.Add(msg14, OutConsole: false);
                                    delegateWriteLine(msg14);

                                    try
                                    {

                                        File.WriteAllBytes(LocalDistFullFileName, buf);
                                        resultMsg = $"受信データファイル保存完了 {LocalDistFullFileName} buf.Count()={buf.Length}バイト ";

                                        fileLists.Add(new KeyValuePair<string, string>(zuban, LocalDistFullFileName));

                                        string msg15 = $"⑦-8  {resultMsg} 受信データファイル保存完了";
                                        evt.Add(msg15, OutConsole: false);
                                        delegateWriteLine(msg15);
                                    }
                                    catch (Exception ex)
                                    {
                                        resultMsg = $"例外エラー発生：{ex.Message}";
                                        string msg16 = $"※GetArcSuiteLatestDrawingFiles(..) {resultMsg}";
                                        evt.Add(msg16, OutConsole: false);
                                        delegateWriteLine(msg16);
                                    }
                                }
                                else
                                {
                                    resultMsg = $"受信データファイル保存失敗 {LocalDistFullFileName} buf.Count()={buf.Length}バイト ";
                                    stst.WriteString(resultMsg); // [■#12 Client-> Server String]
                                    string msg17 = $"※GetArcSuiteLatestDrawingFile(..) {resultMsg}";
                                    evt.Add(msg17, OutConsole: false);
                                    delegateWriteLine(msg17);
                                }
                            }

                        }
                    }
                    else
                    {
                        resultMsg = $"書出し失敗";
                        string msg18 = $"※GetArcSuiteLatestDrawingFile(..) {resultMsg} サーバーからの接続回答が期待したものと違います {tokenRead1}";
                        evt.Add(msg18, OutConsole: false);
                        delegateWriteLine(msg18);
                        pipeCltStream.Close();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    resultMsg = $"書出し失敗";
                    string msg19 = $"※GetArcSuiteLatestDrawingFile(..) {resultMsg} PIPEサーバー接続エラー{ex.Message}";
                    evt.Add(msg19, OutConsole: false);
                    delegateWriteLine(msg19);
                    pipeCltStream.Close();
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region ●メソッド 図面イメージ取得系

        /// <summary>
        ///■ PIPE ストリームからGUIDBASE64を指定しイメージを得る　"LoadImage"
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <returns></returns>
        public System.Drawing.Image GetImageFromPIPE(string GUIDBASE64, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            if (GUIDBASE64 == null)
                return null;
            System.Drawing.Bitmap BitmapData = null;

            //SasaLib.StopWatch stopWatch = new StopWatch($"GetImageFromPIPE");
            NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
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
                    return null;
                }
                if (CheckFirstMessage(input0))
                {
                    int writeResult = stst.WriteString(CMDS.DR_LoadImage);
                    stst.WriteString(GUIDBASE64);
                    // サーバからレスポンスを受信する
                    using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                    {
                        BitmapData = reader.ReadObject<Bitmap>();
                    }
                }
                else
                {
                    delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                    return null;
                }
                ///
                pipeCltStream.Close();
                //stopWatch.Stop("RMloadImage({GUIDBASE64}・・・)");
                return BitmapData;
            }
            catch (Exception ex)
            {
                delegateWriteLine($"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                pipeCltStream.Close();
                return null;
            }
        }

        /// <summary>
        /// ■ PIPE ストリームからGUIDBASE64とクリップ範囲を指定しイメージを得る "LoadImage2"
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <param name="ClipW"></param>
        /// <param name="ClipH"></param>
        /// <returns></returns>
        public System.Drawing.Image GetImageFromPIPE2(string GUIDBASE64, int ClipW = 4677, int ClipH = 900, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            if (GUIDBASE64 == null)
                return null;
            delegateWriteLine($"GetImageFromPIPE2({GUIDBASE64})実行");
            System.Drawing.Bitmap BitmapData = null;
            NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
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
                    return null;
                }
                if (CheckFirstMessage(input0))
                {
                    int writeResult = stst.WriteString(CMDS.DR_LoadImage2);
                    stst.WriteString(GUIDBASE64);
                    string AnsMsg = stst.ReadString(ReadStreamStringTimeOut, null);
                    if (AnsMsg != "NoImage")
                    {
                        Console.Write($"大きさ受信 {AnsMsg}・・");
                        string[] xy = AnsMsg.Replace(" ", "").Split(',');
                        int x = Convert.ToInt32(xy[0]);
                        int y = Convert.ToInt32(xy[1]);
                        string WW = ClipW.ToString();
                        string HH = ClipH.ToString();
                        string XX = (x - ClipW).ToString();
                        string YY = (y - ClipH).ToString();
                        delegateWriteLine($"・・クリップ範囲送信 {XX},{YY},{WW},{HH}");
                        stst.WriteString($"{XX},{YY},{WW},{HH}");
                        // サーバからレスポンスを受信する
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            BitmapData = reader.ReadObject<Bitmap>();
                        }
                    }
                }
                else
                {
                    delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                    return null;
                }
                pipeCltStream.Close();
                return BitmapData;
            }
            catch (Exception ex)
            {
                delegateWriteLine($"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                pipeCltStream.Close();
                return null;
            }
        }

        /// <summary>
        /// 直接ファイルストアからイメージ全てをゲット
        /// </summary>
        /// <param name="Folder"></param>
        /// <param name="fieldValueSet"></param>
        /// <returns></returns>
        public System.Drawing.Image GetImageFromFile(string Folder, FieldValueSet fieldValueSet, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            if (String.IsNullOrWhiteSpace(Folder) || fieldValueSet == null)
            {
                delegateWriteLine("▼エラー。RemoteClientDRAWREGIST.GetImageFromFile(...) 引数にnullまたは空行が与えられましt");
                return null;
            }
            var tiffImageFullPath = $"{Folder}\\{fieldValueSet.SearchKey("TICKETCODE")}.TIF";
            try
            {
                var img = SasaLib.ImageUtil.FromFile(tiffImageFullPath);
                return img;
            }
            catch (IOException ioe)
            {
                delegateWriteLine($"東陽機械技術部 承認登録クライアント PIEP接続失敗{ioe.Message}\n");
                return null;
            }
        }

        #endregion

        #region ●メソッド ArcSuite登録指示/ ArcSuite属性値変更系

        /// <summary>
        /// ■DB内にArcSuite登録予定を示すフラグをに立てる(複数指定)　"SetRegistWaitingFlag"
        /// </summary>
        /// <param name="GUIDBASE64List">登録予定のGUIDBASE64コレクション</param>
        /// <param name="USERID"></param>
        public void SetRegistWaitingFlag(List<string> GUIDBASE64List, string USERID, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    AnserMessage = $"PIPE接続失敗 {ex.Message}";
                    delegateWriteLine($"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                    return;
                }
                // サーバーからの書き込みを受け取ります。
                StreamString stst = new StreamString(pipeCltStream);
                // ① サーバーメッセージを受信
                bool OperationCanceledException;
                bool AggregateException;

                string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                if (OperationCanceledException || AggregateException)
                {
                    delegateWriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                    return;
                }
                // ② サーバーメッセージをチェック
                if (CheckFirstMessage(input0))
                {
                    // ③コマンド送信
                    int writeResult = stst.WriteString(CMDS.DR_SetRegistWaitingFlag);
                    // ④GUIDBASE64Listオブジェクトを送信
                    using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                    {
                        writer.WriteObject(GUIDBASE64List); //④send
                    }
                    // ⑤承認実行者のID 例 0123 を送信
                    stst.WriteString(USERID);
                }
                else
                {
                    delegateWriteLine("Server could not be verified.");
                }
                //
                pipeCltStream.Close();
            }
            delegateWriteLine($"GUIDBASE64List.Count = {GUIDBASE64List.Count}");
            for (int i = 0; i > GUIDBASE64List.Count; i++)
            {
                delegateWriteLine($"index = {i}");
            }
        }

        /// <summary>
        /// ■ArcSuite図面の属性を変更（複数候補）。"MergeArcSuiteAtrtribute"
        /// </summary>
        /// <param name="ZUBANlistStr">属性を変更する図番のコレクション</param>
        /// <param name="ArcSuiteAttrField">変更する属性名</param>
        /// <param name="ArcSuiteAttrValue">変更する属性値</param>
        /// <param name="ArcSuiteUserID">実際に変更を行うアークスイートユーザー</param>
        /// <param name="ArcSuiteUserPassword">アークスイートユーザーのパスワード</param>
        /// <returns></returns>
        public List<KeyValuePair<string, bool>> MergeArcSuiteAttribute(List<string> ZUBANlistStr, string ArcSuiteAttrField, string ArcSuiteAttrValue, string ArcSuiteUserID, string ArcSuiteUserPassword, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            delegateWriteLine($"{ArcSuiteAttrField} {ArcSuiteAttrValue}\n{ArcSuiteUserID},{ArcSuiteUserPassword}");
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    List<KeyValuePair<string, bool>> results = new List<KeyValuePair<string, bool>>();
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    // ①接続文字の受信とチェック
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
                        // ②コマンド名送信
                        int writeResult = stst.WriteString(CMDS.DR_MergeArcSuiteAtrtribute);
                        // ③検索図面番号送信
                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            // サーバーに送出
                            writer.WriteObject(ZUBANlistStr);
                        }
                        //  ④対象のArcSuiteユーザー属性名を送信
                        stst.WriteString(ArcSuiteAttrField);
                        //  ⑤属性値を送信
                        stst.WriteString(ArcSuiteAttrValue);
                        // ⑥変更を実行するアークスイートユーザーIDを送信
                        stst.WriteString(ArcSuiteUserID);
                        // ⑦パスワードを送信
                        stst.WriteString(ArcSuiteUserPassword);
                        // サーバーからList<string>を受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            results = reader.ReadObject<List<KeyValuePair<string, bool>>>();
                        }
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        pipeCltStream.Close();
                        return null;
                    }
                    pipeCltStream.Close();
                    //
                    return results;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"東陽機械技術部 承認登録クライアント PIEP接続失敗{ex.Message}\n");
                    pipeCltStream.Close();
                    return null;
                }
            }
        }

        /// <summary>
        /// ■ArcSuite図面の属性を変更（1つの図番）。"MergeArcSuiteAtrtribute1"
        /// </summary>
        /// <param name="Zuban"></param>
        /// <param name="ArcSuiteAttrField"></param>
        /// <param name="ArcSuiteAttrValue"></param>
        /// <param name="ArcSuiteUserID"></param>
        /// <param name="ArcSuiteUserPassword"></param>
        /// <returns></returns>
        public bool MergeArcSuiteAttribute1(string Zuban, string ArcSuiteAttrField, string ArcSuiteAttrValue, string ArcSuiteUserID, string ArcSuiteUserPassword, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                // 待機中のサーバーへ接続
                try
                {
                    pipeCltStream.Connect(ClientTimeOut);
                    bool Ans = false;
                    // サーバーからの書き込みを受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    // ①接続文字の受信とチェック
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
                        // ②コマンド名送信
                        int writeResult = stst.WriteString(CMDS.DR_MergeArcSuiteAtrtribute1);
                        // ③検索図面番号送信
                        stst.WriteString(Zuban);
                        //  ④対象のArcSuiteユーザー属性名を送信
                        stst.WriteString(ArcSuiteAttrField);
                        //  ⑤属性値を送信
                        stst.WriteString(ArcSuiteAttrValue);
                        // ⑥変更を実行するアークスイートユーザーIDを送信
                        stst.WriteString(ArcSuiteUserID);
                        // ⑦パスワードを送信
                        stst.WriteString(ArcSuiteUserPassword);
                        // サーバーから結果を受信
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            Ans = reader.ReadObject<bool>();
                        }
                    }
                    else
                    {
                        delegateWriteLine($"▲サーバーからの接続回答が期待したものと違います {input0}");
                        pipeCltStream.Close();
                        return false;
                    }
                    //
                    pipeCltStream.Close();
                    return Ans;
                }
                catch (Exception ex)
                {
                    delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                    pipeCltStream.Close();
                    return false;
                }
            }
        }

        #endregion

        #region ●メソッド 関連サーバー状態チェック・ステータス取得系

        /// <summary>
        /// ■FILESTORE内のファイルでデータベースにリンクされていない個数を調査 "SystemCheck" "FILESTORE_CHECK"
        /// </summary>
        /// <returns></returns>
        public int SystemCheckFileStore(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
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
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                        Debug.WriteLine($"PIPEサーバー接続エラー {ex.Message}");
                        delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                        return -1;
                    }
                    // サーバーからのサーバ識別文字列を受け取ります。
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
                        string command = CMDS.DC_DR_SystemCheck;
                        delegateWriteLine($"【{command}】を送信");
                        stst.WriteString(command);
                        string str = CMDS.DR_SystemCheck_FILESTORE_CHECK;
                        delegateWriteLine($"【{str}】を送信");
                        stst.WriteString(str);
                        List<string> FileNameStringList = new List<string>();
                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            FileNameStringList = reader.ReadObject<List<string>>();

                            delegateWriteLine($"【{FileNameStringList.Count}】件あります");

                            foreach (var x in FileNameStringList)
                            {
                                delegateWriteLine($"非管理ファイル：{x}");
                            }
                        }
                        pipeCltStream.Close();

                        return FileNameStringList.Count;

                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return -1;
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");

                }
            }
            return -1;
        }

        /// <summary>
        /// ■FILESTORE内のファイルでデータベースにリンクされてないファイルをゴミとして削除　"SystemCheck" "FILESTORE_REPARE"
        /// </summary>
        /// <returns></returns>
        public bool SystemCheckFileStoreRepare(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
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
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine(ex.Message);
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        return false;
                    }
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
                        string command = CMDS.DC_DR_SystemCheck;
                        delegateWriteLine($"【{command}】を送信");
                        stst.WriteString(command);
                        string str = CMDS.DR_SystemCheck_FILESTORE_REPARE;
                        delegateWriteLine($"【{str}】を送信");
                        stst.WriteString(str);
                        /// サーバーから結果情報を取得
                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
                        delegateWriteLine($"SystemCheckFileStoreRepare()【{AnserMessage}】を受信しました");
                        pipeCltStream.Close();
                        return true;
                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return false;
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");

                }
            }
            return false;
        }

        /// <summary>
        /// PIPEサーバーにコミット待ちチケットがあるか問い合わせる "SystemCheck" "TICKETFILE_EXIST_CHECK"
        /// 廃止予定
        /// </summary>
        /// <returns></returns>
        public List<FieldValueSet> SystemCheckTICKETFILEexist(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            List<FieldValueSet> LinkDownRecords = new List<FieldValueSet>();

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
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        return null;
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
                        string command = CMDS.DC_DR_SystemCheck;
                        delegateWriteLine($"【{command}】を送信");
                        stst.WriteString(command);

                        string str = CMDS.DR_SystemCheck_TICKETFILE_EXIST_CHECK;
                        delegateWriteLine($"【{str}】を送信");
                        stst.WriteString(str);

                        /// サーバーから結果情報を取得
                        using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            LinkDownRecords = reader.ReadObject<List<FieldValueSet>>();
                        }

                        delegateWriteLine($"SystemCheckFileStoreRepare()【{AnserMessage}】を受信しました");

                        pipeCltStream.Close();
                        return LinkDownRecords;
                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return null;
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");

                }
            }
            return null;

        }

        /// <summary>
        /// ■サーバへの接続可否を確認する
        /// </summary>
        /// <returns></returns>
        public bool ConnectTest(string sendTestMsg, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

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
        /// ■アークスイートサーバーの生存状態をチェック
        /// </summary>
        /// <returns></returns>
        public bool CheckArcSuiteAndDatabaseServer(ref bool ArcSuiteALive, ref bool DataBaseAlive, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            bool IsOutMsg = false;
            if (delegateWriteLine == null) { delegateWriteLine = Console.WriteLine; IsOutMsg = false; }

            DateTime dt1 = DateTime.Now;
            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy, debugConsoleMsg: false))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                        if (IsOutMsg)
                            delegateWriteLine($"{DateTime.Now.ToString()} CheckArcSuiteAndDatabaseServer(...),PIPEサーバー {PipeServerName} PIPE名 {pipename} 接続成功 IsConnected={pipeCltStream.IsConnected}");

                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"CheckArcSuiteAndDatabaseServer(...),PIPEサーバー {PipeServerName} PIPE名 {pipename} 接続エラー IsConnected={pipeCltStream.IsConnected}\n{ex.Message}");
                        pipeCltStream.Close();
                        return false;
                    }
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
                        if (IsOutMsg)
                            delegateWriteLine($"■CheckServerAlive():{dt1}:サーバーからの接続文字列 \"{input0}\" は期待値です");

                        if (pipeCltStream.IsConnected == false)
                        {
                            delegateWriteLine($"●pipeCltStream.IsConnected == false");
                            return false;
                        }

                        int writeResult = stst.WriteString(CMDS.DC_DR_Status);

                        if (writeResult == -1)
                            throw new Exception($"コマンド 送信に失敗しました \"AAAAA \"");

                        if (pipeCltStream.IsConnected == false)
                        {
                            delegateWriteLine($"●pipeCltStream.IsConnected == false");
                            return false;
                        }

                        stst.WriteString(CMDS.DR_Status_SERVER_ALIVE);

                        string Msg1 = stst.ReadString(ReadStreamStringTimeOut, null);
                        if (IsOutMsg)
                            delegateWriteLine($"■CheckServerAlive():{dt1}:【{Msg1}】を受信しました");
                        string Msg2 = stst.ReadString(ReadStreamStringTimeOut, null);
                        if (IsOutMsg)
                            delegateWriteLine($"■CheckServerAlive():{dt1}:【{Msg2}】を受信しました");
                        if (Msg1 == "ArcSuite正常")
                        {
                            ArcSuiteALive = true;
                        }
                        else
                            ArcSuiteALive = false;

                        if (Msg2 == "DataBase正常")
                        {
                            DataBaseAlive = true;
                        }
                        else
                            DataBaseAlive = false;
                        pipeCltStream.Close();
                        return true;
                    }
                    else
                    {
                        ArcSuiteALive = false;
                        DataBaseAlive = false;
                        delegateWriteLine($"■CheckServerMode():{dt1}:サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return false;
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    ArcSuiteALive = false;
                    DataBaseAlive = false;
                    delegateWriteLine($"CheckServerAlive(...) \nPIPEサーバー接続エラー\n {ex.Message}");
                }
                return true;
            }
        }

        /// <summary>
        /// ■承認処理受付可能かを調べる
        /// </summary>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string ApprovalRecepitonState(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy, debugConsoleMsg: false))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;

                        delegateWriteLine($"PIPEサーバーへの接続エラー {ex.Message}");
                        return $"承認受付サーバーは停止しているようです。\nサーバー{PipeServerName}の ネットワークパイプ:{pipename} に接続できません";
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
                        //delegateWriteLine($"ステージサーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DR_ApprovalRecepitonState);

                        if (writeResult == -1)
                            throw new Exception($"ハンドシェイク後のコマンド送信に失敗 \"{CMDS.DR_ApprovalRecepitonState}\"");

                        string ApprovalRecepitonState = stst.ReadString(ReadStreamStringTimeOut, null);

                        pipeCltStream.Close();

                        return ApprovalRecepitonState;
                    }
                    else
                    {
                        delegateWriteLine($"PIPEサーバーへのからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return $"ステージサーバーからの接続文字列{input0}が期待と違います";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;

                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
                }
                return $"PIPEサーバー接続エラー";
            }
        }

        /// <summary>
        /// ■データベースサーバホスト問合せ
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseHOSTNAME(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

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
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"DATABASEHOST()\nPIPEサーバー接続エラー\n {ex.Message}");
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
                        // delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_Status);
                        stst.WriteString(CMDS.DR_Status_DATABASE_HOSTNAME);
                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
                        //delegateWriteLine($"DATABASEHOST()【{AnserMessage}】を受信しました");
                        pipeCltStream.Close();
                        return AnserMessage;
                    }
                    else if (input0 == null)
                    {
                        delegateWriteLine($"コミットサーバーに接続できませんでした(タイムアウト)");
                        return "エラー";
                    }
                    else
                    {
                        delegateWriteLine($"DATABASEHOST(...)サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"DATABASEHOST(...) PIPEサーバー接続エラー\n{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■クライアントから見たコミット先フォルダを問い合わせ
        /// </summary>
        /// <returns></returns>
        public string GetCommitFolderPATH(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

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
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6001, $"PIPEサーバー接続エラー\n{ex.Message}");
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
                        // delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_Status);
                        stst.WriteString(CMDS.DR_Status_COMMITPATH);
                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
                        delegateWriteLine($"GetCommitPATH()【{AnserMessage}】を受信しました");
                        pipeCltStream.Close();
                        return AnserMessage;
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

                    delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■クライアントから見たFileStoreパスを問い合わせる
        /// </summary>
        /// <returns></returns>
        public string GetFileStoreFolderPATH(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                    // 待機中のサーバーへ接続
                    pipeCltStream.Connect(ClientTimeOut);
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
                        // delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_Status);
                        stst.WriteString(CMDS.DR_Status_FILESTOREPATH);
                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
                        //delegateWriteLine($"GetFileStorePATH()を実行【{AnserMessage}】を受信しました");
                        pipeCltStream.Close();
                        return AnserMessage;
                    }
                    else if (input0 == null)
                    {
                        delegateWriteLine($"コミットサーバーに接続できませんでした(タイムアウト)");
                        return "エラー";
                    }
                    else
                    {
                        delegateWriteLine($"GetFileStorePATH()を実行。しかしサーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return "エラー";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;
                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");
                    return "エラー";
                }
            }
        }

        /// <summary>
        /// ■クライアントから見たアークスイートサーバーホスト問い合わせ
        /// </summary>
        /// <returns></returns>
        public string Get_Status_ARCSUITE_SERVERHOST(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            bool IsOutMsg = false;
            if (delegateWriteLine == null) { delegateWriteLine = Console.WriteLine; IsOutMsg = false; }

            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy, debugConsoleMsg: false))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(ClientTimeOut);
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;

                        delegateWriteLine(ex.Message);
                        delegateWriteLine($"DATABASEHOST()\nPIPEサーバー接続エラー\n {ex.Message}");
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
                        // delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_Status);
                        stst.WriteString(CMDS.DR_Status_ARCSUITE_SERVERHOST);
                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
                        if (IsOutMsg)
                            delegateWriteLine($"{DateTime.Now.ToString()} ARCSUITESERVERHOST()【{AnserMessage}】を受信しました");
                        pipeCltStream.Close();
                        return AnserMessage;
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

                }
                return "エラー";
            }

        }

        /// <summary>
        /// ■アークスイート登録済みファイルの保存場所のパス問い合わせ
        /// </summary>
        /// <returns></returns>
        public string GetArcSuiteWorkingFolderPATH(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

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
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
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
                        // delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_Status);
                        stst.WriteString(CMDS.DR_Status_ARCSUITE_WORKFOLDER);
                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
                        delegateWriteLine($"ARCSUITE WORKFOLDER()【{AnserMessage}】を受信しました");
                        pipeCltStream.Close();
                        return AnserMessage;
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
                    delegateWriteLine($"PIPEサーバー接続エラー\n{ex.Message}");

                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■パイプコネクションをチェックする
        /// </summary>
        /// <returns></returns>
        public string GETPIPECONNECTION(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

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
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
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
                        // delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_Status);
                        stst.WriteString(CMDS.DC_DR_Status_GET_PIPE_CONNECTION);
                        string AnserMessage = stst.ReadString(ReadStreamStringTimeOut, null);
                        delegateWriteLine($"ARCSUITE WORKFOLDER()【{AnserMessage}】を受信しました");
                        pipeCltStream.Close();
                        return AnserMessage;
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
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■ToyoDRAWREGISTserviceのログレベル変数をセット
        /// </summary>
        /// <param name="Level"></param>
        /// <returns></returns>
        public string SetDRAWREGISTserviceLogLevel(int Level, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

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
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
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
                        // delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl);
                        stst.WriteString(CMDS.DC_DR_SW_SS_ServerCOntorl_CONSOLE_LOGLEVEL_SET);

                        using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                        {
                            writer.WriteObject(Level); //④send
                        }
                        pipeCltStream.Close();
                        return $"DRAWREGISTserviveにログレベル{Level}を送りました";
                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return $"DRAWREGISTserviveにログレベル{Level}を送るのを失敗！！";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;

                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6001, $"PIPEサーバー接続エラー\n{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■ToyoDRAWREGISTserviceのログレベル変数を取得
        /// </summary>
        /// <param name="Level"></param>
        /// <returns></returns>
        public string GetDRAWREGISTserviceLogLevel(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

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
                        // delegateWriteLine($"サーバーからの接続文字列{input0}は期待値です");
                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl);
                        stst.WriteString(CMDS.DC_DR_SW_SS_ServerControl_CONSOLE_LOGLEVEL_GET);
                        string currentLogLevel = stst.ReadString(ReadStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);

                        if (OperationCanceledException == true || AggregateException == true)
                        {
                            MessageBox.Show($"タイムアウト又はその他のエラー OperationCanceledException:{OperationCanceledException} AggregateException:{AggregateException} ");
                        }
                        pipeCltStream.Close();

                        return currentLogLevel;

                    }
                    else
                    {
                        delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return $"DRAWREGISTserviveからログレベルの取得に失敗！！";
                    }
                    // Give the client process some time to display results before exiting.
                }
                catch (Exception ex)
                {
                    PipeConnectionStatus = false;

                    delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                }
                return "エラー";
            }
        }

        /// <summary>
        /// ■DRAWREGISTserviceのPIPEサーバーのアセンブリバージョンを得る
        /// </summary>
        /// <returns></returns>
        public string GetDRAWREGISTserviceVersion(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;

            using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
            {
                try
                {
                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(this.PipeServerName, pipename);
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
                        int writeResult = stst.WriteString(CMDS.DC_DR_SW_GetVersion);

                        string ServerVersion = stst.ReadString(ReadStreamStringTimeOut, null);

                        pipeCltStream.Close();

                        return ServerVersion;
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
                }
                return "エラー";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public string GetPipeCommandLog(string pipeName, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            try
            {


                if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;
                int ClientTimeOut = 10000;
                //bool PipeConnectionStatus = false;
                int ReadStreamStringTimeOut = 10000;
                using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy, debugConsoleMsg: false))
                {
                    try
                    {
                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(this.PipeServerName, pipename);
                        // 待機中のサーバーへ接続
                        try
                        {
                            pipeCltStream.Connect(ClientTimeOut);
                        }
                        catch (Exception ex)
                        {
                            //PipeConnectionStatus = false;

                            delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
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
                            int writeResult = stst.WriteString(CMDS.DC_DR_SW_GetPipeCommandLog);
                            if (writeResult == -1)
                                throw new Exception("PIPEコマンドを送信できませんでした");

                            string pipeCommandLog = stst.ReadString(ReadStreamStringTimeOut, null);

                            pipeCltStream.Close();

                            return pipeCommandLog;
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
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    }
                    return "エラー";
                }
            }
            catch (Exception ex)
            {
                delegateWriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public List<AcceptPipeCommand> GetAuthorizedUser(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            try
            {
                List<AcceptPipeCommand> connectClients;


                if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;
                int ClientTimeOut = 10000;
                using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy, debugConsoleMsg: false))
                {
                    try
                    {
                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(this.PipeServerName, pipename);
                        // 待機中のサーバーへ接続
                        try
                        {
                            pipeCltStream.Connect(ClientTimeOut);
                        }
                        catch (Exception ex)
                        {
                            PipeConnectionStatus = false;

                            delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                            return null;
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
                            int writeResult = stst.WriteString(CMDS.DR_GetAuthorizedUser);

                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                connectClients = reader.ReadObject<List<AcceptPipeCommand>>(); // ｻｰﾊﾞｰからオブジェクト受信
                            }


                            return connectClients;
                        }
                        else
                        {
                            delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return null;
                        }
                        // Give the client process some time to display results before exiting.
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                delegateWriteLine(ex.Message);
            }
            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public List<ClientPreInputTICKET> GetCommonApprovalWaitingTicketList(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            List<ClientPreInputTICKET> CommonApprovalWaitingTicketList;

            try
            {
                if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;
                int ClientTimeOut = 10000;
                //bool PipeConnectionStatus = false;
                //int ReadStreamStringTimeOut = 10000;
                using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy, debugConsoleMsg: false))
                {
                    try
                    {
                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(this.PipeServerName, pipename);
                        // 待機中のサーバーへ接続
                        try
                        {
                            pipeCltStream.Connect(ClientTimeOut);
                        }
                        catch (Exception ex)
                        {
                            PipeConnectionStatus = false;

                            delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                            return null;
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
                            int writeResult = stst.WriteString(CMDS.DR_GetCommonApprovalWaitingTicketList);

                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                CommonApprovalWaitingTicketList = reader.ReadObject<List<ClientPreInputTICKET>>(); // ｻｰﾊﾞｰからオブジェクト受信
                            }

                            pipeCltStream.Close();

                            return CommonApprovalWaitingTicketList;
                        }
                        else
                        {
                            delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return null;
                        }
                        // Give the client process some time to display results before exiting.
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                delegateWriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TICKETCODEs"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public List<string> RemovePreInputTIKECTCODEs(List<string> TICKETCODEs, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;
            List<string> result;


            try
            {
                if (delegateWriteLine == null) delegateWriteLine = Console.WriteLine;
                int ClientTimeOut = 10000;
                //bool PipeConnectionStatus = false;
                //int ReadStreamStringTimeOut = 10000;
                using (new ClsLogonDummy(DomainName, UserName, UserPassword, ClsLogonDummy))
                {
                    try
                    {
                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(this.PipeServerName, pipename);
                        // 待機中のサーバーへ接続
                        try
                        {
                            pipeCltStream.Connect(ClientTimeOut);
                        }
                        catch (Exception ex)
                        {
                            PipeConnectionStatus = false;

                            delegateWriteLine($"PIPEサーバー接続エラー {ex.Message}");
                            return null;
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
                            stst.WriteString(CMDS.DR_RemovePreInputTIKECTCODEs);

                            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                            {
                                // サーバーに送出
                                writer.WriteObject(TICKETCODEs);
                            }

                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                result = reader.ReadObject<List<string>>(); // ｻｰﾊﾞｰからオブジェクト受信
                            }

                            pipeCltStream.Close();

                            return result;
                        }
                        else
                        {
                            delegateWriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return null;
                        }
                        // Give the client process some time to display results before exiting.
                    }
                    catch (Exception ex)
                    {
                        PipeConnectionStatus = false;
                        delegateWriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                delegateWriteLine(ex.Message);
                return null;
            }
        }

        #endregion

    }
}


