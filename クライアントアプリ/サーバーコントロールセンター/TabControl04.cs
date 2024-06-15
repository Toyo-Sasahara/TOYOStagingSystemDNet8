using SasaLib;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToyoMcMfg.Staging.DataBaseConfig;
using SasaLib.PIPE;
using ToyoStageService;
using RemoteClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ServerControlCenterApplication
{
    public partial class TabControl04 : UserControl
    {
        Form1 mainForm;
        CommitPrinters CommitPrinters = new CommitPrinters();


        readonly RemoteClientMemoryMapdFile mmapdFile = new RemoteClientMemoryMapdFile(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost);


        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="form"></param>
        public TabControl04(Form1 form)
        {
            this.mainForm = form;
            InitializeComponent();

            accountUserForm.parentControl = this;

            // カスタムイベントのハンドラを追加
            accountUserForm.AccountChanged += accountUserForm_AccountChanged;


            BackupFolderPathTextBox.Text = SccConfig.Config.BackupFolderPath;
            if (SccConfig.Config.BackupFolderPath != null)
                BackupDistServer_comboBox.Text = GetHostnameFromUNC(SccConfig.Config.BackupFolderPath);

            RestoreSourceFolder_textBox.Text = SccConfig.Config.RestoreFolderPath;
            if (SccConfig.Config.RestoreFolderPath != null)
                RestoreTargetSerer_comboBox.Text = GetHostnameFromUNC(SccConfig.Config.RestoreFolderPath);


        }


        private void TabControl04_Load(object sender, EventArgs e)
        {
            logWindowControl.WriteLine($"TabControl04.load(..)実行開始");
        }

        /// <summary>
        /// ■このタブコントロールの表示状態が変わったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl04_VisibleChanged(object sender, EventArgs e)
        {
            logWindowControl.WriteLine($"TabControl04_VisibleChanged(..)実行開始");
            Task.Run(() =>
            {
                System.Windows.Forms.MethodInvoker method = () =>
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


        private async void accountUserForm_AccountChanged(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("■accountUserForm_AccountChanged(..)開始・・・");

            string DRAWREGISTservice_DebugLevel = null;
            string DRAWCAPTUREservice_DebugLevel = null;
            string STAGINGSYSTEMwatch_DebugLevel = null;

            RestoreTargetServer_label.Text = SccConfig.Config.StageServerHost;

            await Task.Run(() =>
            {
                logWindowControl.WriteLine("■accountUserForm_AccountChanged(..)Task.Run実行中・・・");
                System.Windows.Forms.MethodInvoker method = () =>
                {
                    try
                    {

                        accountUserForm.SetToControls();

                        mainForm.CommitPrinters.GetData(logWindowControl.WriteLine);

                        ReadMMPFAndSetInTheFormContorols();
                        DRAWREGISTservice_DebugLevel = Command_ServerControl.SetOrGet_DRAWREGISTserviceDEBUGLevel(int.Parse(DRAWREGISTservice_DebugLevelComboBox.Text), false, logWindowControl.WriteLine).ToString();
                        DRAWCAPTUREservice_DebugLevel = Command_ServerControl.SetOrGet_DRAWCAPTUREserviceDEBUGLevel(int.Parse(DRAWCAPTUREservice_DebugLevelComboBox.Text), false, logWindowControl.WriteLine).ToString();
                        STAGINGSYSTEMwatch_DebugLevel = Command_ServerControl.SetOrGet_STAGINGSYSTEMwatchDEBUGLevel(int.Parse(STAGINGSYSTEMwatch_DebugLevelComboBox.Text), false, logWindowControl.WriteLine).ToString();

                        logWindowControl.WriteLine("■コントロールへの割り当て開始・・・");
                        DRAWREGISTservice_DebugLevelComboBox.Text = DRAWREGISTservice_DebugLevel;
                        DRAWCAPTUREservice_DebugLevelComboBox.Text = DRAWCAPTUREservice_DebugLevel;
                        STAGINGSYSTEMwatch_DebugLevelComboBox.Text = STAGINGSYSTEMwatch_DebugLevel;
                        logWindowControl.WriteLine("■コントロールへの割り当て完了");

                        logWindowControl.WriteLine("■accountUserForm_AccountChanged(..)Task.Run実行完了");
                    }
                    catch (Exception ex) { logWindowControl.WriteLine($"※accountUserForm_AccountChanged(..)例外 {ex.Message}"); }

                }; if (InvokeRequired) { Invoke(method); } else { method(); }
            });


            //if (mainForm.CommitPrinters.resultGetCommitPrinterShortCutName != null && mainForm.CommitPrinters.resultGetCommitPrinterShortCutName.Count > 0)
            //{
            //    PrinterSel_comboBox.DataSource = mainForm.CommitPrinters.resultGetCommitPrinterShortCutName;
            //    PrinterSel_comboBox.DisplayMember = "key";
            //    PrinterSel_comboBox.ValueMember = "value";
            //}

        }


        private void accountUserForm5_Paint(object sender, PaintEventArgs e)
        {

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackupDistServer_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BackupFolderPathTextBox.Text = System.IO.Path.Combine(@"\\" + BackupDistServer_comboBox.Text, "BACKUPFOLDER$");
            SccConfig.Config.BackupFolderPath = BackupFolderPathTextBox.Text;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestoreTargetSerer_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RestoreSourceFolder_textBox.Text = System.IO.Path.Combine(@"\\" + RestoreTargetSerer_comboBox.Text, "BACKUPFOLDER$");
            SccConfig.Config.RestoreFolderPath = RestoreSourceFolder_textBox.Text;
        }

        /// <summary>
        /// ■コントロールの内容を読み出し該当するメモリマップドファイル(MMPF)にセットする。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool ReadFormControlsAndSetInTheMMPF()
        {
            if (MessageBox.Show(caption: "最終確認", text: $"接続先は {accountUserForm.StageServerHostName_comboBox.Text} です。\r\n" +
                $"正しいですか？", buttons: MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return true;
            }
            mmapdFile.StageServerHost = accountUserForm.StageServerHostName_comboBox.Text;

            if (SERVERMODE_Master_RadioButton.Checked)
            {
                mmapdFile.SERVERMODE = RemoteClientMemoryMapdFile.ServerMode.Master;
            }
            else if (SERVERMODE_Slave_RadioButton.Checked)
            {
                mmapdFile.SERVERMODE = RemoteClientMemoryMapdFile.ServerMode.Slave;
            }
            mmapdFile.ArcSuiteRegistrationCycle = ArcSuiteRegistrationCycle_CheckBox.Checked;

            mmapdFile.COMMITACCEPT = COMMITACCEPT_CheckBox.Checked;

            mmapdFile.COMMITACCEPTFALSEMSG = COMMITACCEPTFALSEMSG_TextBox.Text;

            mmapdFile.APPROVINGACCEPT = APPROVINGACCEPT_CheckBox.Checked;

            mmapdFile.APPROVINGACCEPTFALSEMSG = APPROVINGACCEPTFALSEMSG_TextBox.Text;

            mmapdFile.ImmediateryPrinting = ImmediateryPrinting_CheckBox.Checked;

            mmapdFile.TESTMODE = TESTMODE_CheckBox.Checked;

            mmapdFile.RemoteServerCommand_DATABASE_EventView_Send = RemoteServRemoteServerCommand_DATABASE_EventView_Send_checkBox.Checked;

            bool ans = mmapdFile.ReadPropertiesAndSetInTheMMPF(logWindowControl.WriteLine);

            return ans;
        }

        private bool IsArcSuiteRegistrationScheduled()
        {
            var sqlSearchStringValues = SqlSyntax.ArcSuiteRegistrationScheduled(365);
            var CommandText = SQLSearchConditions.Create(sqlSearchStringValues);


            RemoteClientDataBase rMCdataBaseTestmd = new RemoteClientDataBase(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

            int topcount = 9999;
            string ORDDERBY = "ORDER BY ID DESC";

            List<FieldValueSet> anser = null;
            anser = rMCdataBaseTestmd.DataBaseSearch4a(sqlSearchStringValues, topcount, ORDDERBY);

            if (anser != null && anser.Count > 1)
            {
                return true;
            }
            else

            {
                return false;
            }
        }

        /// <summary>
        /// ■データベースのバックアップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DATABASEbackupButton_Click(object sender, EventArgs e)
        {
            Command_ServerControl.Backup(BackupFolderPathTextBox.Text, 1, logWindowControl.WriteLine);

        }

        private void RestoreStart_button_Click(object sender, EventArgs e)
        {
            DateTime createDateTime = DataBaseBackupCreateTime_dateTimePicker.Value;
            Command_ServerControl.Restore(RestoreSourceFolder_textBox.Text, createDateTime, logWindowControl.WriteLine);

        }


        /// <summary>
        /// ■ToyoDRAWREGISTserviceのログ表示レベルを変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DRAWREGISTservice_DebugLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Command_ServerControl.SetOrGet_DRAWREGISTserviceDEBUGLevel(int.Parse(DRAWREGISTservice_DebugLevelComboBox.Text), true, logWindowControl.WriteLine);
        }

        /// <summary>
        /// ■ToyoDRAWCAPTUREserviceのログ表示レベルを変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DRAWCAPTUREservice_DebugLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Command_ServerControl.SetOrGet_DRAWCAPTUREserviceDEBUGLevel(int.Parse(DRAWCAPTUREservice_DebugLevelComboBox.Text), true, logWindowControl.WriteLine);
        }

        private void STAGINGSYSTEMwatch_DebugLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Command_ServerControl.SetOrGet_STAGINGSYSTEMwatchDEBUGLevel(int.Parse(STAGINGSYSTEMwatch_DebugLevelComboBox.Text), true, logWindowControl.WriteLine);
        }

        /// <summary>
        /// ■メモリマップドファイルへ書き込む
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetMMapdButton_Click(object sender, EventArgs e)
        {
            ReadFormControlsAndSetInTheMMPF();
        }

        /// <summary>
        /// ●メモリマップドファイルを読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetMMapdButton_Click(object sender, EventArgs e)
        {
            ReadMMPFAndSetInTheFormContorols();
        }

        private void SERVERMODE_MasterAndSlave_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = IsArcSuiteRegistrationScheduled();

            if (enabled)
                MessageBox.Show("DB上にアークスイート登録待ち図面が存在します。このままMasterに切り替えると意図しない二重登録が発生します！");
        }

        /// <summary>
        /// ●
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectCheckButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未実装");

        }

        /// <summary>
        /// ●
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetCOMMITFolderButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未実装");

        }

        private void ARCSUITESENDPATHbutton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未実装");

        }

        /// <summary>
        /// ●
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetArcSuiteDMSHostNameButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未実装");

        }

        /// <summary>
        /// ●
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CHECKPIPECONNECTIONbutton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未実装");

        }

        private void GetTextFileTestButton_Click(object sender, EventArgs e)
        {
            RemoteClientDRAWCAPTURE remoteClientDC = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDC);
            var ans = remoteClientDC.GetTextFileFromPIPE(LoadTextFileSource_textBox.Text, WriteTextFileDist_textbox.Text, logWindowControl.WriteLine);
            if (ans)
                MessageBox.Show($"読み出し成功");
            else
                MessageBox.Show($"読み出し失敗");

        }

        /// <summary>
        /// ●FILESTORE内のファイルをすべて検索し、データベースにリンクされていないファイルの個数を調査
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileStoreCheckButton_Click(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("FILESTORE不要ファイル検索中");

            Task.Run(() =>
            {
                /// <summary>
                /// メンテナンス用ユーティリティーを初期化
                /// </summary>
                RemoteClientMaintenance maintenace = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);
                var ans = maintenace.FILESTORECehck(logWindowControl.WriteLine);

                logWindowControl.WriteLine($"FILESTORE不要ファイル総数は {ans}件");
            });

        }

        /// <summary>
        /// ●ファイルへのリンク切れを起こしているチケットコードを検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FINDfilelinkdowmnButton1_Click(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("リンク切れを起こしているチケットコードを検索開始");

            List<FieldValueSet> linkdownans;
            Task.Run(() =>
            {
                /// <summary>
                /// メンテナンス用ユーティリティーを初期化
                /// </summary>
                RemoteClientMaintenance maintenace = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

                linkdownans = maintenace.TICKETFILEexistCHeck();
                foreach (var fieldValuseSet in linkdownans)
                {
                    logWindowControl.WriteLine($"リン先ファイルが無いチケットコード：{fieldValuseSet.SearchKey("TICKETCODE")}");
                }

                logWindowControl.WriteLine($"リンク切れを起こしているチケットコードを検索完了 {linkdownans.Count} 件");

            });

        }

        /// <summary>
        /// ●
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckMasterOrSlave_button_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ●
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ●FILESTORE内のファイルをすべて検索し、データベースにリンクされていないファイルをゴミとして削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FILESTORERepareButton_Click(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("FILESTORE不要ファイル清掃開始");
            Task.Run(() =>
            {
                /// <summary>
                /// メンテナンス用ユーティリティーを初期化
                /// </summary>
                RemoteClientMaintenance maintenace = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);
                maintenace.FILESTORERepare();
                logWindowControl.WriteLine("FILESTORE不要ファイル清掃終了");
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteTextFileDist_textbox_TextChanged(object sender, EventArgs e)
        {
            WriteTextFileDist_textbox.Text = WriteTextFileDist_textbox.Text.TrimStart('\"').TrimEnd('\"');
        }
        /// <summary>
        /// テキストファイルゲットテスト
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private void LoadTextFileSource_textBox_TextChanged(object sender, EventArgs e)
        {
            LoadTextFileSource_textBox.Text = LoadTextFileSource_textBox.Text.TrimStart('\"').TrimEnd('\"');
        }

        /// <summary>
        /// ●暗号化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartCryptButton_Click(object sender, EventArgs e)
        {
            Encoded_textBox.Text = "";
            Decoded_TextBox.Text = "";

            var input = PlaneInputTextBox.Text;

            SasaLib.Encryption encryption = new Encryption(AuthPatern_comboBox.Text);

            var encoded = encryption.Encoding(input);

            Encoded_textBox.Text = encoded;

            var decoded = encryption.Decoding(Encoded_textBox.Text);

            Decoded_TextBox.Text = decoded;

        }

        /// <summary>
        /// ●復号化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartDeCryptButton_Click(object sender, EventArgs e)
        {
            Encoded_textBox.Text = "";
            Decoded_TextBox.Text = "";

            var input = PlaneInputTextBox.Text;

            SasaLib.Encryption encryption = new Encryption(AuthPatern_comboBox.Text);

            var decoded = encryption.Decoding(input);

            Decoded_TextBox.Text = decoded;

        }

        /// <summary>
        /// ●チケットテンプレートの作成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateTicketTemplate_button_Click(object sender, EventArgs e)
        {
            // チケットコードのテンプレートファイル名を生成
            string template_Ticket_File = SccConfigWork.GetAppConfigFolder() + System.IO.Path.DirectorySeparatorChar + @"TEMPLATE_TICKET.XML";

            //チケットテンプレートの作成
            CommonTicketWork.MakeTicketTemplate(template_Ticket_File);

        }



        /// <summary>
        /// ■メモリマップドファイル(MMPF)の内容を読み出し該当するフォームコントロールにセットする
        /// </summary>
        internal void ReadMMPFAndSetInTheFormContorols()
        {
            mmapdFile.StageServerHost = accountUserForm.StageServerHostName_comboBox.Text;


            /// 処理１
            // メモリマップドファイルをプロパティに読込する
            var ans = mmapdFile.ReadMMPFAndSetInTheProperties(logWindowControl.WriteLine);


            if (ans)
            {
                MMapdValue_panel.Enabled = true;

                switch (mmapdFile.SERVERMODE)
                {
                    case RemoteClientMemoryMapdFile.ServerMode.Master:
                        SERVERMODE_Master_RadioButton.Checked = true;
                        SERVERMODE_Slave_RadioButton.Checked = false;
                        break;
                    case RemoteClientMemoryMapdFile.ServerMode.Slave:
                        SERVERMODE_Slave_RadioButton.Checked = true;
                        SERVERMODE_Master_RadioButton.Checked = false;
                        break;
                }

                ArcSuiteRegistrationCycle_CheckBox.Checked = mmapdFile.ArcSuiteRegistrationCycle;
                COMMITACCEPT_CheckBox.Checked = mmapdFile.COMMITACCEPT;
                COMMITACCEPTFALSEMSG_TextBox.Text = mmapdFile.COMMITACCEPTFALSEMSG;
                APPROVINGACCEPT_CheckBox.Checked = mmapdFile.APPROVINGACCEPT;
                APPROVINGACCEPTFALSEMSG_TextBox.Text = mmapdFile.APPROVINGACCEPTFALSEMSG;
                ImmediateryPrinting_CheckBox.Checked = mmapdFile.ImmediateryPrinting;
                TESTMODE_CheckBox.Checked = mmapdFile.TESTMODE;
                RemoteServRemoteServerCommand_DATABASE_EventView_Send_checkBox.Checked = mmapdFile.RemoteServerCommand_DATABASE_EventView_Send;

                return;
            }
            else
            {
                MMapdValue_panel.Enabled = false;
                return;
            }

        }


        /// <summary>
        /// UNCからhostnameを取り出し
        /// </summary>
        /// <param name="unc"></param>
        /// <returns></returns>
        string GetHostnameFromUNC(string unc)
        {
            var uriPath = new System.Uri(unc);
            var hostname = uriPath.Host;
            return hostname;
        }





        private void RestoreSourceFolder_textBox_TextChanged(object sender, EventArgs e)
        {

        }


        private void RestoreTargetServer_label_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Enabled = checkBox1.Checked;
        }

        private void Decoded_TextBox_TextChanged(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetNumberOfJob_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH remoteClientSW = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameSW);
            var ans = remoteClientSW.ShowPrinterQueue(SccConfig.Config.StageServerHost, PrinterSel_comboBox.Text, logWindowControl.WriteLine);

            logWindowControl.WriteLine(ans);

        }

        private void SetOrGet_DRAWREGISTserviceDEBUGLevelButton_Click(object sender, EventArgs e)
        {
            Command_ServerControl.SetOrGet_DRAWREGISTserviceDEBUGLevel(int.Parse(DRAWREGISTservice_DebugLevelComboBox.Text), true, logWindowControl.WriteLine);
            Command_ServerControl.SetOrGet_DRAWCAPTUREserviceDEBUGLevel(int.Parse(DRAWCAPTUREservice_DebugLevelComboBox.Text), true, logWindowControl.WriteLine);
            Command_ServerControl.SetOrGet_STAGINGSYSTEMwatchDEBUGLevel(int.Parse(STAGINGSYSTEMwatch_DebugLevelComboBox.Text), true, logWindowControl.WriteLine);
        }

    }
}
