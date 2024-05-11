using SasaLib;
using SasaLib.NumberingSupport;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CommonCommitLogic
{
    [SupportedOSPlatform("windows")]
    internal class CommitSupport
    {
        CommitParam commitParam;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="commitParam"></param>
        internal CommitSupport(CommitParam commitParam)
        {
            this.commitParam = commitParam;
        }

        /// <summary>
        /// アセンブリバージョンを取得
        /// </summary>
        /// <returns></returns>
        internal string GetAssemblyVersion()
        {
            System.Diagnostics.FileVersionInfo ver =
                System.Diagnostics.FileVersionInfo.GetVersionInfo(
                System.Reflection.Assembly.GetExecutingAssembly().Location);

            string fileVer = ver.FileVersion;
            string assemblyVer = ver.ProductVersion;
            string fileName = ver.FileName;

            return $"{fileName}, 製品バージョン{assemblyVer} ファイルバージョン{fileVer}";
        }

        /// <summary>
        /// コミットプリンターの故障情報を保持（コントロールパネルのプリンタ名:false or true）
        /// </summary>
        private List<KeyValuePair<string, bool>> CommitPrinterIsFailStatus;

        /// <summary>
        ///コミットプリンターのエイリアスを保持する（コントロールパネルのプリンタ名:COMMIT1等）
        /// </summary>
        private List<KeyValuePair<string, string>> CommitPrinterNameAndAlias;

        /// <summary>
        /// コミットプリンターのショートカット名を保持する（コントロールパネルのプリンタ名:PRINTER1.LNK等）
        /// </summary>
        private List<KeyValuePair<string, string>> CommitPrinterShortCutName;

        /// <summary>
        /// コミット先が受付可能かを知らせる
        /// </summary>
        /// <returns></returns>
        internal bool CheckCommitRecepitonState(out string Message, string callerMsg, SasaLibDelegateWriteLine WriteLine)
        {
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();


            var StageServerHost = commitParam.StageServerHost;
            var ClientDomainName = commitParam.ClientDomainName;
            var ClientUserName = commitParam.ClientUserName;

            Encryption sasaLibencryptionPipeConnection = new Encryption("SasaAuth3.1");
            string PipeClientPlanePass = sasaLibencryptionPipeConnection.Decoding(commitParam.PipeConnection31Password); //復号化

            var ClientUserPassword = PipeClientPlanePass;
            var ClsLogon = commitParam.ClsLogon;
            var PipeNameDC = commitParam.PipeNameDC;

            StageServerRemote.RemoteClientDRAWCAPTURE stageserver = new StageServerRemote.RemoteClientDRAWCAPTURE(
                ClientDomainName,
                ClientUserName,
                ClientUserPassword,
                ClsLogon,
                StageServerHost,
                PipeNameDC
                );

            stageserver.ClientTimeOut = commitParam.CommitRecepitonStateConnectTimeOut; // NamedPipeClientStream.Connect のタイムアウト値
                                                                                        //stageserver.ClientTimeOut = 20000; // NamedPipeClientStream.Connect のタイムアウト値

            string msg = stageserver.CommitRecepitonState(StageServerHost);

            // sw.Stop(); TimeSpan ts = sw.Elapsed; // 計測終了
            //string timespanstr = $"■コミット先調査1 stageserver.CommitRecepitonState(StageServerHost) <<処理(A)経過時間:{ts.Hours} 時間{ts.Minutes}分 {ts.Seconds}秒 , ({sw.ElapsedMilliseconds}msec)>";
            //InventorAddInServer.s_DockableLogWindowForm.WriteLine($"{timespanstr}");

            //sw.Start();
            CommitPrinterIsFailStatus = stageserver.GetCommitPrinterIsFailStatus();
            CommitPrinterNameAndAlias = stageserver.GetCommitPrinterNameAndAlias();
            CommitPrinterShortCutName = stageserver.GetCommitPrinterShortCutName();

            //sw.Stop(); TimeSpan ts2 = sw.Elapsed; // 計測終了
            //string timespanstr2 = $"■コミット先調査2 stageserver.GetCommitPrinterSettingFromPaperSize(StageServerHost) <<処理(A)経過時間:{ts2.Hours} 時間{ts2.Minutes}分 {ts2.Seconds}秒 , ({sw.ElapsedMilliseconds}msec)>";
            //InventorAddInServer.s_DockableLogWindowForm.WriteLine($"{timespanstr2}");

            if (msg == "NORMAL" || msg == "" || msg == null)
            {
                Message = msg;
                return true;
            }
            else
            {
                WriteLine($"※ｺﾐｯﾄｻｰﾊﾞｰ受付できません。{callerMsg} CheckCommitRecepitonStateFalseCount を加算しました。理由：{msg}");
                Message = msg;

                commitParam.CheckCommitRecepitonStateFalseCount++; // エラーカウンタインクリメント
                return false;
            }

        }

        /// <summary>
        /// コミット先プリンターが故障中ならtrueを返す
        /// </summary>
        /// <param name="PrinterName"></param>
        /// <returns></returns>
        internal bool IsCommitPrinterFiler(string PrinterName, SasaLibDelegateWriteLine WriteLine)
        {
            if (CommitPrinterIsFailStatus == null)
                return false;

            var prinerControlPanelDriverName = GetPrinterName(PrinterName);
            if (prinerControlPanelDriverName != null)
            {
                bool ans = CommitPrinterIsFailStatus.FirstOrDefault(x => x.Key.ToUpper() == prinerControlPanelDriverName.ToUpper()).Value;
                return ans;
            }
            else
            {
                WriteLine($"プリンターショートカット名 {PrinterName} に 該当するコントロールパネル上のプリンタが見つかりません");
                return false;
            }
        }

        /// <summary>
        /// コントロールパネルのプリンタドライバ名をエイリアス名またはショートカット名から得る
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        private string GetPrinterName(string alias)
        {
            if (CommitPrinterNameAndAlias == null || CommitPrinterShortCutName == null)
                return null;

            if (string.IsNullOrWhiteSpace(alias))
                return null;

            string aliasNameToPrinterName = CommitPrinterNameAndAlias.FirstOrDefault(x => x.Value.ToUpper() == alias.ToUpper()).Key;
            string shortCutNameToPrinterName = CommitPrinterShortCutName.FirstOrDefault(x => x.Value.ToUpper() == alias.ToUpper()).Key;

            if (aliasNameToPrinterName == null && shortCutNameToPrinterName != null)
            {
                return shortCutNameToPrinterName;
            }
            else if (aliasNameToPrinterName != null && shortCutNameToPrinterName == null)
            {
                return aliasNameToPrinterName;
            }
            else
            {
                return aliasNameToPrinterName;
            }
        }


        /// <summary>
        /// 残りの処理待ち図面の調査
        /// </summary>
        /// <returns></returns>
        internal int GetGemainingDrawing()
        {
            List<string> TIFFfileList = new List<string>();


            int Count = System.IO.Directory.GetFiles(commitParam.CommitPath, "*.TIF").Length;

            return Count;
        }


    }
}
