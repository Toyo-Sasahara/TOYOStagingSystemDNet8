using MailNotice;
using SasaLib;
using SasaLib.SQL;
using SharedClassLibrary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToyoStageService
{
    /// <summary>
    /// ■ステージデータベース追加クラス
    /// ※サブデータベースへの適応も行う
    /// </summary>
    public class DataBaseInsert
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

        EventsSummary evt;

        /// <summary>
        /// 戻り値
        /// クラスのインスタンスに対して遅延バインディングを有効にするには、 ExpandoObject キーワードを使用する
        /// </summary>
        public dynamic dynamicAns_Main = new ExpandoObject();
        public dynamic dynamicAns_Variant = new ExpandoObject();
        public dynamic dynamicAns_Sub = new ExpandoObject();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataBaseInsert(EventsSummary evt)
        {
            this.evt = evt;
        }

        /// <summary>
        /// ■■データベースへチケットファイルの情報を追加. サニタイズ処理もここで行います。（サニタイズ後はチケットを書き戻します）
        /// ※サブデータベースへの適応も行う
        /// </summary>
        /// <param name="CommitFolderTicketFullFilename"></param>
        public void DataBaseInsertStart_Main(string CommitFolderTicketFullFilename)
        {
            evt.Add($"DataBase 処理開始 対象： {CommitFolderTicketFullFilename}", true, true);

            // チケット情報のPARTNUMBERがNullOrWhiteSpacの場合、エラー扱いとする
            if (String.IsNullOrWhiteSpace((string)GetTicketValue("PARTNUMBER")))
            {
                evt.Add($"※チケットファイル{CommitFolderTicketFullFilename}にはPARTNUMBERが無い", true, true);

                dynamicAns_Main.ANS = false;
                dynamicAns_Main.MSG = $"DataBaseInsertStart_Main(..) チケットファイル{CommitFolderTicketFullFilename}にはPARTNUMBERが無い";
                // メソッドを抜ける
                return;
            }

            //■■サニタイズ。CADが吐き出したチケットファイルから抜けている情報を補完する
            SanitizingTicket(TicketProcess.TicketData);
            evt.Add($"CADが吐き出したチケットファイルから抜けている情報を補完しました (SanitizingTicket(TicketProcess.TicketData))", true, true);

            // チケットファイルに足りないDESCRIPTIONとPARTSNAME項目をPARTSTYPEに合わせてここで準備
            AddPARTSNAMEorDESCRIPTION(TicketProcess.TicketData);
            evt.Add($"チケットファイルに足りないDESCRIPTIONとPARTSNAME項目をPARTSTYPEに合わせて準備しました (AddPARTSNAMEorDESCRIPTION(TicketProcess.TicketData))", true, true);

            //// サニタイズ後のデータでチケットファイルを書き戻す
            if (TicketProcess.SerializeTicketFile(CommitFolderTicketFullFilename) == false)
            {
                evt.Add($"※チケットファイル{CommitFolderTicketFullFilename}サニタイズ後のデシリアライズに失敗", true, true);

                dynamicAns_Main.ANS = false;
                dynamicAns_Main.MSG = $"DataBaseInsertStart_Main(..) チケットファイル{CommitFolderTicketFullFilename}サニタイズ後のデシリアライズに失敗";
                // メソッドを抜ける
                return;
            }

            evt.Add($"サニタイズ後のデータでチケットファイルを書き戻しました {CommitFolderTicketFullFilename}", true, true);

            #region データベース接続とInsert

            // メインデータベースInsert
            SQLSV db;
            try
            {
            
                // SasaLib.Encryption クラスを使い 暗号を復号化する
                SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

                //DB接続情報
                db = new SasaLib.SQL.SQLSV(
                    StageServerConfig.Config.DATASOURCE,            // ホスト名
                    StageServerConfig.Config.DBNAME,            // データベース名
                    StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
                    encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)  // 接続ユーザーパスワード(暗号化を解除しています)
                    );
                //DBに接続開始
                dynamicAns_Main = SqlDBInsertExecute(db);

                /// Variantデータベースを更新
                VariantExecute(db);
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5102, $"{AssemblyInternalName} DataBaseInsertStart_Main(..) 例外発生：{ex.Message}, {ex.InnerException}");

                // 別アセンブリからdynamicを取得するため、ExpandoObjectを使用する。
                dynamicAns_Main.ANS = false;
                dynamicAns_Main.MSG = @"DataBaseInsert() 例外メッセージex.Message= " + ex.Message + "\n";

                return;
            }

            #endregion
        }

        /// <summary>
        /// 表形式図面データベース処理。チケットコードに
        /// </summary>
        /// <param name="db"></param>
        private void VariantExecute(SQLSV db)
        {

            /// 追加
            try
            {
                string ConnectionString = db.connection.ConnectionString;

                if (TicketProcess.TicketData.Variant.Count > 0)
                {

                    evt.Add($"VariantExecute(..) TicketProcess.TicketData.Variant.Count = {TicketProcess.TicketData.Variant.Count} です。処理開始");

                    foreach (CommonTicket.Param param in TicketProcess.TicketData.Variant)
                    {
                        if (param.Key == "PARTNUMBER")
                        {
                            string value = param.Value;
                            // Variantデータベースへ表の行数分を書き込み
                            dynamicAns_Variant = VariantSqlDBInsertExecute(db, TicketProcess.TicketData.TICKETCODE, value);
                            if (dynamicAns_Variant.ANS == true)
                            {
                                evt.Add($"Variantデータベースに追加成功  {value}", true, true);
                            }
                            else
                            {
                                evt.Add($"Variantデータベースに追加失敗  {value}", true, true);
                            }
                        }
                    }
                }
                else
                {
                    evt.Add($"VariantExecute(..) TicketProcess.TicketData.Variant.Count = 0 です。処理をスキップします");

                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5102, $"Variantデータベースの挿入に失敗 dynamicAns_Variant {AssemblyInternalName} DataBaseInsert(...) 例外発生：{ex.Message}");

                // 別アセンブリからdynamicを取得するため、ExpandoObjectを使用する。
                dynamicAns_Variant.ANS = false;
                dynamicAns_Variant.MSG = @"DataBaseInsert() dynamicAns_Variant 例外メッセージex.Message= " + ex.Message + "\n";
                return;
            }
            /// 追加
        }

        /// <summary>
        /// サブデータベースに指定したチケットファイルの情報を挿入する
        /// </summary>
        /// <param name="CommitFolderTicketFullFilename"></param>
        /// <returns></returns>
        public void DataBaseInsertStart_Sub(string CommitFolderTicketFullFilename)
        {
            SQLSV db;
            try
            {
                // StageServerConfig.Config.SUB_DBHOST に文字列がある場合に実行する
                if (String.IsNullOrWhiteSpace(StageServerConfig.Config.SUB_DBHOST) == false)
                {
                    // SasaLib.Encryption クラスを使い 暗号を復号化する
                    SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.DBcontrolUserPass_SasaLibEncryptionType);

                    //DB接続情報
                    db = new SasaLib.SQL.SQLSV(
                        StageServerConfig.Config.SUB_DATASOURCE,            // サブホスト名
                        StageServerConfig.Config.DBNAME,            // データベース名
                        StageServerConfig.Config.DBcontrolUser,     // 接続ユーザー名
                        encryption.Decoding(StageServerConfig.Config.DBcontrolUserPass)   // 接続ユーザーパスワード(暗号化を解除しています)
                        );

                    //DBに接続開始と実行
                    dynamicAns_Sub = SqlDBInsertExecute(db);

                    /// 追加
                    VariantExecute(db);

                    if (dynamicAns_Sub.ANS == false)
                    {
                        if (GlovalValues.ConsoleWriteLevel > 0)
                        {
                            SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Warning, 5102, $"サブホストへのSqlDBInsertExecute(..) 戻り値dynamicAnsSub.ANS==false\r\n" +
                                $"ホスト{StageServerConfig.Config.SUB_DBHOST},データベース{StageServerConfig.Config.DBNAME}\r\n" +
                                $"対象チケット{CommitFolderTicketFullFilename}");
                        }
                    }
                    return;
                }
                else
                {
                    dynamicAns_Sub.ANS = false;
                    dynamicAns_Sub.MSG = @"DataBaseInsert()  サブホストは指定が無かったため書き込みはおこないません";

                    SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5102, $"サブホストは指定が無かったため書き込みはおこないません");
                    return;
                }

            }
            catch (Exception ex)
            {

                dynamicAns_Sub.ANS = false;
                dynamicAns_Sub.MSG = @"DataBaseInsert()  サブホストへのinsert例外メッセージex.Message= " + ex.Message + "\n";

                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Warning, 5102, $"{AssemblyInternalName} DataBaseInsert(...) スレーブデータベースへのコミット情報登録に失敗しました.\r\n" +
                    $"ホスト{StageServerConfig.Config.SUB_DBHOST},データベース{StageServerConfig.Config.DBNAME}\r\n" +
                    $"対象チケット{CommitFolderTicketFullFilename}");

                MailNotice.SendEmailFromCommonLibrary("ToyoDATABASE", "エラー報告",
                    $"{AssemblyInternalName} DataBaseInsert(...) スレーブデータベースへのコミット情報登録に失敗しました。\r\n" +
                    $"ホスト{StageServerConfig.Config.SUB_DBHOST},データベース{StageServerConfig.Config.DBNAME}\r\n" +
                    $"対象チケット{CommitFolderTicketFullFilename}\r\n" +
                    $"dynamicAnsSub.MSG={dynamicAns_Sub.MSG}", mailAccount,StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);

                return;
            }
        }

        /// <summary>
        /// SQL文の組立と実行。コネクションは別
        /// </summary>
        /// <param name="sqlParams"></param>
        /// <returns>bool dynamicAns.ANS とstring dynamicAns.MSG</returns>
        private dynamic SqlDBInsertExecute(SQLSV db, bool eventLog = false)
        {
            dynamic ans = new ExpandoObject();
            string SqlConnectonServerVersion = null;
            string SqlConnectonDataSource = null;
            string SqlConnectonDatabase = null;
            try
            {
                using (SqlCommand command = db.connection.CreateCommand())
                {
                    db.connection.Open();
                    SqlConnectonServerVersion = db.connection.ServerVersion;
                    SqlConnectonDataSource = db.connection.DataSource;
                    SqlConnectonDatabase = db.connection.Database;

                    List<SqlParameter> sqlParams = new List<SqlParameter>();

                    dynamic packed = MakeSqlCommandLines(
                        "GUID_RAW,GUIDBASE64,TICKETCODE,TIMESTAMP,COMMITHOST,COMMITUSER,REQUESTPRINTER,PRINTINGTIME,CREATESOFTWARE,DOCUMENTNAME,PLOTPAPERSIZE,PAPERSIZE",
                        "@GUID_RAW,@GUIDBASE64,@TICKETCODE,@TIMESTAMP,@COMMITHOST,@COMMITUSER,@REQUESTPRINTER,@PRINTINGTIME,@CREATESOFTWARE,@DOCUMENTNAME,@PLOTPAPERSIZE,@PAPERSIZE",
                        TicketProcess.TicketData);
                    sqlParams.AddSqlParameter(new SqlParameter("@GUID_RAW", System.Data.SqlDbType.UniqueIdentifier)).Value = TicketProcess.TicketData.GUID;
                    sqlParams.AddSqlParameter(new SqlParameter("@GUIDBASE64", System.Data.SqlDbType.NVarChar)).Value = TicketProcess.TicketData.GUIDBASE64;
                    sqlParams.AddSqlParameter(new SqlParameter("@TICKETCODE", System.Data.SqlDbType.NVarChar)).Value = TicketProcess.TicketData.TICKETCODE;
                    sqlParams.AddSqlParameter(new SqlParameter("@TIMESTAMP", System.Data.SqlDbType.DateTime2)).Value = TicketProcess.TicketData.TIMESTAMP;
                    sqlParams.AddSqlParameter(new SqlParameter("@COMMITHOST", System.Data.SqlDbType.NVarChar)).Value = TicketProcess.TicketData.COMMITHOST;
                    sqlParams.AddSqlParameter(new SqlParameter("@COMMITUSER", System.Data.SqlDbType.NVarChar)).Value = TicketProcess.TicketData.COMMITUSER;
                    sqlParams.AddSqlParameter(new SqlParameter("@REQUESTPRINTER", System.Data.SqlDbType.NVarChar)).Value = TicketProcess.TicketData.REQUESTPRINTER;
                    sqlParams.AddSqlParameter(new SqlParameter("@PRINTINGTIME", System.Data.SqlDbType.DateTime2)).Value = TicketProcess.TicketData.PRINTINGTIME;
                    sqlParams.AddSqlParameter(new SqlParameter("@CREATESOFTWARE", System.Data.SqlDbType.NVarChar)).Value = TicketProcess.TicketData.CREATESOFTWARE;
                    sqlParams.AddSqlParameter(new SqlParameter("@DOCUMENTNAME", System.Data.SqlDbType.NVarChar)).Value = TicketProcess.TicketData.DOCUMENTNAME;
                    sqlParams.AddSqlParameter(new SqlParameter("@PLOTPAPERSIZE", System.Data.SqlDbType.NVarChar)).Value = TicketProcess.TicketData.PLOTPAPERSIZE;
                    sqlParams.AddSqlParameter(new SqlParameter("@PAPERSIZE", System.Data.SqlDbType.NVarChar)).Value = TicketProcess.TicketData.PAPERSIZE;
                    sqlParams.AddRange(packed.SQLPARAMS);

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                    command.CommandText = @"INSERT INTO FILESTORE(" + packed.STR1 + ")" + " VALUES(" + packed.STR2 + ")";
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

                    command.Parameters.AddParams(sqlParams);

                    int ansline = command.ExecuteNonQuery();

                    db.connection.Close();

                    // 別アセンブリからdynamicを取得するため、ExpandoObjectを使用する。
                    ans.ANS = true;
                    ans.MSG = @"command.ExecuteNonQuery()実行・影響を受けた行:" + ansline;

                    if (eventLog)
                        SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5102, $"ToyoStageService.DataBaseInsert.SqlDBInsertExecute(..)\n成功しました。SQL command.CommandText は 次の通り\n{command.CommandText}");
                    return ans;
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5102, $"{AssemblyInternalName} SqlDBInsertExecute(..)にて例外検知   ServerVersion:{SqlConnectonServerVersion} DataSource:{SqlConnectonDataSource} Database:{SqlConnectonDatabase} ex.Message={ex.Message}");

                // 別アセンブリからdynamicを取得するため、ExpandoObjectを使用する。
                ans.ANS = false;
                ans.MSG = $"SqlDBInsertExecute(..) ServerVersion:{SqlConnectonServerVersion} DataSource:{SqlConnectonDataSource} Database:{SqlConnectonDatabase} 例外メッセージ {ex.Message}";
                return ans;
            }
        }

        /// <summary>
        /// VariantデータベースにInsert
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private dynamic VariantSqlDBInsertExecute(SQLSV db, string TIKCETCODE, string PARTNUMBER)
        {
            dynamic ans = new ExpandoObject();
            try
            {
                using (SqlCommand command = db.connection.CreateCommand())
                {
                    db.connection.Open();

                    List<SqlParameter> sqlParams = new List<SqlParameter>();

                    sqlParams.AddSqlParameter(new SqlParameter("@TICKETCODE", System.Data.SqlDbType.NVarChar)).Value = TIKCETCODE;
                    sqlParams.AddSqlParameter(new SqlParameter("@PARTNUMBER", System.Data.SqlDbType.NVarChar)).Value = PARTNUMBER;

                    dynamic packed = new
                    {
                        SQLPARAMS = sqlParams,
                        STR1 = "TICKETCODE,PARTNUMBER",
                        STR2 = "@TICKETCODE,@PARTNUMBER",
                    };

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                    command.CommandText = @"INSERT INTO VARIANT(" + packed.STR1 + ")" + " VALUES(" + packed.STR2 + ")";
                    SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5102, $"ToyoStageService.DataBaseInsert.VariantSqlDBInsertExecute(..)\ncommand.CommandTextを生成・・" +
                        $"=\n{command.CommandText}");
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

                    command.Parameters.AddParams(sqlParams);

                    int ansline = command.ExecuteNonQuery();

                    db.connection.Close();


                    // 別アセンブリからdynamicを取得するため、ExpandoObjectを使用する。
                    ans.ANS = true;
                    ans.MSG = @"command.ExecuteNonQuery()実行・影響を受けた行:" + ansline;

                    return ans;
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5102, $"{AssemblyInternalName} VariantSqlDBInsertExecute(..)にて例外検知 ex.Message={ex.Message}");

                // 別アセンブリからdynamicを取得するため、ExpandoObjectを使用する。
                ans.ANS = false;
                ans.MSG = $"VariantSqlDBInsertExecute() 例外メッセージ ex.Message= {ex.Message}";
                return ans;
            }
        }

        /// <summary>
        /// TicketDataからキーを指定し値を得る。コンストラクタにてチケット情報が更新されていることが前提
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public object GetTicketValue(string Key)
        {
            return TicketProcess.TicketData.GetParamKeyValue(Key);
        }

        /// <summary>
        /// ■■チケット情報の各項目をサニタイズする（全角->半角化、ゼロ補完、 図面種類特定など）
        /// </summary>
        /// <param name="ticketObj"></param>
        /// <returns></returns>
        static void SanitizingTicket(CommonTicket ticketObj)
        {
            #region ※※※メソッド内メソッドの定義※※※
            // CommonTicket.Param に 指定したkeyが存在しない場合はkeyのみ追加する
            void _SanitaizingRequiredKeyAdd(CommonTicket _ticket, string key)
            {
                // Param<List> の中で必須keyの存在を確認する。（Valueがなくても追加する）
                if (_ticket.Params.FindLast(a => a.Key == key).Key == null)
                {
                    _ticket.Params.Add(new CommonTicket.Param { Key = key });
                    SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Warning, 5102, $"key名 {key} が Paramsに無いため追加しました");
                }
            }


            // サニタイズ関数「SANITIZEDPARTNUMBER」専用（空白を削除したのち、全角を半角にする。（半角ｶﾅは使用しない）最後に、ハイフンで区切られた3項目目をゼロ保管処理）
            CommonTicket.Param _Sanitaizing_SANITIZEDPARTNUMBER(string key, string orgValue)
            {
                string sanitaizedValue = SanitizingPARTSNUMBERzeroPadding(orgValue);

                if (StageServerConfig.Config.TESTMODE == true)
                {
                    sanitaizedValue = CommonTicket.GetSererTestModeSanitizedPartNumber(sanitaizedValue);
                }
                
                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, $"_Sanitaizing_SANITIZEDPARTNUMBER(..)  key=\"{key}\" orgValue=\"{orgValue}\"  sanitaizedValue\"{sanitaizedValue}\"");

                return new CommonTicket.Param { Key = key, Value = sanitaizedValue };
            }


            // 図面種類判定関数
            CommonTicket.Param _Sanitaizing_DRAWINGTYPE(string key, string PARTNUMBER)
            {
                string drawTypeStr = GetToyoDrawingTypeName(SanitizingPARTSNUMBERzeroPadding(PARTNUMBER), "データベース登録時");
                ;
                // null なら 分類不能 にする
                if (drawTypeStr == null)
                {
                    drawTypeStr = "分類不能";
                    SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Warning, 5102, $"▲_SanitaizingGetDRAWINGTYPEforPARTNUMBER(..) 警告：TicketCode【{ticketObj.TICKETCODE}】,【{PARTNUMBER}】について GetToyoDrawingType(..)の戻り値がnullのため、{key}を'分類不能'にしました");
                    MailNotice.SendEmailFromCommonLibrary("ToyoDATABASE", "警告", $"▲_SanitaizingGetDRAWINGTYPEforPARTNUMBER(..) 警告：TicketCode【{ticketObj.TICKETCODE}】,【{PARTNUMBER}】について GetToyoDrawingType(..)の戻り値がnullのため、{key}を'分類不能'にしました" , mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);
                }

                return new CommonTicket.Param { Key = key, Value = drawTypeStr };
            }

            // サニタイズ関数 日時文字列清書
            CommonTicket.Param _SanitaizingDATETIMEString(string key, string orgValue)
            {
                if (!string.IsNullOrEmpty(orgValue))
                {
                    string sanitaizedValue;
                    try
                    {
                        sanitaizedValue = DateTime.Parse(orgValue).ToShortDateString();
                    }
                    catch (Exception ex)
                    {
                        sanitaizedValue = "";

                        SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Warning, 5102, $"_SanitaizingSimplificationString4()のエラー。\n" +
                            $"{ex.Message}\n{key} => {orgValue} 文字列は空行とします" +
                            $"\nチケットコード：\n{ticketObj.TICKETCODE}"
                            );
                    }

                    return new CommonTicket.Param { Key = key, Value = sanitaizedValue };

                }
                return new CommonTicket.Param { Key = key, Value = "" };

            }

            // サニタイズ関数 REV専用（入力文字が"0"なら""を返す.全角は半角へ）
            CommonTicket.Param _Sanitaizing_REV(string key, string orgValue)
            {
                if (!string.IsNullOrEmpty(orgValue))
                {
                    string sanitaizedValue = orgValue.Replace(" ", "").Replace("　", "").Zen2HanANK(); try
                    {
                        if (orgValue == "0")
                        {
                            sanitaizedValue = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        sanitaizedValue = "";

                        SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Warning, 5102, $"_SanitaizingSimplificationString4()のエラー。\n" +
                            $"{ex.Message}\n{key} => {orgValue} 文字列は空行とします" +
                            $"\nチケットコード：\n{ticketObj.TICKETCODE}"
                            );
                    }

                    return new CommonTicket.Param { Key = key, Value = sanitaizedValue };

                }
                return new CommonTicket.Param { Key = key, Value = "" };

            }

            // サニタイズ関数（空白を削除したのち、全角を半角にする。（半角ｶﾅは使用しない））
            CommonTicket.Param _SanitaizingSimplificationString(string key, string orgValue)
            {
                if (orgValue != null)
                {
                    string sanitaizedValue = orgValue.Replace(" ", "").Replace("　", "").Zen2HanANK();

                    return new CommonTicket.Param { Key = key, Value = sanitaizedValue };
                }
                return new CommonTicket.Param { Key = key, Value = "" };
            }

            #endregion

            // サニタイズ開始


            if (string.IsNullOrEmpty(ticketObj.REQUESTPRINTER))
                ticketObj.REQUESTPRINTER = ""; // チケットファイルに REQUESTPRINTER の設定を取得できない場合 "" とする

            // 指定したkeyがParamsにない場合は追加する
            _SanitaizingRequiredKeyAdd(ticketObj, "PARTNUMBER");
            _SanitaizingRequiredKeyAdd(ticketObj, "SANITIZEDPARTNUMBER");
            _SanitaizingRequiredKeyAdd(ticketObj, "REV");
            _SanitaizingRequiredKeyAdd(ticketObj, "AUTHOR");
            _SanitaizingRequiredKeyAdd(ticketObj, "AUTHORDATE");
            _SanitaizingRequiredKeyAdd(ticketObj, "DESIGNER");
            _SanitaizingRequiredKeyAdd(ticketObj, "CHECKDATE");
            _SanitaizingRequiredKeyAdd(ticketObj, "DRAWINGTYPE");
            _SanitaizingRequiredKeyAdd(ticketObj, "ORDERNUMBER");
            _SanitaizingRequiredKeyAdd(ticketObj, "TITLE");
            _SanitaizingRequiredKeyAdd(ticketObj, "DESCRIPTION");

            _SanitaizingRequiredKeyAdd(ticketObj, "MATERIAL");
            _SanitaizingRequiredKeyAdd(ticketObj, "MATERIALCODE");
            _SanitaizingRequiredKeyAdd(ticketObj, "MACHINETYPE");
            _SanitaizingRequiredKeyAdd(ticketObj, "FIRSTCUSTOMER");
            _SanitaizingRequiredKeyAdd(ticketObj, "CUSTOMER");

            // CommonTicket.Param を調査しサニタイズ関数によって書き換え
            // Paramに追加されたキーはすべて下記で操作必要。
            for (int count = 0; count < ticketObj.Params.Count; count++)
            {
                switch (ticketObj.Params[count].Key)
                {
                    case "SANITIZEDPARTNUMBER":
                        // ■重要■ SANITIZEDPARTNUMBER はここで"PARTNUMBER"をもとにサニタイズされ書き換えられます。
                        ticketObj.Params[count] = _Sanitaizing_SANITIZEDPARTNUMBER(ticketObj.Params[count].Key, (string)ticketObj.GetParamKeyValue("PARTNUMBER"));
                        break;
                    case "DRAWINGTYPE":
                        ticketObj.Params[count] = _Sanitaizing_DRAWINGTYPE(ticketObj.Params[count].Key, (string)ticketObj.GetParamKeyValue("PARTNUMBER"));
                        break;
                    case "AUTHORDATE":
                        ticketObj.Params[count] = _SanitaizingDATETIMEString(ticketObj.Params[count].Key, (string)ticketObj.GetParamKeyValue("AUTHORDATE"));
                        break;
                    case "CHECKDATE":
                        ticketObj.Params[count] = _SanitaizingDATETIMEString(ticketObj.Params[count].Key, (string)ticketObj.GetParamKeyValue("CHECKDATE"));
                        break;
                    case "APPROVEDDATE":
                        ticketObj.Params[count] = _SanitaizingDATETIMEString(ticketObj.Params[count].Key, (string)ticketObj.GetParamKeyValue("APPROVEDDATE"));
                        break;
                    case "REV":
                        ticketObj.Params[count] = _Sanitaizing_REV(ticketObj.Params[count].Key, (string)ticketObj.GetParamKeyValue("REV"));
                        break;
                    default:
                        /// 他のすべてのキーも一度下記の処理を行う。
                        ticketObj.Params[count] = _SanitaizingSimplificationString(ticketObj.Params[count].Key, ticketObj.Params[count].Value);
                        break;
                }
            }

            //デバッグ内容表示
            foreach (var Param in ticketObj.Params) { SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"サニタイズ結果 {Param.Key}→{Param.Value}"); }
        }

        /// <summary>
        /// ■サニタイズ関数　部品図・組図のみゼロ補完のため.大文字に統一後、全角文字列を半角に変更（例外あり）を実行後、ゼロ補完処理
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SanitizingPARTSNUMBERzeroPadding(string orgValue)
        {
            string orgVauleToUppered = orgValue.ToUpper(); //2022-03-02 大文字化
            string str = orgVauleToUppered.Replace(" ", "").Replace("　", "").Replace("ー", "-").Zen2HanANK(); // 全角の対応を実行 全角の長音記号を半角のマイナスへ

            if (Regex.IsMatch(str, @"^[A-Z][A-Z0-9]*-\d{5}-\d{1,3}[A-Z]?[A-Z]?$") || (Regex.IsMatch(str, @"^[A-Z][A-Z0-9]*-\d{5}$")))
            {
                string zeroPaddingPartNumber;
                // 文字列を'-'で分解
                string[] splitWord = str.Split('-');
                // いくつの単語に分かれたか調査
                switch (splitWord.Length)
                {
                    case 0:
                        //
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"GetZeroPaddingPARTNUMBER(string)配列内に要素なし={str}");
                        zeroPaddingPartNumber = str;
                        return zeroPaddingPartNumber;
                    case 1:
                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"GetZeroPaddingPARTNUMBER(string)配列要素1コのみ={str}");
                        zeroPaddingPartNumber = String.Join("-", splitWord);
                        return zeroPaddingPartNumber;
                    case 2:
                        // M-10201,M-123446,
                        // M-11123456R 等、ハイフン一つで構成されている文字列の処理
                        SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"GetZeroPaddingPARTNUMBER(string)配列要素2コ={str}");
                        if (splitWord[1] == "")
                        {
                            return str;
                        }
                        // 1つ目のハイフンの次のトークンの最後が英数字の場合、-000を付加。
                        // M-10201  -> M-10201-000
                        // M-ABCDEFG3   ->  M-ABCDEFG3-000
                        else if (char.IsNumber(splitWord[1], splitWord[1].Length - 1))
                        {
                            zeroPaddingPartNumber = str + "-000";
                            return zeroPaddingPartNumber;
                        }
                        // 1つ目のハイフンの次のトークンの最後がRLの場合、-000RLを付加
                        // M-10201RL    ->  M-10201-000RL
                        else if (Regex.IsMatch(str, @"[Rr][Ll]$"))
                        {
                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"GetZeroPaddingPARTNUMBER(string)1つめのハイフンの次のトークンの最後がRLかrlかRlかrLです={str}");
                            zeroPaddingPartNumber = Regex.Replace(str, @"[Rr][Ll]$", "-000RL");
                            return zeroPaddingPartNumber;
                        }
                        // 1つ目のハイフンの次のトークンの最後がRの場合、-000Rを付加
                        // M-10201R M-10201-000R
                        else if (Regex.IsMatch(str, @"[Rr]$"))
                        {
                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"GetZeroPaddingPARTNUMBER(string)1つめのハイフンの次のトークンの最後がRかrです={str}");
                            zeroPaddingPartNumber = Regex.Replace(str, @"[Rr]$", "-000R");
                            return zeroPaddingPartNumber;

                        }
                        // 1つ目のハイフンの次のトークンの最後がLの場合、-000Lを付加
                        // M-10201L M-10201-000L
                        else if (Regex.IsMatch(str, @"[Ll]$"))
                        {
                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"GetZeroPaddingPARTNUMBER(string)1つめのハイフンの次のトークンの最後がLかlです={str}");
                            zeroPaddingPartNumber = Regex.Replace(str, @"[Ll]$", "-000L");
                            return zeroPaddingPartNumber;
                        }
                        // 該当なしの場合、分割した文字列を再結合
                        else
                        {
                            //
                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"GetZeroPaddingPARTNUMBER(string)該当なし。そのまま再結合します={str}");
                            zeroPaddingPartNumber = String.Join("-", splitWord);
                            return zeroPaddingPartNumber;

                        }

                    case 3:
                        // 文字列中のハイフンの数が2個以上はここで処理
                        // M-10201-R    ->  M-10201-000R
                        // M-10201-RL    ->  M-10201-000RL
                        // M-10201-12RL    ->  M-10201-012RL
                        // M-10201-1    ->  M-10201-001
                        // M-10201-3R    ->  M-10201-003RL
                        //SharedClassLibrary.DebugClass.ConsoleDebugOut(0, "GetZeroPaddingPARTNUMBER(string)配列要素３こ以上={0}\n", str);
                        splitWord[2] = StringUtil.GetStringZeroPadding(splitWord[2], 3);
                        zeroPaddingPartNumber = String.Join("-", splitWord);
                        return zeroPaddingPartNumber;
                    default:
                        return str;
                }
            }
            return str;
        }

        /// <summary>
        /// ■図面種類を判定する 共通メソッド SasaLib.NumberingSupport.ToyoDrawingTypeClassify.CheckNumber()を呼び出す
        /// </summary>
        /// <param name="PARTNUMBER"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static string GetToyoDrawingTypeName(string PARTNUMBER, string Msg = "")
        {
            SasaLib.NumberingSupport.NumberTypeConfig.DrawingType drawingType;
            bool isVariant = true;
            string suffixMIN = null;
            string suffixMAX = null;
            /// 図面種類文字列
            string TypeName;
            bool result = SasaLib.NumberingSupport.ToyoDrawingTypeClassify.CheckNumber(PARTNUMBER, out drawingType, ref isVariant, ref suffixMIN, ref suffixMAX, out TypeName);
            if (result)
            {
                return TypeName;
            }
            else
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Error, 5102, $"GetToyoDrawingType(..)にて【{PARTNUMBER}】を '分類不能' と認識しました。メッセージ【{Msg}】");
                MailNotice.SendEmailFromCommonLibrary("ToyoDATABASE", "エラー報告", $"GetToyoDrawingType(..)にて【{PARTNUMBER}】を '分類不能' と認識しました。メッセージ【{Msg}】",mailAccount, StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR);

                return null;
            }
        }

        /// <summary>
        /// PARTSNAMEとDESCRIPTIONをDRAWINGTYPEに合わせて準備
        /// </summary>
        /// <param name="ticketObj"></param>
        static void AddPARTSNAMEorDESCRIPTION(CommonTicket ticketObj)
        {
            if ("部品図" == (string)ticketObj.GetParamKeyValue("DRAWINGTYPE"))
            {
                ticketObj.ReplaceOrCreateKeyValue("PARTSNAME", (string)ticketObj.GetParamKeyValue("TITLE"));
            }
            else
            {
                ticketObj.ReplaceOrCreateKeyValue("DESCRIPTION", (string)ticketObj.GetParamKeyValue("TITLE"));
            }
        }

        /// <summary>
        /// SqlParameterで使用する、列名、パラメーター それぞれを 引数で指定した文字列と、CommonTicketクラスから抽出したものと合わせて作る
        /// </summary>
        /// <param name="ColumnNames">カラムの指定。　例 "A,B,C"</param>
        /// <param name="Parameters">カラムの指定。　例 "@A,@B,@C"</param>
        /// <param name="ticketObj">CommonTicket 型</param>
        /// <returns></returns>
        static dynamic MakeSqlCommandLines(string ColumnNames, string Parameters, CommonTicket ticketObj)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            // ローカル関数
            string JoinSqlParam(string sepa, List<CommonTicket.Param> vars)
            {
                StringBuilder joinsqlsb = new StringBuilder();
                foreach (CommonTicket.Param a in vars)
                {
                    joinsqlsb.Append(sepa + a.Key);
                }
                return joinsqlsb.ToString();
            }

            // スタート
            System.Text.StringBuilder InserDBsb = new System.Text.StringBuilder();
            foreach (CommonTicket.Param paramObj in ticketObj.Params)
            {
                InserDBsb.Append("\n" + "key=" + paramObj.Key + " value=" + paramObj.Value);

                SqlParameter sqlParam = new SqlParameter("@" + paramObj.Key, paramObj.Value);
                sqlParams.AddSqlParameter(sqlParam);
            }
            //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASE", EventLogEntryType.Information, 5102, $"MakeSqlCommandLines(..) InserDBsb = {InserDBsb.ToString()}\n\n②sqlParams = {sqlParams}");

            string str1 = ColumnNames + JoinSqlParam(", ", ticketObj.Params);
            string str2 = Parameters + JoinSqlParam(", @", ticketObj.Params);

            // dynamic型は別アセンブリから参照不可注意
            return new
            {
                SQLPARAMS = sqlParams,
                STR1 = str1,
                STR2 = str2
            };
        }
    }
}
