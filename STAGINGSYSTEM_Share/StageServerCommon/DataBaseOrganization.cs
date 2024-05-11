using SasaLib;
using SasaLib.SQL;
using SharedClassLibrary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
まだバグがあるので呼び出さない
 */

namespace ToyoStageService
{
    public class DataBaseOrganization
    {
        public string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        public static bool SQLServerIsDataBaseOrganizationRunning { get; private set; } = false;

        string DBUser;
        string DBPass;

        /// <summary>
        /// コンストラクタ
        /// </summary>

        public DataBaseOrganization()
        {
            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

            // 接続ユーザー
            this.DBUser = StageServerConfig.Config.DBcontrolUser;

            // デコードされた正式パスワード)
            this.DBPass = encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass); //接続ユーザーパスワード(暗号化を解除しています)
        }

        public bool ExecuteMarkingArcSuiteRegisteredUnnecessaryData()
        {
            if (SQLServerIsDataBaseOrganizationRunning == true)
                return false;
            else
                SQLServerIsDataBaseOrganizationRunning = true;

            string commmandText =
            "UPDATE FILESTORE SET ARCSUITE_REGISTERED_UNNECESSARY = 1 " +
            "WHERE PARTNUMBER IN(SELECT DISTINCT PARTNUMBER  FROM FILESTORE WHERE ARCSUITEID IS NOT NULL)  AND ARCSUITEID IS NULL AND REGISTWAITINGFLAG<> 1 AND PRIORITYREGISTFLAG<> 1";

            try
            {
                //DBに接続開始
                SQLSV db = new SQLSV(
                    StageServerConfig.Config.DATASOURCE,            // ホスト名
                    StageServerConfig.Config.DBNAME,            // データベース名
                    DBUser,     // 接続ユーザー名
                    DBPass  // 接続ユーザーパスワード
                    );
                using (SqlCommand objCommand = db.connection.CreateCommand())
                {
                    try
                    {
                        db.connection.Open();
                        objCommand.CommandText = commmandText;

                        DebugClass.ConsoleDebugOut(1, $"■■{AssemblyInternalName} DataBaseOrganization.ExecuteMarkingArcSuiteRegisteredUnnecessaryData() \n SQLコマンドライン：{objCommand.CommandText}開始");

                        SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASEorganization", EventLogEntryType.Information, 5100,
                            $"{{AssemblyInternalName}} DataBaseOrganization.ExecuteMarkingArcSuiteRegisteredUnnecessaryData() \\n SQLコマンドライン：{{objCommand.CommandText}}開始\"");

                        IAsyncResult result = objCommand.BeginExecuteNonQuery();

                        result.AsyncWaitHandle.WaitOne();

                        int col = objCommand.EndExecuteNonQuery(result);

                        DebugClass.ConsoleDebugOut(1, $"■■{AssemblyInternalName} DataBaseOrganization.ExecuteMarkingArcSuiteRegisteredUnnecessaryData()\n SQLコマンドライン：{objCommand.CommandText}　完了");

                        SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASEorganization", EventLogEntryType.Information, 5100, $"{AssemblyInternalName} DataBaseOrganization.ExecuteMarkingArcSuiteRegisteredUnnecessaryData()\n SQLコマンドライン：{objCommand.CommandText}　完了 変更された行 {col}\n" +
                            $"同じPARTSNUMBERのうちアークスイートオブジェクト番号を持たないレコードを 不要対象としてマークしました");

                        SQLServerIsDataBaseOrganizationRunning = false; // 

                        return true;
                    }
                    catch (SqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        db.connection.Close();
                        SQLServerIsDataBaseOrganizationRunning = false; // バックアップ終了
                    }
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASEorganization", EventLogEntryType.Error, 5100, $"{AssemblyInternalName} DataBaseOrganization.ExecuteMarkingArcSuiteRegisteredUnnecessaryData()\n例外発生{ex.Message}");
            }
            return false;

        }

    }
}
