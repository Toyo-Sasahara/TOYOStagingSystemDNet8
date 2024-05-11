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
/*
BACKUP DATABASE DATABASE1 TO DISK='D:\SQLDBBACKUP.BAK' WITH INIT
GO
 */

namespace ToyoStageService
{
    /// <summary>
    /// STAGESERVERデータベースをバックアップ
    /// </summary>
    public class DataBaseBackup
    {
        public string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        public static bool SQLServerIsBackupRunning { get; private set; } = false;

        string DBUser;
        string DBPass;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataBaseBackup()
        {
            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

            // 接続ユーザー
            this.DBUser = StageServerConfig.Config.DBcontrolUser;

            // デコードされた正式パスワード)
            this.DBPass = encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass); //接続ユーザーパスワード(暗号化を解除しています)
        }

        /// <summary>
        /// SQLサーバーデータベースバックアップ
        /// </summary>
        /// <param name="BackupDistnationFileFullPath"></param>
        /// <param name="BackupName"></param>
        /// <returns></returns>
        public bool Execute(string BackupDistnationFileFullPath, string BackupName)
        {
            if (SQLServerIsBackupRunning == true)
                return false;
            else
                SQLServerIsBackupRunning = true;

            string commmandText = "BACKUP DATABASE @DBName TO DISK=@FilePath WITH NOFORMAT, INIT, NAME = @BackUpName";

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
                        objCommand.Parameters.AddWithValue("@dbName", StageServerConfig.Config.DBNAME);
                        objCommand.Parameters.AddWithValue("@FilePath", BackupDistnationFileFullPath);
                        objCommand.Parameters.AddWithValue("@BackUpName", BackupName);

                        DebugClass.ConsoleDebugOut(1, $"■■{AssemblyInternalName}【Execute({BackupDistnationFileFullPath},{BackupName})】\n SQLコマンドライン：{objCommand.CommandText}開始");

                        SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASEbackup", EventLogEntryType.Information, 5100, $"{AssemblyInternalName}【Execute({BackupDistnationFileFullPath},{BackupName})】\n SQLコマンドライン：{objCommand.CommandText}開始");

                        IAsyncResult result = objCommand.BeginExecuteNonQuery();

                        result.AsyncWaitHandle.WaitOne();

                        int col = objCommand.EndExecuteNonQuery(result);

                        DebugClass.ConsoleDebugOut(1, $"■■{AssemblyInternalName}【Execute({BackupDistnationFileFullPath},{BackupName})】\n SQLコマンドライン：{objCommand.CommandText}　完了");

                        SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASEbackup", EventLogEntryType.Information, 5100, $"{AssemblyInternalName}【Execute({BackupDistnationFileFullPath},{BackupName})】\n SQLコマンドライン：{objCommand.CommandText}　完了 変更された行 {col}");

                        SQLServerIsBackupRunning = false; // バックアップ終了

                        return true;
                    }
                    catch (SqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        db.connection.Close();
                        SQLServerIsBackupRunning = false; // バックアップ終了
                    }
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASEbackup", EventLogEntryType.Error, 5100, $"{AssemblyInternalName}【Execute({BackupDistnationFileFullPath}\n例外発生{ex.Message}");
            }
            return false;
        }

        private void Kill_User(System.Data.SqlClient.SqlConnection _con)
        {
            // 全てのDB接続セッション取得SQL作成
            String sql =
                @"SELECT spid FROM master..sysprocesses,master..sysdatabases " +
                @"WHERE master..sysprocesses.dbid = master..sysdatabases.dbid AND name = 'TEST';";
            System.Data.SqlClient.SqlCommand sqlCommand =
                new System.Data.SqlClient.SqlCommand(sql, _con);
            System.Data.SqlClient.SqlDataAdapter adapter =
                new System.Data.SqlClient.SqlDataAdapter(sqlCommand);

            // 全てのDB接続セッション取得
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            // 全てのDB接続セッション取得SQLの解放
            adapter.Dispose();
            sqlCommand.Dispose();

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                // 個々のDB接続セッションを強制切断SQL作成
                String sql2 = "KILL " + dt.Rows[i][0];
                System.Data.SqlClient.SqlCommand sqlCommand2 =
                    new SqlCommand(sql2, _con);
                System.Data.SqlClient.SqlDataAdapter adapter2 =
                    new System.Data.SqlClient.SqlDataAdapter(sqlCommand2);

                // 個々のDB接続セッションを強制切断
                DataTable dt2 = new DataTable();
                adapter2.Fill(dt2);

                // 個々のDB接続セッションを強制切断SQLの解放
                adapter2.Dispose();
                sqlCommand2.Dispose();
            }
        }

        /// <summary>
        /// ZIP圧縮
        /// </summary>
        /// <param name="sourceFileFolder"></param>
        /// <param name="distZipFileFullPath"></param>
        /// <param name="BufferSize"></param>
        /// <returns></returns>
        public bool CleateZipArchive(string sourceFileFolder, string distZipFileFullPath)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(
                sourceFileFolder,
                distZipFileFullPath,
                System.IO.Compression.CompressionLevel.Optimal,
                true,
                System.Text.Encoding.GetEncoding("shift_jis"));

            while (!File.Exists(distZipFileFullPath))
            {
                System.Threading.Thread.Sleep(100);
            }

            if (File.Exists(distZipFileFullPath))
            {
                return true;
            }
            return false;
        }
    }
}
