using SasaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerControlCenterApplication
{
    /// <summary>
    /// 
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class AccountUserForm : UserControl
    {
        public System.Windows.Forms.Control parentControl { get; set; }

        // カスタムイベントの定義
        public event EventHandler AccountChanged;

        public AccountUserForm()
        {
            InitializeComponent();

        }

        public void SetToControls()
        {
            Encryption sasaLibencryption = new Encryption(SccConfig.Config.SasaLibEncryptionType);
            ClientImpersonationCheckBox.Checked = SccConfig.Config.ClsLogon;     // 1
            LogonDomainTextBox.Text = SccConfig.Config.ClientDomainName;         // 2
            LogonUserTextBox.Text = SccConfig.Config.ClientUserName;            // 3

            SccConfig.Config.ClientUserPassword = sasaLibencryption.Decoding(SccConfig.Config.ClientUserCryptUserPass);
            LogonPasswordTextBox.Text = SccConfig.Config.ClientUserPassword;    // 4

            StageServerHostName_comboBox.Text = SccConfig.Config.StageServerHost;        // 5
            DrawcapturePIPEnameTextBox.Text = SccConfig.Config.PipeNameDC;       // 6
            DrawregistPIPEnameTextBox.Text = SccConfig.Config.PipeNameDR;        // 7
            DrawWatchPIPEnameTextBox.Text = SccConfig.Config.PipeNameSW;   // 7.5

            CommitPathTextBox.Text = @"\\" + StageServerHostName_comboBox.Text + @"\" + CommitShareNameTextBox.Text;               // 8
        }

        public void ControlChanged(object sender, EventArgs e)
        {
            Encryption sasaLibencryption = new Encryption(SccConfig.Config.SasaLibEncryptionType);

            SccConfig.Config.ClsLogon = ClientImpersonationCheckBox.Checked;// 1
            SccConfig.Config.ClientDomainName = LogonDomainTextBox.Text;    // 2
            SccConfig.Config.ClientUserName = LogonUserTextBox.Text;        // 3

            SccConfig.Config.ClientUserPassword = LogonPasswordTextBox.Text;// 4
            SccConfig.Config.ClientUserCryptUserPass = sasaLibencryption.Encoding(SccConfig.Config.ClientUserPassword);

            SccConfig.Config.StageServerHost = StageServerHostName_comboBox.Text;    // 5
            SccConfig.Config.PipeNameDR = DrawregistPIPEnameTextBox.Text;   // 6
            SccConfig.Config.PipeNameDC = DrawcapturePIPEnameTextBox.Text;  // 7
            SccConfig.Config.PipeNameSW = DrawWatchPIPEnameTextBox.Text;   // 7.5

            SccConfig.Config.CommitPath = @"\\" + StageServerHostName_comboBox.Text + @"\" + CommitShareNameTextBox.Text;
            SccConfig.Config.Save();

        }

        public void StageServerHostName_comboBox_SetText(string  text)
        {
            StageServerHostName_comboBox.Text = text;
        }

        private void StageServerHostName_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SccConfig.Config.StageServerHost = StageServerHostName_comboBox.Text;    // 5
            if (parentControl != null)
                parentControl.Refresh();

            // カスタムイベントを発生
            OnAccountChanged(EventArgs.Empty);
        }

        protected virtual void OnAccountChanged(EventArgs e)
        {
            // カスタムイベントを発生させる
            AccountChanged?.Invoke(this, e);
        }

        private void ClientImpersonationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // カスタムイベントを発生
          //  OnAccountChanged(EventArgs.Empty);

        }

        private void LogonDomainTextBox_TextChanged(object sender, EventArgs e)
        {
            // カスタムイベントを発生
            OnAccountChanged(EventArgs.Empty);

        }

        private void LogonUserTextBox_TextChanged(object sender, EventArgs e)
        {
            // カスタムイベントを発生
            OnAccountChanged(EventArgs.Empty);
        }

        private void LogonPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            // カスタムイベントを発生
            OnAccountChanged(EventArgs.Empty);

        }

        public void saveAccount()
        {
            SccConfig.Config.ClsLogon = ClientImpersonationCheckBox.Checked;// 1

            SccConfig.Config.ClientDomainName = LogonDomainTextBox.Text;    // 2
            SccConfig.Config.ClientUserName = LogonUserTextBox.Text;        // 3
            SccConfig.Config.ClientUserPassword = LogonPasswordTextBox.Text;// 4

            Encryption sasaLibencryption = new Encryption(SccConfig.Config.SasaLibEncryptionType);
            SccConfig.Config.ClientUserCryptUserPass = sasaLibencryption.Encoding(SccConfig.Config.ClientUserPassword);

        }

        private void LogonDomainTextBox_Leave(object sender, EventArgs e)
        {
            saveAccount();
        }

        private void LogonUserTextBox_Leave(object sender, EventArgs e)
        {
            saveAccount();

        }

        private void LogonPasswordTextBox_Leave(object sender, EventArgs e)
        {
            saveAccount();
        }

        private void ClientImpersonationCheckBox_Leave(object sender, EventArgs e)
        {
            saveAccount();

        }

    }
}
