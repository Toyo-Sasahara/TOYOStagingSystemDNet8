using SasaLib;
using SasaLib.PIPE;
using STAGINGSYSTEM_COMMANDS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Text;
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
    public partial class AccountUserForm : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public System.Windows.Forms.Control parentControl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<string> WriteLine { get; set; } = DebugConsole.WriteLine;

        /// <summary>
        /// カスタムイベントの定義
        /// </summary>
        public event EventHandler AccountChanged;

        /// <summary>
        /// カスタムイベントの定義
        /// </summary>
        public event EventHandler HostChanged;

        /// <summary>
        /// 
        /// </summary>
        public AccountUserForm()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnHostChange(EventArgs e)
        {
            //WriteLine("OnHostChange(..)実行・・・");
            // カスタムイベントを発生させる
            HostChanged?.Invoke(this, e);
        }

        protected virtual void OnAccountChanged(EventArgs e)
        {
            // カスタムイベントを発生させる
            AccountChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetToControls()
        {
            //Encryption sasaLibencryption = new Encryption(SccConfig.Config.SasaLibEncryptionType);
            ClientImpersonationCheckBox.Checked = SccConfig.Config.ClsLogon;     // 1
            LogonDomainTextBox.Text = SccConfig.Config.ClientDomainName;         // 2
            LogonUserTextBox.Text = SccConfig.Config.ClientUserName;            // 3

            SccConfig.Config.ClientUserPassword = Encryption.Decode(SccConfig.Config.SasaLibEncryptionType, SccConfig.Config.ClientUserCryptUserPass);
            LogonPasswordTextBox.Text = SccConfig.Config.ClientUserPassword;    // 4

            StageServerHostName_comboBox.Text = SccConfig.Config.StageServerHost;        // 5
            DrawcapturePIPEnameTextBox.Text = SccConfig.Config.PipeNameDC;       // 6
            DrawregistPIPEnameTextBox.Text = SccConfig.Config.PipeNameDR;        // 7
            DrawWatchPIPEnameTextBox.Text = SccConfig.Config.PipeNameSW;   // 7.5

            CommitPathTextBox.Text = SccConfig.Config.CommitPath;               // 8

            NewStreamMode_checkBox.Checked = SccConfig.Config.NewStreamMode;
            NewStreamModeTpPort_textBox.Text = SccConfig.Config.NewStreamMode_DC_TcpPort.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ControlChanged(object sender, EventArgs e)
        {
            //Encryption sasaLibencryption = new Encryption(SccConfig.Config.SasaLibEncryptionType);

            SccConfig.Config.ClsLogon = ClientImpersonationCheckBox.Checked;// 1
            SccConfig.Config.ClientDomainName = LogonDomainTextBox.Text;    // 2
            SccConfig.Config.ClientUserName = LogonUserTextBox.Text;        // 3

            SccConfig.Config.ClientUserPassword = LogonPasswordTextBox.Text;// 4
            SccConfig.Config.ClientUserCryptUserPass = Encryption.Decode(SccConfig.Config.SasaLibEncryptionType, SccConfig.Config.ClientUserPassword);

            SccConfig.Config.StageServerHost = StageServerHostName_comboBox.Text;    // 5
            SccConfig.Config.PipeNameDR = DrawregistPIPEnameTextBox.Text;   // 6
            SccConfig.Config.PipeNameDC = DrawcapturePIPEnameTextBox.Text;  // 7
            SccConfig.Config.PipeNameSW = DrawWatchPIPEnameTextBox.Text;   // 7.5

            SccConfig.Config.CommitPath = @"\\" + StageServerHostName_comboBox.Text + @"\" + CommitShareNameTextBox.Text;

            SccConfig.Config.NewStreamMode = NewStreamMode_checkBox.Checked;
            SccConfig.Config.NewStreamMode_DC_TcpPort = StringToInt(NewStreamModeTpPort_textBox.Text);

            SccConfig.Config.Save();

        }

        private int StringToInt(string text)
        {
            if (int.TryParse(text, out int number))
            {
                return number;
            }
            else
            {
                return 28080;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void StageServerHostName_comboBox_SetText(string text)
        {
            StageServerHostName_comboBox.Text = text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StageServerHostName_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SccConfig.Config.StageServerHost = StageServerHostName_comboBox.Text;    // 5

            if (parentControl != null)
                parentControl.Refresh();

            // カスタムイベントを発生
            OnHostChange(EventArgs.Empty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DR_ConnectTest_button_Click(object sender, EventArgs e)
        {
            bool result = await Command_Status.DR_ConnectTestAsync($"{PIPETESTMSG_textBox.Text}", 1000, WriteLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DC_ConnectTest_button_Click(object sender, EventArgs e)
        {
            bool result = await Command_Status.DC_ConnectTestAsync($"{PIPETESTMSG_textBox.Text}", 1000, WriteLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SW_ConnectTest_button_Click(object sender, EventArgs e)
        {
            bool result = await Command_Status.SW_ConnectTestAsync($"{PIPETESTMSG_textBox.Text}", 1000, WriteLine);
        }

        private void LogonDomainTextBox_Leave(object sender, EventArgs e)
        {
            SccConfig.Config.ClientDomainName = LogonDomainTextBox.Text;    // 2
            // カスタムイベントを発生
            OnAccountChanged(EventArgs.Empty);
        }

        private void LogonUserTextBox_Leave(object sender, EventArgs e)
        {
            SccConfig.Config.ClientUserName = LogonUserTextBox.Text;        // 3
            // カスタムイベントを発生
            OnAccountChanged(EventArgs.Empty);

        }

        private void LogonPasswordTextBox_Leave(object sender, EventArgs e)
        {
            SccConfig.Config.ClientUserPassword = LogonPasswordTextBox.Text;// 4

            //Encryption sasaLibencryption = new Encryption(SccConfig.Config.SasaLibEncryptionType);
            SccConfig.Config.ClientUserCryptUserPass = Encryption.Encode(SccConfig.Config.SasaLibEncryptionType, SccConfig.Config.ClientUserPassword);
            // カスタムイベントを発生
            OnAccountChanged(EventArgs.Empty);
        }

        private void ClientImpersonationCheckBox_Leave(object sender, EventArgs e)
        {
            SccConfig.Config.ClsLogon = ClientImpersonationCheckBox.Checked;// 1

            // カスタムイベントを発生
            OnAccountChanged(EventArgs.Empty);

        }

        private void DC_Shudown_button_Click(object sender, EventArgs e)
        {
            var result = serviceShutdown(SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDC);

            if (result != null)
            {
                foreach (var a in result)
                {
                    var x = $"接続中のユーザー {a.ClientHost} {a.ClientUser} {a.CommandName} {a.toyoUSERID} {a.toyoFULLNAME}{a.Command_Accept_DateTime}";
                    WriteLine(x);
                }
            }
        }

        private void DR_Shudown_button_Click(object sender, EventArgs e)
        {
            var result = serviceShutdown(SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

            if (result != null)
            {
                foreach (var a in result)
                {
                    var x = $"接続中のユーザー {a.ClientHost} {a.ClientUser} {a.CommandName} {a.toyoUSERID} {a.toyoFULLNAME}{a.Command_Accept_DateTime}";
                    WriteLine(x);
                }
            }
        }

        private void SW_Shudown_button_Click(object sender, EventArgs e)
        {
            var result = serviceShutdown(SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameSW);

            if (result != null)
            {
                foreach (var a in result)
                {
                    var x = $"接続中のユーザー {a.ClientHost} {a.ClientUser} {a.CommandName} {a.toyoUSERID} {a.toyoFULLNAME}{a.Command_Accept_DateTime}";
                    WriteLine(x);
                }
            }
        }

        private List<AcceptCommand> serviceShutdown(string PipeServerName, string pipename)
        {
            // TODO: ClsLogonDummy を 書き換えた
            WithFakeAccount withFakeAccount = new WithFakeAccount(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, _fakeExecute);

            List<AcceptCommand> fakedExecuteresult = (List<AcceptCommand>)withFakeAccount.GetResult();

            return fakedExecuteresult;

            object _fakeExecute()
            {
                try
                {
                    object writeResult;
                    List<AcceptCommand> acceptPIpeCommmands = null;

                    NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(PipeServerName, pipename);
                    // 待機中のサーバーへ接続
                    try
                    {
                        pipeCltStream.Connect(1000);
                    }
                    catch (Exception ex)
                    {

                        WriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterInfo(..)にて例外発生 {ex.Message}");
                        return null;
                    }
                    // サーバーからのサーバ識別文字列を受け取ります。
                    StreamString stst = new StreamString(pipeCltStream);
                    bool OperationCanceledException;
                    bool AggregateException;

                    string input0 = stst.ReadString(Command_ServerControl.ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                    if (OperationCanceledException || AggregateException)
                    {
                        WriteLine($"RemoteClientDRAWCAPTURE.GetCommitPrinterInfo(..) 最初のハンドシェイクにてタイムアウトが発生");
                        return null;
                    }
                    if (Command_ServerControl.CheckFirstMessage(input0))
                    {
                        writeResult = stst.WriteString(CMDS.DC_DR_SW_ServerControl_CMDGROUP);
                        writeResult = stst.WriteString(CMDS.DC_DR_SW_ServiceStop);

                        // クライアントから送られてきた 検索結果で出力するカラム名のListを取得す
                        using (var reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                        {
                            acceptPIpeCommmands = BinaryReaderExtensions.ReadObject<List<AcceptCommand>>(reader, binaryConvertType: BinaryConvertTYPE.JsonSerializer);
                        }

                        if (acceptPIpeCommmands != null && acceptPIpeCommmands.Count > 0)
                        {
                            WriteLine($"接続中の承認クライアントが{acceptPIpeCommmands.Count} 件あります.サービスシャットダウン要請は却下されました");

                            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                            {
                                writer.WriteObject(true, binaryConvertType: BinaryConvertTYPE.JsonSerializer);
                            }

                        }
                        else
                        {
                            WriteLine($"接続中の承認クライアントは有りません。終了させます");

                            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                            {
                                writer.WriteObject(true, binaryConvertType: BinaryConvertTYPE.JsonSerializer);
                            }

                        }


                        pipeCltStream.Close();
                    }
                    else
                    {
                        WriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterInfo(..)　PIPEサーバーへのからの接続文字列{input0}が期待と違います");
                        pipeCltStream.Close();
                        return null;
                    }
                    // Give the client process some time to display results before exiting.

                    return acceptPIpeCommmands;
                }
                catch (Exception ex)
                {

                    WriteLine($"※RemoteClientDRAWCAPTURE.GetCommitPrinterInfo(..)　例外発生{ex.Message}");
                    SasaLib.Eventlog.Log.WriteEntry("TOYODATABASE", EventLogEntryType.Error, 6000, $"RemoteClientDRAWCAPTURE.GetCommitPrinterInfo(..)　例外発生 {ex.Message}");
                }
                return null;
            }

        }

        private void NewStreamMode_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            SccConfig.Config.NewStreamMode = NewStreamMode_checkBox.Checked;
        }

        private void NewStreamModeTpPort_textBox_TextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;

            if (tb.Text.Length == 0) return;

            // ① そのまま int にできる？
            if (int.TryParse(tb.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var val)
                && val >= 0 && val <= 65535)
            {
                // ←← ここで確定反映
                SccConfig.Config.NewStreamMode_DC_TcpPort = val;
                return;
            }

            // ② 無効入力：前回に戻す or 補正
            tb.TextChanged -= NewStreamModeTpPort_textBox_TextChanged;

            string before = tb.Text;
            tb.Undo();

            if (tb.Text == before)
            {
                string cleaned = SanitizeToInt(before);

                if (int.TryParse(cleaned, NumberStyles.Integer, CultureInfo.InvariantCulture, out var fixedVal)
                    && fixedVal >= 0 && fixedVal <= 65535)
                {
                    tb.Text = fixedVal.ToString(CultureInfo.InvariantCulture);

                    // ←← 補正後もここで確定反映
                    SccConfig.Config.NewStreamMode_DC_TcpPort = fixedVal;
                }
                else
                {
                    tb.Text = string.Empty;
                }
            }

            tb.SelectionStart = tb.Text.Length;
            tb.SelectionLength = 0;
            tb.TextChanged += NewStreamModeTpPort_textBox_TextChanged;
            System.Media.SystemSounds.Beep?.Play();

            // ローカル関数：先頭の '-' を許可しつつ、数字以外を除去（ポートなら '-' は最終的に弾かれる）
            string SanitizeToInt(string s)
            {
                if (string.IsNullOrEmpty(s)) return string.Empty;
                var sb = new System.Text.StringBuilder(s.Length);
                int i = 0;
                if (s[0] == '-') { sb.Append('-'); i = 1; }
                for (; i < s.Length; i++)
                    if (char.IsDigit(s[i])) sb.Append(s[i]);
                return sb.ToString();
            }

        }
    }
}
