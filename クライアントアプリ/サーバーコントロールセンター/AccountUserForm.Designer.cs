namespace ServerControlCenterApplication
{
    partial class AccountUserForm
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
            this.サービス接続情報 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CommitPathTextBox = new System.Windows.Forms.TextBox();
            this.CommitShareNameTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.CommitServerHostLabel = new System.Windows.Forms.Label();
            this.DrawWatchPIPEnameTextBox = new System.Windows.Forms.TextBox();
            this.StageServerHostName_comboBox = new System.Windows.Forms.ComboBox();
            this.ClientImpersonationCheckBox = new System.Windows.Forms.CheckBox();
            this.LogonPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.LogonUserTextBox = new System.Windows.Forms.TextBox();
            this.DrawregistPIPEnameTextBox = new System.Windows.Forms.TextBox();
            this.LogonDomainTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.DrawcapturePIPEnameTextBox = new System.Windows.Forms.TextBox();
            this.サービス接続情報.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // サービス接続情報
            // 
            this.サービス接続情報.Controls.Add(this.label3);
            this.サービス接続情報.Controls.Add(this.groupBox1);
            this.サービス接続情報.Controls.Add(this.DrawWatchPIPEnameTextBox);
            this.サービス接続情報.Controls.Add(this.StageServerHostName_comboBox);
            this.サービス接続情報.Controls.Add(this.ClientImpersonationCheckBox);
            this.サービス接続情報.Controls.Add(this.LogonPasswordTextBox);
            this.サービス接続情報.Controls.Add(this.label10);
            this.サービス接続情報.Controls.Add(this.LogonUserTextBox);
            this.サービス接続情報.Controls.Add(this.DrawregistPIPEnameTextBox);
            this.サービス接続情報.Controls.Add(this.LogonDomainTextBox);
            this.サービス接続情報.Controls.Add(this.label8);
            this.サービス接続情報.Controls.Add(this.label5);
            this.サービス接続情報.Controls.Add(this.label9);
            this.サービス接続情報.Controls.Add(this.label6);
            this.サービス接続情報.Controls.Add(this.label7);
            this.サービス接続情報.Controls.Add(this.DrawcapturePIPEnameTextBox);
            this.サービス接続情報.Dock = System.Windows.Forms.DockStyle.Fill;
            this.サービス接続情報.Location = new System.Drawing.Point(0, 0);
            this.サービス接続情報.Name = "サービス接続情報";
            this.サービス接続情報.Size = new System.Drawing.Size(798, 105);
            this.サービス接続情報.TabIndex = 0;
            this.サービス接続情報.TabStop = false;
            this.サービス接続情報.Text = "サービス接続情報";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(432, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "DRAWWATCH用パイプ名";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.CommitPathTextBox);
            this.groupBox1.Controls.Add(this.CommitShareNameTextBox);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.CommitServerHostLabel);
            this.groupBox1.Location = new System.Drawing.Point(576, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 80);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "コミット先URL";
            // 
            // CommitPathTextBox
            // 
            this.CommitPathTextBox.Location = new System.Drawing.Point(88, 45);
            this.CommitPathTextBox.Name = "CommitPathTextBox";
            this.CommitPathTextBox.ReadOnly = true;
            this.CommitPathTextBox.Size = new System.Drawing.Size(116, 19);
            this.CommitPathTextBox.TabIndex = 3;
            this.CommitPathTextBox.Text = "---";
            // 
            // CommitShareNameTextBox
            // 
            this.CommitShareNameTextBox.Location = new System.Drawing.Point(88, 16);
            this.CommitShareNameTextBox.Name = "CommitShareNameTextBox";
            this.CommitShareNameTextBox.Size = new System.Drawing.Size(116, 19);
            this.CommitShareNameTextBox.TabIndex = 1;
            this.CommitShareNameTextBox.Text = "COMMIT$";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(38, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "共有名";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CommitServerHostLabel
            // 
            this.CommitServerHostLabel.AutoSize = true;
            this.CommitServerHostLabel.Location = new System.Drawing.Point(7, 48);
            this.CommitServerHostLabel.Name = "CommitServerHostLabel";
            this.CommitServerHostLabel.Size = new System.Drawing.Size(72, 12);
            this.CommitServerHostLabel.TabIndex = 2;
            this.CommitServerHostLabel.Text = "ｺﾐｯﾄ先ﾌｫﾙﾀﾞ:";
            this.CommitServerHostLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DrawWatchPIPEnameTextBox
            // 
            this.DrawWatchPIPEnameTextBox.Location = new System.Drawing.Point(435, 77);
            this.DrawWatchPIPEnameTextBox.Name = "DrawWatchPIPEnameTextBox";
            this.DrawWatchPIPEnameTextBox.ReadOnly = true;
            this.DrawWatchPIPEnameTextBox.Size = new System.Drawing.Size(137, 19);
            this.DrawWatchPIPEnameTextBox.TabIndex = 14;
            this.DrawWatchPIPEnameTextBox.Text = "WatchService";
            // 
            // StageServerHostName_comboBox
            // 
            this.StageServerHostName_comboBox.Font = new System.Drawing.Font("MS UI Gothic", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.StageServerHostName_comboBox.FormattingEnabled = true;
            this.StageServerHostName_comboBox.Items.AddRange(new object[] {
            "CS1",
            "CS2",
            "CS3",
            "ADS1",
            "ADS2",
            "DC3",
            "DC4",
            "ACVLT1",
            "ACVLT3",
            "SWEPDM2",
            "localhost"});
            this.StageServerHostName_comboBox.Location = new System.Drawing.Point(8, 33);
            this.StageServerHostName_comboBox.Name = "StageServerHostName_comboBox";
            this.StageServerHostName_comboBox.Size = new System.Drawing.Size(129, 43);
            this.StageServerHostName_comboBox.TabIndex = 11;
            this.StageServerHostName_comboBox.Text = "CS2";
            this.StageServerHostName_comboBox.SelectedIndexChanged += new System.EventHandler(this.StageServerHostName_comboBox_SelectedIndexChanged);
            // 
            // ClientImpersonationCheckBox
            // 
            this.ClientImpersonationCheckBox.AutoSize = true;
            this.ClientImpersonationCheckBox.Location = new System.Drawing.Point(148, 32);
            this.ClientImpersonationCheckBox.Name = "ClientImpersonationCheckBox";
            this.ClientImpersonationCheckBox.Size = new System.Drawing.Size(88, 16);
            this.ClientImpersonationCheckBox.TabIndex = 3;
            this.ClientImpersonationCheckBox.Text = "ｸﾗｲｱﾝﾄ偽装";
            this.ClientImpersonationCheckBox.UseVisualStyleBackColor = true;
            this.ClientImpersonationCheckBox.CheckedChanged += new System.EventHandler(this.ClientImpersonationCheckBox_CheckedChanged);
            this.ClientImpersonationCheckBox.Leave += new System.EventHandler(this.ClientImpersonationCheckBox_Leave);
            // 
            // LogonPasswordTextBox
            // 
            this.LogonPasswordTextBox.Location = new System.Drawing.Point(406, 31);
            this.LogonPasswordTextBox.Name = "LogonPasswordTextBox";
            this.LogonPasswordTextBox.PasswordChar = '*';
            this.LogonPasswordTextBox.Size = new System.Drawing.Size(86, 19);
            this.LogonPasswordTextBox.TabIndex = 6;
            this.LogonPasswordTextBox.TextChanged += new System.EventHandler(this.LogonPasswordTextBox_TextChanged);
            this.LogonPasswordTextBox.Leave += new System.EventHandler(this.LogonPasswordTextBox_Leave);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(286, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(130, 12);
            this.label10.TabIndex = 9;
            this.label10.Text = "DRAWREGIST用パイプ名";
            // 
            // LogonUserTextBox
            // 
            this.LogonUserTextBox.Location = new System.Drawing.Point(314, 31);
            this.LogonUserTextBox.Name = "LogonUserTextBox";
            this.LogonUserTextBox.Size = new System.Drawing.Size(86, 19);
            this.LogonUserTextBox.TabIndex = 5;
            this.LogonUserTextBox.TextChanged += new System.EventHandler(this.LogonUserTextBox_TextChanged);
            this.LogonUserTextBox.Leave += new System.EventHandler(this.LogonUserTextBox_Leave);
            // 
            // DrawregistPIPEnameTextBox
            // 
            this.DrawregistPIPEnameTextBox.Location = new System.Drawing.Point(289, 77);
            this.DrawregistPIPEnameTextBox.Name = "DrawregistPIPEnameTextBox";
            this.DrawregistPIPEnameTextBox.ReadOnly = true;
            this.DrawregistPIPEnameTextBox.Size = new System.Drawing.Size(137, 19);
            this.DrawregistPIPEnameTextBox.TabIndex = 13;
            this.DrawregistPIPEnameTextBox.Text = "ApprovalServer";
            // 
            // LogonDomainTextBox
            // 
            this.LogonDomainTextBox.Location = new System.Drawing.Point(242, 31);
            this.LogonDomainTextBox.Name = "LogonDomainTextBox";
            this.LogonDomainTextBox.Size = new System.Drawing.Size(66, 19);
            this.LogonDomainTextBox.TabIndex = 4;
            this.LogonDomainTextBox.TextChanged += new System.EventHandler(this.LogonDomainTextBox_TextChanged);
            this.LogonDomainTextBox.Leave += new System.EventHandler(this.LogonDomainTextBox_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "接続先ホスト";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(255, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "ドメイン";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(143, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(143, 12);
            this.label9.TabIndex = 8;
            this.label9.Text = "DRAWCAPTURE用パイプ名";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(332, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "ユーザー";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(424, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "パスワード";
            // 
            // DrawcapturePIPEnameTextBox
            // 
            this.DrawcapturePIPEnameTextBox.Location = new System.Drawing.Point(146, 77);
            this.DrawcapturePIPEnameTextBox.Name = "DrawcapturePIPEnameTextBox";
            this.DrawcapturePIPEnameTextBox.ReadOnly = true;
            this.DrawcapturePIPEnameTextBox.Size = new System.Drawing.Size(137, 19);
            this.DrawcapturePIPEnameTextBox.TabIndex = 12;
            this.DrawcapturePIPEnameTextBox.Text = "CaptureService";
            // 
            // AccountUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.サービス接続情報);
            this.Name = "AccountUserForm";
            this.Size = new System.Drawing.Size(798, 105);
            this.サービス接続情報.ResumeLayout(false);
            this.サービス接続情報.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox サービス接続情報;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.TextBox DrawWatchPIPEnameTextBox;
        public System.Windows.Forms.ComboBox StageServerHostName_comboBox;
        internal System.Windows.Forms.CheckBox ClientImpersonationCheckBox;
        internal System.Windows.Forms.TextBox LogonPasswordTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.TextBox CommitPathTextBox;
        internal System.Windows.Forms.TextBox CommitShareNameTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label CommitServerHostLabel;
        private System.Windows.Forms.Label label10;
        internal System.Windows.Forms.TextBox LogonUserTextBox;
        internal System.Windows.Forms.TextBox DrawregistPIPEnameTextBox;
        internal System.Windows.Forms.TextBox LogonDomainTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.TextBox DrawcapturePIPEnameTextBox;
    }
}
