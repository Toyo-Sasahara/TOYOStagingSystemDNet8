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
            this.panel10 = new System.Windows.Forms.Panel();
            this.SW_ConnectTest_button = new System.Windows.Forms.Button();
            this.DC_ConnectTest_button = new System.Windows.Forms.Button();
            this.DR_ConnectTest_button = new System.Windows.Forms.Button();
            this.PIPETESTMSG_textBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DC_Shudown_button = new System.Windows.Forms.Button();
            this.SW_Shudown_button = new System.Windows.Forms.Button();
            this.DR_Shudown_button = new System.Windows.Forms.Button();
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
            this.panel10.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // サービス接続情報
            // 
            this.サービス接続情報.Controls.Add(this.panel10);
            this.サービス接続情報.Controls.Add(this.groupBox2);
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
            this.サービス接続情報.Size = new System.Drawing.Size(1049, 105);
            this.サービス接続情報.TabIndex = 0;
            this.サービス接続情報.TabStop = false;
            this.サービス接続情報.Text = "サービス接続情報";
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.SW_ConnectTest_button);
            this.panel10.Controls.Add(this.DC_ConnectTest_button);
            this.panel10.Controls.Add(this.DR_ConnectTest_button);
            this.panel10.Controls.Add(this.PIPETESTMSG_textBox);
            this.panel10.Location = new System.Drawing.Point(800, 15);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(121, 80);
            this.panel10.TabIndex = 128;
            // 
            // SW_ConnectTest_button
            // 
            this.SW_ConnectTest_button.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SW_ConnectTest_button.Location = new System.Drawing.Point(5, 57);
            this.SW_ConnectTest_button.Name = "SW_ConnectTest_button";
            this.SW_ConnectTest_button.Size = new System.Drawing.Size(113, 18);
            this.SW_ConnectTest_button.TabIndex = 127;
            this.SW_ConnectTest_button.Text = "SW_ConnectTest";
            this.SW_ConnectTest_button.UseVisualStyleBackColor = true;
            this.SW_ConnectTest_button.Click += new System.EventHandler(this.SW_ConnectTest_button_Click);
            // 
            // DC_ConnectTest_button
            // 
            this.DC_ConnectTest_button.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DC_ConnectTest_button.Location = new System.Drawing.Point(6, 21);
            this.DC_ConnectTest_button.Name = "DC_ConnectTest_button";
            this.DC_ConnectTest_button.Size = new System.Drawing.Size(113, 18);
            this.DC_ConnectTest_button.TabIndex = 126;
            this.DC_ConnectTest_button.Text = "DC_ConnectTest";
            this.DC_ConnectTest_button.UseVisualStyleBackColor = true;
            this.DC_ConnectTest_button.Click += new System.EventHandler(this.DC_ConnectTest_button_Click);
            // 
            // DR_ConnectTest_button
            // 
            this.DR_ConnectTest_button.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DR_ConnectTest_button.Location = new System.Drawing.Point(5, 39);
            this.DR_ConnectTest_button.Name = "DR_ConnectTest_button";
            this.DR_ConnectTest_button.Size = new System.Drawing.Size(113, 18);
            this.DR_ConnectTest_button.TabIndex = 1;
            this.DR_ConnectTest_button.Text = "DR_ConnectTest";
            this.DR_ConnectTest_button.UseVisualStyleBackColor = true;
            this.DR_ConnectTest_button.Click += new System.EventHandler(this.DR_ConnectTest_button_Click);
            // 
            // PIPETESTMSG_textBox
            // 
            this.PIPETESTMSG_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PIPETESTMSG_textBox.Location = new System.Drawing.Point(8, 3);
            this.PIPETESTMSG_textBox.Name = "PIPETESTMSG_textBox";
            this.PIPETESTMSG_textBox.Size = new System.Drawing.Size(110, 19);
            this.PIPETESTMSG_textBox.TabIndex = 125;
            this.PIPETESTMSG_textBox.Text = "送出テスト文字列";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.DC_Shudown_button);
            this.groupBox2.Controls.Add(this.SW_Shudown_button);
            this.groupBox2.Controls.Add(this.DR_Shudown_button);
            this.groupBox2.Location = new System.Drawing.Point(927, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(116, 80);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "サービスシャットダウン";
            // 
            // DC_Shudown_button
            // 
            this.DC_Shudown_button.Location = new System.Drawing.Point(6, 12);
            this.DC_Shudown_button.Name = "DC_Shudown_button";
            this.DC_Shudown_button.Size = new System.Drawing.Size(104, 20);
            this.DC_Shudown_button.TabIndex = 18;
            this.DC_Shudown_button.Text = "DC";
            this.DC_Shudown_button.UseVisualStyleBackColor = true;
            this.DC_Shudown_button.Click += new System.EventHandler(this.DC_Shudown_button_Click);
            // 
            // SW_Shudown_button
            // 
            this.SW_Shudown_button.Location = new System.Drawing.Point(6, 52);
            this.SW_Shudown_button.Name = "SW_Shudown_button";
            this.SW_Shudown_button.Size = new System.Drawing.Size(104, 20);
            this.SW_Shudown_button.TabIndex = 16;
            this.SW_Shudown_button.Text = "SW";
            this.SW_Shudown_button.UseVisualStyleBackColor = true;
            this.SW_Shudown_button.Click += new System.EventHandler(this.SW_Shudown_button_Click);
            // 
            // DR_Shudown_button
            // 
            this.DR_Shudown_button.Location = new System.Drawing.Point(6, 32);
            this.DR_Shudown_button.Name = "DR_Shudown_button";
            this.DR_Shudown_button.Size = new System.Drawing.Size(104, 20);
            this.DR_Shudown_button.TabIndex = 17;
            this.DR_Shudown_button.Text = "DR";
            this.DR_Shudown_button.UseVisualStyleBackColor = true;
            this.DR_Shudown_button.Click += new System.EventHandler(this.DR_Shudown_button_Click);
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
            this.groupBox1.Controls.Add(this.CommitPathTextBox);
            this.groupBox1.Controls.Add(this.CommitShareNameTextBox);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.CommitServerHostLabel);
            this.groupBox1.Location = new System.Drawing.Point(578, 15);
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
            this.ClientImpersonationCheckBox.Leave += new System.EventHandler(this.ClientImpersonationCheckBox_Leave);
            // 
            // LogonPasswordTextBox
            // 
            this.LogonPasswordTextBox.Location = new System.Drawing.Point(406, 31);
            this.LogonPasswordTextBox.Name = "LogonPasswordTextBox";
            this.LogonPasswordTextBox.PasswordChar = '*';
            this.LogonPasswordTextBox.Size = new System.Drawing.Size(86, 19);
            this.LogonPasswordTextBox.TabIndex = 6;
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
            this.Size = new System.Drawing.Size(1049, 105);
            this.サービス接続情報.ResumeLayout(false);
            this.サービス接続情報.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.groupBox2.ResumeLayout(false);
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
