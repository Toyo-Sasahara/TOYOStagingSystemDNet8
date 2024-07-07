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
            サービス接続情報 = new System.Windows.Forms.GroupBox();
            label3 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            CommitPathTextBox = new System.Windows.Forms.TextBox();
            CommitShareNameTextBox = new System.Windows.Forms.TextBox();
            label11 = new System.Windows.Forms.Label();
            CommitServerHostLabel = new System.Windows.Forms.Label();
            DrawWatchPIPEnameTextBox = new System.Windows.Forms.TextBox();
            StageServerHostName_comboBox = new System.Windows.Forms.ComboBox();
            ClientImpersonationCheckBox = new System.Windows.Forms.CheckBox();
            LogonPasswordTextBox = new System.Windows.Forms.TextBox();
            label10 = new System.Windows.Forms.Label();
            LogonUserTextBox = new System.Windows.Forms.TextBox();
            DrawregistPIPEnameTextBox = new System.Windows.Forms.TextBox();
            LogonDomainTextBox = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            DrawcapturePIPEnameTextBox = new System.Windows.Forms.TextBox();
            サービス接続情報.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // サービス接続情報
            // 
            サービス接続情報.Controls.Add(label3);
            サービス接続情報.Controls.Add(groupBox1);
            サービス接続情報.Controls.Add(DrawWatchPIPEnameTextBox);
            サービス接続情報.Controls.Add(StageServerHostName_comboBox);
            サービス接続情報.Controls.Add(ClientImpersonationCheckBox);
            サービス接続情報.Controls.Add(LogonPasswordTextBox);
            サービス接続情報.Controls.Add(label10);
            サービス接続情報.Controls.Add(LogonUserTextBox);
            サービス接続情報.Controls.Add(DrawregistPIPEnameTextBox);
            サービス接続情報.Controls.Add(LogonDomainTextBox);
            サービス接続情報.Controls.Add(label8);
            サービス接続情報.Controls.Add(label5);
            サービス接続情報.Controls.Add(label9);
            サービス接続情報.Controls.Add(label6);
            サービス接続情報.Controls.Add(label7);
            サービス接続情報.Controls.Add(DrawcapturePIPEnameTextBox);
            サービス接続情報.Dock = System.Windows.Forms.DockStyle.Fill;
            サービス接続情報.Location = new System.Drawing.Point(0, 0);
            サービス接続情報.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            サービス接続情報.Name = "サービス接続情報";
            サービス接続情報.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            サービス接続情報.Size = new System.Drawing.Size(1003, 126);
            サービス接続情報.TabIndex = 0;
            サービス接続情報.TabStop = false;
            サービス接続情報.Text = "サービス接続情報";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(504, 74);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(134, 15);
            label3.TabIndex = 10;
            label3.Text = "DRAWWATCH用パイプ名";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(CommitPathTextBox);
            groupBox1.Controls.Add(CommitShareNameTextBox);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(CommitServerHostLabel);
            groupBox1.Location = new System.Drawing.Point(744, 18);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            groupBox1.Size = new System.Drawing.Size(252, 95);
            groupBox1.TabIndex = 15;
            groupBox1.TabStop = false;
            groupBox1.Text = "コミット先URL";
            // 
            // CommitPathTextBox
            // 
            CommitPathTextBox.Location = new System.Drawing.Point(103, 56);
            CommitPathTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            CommitPathTextBox.Name = "CommitPathTextBox";
            CommitPathTextBox.ReadOnly = true;
            CommitPathTextBox.Size = new System.Drawing.Size(135, 23);
            CommitPathTextBox.TabIndex = 3;
            CommitPathTextBox.Text = "---";
            // 
            // CommitShareNameTextBox
            // 
            CommitShareNameTextBox.Location = new System.Drawing.Point(103, 20);
            CommitShareNameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            CommitShareNameTextBox.Name = "CommitShareNameTextBox";
            CommitShareNameTextBox.Size = new System.Drawing.Size(135, 23);
            CommitShareNameTextBox.TabIndex = 1;
            CommitShareNameTextBox.Text = "COMMIT$";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(44, 24);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(43, 15);
            label11.TabIndex = 0;
            label11.Text = "共有名";
            label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CommitServerHostLabel
            // 
            CommitServerHostLabel.AutoSize = true;
            CommitServerHostLabel.Location = new System.Drawing.Point(8, 60);
            CommitServerHostLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            CommitServerHostLabel.Name = "CommitServerHostLabel";
            CommitServerHostLabel.Size = new System.Drawing.Size(76, 15);
            CommitServerHostLabel.TabIndex = 2;
            CommitServerHostLabel.Text = "ｺﾐｯﾄ先ﾌｫﾙﾀﾞ:";
            CommitServerHostLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DrawWatchPIPEnameTextBox
            // 
            DrawWatchPIPEnameTextBox.Location = new System.Drawing.Point(507, 96);
            DrawWatchPIPEnameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            DrawWatchPIPEnameTextBox.Name = "DrawWatchPIPEnameTextBox";
            DrawWatchPIPEnameTextBox.ReadOnly = true;
            DrawWatchPIPEnameTextBox.Size = new System.Drawing.Size(159, 23);
            DrawWatchPIPEnameTextBox.TabIndex = 14;
            DrawWatchPIPEnameTextBox.Text = "WatchService";
            // 
            // StageServerHostName_comboBox
            // 
            StageServerHostName_comboBox.Font = new System.Drawing.Font("MS UI Gothic", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 128);
            StageServerHostName_comboBox.FormattingEnabled = true;
            StageServerHostName_comboBox.Items.AddRange(new object[] { "CS1", "CS2", "CS3", "ADS1", "ADS2", "DC3", "DC4", "ACVLT1", "ACVLT3", "SWEPDM2", "localhost" });
            StageServerHostName_comboBox.Location = new System.Drawing.Point(9, 41);
            StageServerHostName_comboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            StageServerHostName_comboBox.Name = "StageServerHostName_comboBox";
            StageServerHostName_comboBox.Size = new System.Drawing.Size(150, 43);
            StageServerHostName_comboBox.TabIndex = 11;
            StageServerHostName_comboBox.Text = "CS2";
            StageServerHostName_comboBox.SelectedIndexChanged += StageServerHostName_comboBox_SelectedIndexChanged;
            // 
            // ClientImpersonationCheckBox
            // 
            ClientImpersonationCheckBox.AutoSize = true;
            ClientImpersonationCheckBox.Location = new System.Drawing.Point(173, 40);
            ClientImpersonationCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            ClientImpersonationCheckBox.Name = "ClientImpersonationCheckBox";
            ClientImpersonationCheckBox.Size = new System.Drawing.Size(86, 19);
            ClientImpersonationCheckBox.TabIndex = 3;
            ClientImpersonationCheckBox.Text = "ｸﾗｲｱﾝﾄ偽装";
            ClientImpersonationCheckBox.UseVisualStyleBackColor = true;
            ClientImpersonationCheckBox.CheckedChanged += ClientImpersonationCheckBox_CheckedChanged;
            ClientImpersonationCheckBox.Leave += ClientImpersonationCheckBox_Leave;
            // 
            // LogonPasswordTextBox
            // 
            LogonPasswordTextBox.Location = new System.Drawing.Point(474, 39);
            LogonPasswordTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            LogonPasswordTextBox.Name = "LogonPasswordTextBox";
            LogonPasswordTextBox.PasswordChar = '*';
            LogonPasswordTextBox.Size = new System.Drawing.Size(100, 23);
            LogonPasswordTextBox.TabIndex = 6;
            LogonPasswordTextBox.TextChanged += LogonPasswordTextBox_TextChanged;
            LogonPasswordTextBox.Leave += LogonPasswordTextBox_Leave;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(334, 74);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(129, 15);
            label10.TabIndex = 9;
            label10.Text = "DRAWREGIST用パイプ名";
            // 
            // LogonUserTextBox
            // 
            LogonUserTextBox.Location = new System.Drawing.Point(366, 39);
            LogonUserTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            LogonUserTextBox.Name = "LogonUserTextBox";
            LogonUserTextBox.Size = new System.Drawing.Size(100, 23);
            LogonUserTextBox.TabIndex = 5;
            LogonUserTextBox.TextChanged += LogonUserTextBox_TextChanged;
            LogonUserTextBox.Leave += LogonUserTextBox_Leave;
            // 
            // DrawregistPIPEnameTextBox
            // 
            DrawregistPIPEnameTextBox.Location = new System.Drawing.Point(337, 96);
            DrawregistPIPEnameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            DrawregistPIPEnameTextBox.Name = "DrawregistPIPEnameTextBox";
            DrawregistPIPEnameTextBox.ReadOnly = true;
            DrawregistPIPEnameTextBox.Size = new System.Drawing.Size(159, 23);
            DrawregistPIPEnameTextBox.TabIndex = 13;
            DrawregistPIPEnameTextBox.Text = "ApprovalServer";
            // 
            // LogonDomainTextBox
            // 
            LogonDomainTextBox.Location = new System.Drawing.Point(282, 39);
            LogonDomainTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            LogonDomainTextBox.Name = "LogonDomainTextBox";
            LogonDomainTextBox.Size = new System.Drawing.Size(76, 23);
            LogonDomainTextBox.TabIndex = 4;
            LogonDomainTextBox.TextChanged += LogonDomainTextBox_TextChanged;
            LogonDomainTextBox.Leave += LogonDomainTextBox_Leave;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(7, 19);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(70, 15);
            label8.TabIndex = 7;
            label8.Text = "接続先ホスト";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(298, 20);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(40, 15);
            label5.TabIndex = 0;
            label5.Text = "ドメイン";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(167, 74);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(142, 15);
            label9.TabIndex = 8;
            label9.Text = "DRAWCAPTURE用パイプ名";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(387, 20);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(43, 15);
            label6.TabIndex = 1;
            label6.Text = "ユーザー";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(495, 20);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(51, 15);
            label7.TabIndex = 2;
            label7.Text = "パスワード";
            // 
            // DrawcapturePIPEnameTextBox
            // 
            DrawcapturePIPEnameTextBox.Location = new System.Drawing.Point(170, 96);
            DrawcapturePIPEnameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            DrawcapturePIPEnameTextBox.Name = "DrawcapturePIPEnameTextBox";
            DrawcapturePIPEnameTextBox.ReadOnly = true;
            DrawcapturePIPEnameTextBox.Size = new System.Drawing.Size(159, 23);
            DrawcapturePIPEnameTextBox.TabIndex = 12;
            DrawcapturePIPEnameTextBox.Text = "CaptureService";
            // 
            // AccountUserForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(サービス接続情報);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "AccountUserForm";
            Size = new System.Drawing.Size(1003, 126);
            サービス接続情報.ResumeLayout(false);
            サービス接続情報.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
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
