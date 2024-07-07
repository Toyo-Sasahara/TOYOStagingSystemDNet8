using SasaLib;
using SasaLib.Winlogon;
using SharedClassLibrary;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToyoMcMfg.Staging.DataBaseConfig;
using ToyoMcMfg.Staging.RemoteObjects;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
//using ToyoMcMfg.Staging.RemoteObjects;

namespace ServerControlCenterApplication
{
    public partial class TabControl03 : UserControl
    {
        Form1 mainForm;


        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="form"></param>
        public TabControl03(Form1 form)
        {
            this.mainForm = form;
            InitializeComponent();
        }

        private void TabControl3_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ■このタブコントロールの表示状態が変わったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl3_VisibleChanged(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("TabControl3_VisibleChanged(..)実行・・・\r\n");

            Task.Run(() =>
            {
                MethodInvoker method = () =>
                {
                    accountUserForm.SetToControls();



                    //mainForm.CommitPrinters.SetComboBox(ref PrinterSelcomboBox);

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



        private void TICKETCODEtextBox_Click(object sender, EventArgs e)
        {
            TICKETCODE_textBox.SelectAll();
        }

        private void GUIDBASE64_search_button_Click(object sender, EventArgs e)
        {

        }

        private void DirectSearchButton_Click(object sender, EventArgs e)
        {
            LogWindowWriteLine($"チケットコード {TICKETCODE_textBox.Text} を検索します");

            FieldValueSet result;

            TICKETCODE_textBox.Text = TICKETCODE_textBox.Text.Trim();

            if (Command_MAINCOMMAND.IsTICKETCODEexist(TICKETCODE_textBox.Text, out result, objectConvNew: objectConvNew_CheckBox.Checked, WriteLine: LogWindowWriteLine))
            {
                string PARTNUMBER = result.SearchKey("PARTNUMBER");
                string GUIDBASE64 = result.SearchKey("GUIDBASE64");
                LogWindowWriteLine($"チケット：{TICKETCODE_textBox.Text} 見つかりました GUIDBASE64={GUIDBASE64} , PARTNUMBER={PARTNUMBER}");

                ShowDrawing(GUIDBASE64);
            }
            else
            {
                LogWindowWriteLine($"チケット：{TICKETCODE_textBox.Text} 見つかりません");
                TICKETCODE_textBox.SelectAll();
            }

        }

        internal void ShowDrawing(string GUIDBASE64)
        {


            RemoteClientDRAWREGIST rmc_DRAWREGIST = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            // 部分表示
            Drawing_pictureBox.Image = rmc_DRAWREGIST.GetImageFromPIPE2(GUIDBASE64, objectConvNew: objectConvNew_CheckBox.Checked, WriteLine: logWindowControl.WriteLine);

        }

        /// <summary>
        /// 押印強制ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApprovedMainProcessDebugButton_Click(object sender, EventArgs e)
        {

            TICKETCODE_textBox.Text = TICKETCODE_textBox.Text.Trim();

            FieldValueSet result;
            if (Command_MAINCOMMAND.IsTICKETCODEexist(TICKETCODE_textBox.Text, out result, objectConvNew: objectConvNew_CheckBox.Checked, WriteLine: LogWindowWriteLine))
            {
                string PARTNUMBER = result.SearchKey("PARTNUMBER");
                string GUIDBASE64 = result.SearchKey("GUIDBASE64");
                LogWindowWriteLine($"チケット：{TICKETCODE_textBox.Text} 見つかりました GUIDBASE64={GUIDBASE64} , PARTNUMBER={PARTNUMBER}");

                LogWindowWriteLine($"押印強制実行 チケット：{TICKETCODE_textBox.Text}");
                Command_MAINCOMMAND.ApprovedMainProcessDebug(TICKETCODE_textBox.Text, UserID_textBox.Text, Approved2cResult, objectConvNew: objectConvNew_CheckBox.Checked, WriteLine: logWindowControl.WriteLine);

            }
            else
            {
                LogWindowWriteLine($"チケット：{TICKETCODE_textBox.Text} 見つかりません");
                TICKETCODE_textBox.SelectAll();
            }


        }



        private void Approved2cResult(Object sender, ApprovedStatus resultAnser)
        {

            if (resultAnser.ApprovedSucess)
            {
                LogWindowWriteLine($"イベントがキックされた ApprovedStatus.ApprovedSucess = {resultAnser.ApprovedSucess}");
                ShowDrawing(resultAnser.GUIDBASE64);
                LogWindowWriteLine($"ShowDrawing({resultAnser.GUIDBASE64})を実行しました");
            }
            else
            {
                LogWindowWriteLine($"イベントがキックされた ApprovedStatus.ApprovedMessage = {resultAnser.ApprovedSucess}");
            }

            LogWindowWriteLine($"ApprovedStatus.ApprovedMessage = {resultAnser.ApprovedMessage}");
            LogWindowWriteLine($"ApprovedStatus.ApprovedSignable = {resultAnser.ApprovedSignable}");

        }


        /// <summary>
        /// チケットコードが実在するか
        /// </summary>
        /// <param name="TICKETCODE"></param>
        /// <returns></returns>
        private bool IsTICKETCODEexist(string TICKETCODE, bool objectConvNew = false, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = DebugConsole.WriteLine;

            SqlFieldValue value = new SqlFieldValue()
            {
                Field = "TICKETCODE",
                Value = TICKETCODE,
                SqlDBType = System.Data.SqlDbType.NVarChar
            };

            RemoteClientDataBase rmc_DataBase = new RemoteClientDataBase(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

            var ans = rmc_DataBase.DataBaseSearch3(value, objectConvNew: objectConvNew);

            LogWindowWriteLine($"検索結果 {ans.Count} 件");

            if (ans.Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void ApprovedCancel2Status(Object sender, bool resultAnser)
        {
            LogWindowWriteLine($"イベントがキックされた ApprovedCancel2Status = {resultAnser}");
            MessageBox.Show($"結果{resultAnser}");
            TICKETCODE_textBox.Focus();
            TICKETCODE_textBox.SelectAll();

        }

        /// <summary>
        /// ●承認全キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApprovedRsetOneButton_Click(object sender, EventArgs e)
        {
            if (IsTICKETCODEexist(TICKETCODE_textBox.Text, objectConvNew_CheckBox.Checked, logWindowControl.WriteLine))
            {
                RemoteClientMaintenance rmc_Maintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

                //rMmaintenance.ApprovedCancel2Status += ApprovedCancel2Status;

                var ans = MessageBox.Show($"続行しますか？\r\n{TICKETCODE_textBox.Text}", "", MessageBoxButtons.YesNo);
                if (ans == DialogResult.Yes)
                {
                    var ans2 = MessageBox.Show($"ほんとうに続行しますか？\r\n{TICKETCODE_textBox.Text}", "", MessageBoxButtons.YesNo);
                    if (ans2 == DialogResult.Yes)
                    {
                        var cancelans = rmc_Maintenance.ApprovedCancel2(TICKETCODE_textBox.Text, ApprovedCancel2Status, objectConvNew: objectConvNew_CheckBox.Checked, WriteLine: logWindowControl.WriteLine);
                    }
                }
            }
            else
            {
                TICKETCODE_textBox.Text = "みつかりませんでした";
                TICKETCODE_textBox.SelectAll();
            }

            TICKETCODE_textBox.Focus();
            TICKETCODE_textBox.SelectAll();

        }

        private void DirectDeleteButton_Click(object sender, EventArgs e)
        {
            if (IsTICKETCODEexist(TICKETCODE_textBox.Text, objectConvNew_CheckBox.Checked, logWindowControl.WriteLine))
            {

                RemoteClientMaintenance rmc_Maintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

                string GUIDBASE64 = SasaLib.GUIDExtensions.GetB64StringFromB64FnameString(TICKETCODE_textBox.Text);


                var ans = MessageBox.Show($"削除します。続行しますか？\r\n{TICKETCODE_textBox.Text}", "", MessageBoxButtons.YesNo);
                if (ans == DialogResult.Yes)
                {
                    var ans2 = MessageBox.Show($"ほんとうに続行しますか？\r\n{TICKETCODE_textBox.Text}", "", MessageBoxButtons.YesNo);
                    if (ans2 == DialogResult.Yes)
                    {
                        var deleteAns = rmc_Maintenance.RecordAndEntityfileDelete(GUIDBASE64);
                        MessageBox.Show($"結果{deleteAns}");

                    }
                }
            }
            else
            {
                TICKETCODE_textBox.Text = "みつかりませんでした";
                TICKETCODE_textBox.SelectAll();
            }

            TICKETCODE_textBox.Focus();
            TICKETCODE_textBox.SelectAll();

        }

        private void ArcSuiteTestRegistButton_Click(object sender, EventArgs e)
        {
            //登録可能のフラグを立てる
            RemoteClientDRAWREGIST rmc_DRAWREGIST = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            var GUIDBASE64 = GUIDExtensions.GetB64StringFromB64FnameString(TICKETCODE_textBox.Text);

            List<string> GUIDBASE64s = new List<string>() { GUIDBASE64 };

            //rmarcSuite.SetRegistWaitingFlag(GUIDBASE64s, UserID_textBox.Text);

            logWindowControl.WriteLine($"強制通常登録実行 (SetRegistWaitingFlag2)");

            List<string> errList = new List<string>();
            rmc_DRAWREGIST.SetRegistWaitingFlag2(GUIDBASE64s, UserID_textBox.Text, ref errList, objectConvNew: objectConvNew_CheckBox.Checked, WriteLine: logWindowControl.WriteLine);

            if (errList.Count > 0)
            {
                // カンマ区切りの文字列に変換
                string result = String.Join(",", errList);

                logWindowControl.WriteLine($"※強制通常登録実行結果・エラーが含まれます。失敗したもの ({result})");
            }
            else
            {
                logWindowControl.WriteLine($"強制通常登録実行 エラーはありませんでした。");
            }

        }

        private void ApprovedCancel3Status(Object sender, List<string> resultAnser)
        {
            LogWindowWriteLine($"イベントがキックされた ApprovedCancel3Status = {resultAnser}");
            MessageBox.Show($"結果{resultAnser}");
            TICKETCODE_textBox.Focus();
            TICKETCODE_textBox.SelectAll();

        }


        /// <summary>
        /// ●ArcSuite登録予定フラグを消去
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuiteTestRegistResetButton_Click(object sender, EventArgs e)
        {
            FieldValueSet fieldValueSet;

            Command_MAINCOMMAND.IsTICKETCODEexist(TICKETCODE_textBox.Text, out fieldValueSet, objectConvNew: objectConvNew_CheckBox.Checked, LogWindowWriteLine);

            // 削除リスト
            List<ApprovedCancel> CancelList = new List<ApprovedCancel>()
            {
                new ApprovedCancel()
                {
                    FieldValueSet = fieldValueSet,
                    AUTHOR = false,
                    DESIGNER = false,
                    APPROVED = true,
                    APPROVEDPCUSER = true
                }
            };


            // キャンセル指示発動
            RemoteClientDataBase rmc_DataBase = new RemoteClientDataBase(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            rmc_DataBase.OnApprovedCancel3Result += ApprovedCancel3Status;

            StringBuilder sb = new StringBuilder();
            foreach (var item in CancelList)
            {
                var TICKETCODE = item.FieldValueSet.SearchKey("TICKETCODE");
                var PARTNUMBER = item.FieldValueSet.SearchKey("PARTNUMBER");
                sb.AppendLine($"{TICKETCODE}{PARTNUMBER}");
            }

            var CancelErrorGUIDBASE64List = new List<string>();
            bool ans = rmc_DataBase.ApprovedCancels3(CancelList, out CancelErrorGUIDBASE64List);

            if (ans == true)
            {
                GlovalValues.Mylog.WriteLine($"次の押印キャンセルが成功しています\n{sb.ToString()}");
            }
            else
            {
                // カンマ区切りの文字列に変換
                string cancelErrsListString = String.Join(",", CancelErrorGUIDBASE64List);

                GlovalValues.Mylog.WriteLine($"次の押印キャンセルはいずれかまたはすべて失敗しました\n{cancelErrsListString}");
            }

        }

        private void UnSetPRIORITYREGISTFLAGisNullButton_Click_1(object sender, EventArgs e)
        {
            if (IsTICKETCODEexist(TICKETCODE_textBox.Text, objectConvNew_CheckBox.Checked, logWindowControl.WriteLine))
            {

                var ans2 = MessageBox.Show($"優先登録フラグを設定します\r\n{TICKETCODE_textBox.Text}", "", MessageBoxButtons.YesNo);
                if (ans2 == DialogResult.Yes)
                {
                    List<string> GUIDBASE64List = new List<string>() { GUIDExtensions.GetB64StringFromB64FnameString(TICKETCODE_textBox.Text) };
                    LogWindowWriteLine($"■【{TICKETCODE_textBox.Text}】をSetPRIORITYREGISTFLAGisNull");

                    FieldValueSet updateDatabseObj = new FieldValueSet();

                    updateDatabseObj.Params.Add(new FieldValueSet.Param
                    {
                        Field = "PRIORITYREGISTFLAG",
                        Value = "1",
                        SqlDBType = System.Data.SqlDbType.NVarChar
                    });

                    RemoteClientDataBase rmc_DataBase = new RemoteClientDataBase(SccConfig.Config.ClientDomainName,
                        SccConfig.Config.ClientUserName,
                        SccConfig.Config.ClientUserPassword,
                        SccConfig.Config.ClsLogon,
                        SccConfig.Config.StageServerHost,
                        SccConfig.Config.PipeNameDR);

                    foreach (string GUIDBASE64 in GUIDBASE64List)
                    {
                        int count = rmc_DataBase.Update(GUIDBASE64, updateDatabseObj, objectConvNew: objectConvNew_CheckBox.Checked);
                        LogWindowWriteLine($"更新結果＝{count}行");
                    }

                }
            }
            else
            {
                TICKETCODE_textBox.Text = "みつかりませんでした";
                TICKETCODE_textBox.SelectAll();
            }
        }

        /// <summary>
        /// ●ArcSuite優先登録フラグの解除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnsetPRIORITYREGISTFLAGisNullButton_Click(object sender, EventArgs e)
        {
            if (IsTICKETCODEexist(TICKETCODE_textBox.Text, objectConvNew_CheckBox.Checked, logWindowControl.WriteLine))
            {

                var ans2 = MessageBox.Show($"優先登録フラグを解除します\r\n{TICKETCODE_textBox.Text}", "", MessageBoxButtons.YesNo);
                if (ans2 == DialogResult.Yes)
                {
                    List<string> GUIDBASE64List = new List<string>() { GUIDExtensions.GetB64StringFromB64FnameString(TICKETCODE_textBox.Text) };
                    LogWindowWriteLine($"■【{TICKETCODE_textBox.Text}】をSetPRIORITYREGISTFLAGisNull");

                    FieldValueSet updateDatabseObj = new FieldValueSet();

                    updateDatabseObj.Params.Add(new FieldValueSet.Param
                    {
                        Field = "PRIORITYREGISTFLAG",
                        Value = System.Data.SqlTypes.SqlString.Null,
                        SqlDBType = System.Data.SqlDbType.NVarChar
                    });

                    RemoteClientDataBase rmc_DataBase = new RemoteClientDataBase(SccConfig.Config.ClientDomainName,
                        SccConfig.Config.ClientUserName,
                        SccConfig.Config.ClientUserPassword,
                        SccConfig.Config.ClsLogon,
                        SccConfig.Config.StageServerHost,
                        SccConfig.Config.PipeNameDR);

                    foreach (string GUIDBASE64 in GUIDBASE64List)
                    {
                        int count = rmc_DataBase.Update(GUIDBASE64, updateDatabseObj);
                        LogWindowWriteLine($"更新結果＝{count}行");
                    }

                }
            }
            else
            {
                TICKETCODE_textBox.Text = "みつかりませんでした";
                TICKETCODE_textBox.SelectAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetArcSuiteAwaitingRegist_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserID_textBox.Text))
                    return;

                #region ArcSuite登録待機一覧リストを作成

                RemoteClientDataBase rmc_DataBase = new RemoteClientDataBase(
                        SccConfig.Config.ClientDomainName,
                        SccConfig.Config.ClientUserName,
                        SccConfig.Config.ClientUserPassword,
                        SccConfig.Config.ClsLogon,
                        SccConfig.Config.StageServerHost,
                        SccConfig.Config.PipeNameDR
                    );

                string hostname = SasaLib.Net.GetHOSTNAME();

                List<FieldValueSet> DBresultList1 = rmc_DataBase.GetArcSuiteAwaitingRegist(hostname, UserID_textBox.Text, 9999, "ORDER BY APPROVEDDATE ASC", objectConvNew: objectConvNew_CheckBox.Checked);
                #endregion


                if (DBresultList1.Count > 0)
                {

                    LogWindowWriteLine($"ArcSuite登録待ちリスト・現在{DBresultList1.Count}件");
                    foreach (FieldValueSet f in DBresultList1)
                    {
                        LogWindowWriteLine($"{f.Sucess} {f.Message} {f.SearchKey("TICKETCODE")}");

                    }
                }
                else
                {
                    LogWindowWriteLine($"ArcSuite登録待ちリストはありません");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }

        }

        private void CheckDrawingTypeButton_Click(object sender, EventArgs e)
        {
            CheckDrwingTypeAnserTextBox.Text = Command_MAINCOMMAND.CheckDrawingType(PARTNUMBERtextBox.Text, LogWindowWriteLine);
        }

        public void LogWindowWriteLine(string msg)
        {
            try
            {
                if (this.InvokeRequired)
                {//https://qiita.com/taiyakisun/items/15b57df979eae7562aef
                    Invoke(new Action<string>(this.UpdateText), msg);
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

        private void DirectPrintButton_Enter(object sender, EventArgs e)
        {

        }

        private void DirectPrintButton_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void TICKETCODE_DIRECT_panel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void DirectPrintButton_Click(object sender, EventArgs e)
        {

        }
    }
}
