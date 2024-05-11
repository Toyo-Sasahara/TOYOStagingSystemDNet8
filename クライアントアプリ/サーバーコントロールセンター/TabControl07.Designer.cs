
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
            this.GetAvailableMemory_button = new System.Windows.Forms.Button();
            this.GetDRusedMemory_button = new System.Windows.Forms.Button();
            this.GetDCusedMemory_button = new System.Windows.Forms.Button();
            this.logWindowControl = new ServerControlCenterApplication.LogWindowControl();
            this.accountUserForm = new ServerControlCenterApplication.AccountUserForm();
            this.GetSWusedMemory_button = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.panel2);
            this.groupBox4.Controls.Add(this.panel1);
            this.groupBox4.Location = new System.Drawing.Point(3, 116);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(622, 283);
            this.groupBox4.TabIndex = 109;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "SendFileテスト";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.FileReceveStart_button);
            this.panel2.Controls.Add(this.ReceveToLocalFullFileName_textBox);
            this.panel2.Controls.Add(this.ReceveFullFileName_textBox);
            this.panel2.Controls.Add(this.SetSamePathServer_button);
            this.panel2.Location = new System.Drawing.Point(6, 160);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(610, 113);
            this.panel2.TabIndex = 117;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 12);
            this.label1.TabIndex = 108;
            this.label1.Text = "ReceveFullFileName";
            // 
            // FileReceveStart_button
            // 
            this.FileReceveStart_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FileReceveStart_button.Location = new System.Drawing.Point(501, 73);
            this.FileReceveStart_button.Name = "FileReceveStart_button";
            this.FileReceveStart_button.Size = new System.Drawing.Size(98, 23);
            this.FileReceveStart_button.TabIndex = 0;
            this.FileReceveStart_button.Text = "FileReceveStart";
            this.FileReceveStart_button.UseVisualStyleBackColor = true;
            this.FileReceveStart_button.Click += new System.EventHandler(this.FileReceveStart_button_Click);
            // 
            // ReceveToLocalFullFileName_textBox
            // 
            this.ReceveToLocalFullFileName_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReceveToLocalFullFileName_textBox.Location = new System.Drawing.Point(247, 49);
            this.ReceveToLocalFullFileName_textBox.Name = "ReceveToLocalFullFileName_textBox";
            this.ReceveToLocalFullFileName_textBox.Size = new System.Drawing.Size(352, 19);
            this.ReceveToLocalFullFileName_textBox.TabIndex = 107;
            // 
            // ReceveFullFileName_textBox
            // 
            this.ReceveFullFileName_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReceveFullFileName_textBox.Location = new System.Drawing.Point(126, 24);
            this.ReceveFullFileName_textBox.Name = "ReceveFullFileName_textBox";
            this.ReceveFullFileName_textBox.Size = new System.Drawing.Size(473, 19);
            this.ReceveFullFileName_textBox.TabIndex = 100;
            this.ReceveFullFileName_textBox.Text = "\"C:\\TOYOSVC\\ToyoDRAWCAPTUREservice\\Debug\\サービスのインストール.pdf\"";
            this.ReceveFullFileName_textBox.TextChanged += new System.EventHandler(this.ReceveFullFileName_textBox_TextChanged);
            // 
            // SetSamePathServer_button
            // 
            this.SetSamePathServer_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SetSamePathServer_button.Location = new System.Drawing.Point(11, 47);
            this.SetSamePathServer_button.Name = "SetSamePathServer_button";
            this.SetSamePathServer_button.Size = new System.Drawing.Size(230, 23);
            this.SetSamePathServer_button.TabIndex = 110;
            this.SetSamePathServer_button.Text = "ローカルフォルダへの書き出しパスを自動設定";
            this.SetSamePathServer_button.UseVisualStyleBackColor = true;
            this.SetSamePathServer_button.Click += new System.EventHandler(this.SetSamePathServer_button_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.SelectSouceFileName_button);
            this.panel1.Controls.Add(this.FileSendStart_button);
            this.panel1.Controls.Add(this.SendToServerRullFileName);
            this.panel1.Controls.Add(this.SourceFromLocalFullFileName_textBox);
            this.panel1.Controls.Add(this.SetSamePath_button);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Location = new System.Drawing.Point(6, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(610, 113);
            this.panel1.TabIndex = 116;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(9, 27);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(97, 12);
            this.label17.TabIndex = 108;
            this.label17.Text = "SendFullFileName";
            // 
            // SelectSouceFileName_button
            // 
            this.SelectSouceFileName_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectSouceFileName_button.Location = new System.Drawing.Point(501, 22);
            this.SelectSouceFileName_button.Name = "SelectSouceFileName_button";
            this.SelectSouceFileName_button.Size = new System.Drawing.Size(98, 23);
            this.SelectSouceFileName_button.TabIndex = 101;
            this.SelectSouceFileName_button.Text = "FileSelect";
            this.SelectSouceFileName_button.UseVisualStyleBackColor = true;
            this.SelectSouceFileName_button.Click += new System.EventHandler(this.SelectSouceFileName_button_Click);
            // 
            // FileSendStart_button
            // 
            this.FileSendStart_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FileSendStart_button.Location = new System.Drawing.Point(501, 73);
            this.FileSendStart_button.Name = "FileSendStart_button";
            this.FileSendStart_button.Size = new System.Drawing.Size(98, 23);
            this.FileSendStart_button.TabIndex = 0;
            this.FileSendStart_button.Text = "FileSendStart";
            this.FileSendStart_button.UseVisualStyleBackColor = true;
            this.FileSendStart_button.Click += new System.EventHandler(this.FileSendStart_button_Click);
            // 
            // SendToServerRullFileName
            // 
            this.SendToServerRullFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SendToServerRullFileName.Location = new System.Drawing.Point(126, 49);
            this.SendToServerRullFileName.Name = "SendToServerRullFileName";
            this.SendToServerRullFileName.Size = new System.Drawing.Size(358, 19);
            this.SendToServerRullFileName.TabIndex = 107;
            // 
            // SourceFromLocalFullFileName_textBox
            // 
            this.SourceFromLocalFullFileName_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SourceFromLocalFullFileName_textBox.Location = new System.Drawing.Point(126, 24);
            this.SourceFromLocalFullFileName_textBox.Name = "SourceFromLocalFullFileName_textBox";
            this.SourceFromLocalFullFileName_textBox.Size = new System.Drawing.Size(358, 19);
            this.SourceFromLocalFullFileName_textBox.TabIndex = 100;
            this.SourceFromLocalFullFileName_textBox.Text = "\"E:\\Downloads\\Windows フォーム アプリケーションの拡張.pdf\"";
            // 
            // SetSamePath_button
            // 
            this.SetSamePath_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SetSamePath_button.Location = new System.Drawing.Point(501, 47);
            this.SetSamePath_button.Name = "SetSamePath_button";
            this.SetSamePath_button.Size = new System.Drawing.Size(98, 23);
            this.SetSamePath_button.TabIndex = 110;
            this.SetSamePath_button.Text = "SetSamePath";
            this.SetSamePath_button.UseVisualStyleBackColor = true;
            this.SetSamePath_button.Click += new System.EventHandler(this.SetSamePath_button_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 52);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(98, 12);
            this.label18.TabIndex = 109;
            this.label18.Text = "WriteFullFileName";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.GetSWusedMemory_button);
            this.groupBox1.Controls.Add(this.GetDCusedMemory_button);
            this.groupBox1.Controls.Add(this.GetDRusedMemory_button);
            this.groupBox1.Controls.Add(this.GetAvailableMemory_button);
            this.groupBox1.Location = new System.Drawing.Point(3, 405);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(619, 156);
            this.groupBox1.TabIndex = 137;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "サーバー利用可能メモリ取得";
            // 
            // GetAvailableMemory_button
            // 
            this.GetAvailableMemory_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GetAvailableMemory_button.Location = new System.Drawing.Point(17, 33);
            this.GetAvailableMemory_button.Name = "GetAvailableMemory_button";
            this.GetAvailableMemory_button.Size = new System.Drawing.Size(230, 23);
            this.GetAvailableMemory_button.TabIndex = 111;
            this.GetAvailableMemory_button.Text = "サーバー利用可能メモリ取得";
            this.GetAvailableMemory_button.UseVisualStyleBackColor = true;
            this.GetAvailableMemory_button.Click += new System.EventHandler(this.GetAvailableMemory_button_Click);
            // 
            // GetDRusedMemory_button
            // 
            this.GetDRusedMemory_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GetDRusedMemory_button.Location = new System.Drawing.Point(17, 71);
            this.GetDRusedMemory_button.Name = "GetDRusedMemory_button";
            this.GetDRusedMemory_button.Size = new System.Drawing.Size(230, 23);
            this.GetDRusedMemory_button.TabIndex = 112;
            this.GetDRusedMemory_button.Text = "DR使用中メモリ";
            this.GetDRusedMemory_button.UseVisualStyleBackColor = true;
            this.GetDRusedMemory_button.Click += new System.EventHandler(this.GetDRusedMemory_button_Click);
            // 
            // GetDCusedMemory_button
            // 
            this.GetDCusedMemory_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GetDCusedMemory_button.Location = new System.Drawing.Point(17, 100);
            this.GetDCusedMemory_button.Name = "GetDCusedMemory_button";
            this.GetDCusedMemory_button.Size = new System.Drawing.Size(230, 23);
            this.GetDCusedMemory_button.TabIndex = 113;
            this.GetDCusedMemory_button.Text = "DC使用中メモリ";
            this.GetDCusedMemory_button.UseVisualStyleBackColor = true;
            this.GetDCusedMemory_button.Click += new System.EventHandler(this.GetDCusedMemory_button_Click);
            // 
            // logWindowControl
            // 
            this.logWindowControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logWindowControl.Location = new System.Drawing.Point(628, 111);
            this.logWindowControl.Name = "logWindowControl";
            this.logWindowControl.Size = new System.Drawing.Size(623, 525);
            this.logWindowControl.TabIndex = 136;
            // 
            // accountUserForm
            // 
            this.accountUserForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.accountUserForm.Location = new System.Drawing.Point(0, 0);
            this.accountUserForm.Name = "accountUserForm";
            this.accountUserForm.parentControl = null;
            this.accountUserForm.Size = new System.Drawing.Size(1250, 110);
            this.accountUserForm.TabIndex = 135;
            this.accountUserForm.Paint += new System.Windows.Forms.PaintEventHandler(this.accountUserForm1_Paint);
            this.accountUserForm.Leave += new System.EventHandler(this.accountUserForm_Leave);
            // 
            // GetSWusedMemory_button
            // 
            this.GetSWusedMemory_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GetSWusedMemory_button.Location = new System.Drawing.Point(17, 129);
            this.GetSWusedMemory_button.Name = "GetSWusedMemory_button";
            this.GetSWusedMemory_button.Size = new System.Drawing.Size(230, 23);
            this.GetSWusedMemory_button.TabIndex = 115;
            this.GetSWusedMemory_button.Text = "SW使用中メモリ";
            this.GetSWusedMemory_button.UseVisualStyleBackColor = true;
            this.GetSWusedMemory_button.Click += new System.EventHandler(this.GetSWusedMemory_button_Click);
            // 
            // TabControl07
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.logWindowControl);
            this.Controls.Add(this.accountUserForm);
            this.Controls.Add(this.groupBox4);
            this.Name = "TabControl07";
            this.Size = new System.Drawing.Size(1254, 639);
            this.Load += new System.EventHandler(this.TabControl07_Load);
            this.VisibleChanged += new System.EventHandler(this.TabControl07_VisibleChanged);
            this.groupBox4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

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
    }
}
