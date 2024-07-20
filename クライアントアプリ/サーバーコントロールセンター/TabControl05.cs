using RemoteClient;
using SasaLib;
using SasaLib.PIPE;
using StageServerRemote;
using STAGINGSYSTEM_COMMANDS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToyoStageService;
#if NETCOREAPP
using System.Runtime.Versioning;
#endif

namespace ServerControlCenterApplication
{
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class TabControl05 : UserControl
    {
        int count;

        Form1 mainForm;

        //static readonly string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        /// <summary>
        /// ■このタブコントロールの表示状態が変わったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public TabControl05(Form1 form)
        {
            this.mainForm = form;
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl05_Load(object sender, EventArgs e)
        {
            accountUserForm.WriteLine = logWindowControl.WriteLine;
        }

        /// <summary>
        /// ■このタブコントロールの表示状態が変わったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl05_VisibleChanged(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                MethodInvoker method = () =>
                {
                    accountUserForm.SetToControls();
                    logWindowControl.WriteLine("TabControl05_VisibleChanged(..)実行・・・\r\n");

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


        /// <summary>
        /// DrawRegistServer ﾓｰﾄﾞ確認
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckDRServerModeButton_Click(object sender, EventArgs e)
        {
            CheckDRServerAnserTextBox.Text = "---";

            LogWindowWriteLine("DRAWREGISTサービスにコマンド \"STATUS\" > \"\"");
            var ans = Command_Status.DR_ServerMode(LogWindowWriteLine);

            CheckDRServerAnserTextBox.Text = ans;
        }

        /// <summary>
        /// DrawCaptureServer ﾓｰﾄﾞ確認
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckDCServerModeButton_Click(object sender, EventArgs e)
        {
            CheckDCServerAnserTextBox.Text = "---";

            var ans = Command_Status.DC_ServerMode(LogWindowWriteLine);

            CheckDCServerAnserTextBox.Text = ans;
        }

        /// <summary>
        /// 
        /// </summary>
        bool AutoDRCurrentModCcheckBox_loopflag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoDRCurrentModCcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoDRCurrentModCcheckBox.Checked == true)
            {
                if (AutoDRCurrentModCcheckBox_loopflag == false)
                {
                    AutoDRCurrentModCcheckBox_loopflag = true;

                    ConectionCheckTaskAsync(2);
                }
            }
            else
            {
                AutoDRCurrentModCcheckBox_loopflag = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sec"></param>
        private async void ConectionCheckTaskAsync(int sec)
        {
            await Task.Factory.StartNew(() =>
            {
                while (AutoDRCurrentModCcheckBox_loopflag)
                {
                    Invoke(new Action(() =>
                    {
                        LogWindowWriteLine($"接続監視チェック");

                        CheckDRServerAnserTextBox.Text = "---";

                        CheckDRServerAnserTextBox.Text = Command_Status.DR_ServerMode(LogWindowWriteLine);

                        CheckDCServerAnserTextBox.Text = "---";

                        CheckDCServerAnserTextBox.Text = Command_Status.DC_ServerMode(LogWindowWriteLine);

                    }));

                    Task.Delay(sec * 1000).Wait();
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DRGetFalseMSGButton_Click(object sender, EventArgs e)
        {
            textBox2.Text = "---";

            var ans = "未実装";

            textBox2.Text = ans;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DCGetFalseMSGButton_Click(object sender, EventArgs e)
        {
            textBox1.Text = "---";

            var ans = "未実装";

            textBox1.Text = ans;

        }

        /// <summary>
        /// 
        /// </summary>
        bool AutoGetFalseMSG_CcheckBox_loopflag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoGetFalseMSG_CcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoGetFalseMSG_CcheckBox.Checked == true)
            {
                if (AutoGetFalseMSG_CcheckBox_loopflag == false)
                {
                    AutoGetFalseMSG_CcheckBox_loopflag = true;

                    ConectionChec2kTaskAsync(2);
                }
            }
            else
            {
                AutoDRCurrentModCcheckBox_loopflag = false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sec"></param>
        private async void ConectionChec2kTaskAsync(int sec)
        {
            await Task.Factory.StartNew(() =>
            {
                while (AutoDRCurrentModCcheckBox_loopflag)
                {
                    Invoke(new Action(() =>
                    {
                        LogWindowWriteLine($"False MSGチェック");

                        CheckDRServerAnserTextBox.Text = "---";

                        CheckDRServerAnserTextBox.Text = Command_Status.DR_ServerMode(LogWindowWriteLine);

                        CheckDCServerAnserTextBox.Text = "---";

                        CheckDCServerAnserTextBox.Text = Command_Status.DC_ServerMode(LogWindowWriteLine);

                    }));

                    Task.Delay(sec * 1000).Wait();
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetDRAWREGISTserviceVersionButton_Click(object sender, EventArgs e)
        {
            string version = Command_ServerControl.GetDRAWREGISTserviceVersion(LogWindowWriteLine);
            LogWindowWriteLine($"");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetDRAWCAPTUREserviceVersioButton_Click(object sender, EventArgs e)
        {
            string version = Command_ServerControl.GetDRAWCAPTUREserviceVersion(LogWindowWriteLine);
            LogWindowWriteLine($"");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetDRAWWATCHEserviceVersioButton_Click(object sender, EventArgs e)
        {
            string version = Command_ServerControl.GetSYSTEMWATCHserviceVersion(LogWindowWriteLine);
            LogWindowWriteLine($"");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleFieldTest_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH rmc_SYSTEMWATCH = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameSW);

            var mydocumentfolder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var ans = rmc_SYSTEMWATCH.TitleFieldTest(@"C:\TOYOSVC\titlefiledtest.tif", System.IO.Path.Combine(mydocumentfolder, "titlefiledtest.tif"), SasaLib.PrintConfig.CommonPaperSize.A4P, WriteLine: LogWindowWriteLine);

            LogWindowWriteLine($"{ans}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckCommitRecepitonStateButton_Click(object sender, EventArgs e)
        {
            /// コミット受付可能かをサーバーに問い合わせる。
            string Message;
            await Task.Run(() =>
            {
                MethodInvoker method = async () =>
                {
                    do
                    {
                        if (Command_MAINCOMMAND.CheckCommitRecepitonState(out Message, LogWindowWriteLine) == false)
                        {
                            //if (RecepitonStateLoadTest_CheckBox.Checked == false)
                            //    MessageBox.Show($"{Message}");
                        }

                        await Task.Delay(Convert.ToInt32(delyaTime_textBox.Text));
                    }
                    while (RecepitonStateLoadTest_CheckBox.Checked);
                }; if (InvokeRequired) { Invoke(method); } else { method(); }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckApprovalRecepitonState_Click(object sender, EventArgs e)
        {
            /// 承認受付可能かをサーバーに問い合わせる。
            string Message;

            await Task.Run(() =>
            {
                MethodInvoker method = async () =>
                {
                    do
                    {
                        if (Command_MAINCOMMAND.CheckApprovalRecepitonState(out Message, LogWindowWriteLine) == false)
                        {
                            LogWindowWriteLine($"※問合せ結果 \"{Message}\"");
                        }
                        else
                        {
                            LogWindowWriteLine($"※問合せ失敗。戻り値 false");
                        }
                        await Task.Delay(Convert.ToInt32(delyaTime_textBox.Text));

                        SasaLib.DoEvents.Run();
                    }
                    while (RecepitonStateLoadTest_CheckBox.Checked);
                }; if (InvokeRequired) { Invoke(method); } else { method(); }
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DR_ReloadConfig_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDR);

            rmc_ServerControl.RELOAD_STAGESERVERCONFIG(SccConfig.Config.StageServerHost, LogWindowWriteLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DC_ReloadConfig_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDC);

            rmc_ServerControl.RELOAD_STAGESERVERCONFIG(SccConfig.Config.StageServerHost, LogWindowWriteLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DW_RELOAD_STAGESERVERCONFIG_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameSW);

            rmc_ServerControl.RELOAD_STAGESERVERCONFIG(SccConfig.Config.StageServerHost, LogWindowWriteLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DC_SAVE_STAGESERVERCONFIG_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDC);

            rmc_ServerControl.SAVE_STAGESERVERCONFIG(SccConfig.Config.StageServerHost, LogWindowWriteLine);
        }

        private void DR_SAVE_STAGESERVERCONFIG_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            rmc_ServerControl.SAVE_STAGESERVERCONFIG(SccConfig.Config.StageServerHost, logWindowControl.WriteLine);

        }

        private void DW_SAVE_STAGESERVERCONFIG_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameSW);

            rmc_ServerControl.SAVE_STAGESERVERCONFIG(SccConfig.Config.StageServerHost, logWindowControl.WriteLine);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DC_SAVE_STAGESERVERDATABASECONFIG_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDC);

            rmc_ServerControl.SAVE_STAGESERVERDATABASECONFIG(SccConfig.Config.StageServerHost, LogWindowWriteLine);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DC_ReloadStampConf_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDC);

            rmc_ServerControl.RELOAD_STAMPCONF(SccConfig.Config.StageServerHost, LogWindowWriteLine);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DR_ReloadStampConf_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDR);

            rmc_ServerControl.RELOAD_STAMPCONF(SccConfig.Config.StageServerHost, LogWindowWriteLine);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DC_ReloadBarcodeConf_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDC);

            rmc_ServerControl.RELOAD_BARCODECONF(SccConfig.Config.StageServerHost, LogWindowWriteLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DR_ReloadBarcodeConf_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDR);

            rmc_ServerControl.RELOAD_BARCODECONF(SccConfig.Config.StageServerHost, LogWindowWriteLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateText(string msg)
        {

            logWindowControl.WriteLine($"{count++}:{msg}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StageServerConfig_GetSet_button_Click(object sender, EventArgs e)
        {
            string host = SccConfig.Config.StageServerHost;


            await Task.Run(() =>
            {
                MethodInvoker method = () =>
                {
                    // コントロールに対する処理
                    var resultdc = Task_SetGetValue(host, SccConfig.Config.PipeNameDC, objectConvNew: objectConvNew_checkBox.Checked);
                    var resultdr = Task_SetGetValue(host, SccConfig.Config.PipeNameDR, objectConvNew: objectConvNew_checkBox.Checked);
                    var resultsw = Task_SetGetValue(host, SccConfig.Config.PipeNameSW, objectConvNew: objectConvNew_checkBox.Checked);
                };
                if (InvokeRequired) { Invoke(method); } else { method(); }
            });

            StageServerConfig_Value_comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// タスク
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="PIPENAME"></param>
        private bool Task_SetGetValue(string hostname, string PIPENAME, bool objectConvNew = false)
        {
            bool result2;
            if (string.IsNullOrWhiteSpace(StageServerConfig_Value_comboBox.Text) == false)
            {
                RemotePipeClient remote = new RemotePipeClient("", "", "", false, hostname, PIPENAME);
                if (objectConvNew)
                    result2 = remote.Command_ConnnectStart(CMDS.DC_DR_SW_SetValue_V2, _Method_SetCommitConfigValue_V2, logWindowControl.WriteLine);
                else
                    result2 = remote.Command_ConnnectStart(CMDS.DC_DR_SW_SetValue, _Method_SetCommitConfigValue, logWindowControl.WriteLine);
            }
            else
            {
                RemotePipeClient remote = new RemotePipeClient("", "", "", false, hostname, PIPENAME);
                if (objectConvNew)
                    result2 = remote.Command_ConnnectStart(CMDS.DC_DR_SW_GetValue_V2, _Method_GetCommitConfigValue_V2, WriteLine: logWindowControl.WriteLine);
                else
                    result2 = remote.Command_ConnnectStart(CMDS.DC_DR_SW_GetValue, _Method_GetCommitConfigValue, WriteLine: logWindowControl.WriteLine);

                if (result2)
                    StageServerConfig_Value_comboBox.Text = null;

            }

            return result2;

            //
            bool _Method_SetCommitConfigValue(NamedPipeClientStream pipeCltStream)
            {
                DebugConsole.WriteLine("SetCommitConfigValue(..) スタート");

                StreamString stst = new StreamString(pipeCltStream);

                string ServerResPon1 = stst.ReadString(10000, null);

                if (ServerResPon1 != null)
                {

                    DebugConsole.WriteLine($"ServerResPon1 = {ServerResPon1}");
                    string command = StageServerConfig_VarbleName_comboBox.Text;
                    stst.WriteString(command);

                    System.Type receveFieldType;
                    using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                    {
                        if (objectConvNew)
                            receveFieldType = reader.ReadObject<System.Type>(binaryConvertType: BinaryConvertTYPE.JsonSerializer);
                        else
                            receveFieldType = reader.ReadObject<System.Type>();
                    }
                    Console.WriteLine($"hostname:{hostname} , receveFieldType = {receveFieldType}");

                    StageServerConfig_VarbleType_textbox.Text = receveFieldType.ToString();

                    object value = Convert.ChangeType(StageServerConfig_Value_comboBox.Text, receveFieldType);

                    using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                    {
                        if (objectConvNew)
                            writer.WriteObject(value, binaryConvertType: BinaryConvertTYPE.JsonSerializer);
                        else
                            writer.WriteObject(value);
                    }

                    return true;
                }
                else
                {
                    logWindowControl.WriteLine($"接続失敗 タイムアウト");
                    return false;
                }
            }

            //
            bool _Method_SetCommitConfigValue_V2(NamedPipeClientStream pipeCltStream)
            {
                DebugConsole.WriteLine("SetCommitConfigValue(..) スタート");

                StreamString stst = new StreamString(pipeCltStream);

                string ServerResPon1 = stst.ReadString(10000, null);

                if (ServerResPon1 != null)
                {

                    DebugConsole.WriteLine($"ServerResPon1 = {ServerResPon1}");
                    string command = StageServerConfig_VarbleName_comboBox.Text;
                    stst.WriteString(command);

                    ObjectWithType objectWithType;
                    using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                    {
                        objectWithType = reader.ReadObject<ObjectWithType>(binaryConvertType: BinaryConvertTYPE.JsonSerializer);
                        if (objectWithType != null)
                        {
                            StageServerConfig_VarbleType_textbox.Text = objectWithType.TypeName;
                            Console.WriteLine($"hostname:{hostname} , receveFieldType = {objectWithType}");

                            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                            {
                                Type targetType = Type.GetType(objectWithType.AssemblyQualifiedName);

                                object value = Convert.ChangeType(StageServerConfig_Value_comboBox.Text, targetType);

                                var options = new JsonSerializerOptions { Converters = { new ObjectWithTypeJsonConverter() }, WriteIndented = true };
                                ObjectWithType outPut_objectWithType = new ObjectWithType { TypeName = objectWithType.TypeName, Data = value }; // 指定したフィールドをカプセル化

                                writer.WriteObject(outPut_objectWithType, binaryConvertType: BinaryConvertTYPE.JsonSerializer, options: options);
                            }

                        }
                        else
                        {
                            logWindowControl.WriteLine($"戻り値objectConvNew がnull");
                        }
                    }



                    return true;
                }
                else
                {
                    logWindowControl.WriteLine($"接続失敗 タイムアウト");
                    return false;
                }
            }

            //
            bool _Method_GetCommitConfigValue(NamedPipeClientStream pipeCltStream)
            {
                logWindowControl.WriteLine("GetCommitConfigValue(..) スタート");

                StreamString stst = new StreamString(pipeCltStream);

                string ServerResPon1 = stst.ReadString(10000, null);

                if (ServerResPon1 != null)
                {
                    logWindowControl.WriteLine($"ServerResPon1 = {ServerResPon1}");

                    stst.WriteString(StageServerConfig_VarbleName_comboBox.Text); // 変数名送信

                    logWindowControl.WriteLine($"ターゲット:{hostname} 問い合わせる変数名 = 【{StageServerConfig_VarbleName_comboBox.Text}】");

                    object receveFieldType;
                    using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                    {
                        if (objectConvNew)
                            receveFieldType = reader.ReadObject<System.Type>(binaryConvertType: BinaryConvertTYPE.JsonSerializer); // 変数型情報受信
                        else
                            receveFieldType = reader.ReadObject<System.Type>(); // 変数型情報受信

                    }
                    logWindowControl.WriteLine($"ｻｰﾊﾞｰから型情報受信 = 【{receveFieldType}】");

                    object receveObj;
                    using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                    {
                        if (objectConvNew)
                            receveObj = reader.ReadObject<Object>(binaryConvertType: BinaryConvertTYPE.JsonSerializer); // ｻｰﾊﾞｰからオブジェクト受信
                        else
                            receveObj = reader.ReadObject<Object>(); // ｻｰﾊﾞｰからオブジェクト受信
                    }

                    object value = Convert.ChangeType(receveObj, receveFieldType as System.Type); // 受信オブジェクトを変換
                    StageServerConfig_VarbleType_textbox.Text = receveFieldType.ToString();

                    switch ((receveFieldType as System.Type).Name)
                    {
                        case "List`1":
                            logWindowControl.WriteLine($"サーバー:{hostname} 名前付きPIPE:{PIPENAME} 、公開変数　【{StageServerConfig_VarbleName_comboBox.Text}】　＝　【{receveFieldType}】");
                            logWindowControl.WriteLine($"個数:{(value as List<string>).Count}");
                            foreach (string a in value as List<string>)
                            {
                                logWindowControl.WriteLine($"内容={a}");
                            }
                            break;
                        default:
                            logWindowControl.WriteLine($"サーバー:{hostname} 名前付きPIPE:{PIPENAME} 、公開変数　【{StageServerConfig_VarbleName_comboBox.Text}】　＝　 【{receveFieldType}】型：{value}");
                            break;
                    }
                    return true;
                }
                else
                {
                    logWindowControl.WriteLine($"接続失敗 タイムアウト");
                    return false;
                }
            }

            //
            bool _Method_GetCommitConfigValue_V2(NamedPipeClientStream pipeCltStream)
            {
                logWindowControl.WriteLine("GetCommitConfigValue(..) スタート");

                StreamString stst = new StreamString(pipeCltStream);

                string ServerResPon1 = stst.ReadString(10000, null);

                if (ServerResPon1 != null)
                {
                    logWindowControl.WriteLine($"ServerResPon1 = {ServerResPon1}");

                    stst.WriteString(StageServerConfig_VarbleName_comboBox.Text); // 変数名送信

                    logWindowControl.WriteLine($"ターゲット:{hostname} 問い合わせる変数名 = 【{StageServerConfig_VarbleName_comboBox.Text}】");


                    ObjectWithType receveObj;
                    using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                    {
                        var options = new JsonSerializerOptions { Converters = { new ObjectWithTypeJsonConverter() }, WriteIndented = true };
                        receveObj = reader.ReadObject<ObjectWithType>(binaryConvertType: BinaryConvertTYPE.JsonSerializer, options: options); // ｻｰﾊﾞｰからオブジェクト受信
                        if (receveObj != null)
                        {
                            StageServerConfig_VarbleType_textbox.Text = receveObj.TypeName;
                            logWindowControl.WriteLine($"サーバー:{hostname} 名前付きPIPE:{PIPENAME} 、公開変数【{StageServerConfig_VarbleName_comboBox.Text}】＝【{receveObj.Data}】 型：{receveObj.TypeName}");
                        }
                        else
                            logWindowControl.WriteLine($"戻り値がnull");
                    }

                    return true;
                }
                else
                {
                    logWindowControl.WriteLine($"接続失敗 タイムアウト");
                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClientPreInputTICKETCODE_button_Click(object sender, EventArgs e)
        {
            PresentTicketCode presentTicketCode = new PresentTicketCode();
            string TICKETCODE = presentTicketCode.CreateTICKETCODE();
            preInputTikectCode_textBox.Text = TICKETCODE;
            string DRAWNNUMBER = DRAWNUMBER_textBox.Text;
            string toyoUSERID = "0135";
            string toyoFULLNAME = "ささはら";
            ClientPreInputTICKET inputTICKET = new ClientPreInputTICKET()
            {
                ClientHost = Net.DnsGetHostNameOrIP(Environment.MachineName),
                ClientUser = Environment.UserName,
                DRAWNUMBER = DRAWNNUMBER,
                PreInputTICKETCODE_DateTime = DateTime.Now,
                PreInputTICKETCODE = TICKETCODE,
                toyoFULLNAME = toyoFULLNAME,
                toyoUSERID = toyoUSERID
            };

            Task.Run(async () =>
            {
                do
                {
                    List<ClientPreInputTICKET> preInputeTicket = Command_ServerControl.AddClientPreInputTICKETCODE(SccConfig.Config.PipeNameDR, inputTICKET, objectConvNew: objcetConvNew_checkBox2.Checked, WriteLine: logWindowControl.WriteLine);

                    if (preInputeTicket != null)
                    {
                        logWindowControl.WriteLine($"個数 {preInputeTicket.Count}");

                        foreach (var a in preInputeTicket)
                        {
                            var x = $"Command_ServerControl.ClientPreInputTICKETCODE {a.PreInputTICKETCODE} {a.DRAWNUMBER} {a.toyoFULLNAME} {a.ClientHost} {a.ClientUser}{a.PreInputTICKETCODE_DateTime}";
                            logWindowControl.WriteLine(x);
                        }

                    }
                    else
                    {
                        logWindowControl.WriteLine("nullが返された");

                    }

                    await Task.Delay(200);
                } while (GetAuthorizedUser_LoopCheck_checkBox.Checked);
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemovePreInputTIKECTCODEs_button_Click(object sender, EventArgs e)
        {
            string ticketcode = preInputTikectCode_textBox.Text;
            List<string> ticketcodes = new List<string>() { ticketcode };

            PresentTicketCode presentTicketCode = new PresentTicketCode();
            string TICKETCODE = presentTicketCode.CreateTICKETCODE();
            string DRAWNNUMBER = DRAWNUMBER_textBox.Text;


            List<string> removeSucessPreInputTicketcodes = Command_ServerControl.RemovePreInputTIKECTCODEs(SccConfig.Config.PipeNameDR, ticketcodes, objectConvNew: objcetConvNew_checkBox2.Checked, WriteLine: logWindowControl.WriteLine);
            if (removeSucessPreInputTicketcodes != null)
            {
                if (ticketcodes.Count == removeSucessPreInputTicketcodes.Count)
                {
                    logWindowControl.WriteLine($"削除成功 {removeSucessPreInputTicketcodes.Count}件");
                }
                else
                {
                    logWindowControl.WriteLine("削除失敗");

                }

            }
            else
            {

                logWindowControl.WriteLine("削除失敗");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetCommonApprovalWaitingTicketList_button_Click(object sender, EventArgs e)
        {
            string ticketcode = preInputTikectCode_textBox.Text;
            List<string> ticketcodes = new List<string>() { ticketcode };

            PresentTicketCode presentTicketCode = new PresentTicketCode();
            string TICKETCODE = presentTicketCode.CreateTICKETCODE();
            string DRAWNNUMBER = DRAWNUMBER_textBox.Text;


            List<ClientPreInputTICKET> CommonApprovalWaitingTicketList = Command_ServerControl.GetCommonApprovalWaitingTicketList(SccConfig.Config.PipeNameDR, objectConvNew: objcetConvNew_checkBox2.Checked, logWindowControl.WriteLine);
            if (CommonApprovalWaitingTicketList != null)
            {
                if (CommonApprovalWaitingTicketList.Count > 0)
                {
                    logWindowControl.WriteLine($"全 {CommonApprovalWaitingTicketList.Count}件");
                    foreach (var a in CommonApprovalWaitingTicketList)
                    {
                        logWindowControl.WriteLine($"【{a.PreInputTICKETCODE}】【{a.DRAWNUMBER}】 [{a.toyoFULLNAME}]  [{a.PreInputTICKETCODE_DateTime}] [{a.ClientHost}] [{a.ClientUser}]");
                    }
                }
                else
                {
                    logWindowControl.WriteLine("なし");

                }

            }
            else
            {

                logWindowControl.WriteLine("エラー");
            }

        }

        /// <summary>
        /// ■現在の接続しているクライアントを表す構造体を取得し、個数を出力する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetAuthorizedUser_button_Click(object sender, EventArgs e)
        {

            Task.Run(async () =>
            {
                do
                {
                    List<AcceptPipeCommand> connectingAuthorizedUser = Command_ServerControl.GetAuthorizedUser(SccConfig.Config.PipeNameDR, objectConvNew: false, WriteLine: LogWindowWriteLine);

                    if (connectingAuthorizedUser != null)
                    {
                        logWindowControl.WriteLine($"個数 {connectingAuthorizedUser.Count}");

                        foreach (var a in connectingAuthorizedUser)
                        {
                            var x = $"Command_ServerControl.GetAuthorizedUser {a.ClientHost} {a.ClientUser} {a.CommandName} {a.toyoUSERID} {a.toyoFULLNAME}{a.Command_Accept_DateTime}";
                            logWindowControl.WriteLine(x);
                        }
                    }
                    else
                    {
                        logWindowControl.WriteLine("nullが返された");
                    }

                    await Task.Delay(200);
                } while (GetAuthorizedUser_LoopCheck_checkBox.Checked);
            });
        }

        /// <summary>
        /// ■DC PIPEサーバーが保持している クライアントの接続記録を文字列で出力する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DC_GetPipeCommandLog_button_Click(object sender, EventArgs e)
        {
            string logmsg = Command_ServerControl.GetPipeCommandLog(
                SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDC, LogWindowWriteLine);
            logWindowControl.WriteLine(logmsg);

        }

        /// <summary>
        /// ■DR PIPEサーバーが保持している クライアントの接続記録を文字列で出力する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DR_GetPipeCommandLog_button_Click(object sender, EventArgs e)
        {
            string logmsg = Command_ServerControl.GetPipeCommandLog(
                SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR, LogWindowWriteLine);

            logWindowControl.WriteLine(logmsg);
        }

        /// <summary>
        /// ■DW PIPEサーバーが保持している クライアントの接続記録を文字列で出力する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DW_GetPipeCommandLog_button_Click(object sender, EventArgs e)
        {
            string logmsg = Command_ServerControl.GetPipeCommandLog(
                SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameSW, LogWindowWriteLine);

            logWindowControl.WriteLine(logmsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListAllValue_button_Click(object sender, EventArgs e)
        {
            bool objectConvNew = objectConvNew_checkBox.Checked;

            string hostname = SccConfig.Config.StageServerHost;
            string PIPENAME = null;

            if (DRAWCAPTUREservice_radioButton.Checked)
                PIPENAME = "CaptureService";
            else if (DRAWREGISTservice_radioButton.Checked)
                PIPENAME = "ApprovalServer";
            else if (STAGINGSYSTEMwatch_radiobutton.Checked)
                PIPENAME = "WatchService";


            RemotePipeClient remote = new RemotePipeClient("", "", "", false, hostname, PIPENAME);
            bool result2;

            if (objectConvNew)
                result2 = remote.Command_ConnnectStart(CMDS.DC_DR_SW_GetVauleList_V2, _Method_ListAllCommitConfigValue_V2, logWindowControl.WriteLine);
            else
                result2 = remote.Command_ConnnectStart(CMDS.DC_DR_SW_GetVauleList, _Method_ListAllCommitConfigValue, logWindowControl.WriteLine);

            if (result2)
                StageServerConfig_Value_comboBox.Text = null;


            bool _Method_ListAllCommitConfigValue(NamedPipeClientStream pipeCltStream)
            {
                DebugConsole.WriteLine("_Method_ListAllCommitConfigValue(..) スタート");

                object receveObj;
                using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                {
                    receveObj = reader.ReadObject<Object>(); // ｻｰﾊﾞｰからオブジェクト受信
                }

                var aa = receveObj as List<string>;

                foreach (var a in aa)
                {
                    logWindowControl.WriteLine(a);
                }
                return true;
            }

            bool _Method_ListAllCommitConfigValue_V2(NamedPipeClientStream pipeCltStream)
            {
                DebugConsole.WriteLine("_Method_ListAllCommitConfigValue(..) スタート");


                List<string> receveObj;
                using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                {
                    //var options = new JsonSerializerOptions { Converters = { new ObjectWithTypeJsonConverter() }, WriteIndented = true };

                    receveObj = reader.ReadObject<List<string>>(binaryConvertType: BinaryConvertTYPE.JsonSerializer); // ｻｰﾊﾞｰからオブジェクト受信
                }

                foreach (var a in receveObj)
                {
                    logWindowControl.WriteLine($"{a}");
                }
                return true;
            }
        }

        private async void DC_StressTest_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmClientDC = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDC);


            var ans = await Task.Run(() =>
            {
                string outMsg;

                var result = rmClientDC.StressTest(5000, out outMsg, waitMin_textBox.Text, logWindowControl.WriteLine);

                if (result)
                    return outMsg;
                else
                    return null;
            });

            if (ans == null)
            {
                logWindowControl.WriteLine($"■DC_StressTest_button_Click() 負荷コマンド実行結果を受信 しましたが null でした");
            }
            else
            {
                logWindowControl.WriteLine($"■DC_StressTest_button_Click() 負荷コマンド実行結果を受信【{ans}】");

            }

        }

        /// <summary>
        /// ■DRAWREGISTサービスで 処理完了に時間のかかるコマンドを疑似実行する ストレステスト
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DR_StressTest_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);


            var ans = await Task.Run(() =>
            {
                string outMsg;

                var result = rmc_ServerControl.StressTest(5000, out outMsg, waitMin_textBox.Text, logWindowControl.WriteLine);

                if (result)
                    return outMsg;
                else
                    return null;
            });

            if (ans == null)
            {
                logWindowControl.WriteLine($"■DR_StressTest_button_Click() 負荷コマンド実行結果を受信 しましたが null でした");
            }
            else
            {
                logWindowControl.WriteLine($"■DR_StressTest_button_Click() 負荷コマンド実行結果を受信【{ans}】");

            }

        }

        /// <summary>
        /// ■DR パイプコマンド "GetPipeServerJobList" DRAWREGISTサービスが保持している List<AcceptPipeCommand> ActiveCommandJobList を得ます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DR_GetActiveSessionCommandList_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDR);


            List<AcceptPipeCommand> ans = await Task.Run(() =>
            {
                List<AcceptPipeCommand> connectClients;
                bool result = rmc_ServerControl.GetActiveSessionCommandList(out connectClients, objectConvNew: objectConvNew_heckBox.Checked);

                if (result)
                    return connectClients;
                else
                    return null;
            });

            if (ans != null)
            {
                logWindowControl.WriteLine($"■実行中全ジョブ一覧 {SccConfig.Config.StageServerHost} {SccConfig.Config.PipeNameDR}---------------------------------------------------------------------------------------");
                foreach (AcceptPipeCommand a in ans)
                {
                    logWindowControl.WriteLine($"■実行中全ジョブ数 {ans.Count},  ServerId : {a.ServerId} TaskID:{a.TaskID} , 実行日時:{a.Command_Accept_DateTime} ClientUser : \"{a.ClientUser}\" , CommandName : \"{a.CommandName}\"");
                }
                logWindowControl.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            }

        }

        private async void DC_GetActiveSessionCommandList_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameDC);


            List<AcceptPipeCommand> ans = await Task.Run(() =>
            {
                List<AcceptPipeCommand> connectClients;
                bool result = rmc_ServerControl.GetActiveSessionCommandList(out connectClients, objectConvNew: objectConvNew_heckBox.Checked);

                if (result)
                    return connectClients;
                else
                    return null;
            });

            if (ans != null)
            {
                logWindowControl.WriteLine($"■実行中全ジョブ一覧 {SccConfig.Config.StageServerHost} {SccConfig.Config.PipeNameDC}---------------------------------------------------------------------------------------");
                foreach (AcceptPipeCommand a in ans)
                {
                    logWindowControl.WriteLine($"■実行中全ジョブ数 {ans.Count},  ServerId : {a.ServerId} TaskID:{a.TaskID} , 実行日時:{a.Command_Accept_DateTime} ClientUser : \"{a.ClientUser}\" , CommandName : \"{a.CommandName}\"");
                }
                logWindowControl.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            }

        }

        private async void SW_GetActiveSessionCommandList_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
            SccConfig.Config.ClientUserName,
            SccConfig.Config.ClientUserPassword,
            SccConfig.Config.ClsLogon,
            SccConfig.Config.StageServerHost,
            SccConfig.Config.PipeNameSW);


            List<AcceptPipeCommand> ans = await Task.Run(() =>
            {
                List<AcceptPipeCommand> connectClients;
                bool result = rmc_ServerControl.GetActiveSessionCommandList(out connectClients, objectConvNew: objectConvNew_heckBox.Checked);

                if (result)
                    return connectClients;
                else
                    return null;
            });

            if (ans != null)
            {
                logWindowControl.WriteLine($"■実行中全ジョブ一覧 {SccConfig.Config.StageServerHost} {SccConfig.Config.PipeNameSW}---------------------------------------------------------------------------------------");
                foreach (AcceptPipeCommand a in ans)
                {
                    logWindowControl.WriteLine($"■実行中全ジョブ数 {ans.Count},  ServerId : {a.ServerId} TaskID:{a.TaskID} , 実行日時:{a.Command_Accept_DateTime} ClientUser : \"{a.ClientUser}\" , CommandName : \"{a.CommandName}\"");
                }
                logWindowControl.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            }

        }

        private void preInputTikectCode_textBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void logwindowClear_button_Click(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("----------------------------------------------------------------------------------------------------------------------------");

        }

        private async void GetCommitPath_button_Click(object sender, EventArgs e)
        {
            /// コミット先UNCパスをサーバーに問い合わせる。
            var a = await Task.Run(() =>
            {
                string Message = null;

                MethodInvoker method = () =>
                {
                    if (Command_MAINCOMMAND.GetCOMMITFolder(out Message, LogWindowWriteLine) == false)
                    {
                        LogWindowWriteLine($"");

                    }

                }; if (InvokeRequired) { Invoke(method); } else { method(); }

                return Message;
            });

            LogWindowWriteLine($"{a}");
        }

        private void accountUserForm_Load(object sender, EventArgs e)
        {

        }

        private void ArcSuiteTicketConfigSave_button_Click(object sender, EventArgs e)
        {
            RemoteClientServerControl rmc_ServerControl = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            rmc_ServerControl.SAVE_ARCSUITETICKETCONFIG(SccConfig.Config.StageServerHost, logWindowControl.WriteLine);

        }
    }
}

