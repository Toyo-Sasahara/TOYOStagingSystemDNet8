using ClientApp.Forms;
using SasaLib;
using SharedClassLibrary;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ToyoMcMfg.Staging.DataBaseConfig;
using ToyoMcMfg.Staging.RemoteObjects;
using ToyoStageService;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ServerControlCenterApplication
{
    public partial class TabControl02 : UserControl
    {
        Form1 mainForm;


        SearchDrawingSetPictureBox sdhelper;

        internal PreviewImageForm previewArcSuiteForm = new PreviewImageForm();


        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="form"></param>
        public TabControl02(Form1 form)
        {
            this.mainForm = form;
            InitializeComponent();
            logWindowControl = new LogWindowControl();
            accountUserForm = new AccountUserForm();

            sdhelper = new SearchDrawingSetPictureBox(DebugListView2, ResultSearchPattern_label, previewArcSuiteForm, logWindowControl.WriteLine);

        }

        private void TabControl02_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ■このタブコントロールの表示状態が変わったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl02_VisibleChanged(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("TabControl02_VisibleChanged(..)実行・・・\r\n");

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

        private void accountUserForm6_Paint(object sender, PaintEventArgs e)
        {

        }


        /// <summary>
        /// ■リストビューが選択された時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DebugListView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;
            sdhelper.GetAndViewDRAWINGimage(Properties.Resources.イメージ読込中, titleOnly: TitleOnly_checkBox.Checked, objectConvNew: object_ConvNew_CheckBox.Checked);
            if (previewArcSuiteForm.Visible == false)
                previewArcSuiteForm.Show(this);
        }

        private void DebugListView2_Click(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            var TICKETCODE = sdhelper.GetSelectedData("TICKETCODE");
        }

        private void DebugListView2_DoubleClick(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            var SANITIZEDPARTNUMBER = sdhelper.GetSelectedData("SANITIZEDPARTNUMBER");

            string ArcSuiteURL = $"http://{SccConfig.Config.ArcSuiteDmsHost}/ArcSuite/docspace/sdk/search.do" +
                                $"?enc=UTF-8&service=cn%3Ddrep_service%40ass1%2Cou%3Dcomponents%2Cdc%3D" +
                                $"ArcSuite&workspace=SYSTEM&condition=%E5%85%A8%E5%9B%B3%E9%9D%A2%E6%A4%9C%E7%B4%A2%E6%97%A7%E7%89%88%E5%90%AB%E3%82%80&item=0%3Aoperator%3AexactlyMatch&item=0%3Avalue%3A{SANITIZEDPARTNUMBER}";
            System.Diagnostics.Process.Start(ArcSuiteURL);

        }

        private void DebugListView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// ■ArcSuite登録済みを検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_ARCSUITEID_IsNotNULL_fromDB_Button_Click(object sender, EventArgs e)
        {
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.ArcSuiteRegisteredList(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");
            sdhelper.Search(sqlSearchStringValues, "ArcSuite登録済み", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_ARCSUITEIDisNull_And_APPROVEDUSERisNotNull_Button_Click(object sender, EventArgs e)
        {
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.ARCSUITEIDisNull_And_APPROVEDUSERisNotNull(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");
            sdhelper.Search(sqlSearchStringValues, "承認済みだがArcSuite未登録かつArcSuite登録指示フラグが無い", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_APPROVEDDATEisNotNull_And_REGISTEDTIMEisZero_And_REGISTWAITINGFLAGisZero_Button_Click(object sender, EventArgs e)
        {
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.APPROVEDDATEisNotNull_And_REGISTEDTIMEisZero_And_REGISTWAITINGFLAGisZero(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");
            sdhelper.Search(sqlSearchStringValues, "承認済みだがArcSuite未登録かつArcSuite登録指示フラグが無い", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);
        }


        /// <summary>
        /// ■設計承認の対象を検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindeCanApprovalButton_Click(object sender, EventArgs e)
        {
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.ApprovableList(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "設計承認が可能", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForcedSign_button_Click(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("押印よろしいか？") == DialogResult.OK)
            {


                if (MessageBox.Show($"{DebugListView2.SelectedItems.Count} 件 全件押印よろしいか？") == DialogResult.OK)
                {
                    if (DebugListView2.SelectedItems.Count > 0)
                    {
                        //sh2.Delete_SelectedDBresultListRecords();

                        foreach (System.Windows.Forms.ListViewItem selects in DebugListView2.SelectedItems)
                        {
                            string TICKETCODE = selects.SubItems["TICKETCODE"].Text;

                            logWindowControl.WriteLine($"押印強制実行");
                            Command_MAINCOMMAND.ApprovedMainProcessDebug(TICKETCODE, UserID_textBox.Text, Approved2cResult, objectConvNew: object_ConvNew_CheckBox.Checked, logWindowControl.WriteLine);
                        }
                    }
                }
            }

        }

        private void Approved2cResult(Object sender, ApprovedStatus resultAnser)
        {
            logWindowControl.WriteLine($"イベントがキックされた ApprovedStatus.ApprovedSucess = {resultAnser.ApprovedSucess}");

        }



        /// <summary>
        /// 強制登録フラグ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForcedRegistFlugEnable_Button_Click(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("強制通常登録よろしいか？") == DialogResult.OK)
            {
                if (MessageBox.Show($"{DebugListView2.SelectedItems.Count} 件 全件強制通常登録よろしいか？") == DialogResult.OK)
                {
                    if (DebugListView2.SelectedItems.Count >= 1)
                    {

                        foreach (System.Windows.Forms.ListViewItem selects in DebugListView2.SelectedItems)
                        {
                            string TICKETCODE = selects.SubItems["TICKETCODE"].Text;

                            logWindowControl.WriteLine($"強制通常登録実行 (SetRegistWaitingFlag2)");

                            //登録可能のフラグを立てる
                            RemoteClientDRAWREGIST rmc_DRAWREGIST = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                                SccConfig.Config.ClientUserName,
                                SccConfig.Config.ClientUserPassword,
                                SccConfig.Config.ClsLogon,
                                SccConfig.Config.StageServerHost,
                                SccConfig.Config.PipeNameDR);

                            var GUIDBASE64 = GUIDExtensions.GetB64StringFromB64FnameString(TICKETCODE);

                            List<string> GUIDBASE64s = new List<string>() { GUIDBASE64 };

                            //rmarcSuite.SetRegistWaitingFlag(GUIDBASE64s, UserID_textBox.Text);

                            List<string> errList = new List<string>();
                            rmc_DRAWREGIST.SetRegistWaitingFlag2(GUIDBASE64s, UserID_textBox.Text, ref errList, objectConvNew: object_ConvNew_CheckBox.Checked, logWindowControl.WriteLine);

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
                    }
                }
            }

        }


        /// <summary>
        /// ■③最終承認の対象を検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindFinalCanApproval_button_Click(object sender, EventArgs e)
        {
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.ApprovableFinulList(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "最終承認可能", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);
        }

        /// <summary>
        /// ■④ArcSuite未登録を検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_ARCSUITEID_IsNULL_fromDB_Button_Click_1(object sender, EventArgs e)
        {
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.ArcSuiteNotRegisteredList(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "ArcSuite未登録", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);
        }

        /// <summary>
        /// ■⑤アークスイート登録予定を検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REGISTWAITTINGFLAGbutton_Click(object sender, EventArgs e)
        {
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.ArcSuiteRegistrationScheduled(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "アークスイート登録指示あり", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);
        }

        /// <summary>
        /// ■⑥無条件ですべて検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindAll_button_Click(object sender, EventArgs e)
        {
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.LISTALL(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "無条件（全てのデータ）", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);
        }

        /// <summary>
        /// ■⑦承認者名で検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REGISTUSERsearch_button_Click(object sender, EventArgs e)
        {
            var name = REGISTUSERNAME_textBox.Text;
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.ApprovalUser(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "承認ユーザー名で検索", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);

        }

        /// <summary>
        /// ■⑧コミットユーザー名で検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitUserSearch_button_Click(object sender, EventArgs e)
        {
            var name = COMMITUSERNAME_textBox.Text;
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.CommitUser(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "コミットユーザー名で検索", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);
        }

        /// <summary>
        /// ■⑧ペーパーサイズ名で検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaperSizeSearchButton_Click(object sender, EventArgs e)
        {
            var name = PaperSizeComboBox.Text;
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.PaperSize(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "ペーパーサイズ名で検索", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);

        }

        private void COMMITHOST_search_button_Click(object sender, EventArgs e)
        {
            var name = COMMITHOST_textBox.Text;
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.CommitHost(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "コミットホスト名で検索", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text, objectConvNew: object_ConvNew_CheckBox.Checked);

        }


        private void DebugListBoxItemsClearButton_Click(object sender, EventArgs e)
        {
            sdhelper.clear();
            ResultSearchPattern_label.Text = "検索待ち";
        }

        private void SortButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未実装");

        }

        private void SelectsDelete_button_Click(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("削除よろしいか？") == DialogResult.OK)
            {
                if (DebugListView2.SelectedItems.Count == 1)
                {
                    sdhelper.DeleteSelectedDBresultListRecordOne();
                }
                else
                {
                    if (MessageBox.Show($"{DebugListView2.SelectedItems.Count} 件 全件削除よろしいか？") == DialogResult.OK)
                    {
                        if (DebugListView2.SelectedItems.Count > 1)
                        {
                            sdhelper.Delete_SelectedDBresultListRecords();
                        }
                    }
                }
            }
        }

        private void SelectedRecord_ApprovedAllReset_Button_Click(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("承認情報を全てリセットします。よろしいか？") == DialogResult.OK)
            {
                if (DebugListView2.SelectedItems.Count == 1)
                {
                    sdhelper.ApprovdReset_SelectedDBresultListRecordOne(objectConvNew: object_ConvNew_CheckBox.Checked, logWindowControl.WriteLine);
                    sdhelper.GetAndViewDRAWINGimage(Properties.Resources.イメージ読込中);
                }
                else
                {
                    if (MessageBox.Show($"{DebugListView2.SelectedItems.Count} 件 全件承認情報削除よろしいか？") == DialogResult.OK)
                    {
                        if (DebugListView2.SelectedItems.Count > 1)
                        {
                            sdhelper.ApprovdReset_SelectedDBresultListRecords();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedRecord_FinalApprovedReset_Button_Click(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("最終承認情報のみをリセットします。よろしいか？") == DialogResult.OK)
            {
                if (DebugListView2.SelectedItems.Count == 1)
                {
                    sdhelper.FinalApprovdReset_SelectedDBresultListRecordOne(objectConvNew: object_ConvNew_CheckBox.Checked, logWindowControl.WriteLine);

                    sdhelper.GetAndViewDRAWINGimage(Properties.Resources.イメージ読込中);
                }
                else
                {
                    MessageBox.Show($"複数の最終承認情報のみﾘｾｯﾄには非対応");
                }
            }

        }

        private void drawpictureBox_Click(object sender, EventArgs e)
        {

        }

        private void InputCommitUesrTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void logWindowControl1_Load(object sender, EventArgs e)
        {

        }


        private string GetSQLWhereSyntax(List<SqlSearchStringValue> searchKeyValues)
        {


            string CommandText = "WHERE ";
            foreach (var searchKeyValue in searchKeyValues)
            {
                var Field = searchKeyValue.Field;
                var Ooperator = searchKeyValue.Ooperator;
                var Value = searchKeyValue.Value;
                var Logic = searchKeyValue.Logic;


                var keylist = new List<string>() { "<", ">", "=", "<=", ">=", "(", ")", "AND", "OR", "NOT" };
                if (string.IsNullOrWhiteSpace(Logic) == false && keylist.Contains(Logic.ToUpper()) == false)
                    throw new Exception($"SqlSearchStringValueオブジェクトの組立過程にて Logic キーワードが想定外です \"{Logic}\"");

                if ((string.IsNullOrWhiteSpace(Field) == false) && (string.IsNullOrWhiteSpace(Ooperator) == false) && (string.IsNullOrWhiteSpace(Value) == false))
                    CommandText += $"( {Field} {Ooperator} {Value} ) {Logic} ";
                else if ((string.IsNullOrWhiteSpace(Field) == true) && (string.IsNullOrWhiteSpace(Ooperator) == true) && (string.IsNullOrWhiteSpace(Value) == true) && (string.IsNullOrWhiteSpace(Logic) == false))
                    CommandText += $"{Logic} ";
                else
                    throw new Exception($"SqlSearchStringValueオブジェクトの組立過程にて想定外の組合せ。Field: \"{Field}\" Ooperator: \"{Ooperator}\" Value: \"{Value}\" Logic* \"{Logic}\"");
            }


            return CommandText;
        }

        private void TICKETCODE_SEARCH_button_Click(object sender, EventArgs e)
        {
            ;

            var name = TICKETCODE_textBox.Text;
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.TICKETCODE(name);
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "TICKETCODEで検索", objectConvNew: object_ConvNew_CheckBox.Checked);

        }

        private void PARTNUMBER_SEARCH_button_Click(object sender, EventArgs e)
        {


            var name = PARTNUMBER_textbox.Text;
            sdhelper.clear();

            var sqlSearchStringValues = SqlSyntax.PARTNUMBER(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sdhelper.Search(sqlSearchStringValues, "TICKETCODEで検索", objectConvNew: object_ConvNew_CheckBox.Checked);

        }

        private void PreviewFormShow_button_Click(object sender, EventArgs e)
        {
            previewArcSuiteForm.Show();
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void DebugListView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //並び替える（ListViewItemSorterを設定するとSortが自動的に呼び出される）
            //ListViewItemSorterを指定する
            DebugListView2.ListViewItemSorter = new ListViewItemComparer(e.Column);
        }

        private void ClearListView_button_Click(object sender, EventArgs e)
        {
            sdhelper.clear();
        }

        int printerSelIndex1;

        private void PrinterSelcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            printerSelIndex1 = PrinterSelcomboBox.SelectedIndex;
        }

        private void PrinterSelcomboBox_DropDown(object sender, EventArgs e)
        {
            logWindowControl.Clear();
            mainForm.CommitPrinters.GetData(objectConvNew: true, logWindowControl.WriteLine);

            mainForm.CommitPrinters.SetComboBox(ref PrinterSelcomboBox);

        }

        private void DirectPrintButton_Click(object sender, EventArgs e)
        {
            string printername = PrinterSelcomboBox.SelectedText;

            RemoteClientMaintenance rMmaintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

            var GUIDBASE64 = sdhelper.GetSelectedData("GUIDBASE64");



            rMmaintenance.PlotouDrawingGUIDBASE64(GUIDBASE64, PrinterSelcomboBox.Text);

        }
    }
}
