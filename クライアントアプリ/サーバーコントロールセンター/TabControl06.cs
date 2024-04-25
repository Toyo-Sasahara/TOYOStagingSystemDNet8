using SasaLib;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SasaLib.NumberingSupport;
using System.Threading;
using SasaLib.ArcSuitePreview;
using ClientApp.Forms;

namespace ServerControlCenterApplication
{
    public partial class TabControl06 : UserControl
    {
        Form1 mainForm;

        private PreviewImageForm previewArcSuiteForm = new PreviewImageForm();


        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="form"></param>
        public TabControl06(Form1 form)
        {
            this.mainForm = form;

            InitializeComponent();
        }

        private void TabControl06_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ■このタブコントロールの表示状態が変わったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl06_VisibleChanged(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("TabControl06_VisibleChanged(..)実行・・・\r\n");

            Task.Run(() =>
            {
                MethodInvoker method = () =>
                {
                    accountUserForm.SetToControls();

                }; if (InvokeRequired) { Invoke(method); } else { method(); }
            });
        }

        /// <summary>
        /// ■アカウントパネルからフォーカスが移動したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountUserForm_Leave(object sender, EventArgs e)
        {
            accountUserForm.ControlChanged(sender, e);
        }

        private void accountUserForm7_Paint(object sender, PaintEventArgs e)
        {

        }


        public void LogWindowWriteLine(string msg)
        {
            try
            {
                if (this.InvokeRequired)
                {//https://qiita.com/taiyakisun/items/15b57df979eae7562aef
                    this.Invoke(new Action<string>(this.UpdateText), msg);
                }
                else
                {
                    UpdateText($"{msg}\n");
                }
            }
            catch (Exception ex)
            {
                logWindowControl.WriteLine($"例外検知{ex.Message}\r\n");

            }
        }
        private void UpdateText(string msg)
        {
            logWindowControl.WriteLine($"{msg}\r\n");
        }

        /// <summary>
        /// ■ArcSuiteから複数の図面を検索し指定フォルダ（サーバーから見たUNCフォルダ）へ一括書き出し　"GetArcSuiteContents"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetContent_button_Click(object sender, EventArgs e)
        {
            logWindowControl.WriteLine($"未完成\r\n");

            string target_ServiceID_CabinetID = target_ServiceID_CabinetID_textBox.Text;

            //\r\n区切りの文字列を配列化する
            string[] ZUBANz = DOWNLOADDRAWINGLIST_textBox.Text.Replace("\r", "").Split("\n".ToCharArray());
            //配列をList<string>コレクションに変換する
            List<string> ZUBANstrings = new List<string>(ZUBANz);

            List<string> resultList;
            RemoteClientDRAWREGIST remoteClientDRAWREGIST = new RemoteClientDRAWREGIST(
                DomainName:SccConfig.Config.ClientDomainName,
                UserName:SccConfig.Config.ClientUserName,
                UserPassword:SccConfig.Config.ClientUserPassword,
                ClsLogonDummy:SccConfig.Config.ClsLogon,
                PipeServerName:SccConfig.Config.StageServerHost,
                PipeName:SccConfig.Config.PipeNameDR
                );

            if (Command_MAINCOMMAND.GetArcSuiteContents(target_ServiceID_CabinetID, ZUBANstrings, DOWNLOADFOLDER_textBox.Text, out resultList, logWindowControl.WriteLine) != false)
            {
                foreach (var downloadFilePath in resultList)
                {
                    logWindowControl.WriteLine($"保存しました：{downloadFilePath}\r\n");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetArcSuiteLatestDrawingFiles_button_Click(object sender, EventArgs e)
        {
            logWindowControl.WriteLine($"未完成\r\n");

            string target_ServiceID_CabinetID = target_ServiceID_CabinetID_textBox.Text;

            //\r\n区切りの文字列を配列化する
            string[] ZUBANz = DOWNLOADDRAWINGLIST_textBox.Text.Replace("\r", "").Split("\n".ToCharArray());
            //配列をList<string>コレクションに変換する
            List<string> ZUBANstrings = new List<string>(ZUBANz);

            List<KeyValuePair<string, string>> fileLists = new List<KeyValuePair<string, string>>();
            string  resultMsg;

            RemoteClientDRAWREGIST remoteClientDRAWREGIST = new RemoteClientDRAWREGIST(
                DomainName: SccConfig.Config.ClientDomainName,
                UserName: SccConfig.Config.ClientUserName,
                UserPassword: SccConfig.Config.ClientUserPassword,
                ClsLogonDummy: SccConfig.Config.ClsLogon,
                PipeServerName: SccConfig.Config.StageServerHost,
                PipeName: SccConfig.Config.PipeNameDR
                );
            remoteClientDRAWREGIST.GetArcSuiteLatestDrawingFiles(target_ServiceID_CabinetID, ZUBANstrings, DOWNLOADFOLDER_textBox.Text, ref fileLists, out resultMsg, logWindowControl.WriteLine);
        }


        private void GetArcSuiteLatestDrawing_button_Click(object sender, EventArgs e)
        {
            logWindowControl.WriteLine($"【GetArcSuiteLatestDrawing】コマンドテスト開始。検索図番：{GetArcSuiteZUBAN_textBox.Text}");

            var zuban = GetArcSuiteZUBAN_textBox.Text;
            var img = Command_MAINCOMMAND.GetArcSuiteLatestDrawing(zuban, logWindowControl.WriteLine);

            if (previewArcSuiteForm.Visible == false)
                previewArcSuiteForm.Show(this);
            previewArcSuiteForm.SetImage(img);

            pictureBox1.Image = img;
        }

        private void GetArcSuiteLatestDrawingFile_button_Click(object sender, EventArgs e)
        {
            var zuban = GetArcSuiteZUBAN_textBox.Text;
            var target_ServiceID_CabinetID = serviceID_cabinetID2_textBox.Text;
            var result = Command_MAINCOMMAND.GetArcSuiteLatestDrawingFile(target_ServiceID_CabinetID, zuban, DrawingFileSavePath_textBox.Text);
        }

        CancellationTokenSource tokenSource;


        private async void GetArcSuiteAtrtributeButton_Click(object sender, EventArgs e)
        {
            tokenSource = new CancellationTokenSource();
            // 非同期処理をCancelするためのTokenを取得.
            var cancelToken = tokenSource.Token;

            await Aa(cancelToken);

            MessageBox.Show(this,"完了");

        }

        private void CancelToken_button_Click(object sender, EventArgs e)
        {
            // 処理をキャンセル.
            tokenSource.Cancel();
        }

        private async Task<string> Aa(CancellationToken cancelToken)
        {
            foreach (var zuban in ArcSuiteZubanTextBox.Lines)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    DebugConsole.WriteLine("Canceled");
                    // キャンセルされたらTaskを終了する.
                    return "Canceled";
                }
                var b = await Task.Run<string>(() =>
                (
                    arcsuite(zuban, @"C:\Users\Public\Documents\TOYOCOMMON\AutocadTOYOaddin\コミットテスト図面.dwg")
                ));
            }
            return "";
        }

        private string arcsuite(string sanitizedNumber, string CadDataFullFileName)
        {
            try
            {
                /// リモート操作をするオブジェクトを生成
                RemoteClientDRAWREGIST remoteClientDR = new RemoteClientDRAWREGIST(
                    SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon,
                    SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR
                    );

                remoteClientDR.ClientTimeOut = 8000;

                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken ct = cts.Token;

                ///ｱｰｸｽｲｰﾄへ問合せを実行する。時間コスト高い処理
                CheckArcSuiteData arcSuiteDuplicateConfirm = new CheckArcSuiteData(remoteClientDR);
                ArcsuitePreview s_arcSuitePreview = arcSuiteDuplicateConfirm.QueryStart(sanitizedNumber,ct, null);

                if (s_arcSuitePreview.Normality == true)
                {
                    if (s_arcSuitePreview.Found == true)
                    {
                        logWindowControl.WriteLine(
                        $"■図面 {sanitizedNumber} が見つかりました 図面番号:{s_arcSuitePreview.user_zuban} 部品名:{s_arcSuitePreview.user_partname} 説明:{s_arcSuitePreview.user_description}" +
                        $"改版:{s_arcSuitePreview.system_editionNumber} 登録日時{s_arcSuitePreview.system_createdon} {s_arcSuitePreview.user_cadtype_string}"
                        );

                        string msg;
                        DateTime dateTime_system_createdOn = ArcSuiteSupport.GetSyssmteCreatedOnTime(s_arcSuitePreview.system_createdon);

                        bool isCadDataOld = s_arcSuitePreview.IsCadDataOlderThanArcSuite(System.IO.File.GetCreationTime(CadDataFullFileName), System.IO.File.GetLastWriteTime(CadDataFullFileName), RemoteClientCADtype.CadType.InventorModel, s_arcSuitePreview.user_cadtype, dateTime_system_createdOn, out msg);
                        if (isCadDataOld)
                        {
                            logWindowControl.WriteLine($"◆◆◆◆コンポーネント {CadDataFullFileName} はアークスイート側で改版されています！！！ {msg}◆◆◆◆");

                        }


                    } /// ArcSuiteに sanitizedNumber の図面が見つかった場合
                    else
                    {
                        logWindowControl.WriteLine($"図面 {sanitizedNumber} は検索できませんでした。");


                    } /// ArcSuiteに sanitizedNumber 図面が見つからない場合
                } /// ArcSuiteの接続に成功している場合
                else
                {
                    logWindowControl.WriteLine($"※ArcSuiteの接続に失敗しています");
                } /// ArcSuiteの接続に失敗している場合

                return "";
            }
            catch (Exception ex)
            {
                logWindowControl.WriteLine($"※SearcArcSuiteRegistedで例外{ex.Message}");
                SasaLib.Eventlog.Log.WriteEntry("AutoCADaddinCommit", EventLogEntryType.Error, 0, $"※SearcArcSuiteRegistedで例外{ex.Message}");
                return null;
            }
        }



        private void GetArcSuiteAtrtribute2Button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未実装");

        }

        private void GetArcsuiteAttrFromObjectIdButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未実装");

        }

        private void AttrMergeArcSuiteButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未実装");

        }

    }
}
