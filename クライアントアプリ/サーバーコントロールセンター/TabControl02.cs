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
//using System.Runtime.Remoting.Metadata;
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


        SearchDrawingSetPictureBox sh2;

        internal PreviewImageForm previewArcSuiteForm = new PreviewImageForm();

        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="form"></param>
        public TabControl02(Form1 form)
        {
            this.mainForm = form;
            InitializeComponent();

            sh2 = new SearchDrawingSetPictureBox(DebugListView2, ResultSearchPattern_label, previewArcSuiteForm, logWindowControl1.WriteLine);

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
            logWindowControl1.WriteLine("TabControl02_VisibleChanged(..)実行・・・\r\n");

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
            sh2.GetAndViewDRAWINGimage(Properties.Resources.イメージ読込中);
            if (previewArcSuiteForm.Visible == false)
                previewArcSuiteForm.Show(this);
        }

        private void DebugListView2_Click(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            var TICKETCODE = sh2.GetSelectedData("TICKETCODE");
        }

        private void DebugListView2_DoubleClick(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            var SANITIZEDPARTNUMBER = sh2.GetSelectedData("SANITIZEDPARTNUMBER");

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
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.ArcSuiteRegisteredList(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");
            sh2.Search(sqlSearchStringValues, "ArcSuite登録済み", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_ARCSUITEIDisNull_And_APPROVEDUSERisNotNull_Button_Click(object sender, EventArgs e)
        {
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.ARCSUITEIDisNull_And_APPROVEDUSERisNotNull(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");
            sh2.Search(sqlSearchStringValues, "承認済みだがArcSuite未登録かつArcSuite登録指示フラグが無い", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_APPROVEDDATEisNotNull_And_REGISTEDTIMEisZero_And_REGISTWAITINGFLAGisZero_Button_Click(object sender, EventArgs e)
        {
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.APPROVEDDATEisNotNull_And_REGISTEDTIMEisZero_And_REGISTWAITINGFLAGisZero(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");
            sh2.Search(sqlSearchStringValues, "承認済みだがArcSuite未登録かつArcSuite登録指示フラグが無い", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);
        }


        /// <summary>
        /// ■設計承認の対象を検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindeCanApprovalButton_Click(object sender, EventArgs e)
        {
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.ApprovableList(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "設計承認が可能", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);
        }

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

                            logWindowControl1.WriteLine($"押印強制実行");
                            Command_MAINCOMMAND.ApprovedMainProcessDebug(TICKETCODE, UserID_textBox.Text, Approved2cResult, logWindowControl1.WriteLine);
                        }
                    }
                }
            }

        }

        private void Approved2cResult(Object sender, ApprovedStatus resultAnser)
        {
            logWindowControl1.WriteLine($"イベントがキックされた ApprovedStatus.ApprovedSucess = {resultAnser.ApprovedSucess}");

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

                            logWindowControl1.WriteLine($"強制通常登録実行");

                            //登録可能のフラグを立てる
                            RemoteClientDRAWREGIST rmarcSuite = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                                SccConfig.Config.ClientUserName,
                                SccConfig.Config.ClientUserPassword,
                                SccConfig.Config.ClsLogon,
                                SccConfig.Config.StageServerHost,
                                SccConfig.Config.PipeNameDR);

                            var GUIDBASE64 = GUIDExtensions.GetB64StringFromB64FnameString(TICKETCODE);

                            List<string> GUIDBASE64s = new List<string>() { GUIDBASE64 };

                            rmarcSuite.SetRegistWaitingFlag(GUIDBASE64s, UserID_textBox.Text);

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
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.ApprovableFinulList(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "最終承認可能", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);
        }

        /// <summary>
        /// ■④ArcSuite未登録を検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_ARCSUITEID_IsNULL_fromDB_Button_Click_1(object sender, EventArgs e)
        {
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.ArcSuiteNotRegisteredList(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "ArcSuite未登録", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);
        }

        /// <summary>
        /// ■⑤アークスイート登録予定を検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REGISTWAITTINGFLAGbutton_Click(object sender, EventArgs e)
        {
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.ArcSuiteRegistrationScheduled(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "アークスイート登録指示あり", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);
        }

        /// <summary>
        /// ■⑥無条件ですべて検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindAll_button_Click(object sender, EventArgs e)
        {
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.LISTALL(int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "無条件（全てのデータ）", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);
        }

        /// <summary>
        /// ■⑦承認者名で検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REGISTUSERsearch_button_Click(object sender, EventArgs e)
        {
            var name = REGISTUSERNAME_textBox.Text;
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.ApprovalUser(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "承認ユーザー名で検索", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);

        }

        /// <summary>
        /// ■⑧コミットユーザー名で検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitUserSearch_button_Click(object sender, EventArgs e)
        {
            var name = COMMITUSERNAME_textBox.Text;
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.CommitUser(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "コミットユーザー名で検索", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);
        }

        /// <summary>
        /// ■⑧ペーパーサイズ名で検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaperSizeSearchButton_Click(object sender, EventArgs e)
        {
            var name = PaperSizeComboBox.Text;
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.PaperSize(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "ペーパーサイズ名で検索", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);

        }

        private void COMMITHOST_search_button_Click(object sender, EventArgs e)
        {
            var name = COMMITHOST_textBox.Text;
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.CommitHost(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "コミットホスト名で検索", int.Parse(MAXSEARCHtextBox2.Text), ORDERBY_comboBox.Text);

        }


        private void DebugListBoxItemsClearButton_Click(object sender, EventArgs e)
        {
            sh2.clear();
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
                    sh2.DeleteSelectedDBresultListRecordOne();
                }
                else
                {
                    if (MessageBox.Show($"{DebugListView2.SelectedItems.Count} 件 全件削除よろしいか？") == DialogResult.OK)
                    {
                        if (DebugListView2.SelectedItems.Count > 1)
                        {
                            sh2.Delete_SelectedDBresultListRecords();
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
                    sh2.ApprovdReset_SelectedDBresultListRecordOne();
                    sh2.GetAndViewDRAWINGimage(Properties.Resources.イメージ読込中);
                }
                else
                {
                    if (MessageBox.Show($"{DebugListView2.SelectedItems.Count} 件 全件承認情報削除よろしいか？") == DialogResult.OK)
                    {
                        if (DebugListView2.SelectedItems.Count > 1)
                        {
                            sh2.ApprovdReset_SelectedDBresultListRecords();
                        }
                    }
                }
            }
        }


        private void SelectedRecord_FinalApprovedReset_Button_Click(object sender, EventArgs e)
        {
            if (DebugListView2.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("最終承認情報のみをリセットします。よろしいか？") == DialogResult.OK)
            {
                if (DebugListView2.SelectedItems.Count == 1)
                {
                    sh2.FinalApprovdReset_SelectedDBresultListRecordOne();

                    sh2.GetAndViewDRAWINGimage(Properties.Resources.イメージ読込中);
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
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.TICKETCODE(name);
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "TICKETCODEで検索");

        }

        private void PARTNUMBER_SEARCH_button_Click(object sender, EventArgs e)
        {


            var name = PARTNUMBER_textbox.Text;
            sh2.clear();

            var sqlSearchStringValues = SqlSyntax.PARTNUMBER(name, int.Parse(TimeSpanDateTextBox.Text));
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);

            logWindowControl1.WriteLine($"WHERE句が生成されました ： \"{CommandText}\"");

            sh2.Search(sqlSearchStringValues, "TICKETCODEで検索");

        }

        private void PreviewFormShow_button_Click(object sender, EventArgs e)
        {
            previewArcSuiteForm.Show();
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }
}
