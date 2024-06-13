//using SharedClassLibrary;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using SasaLib;
//using SasaLib.SQL;
//using ToyoMcMfg.Staging.DataBaseConfig;
//using MailNotice;
//using System.Security.AccessControl;
//using System.Runtime.InteropServices.WindowsRuntime;
//#if NETCOREAPP
//using System.Runtime.Versioning;
//#endif

//namespace ToyoStageService
//{
//    [SupportedOSPlatform("windows")]

//    /// <summary>
//    ///■ステージデータベース更新クラス
//    /// ※サブデータベースへの適応も行う
//    /// </summary>
//    public class DataBaseUpdate
//    {
//        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

//        MailAccount mailAccount = new MailAccount(
//            StageServerConfig.Config.EMAILSERVER,
//            StageServerConfig.Config.EMAILSENDPORT,
//            StageServerConfig.Config.EMAILNOTICE_SendToADDR,
//            StageServerConfig.Config.SMTPAUTHUSER,
//            StageServerConfig.Config.SMTPAUTHPASS_SasaLibEncryptionType,
//            StageServerConfig.Config.SMTPAUTHPASS
//        );

//        /// <summary>
//        /// UPDATEされた行数を返す
//        /// </summary>
//        public int ResultNumbrOfLines { get; set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        public int ResultNumbrOfLines_Sub { get; set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        private string TABLENAME { get; set; }

//        /// <summary>
//        /// ■コンストラクタ(Updateするレコードを特定するための検索キーと値、UPDATEするキーと値を指定)
//        /// ※サブデータベースへの適応も行う
//        /// </summary>
//        /// <param name="FieldName"></param>
//        /// <param name="FieldValue"></param>
//        /// <param name="FieldValueSet"></param>
//        public DataBaseUpdate(string FieldName, string FieldValue, FieldValueSet FieldValueSet, EventsSummary evtOutside = null)
//        {
//            DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName} 【DataBaseUpdate(...)】 開始");

//            EventsSummary evt;
//            if (evtOutside == null)
//                evt = new EventsSummary("ToyoDATABASE", 5104);
//            else
//                evt = evtOutside;

//            // SasaLib.Encryption クラスを使い 暗号を復号化する
//            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

//            try
//            {
//                ResultNumbrOfLines = 0;

//                if (
//                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DATASOURCE) ||
//                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBNAME) ||
//                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUser) ||
//                    string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUserPass)
//                    )
//                {
//                    throw new Exception("設定ﾌｧｲﾙの DATASOURCE , DBNAME , DBcontrolUser , DBcontrolUserPass のどれかまたは全てが未設定です");
//                }

//                SQLSV db = new SasaLib.SQL.SQLSV(
//                    StageServerConfig.Config.DATASOURCE,            // ホスト名
//                    StageServerConfig.Config.DBNAME,            // データベース名
//                    StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
//                     encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)   // 接続ユーザーパスワード(暗号化を解除しています)
//                    );

//                evt.Add($"■■【DataBaseUpdate(...)】 実行開始", OutConsole: false);
//                evt.Add($"FieldName = \"{FieldName}\" FieldValue = \"{FieldValue}\"", OutConsole: false);



//                // データベースアップデートの実行
//                ResultNumbrOfLines = SqlDBUpdateExecute(db, FieldName, FieldValue, FieldValueSet);
//                DebugClass.ConsoleDebugOut(5, $"■■【DataBaseUpdate(...)】 変更された総行数 {ResultNumbrOfLines}");

//                evt.Add($"■■【DataBaseUpdate(...)】実行完了 変更された総行数 {ResultNumbrOfLines}", OutConsole: false);

//                #region サブデータベースアップデート
//                if (StageServerConfig.Config.REPLICATIONTOSUBHOST == true && string.IsNullOrWhiteSpace(StageServerConfig.Config.SUB_DBHOST) == false)
//                {
//                    // サブホストの受入れモードが マスターかスレーブか、この変数に格納
//                    StageServerConfig.ServerMode subHostmode = CheckSubHostConfig.ServerMode(evt);

//                    if (subHostmode == StageServerConfig.ServerMode.Slave)
//                    {
//                        if (ResultNumbrOfLines > 0)
//                        {
//                            evt.Add($"サブデータベースホスト  {StageServerConfig.Config.SUB_DBHOST} へのアップデートが可能っです", OutConsole: false);
//                            evt.Add($"Key:{FieldName},Value: {FieldValue}", OutConsole: false);

//                            bool subdataUpdateresult = DataBaseUpdate_Sub(FieldName, FieldValue, FieldValueSet, evt);

//                            if (subdataUpdateresult)
//                                evt.Add($"サブデータベースへのアップデートは成功です", OutConsole: false);
//                            else
//                                evt.Add($"▲サブデータベースへのアップデートに失敗しました", OutConsole: false);

//                        }
//                    }
//                    else
//                    {
//                        evt.Add($"▲サブデータベースホストのサーバーモードは  {StageServerConfig.ServerMode.Slave} です。アップデートできません", OutConsole: false);

//                    }

//                }
//                else
//                {
//                    evt.Add($"サブデータベースホスト へのアップデートは行わない設定です。", OutConsole: false);
//                    evt.Add($"REPLICATIONTOSUBHOST : {StageServerConfig.Config.REPLICATIONTOSUBHOST} , SUB_DBHOST : \"{StageServerConfig.Config.SUB_DBHOST}\"", OutConsole: false);
//                    evt.Add($"RKey:{FieldName},Value:{FieldValue}", OutConsole: false);
//                    //evt.SendEntry(EventLogEntryType.Information, $"");
//                }

//                #endregion

//                if (evtOutside == null)
//                    evt.SendEntry(EventLogEntryType.Information, $"【DataBaseUpdate(...)】■正常終了", $"【DataBaseUpdate(...)】■正常終了");

//                DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName} 【DataBaseUpdate(...)】 終了");
//                return;
//            }
//            catch (Exception ex)
//            {

//                evt.Add($"※※【DataBaseUpdate(...)】■■【DataBaseUpdate(...)】 例外発生 {ex.Message}", OutConsole: false);

//                if (evtOutside == null)
//                    evt.SendEntry(EventLogEntryType.Error, $"【DataBaseUpdate(...)】異常終了", "【DataBaseUpdate(...)】異常終了");

//                DebugClass.ConsoleDebugOut(5, $"■■【DataBaseUpdate(...)】 例外発生 {ex.Message}");
//                return;
//            }
//        }

//        /// <summary>
//        /// ■コンストラクタ (テーブル名のみ指定)
//        /// </summary>
//        /// <param name="TABLENAME"></param>
//        public DataBaseUpdate(string TABLENAME)
//        {
//            this.TABLENAME = TABLENAME;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="FieldName"></param>
//        /// <param name="FieldValue"></param>
//        /// <param name="FieldValueSet"></param>
//        private bool DataBaseUpdate_Sub(string FieldName, string FieldValue, FieldValueSet FieldValueSet, EventsSummary evt)
//        {
//            // SasaLib.Encryption クラスを使い 暗号を復号化する
//            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

//            SQLSV dbSub = new SasaLib.SQL.SQLSV(
//                StageServerConfig.Config.SUB_DATASOURCE,            // ホスト名
//                StageServerConfig.Config.DBNAME,            // データベース名
//                StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
//                encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)  // 接続ユーザーパスワード(暗号化を解除しています)
//                );
//            // データベースアップデートの実行
//            int anserSubHostDBupdate = SqlDBUpdateExecute(dbSub, FieldName, FieldValue, FieldValueSet);
//            DebugClass.ConsoleDebugOut(5, $"■■【DataBaseUpdate(...)】 変更された総行数 {anserSubHostDBupdate}");

//            StringBuilder sb = new StringBuilder();

//            foreach (FieldValueSet.Param param in FieldValueSet.Params)
//            {
//                string key = param.Field;
//                string value = FieldValueSet.SearchKey(key);
//                sb.AppendLine($"\"{key}\" = \"{value}\"");
//            }
//            string keyValuePare = sb.ToString();

//            string FieldVauleSetString = $"{keyValuePare}";


//            if (anserSubHostDBupdate > 0)
//            {
//                //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104,
//                //    $"DataBaseUpdate_Sub(...)は{StageServerConfig.Config.SUB_DBHOST}へのアップデートを実行しました\n" +
//                //    $"Key:【{FieldName}】\n" +
//                //    $"Value:【{FieldValue}】\n" +
//                //    $"FieldValueSetは次の通り\n" +
//                //    $"{FieldVauleSetString}\n");
//                evt.Add(
//                    $"DataBaseUpdate_Sub(...)は{StageServerConfig.Config.SUB_DBHOST}へのアップデートを実行しました\n" +
//                    $"Key:【{FieldName}】\n" +
//                    $"Value:【{FieldValue}】\n" +
//                    $"FieldValueSetは次の通り\n" +
//                    $"{FieldVauleSetString}\n"
//                );

//                return true;
//            }
//            else
//            {
//                ServerLog.Logging.LogRotateWriteLine($"DataBaseUpdate_Sub(...)は{StageServerConfig.Config.SUB_DBHOST}へのアップデートに失敗しました（対象が見つからない可能性が高いです）。" +
//                    $"Key:【{FieldName}】Value:【{FieldValue}】 FieldValueSetは次の通り{FieldVauleSetString} ");


//                //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5104,
//                //    $"DataBaseUpdate_Sub(...)は{StageServerConfig.Config.SUB_DBHOST}へのアップデートに失敗しました\n" +
//                //    $"Key:【{FieldName}】\n" +
//                //    $"Value:【{FieldValue}】\n" +
//                //    $"FieldValueSetは次の通り\n" +
//                //    $"{FieldVauleSetString}\n");
//                evt.Add(
//                    $"▲DataBaseUpdate_Sub(...)は{StageServerConfig.Config.SUB_DBHOST}へのアップデートに失敗しました（対象が見つからない可能性が高いです）\n" +
//                    $"Key:【{FieldName}】\n" +
//                    $"Value:【{FieldValue}】\n" +
//                    $"FieldValueSetは次の通り\n" +
//                    $"{FieldVauleSetString}\n"
//                    );


//                //MailNotice.SendEmailFromCommonLibrary("ToyoDATABASE", "エラー報告",
//                //        $"DataBaseUpdate_Sub(...)は{StageServerConfig.Config.SUB_DBHOST}へのアップデートに失敗しました\n" +
//                //        $"Key:【{FieldName}】\n" +
//                //        $"Value:【{FieldValue}】\n" +
//                //        $"FieldValueSetは次の通り\n" +
//                //        $"{FieldVauleSetString}\n", mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);

//                return false;
//            }
//        }

//        /// <summary>
//        /// データベースアップデート、UPDATEする 複数のキーと値をList形式で与える
//        /// サブデータベースへのUPDATEも同時に行う。
//        /// </summary>
//        /// <param name="FieldName"></param>
//        /// <param name="FieldValue"></param>
//        /// <param name="sqlKeyValues"></param>
//        /// <returns></returns>
//        public int DataBaseUpdate2(string FieldName, string FieldValue, List<SqlFieldValue> sqlKeyValues)
//        {
//            DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName} 【DataBaseUpdate2(...)】 開始");

//            try
//            {

//                ResultNumbrOfLines = 0;

//                // SasaLib.Encryption クラスを使い 暗号を復号化する
//                SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

//                SQLSV db = new SasaLib.SQL.SQLSV(
//                    StageServerConfig.Config.DATASOURCE,            // ホスト名
//                    StageServerConfig.Config.DBNAME,            // データベース名
//                    StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
//                    encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)  // 接続ユーザーパスワード(暗号化を解除しています)
//                    );

//                string valueData = FieldValueToStr("DataBaseUpdate2", FieldName, FieldValue, sqlKeyValues); // 確認用 データ組立

//                // データベースアップデートの実行
//                ResultNumbrOfLines = SqlDBUpdateExecute(db, FieldName, FieldValue, sqlKeyValues);

//                #region ここからサブデータベースアップデート


//                if (StageServerConfig.Config.REPLICATIONTOSUBHOST == true)
//                {
//                    /// <summary>
//                    /// 正常時のイベントログ収集用オブジェクト
//                    /// </summary>
//                    EventsSummary evt = new EventsSummary();

//                    // サブホストの受入れモードが マスターかスレーブか、この変数に格納
//                    StageServerConfig.ServerMode subHostmode = CheckSubHostConfig.ServerMode(evt);


//                    if (subHostmode == StageServerConfig.ServerMode.Slave) //Masterなら実行しない
//                    {
//                        evt.Add($"StageServerConfig.XMLで指定された サブホスト{StageServerConfig.Config.SUB_DBHOST}のサーバーモードは{subHostmode}です。サブデータベースとしてUpdateできます", true, true);

//                        if (ResultNumbrOfLines > 0)
//                        {
//                            evt.Add($"DataBaseUpdate2(...)は 条件がそろったので {StageServerConfig.Config.SUB_DBHOST}へのアップデートを実行します", true, true);

//                            DataBaseUpdate2_Sub(FieldName, FieldValue, sqlKeyValues);
//                        }
//                    }
//                    else
//                    {
//                        evt.Add($"▲StageServerConfig.XMLで指定された サブホスト {StageServerConfig.Config.SUB_DBHOST} のサーバーモードは{subHostmode}です。アップデートを実行しません", true, true);
//                    }

//                    evt.SendEntry("ToyoDATABASE", EventLogEntryType.Information, 5104, "DataBaseUpdate2(...)  サブホスト更新"); ;
//                }

//                #endregion


//            }
//            catch (Exception ex)
//            {
//                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104, $"{AssemblyInternalName} DataBaseUpdate2(...) のエラー。\n" +
//                    $"Key:{FieldName},Value: {FieldValue}\n" +
//                    $"{ex.Message}");

//                return -1;

//            }
//            return ResultNumbrOfLines;
//        }

//        /// <summary>
//        /// サブデータベースアップデート、UPDATEする 複数のキーと値をList形式で与える
//        /// </summary>
//        /// <param name="FieldName"></param>
//        /// <param name="FieldValue"></param>
//        /// <param name="sqlKeyValues"></param>
//        /// <returns></returns>
//        public int DataBaseUpdate2_Sub(string FieldName, string FieldValue, List<SqlFieldValue> sqlKeyValues)
//        {
//            // SasaLib.Encryption クラスを使い 暗号を復号化する
//            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

//            if (
//                string.IsNullOrWhiteSpace(StageServerConfig.Config.SUB_DATASOURCE) ||
//                string.IsNullOrWhiteSpace(StageServerConfig.Config.DBNAME) ||
//                string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUser) ||
//                string.IsNullOrWhiteSpace(StageServerConfig.Config.DBcontrolUserPass)
//                )
//            {
//                throw new Exception("設定ﾌｧｲﾙの DATASOURCE , DBNAME , DBcontrolUser , DBcontrolUserPass のどれかまたは全てが未設定です");
//            }

//            SQLSV dbSub = new SasaLib.SQL.SQLSV(
//                StageServerConfig.Config.SUB_DATASOURCE,            // ホスト名
//                StageServerConfig.Config.DBNAME,            // データベース名
//                StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
//                encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)   // 接続ユーザーパスワード(暗号化を解除しています)
//                );

//            string valueData = FieldValueToStr("DataBaseUpdate2_Sub(...)", FieldName, FieldValue, sqlKeyValues); // 確認用 データ組立

//            // データベースアップデートの実行
//            ResultNumbrOfLines_Sub = SqlDBUpdateExecute(dbSub, FieldName, FieldValue, sqlKeyValues);
//            DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName} 【DataBaseUpdate2_Sub(...)】 変更された総行数 {ResultNumbrOfLines_Sub}");

//            if (ResultNumbrOfLines_Sub > 0)
//            {
//                //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104,
//                //    $"{AssemblyInternalName} DataBaseUpdate2_Sub(...)は{StageServerConfig.Config.SUB_DBHOST} へのアップデートに成功しました。\n" +
//                //    valueData);
//                return ResultNumbrOfLines_Sub;
//            }
//            else
//            {
//                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5104,
//                    $"{AssemblyInternalName} DataBaseUpdate2_Sub(...)は{StageServerConfig.Config.SUB_DBHOST}へのアップデートに失敗しました。\n" +
//                    valueData);
//                return ResultNumbrOfLines_Sub;
//            }
//        }

//        /// <summary>
//        /// DB Update実行
//        /// </summary>
//        /// <param name="db"></param>
//        /// <param name="FieldName"></param>
//        /// <param name="FieldValue"></param>
//        /// <param name="FieldValueSet"></param>
//        /// <returns></returns>
//        private int SqlDBUpdateExecute(SQLSV db, string FieldName, string FieldValue, FieldValueSet FieldValueSet)
//        {
//            int ans = 0;

//            int allCount = FieldValueSet.Params.Count();

//            try
//            {

//                using (SqlCommand command = db.connection.CreateCommand())
//                {
//                    StringBuilder sb = new StringBuilder();
//                    db.connection.Open();
//                    sb.Append("UPDATE FILESTORE SET ");
//                    for (int count = 0; allCount > count; count++)
//                    {
//                        sb.Append(String.Format("{0}=@{0}", FieldValueSet.Params[count].Field));
//                        sb.Append(",");
//                        command.Parameters.Add(new SqlParameter("@" + FieldValueSet.Params[count].Field, FieldValueSet.Params[count].SqlDBType)).Value = FieldValueSet.Params[count].Value;
//                    }
//                    command.CommandText = sb.ToString().TrimEnd(',') + " WHERE " + FieldName + " LIKE @SEARCH";
//                    command.Parameters.Add(new SqlParameter("@SEARCH", SqlDbType.NVarChar, FieldValue.Count())).Value = FieldValue;
//                    command.CommandType = System.Data.CommandType.Text;

//                    DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName} 【DataBaseUpdate(...)】\n command.CommandText = {command.CommandText}");

//                    ans += command.ExecuteNonQuery();
//                    db.connection.Close();
//                }
//                DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName} 【DataBaseUpdate(...)】 変更された総行数 {ans}");

//                return ans;

//            }
//            catch (Exception ex)
//            {
//                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104, $"DataBaseUpdate2(...) のエラー。{ex.Message}");

//                return -1;
//            }
//        }

//        /// <summary>
//        /// DB Update実行
//        /// </summary>
//        /// <param name="db"></param>
//        /// <param name="FieldName"></param>
//        /// <param name="FieldValue"></param>
//        /// <param name="sqlKeyValues"></param>
//        /// <returns></returns>
//        private int SqlDBUpdateExecute(SQLSV db, string FieldName, string FieldValue, List<SqlFieldValue> sqlKeyValues)
//        {
//            int ans = 0;

//            string valueData = FieldValueToStr("SqlDBUpdateExecute(...)", FieldName, FieldValue, sqlKeyValues); // 確認用 データ組立

//            try
//            {
//                int allCount = sqlKeyValues.Count();
//                using (SqlCommand command = db.connection.CreateCommand())
//                {
//                    StringBuilder sb = new StringBuilder();
//                    db.connection.Open();
//                    sb.Append($"UPDATE {TABLENAME} SET ");

//                    for (int count = 0; allCount > count; count++)
//                    {
//                        sb.Append(String.Format("{0}=@{0}", sqlKeyValues[count].Field));
//                        sb.Append(",");
//                        command.Parameters.Add(new SqlParameter("@" + sqlKeyValues[count].Field, sqlKeyValues[count].SqlDBType)).Value = sqlKeyValues[count].Value;
//                    }
//                    command.CommandText = sb.ToString().TrimEnd(',') + " WHERE " + FieldName + " LIKE @SEARCH";
//                    command.Parameters.Add(new SqlParameter("@SEARCH", SqlDbType.NVarChar, FieldValue.Count())).Value = FieldValue;
//                    command.CommandType = System.Data.CommandType.Text;

//                    DebugClass.ConsoleDebugOut(5, $"■■【SqlDBUpdateExecute(...)】\n command.CommandText = {command.CommandText}");

//                    ans += command.ExecuteNonQuery();
//                    db.connection.Close();
//                }
//                DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName} 【SqlDBUpdateExecute(...)】  変更された総行数 {ans}");
//                DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName} 【SqlDBUpdateExecute(...)】 終了");
//                return ans;
//            }
//            catch (Exception ex)
//            {
//                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5104, $"{AssemblyInternalName} SqlDBUpdateExecute(...) のエラー。\n{valueData}{ex.Message}");
//                return -1;
//            }
//        }

//        /// <summary>
//        /// 確認用文字列生成
//        /// </summary>
//        /// <param name="FieldName"></param>
//        /// <param name="FieldValue"></param>
//        /// <param name="sqlKeyValues"></param>
//        /// <returns></returns>

//        private string FieldValueToStr(string TitleName, string FieldName, string FieldValue, List<SqlFieldValue> sqlKeyValues)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.AppendLine($"{TitleName} FieldName = \"{FieldName}\" ,  FieldValue = \"{FieldValue}\"");
//            foreach (var a in sqlKeyValues)
//            {
//                sb.AppendLine($"{TitleName} Field:\"{a.Field}\" = Value:\"{a.Value.ToString()}\" ,  Type = {a.SqlDBType}");
//            }
//            sb.AppendLine($"{TitleName}  ----------------------------");
//            string valueData = sb.ToString();

//            return valueData;
//        }
//    }
//}
