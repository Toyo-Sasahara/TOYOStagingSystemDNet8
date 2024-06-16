
namespace ServerControlCenterApplication
{
    partial class TabControl07
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.FileReceveStart_button = new System.Windows.Forms.Button();
            this.ReceveToLocalFullFileName_textBox = new System.Windows.Forms.TextBox();
            this.ReceveFullFileName_textBox = new System.Windows.Forms.TextBox();
            this.SetSamePathServer_button = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.SelectSouceFileName_button = new System.Windows.Forms.Button();
            this.FileSendStart_button = new System.Windows.Forms.Button();
            this.SendToServerRullFileName = new System.Windows.Forms.TextBox();
            this.SourceFromLocalFullFileName_textBox = new System.Windows.Forms.TextBox();
            this.SetSamePath_button = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FromByteArrayToBitmap_button = new System.Windows.Forms.Button();
            this.DebugForm_PictureBox = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.ObjecttoBytebutton = new System.Windows.Forms.Button();
            this.ObjecttoByteViaJsonSerializer_button = new System.Windows.Forms.Button();
            this.GetSWusedMemory_button = new System.Windows.Forms.Button();
            this.GetDCusedMemory_button = new System.Windows.Forms.Button();
            this.GetDRusedMemory_button = new System.Windows.Forms.Button();
            this.GetAvailableMemory_button = new System.Windows.Forms.Button();
            this.logWindowControl = new LogWindowControl();
            this.accountUserForm = new AccountUserForm();
            this.groupBox4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.DebugForm_PictureBox).BeginInit();
            SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.panel2);
            this.groupBox4.Controls.Add(this.panel1);
            this.groupBox4.Location = new System.Drawing.Point(4, 145);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(726, 354);
            this.groupBox4.TabIndex = 109;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "SendFileテスト";
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.FileReceveStart_button);
            this.panel2.Controls.Add(this.ReceveToLocalFullFileName_textBox);
            this.panel2.Controls.Add(this.ReceveFullFileName_textBox);
            this.panel2.Controls.Add(this.SetSamePathServer_button);
            this.panel2.Location = new System.Drawing.Point(7, 200);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(712, 141);
            this.panel2.TabIndex = 117;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 15);
            this.label1.TabIndex = 108;
            this.label1.Text = "ReceveFullFileName";
            // 
            // FileReceveStart_button
            // 
            this.FileReceveStart_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.FileReceveStart_button.Location = new System.Drawing.Point(584, 91);
            this.FileReceveStart_button.Margin = new System.Windows.Forms.Padding(4);
            this.FileReceveStart_button.Name = "FileReceveStart_button";
            this.FileReceveStart_button.Size = new System.Drawing.Size(114, 29);
            this.FileReceveStart_button.TabIndex = 0;
            this.FileReceveStart_button.Text = "FileReceveStart";
            this.FileReceveStart_button.UseVisualStyleBackColor = true;
            this.FileReceveStart_button.Click += FileReceveStart_button_Click;
            // 
            // ReceveToLocalFullFileName_textBox
            // 
            this.ReceveToLocalFullFileName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.ReceveToLocalFullFileName_textBox.Location = new System.Drawing.Point(288, 61);
            this.ReceveToLocalFullFileName_textBox.Margin = new System.Windows.Forms.Padding(4);
            this.ReceveToLocalFullFileName_textBox.Name = "ReceveToLocalFullFileName_textBox";
            this.ReceveToLocalFullFileName_textBox.Size = new System.Drawing.Size(410, 23);
            this.ReceveToLocalFullFileName_textBox.TabIndex = 107;
            // 
            // ReceveFullFileName_textBox
            // 
            this.ReceveFullFileName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.ReceveFullFileName_textBox.Location = new System.Drawing.Point(147, 30);
            this.ReceveFullFileName_textBox.Margin = new System.Windows.Forms.Padding(4);
            this.ReceveFullFileName_textBox.Name = "ReceveFullFileName_textBox";
            this.ReceveFullFileName_textBox.Size = new System.Drawing.Size(551, 23);
            this.ReceveFullFileName_textBox.TabIndex = 100;
            this.ReceveFullFileName_textBox.Text = "\"C:\\TOYOSVC\\ToyoDRAWCAPTUREservice\\Debug\\サービスのインストール.pdf\"";
            this.ReceveFullFileName_textBox.TextChanged += ReceveFullFileName_textBox_TextChanged;
            // 
            // SetSamePathServer_button
            // 
            this.SetSamePathServer_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.SetSamePathServer_button.Location = new System.Drawing.Point(13, 59);
            this.SetSamePathServer_button.Margin = new System.Windows.Forms.Padding(4);
            this.SetSamePathServer_button.Name = "SetSamePathServer_button";
            this.SetSamePathServer_button.Size = new System.Drawing.Size(268, 29);
            this.SetSamePathServer_button.TabIndex = 110;
            this.SetSamePathServer_button.Text = "ローカルフォルダへの書き出しパスを自動設定";
            this.SetSamePathServer_button.UseVisualStyleBackColor = true;
            this.SetSamePathServer_button.Click += SetSamePathServer_button_Click;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.SelectSouceFileName_button);
            this.panel1.Controls.Add(this.FileSendStart_button);
            this.panel1.Controls.Add(this.SendToServerRullFileName);
            this.panel1.Controls.Add(this.SourceFromLocalFullFileName_textBox);
            this.panel1.Controls.Add(this.SetSamePath_button);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Location = new System.Drawing.Point(7, 51);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(712, 141);
            this.panel1.TabIndex = 116;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 34);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 15);
            this.label17.TabIndex = 108;
            this.label17.Text = "SendFullFileName";
            // 
            // SelectSouceFileName_button
            // 
            this.SelectSouceFileName_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.SelectSouceFileName_button.Location = new System.Drawing.Point(584, 28);
            this.SelectSouceFileName_button.Margin = new System.Windows.Forms.Padding(4);
            this.SelectSouceFileName_button.Name = "SelectSouceFileName_button";
            this.SelectSouceFileName_button.Size = new System.Drawing.Size(114, 29);
            this.SelectSouceFileName_button.TabIndex = 101;
            this.SelectSouceFileName_button.Text = "FileSelect";
            this.SelectSouceFileName_button.UseVisualStyleBackColor = true;
            this.SelectSouceFileName_button.Click += SelectSouceFileName_button_Click;
            // 
            // FileSendStart_button
            // 
            this.FileSendStart_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.FileSendStart_button.Location = new System.Drawing.Point(584, 91);
            this.FileSendStart_button.Margin = new System.Windows.Forms.Padding(4);
            this.FileSendStart_button.Name = "FileSendStart_button";
            this.FileSendStart_button.Size = new System.Drawing.Size(114, 29);
            this.FileSendStart_button.TabIndex = 0;
            this.FileSendStart_button.Text = "FileSendStart";
            this.FileSendStart_button.UseVisualStyleBackColor = true;
            this.FileSendStart_button.Click += FileSendStart_button_Click;
            // 
            // SendToServerRullFileName
            // 
            this.SendToServerRullFileName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.SendToServerRullFileName.Location = new System.Drawing.Point(147, 61);
            this.SendToServerRullFileName.Margin = new System.Windows.Forms.Padding(4);
            this.SendToServerRullFileName.Name = "SendToServerRullFileName";
            this.SendToServerRullFileName.Size = new System.Drawing.Size(417, 23);
            this.SendToServerRullFileName.TabIndex = 107;
            // 
            // SourceFromLocalFullFileName_textBox
            // 
            this.SourceFromLocalFullFileName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.SourceFromLocalFullFileName_textBox.Location = new System.Drawing.Point(147, 30);
            this.SourceFromLocalFullFileName_textBox.Margin = new System.Windows.Forms.Padding(4);
            this.SourceFromLocalFullFileName_textBox.Name = "SourceFromLocalFullFileName_textBox";
            this.SourceFromLocalFullFileName_textBox.Size = new System.Drawing.Size(417, 23);
            this.SourceFromLocalFullFileName_textBox.TabIndex = 100;
            this.SourceFromLocalFullFileName_textBox.Text = "\"E:\\Downloads\\Windows フォーム アプリケーションの拡張.pdf\"";
            // 
            // SetSamePath_button
            // 
            this.SetSamePath_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.SetSamePath_button.Location = new System.Drawing.Point(584, 59);
            this.SetSamePath_button.Margin = new System.Windows.Forms.Padding(4);
            this.SetSamePath_button.Name = "SetSamePath_button";
            this.SetSamePath_button.Size = new System.Drawing.Size(114, 29);
            this.SetSamePath_button.TabIndex = 110;
            this.SetSamePath_button.Text = "SetSamePath";
            this.SetSamePath_button.UseVisualStyleBackColor = true;
            this.SetSamePath_button.Click += SetSamePath_button_Click;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 65);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(102, 15);
            this.label18.TabIndex = 109;
            this.label18.Text = "WriteFullFileName";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FromByteArrayToBitmap_button);
            this.groupBox1.Controls.Add(this.DebugForm_PictureBox);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.ObjecttoBytebutton);
            this.groupBox1.Controls.Add(this.ObjecttoByteViaJsonSerializer_button);
            this.groupBox1.Controls.Add(this.GetSWusedMemory_button);
            this.groupBox1.Controls.Add(this.GetDCusedMemory_button);
            this.groupBox1.Controls.Add(this.GetDRusedMemory_button);
            this.groupBox1.Controls.Add(this.GetAvailableMemory_button);
            this.groupBox1.Location = new System.Drawing.Point(4, 506);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(722, 195);
            this.groupBox1.TabIndex = 137;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "サーバー利用可能メモリ取得";
            // 
            // FromByteArrayToBitmap_button
            // 
            this.FromByteArrayToBitmap_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.FromByteArrayToBitmap_button.Location = new System.Drawing.Point(322, 78);
            this.FromByteArrayToBitmap_button.Margin = new System.Windows.Forms.Padding(4);
            this.FromByteArrayToBitmap_button.Name = "FromByteArrayToBitmap_button";
            this.FromByteArrayToBitmap_button.Size = new System.Drawing.Size(176, 29);
            this.FromByteArrayToBitmap_button.TabIndex = 120;
            this.FromByteArrayToBitmap_button.Text = "FromByteArrayToBitmap";
            this.FromByteArrayToBitmap_button.UseVisualStyleBackColor = true;
            this.FromByteArrayToBitmap_button.Click += FromByteArrayToBitmap_button_Click;
            // 
            // DebugForm_PictureBox
            // 
            this.DebugForm_PictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.DebugForm_PictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DebugForm_PictureBox.Location = new System.Drawing.Point(506, 13);
            this.DebugForm_PictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.DebugForm_PictureBox.Name = "DebugForm_PictureBox";
            this.DebugForm_PictureBox.Size = new System.Drawing.Size(208, 174);
            this.DebugForm_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DebugForm_PictureBox.TabIndex = 119;
            this.DebugForm_PictureBox.TabStop = false;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.button1.Location = new System.Drawing.Point(322, 141);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(176, 29);
            this.button1.TabIndex = 118;
            this.button1.Text = "ObjecttoByte[] ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += button1_Click;
            // 
            // ObjecttoBytebutton
            // 
            this.ObjecttoBytebutton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.ObjecttoBytebutton.Location = new System.Drawing.Point(322, 115);
            this.ObjecttoBytebutton.Margin = new System.Windows.Forms.Padding(4);
            this.ObjecttoBytebutton.Name = "ObjecttoBytebutton";
            this.ObjecttoBytebutton.Size = new System.Drawing.Size(176, 29);
            this.ObjecttoBytebutton.TabIndex = 117;
            this.ObjecttoBytebutton.Text = "ObjecttoByte[] ";
            this.ObjecttoBytebutton.UseVisualStyleBackColor = true;
            this.ObjecttoBytebutton.Click += ObjecttoBytebutton_Click;
            // 
            // ObjecttoByteViaJsonSerializer_button
            // 
            this.ObjecttoByteViaJsonSerializer_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.ObjecttoByteViaJsonSerializer_button.Location = new System.Drawing.Point(322, 13);
            this.ObjecttoByteViaJsonSerializer_button.Margin = new System.Windows.Forms.Padding(4);
            this.ObjecttoByteViaJsonSerializer_button.Name = "ObjecttoByteViaJsonSerializer_button";
            this.ObjecttoByteViaJsonSerializer_button.Size = new System.Drawing.Size(176, 43);
            this.ObjecttoByteViaJsonSerializer_button.TabIndex = 116;
            this.ObjecttoByteViaJsonSerializer_button.Text = "ObjecttoByte[] Via JsonSerializer";
            this.ObjecttoByteViaJsonSerializer_button.UseVisualStyleBackColor = true;
            this.ObjecttoByteViaJsonSerializer_button.Click += ObjecttoByteViaJsonSerializer_Click;
            // 
            // GetSWusedMemory_button
            // 
            this.GetSWusedMemory_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.GetSWusedMemory_button.Location = new System.Drawing.Point(20, 161);
            this.GetSWusedMemory_button.Margin = new System.Windows.Forms.Padding(4);
            this.GetSWusedMemory_button.Name = "GetSWusedMemory_button";
            this.GetSWusedMemory_button.Size = new System.Drawing.Size(268, 29);
            this.GetSWusedMemory_button.TabIndex = 115;
            this.GetSWusedMemory_button.Text = "SW使用中メモリ";
            this.GetSWusedMemory_button.UseVisualStyleBackColor = true;
            this.GetSWusedMemory_button.Click += GetSWusedMemory_button_Click;
            // 
            // GetDCusedMemory_button
            // 
            this.GetDCusedMemory_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.GetDCusedMemory_button.Location = new System.Drawing.Point(20, 125);
            this.GetDCusedMemory_button.Margin = new System.Windows.Forms.Padding(4);
            this.GetDCusedMemory_button.Name = "GetDCusedMemory_button";
            this.GetDCusedMemory_button.Size = new System.Drawing.Size(268, 29);
            this.GetDCusedMemory_button.TabIndex = 113;
            this.GetDCusedMemory_button.Text = "DC使用中メモリ";
            this.GetDCusedMemory_button.UseVisualStyleBackColor = true;
            this.GetDCusedMemory_button.Click += GetDCusedMemory_button_Click;
            // 
            // GetDRusedMemory_button
            // 
            this.GetDRusedMemory_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.GetDRusedMemory_button.Location = new System.Drawing.Point(20, 89);
            this.GetDRusedMemory_button.Margin = new System.Windows.Forms.Padding(4);
            this.GetDRusedMemory_button.Name = "GetDRusedMemory_button";
            this.GetDRusedMemory_button.Size = new System.Drawing.Size(268, 29);
            this.GetDRusedMemory_button.TabIndex = 112;
            this.GetDRusedMemory_button.Text = "DR使用中メモリ";
            this.GetDRusedMemory_button.UseVisualStyleBackColor = true;
            this.GetDRusedMemory_button.Click += GetDRusedMemory_button_Click;
            // 
            // GetAvailableMemory_button
            // 
            this.GetAvailableMemory_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.GetAvailableMemory_button.Location = new System.Drawing.Point(20, 41);
            this.GetAvailableMemory_button.Margin = new System.Windows.Forms.Padding(4);
            this.GetAvailableMemory_button.Name = "GetAvailableMemory_button";
            this.GetAvailableMemory_button.Size = new System.Drawing.Size(268, 29);
            this.GetAvailableMemory_button.TabIndex = 111;
            this.GetAvailableMemory_button.Text = "サーバー利用可能メモリ取得";
            this.GetAvailableMemory_button.UseVisualStyleBackColor = true;
            this.GetAvailableMemory_button.Click += GetAvailableMemory_button_Click;
            // 
            // logWindowControl
            // 
            this.logWindowControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.logWindowControl.Location = new System.Drawing.Point(733, 139);
            this.logWindowControl.Margin = new System.Windows.Forms.Padding(5);
            this.logWindowControl.Name = "logWindowControl";
            this.logWindowControl.Size = new System.Drawing.Size(727, 656);
            this.logWindowControl.TabIndex = 136;
            // 
            // accountUserForm
            // 
            this.accountUserForm.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.accountUserForm.Location = new System.Drawing.Point(0, 0);
            this.accountUserForm.Margin = new System.Windows.Forms.Padding(5);
            this.accountUserForm.Name = "accountUserForm";
            this.accountUserForm.parentControl = null;
            this.accountUserForm.Size = new System.Drawing.Size(1458, 138);
            this.accountUserForm.TabIndex = 135;
            this.accountUserForm.Paint += accountUserForm1_Paint;
            this.accountUserForm.Leave += accountUserForm_Leave;
            // 
            // TabControl07
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(this.groupBox1);
            Controls.Add(this.logWindowControl);
            Controls.Add(this.accountUserForm);
            Controls.Add(this.groupBox4);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "TabControl07";
            Size = new System.Drawing.Size(1463, 799);
            Load += TabControl07_Load;
            VisibleChanged += TabControl07_VisibleChanged;
            this.groupBox4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.DebugForm_PictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button SetSamePath_button;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        internal System.Windows.Forms.TextBox SourceFromLocalFullFileName_textBox;
        internal System.Windows.Forms.TextBox SendToServerRullFileName;
        private System.Windows.Forms.Button FileSendStart_button;
        private System.Windows.Forms.Button SelectSouceFileName_button;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button FileReceveStart_button;
        internal System.Windows.Forms.TextBox ReceveToLocalFullFileName_textBox;
        internal System.Windows.Forms.TextBox ReceveFullFileName_textBox;
        private System.Windows.Forms.Button SetSamePathServer_button;
        private System.Windows.Forms.Panel panel1;
        internal AccountUserForm accountUserForm;
        private LogWindowControl logWindowControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button GetAvailableMemory_button;
        private System.Windows.Forms.Button GetDRusedMemory_button;
        private System.Windows.Forms.Button GetDCusedMemory_button;
        private System.Windows.Forms.Button GetSWusedMemory_button;
        private System.Windows.Forms.Button ObjecttoByteViaJsonSerializer_button;
        private System.Windows.Forms.Button ObjecttoBytebutton;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.PictureBox DebugForm_PictureBox;
        private System.Windows.Forms.Button FromByteArrayToBitmap_button;
    }
}
