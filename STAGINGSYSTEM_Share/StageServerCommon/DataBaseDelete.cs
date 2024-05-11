using SasaLib;
using SasaLib.SQL;
using SharedClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyoMcMfg.Staging.DataBaseConfig;

namespace ToyoStageService
{
    /// <summary>
    /// ■ステージデータベース内容と実体を消去
    /// ※サブデータベースへの適応も行う
    /// </summary>
    public class DataBaseDelete
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        SasaLib.EventsSummary evt;

        /// <summary>
        /// サブデータベースへの削除結果
        /// </summary>
        public bool Result { set; get; }

        /// <summary>
        /// サブデータベースへの削除結果
        /// </summary>
        public bool Result_Sub { set; get; }

        /// <summary>
        /// 
        /// </summary>
        private FieldValueSet DBresultOne { set; get; }

        /// <summary>
        /// 
        /// </summary>
        private List<FieldValueSet> DBresultList { get; set; }

        /// <summary>
        /// コンストラクタ。データベースと実体ファイルを削除
        /// ※サブデータベースへの適応も行う
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="FieldValue"></param>
        public DataBaseDelete(string FieldName, string FieldValue)
        {
            DebugClass.ConsoleDebugOut(0, $"■■{AssemblyInternalName} 【DataBaseDelete({FieldName},{FieldValue})】開始");

            evt = new EventsSummary("ToyoDATABASE", 5101);

            DBresultOne = new FieldValueSet(); // 1行も検索されなかった時のための保険

            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);


            //DBに接続開始
            SQLSV db = new SasaLib.SQL.SQLSV(
                StageServerConfig.Config.DATASOURCE,            // ホスト名
                StageServerConfig.Config.DBNAME,            // データベース名
                StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
                encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)   // 接続ユーザーパスワード(暗号化を解除しています)
            );

            // DBのテーブルを検索し削除
            DBDelete(db, FieldName, FieldValue);

            // 実体を削除
            Result = DeleteGUIDBASE84ListProcess(StageServerConfig.Config.FileStoreFolder);


            #region サブデータベース 削除プロセス
            if (StageServerConfig.Config.REPLICATIONTOSUBHOST == true)
            {
                // サブホストの受入れモードが マスターかスレーブか、この変数に格納
                StageServerConfig.ServerMode subHostmode = CheckSubHostConfig.ServerMode(evt);

                if (Result)
                {
                    if (subHostmode == StageServerConfig.ServerMode.Slave)
                    {
                        //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5101,
                        //    $"DataBaseDelete(...)は 条件がそろったので {StageServerConfig.Config.SUB_DBHOST}への指定データ削除を実行します。\n" +
                        //    $"Key:{FieldName},Value:{FieldValue}");
                        evt.Add(
                                $"DataBaseDelete(...)は 条件がそろったので {StageServerConfig.Config.SUB_DBHOST}への指定データ削除を実行します。\n" +
                                $"Key:{FieldName},Value:{FieldValue}"
                            );

                        Result_Sub = DataBaseDelete_Sub(FieldName, FieldValue);

                        evt.Add(                               
                                $"DataBaseDelete(...) Key:{FieldName},Value:{FieldValue} 結果:{Result_Sub}"
                            );

                        evt.SendEntry(EventLogEntryType.Information, "■DataBaseDelete(...)");
                    }
                    else
                    {
                        //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5101,
                        //    $"DataBaseDelete(...)は SubHostのモードが Master のため データ削除は実行しません\n" +
                        //    $"Key:{FieldName},Value:{FieldValue}");
                        evt.Add(
                            $"DataBaseDelete(...)は SubHostのモードが Master のため データ削除は実行しません\n" +
                            $"Key:{FieldName},Value:{FieldValue}"
                            );

                        evt.SendEntry(EventLogEntryType.Warning, "▲DataBaseDelete(...)");
                    }
                }

            }
            #endregion

            DebugClass.ConsoleDebugOut(0, $"■■{AssemblyInternalName} 【DataBaseDelete({FieldName},{FieldValue})】終了。結果 {Result}");
        }

        /// <summary>
        /// サブデータベースでの削除
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="FieldValue"></param>
        /// <returns></returns>
        public bool DataBaseDelete_Sub(string FieldName, string FieldValue)
        {
            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

            //サブホストDBに接続開始
            SQLSV dBSub = new SasaLib.SQL.SQLSV(
                StageServerConfig.Config.SUB_DATASOURCE,            // ホスト名
                StageServerConfig.Config.DBNAME,            // データベース名
                StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
                encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)   // 接続ユーザーパスワード(暗号化を解除しています)
            );

            // SUBDBのテーブルを検索し削除
            DBDelete(dBSub, FieldName, FieldValue);

            // 実体の削除
            bool Result_Sub = DeleteGUIDBASE84ListProcess(StageServerConfig.Config.SUB_FileStoreForderUNC);
            if (Result_Sub)
            {
                //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5101,
                //    $"DataBaseDelete_Sub() サブホストのデータベースからレコードを削除しました。\n" +
                //    $" Key:{FieldName},Value:{FieldValue}");
                evt.Add(
                    $"DataBaseDelete_Sub() サブホストのデータベースからレコードを削除しました。\n" +
                    $" Key:{FieldName},Value:{FieldValue}"
                    );

                return true;
            }
            else
            {
                //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5101,
                //    $"DataBaseDelete_Sub() サブホストのデータベースからレコードの削除に失敗しています\n" +
                //    $" Key:{FieldName},Value:{FieldValue}");
                evt.Add(
                    $"DataBaseDelete_Sub() サブホストのデータベースからレコードの削除に失敗しています\n" +
                    $" Key:{FieldName},Value:{FieldValue}"
                );

                return false;
            }
        }

        /// <summary>
        /// 削除対象を検索(DBコネクションの指定が必要)
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="FieldValue"></param>
        /// <returns></returns>
        private bool DBDelete(SQLSV db, string FieldName, string FieldValue)
        {
            DBresultList = new List<FieldValueSet>();

            try
            {
                using (SqlCommand command = db.connection.CreateCommand())
                {
                    db.connection.Open();
                    command.CommandText = "SELECT * FROM FILESTORE" + " WHERE " + FieldName + " = @SEARCH;";
                    command.CommandText = command.CommandText + "DELETE FROM FILESTORE WHERE " + FieldName + " LIKE @SEARCH;";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@SEARCH", SqlDbType.NVarChar, FieldValue.Count())).Value = FieldValue.Trim(); //トリム
                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        FieldValueSet fieldValueSet = new FieldValueSet();

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            fieldValueSet.Params.Add(new FieldValueSet.Param { Field = dataReader.GetName(i), Value = dataReader.GetValue(i).ToString(), SqlDBType = (SqlDbType)(int)dataReader.GetSchemaTable().Rows[i]["ProviderType"] });

                            DebugClass.ConsoleDebugOut(5, $"■■【DatabaseDelete.Search(...)】\n dataReader[{i}] = {dataReader[i]}");
                        }

                        fieldValueSet.Sucess = true;


                        DBresultList.Add(fieldValueSet);
                        DBresultOne = fieldValueSet;
                    }
                    dataReader.Close();


                    var anserOneLine = new List<string>();
                    DBresultList.ForEach(x => anserOneLine.Add(
                        x.SearchKey("TICKETCODE") + "," + x.SearchKey("PARTNUMBER") + "," + x.SearchKey("APPROVEDUSER") + "," + x.SearchKey("APPROVEDDATE")
                        ));
                    string msg = string.Join("\n", anserOneLine);

                    //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5101, $"DBDelete(...)削除に成功 {db.connection.DataSource}\n{msg} ");
                    evt.Add(
                         $"DBDelete(...)削除に成功 {db.connection.DataSource}\n{msg} "
                        );
                }
                return true;
            }
            catch (Exception ex)
            {
                //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5101, $"DataBaseDelete(...) のエラー。 {db.connection.DataSource}\n{ex.Message}");
                evt.Add(
                    $"DataBaseDelete(...) のエラー。 {db.connection.DataSource}\n{ex.Message} {ex.InnerException}"
                );

                return false;
            }
        }

        /// <summary>
        /// データベースから検索が成功した削除対象ファイルを1件づつ削除
        /// </summary>
        /// <returns></returns>
        private bool DeleteGUIDBASE84ListProcess(string FileStoreFolder)
        {
            bool ans = false;
            foreach (var dbResult in DBresultList)
            {
                if (dbResult.Sucess)
                {
                    string GUIDBASE64 = dbResult.SearchKey("GUIDBASE64");
                    DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName}【DeleteGUIDBASE84ListProcess({FileStoreFolder})】開始。");

                    ans = FileHundling.DeleteFile(GUIDBASE64, FileStoreFolder);

                    DebugClass.ConsoleDebugOut(5, $"■■{AssemblyInternalName}【FileHundling({FileStoreFolder})】の削除結果 {ans}");
                }
            }
            return ans;
        }

    }
}
