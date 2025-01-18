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
            panel10 = new System.Windows.Forms.Panel();
            SW_ConnectTest_button = new System.Windows.Forms.Button();
            DC_ConnectTest_button = new System.Windows.Forms.Button();
            DR_ConnectTest_button = new System.Windows.Forms.Button();
            PIPETESTMSG_textBox = new System.Windows.Forms.TextBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            DC_Shudown_button = new System.Windows.Forms.Button();
            SW_Shudown_button = new System.Windows.Forms.Button();
            DR_Shudown_button = new System.Windows.Forms.Button();
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
            panel10.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // サービス接続情報
            // 
            サービス接続情報.Controls.Add(panel10);
            サービス接続情報.Controls.Add(groupBox2);
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
            サービス接続情報.Name = "サービス接続情報";
            サービス接続情報.Size = new System.Drawing.Size(1049, 105);
            サービス接続情報.TabIndex = 0;
            サービス接続情報.TabStop = false;
            サービス接続情報.Text = "サービス接続情報";
            // 
            // panel10
            // 
            panel10.Controls.Add(SW_ConnectTest_button);
            panel10.Controls.Add(DC_ConnectTest_button);
            panel10.Controls.Add(DR_ConnectTest_button);
            panel10.Controls.Add(PIPETESTMSG_textBox);
            panel10.Location = new System.Drawing.Point(800, 13);
            panel10.Name = "panel10";
            panel10.Size = new System.Drawing.Size(121, 92);
            panel10.TabIndex = 128;
            // 
            // SW_ConnectTest_button
            // 
            SW_ConnectTest_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            SW_ConnectTest_button.Location = new System.Drawing.Point(5, 68);
            SW_ConnectTest_button.Name = "SW_ConnectTest_button";
            SW_ConnectTest_button.Size = new System.Drawing.Size(113, 20);
            SW_ConnectTest_button.TabIndex = 127;
            SW_ConnectTest_button.Text = "SW_ConnectTest";
            SW_ConnectTest_button.UseVisualStyleBackColor = true;
            SW_ConnectTest_button.Click += SW_ConnectTest_button_Click;
            // 
            // DC_ConnectTest_button
            // 
            DC_ConnectTest_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            DC_ConnectTest_button.Location = new System.Drawing.Point(6, 26);
            DC_ConnectTest_button.Name = "DC_ConnectTest_button";
            DC_ConnectTest_button.Size = new System.Drawing.Size(113, 20);
            DC_ConnectTest_button.TabIndex = 126;
            DC_ConnectTest_button.Text = "DC_ConnectTest";
            DC_ConnectTest_button.UseVisualStyleBackColor = true;
            DC_ConnectTest_button.Click += DC_ConnectTest_button_Click;
            // 
            // DR_ConnectTest_button
            // 
            DR_ConnectTest_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            DR_ConnectTest_button.Location = new System.Drawing.Point(5, 47);
            DR_ConnectTest_button.Name = "DR_ConnectTest_button";
            DR_ConnectTest_button.Size = new System.Drawing.Size(113, 20);
            DR_ConnectTest_button.TabIndex = 1;
            DR_ConnectTest_button.Text = "DR_ConnectTest";
            DR_ConnectTest_button.UseVisualStyleBackColor = true;
            DR_ConnectTest_button.Click += DR_ConnectTest_button_Click;
            // 
            // PIPETESTMSG_textBox
            // 
            PIPETESTMSG_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PIPETESTMSG_textBox.Font = new System.Drawing.Font("Yu Gothic UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            PIPETESTMSG_textBox.Location = new System.Drawing.Point(8, 3);
            PIPETESTMSG_textBox.Name = "PIPETESTMSG_textBox";
            PIPETESTMSG_textBox.Size = new System.Drawing.Size(110, 22);
            PIPETESTMSG_textBox.TabIndex = 125;
            PIPETESTMSG_textBox.Text = "送出テスト文字列";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(DC_Shudown_button);
            groupBox2.Controls.Add(SW_Shudown_button);
            groupBox2.Controls.Add(DR_Shudown_button);
            groupBox2.Location = new System.Drawing.Point(927, 15);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(116, 80);
            groupBox2.TabIndex = 19;
            groupBox2.TabStop = false;
            groupBox2.Text = "サービスシャットダウン";
            // 
            // DC_Shudown_button
            // 
            DC_Shudown_button.Location = new System.Drawing.Point(6, 12);
            DC_Shudown_button.Name = "DC_Shudown_button";
            DC_Shudown_button.Size = new System.Drawing.Size(104, 20);
            DC_Shudown_button.TabIndex = 18;
            DC_Shudown_button.Text = "DC";
            DC_Shudown_button.UseVisualStyleBackColor = true;
            DC_Shudown_button.Click += DC_Shudown_button_Click;
            // 
            // SW_Shudown_button
            // 
            SW_Shudown_button.Location = new System.Drawing.Point(6, 52);
            SW_Shudown_button.Name = "SW_Shudown_button";
            SW_Shudown_button.Size = new System.Drawing.Size(104, 20);
            SW_Shudown_button.TabIndex = 16;
            SW_Shudown_button.Text = "SW";
            SW_Shudown_button.UseVisualStyleBackColor = true;
            SW_Shudown_button.Click += SW_Shudown_button_Click;
            // 
            // DR_Shudown_button
            // 
            DR_Shudown_button.Location = new System.Drawing.Point(6, 32);
            DR_Shudown_button.Name = "DR_Shudown_button";
            DR_Shudown_button.Size = new System.Drawing.Size(104, 20);
            DR_Shudown_button.TabIndex = 17;
            DR_Shudown_button.Text = "DR";
            DR_Shudown_button.UseVisualStyleBackColor = true;
            DR_Shudown_button.Click += DR_Shudown_button_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(432, 59);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(134, 15);
            label3.TabIndex = 10;
            label3.Text = "DRAWWATCH用パイプ名";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(CommitPathTextBox);
            groupBox1.Controls.Add(CommitShareNameTextBox);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(CommitServerHostLabel);
            groupBox1.Location = new System.Drawing.Point(578, 13);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(216, 89);
            groupBox1.TabIndex = 15;
            groupBox1.TabStop = false;
            groupBox1.Text = "コミット先URL";
            // 
            // CommitPathTextBox
            // 
            CommitPathTextBox.Location = new System.Drawing.Point(88, 45);
            CommitPathTextBox.Name = "CommitPathTextBox";
            CommitPathTextBox.ReadOnly = true;
            CommitPathTextBox.Size = new System.Drawing.Size(116, 23);
            CommitPathTextBox.TabIndex = 3;
            CommitPathTextBox.Text = "---";
            // 
            // CommitShareNameTextBox
            // 
            CommitShareNameTextBox.Location = new System.Drawing.Point(88, 16);
            CommitShareNameTextBox.Name = "CommitShareNameTextBox";
            CommitShareNameTextBox.ReadOnly = true;
            CommitShareNameTextBox.Size = new System.Drawing.Size(116, 23);
            CommitShareNameTextBox.TabIndex = 1;
            CommitShareNameTextBox.Text = "COMMIT$";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(38, 19);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(43, 15);
            label11.TabIndex = 0;
            label11.Text = "共有名";
            label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CommitServerHostLabel
            // 
            CommitServerHostLabel.AutoSize = true;
            CommitServerHostLabel.Location = new System.Drawing.Point(7, 48);
            CommitServerHostLabel.Name = "CommitServerHostLabel";
            CommitServerHostLabel.Size = new System.Drawing.Size(76, 15);
            CommitServerHostLabel.TabIndex = 2;
            CommitServerHostLabel.Text = "ｺﾐｯﾄ先ﾌｫﾙﾀﾞ:";
            CommitServerHostLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DrawWatchPIPEnameTextBox
            // 
            DrawWatchPIPEnameTextBox.Location = new System.Drawing.Point(435, 77);
            DrawWatchPIPEnameTextBox.Name = "DrawWatchPIPEnameTextBox";
            DrawWatchPIPEnameTextBox.ReadOnly = true;
            DrawWatchPIPEnameTextBox.Size = new System.Drawing.Size(137, 23);
            DrawWatchPIPEnameTextBox.TabIndex = 14;
            DrawWatchPIPEnameTextBox.Text = "WatchService";
            // 
            // StageServerHostName_comboBox
            // 
            StageServerHostName_comboBox.Font = new System.Drawing.Font("MS UI Gothic", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 128);
            StageServerHostName_comboBox.FormattingEnabled = true;
            StageServerHostName_comboBox.Items.AddRange(new object[] { "CS1", "CS2", "CS3", "ADS1", "ADS2", "DC3", "DC4", "ACVLT1", "ACVLT3", "SWEPDM2", "localhost" });
            StageServerHostName_comboBox.Location = new System.Drawing.Point(8, 33);
            StageServerHostName_comboBox.Name = "StageServerHostName_comboBox";
            StageServerHostName_comboBox.Size = new System.Drawing.Size(129, 43);
            StageServerHostName_comboBox.TabIndex = 11;
            StageServerHostName_comboBox.Text = "CS2";
            StageServerHostName_comboBox.SelectedIndexChanged += StageServerHostName_comboBox_SelectedIndexChanged;
            // 
            // ClientImpersonationCheckBox
            // 
            ClientImpersonationCheckBox.AutoSize = true;
            ClientImpersonationCheckBox.Location = new System.Drawing.Point(148, 32);
            ClientImpersonationCheckBox.Name = "ClientImpersonationCheckBox";
            ClientImpersonationCheckBox.Size = new System.Drawing.Size(86, 19);
            ClientImpersonationCheckBox.TabIndex = 3;
            ClientImpersonationCheckBox.Text = "ｸﾗｲｱﾝﾄ偽装";
            ClientImpersonationCheckBox.UseVisualStyleBackColor = true;
            ClientImpersonationCheckBox.Leave += ClientImpersonationCheckBox_Leave;
            // 
            // LogonPasswordTextBox
            // 
            LogonPasswordTextBox.Location = new System.Drawing.Point(406, 31);
            LogonPasswordTextBox.Name = "LogonPasswordTextBox";
            LogonPasswordTextBox.PasswordChar = '*';
            LogonPasswordTextBox.Size = new System.Drawing.Size(86, 23);
            LogonPasswordTextBox.TabIndex = 6;
            LogonPasswordTextBox.Leave += LogonPasswordTextBox_Leave;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(286, 59);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(129, 15);
            label10.TabIndex = 9;
            label10.Text = "DRAWREGIST用パイプ名";
            // 
            // LogonUserTextBox
            // 
            LogonUserTextBox.Location = new System.Drawing.Point(314, 31);
            LogonUserTextBox.Name = "LogonUserTextBox";
            LogonUserTextBox.Size = new System.Drawing.Size(86, 23);
            LogonUserTextBox.TabIndex = 5;
            LogonUserTextBox.Leave += LogonUserTextBox_Leave;
            // 
            // DrawregistPIPEnameTextBox
            // 
            DrawregistPIPEnameTextBox.Location = new System.Drawing.Point(289, 77);
            DrawregistPIPEnameTextBox.Name = "DrawregistPIPEnameTextBox";
            DrawregistPIPEnameTextBox.ReadOnly = true;
            DrawregistPIPEnameTextBox.Size = new System.Drawing.Size(137, 23);
            DrawregistPIPEnameTextBox.TabIndex = 13;
            DrawregistPIPEnameTextBox.Text = "ApprovalServer";
            // 
            // LogonDomainTextBox
            // 
            LogonDomainTextBox.Location = new System.Drawing.Point(242, 31);
            LogonDomainTextBox.Name = "LogonDomainTextBox";
            LogonDomainTextBox.Size = new System.Drawing.Size(66, 23);
            LogonDomainTextBox.TabIndex = 4;
            LogonDomainTextBox.Leave += LogonDomainTextBox_Leave;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(6, 15);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(70, 15);
            label8.TabIndex = 7;
            label8.Text = "接続先ホスト";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(255, 16);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(40, 15);
            label5.TabIndex = 0;
            label5.Text = "ドメイン";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(143, 59);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(142, 15);
            label9.TabIndex = 8;
            label9.Text = "DRAWCAPTURE用パイプ名";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(332, 16);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(43, 15);
            label6.TabIndex = 1;
            label6.Text = "ユーザー";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(424, 16);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(51, 15);
            label7.TabIndex = 2;
            label7.Text = "パスワード";
            // 
            // DrawcapturePIPEnameTextBox
            // 
            DrawcapturePIPEnameTextBox.Location = new System.Drawing.Point(146, 77);
            DrawcapturePIPEnameTextBox.Name = "DrawcapturePIPEnameTextBox";
            DrawcapturePIPEnameTextBox.ReadOnly = true;
            DrawcapturePIPEnameTextBox.Size = new System.Drawing.Size(137, 23);
            DrawcapturePIPEnameTextBox.TabIndex = 12;
            DrawcapturePIPEnameTextBox.Text = "CaptureService";
            // 
            // AccountUserForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            Controls.Add(サービス接続情報);
            Name = "AccountUserForm";
            Size = new System.Drawing.Size(1049, 105);
            サービス接続情報.ResumeLayout(false);
            サービス接続情報.PerformLayout();
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            groupBox2.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button DC_Shudown_button;
        private System.Windows.Forms.Button SW_Shudown_button;
        private System.Windows.Forms.Button DR_Shudown_button;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button SW_ConnectTest_button;
        private System.Windows.Forms.Button DC_ConnectTest_button;
        private System.Windows.Forms.Button DR_ConnectTest_button;
        private System.Windows.Forms.TextBox PIPETESTMSG_textBox;
    }
}
