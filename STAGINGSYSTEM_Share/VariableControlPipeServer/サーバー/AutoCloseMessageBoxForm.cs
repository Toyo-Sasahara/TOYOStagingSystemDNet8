using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Security.Permissions;
using SasaLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SasaLib.VariableControlPipeServer
{
    public partial class AutoCloseMessageBoxForm : Form
    {
        static bool NotActivate { set; get; }

        MessageBoxButtons Buttons { get; set; }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            const int WM_NCLBUTTONDBLCLK = 0xA3;

            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                //非クライアント領域がダブルクリックされた時
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 自動的に閉じるまでの秒数
        /// </summary>
        public int timeRemaining { get; set; } = 20;
        public string OK_Button_Text { get; set; } = "OK";

        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        int counter;
        public AutoCloseMessageBoxForm(string message, string titile, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            InitializeComponent();

            MessageText_label.Text = message;
            this.Text = titile;
            this.Buttons = buttons;


            switch (Buttons)
            {
                case MessageBoxButtons.OK:
                    button1.Visible = false;
                    button1.Text = "";
                    button2.Visible = false;
                    button2.Text = "";
                    button3.Visible = true;
                    button3.Text = "OK";
                    break;

                case MessageBoxButtons.OKCancel:
                    button1.Visible = false;
                    button1.Text = "";
                    button2.Visible = true;
                    button2.Text = "OK";
                    button3.Visible = true;
                    button3.Text = "キャンセル";
                    break;

                case MessageBoxButtons.AbortRetryIgnore:
                    button1.Visible = true;
                    button1.Text = "中止";
                    button2.Visible = true;
                    button2.Text = "再試行";
                    button3.Visible = true;
                    button3.Text = "無視";
                    break;

                case MessageBoxButtons.YesNoCancel:
                    button1.Visible = true;
                    button1.Text = "はい";
                    button2.Visible = true;
                    button2.Text = "いいえ";
                    button3.Visible = true;
                    button3.Text = "キャンセル";
                    break;

                case MessageBoxButtons.YesNo:
                    button1.Visible = false;
                    button1.Text = "";
                    button2.Visible = true;
                    button2.Text = "はい";
                    button3.Visible = true;
                    button3.Text = "いいえ";
                    break;

                case MessageBoxButtons.RetryCancel:
                    button1.Visible = false;
                    button1.Text = "";
                    button2.Visible = true;
                    button2.Text = "再試行";
                    button3.Visible = true;
                    button3.Text = "キャンセル";
                    break;
            }

            switch (icon)
            {
                case MessageBoxIcon.None:
                    Icon_picturebox.Image = Properties.Resources.Asterisk;
                    break;
                case MessageBoxIcon.Hand:
                    Icon_picturebox.Image = Properties.Resources.Hand;
                    break;
                case MessageBoxIcon.Question:
                    Icon_picturebox.Image = Properties.Resources.Question;
                    break;
                case MessageBoxIcon.Exclamation:
                    Icon_picturebox.Image = Properties.Resources.Exclamation;
                    break;
                case MessageBoxIcon.Asterisk:
                    Icon_picturebox.Image = Properties.Resources.Asterisk;
                    break;
            }
        }

        ~AutoCloseMessageBoxForm()
        {
        }

        /// <summary>
        /// タイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClose_timer_Tick(object sender, EventArgs e)
        {
            if (timeRemaining == 0)
                return;
            try
            {
                counter--;
                int bar = counter * (progressBar1.Maximum / timeRemaining);
                if (progressBar1.Minimum < bar && bar < progressBar1.Maximum)
                    progressBar1.Value = bar;

                if (counter < 0)
                {
                    FormClose_timer.Stop();
                    DialogResult = DialogResult.Ignore;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                DebugConsole.WriteLine($"※タイマーイベントハンドラ FormClose_timer_Tick(..)にて例外発生。 {ex.Message}");
            }
        }

        private void AutoCloseMessageBoxForm_Load(object sender, EventArgs e)
        {
            progressBar1.Value = progressBar1.Maximum;
            if (NotActivate)
            {
                DebugConsole.WriteLine($"NotActivate ==  {NotActivate} のためこのセッションでは次のメッセージはポップアップさせませんでした \"{MessageTest_label.Text}\"");
                this.Close();
            }
        }

        private void AutoCloseMessageBoxForm_Shown(object sender, EventArgs e)
        {
            if (timeRemaining == 0)
                progressBar1.Visible = false;
            else
                progressBar1.Visible = true;

            counter = timeRemaining;
            progressBar1.Value = progressBar1.Maximum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            switch (Buttons)
            {
                case MessageBoxButtons.OK:
                    DialogResult = DialogResult.None;
                    this.Close();
                    break;
                case MessageBoxButtons.OKCancel:
                    DialogResult = DialogResult.None;
                    this.Close();
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    DialogResult = DialogResult.Abort;
                    this.Close();
                    break;
                case MessageBoxButtons.YesNoCancel:
                    DialogResult = DialogResult.Yes;
                    this.Close();
                    break;
                case MessageBoxButtons.YesNo:
                    DialogResult = DialogResult.None;
                    this.Close();
                    break;

                case MessageBoxButtons.RetryCancel:
                    DialogResult = DialogResult.None;
                    this.Close();
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (Buttons)
            {
                case MessageBoxButtons.OK:
                    DialogResult = DialogResult.None;
                    this.Close();
                    break;
                case MessageBoxButtons.OKCancel:
                    DialogResult = DialogResult.OK;
                    this.Close();
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    DialogResult = DialogResult.Retry;
                    this.Close();
                    break;
                case MessageBoxButtons.YesNoCancel:
                    DialogResult = DialogResult.No;
                    this.Close();
                    break;
                case MessageBoxButtons.YesNo:
                    DialogResult = DialogResult.Yes;
                    this.Close();
                    break;

                case MessageBoxButtons.RetryCancel:
                    DialogResult = DialogResult.Retry;
                    this.Close();
                    break;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            switch (Buttons)
            {
                case MessageBoxButtons.OK:
                    DialogResult = DialogResult.OK;
                    this.Close();
                    break;
                case MessageBoxButtons.OKCancel:
                    DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    DialogResult = DialogResult.Ignore;
                    this.Close();
                    break;
                case MessageBoxButtons.YesNoCancel:
                    DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;
                case MessageBoxButtons.YesNo:
                    DialogResult = DialogResult.No;
                    this.Close();
                    break;

                case MessageBoxButtons.RetryCancel:
                    DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;
            }

        }


    }
}
