
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
            this.SetSamePathServer_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ReceveFileObjectConvNew_checkBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FileReceveStart_button = new System.Windows.Forms.Button();
            this.FileRecvTest_Dist_Server_FullFileName_textBox = new System.Windows.Forms.TextBox();
            this.FileRecvTest_Source_ServerFullFIleName_textBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.FileSendWriteObjectConvNew_checkBox = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.SelectSouceFileName_button = new System.Windows.Forms.Button();
            this.FileSendStart_button = new System.Windows.Forms.Button();
            this.FileSendTest_Dist_ServerFuleFileName_textBox = new System.Windows.Forms.TextBox();
            this.FileSendTest_Souce_LocalFullFileName_textBox = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.FieldValuseSets_JSONCONV_button = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.objectConvNew_checkBox = new System.Windows.Forms.CheckBox();
            this.GetAvailableMemory_button = new System.Windows.Forms.Button();
            this.GetDRusedMemory_button = new System.Windows.Forms.Button();
            this.GetSWusedMemory_button = new System.Windows.Forms.Button();
            this.GetDCusedMemory_button = new System.Windows.Forms.Button();
            this.JsonTest_button = new System.Windows.Forms.Button();
            this.logWindowControl = new LogWindowControl();
            this.accountUserForm = new AccountUserForm();
            this.logwindowClear_button = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.objectConvNew_GetFileLst_CheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.GetFileList_button = new System.Windows.Forms.Button();
            this.SearchPath_textBox = new System.Windows.Forms.TextBox();
            this.ServerSourceFolderNaeme_textBox = new System.Windows.Forms.TextBox();
            this.groupBox4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.panel2);
            this.groupBox4.Controls.Add(this.panel1);
            this.groupBox4.Location = new System.Drawing.Point(3, 116);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(679, 283);
            this.groupBox4.TabIndex = 109;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "SendFileテスト";
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.panel2.Controls.Add(this.SetSamePathServer_button);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.ReceveFileObjectConvNew_checkBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.FileReceveStart_button);
            this.panel2.Controls.Add(this.FileRecvTest_Dist_Server_FullFileName_textBox);
            this.panel2.Controls.Add(this.FileRecvTest_Source_ServerFullFIleName_textBox);
            this.panel2.Location = new System.Drawing.Point(6, 160);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(667, 113);
            this.panel2.TabIndex = 117;
            // 
            // SetSamePathServer_button
            // 
            this.SetSamePathServer_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.SetSamePathServer_button.Location = new System.Drawing.Point(558, 54);
            this.SetSamePathServer_button.Name = "SetSamePathServer_button";
            this.SetSamePathServer_button.Size = new System.Drawing.Size(98, 23);
            this.SetSamePathServer_button.TabIndex = 153;
            this.SetSamePathServer_button.Text = "FileReceveStart";
            this.SetSamePathServer_button.UseVisualStyleBackColor = true;
            this.SetSamePathServer_button.Click += SetSamePathServer_button_Click_1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(295, 15);
            this.label5.TabIndex = 152;
            this.label5.Text = "■APIテスト 【サーバーからクライアントPCへファイルを受信する】";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 15);
            this.label4.TabIndex = 151;
            this.label4.Text = "ｸﾗｲｱﾝﾄ側ﾌｧｲﾙﾊﾟｽ(受信)";
            this.label4.Click += label4_Click;
            // 
            // ReceveFileObjectConvNew_checkBox
            // 
            this.ReceveFileObjectConvNew_checkBox.AutoSize = true;
            this.ReceveFileObjectConvNew_checkBox.Checked = true;
            this.ReceveFileObjectConvNew_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ReceveFileObjectConvNew_checkBox.Location = new System.Drawing.Point(11, 87);
            this.ReceveFileObjectConvNew_checkBox.Name = "ReceveFileObjectConvNew_checkBox";
            this.ReceveFileObjectConvNew_checkBox.Size = new System.Drawing.Size(110, 19);
            this.ReceveFileObjectConvNew_checkBox.TabIndex = 150;
            this.ReceveFileObjectConvNew_checkBox.Text = "objectConvNew";
            this.ReceveFileObjectConvNew_checkBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 15);
            this.label1.TabIndex = 108;
            this.label1.Text = "ｻｰﾊﾞｰ側ﾌｧｲﾙﾊﾟｽ(送信)";
            // 
            // FileReceveStart_button
            // 
            this.FileReceveStart_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.FileReceveStart_button.Location = new System.Drawing.Point(558, 83);
            this.FileReceveStart_button.Name = "FileReceveStart_button";
            this.FileReceveStart_button.Size = new System.Drawing.Size(98, 23);
            this.FileReceveStart_button.TabIndex = 0;
            this.FileReceveStart_button.Text = "FileReceveStart";
            this.FileReceveStart_button.UseVisualStyleBackColor = true;
            this.FileReceveStart_button.Click += FileReceveStart_button_Click;
            // 
            // FileRecvTest_Dist_Server_FullFileName_textBox
            // 
            this.FileRecvTest_Dist_Server_FullFileName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.FileRecvTest_Dist_Server_FullFileName_textBox.Location = new System.Drawing.Point(145, 53);
            this.FileRecvTest_Dist_Server_FullFileName_textBox.Name = "FileRecvTest_Dist_Server_FullFileName_textBox";
            this.FileRecvTest_Dist_Server_FullFileName_textBox.Size = new System.Drawing.Size(407, 23);
            this.FileRecvTest_Dist_Server_FullFileName_textBox.TabIndex = 107;
            // 
            // FileRecvTest_Source_ServerFullFIleName_textBox
            // 
            this.FileRecvTest_Source_ServerFullFIleName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.FileRecvTest_Source_ServerFullFIleName_textBox.Location = new System.Drawing.Point(145, 24);
            this.FileRecvTest_Source_ServerFullFIleName_textBox.Name = "FileRecvTest_Source_ServerFullFIleName_textBox";
            this.FileRecvTest_Source_ServerFullFIleName_textBox.Size = new System.Drawing.Size(511, 23);
            this.FileRecvTest_Source_ServerFullFIleName_textBox.TabIndex = 100;
            this.FileRecvTest_Source_ServerFullFIleName_textBox.TextChanged += ReceveFullFileName_textBox_TextChanged;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.FileSendWriteObjectConvNew_checkBox);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.SelectSouceFileName_button);
            this.panel1.Controls.Add(this.FileSendStart_button);
            this.panel1.Controls.Add(this.FileSendTest_Dist_ServerFuleFileName_textBox);
            this.panel1.Controls.Add(this.FileSendTest_Souce_LocalFullFileName_textBox);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Location = new System.Drawing.Point(6, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(667, 113);
            this.panel1.TabIndex = 116;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(295, 15);
            this.label3.TabIndex = 150;
            this.label3.Text = "■APIテスト 【クライアントからサーバーPCへファイルを送信する】";
            // 
            // FileSendWriteObjectConvNew_checkBox
            // 
            this.FileSendWriteObjectConvNew_checkBox.AutoSize = true;
            this.FileSendWriteObjectConvNew_checkBox.Checked = true;
            this.FileSendWriteObjectConvNew_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FileSendWriteObjectConvNew_checkBox.Location = new System.Drawing.Point(11, 91);
            this.FileSendWriteObjectConvNew_checkBox.Name = "FileSendWriteObjectConvNew_checkBox";
            this.FileSendWriteObjectConvNew_checkBox.Size = new System.Drawing.Size(110, 19);
            this.FileSendWriteObjectConvNew_checkBox.TabIndex = 149;
            this.FileSendWriteObjectConvNew_checkBox.Text = "objectConvNew";
            this.FileSendWriteObjectConvNew_checkBox.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(9, 27);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(129, 15);
            this.label17.TabIndex = 108;
            this.label17.Text = "ｸﾗｲｱﾝﾄ側ﾌｧｲﾙﾊﾟｽ(送信)";
            // 
            // SelectSouceFileName_button
            // 
            this.SelectSouceFileName_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.SelectSouceFileName_button.Location = new System.Drawing.Point(589, 22);
            this.SelectSouceFileName_button.Name = "SelectSouceFileName_button";
            this.SelectSouceFileName_button.Size = new System.Drawing.Size(67, 23);
            this.SelectSouceFileName_button.TabIndex = 101;
            this.SelectSouceFileName_button.Text = "FileSelect";
            this.SelectSouceFileName_button.UseVisualStyleBackColor = true;
            this.SelectSouceFileName_button.Click += SelectSouceFileName_button_Click;
            // 
            // FileSendStart_button
            // 
            this.FileSendStart_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.FileSendStart_button.Location = new System.Drawing.Point(558, 73);
            this.FileSendStart_button.Name = "FileSendStart_button";
            this.FileSendStart_button.Size = new System.Drawing.Size(98, 23);
            this.FileSendStart_button.TabIndex = 0;
            this.FileSendStart_button.Text = "FileSendStart";
            this.FileSendStart_button.UseVisualStyleBackColor = true;
            this.FileSendStart_button.Click += FileSendStart_button_Click;
            // 
            // FileSendTest_Dist_ServerFuleFileName_textBox
            // 
            this.FileSendTest_Dist_ServerFuleFileName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.FileSendTest_Dist_ServerFuleFileName_textBox.Location = new System.Drawing.Point(126, 49);
            this.FileSendTest_Dist_ServerFuleFileName_textBox.Name = "FileSendTest_Dist_ServerFuleFileName_textBox";
            this.FileSendTest_Dist_ServerFuleFileName_textBox.Size = new System.Drawing.Size(530, 23);
            this.FileSendTest_Dist_ServerFuleFileName_textBox.TabIndex = 107;
            this.FileSendTest_Dist_ServerFuleFileName_textBox.Text = "C:\\ProgramData\\TOYOCOMMON\\PIPE接続ファイル送受信テストデータ.txt";
            this.FileSendTest_Dist_ServerFuleFileName_textBox.TextChanged += FileSendTest_Dist_ServerFuleFileName_textBox_TextChanged;
            // 
            // FileSendTest_Souce_LocalFullFileName_textBox
            // 
            this.FileSendTest_Souce_LocalFullFileName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.FileSendTest_Souce_LocalFullFileName_textBox.Location = new System.Drawing.Point(147, 24);
            this.FileSendTest_Souce_LocalFullFileName_textBox.Name = "FileSendTest_Souce_LocalFullFileName_textBox";
            this.FileSendTest_Souce_LocalFullFileName_textBox.Size = new System.Drawing.Size(436, 23);
            this.FileSendTest_Souce_LocalFullFileName_textBox.TabIndex = 100;
            this.FileSendTest_Souce_LocalFullFileName_textBox.Text = "C:\\ProgramData\\TOYOCOMMON\\StageServerDatabaseConfig.XML";
            this.FileSendTest_Souce_LocalFullFileName_textBox.TextChanged += SourceFromLocalFullFileName_textBox_TextChanged;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 52);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(123, 15);
            this.label18.TabIndex = 109;
            this.label18.Text = "ｻｰﾊﾞｰ側ﾌｧｲﾙﾊﾟｽ(受信)";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.FieldValuseSets_JSONCONV_button);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.JsonTest_button);
            this.groupBox1.Location = new System.Drawing.Point(3, 405);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(679, 176);
            this.groupBox1.TabIndex = 137;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "サーバー利用可能メモリ取得";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(490, 83);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(183, 23);
            this.button4.TabIndex = 154;
            this.button4.Text = "FieldValuseSet_JSONCONV";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += button4_Click;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(287, 141);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(183, 23);
            this.button2.TabIndex = 153;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += button2_Click;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(287, 112);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(183, 23);
            this.button3.TabIndex = 152;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += button3_Click;
            // 
            // FieldValuseSets_JSONCONV_button
            // 
            this.FieldValuseSets_JSONCONV_button.Location = new System.Drawing.Point(287, 83);
            this.FieldValuseSets_JSONCONV_button.Name = "FieldValuseSets_JSONCONV_button";
            this.FieldValuseSets_JSONCONV_button.Size = new System.Drawing.Size(183, 23);
            this.FieldValuseSets_JSONCONV_button.TabIndex = 151;
            this.FieldValuseSets_JSONCONV_button.Text = "FieldValuseSets_JSONCONV";
            this.FieldValuseSets_JSONCONV_button.UseVisualStyleBackColor = true;
            this.FieldValuseSets_JSONCONV_button.Click += FieldValuseSets_JSONCONV_button_Click;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(287, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(183, 23);
            this.button1.TabIndex = 150;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += button1_Click;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.objectConvNew_checkBox);
            this.panel3.Controls.Add(this.GetAvailableMemory_button);
            this.panel3.Controls.Add(this.GetDRusedMemory_button);
            this.panel3.Controls.Add(this.GetSWusedMemory_button);
            this.panel3.Controls.Add(this.GetDCusedMemory_button);
            this.panel3.Location = new System.Drawing.Point(6, 18);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(275, 152);
            this.panel3.TabIndex = 149;
            // 
            // objectConvNew_checkBox
            // 
            this.objectConvNew_checkBox.AutoSize = true;
            this.objectConvNew_checkBox.Checked = true;
            this.objectConvNew_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.objectConvNew_checkBox.Location = new System.Drawing.Point(3, 3);
            this.objectConvNew_checkBox.Name = "objectConvNew_checkBox";
            this.objectConvNew_checkBox.Size = new System.Drawing.Size(110, 19);
            this.objectConvNew_checkBox.TabIndex = 148;
            this.objectConvNew_checkBox.Text = "objectConvNew";
            this.objectConvNew_checkBox.UseVisualStyleBackColor = true;
            // 
            // GetAvailableMemory_button
            // 
            this.GetAvailableMemory_button.Location = new System.Drawing.Point(11, 25);
            this.GetAvailableMemory_button.Name = "GetAvailableMemory_button";
            this.GetAvailableMemory_button.Size = new System.Drawing.Size(230, 23);
            this.GetAvailableMemory_button.TabIndex = 111;
            this.GetAvailableMemory_button.Text = "サーバー利用可能メモリ取得";
            this.GetAvailableMemory_button.UseVisualStyleBackColor = true;
            this.GetAvailableMemory_button.Click += GetAvailableMemory_button_Click;
            // 
            // GetDRusedMemory_button
            // 
            this.GetDRusedMemory_button.Location = new System.Drawing.Point(11, 54);
            this.GetDRusedMemory_button.Name = "GetDRusedMemory_button";
            this.GetDRusedMemory_button.Size = new System.Drawing.Size(230, 23);
            this.GetDRusedMemory_button.TabIndex = 112;
            this.GetDRusedMemory_button.Text = "DR使用中メモリ";
            this.GetDRusedMemory_button.UseVisualStyleBackColor = true;
            this.GetDRusedMemory_button.Click += GetDRusedMemory_button_Click;
            // 
            // GetSWusedMemory_button
            // 
            this.GetSWusedMemory_button.Location = new System.Drawing.Point(11, 112);
            this.GetSWusedMemory_button.Name = "GetSWusedMemory_button";
            this.GetSWusedMemory_button.Size = new System.Drawing.Size(230, 23);
            this.GetSWusedMemory_button.TabIndex = 115;
            this.GetSWusedMemory_button.Text = "SW使用中メモリ";
            this.GetSWusedMemory_button.UseVisualStyleBackColor = true;
            this.GetSWusedMemory_button.Click += GetSWusedMemory_button_Click;
            // 
            // GetDCusedMemory_button
            // 
            this.GetDCusedMemory_button.Location = new System.Drawing.Point(11, 83);
            this.GetDCusedMemory_button.Name = "GetDCusedMemory_button";
            this.GetDCusedMemory_button.Size = new System.Drawing.Size(230, 23);
            this.GetDCusedMemory_button.TabIndex = 113;
            this.GetDCusedMemory_button.Text = "DC使用中メモリ";
            this.GetDCusedMemory_button.UseVisualStyleBackColor = true;
            this.GetDCusedMemory_button.Click += GetDCusedMemory_button_Click;
            // 
            // JsonTest_button
            // 
            this.JsonTest_button.Location = new System.Drawing.Point(287, 21);
            this.JsonTest_button.Name = "JsonTest_button";
            this.JsonTest_button.Size = new System.Drawing.Size(183, 23);
            this.JsonTest_button.TabIndex = 116;
            this.JsonTest_button.Text = "JsonTest";
            this.JsonTest_button.UseVisualStyleBackColor = true;
            this.JsonTest_button.Click += JsonTest_button_Click;
            // 
            // logWindowControl
            // 
            this.logWindowControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.logWindowControl.Location = new System.Drawing.Point(689, 267);
            this.logWindowControl.Margin = new System.Windows.Forms.Padding(4);
            this.logWindowControl.Name = "logWindowControl";
            this.logWindowControl.Size = new System.Drawing.Size(562, 359);
            this.logWindowControl.TabIndex = 136;
            // 
            // accountUserForm
            // 
            this.accountUserForm.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.accountUserForm.Location = new System.Drawing.Point(0, 0);
            this.accountUserForm.Margin = new System.Windows.Forms.Padding(4);
            this.accountUserForm.Name = "accountUserForm";
            this.accountUserForm.parentControl = null;
            this.accountUserForm.Size = new System.Drawing.Size(1051, 110);
            this.accountUserForm.TabIndex = 135;
            this.accountUserForm.WriteLine = null;
            this.accountUserForm.Paint += accountUserForm1_Paint;
            this.accountUserForm.Leave += accountUserForm_Leave;
            // 
            // logwindowClear_button
            // 
            this.logwindowClear_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.logwindowClear_button.Location = new System.Drawing.Point(1165, 633);
            this.logwindowClear_button.Name = "logwindowClear_button";
            this.logwindowClear_button.Size = new System.Drawing.Size(75, 23);
            this.logwindowClear_button.TabIndex = 138;
            this.logwindowClear_button.Text = "区切り線";
            this.logwindowClear_button.UseVisualStyleBackColor = true;
            this.logwindowClear_button.Click += logwindowClear_button_Click;
            // 
            // panel4
            // 
            this.panel4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.panel4.Controls.Add(this.objectConvNew_GetFileLst_CheckBox);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.GetFileList_button);
            this.panel4.Controls.Add(this.SearchPath_textBox);
            this.panel4.Controls.Add(this.ServerSourceFolderNaeme_textBox);
            this.panel4.Location = new System.Drawing.Point(689, 140);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(551, 113);
            this.panel4.TabIndex = 139;
            // 
            // objectConvNew_GetFileLst_CheckBox
            // 
            this.objectConvNew_GetFileLst_CheckBox.AutoSize = true;
            this.objectConvNew_GetFileLst_CheckBox.Checked = true;
            this.objectConvNew_GetFileLst_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.objectConvNew_GetFileLst_CheckBox.Location = new System.Drawing.Point(11, 3);
            this.objectConvNew_GetFileLst_CheckBox.Name = "objectConvNew_GetFileLst_CheckBox";
            this.objectConvNew_GetFileLst_CheckBox.Size = new System.Drawing.Size(110, 19);
            this.objectConvNew_GetFileLst_CheckBox.TabIndex = 150;
            this.objectConvNew_GetFileLst_CheckBox.Text = "objectConvNew";
            this.objectConvNew_GetFileLst_CheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 15);
            this.label2.TabIndex = 108;
            this.label2.Text = "ReceveFullFileName";
            // 
            // GetFileList_button
            // 
            this.GetFileList_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.GetFileList_button.Location = new System.Drawing.Point(442, 73);
            this.GetFileList_button.Name = "GetFileList_button";
            this.GetFileList_button.Size = new System.Drawing.Size(98, 23);
            this.GetFileList_button.TabIndex = 0;
            this.GetFileList_button.Text = "GetFileList";
            this.GetFileList_button.UseVisualStyleBackColor = true;
            this.GetFileList_button.Click += GetFileList_button_Click;
            // 
            // SearchPath_textBox
            // 
            this.SearchPath_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.SearchPath_textBox.Location = new System.Drawing.Point(126, 49);
            this.SearchPath_textBox.Name = "SearchPath_textBox";
            this.SearchPath_textBox.Size = new System.Drawing.Size(414, 23);
            this.SearchPath_textBox.TabIndex = 107;
            this.SearchPath_textBox.Text = "*.*";
            // 
            // ServerSourceFolderNaeme_textBox
            // 
            this.ServerSourceFolderNaeme_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.ServerSourceFolderNaeme_textBox.Location = new System.Drawing.Point(126, 24);
            this.ServerSourceFolderNaeme_textBox.Name = "ServerSourceFolderNaeme_textBox";
            this.ServerSourceFolderNaeme_textBox.Size = new System.Drawing.Size(414, 23);
            this.ServerSourceFolderNaeme_textBox.TabIndex = 100;
            this.ServerSourceFolderNaeme_textBox.Text = "C:\\TOYOSVC";
            // 
            // TabControl07
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            Controls.Add(this.panel4);
            Controls.Add(this.logwindowClear_button);
            Controls.Add(this.groupBox1);
            Controls.Add(this.logWindowControl);
            Controls.Add(this.accountUserForm);
            Controls.Add(this.groupBox4);
            Name = "TabControl07";
            Size = new System.Drawing.Size(1254, 669);
            Load += TabControl07_Load;
            VisibleChanged += TabControl07_VisibleChanged;
            this.groupBox4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        internal System.Windows.Forms.TextBox FileSendTest_Souce_LocalFullFileName_textBox;
        internal System.Windows.Forms.TextBox FileSendTest_Dist_ServerFuleFileName_textBox;
        private System.Windows.Forms.Button FileSendStart_button;
        private System.Windows.Forms.Button SelectSouceFileName_button;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button FileReceveStart_button;
        internal System.Windows.Forms.TextBox FileRecvTest_Dist_Server_FullFileName_textBox;
        internal System.Windows.Forms.TextBox FileRecvTest_Source_ServerFullFIleName_textBox;
        private System.Windows.Forms.Panel panel1;
        internal AccountUserForm accountUserForm;
        private LogWindowControl logWindowControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button GetAvailableMemory_button;
        private System.Windows.Forms.Button GetDRusedMemory_button;
        private System.Windows.Forms.Button GetDCusedMemory_button;
        private System.Windows.Forms.Button GetSWusedMemory_button;
        private System.Windows.Forms.Button JsonTest_button;
        private System.Windows.Forms.CheckBox objectConvNew_checkBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox FileSendWriteObjectConvNew_checkBox;
        private System.Windows.Forms.CheckBox ReceveFileObjectConvNew_checkBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button FieldValuseSets_JSONCONV_button;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button logwindowClear_button;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.CheckBox objectConvNew_GetFileLst_CheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button GetFileList_button;
        internal System.Windows.Forms.TextBox SearchPath_textBox;
        internal System.Windows.Forms.TextBox ServerSourceFolderNaeme_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button SetSamePathServer_button;
    }
}
