using SasaLib;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ToyoMcMfg.Staging.DataBaseConfig;
using ToyoMcMfg.Staging.RemoteObjects;

namespace StageServerRemote
{

    /// <summary>
    /// メンテナンスを実行するクラス
    /// </summary>
    public class RemoteClientMaintenance : RMCsupport
    {

        /// <summary>
        /// 設定ファイル記述の接続種別及びアカウントをセット
        /// </summary>
        readonly bool ClsLogonDummy;
        readonly string DomainName;
        readonly string UserName;
        readonly string UserPassword;
        readonly string PipeServerName;
        readonly string PipeName;

        /// <summary>
        /// ■承認処理結果を得ることのできる含むイベントハンドラ
        /// </summary>
        public event EventHandler<ApprovedStatus> ApprovedStatus;

        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="ForcedDomainName"></param>
        /// <param name="ForcedUserName"></param>
        /// <param name="ForcedUserPassword"></param>
        /// <param name="ForcedAccountFlag"></param>
        /// <param name="PipeServerName"></param>
        /// <param name="PipeName"></param>
        public RemoteClientMaintenance(string ForcedDomainName, string ForcedUserName, string ForcedUserPassword, bool ForcedAccountFlag, string PipeServerName, string PipeName)
        {
            this.DomainName = ForcedDomainName;
            this.UserName = ForcedUserName;
            this.UserPassword = ForcedUserPassword;
            this.ClsLogonDummy = ForcedAccountFlag;
            this.PipeServerName = PipeServerName;
            this.PipeName = PipeName;
        }

        /// <summary>
        /// ■接続チェック
        /// 2022/08/24 時点 クライントソフトウェアから呼び出しを確認
        /// </summary>
        /// <param name="DelegateWriteLine"></param>
        /// <returns></returns>
        public bool ConnectCheck(SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;

            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                DelegateWriteLine(msg);
            }

            RemoteClientDRAWREGIST mainClientLogic = new RemoteClientDRAWREGIST(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                PipeName);

            bool ans = mainClientLogic.ConnectTest("ConnectCheck()から呼び出し", localLogWrite);
            return ans;
        }

        /// <summary>
        /// ■指定したGUIDBASE64コードの図面の実体及びレコードの削除
        /// 2022/08/24 時点 クライントソフトウェアとサーバーコントローラーから呼び出しを確認
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <param name="DelegateWriteLine"></param>
        /// <returns></returns>
        public bool RecordAndEntityfileDelete(string GUIDBASE64, SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;

            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                DelegateWriteLine(msg);
            }

            RemoteClientDataBase rMdataBase = new RemoteClientDataBase(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                PipeName);

            bool ans = rMdataBase.RecordAndEntityfileDelete(GUIDBASE64, localLogWrite);
            return ans;
        }

        /// <summary>
        /// ■ステージサーバーデータベース管理下のGUIDBASE64値の図面を印刷プロッタ設定ファイル*.XMLを指定して印刷実行
        /// 2022/08/24 時点 クライントソフトウェアとサーバーコントローラーから呼び出しを確認
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        public void PlotouDrawingGUIDBASE64(string GUIDBASE64, string PlotterSetting = "", SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null) DelegateWriteLine = DebugConsole.WriteLine;

            DelegateWriteLine("印刷");

            RemoteClientPrintProcess rMPrintProcess = new RemoteClientPrintProcess(DomainName, UserName, UserPassword, ClsLogonDummy, PipeServerName, PipeName);

            rMPrintProcess.PrintStart(GUIDBASE64, PlotterSetting);
        }

        /// <summary>
        /// ■手動押印テスト
        /// 2022/08/24 時点 クライントソフトウェアとサーバーコントローラーから呼び出しを確認
        /// </summary>
        /// <param name="TICKETCODE"></param>
        /// <param name="USERID"></param>
        public bool ApprovedMainProcessDebug(string TICKETCODE, string USERID, SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;

            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                DelegateWriteLine(msg);
            }

            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"■【{TICKETCODE}】を検索開始");
            List<FieldValueSet> DBresultList;

            //チケット名からDBを検索
            RemoteClientDRAWREGIST rMmainClientLogic = new RemoteClientDRAWREGIST(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                PipeName);

            DBresultList = rMmainClientLogic.DBsearchFromTICKETCODE(TICKETCODE);

            if (DBresultList != null)
            {

                if (DBresultList.Count == 1)  //検索結果が１つの場合
                {
                    localLogWrite($"■チケットコード【{TICKETCODE}】が１件検索されました");


                    RemoteClientDataBase approval = new RemoteClientDataBase(DomainName,
                        UserName,
                        UserPassword,
                        ClsLogonDummy,
                        PipeServerName,
                        PipeName);
                    string ApprovedMessage;


                    ApprovedStatus approvedStatuis = new ApprovedStatus();

                    var result = approval.Approved2c(DBresultList[0].SearchKey("GUIDBASE64"), USERID, ref approvedStatuis);

                    ApprovedStatus?.Invoke(this, approvedStatuis); // 承認処理結果を含むイベントを発火

                    ApprovedMessage = approval.AnserMessage;

                    if (result)
                    {
                        if (approval.AnserSucess)
                        {
                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"approval.AnserMessage = {approval.AnserMessage}");
                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"approval.Count = {approval.Count}");
                            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"approval.Count = {approval.DBresultList}");
                        }
                        return true;
                    }
                    else
                    {
                        localLogWrite($"■approval.Approved2b(...)戻り値がfalse, サーバーからのメッセージ {ApprovedMessage}");
                        return false;
                    }
                }
                else if (DBresultList.Count > 1)
                {
                    localLogWrite($"■チケットコード【{TICKETCODE}】が１件以上検索されました");
                    return false;
                }
                else
                {
                    localLogWrite($"ありえないエラー");
                    return false;
                }
            }
            else
            {
                localLogWrite($"みつかりません");
                return false;
            }
        }


        /// <summary>
        /// ■StageServer上の承認情報クリア デバッグ用
        /// 2022/08/24 時点 クライントソフトウェアとサーバーコントローラーから呼び出しを確認
        /// 2022/08/24 時点 ここから rMdataBase.DataBaseSearch3(value, 9999);を呼び出し
        /// 2022/08/24 時点 ここから rMdataBase.ApprovedCancels2(GUIDBASE64s)を呼び出し
        /// </summary>
        /// <param name="TICKETCODE">クリアするチケットコード</param>
        /// <returns></returns>
        public bool ApprovedCancel2(string TICKETCODE, EventHandler<bool> ApprovedCancel2Status = null, SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;

            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                DelegateWriteLine(msg);
            }

            SqlFieldValue value = new SqlFieldValue()
            {
                Field = "TICKETCODE",
                Value = TICKETCODE,
                SqlDBType = System.Data.SqlDbType.NVarChar
            };

            // DBを検索
            RemoteClientDataBase rMdataBase = new RemoteClientDataBase(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                PipeName);

            List<FieldValueSet> fieldValueSets;


            fieldValueSets = rMdataBase.DataBaseSearch3(value, 9999);

            rMdataBase.OnApprovedCancel2Result += ApprovedCancel2Status;

            if (fieldValueSets.Count == 1)
            {
                List<string> GUIDBASE64s = new List<string>() { fieldValueSets[0].SearchKey("GUIDBASE64") };

                bool ans = rMdataBase.ApprovedCancels2(GUIDBASE64s, localLogWrite);


                return ans;
            }
            else
            {
                localLogWrite($"{TICKETCODE} は見つかりません");


                return false;
            }
        }

        /// <summary>
        /// ■
        /// </summary>
        /// <param name="TICKETCODE"></param>
        /// <param name="DelegateWriteLine"></param>
        public void ApprovedCancel3(string TICKETCODE, SasaLibDelegateWriteLine DelegateWriteLine = null)
        {

        }

        /// <summary>
        /// ■FILESTORE内のファイルをすべて検索し、データベースにリンクされていないファイルの個数を調査
        /// 2022/08/24 時点 クライントソフトウェアとサーバーコントローラーから呼び出しを確認
        /// </summary>
        public int FILESTORECehck(SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;

            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                DelegateWriteLine(msg);
            }


            RemoteClientDRAWREGIST mainClientLogic = new RemoteClientDRAWREGIST(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                PipeName);

            int ans = mainClientLogic.SystemCheckFileStore(localLogWrite);

            DelegateWriteLine($"FILESTORE内のファイルをすべて検索し、データベースにリンクされていないファイルの個数は={ans}件です");
            return ans;
        }

        /// <summary>
        /// ■FILESTORE内のファイルをすべて検索し、データベースにリンクされていないファイルをゴミとして削除
        /// 2022/08/24 時点 クライントソフトウェアとサーバーコントローラーから呼び出しを確認
        /// </summary>
        public void FILESTORERepare(SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;

            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                DelegateWriteLine(msg);
            }

            RemoteClientDRAWREGIST mainClientLogic = new RemoteClientDRAWREGIST(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                PipeName);

            bool ans = mainClientLogic.SystemCheckFileStoreRepare(localLogWrite);

            DelegateWriteLine($"結果={ans}");

        }

        /// <summary>
        /// ■FILESTORE内のファイルをすべて検索し、データベースにリンクされていないファイルの個数を調査
        /// 2022/08/24 時点 クライントソフトウェアとサーバーコントローラーから呼び出しを確認
        /// </summary>
        public List<FieldValueSet> TICKETFILEexistCHeck(SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;

            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                DelegateWriteLine(msg);
            }

            RemoteClientDRAWREGIST mainClientLogic = new RemoteClientDRAWREGIST(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                PipeName);

            var ansers = mainClientLogic.SystemCheckTICKETFILEexist(localLogWrite);

            foreach (var x in ansers)
            {
                DelegateWriteLine($"TICKETCODE = {x}");
            }
            return ansers;
        }

        /// <summary>
        /// 2022/08/24 時点 どのメソッドからも未使用
        /// </summary>
        /// <param name="DelegateWriteLine"></param>
        /// <returns></returns>
        public List<FieldValueSet> RELOAD_STAGESERVERCONFIG(SasaLibDelegateWriteLine DelegateWriteLine = null)
        {
            if (DelegateWriteLine == null)
                DelegateWriteLine = DebugConsole.WriteLine;

            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                DelegateWriteLine(msg);
            }

            RemoteClientDRAWREGIST mainClientLogic = new RemoteClientDRAWREGIST(DomainName,
                UserName,
                UserPassword,
                ClsLogonDummy,
                PipeServerName,
                PipeName);

            var ansers = mainClientLogic.SystemCheckTICKETFILEexist(localLogWrite);

            foreach (var x in ansers)
            {
                DelegateWriteLine($"TICKETCODE = {x}");
            }
            return ansers;

        }
    }
}
