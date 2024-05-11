using SasaLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ToyoMcMfg.Staging.DataBaseConfig;

namespace ToyoStageService
{

    /// <summary>
    /// データベースに対する実務オペレーションクラス
    /// </summary>
    public static class DataBasePracticalOperations
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        /// <summary>
        /// 時間記述文字列をDateTimeに変換。
        /// </summary>
        /// <param name="DateTimeStr"></param>
        /// <returns></returns>
        public static DateTime DateTimeStrToDateTime(string DateTimeStr)
        {
            DateTime dateTime = DateTime.MinValue;

            try
            {
                if (string.IsNullOrWhiteSpace(DateTimeStr) == true)
                    dateTime = DateTime.Now;//現在の日付を取得
                else
                {
                    try { dateTime = DateTime.ParseExact(DateTimeStr, "yyyy-MM-dd HH:mm:ss.fffffff", null); }
                    catch (FormatException)
                    {
                        try { dateTime = DateTime.ParseExact(DateTimeStr, "yyyy.MM.dd HH:mm:ss.fffffff", null); }
                        catch (FormatException)
                        {
                            try { dateTime = DateTime.ParseExact(DateTimeStr, "yyyy/MM/dd HH:mm:ss.fffffff", null); }
                            catch (FormatException)
                            {
                                try { dateTime = DateTime.ParseExact(DateTimeStr, "yyyy-MM-dd HH:mm:ss", null); }
                                catch (FormatException)
                                {
                                    try { dateTime = DateTime.ParseExact(DateTimeStr, "yyyy.MM.dd HH:mm:ss", null); }
                                    catch (FormatException)
                                    {
                                        try { dateTime = DateTime.ParseExact(DateTimeStr, "yyyy/MM/dd HH:mm:ss", null); }
                                        catch (FormatException)
                                        {
                                            try { dateTime = DateTime.ParseExact(DateTimeStr, "yyyy-MM-dd", null); }
                                            catch (FormatException)
                                            {
                                                try { dateTime = DateTime.ParseExact(DateTimeStr, "yyyy.MM.dd", null); }
                                                catch (FormatException)
                                                {
                                                    try { dateTime = DateTime.ParseExact(DateTimeStr, "yyyy/MM/dd", null); }
                                                    catch (FormatException ex)
                                                    {
                                                        SasaLib.Eventlog.Log.WriteEntry("DataBasePracticalOperations", EventLogEntryType.Information, 5103,
                                                        $"DataBasePracticalOperations.DateTimeStrToDateTime(..) DateTimeStr = \"{DateTimeStr}\" 日付文字列をDateTimeに変換に失敗 {ex.Message} DateTime.Nowに代替しました");
                                                        dateTime = DateTime.Now;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry($"ToyoDATABASEpracticalOperations.DateTimeStrToDateTime(...) にて例外検知 {ex.Message} {ex.InnerException}", EventLogEntryType.Information, 5103,
                    $"{AssemblyInternalName} DataBasePracticalOperations.");
            }
            // 
            return dateTime;
        }

        /// <summary>
        /// ■ステージサーバーデータベースへ承認関係のアップデート
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <param name="SignTypeFieldName"></param>
        /// <param name="SignDateFieldName"></param>
        /// <param name="DateStr"></param>
        /// <param name="SignFULLNAME"></param>
        /// <param name="SingPCUserNameField">クライアントにサインインしたユーザーを登録するﾃﾞｰﾀﾍﾞｰｽフィールド名</param>
        /// <param name="PCUserName">クライアントにサインインしたユーザー</param>
        /// <returns></returns>
        public static bool UpdateSignInfo(string GUIDBASE64, string SignTypeFieldName, string SignDateFieldName, string DateStr, string SignFULLNAME, string SingPCUserNameField = "", string PCUserName = "", EventsSummary evt = null)
        {
            try
            {
                if (evt == null)
                    evt = new EventsSummary("ToyoDATABASEpracticalOperations", 5104);

                evt.Add($"UpdateSignInfo({GUIDBASE64},{SignTypeFieldName},{SignDateFieldName},{DateStr},{SignFULLNAME},{SingPCUserNameField},{PCUserName},{evt})開始");

                DateTime dateTime = DateTimeStrToDateTime(DateStr); // 日付文字列の変換

                // データベースアップデートのクラスのｵﾌﾞｼﾞｪｸﾄ宣言
                DataBaseUpdate DataBaseUpdate;

                if (String.IsNullOrWhiteSpace(SingPCUserNameField) == true)
                {
                    // SingPCUserNameFieldに文字列が与えられていない場合

                    //データベースに登録する キーと値、型を準備
                    FieldValueSet fieldValueSet = new FieldValueSet
                    {
                        Params = new List<FieldValueSet.Param>()
                        {
                            new FieldValueSet.Param
                            {
                                Field = SignDateFieldName , Value = dateTime,
                                SqlDBType = System.Data.SqlDbType.DateTime2
                            },
                            new FieldValueSet.Param
                            {
                                Field = SignTypeFieldName , Value = SignFULLNAME,
                                SqlDBType = System.Data.SqlDbType.NVarChar
                            }
                        }
                    };

                    // ﾃﾞｰﾀﾍﾞｰｽ ｵﾌﾞｼﾞｪｸﾄの初期化 と 更新
                    DataBaseUpdate = new DataBaseUpdate("GUIDBASE64", GUIDBASE64, fieldValueSet,  evt);
                    // SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASEpracticalOperations", EventLogEntryType.Information, 5103, $"{AssemblyInternalName} DataBasePracticalOperations.UpdateSignInfo({GUIDBASE64},{SignTypeFieldName},{SignDateFieldName},{SignFULLNAME}) を実行しました");
                    evt.Add($"UpdateSignInfo({GUIDBASE64},{SignTypeFieldName},{SignDateFieldName},{SignFULLNAME}) を実行しました");
                }
                else
                {
                    // SingPCUserNameFieldに文字列が与えられているなら。

                    //データベースに登録する キーと値、型を準備（SingPCUserNameFieldに文字列が与えられている場合）
                    FieldValueSet fieldValueSetWithLogonUserName = new FieldValueSet
                    {
                        Params = new List<FieldValueSet.Param>()
                        {
                            new FieldValueSet.Param
                            {
                                //Field = SignDateFieldName , Value = dbInsertDateTimeString,
                                Field = SignDateFieldName , Value = dateTime,
                                //SqlDBType = System.Data.SqlDbType.NVarChar
                                SqlDBType = System.Data.SqlDbType.DateTime2 ///APPROVEDDATEのデータベース更新方法に変更7/1
                            },
                            new FieldValueSet.Param
                            {
                                Field = SignTypeFieldName , Value = SignFULLNAME,
                                SqlDBType = System.Data.SqlDbType.NVarChar
                            },
                            new FieldValueSet.Param
                            {
                                Field = SingPCUserNameField , Value = PCUserName,
                                SqlDBType = System.Data.SqlDbType.NVarChar
                            }
                        }
                    };

                    // ﾃﾞｰﾀﾍﾞｰｽ ｵﾌﾞｼﾞｪｸﾄの初期化 と 更新
                    DataBaseUpdate = new DataBaseUpdate("GUIDBASE64", GUIDBASE64, fieldValueSetWithLogonUserName, evt);
                    //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASEpracticalOperations", EventLogEntryType.Information, 5103, $"{AssemblyInternalName} DataBasePracticalOperations.UpdateSignInfo({GUIDBASE64},{SignTypeFieldName},{SignDateFieldName},{SignFULLNAME},PCUserName={PCUserName}) を実行しました");
                    evt.Add($"UpdateSignInfo({GUIDBASE64},{SignTypeFieldName},{SignDateFieldName},{SignFULLNAME},PCUserName={PCUserName}) を実行しました");
                }

                if (DataBaseUpdate.ResultNumbrOfLines > 0)
                {
                    evt.Add($"UpdateSignInfo() 戻り値は True です");

                    return true;
                }
                else
                {
                    evt.Add($"▲UpdateSignInfo() 戻り値は False です");

                    return false;
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry($"ToyoDATABASEpracticalOperations.UpdateSignInfo(...) にて例外検知 {ex.Message} {ex.InnerException}", EventLogEntryType.Information, 5103,
                    $"{AssemblyInternalName} DataBasePracticalOperations.");

                return false;

            }
        }

        /// <summary>
        /// 承認情報をアップデートする
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <param name="SignTypeFieldName"></param>
        /// <param name="SignDateFieldName"></param>
        /// <param name="DateStr"></param>
        /// <param name="SignFULLNAME"></param>
        /// <param name="SingPCUserNameField"></param>
        /// <param name="PCUserName"></param>
        /// <param name="OperateClientComputerNameField"></param>
        /// <param name="OperateClientComputerName"></param>
        /// <returns></returns>
        public static bool UpdateSignInfo(string GUIDBASE64, string SignTypeFieldName, string SignDateFieldName, string DateStr, string SignFULLNAME, string SingPCUserNameField, string PCUserName, string OperateClientComputerNameField, string OperateClientComputerName, EventsSummary evt = null)
        {
            try
            {
                if (evt == null)
                    evt = new EventsSummary("ToyoDATABASEpracticalOperations", 5104);

                evt.Add($"UpdateSignInfo({GUIDBASE64},{SignTypeFieldName},{SignDateFieldName},{DateStr},{SignFULLNAME},{SingPCUserNameField},{PCUserName},{OperateClientComputerNameField},{OperateClientComputerName})開始");

                DateTime dateTime = DateTimeStrToDateTime(DateStr); // 日付文字列の変換

                // データベースアップデートのクラスのｵﾌﾞｼﾞｪｸﾄ宣言
                DataBaseUpdate DataBaseUpdate;

                //データベースに登録する キーと値、型を準備（SingPCUserNameFieldに文字列が与えられている場合）
                FieldValueSet fieldValueSetWithLogonUserName = new FieldValueSet
                {
                    Params = new List<FieldValueSet.Param>()
                {
                    new FieldValueSet.Param
                    {
                       //Field = SignDateFieldName , Value = dbInsertDateTimeString,
                       Field = SignDateFieldName , Value = dateTime,
                       //SqlDBType = System.Data.SqlDbType.NVarChar
                        SqlDBType = System.Data.SqlDbType.DateTime2
                    },
                    new FieldValueSet.Param
                    {
                       Field = SignTypeFieldName , Value = SignFULLNAME,
                       SqlDBType = System.Data.SqlDbType.NVarChar
                    },
                    new FieldValueSet.Param
                    {
                       Field = SingPCUserNameField , Value = PCUserName,
                       SqlDBType = System.Data.SqlDbType.NVarChar
                    },
                    new FieldValueSet.Param
                    {
                       Field = OperateClientComputerNameField , Value = OperateClientComputerName,
                       SqlDBType = System.Data.SqlDbType.NVarChar
                    },
                }
                };

                // ﾃﾞｰﾀﾍﾞｰｽ ｵﾌﾞｼﾞｪｸﾄの初期化 と 更新
                DataBaseUpdate = new DataBaseUpdate("GUIDBASE64", GUIDBASE64, fieldValueSetWithLogonUserName,  evt);

                //SasaLib.Eventlog.Log.WriteEntry("ToyoDATABASEpracticalOperations", EventLogEntryType.Information, 5103,
                //    $"{AssemblyInternalName} DataBasePracticalOperations.UpdateSignInfo({GUIDBASE64},{SignTypeFieldName},{SignDateFieldName},{SignFULLNAME},PCUserName={PCUserName} ,OperateClientComputerName={OperateClientComputerName}) を実行しました");

                if (DataBaseUpdate.ResultNumbrOfLines > 0)
                {
                    evt.Add($"UpdateSignInfo() 戻り値は True です");
                    return true;
                }
                else
                {
                    evt.Add($"▲UpdateSignInfo() 戻り値は False です");

                    return false;
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry($"ToyoDATABASEpracticalOperations.UpdateSignInfo(...) にて例外検知 {ex.Message} {ex.InnerException}", EventLogEntryType.Information, 5103,
                    $"{AssemblyInternalName} DataBasePracticalOperations.");

                return false;
            }
        }

        /// <summary>
        /// ■【2019-06-27時点正規のルート】StageServer側のデータベースを更新　APPROVEDDATEとAPPROVEDUSER,ARCSUITEID
        /// </summary>
        /// <param name="B64String"></param>
        /// <returns></returns>
        public static bool UpdateArcSuiteRegistData(string B64String, string ARCSUITEID, string ApprovedUser, EventsSummary evt)
        {
            /// <summary>
            /// 登録用の日付
            /// </summary>
            System.DateTime dateTime;
            //現在の日付を取得
            dateTime = DateTime.Now;

            try
            {
                if (evt == null)
                    evt = new EventsSummary("ToyoDATABASEpracticalOperations", 5104);

                evt.Add($"UpdateArcSuiteRegistData()開始");

                //データベースに登録する キーと値、型を準備
                FieldValueSet fieldValueSet = new FieldValueSet
                {
                    Params = new List<FieldValueSet.Param>()
                {
                    new FieldValueSet.Param
                    {
                       Field = "APPROVEDDATE", Value = dateTime,
                       SqlDBType = System.Data.SqlDbType.DateTime2
                    },
                    new FieldValueSet.Param
                    {
                       Field = "APPROVEDUSER", Value = ApprovedUser,
                       SqlDBType = System.Data.SqlDbType.NVarChar
                    },
                    new FieldValueSet.Param
                    {
                       Field = "ARCSUITEID", Value = ARCSUITEID,
                       SqlDBType = System.Data.SqlDbType.NVarChar
                    }

                }
                };

                // StageServer側のデータベースをアップデートする。
                DataBaseUpdate DataBaseUpdate = new DataBaseUpdate("GUIDBASE64", B64String, fieldValueSet, evt);
                if (DataBaseUpdate.ResultNumbrOfLines > 0)
                {
                    evt.Add($"UpdateArcSuiteRegistData() 戻り値は True です");
                    return true;
                }
                else
                {
                    evt.Add($"▲UpdateArcSuiteRegistData() 戻り値は False です");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry($"DataBasePracticalOperations.UpdateArcSuiteRegistData(...) にて例外検知 {ex.Message} {ex.InnerException}", EventLogEntryType.Information, 5103,
                    $"{AssemblyInternalName} DataBasePracticalOperations.");

                return false;
            }
        }
    }
}
