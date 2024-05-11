using SasaLib;
using SasaLib.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using SIP = System.IO.Path;
using ToyoMcMfg.Staging.DataBaseConfig;
using SharedClassLibrary;
using System.Windows.Forms;
using MailNotice;

namespace ToyoStageService
{
    /// <summary>
    /// ■ステージデータベース検索クラス
    /// </summary>
    public class DataBaseSearch
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        static MailAccount mailAccount = new MailAccount(
            StageServerConfig.Config.EMAILSERVER,
            StageServerConfig.Config.EMAILSENDPORT,
            StageServerConfig.Config.EMAILNOTICE_SendToADDR,
            StageServerConfig.Config.SMTPAUTHUSER,
            StageServerConfig.Config.SMTPAUTHPASS_SasaLibEncryptionType,
            StageServerConfig.Config.SMTPAUTHPASS
        );

        /// <summary>
        /// 検索結果を保持(結果セットが複数の場合は最後のもの)
        /// </summary>
        public FieldValueSet DBresultOne { set; get; }

        /// <summary>
        /// 検索結果を保持(結果セットが複数)
        /// </summary>
        public List<FieldValueSet> DBresultList { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataBaseSearch()
        {
        }

        /// <summary>
        /// コンストラクタ(データベースを検索)
        /// ORDER BY ID DESC (IDを降順でソート 99,98,97,96,95....)
        /// </summary>
        /// <param name="TableName">検索対象のテーブル名</param>
        /// <param name="FieldName">検索フィールド名</param>
        /// <param name="FieldValue">検索するフィールドの値</param>
        /// <param name="FieldNameList">検索結果に表示するフィールドリスト</param>
        /// <param name="SelectOption">SQLのSelectオプションを指定</param>
        /// <param name="TIFFfolder">書き出しフォルダを指定。""の場合は何もしない</param>
        public DataBaseSearch(string TableName, string FieldName, string FieldValue, List<string> FieldNameList, string clientInfo, string SelectOption = "TOP (5000)", string TIFFfolder = null, bool EventLog = true)
        {
            DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName}  {clientInfo}【DataBaseSearch(...)】開始");

            if (string.IsNullOrEmpty(FieldValue))
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5104, $"{AssemblyInternalName}  {clientInfo}  DataBaseSearch(...)　引数 FieldValueが空です。メソッドを終了します");
                return;
            }

            DBresultList = new List<FieldValueSet>();

            // 
            string FieldNames = string.Join(",", FieldNameList);

            DBresultOne = new FieldValueSet(); // 1行も検索されなかった時のための保険

            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

            try
            {
                if (
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DATASOURCE) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBNAME) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUser) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUserPass)
                    )
                {
                    throw new Exception("設定ﾌｧｲﾙの DATASOURCE , DBNAME , DBcontrolUser , DBcontrolUserPass のどれかまたは全てが未設定です");
                }

                //DBに接続開始
                SQLSV db = new SQLSV(
                    StageServerConfig.Config.DATASOURCE,            // ホスト名
                    StageServerConfig.Config.DBNAME,            // データベース名
                    StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
                    encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)  // 接続ユーザーパスワード(暗号化を解除しています)
                    );

                if (db != null)
                {
                    using (SqlCommand command = db.connection.CreateCommand())
                {
                    db.connection.Open();
                    command.CommandText = $"SELECT {SelectOption} {FieldNames} FROM {TableName} WHERE {FieldName} COLLATE Japanese_CS_AS_KS_WS LIKE  @SEARCH ORDER BY ID DESC";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@SEARCH", SqlDbType.NVarChar, FieldValue.Count())).Value = FieldValue.Trim(); //トリム

                    DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName}  {clientInfo}【DataBaseSearch(...)】\n SQLコマンドライン：{command.CommandText}");

                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        FieldValueSet fieldValueSet = new FieldValueSet();

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            fieldValueSet.Params.Add(new FieldValueSet.Param
                            {
                                Field = dataReader.GetName(i),
                                Value = dataReader.GetValue(i).ToString(),
                                SqlDBType = (SqlDbType)(int)dataReader.GetSchemaTable().Rows[i]["ProviderType"]
                            });
                        }

                        fieldValueSet.Sucess = true;

                        //検索結果をListへ追加
                        DBresultList.Add(fieldValueSet);
                        //最後の検索結果だけは別に保存
                        DBresultOne = fieldValueSet;
                    }

                    if (EventLog)
                    {
                        if (DBresultList.Count == 1)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var a in DBresultOne.Params)
                            {
                                sb.Append($"{a.Field}={a.Value}\n");
                            }
                            if (GlovalValues.ConsoleWriteLevel > 0)
                            {
                                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104,
                                    $"{AssemblyInternalName} {clientInfo} DataBaseSearch(...)呼び出されました\n" +
                                    $"SQL command.CommandText = {command.CommandText}\n結果：{DBresultList.Count}行\n" +
                                    $"{sb.ToString()}"
                                    );
                            }
                        }
                        else
                        {
                            if (GlovalValues.ConsoleWriteLevel > 0)
                            {
                                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104,
                                $"{AssemblyInternalName}  {clientInfo} DataBaseSearch(...)呼び出されました\n" +
                                $"SQL command.CommandText = {command.CommandText}\n結果：{DBresultList.Count}行\n");
                            }
                        }
                    }

                    dataReader.Close();

                    //検索結果からTiFFファイル（複数）を指定場所へ複写する。
                    if (TIFFfolder != null && TIFFfolder != "")
                    {
                        /// 検索結果からTIFFファイルをコピー
                        FileHundling.CopyTiffFilesWitthSANITIZEDPARTNUMBER(DBresultList, TIFFfolder);
                    }

                    DBresultOne.Sucess = true;
                }
                }
                else
                {
                    ServerLog.Logging.WriteLine($"※DataBaseSearch(..) SSQLSV db == null でした");
                }

            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5104, $"{AssemblyInternalName}  {clientInfo} DataBaseSearch()のエラー。{ex.Message}");
                ServerLog.Logging.WriteLine($"※DataBaseSearch(..) のエラー。 {ex.Message}");
                DBresultOne.Sucess = false;
                DBresultOne.Message = ex.Message;
            }
            DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName}  {clientInfo}【DataBaseSearch(...)】終了");
        }

        /// <summary>
        /// ■データベースを検索。検索キーと値はKSqlKeyValue構造体オブジェクトで渡す。フィールド名は複数Listで提供する
        /// </summary>
        /// <param name="TableName">テーブル名</param>
        /// <param name="searchKeyValue">検索キーと値</param>
        /// <param name="FieldNameList">結果表示するリスト</param>
        /// <param name="SelectOption"></param>
        public void DataBaseSearch3(string TableName, SqlFieldValue searchKeyValue, List<string> FieldNameList, string SelectOption = "TOP (5000)", bool EventLog = true)
        {
            DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName} 【DataBaseSearch3(...)】開始");

            DBresultList = new List<FieldValueSet>();

            string FieldNames = string.Join(",", FieldNameList);

            DBresultOne = new FieldValueSet(); // 1行も検索されなかった時のための保険

            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

            try
            {
                if (
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DATASOURCE) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBNAME) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUser) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUserPass)
                    )
                {
                    throw new Exception("設定ﾌｧｲﾙの DATASOURCE , DBNAME , DBcontrolUser , DBcontrolUserPass のどれかまたは全てが未設定です");
                }

                //DBに接続開始
                SQLSV db = new SQLSV(
                    StageServerConfig.Config.DATASOURCE,            // ホスト名
                    StageServerConfig.Config.DBNAME,            // データベース名
                    StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
                    encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)  // 接続ユーザーパスワード(暗号化を解除しています)
                    );

                if (db != null)
                {
                    using (SqlCommand command = db.connection.CreateCommand())
                    {
                        var Key = searchKeyValue.Field;
                        var FieldValue = searchKeyValue.Value;
                        var SqlDbType = searchKeyValue.SqlDBType;

                        db.connection.Open();
                        command.CommandText = $"SELECT {SelectOption} {FieldNames} FROM {TableName} WHERE  {Key}  LIKE  @SEARCH COLLATE Japanese_CS_AS_KS_WS ";
                        command.CommandText += $"ORDER BY ID DESC ";
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@SEARCH", SqlDbType)).Value = FieldValue;

                        DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName} 【DataBaseSearch3(...)】\n command.CommandText = {command.CommandText}\n");

                        SqlDataReader dataReader = command.ExecuteReader();

                        while (dataReader.Read())
                        {
                            #region 結果リスト作成
                            FieldValueSet fieldValueSet = new FieldValueSet();

                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                fieldValueSet.Params.Add(new FieldValueSet.Param
                                {
                                    Field = dataReader.GetName(i),
                                    Value = dataReader.GetValue(i).ToString(),
                                    // ここ再調査必要
                                    SqlDBType = (SqlDbType)(int)dataReader.GetSchemaTable().Rows[i]["ProviderType"]
                                });
                            }

                            fieldValueSet.Sucess = true;
                            #endregion

                            //検索結果をListへ追加
                            DBresultList.Add(fieldValueSet);
                            //最後の検索結果だけは別に保存
                            DBresultOne = fieldValueSet;
                        }

                        if (EventLog)
                        {
                            if (GlovalValues.ConsoleWriteLevel > 0)
                            {
                                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104,
                                    $"{AssemblyInternalName} DataBaseSearch3(...)呼び出されました\n" +
                                    $"SQL command.CommandText = {command.CommandText}\n結果：{DBresultList.Count}行");
                            }
                        }

                        dataReader.Close();

                        DBresultOne.Sucess = true;
                    }
                }
                else
                {
                    ServerLog.Logging.WriteLine($"※DataBaseSearch3(..) SSQLSV db == null でした");
                }

            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5104, $"{AssemblyInternalName} DataBaseSearch3()のエラー。{ex.Message}");
                ServerLog.Logging.WriteLine($"※DataBaseSearch3(..) のエラー。 {ex.Message}");

                DBresultOne.Sucess = false;
                DBresultOne.Message = ex.Message;
            }
            DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName} 【DataBaseSearch3(...)】終了");
        }

        /// <summary>
        /// ■ステージサーバデータベースを検索。検索条件をすべて文字列で指定。複数指定可能.ORDER BY対応
        /// </summary>
        /// <param name="CommentString">コメント（呼び出し元情報等自由に記入）</param>
        /// <param name="EventLog">イベントログにSQL命令記録</param>
        /// <param name="TableName">検索するテーブル名</param>
        /// <param name="searchKeyValues">検索条件struct SqlSearchStringValueのList</param>
        /// <param name="FieldNameList">結果を表示するカラムのリスト FROMに該当</param>
        /// <param name="TopOption">結果の表示個数を調整するTOPオプション</param>
        /// <param name="OderByOption">結果のソートを決定するORDER BYオプション</param>
        public void DataBaseSearch4(string TableName, List<SqlSearchStringValue> searchKeyValues, List<string> FieldNameList, string CommentString, string TopOption = "TOP (5000)", string OderByOption = "ORDER BY ID DESC", bool EventLog = true)
        {
            DebugClass.ConsoleDebugOut(6, $"■{AssemblyInternalName} {CommentString}【DataBaseSearch4(...)】開始");

            DBresultList = new List<FieldValueSet>();

            string FieldNames = string.Join(",", FieldNameList);

            DBresultOne = new FieldValueSet(); // 1行も検索されなかった時のための保険

            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

            SQLSV db = null;
            try
            {
                if (
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DATASOURCE) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBNAME) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUser) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUserPass)
                    )
                {
                    throw new Exception("設定ﾌｧｲﾙの DATASOURCE , DBNAME , DBcontrolUser , DBcontrolUserPass のどれかまたは全てが未設定です");
                }
                //DBに接続開始
                db = new SQLSV(
                    StageServerConfig.Config.DATASOURCE,            // ホスト名
                    StageServerConfig.Config.DBNAME,            // データベース名
                    StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
                    encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)   // 接続ユーザーパスワード(暗号化を解除しています)
                    );
            }
            catch (Exception ex)
            {
                ServerLog.Logging.WriteLine($"※DataBaseSearch4(..) SSQLSV db = new SQLSV() にて例外 {ex.Message}");
            }

            if (db != null)
            {
                using (SqlCommand command = db.connection.CreateCommand())
                {
                    try
                    {
                        db.connection.Open();
                        command.CommandText = $"SELECT {TopOption} {FieldNames} FROM {TableName} WHERE {SQLSearchConditions.Create(searchKeyValues)}";


                        command.CommandText += OderByOption;
                        command.CommandType = System.Data.CommandType.Text;

                        DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName} 【DataBaseSearch4(...)】呼出し元からのコメント：{CommentString}, command.CommandText = \"{command.CommandText}\"");

                        SqlDataReader dataReader = command.ExecuteReader();

                        while (dataReader.Read())
                        {
                            #region 結果リスト作成
                            FieldValueSet fieldValueSet = new FieldValueSet();

                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                fieldValueSet.Params.Add(new FieldValueSet.Param
                                {
                                    Field = dataReader.GetName(i),
                                    Value = dataReader.GetValue(i).ToString(),
                                    // ここ再調査必要
                                    SqlDBType = (SqlDbType)(int)dataReader.GetSchemaTable().Rows[i]["ProviderType"]
                                });
                            }

                            fieldValueSet.Sucess = true;
                            #endregion

                            //検索結果をListへ追加
                            DBresultList.Add(fieldValueSet);
                            //最後の検索結果だけは別に保存
                            DBresultOne = fieldValueSet;
                        }

                        if (EventLog)
                        {
                            if (GlovalValues.ConsoleWriteLevel > 0)
                            {
                                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104,
                                    $"{AssemblyInternalName} DataBaseSearch4(コメント:{CommentString}, テーブル名:{TableName}, 検索条件のリスト , 結果表示カラムのリスト, TOPオプション:{TopOption},Order Byオプション:{OderByOption})\n" +
                                    $"command.CommandText = {command.CommandText}\n結果：{DBresultList.Count}行");
                            }
                        }

                        dataReader.Close();

                        DBresultOne.Sucess = true;
                    }
                    catch (Exception ex)
                    {
                        SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5104,
                            $"{AssemblyInternalName} DataBaseSearch4(...)にて例外発生 [DBHOST:{StageServerConfig.Config.DBHOST} DBNAME:{StageServerConfig.Config.DBNAME} DBCONTROLUSER:{StageServerConfig.Config.DBcontrolUser}]・・・{ex.Message}\n\n" +
                            $"ｻｰﾊﾞｰ,呼出し文：DataBaseSearch4(コメント:{CommentString}, テーブル名:{TableName}, 検索条件のリスト , 結果表示カラムのリスト, TOPオプション:{TopOption},Order Byオプション:{OderByOption})\n\n" +
                            $"SQL構文：command.CommandText = {command.CommandText}\n結果：{DBresultList.Count}行");

                        MailNotice.SendEmailFromCommonLibrary("ToyoDATABASE", "エラー報告",
                            $"{AssemblyInternalName} DataBaseSearch4(...)にて例外発生 [DBHOST:{StageServerConfig.Config.DBHOST} DBNAME:{StageServerConfig.Config.DBNAME} DBCONTROLUSER:{StageServerConfig.Config.DBcontrolUser}]・・・{ex.Message}\n\n" +
                            $"呼出し文：DataBaseSearch4(コメント:{CommentString}, テーブル名:{TableName}, 検索条件のリスト , 結果表示カラムのリスト, TOPオプション:{TopOption},Order Byオプション:{OderByOption})\n\n" +
                            $"SQL構文：command.CommandText = {command.CommandText}\n結果：{DBresultList.Count}行", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);

                        DBresultOne.Sucess = false;
                        DBresultOne.Message = ex.Message;
                    }
                }
                DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName} {CommentString}【DataBaseSearch4(...)】終了");
            }
            else
            {
                ServerLog.Logging.WriteLine($"※DataBaseSearch4(..) SSQLSV db == null でした");
            }
        }

        /// <summary>
        /// ■ArcSuite登録前を検索
        /// ORDER BY [APPROVEDDATE] にて　APPROVEDDATE を 昇順でソート 2015/01/01,2015/01/02,2015/01/03 ....
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="FieldNameList"></param>
        /// <param name="FD1">フィールドその１</param>
        /// <param name="CO1">条件その１</param>
        /// <param name="F1V">値その１</param>
        /// <param name="LA">論理条件 ANDなど</param>
        /// <param name="FD2">フィールドその2</param>
        /// <param name="CO2">条件その2</param>
        /// <param name="F2V">フィールドその2</param>
        /// <param name="LB"></param>
        /// <param name="FD3">フィールドその3</param>
        /// <param name="CO3">条件その3</param>
        /// <param name="F3V"></param>
        /// <param name="SelectOption">TOP (500)など</param>
        /// exp                 dbSearchObj.DataBaseSearchBeforeArcSuite(DBTABLENAME, selectField, "AUTHOR", "IS", "NOT NULL", "AND", "DESIGNER", "IS", "NOT NULL", "AND", "APPROVEDUSER", "IS", "NULL");
        public void DataBaseSearchBeforeArcSuite(string TableName, List<string> FieldNameList,
            string FD1, string CO1, string F1V,
            string LA,
            string FD2, string CO2, string F2V,
            string LB,
            string FD3, string CO3, string F3V,
            string SelectOption = "TOP (5000)")
        {
            DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName} 【DataBaseSearchBeforeArcSuite(...)】開始");

            DBresultList = new List<FieldValueSet>();
            // 
            string FieldNames = string.Join(",", FieldNameList);

            DBresultOne = new FieldValueSet(); // 1行も検索されなかった時のための保険

            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

            try
            {
                if (
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DATASOURCE) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBNAME) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUser) ||
                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUserPass)
                    )
                {
                    throw new Exception("設定ﾌｧｲﾙの DATASOURCE , DBNAME , DBcontrolUser , DBcontrolUserPass のどれかまたは全てが未設定です");
                }

                //DBに接続開始
                SQLSV db = new SQLSV(
                    StageServerConfig.Config.DATASOURCE,            // ホスト名
                    StageServerConfig.Config.DBNAME,            // データベース名
                    StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
                    encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)  // 接続ユーザーパスワード(暗号化を解除しています)
                    );

                using (SqlCommand command = db.connection.CreateCommand())
                {
                    db.connection.Open();
                    command.CommandText = $"SELECT {SelectOption} {FieldNames} FROM {TableName} " +
                        $"WHERE {FD1} {CO1} {F1V} " +
                        $"{LA} {FD2} {CO2} {F2V} " +
                        $"{LB} {FD3} {CO3} {F3V} " +
                        $"ORDER BY [APPROVEDDATE]";
                    DebugClass.ConsoleDebugOut(5, $"SQLコマンドライン：{command.CommandText}");
                    command.CommandType = System.Data.CommandType.Text;
                    SqlDataReader dataReader = command.ExecuteReader();


                    while (dataReader.Read())
                    {
                        FieldValueSet fieldValueSet = new FieldValueSet();

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            fieldValueSet.Params.Add(new FieldValueSet.Param
                            {
                                Field = dataReader.GetName(i),
                                Value = dataReader.GetValue(i).ToString(),
                                SqlDBType = (SqlDbType)(int)dataReader.GetSchemaTable().Rows[i]["ProviderType"]
                            });
                        }
                        fieldValueSet.Sucess = true;

                        DBresultList.Add(fieldValueSet);
                        DBresultOne = fieldValueSet;
                    }

                    dataReader.Close();
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104, $"{AssemblyInternalName} DataBaseSearchBeforeArcSuite()のエラー。{ex.Message}");
            }
            DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName} 【DataBaseSearchBeforeArcSuite(...)】終了");
        }

        /// <summary>
        /// GUIDBASE64からTIffファイルパスをデータベースを検索して得る
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <returns></returns>
        public static string GetTiffDrawFilePathFromDB(string GUIDBASE64)
        {
            DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName} 【GetTiffDrawFilePathFromDB(...)】開始");

            string FILESTORE = "";
            string TICKETCODE;
            string clientInfo = $"DataBaseSearch.GetTiffDrawFilePathFromDB({GUIDBASE64})から呼び出し";

            if (GUIDBASE64 != null)
            {
                string SearchKey = @"GUIDBASE64";
                string SearchValue = GUIDBASE64;
                List<string> Keys = new List<string>() { "TICKETCODE" };


                DataBaseSearch dbSearch = new DataBaseSearch("FILESTORE", SearchKey, SearchValue, Keys, clientInfo, EventLog: StageServerConfig.Config.RecordOfNormaEvents_DataBaseSearch);

                if (dbSearch.DBresultList.Count == 1)
                {
                    TICKETCODE = dbSearch.DBresultOne.SearchKey("TICKETCODE").TrimEnd(); //後の空白は削除
                    FILESTORE = StageServerConfig.Config.FileStoreFolder + SIP.DirectorySeparatorChar + TICKETCODE + @".TIF";
                }
                else
                {
                    TICKETCODE = null;
                }

                DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName} 【GetTiffDrawFilePathFromDB(...)】終了 FILESTORE={FILESTORE}");

                return FILESTORE;
            }
            else
            {
                DebugClass.ConsoleDebugOut(6, $"■■{AssemblyInternalName} 【GetTiffDrawFilePathFromDB(...)】終了(みつからない)");

                return null;
            }
        }
    }

}
