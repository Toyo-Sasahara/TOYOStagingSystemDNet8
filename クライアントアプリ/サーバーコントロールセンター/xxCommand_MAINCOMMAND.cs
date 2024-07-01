//using SasaLib;
//using SasaLib.PrintConfig;
//using StageServerRemote;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Runtime.Versioning;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Xml.Serialization;
//using ToyoMcMfg.Staging.DataBaseConfig;
//using ToyoMcMfg.Staging.RemoteObjects;

//namespace ServerControlCenterApplication
//{
//    [SupportedOSPlatform("windows")]


//    public static class Command_MAINCOMMAND
//    {

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="PARTNUMBER"></param>
//        /// <returns></returns>
//        public static string CheckDrawingType(string PARTNUMBER, SasaLibDelegateWriteLine WriteLine = null)
//        {
//            if (WriteLine == null)
//                WriteLine = Console.WriteLine;

//            var hostname = SccConfig.Config.StageServerHost;

//            RemoteClientDRAWCAPTURE stageserver = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDC);
//            var DrawingType = stageserver.CHECK_DRAWING_TYPE(hostname, PARTNUMBER);

//            WriteLine($"接続先[{SccConfig.Config.StageServerHost}],パイプ名:[{SccConfig.Config.PipeNameDC}], 調査した図番:{PARTNUMBER} 結果：{DrawingType}");

//            return DrawingType;
//        }

//        /// <summary>
//        /// チケットコードが実在するか
//        /// </summary>
//        /// <param name="TICKETCODE"></param>
//        /// <returns></returns>
//        public static bool IsTICKETCODEexist(string TICKETCODE, out FieldValueSet result, SasaLibDelegateWriteLine WriteLine = null)
//        {
//            if (WriteLine == null)
//                WriteLine = Console.WriteLine;

//            SqlFieldValue sqlStr = new SqlFieldValue()
//            {
//                Field = "TICKETCODE",
//                Value = TICKETCODE,
//                SqlDBType = System.Data.SqlDbType.NVarChar
//            };

//            RemoteClientDataBase rMdataBase = new RemoteClientDataBase(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword,
//                SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

//            var ans = rMdataBase.DataBaseSearch3(sqlStr);

//            if (ans.Count == 1)
//            {
//                result = ans[0];
//                return true;
//            }
//            else
//            {
//                WriteLine($"検索結果 {ans.Count} 件");
//                result = null;
//                return false;
//            }

//        }


//        /// <summary>
//        /// ■承認印押印テスト
//        /// </summary>
//        /// <param name="TKCKETCODE"></param>
//        /// <param name="UserID"></param>
//        /// <param name="Approved2cResult">イベントハンドラを指定</param>
//        /// <param name="delegateWriteLine"></param>
//        public static void ApprovedMainProcessDebug(string TKCKETCODE, string UserID,EventHandler<ApprovedStatus> Approved2cResult, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            if (delegateWriteLine == null)
//                delegateWriteLine = Console.WriteLine;

//            RemoteClientMaintenance rMmaintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

//            rMmaintenance.ApprovedStatus += Approved2cResult;
//            Task.Run(() =>
//            {
//                //手動押印テスト
//                bool ans = rMmaintenance.ApprovedMainProcessDebug(TKCKETCODE, UserID);

//                if (ans)
//                {

//                    delegateWriteLine($" rMmaintenance.ApprovedMainProcessDebug({TKCKETCODE}, {UserID}) は成功したようです");
//                }
//                else
//                {
//                    delegateWriteLine($" rMmaintenance.ApprovedMainProcessDebug({TKCKETCODE}, {UserID}) は失敗しました");

//                }
//            });
//        }


//        /// <summary>
//        /// ■メモリぱっぷどファイル書き込み
//        /// </summary>
//        /// <param name="Label"></param>
//        /// <param name="typestr"></param>
//        /// <param name="Value"></param>
//        /// <param name="WriteLine"></param>
//        //public static void SetDRAWWATCHserviceMmap(string Label, string typestr, object Value, SasaLibDelegateWriteLine delegateWriteLine = null)
//        //{
//        //    if (delegateWriteLine == null)
//        //        delegateWriteLine = Console.WriteLine;

//        //    RemoteClientSYSTEMWATCH remoteDRAWCAPTURE = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
//        //        SccConfig.Config.ClientUserName,
//        //        SccConfig.Config.ClientUserPassword,
//        //        SccConfig.Config.ClsLogon,
//        //        SccConfig.Config.StageServerHost,
//        //        "WatchService");

//        //    var result = remoteDRAWCAPTURE.SetDRAWWATCHserviceMmapvalue(SccConfig.Config.StageServerHost, Label, typestr, Value);
//        //    delegateWriteLine($"{result}");
//        //}


//        //public static void GetMMAPDFiles()
//        //{

//        //}

//        /// <summary>
//        /// コミット受付状態をチェックする
//        /// </summary>
//        /// <param name="Message"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        public static bool CheckCommitRecepitonState(out string Message, SasaLibDelegateWriteLine WriteLine = null)
//        {
//            if (WriteLine == null)
//                WriteLine = Console.WriteLine;

//            var StageServerHost = SccConfig.Config.StageServerHost;
//            var ClientDomainName = SccConfig.Config.ClientDomainName;
//            var ClientUserName = SccConfig.Config.ClientUserName;
//            var ClientUserPassword = SccConfig.Config.ClientUserPassword;
//            var ClsLogon = SccConfig.Config.ClsLogon;
//            var PipeName = SccConfig.Config.PipeNameDC;

//            StageServerRemote.RemoteClientDRAWCAPTURE remoteDC = new StageServerRemote.RemoteClientDRAWCAPTURE(
//                ClientDomainName,
//                ClientUserName,
//                ClientUserPassword,
//                ClsLogon,
//                StageServerHost,
//                PipeName
//                );
//            string msg = remoteDC.CommitRecepitonState(StageServerHost);

//            WriteLine($"\\\\{StageServerHost}\\PIPE\\{PipeName} サーバーからの返答 {msg}");

//            if (msg == "NORMAL" || msg == "" || msg == null)
//            {
//                Message = msg;
//                return true;
//            }
//            else
//            {
//                Message = msg;
//                return false;
//            }
//        }

//        /// <summary>
//        /// 承認システム受付状態をチェックする
//        /// </summary>
//        /// <param name="Message"></param>
//        /// <param name="WriteLine"></param>
//        /// <returns></returns>
//        public static bool CheckApprovalRecepitonState(out string Message, SasaLibDelegateWriteLine WriteLine = null)
//        {
//            if (WriteLine == null)
//                WriteLine = Console.WriteLine;

//            var StageServerHost = SccConfig.Config.StageServerHost;
//            var ClientDomainName = SccConfig.Config.ClientDomainName;
//            var ClientUserName = SccConfig.Config.ClientUserName;
//            var ClientUserPassword = SccConfig.Config.ClientUserPassword;
//            var ClsLogon = SccConfig.Config.ClsLogon;
//            var PipeName = SccConfig.Config.PipeNameDR;

//            StageServerRemote.RemoteClientDRAWREGIST remoteDR = new StageServerRemote.RemoteClientDRAWREGIST(
//                ClientDomainName,
//                ClientUserName,
//                ClientUserPassword,
//                ClsLogon,
//                StageServerHost,
//                PipeName
//                );
//            string msg = remoteDR.ApprovalRecepitonState(WriteLine);

//            WriteLine($"\\\\{StageServerHost}\\PIPE\\{PipeName} サーバーからの返答 {msg}");

//            if (msg == "NORMAL")
//            {
//                Message = msg;
//                return true;
//            }
//            else
//            {
//                Message = msg;
//                return false;
//            }
//        }

//        /// <summary>
//        /// ■ArcSuiteから複数の図面を検索し指定フォルダ（サーバーから見たUNCフォルダ）へ一括書き出し　"GetArcSuiteContents"
//        /// </summary>
//        /// <param name="ZUBANstrings"></param>
//        /// <param name="DownloadFolder"></param>
//        /// <param name="listResult"></param>
//        /// <param name="WriteLine"></param>
//        /// <returns></returns>
//        public static bool GetArcSuiteContents(string target_ServiceID_CabinetID, List<string> ZUBANstrings, string DownloadFolder, out List<string> listResult, SasaLibDelegateWriteLine WriteLine = null)
//        {
//            if (WriteLine == null)
//                WriteLine = Console.WriteLine;

//            var StageServerHost = SccConfig.Config.StageServerHost;
//            var ClientDomainName = SccConfig.Config.ClientDomainName;
//            var ClientUserName = SccConfig.Config.ClientUserName;
//            var ClientUserPassword = SccConfig.Config.ClientUserPassword;
//            var ClsLogon = SccConfig.Config.ClsLogon;
//            var PipeName = SccConfig.Config.PipeNameDR;

//            RemoteClientDRAWREGIST remoteDR = new StageServerRemote.RemoteClientDRAWREGIST(
//                ClientDomainName,
//                ClientUserName,
//                ClientUserPassword,
//                ClsLogon,
//                StageServerHost,
//                PipeName
//                );
//            listResult = remoteDR.GetArcSuiteContents(target_ServiceID_CabinetID, ZUBANstrings, DownloadFolder, WriteLine:WriteLine);

//            if (listResult == null)
//                return false;
//            else
//                return true;
//        }


//        /// <summary>
//        /// ■ArcSuiteから図面を検索し見つかればSystem.Drawing.Imageオブジェクトとして取得する。（最初の1ページのみ）
//        /// </summary>
//        /// <param name="ZUBAN"></param>
//        /// <param name="WriteLine"></param>
//        /// <returns></returns>
//        public static System.Drawing.Image GetArcSuiteLatestDrawing(string ZUBAN, SasaLibDelegateWriteLine WriteLine = null)
//        {
//            if (WriteLine == null)
//                WriteLine = Console.WriteLine;

//            var StageServerHost = SccConfig.Config.StageServerHost;
//            var ClientDomainName = SccConfig.Config.ClientDomainName;
//            var ClientUserName = SccConfig.Config.ClientUserName;
//            var ClientUserPassword = SccConfig.Config.ClientUserPassword;
//            var ClsLogon = SccConfig.Config.ClsLogon;
//            var PipeName = SccConfig.Config.PipeNameDR;

//            RemoteClientDRAWREGIST remoteDR = new StageServerRemote.RemoteClientDRAWREGIST(
//                ClientDomainName,
//                ClientUserName,
//                ClientUserPassword,
//                ClsLogon,
//                StageServerHost,
//                PipeName
//                );
//            System.Drawing.Image listResult = remoteDR.GetArcSuiteLatestDrawing(ZUBAN, WriteLine: WriteLine);

//            if (listResult == null)
//                return null;
//            else
//                return listResult;
//        }

//        public static bool GetArcSuiteLatestDrawingFile(string target_ServiceID_CabinetID, string ZUBAN, string LocalDistFullFileName, SasaLibDelegateWriteLine WriteLine = null)
//        {
//            if (WriteLine == null)
//                WriteLine = Console.WriteLine;

//            var StageServerHost = SccConfig.Config.StageServerHost;
//            var ClientDomainName = SccConfig.Config.ClientDomainName;
//            var ClientUserName = SccConfig.Config.ClientUserName;
//            var ClientUserPassword = SccConfig.Config.ClientUserPassword;
//            var ClsLogon = SccConfig.Config.ClsLogon;
//            var PipeName = SccConfig.Config.PipeNameDR;

//            RemoteClientDRAWREGIST remoteDR = new StageServerRemote.RemoteClientDRAWREGIST(
//                ClientDomainName,
//                ClientUserName,
//                ClientUserPassword,
//                ClsLogon,
//                StageServerHost,
//                PipeName
//                );

//            string resultMsg;
//            bool result = remoteDR.GetArcSuiteLatestDrawingFile(target_ServiceID_CabinetID, ZUBAN, LocalDistFullFileName, out resultMsg, WriteLine: WriteLine);

//            return result;
//        }

//    }
//}
