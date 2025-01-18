
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
            components = new System.ComponentModel.Container();
            groupBox4 = new System.Windows.Forms.GroupBox();
            panel2 = new System.Windows.Forms.Panel();
            SetSamePathServer_button = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            ReceveFileObjectConvNew_checkBox = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            FileReceveStart_button = new System.Windows.Forms.Button();
            FileRecvTest_Dist_Server_FullFileName_textBox = new System.Windows.Forms.TextBox();
            FileRecvTest_Source_ServerFullFIleName_textBox = new System.Windows.Forms.TextBox();
            panel1 = new System.Windows.Forms.Panel();
            label3 = new System.Windows.Forms.Label();
            FileSendWriteObjectConvNew_checkBox = new System.Windows.Forms.CheckBox();
            label17 = new System.Windows.Forms.Label();
            SelectSouceFileName_button = new System.Windows.Forms.Button();
            FileSendStart_button = new System.Windows.Forms.Button();
            FileSendTest_Dist_ServerFuleFileName_textBox = new System.Windows.Forms.TextBox();
            FileSendTest_Souce_LocalFullFileName_textBox = new System.Windows.Forms.TextBox();
            label18 = new System.Windows.Forms.Label();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            groupBox1 = new System.Windows.Forms.GroupBox();
            Object_pictureBox = new System.Windows.Forms.PictureBox();
            FromObject_to_JSON_button = new System.Windows.Forms.Button();
            FromJSON_to_Object_button = new System.Windows.Forms.Button();
            JSONSTR_textBox = new System.Windows.Forms.TextBox();
            button4 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            FieldValuseSets_JSONCONV_button = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            panel3 = new System.Windows.Forms.Panel();
            objectConvNew_checkBox = new System.Windows.Forms.CheckBox();
            GetAvailableMemory_button = new System.Windows.Forms.Button();
            GetDRusedMemory_button = new System.Windows.Forms.Button();
            GetSWusedMemory_button = new System.Windows.Forms.Button();
            GetDCusedMemory_button = new System.Windows.Forms.Button();
            JsonTest_button = new System.Windows.Forms.Button();
            logWindowControl = new LogWindowControl();
            accountUserForm = new AccountUserForm();
            logwindowClear_button = new System.Windows.Forms.Button();
            panel4 = new System.Windows.Forms.Panel();
            objectConvNew_GetFileLst_CheckBox = new System.Windows.Forms.CheckBox();
            label2 = new System.Windows.Forms.Label();
            GetFileList_button = new System.Windows.Forms.Button();
            SearchPath_textBox = new System.Windows.Forms.TextBox();
            ServerSourceFolderNaeme_textBox = new System.Windows.Forms.TextBox();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            object_textBox = new System.Windows.Forms.TextBox();
            groupBox4.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Object_pictureBox).BeginInit();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(panel2);
            groupBox4.Controls.Add(panel1);
            groupBox4.Location = new System.Drawing.Point(3, 116);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(679, 283);
            groupBox4.TabIndex = 109;
            groupBox4.TabStop = false;
            groupBox4.Text = "SendFileテスト";
            // 
            // panel2
            // 
            panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel2.Controls.Add(SetSamePathServer_button);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(ReceveFileObjectConvNew_checkBox);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(FileReceveStart_button);
            panel2.Controls.Add(FileRecvTest_Dist_Server_FullFileName_textBox);
            panel2.Controls.Add(FileRecvTest_Source_ServerFullFIleName_textBox);
            panel2.Location = new System.Drawing.Point(6, 160);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(667, 113);
            panel2.TabIndex = 117;
            // 
            // SetSamePathServer_button
            // 
            SetSamePathServer_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            SetSamePathServer_button.Location = new System.Drawing.Point(558, 54);
            SetSamePathServer_button.Name = "SetSamePathServer_button";
            SetSamePathServer_button.Size = new System.Drawing.Size(98, 23);
            SetSamePathServer_button.TabIndex = 153;
            SetSamePathServer_button.Text = "FileSelect";
            SetSamePathServer_button.UseVisualStyleBackColor = true;
            SetSamePathServer_button.Click += SetSamePathServer_button_Click_1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(9, 6);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(295, 15);
            label5.TabIndex = 152;
            label5.Text = "■APIテスト 【サーバーからクライアントPCへファイルを受信する】";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(10, 53);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(129, 15);
            label4.TabIndex = 151;
            label4.Text = "ｸﾗｲｱﾝﾄ側ﾌｧｲﾙﾊﾟｽ(受信)";
            label4.Click += label4_Click;
            // 
            // ReceveFileObjectConvNew_checkBox
            // 
            ReceveFileObjectConvNew_checkBox.AutoSize = true;
            ReceveFileObjectConvNew_checkBox.Checked = true;
            ReceveFileObjectConvNew_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            ReceveFileObjectConvNew_checkBox.Location = new System.Drawing.Point(11, 87);
            ReceveFileObjectConvNew_checkBox.Name = "ReceveFileObjectConvNew_checkBox";
            ReceveFileObjectConvNew_checkBox.Size = new System.Drawing.Size(110, 19);
            ReceveFileObjectConvNew_checkBox.TabIndex = 150;
            ReceveFileObjectConvNew_checkBox.Text = "objectConvNew";
            ReceveFileObjectConvNew_checkBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 27);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(123, 15);
            label1.TabIndex = 108;
            label1.Text = "ｻｰﾊﾞｰ側ﾌｧｲﾙﾊﾟｽ(送信)";
            // 
            // FileReceveStart_button
            // 
            FileReceveStart_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            FileReceveStart_button.Location = new System.Drawing.Point(558, 83);
            FileReceveStart_button.Name = "FileReceveStart_button";
            FileReceveStart_button.Size = new System.Drawing.Size(98, 23);
            FileReceveStart_button.TabIndex = 0;
            FileReceveStart_button.Text = "FileReceveStart";
            toolTip1.SetToolTip(FileReceveStart_button, "DC_ServerControl_FileRecv , DC_ServerControl_FileRecv_V2");
            FileReceveStart_button.UseVisualStyleBackColor = true;
            FileReceveStart_button.Click += FileReceveStart_button_Click;
            // 
            // FileRecvTest_Dist_Server_FullFileName_textBox
            // 
            FileRecvTest_Dist_Server_FullFileName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FileRecvTest_Dist_Server_FullFileName_textBox.Location = new System.Drawing.Point(145, 53);
            FileRecvTest_Dist_Server_FullFileName_textBox.Name = "FileRecvTest_Dist_Server_FullFileName_textBox";
            FileRecvTest_Dist_Server_FullFileName_textBox.Size = new System.Drawing.Size(407, 23);
            FileRecvTest_Dist_Server_FullFileName_textBox.TabIndex = 107;
            // 
            // FileRecvTest_Source_ServerFullFIleName_textBox
            // 
            FileRecvTest_Source_ServerFullFIleName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FileRecvTest_Source_ServerFullFIleName_textBox.Location = new System.Drawing.Point(145, 24);
            FileRecvTest_Source_ServerFullFIleName_textBox.Name = "FileRecvTest_Source_ServerFullFIleName_textBox";
            FileRecvTest_Source_ServerFullFIleName_textBox.Size = new System.Drawing.Size(511, 23);
            FileRecvTest_Source_ServerFullFIleName_textBox.TabIndex = 100;
            FileRecvTest_Source_ServerFullFIleName_textBox.TextChanged += ReceveFullFileName_textBox_TextChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(label3);
            panel1.Controls.Add(FileSendWriteObjectConvNew_checkBox);
            panel1.Controls.Add(label17);
            panel1.Controls.Add(SelectSouceFileName_button);
            panel1.Controls.Add(FileSendStart_button);
            panel1.Controls.Add(FileSendTest_Dist_ServerFuleFileName_textBox);
            panel1.Controls.Add(FileSendTest_Souce_LocalFullFileName_textBox);
            panel1.Controls.Add(label18);
            panel1.Location = new System.Drawing.Point(6, 41);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(667, 113);
            panel1.TabIndex = 116;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(5, 4);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(295, 15);
            label3.TabIndex = 150;
            label3.Text = "■APIテスト 【クライアントからサーバーPCへファイルを送信する】";
            // 
            // FileSendWriteObjectConvNew_checkBox
            // 
            FileSendWriteObjectConvNew_checkBox.AutoSize = true;
            FileSendWriteObjectConvNew_checkBox.Checked = true;
            FileSendWriteObjectConvNew_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            FileSendWriteObjectConvNew_checkBox.Location = new System.Drawing.Point(11, 91);
            FileSendWriteObjectConvNew_checkBox.Name = "FileSendWriteObjectConvNew_checkBox";
            FileSendWriteObjectConvNew_checkBox.Size = new System.Drawing.Size(110, 19);
            FileSendWriteObjectConvNew_checkBox.TabIndex = 149;
            FileSendWriteObjectConvNew_checkBox.Text = "objectConvNew";
            FileSendWriteObjectConvNew_checkBox.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(9, 27);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(129, 15);
            label17.TabIndex = 108;
            label17.Text = "ｸﾗｲｱﾝﾄ側ﾌｧｲﾙﾊﾟｽ(送信)";
            // 
            // SelectSouceFileName_button
            // 
            SelectSouceFileName_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            SelectSouceFileName_button.Location = new System.Drawing.Point(589, 22);
            SelectSouceFileName_button.Name = "SelectSouceFileName_button";
            SelectSouceFileName_button.Size = new System.Drawing.Size(67, 23);
            SelectSouceFileName_button.TabIndex = 101;
            SelectSouceFileName_button.Text = "FileSelect";
            SelectSouceFileName_button.UseVisualStyleBackColor = true;
            SelectSouceFileName_button.Click += SelectSouceFileName_button_Click;
            // 
            // FileSendStart_button
            // 
            FileSendStart_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            FileSendStart_button.Location = new System.Drawing.Point(558, 73);
            FileSendStart_button.Name = "FileSendStart_button";
            FileSendStart_button.Size = new System.Drawing.Size(98, 23);
            FileSendStart_button.TabIndex = 0;
            FileSendStart_button.Text = "FileSendStart";
            toolTip1.SetToolTip(FileSendStart_button, "ServerControl_FileSend , ServerControl_FileSend_V2");
            FileSendStart_button.UseVisualStyleBackColor = true;
            FileSendStart_button.Click += FileSendStart_button_Click;
            // 
            // FileSendTest_Dist_ServerFuleFileName_textBox
            // 
            FileSendTest_Dist_ServerFuleFileName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FileSendTest_Dist_ServerFuleFileName_textBox.Location = new System.Drawing.Point(126, 49);
            FileSendTest_Dist_ServerFuleFileName_textBox.Name = "FileSendTest_Dist_ServerFuleFileName_textBox";
            FileSendTest_Dist_ServerFuleFileName_textBox.Size = new System.Drawing.Size(530, 23);
            FileSendTest_Dist_ServerFuleFileName_textBox.TabIndex = 107;
            FileSendTest_Dist_ServerFuleFileName_textBox.Text = "C:\\ProgramData\\TOYOCOMMON\\PIPE接続ファイル送受信テストデータ.txt";
            FileSendTest_Dist_ServerFuleFileName_textBox.TextChanged += FileSendTest_Dist_ServerFuleFileName_textBox_TextChanged;
            // 
            // FileSendTest_Souce_LocalFullFileName_textBox
            // 
            FileSendTest_Souce_LocalFullFileName_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FileSendTest_Souce_LocalFullFileName_textBox.Location = new System.Drawing.Point(147, 24);
            FileSendTest_Souce_LocalFullFileName_textBox.Name = "FileSendTest_Souce_LocalFullFileName_textBox";
            FileSendTest_Souce_LocalFullFileName_textBox.Size = new System.Drawing.Size(436, 23);
            FileSendTest_Souce_LocalFullFileName_textBox.TabIndex = 100;
            FileSendTest_Souce_LocalFullFileName_textBox.Text = "C:\\ProgramData\\TOYOCOMMON\\StageServerDatabaseConfig.XML";
            FileSendTest_Souce_LocalFullFileName_textBox.TextChanged += SourceFromLocalFullFileName_textBox_TextChanged;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new System.Drawing.Point(9, 52);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(123, 15);
            label18.TabIndex = 109;
            label18.Text = "ｻｰﾊﾞｰ側ﾌｧｲﾙﾊﾟｽ(受信)";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(Object_pictureBox);
            groupBox1.Controls.Add(FromObject_to_JSON_button);
            groupBox1.Controls.Add(FromJSON_to_Object_button);
            groupBox1.Controls.Add(JSONSTR_textBox);
            groupBox1.Controls.Add(button4);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(FieldValuseSets_JSONCONV_button);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(panel3);
            groupBox1.Controls.Add(JsonTest_button);
            groupBox1.Location = new System.Drawing.Point(3, 405);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(679, 251);
            groupBox1.TabIndex = 137;
            groupBox1.TabStop = false;
            groupBox1.Text = "サーバー利用可能メモリ取得";
            // 
            // Object_pictureBox
            // 
            Object_pictureBox.Location = new System.Drawing.Point(573, 170);
            Object_pictureBox.Name = "Object_pictureBox";
            Object_pictureBox.Size = new System.Drawing.Size(100, 69);
            Object_pictureBox.TabIndex = 158;
            Object_pictureBox.TabStop = false;
            // 
            // FromObject_to_JSON_button
            // 
            FromObject_to_JSON_button.Location = new System.Drawing.Point(316, 217);
            FromObject_to_JSON_button.Name = "FromObject_to_JSON_button";
            FromObject_to_JSON_button.Size = new System.Drawing.Size(138, 23);
            FromObject_to_JSON_button.TabIndex = 157;
            FromObject_to_JSON_button.Text = "←オブジェクトからJSON";
            FromObject_to_JSON_button.UseVisualStyleBackColor = true;
            FromObject_to_JSON_button.Click += FromObject_to_JSON_button_Click;
            // 
            // FromJSON_to_Object_button
            // 
            FromJSON_to_Object_button.Location = new System.Drawing.Point(316, 176);
            FromJSON_to_Object_button.Name = "FromJSON_to_Object_button";
            FromJSON_to_Object_button.Size = new System.Drawing.Size(138, 23);
            FromJSON_to_Object_button.TabIndex = 156;
            FromJSON_to_Object_button.Text = "JSOｎからオブジェクト→";
            FromJSON_to_Object_button.UseVisualStyleBackColor = true;
            FromJSON_to_Object_button.Click += FromJSON_to_Object_button_Click;
            // 
            // JSONSTR_textBox
            // 
            JSONSTR_textBox.Location = new System.Drawing.Point(6, 176);
            JSONSTR_textBox.Multiline = true;
            JSONSTR_textBox.Name = "JSONSTR_textBox";
            JSONSTR_textBox.Size = new System.Drawing.Size(304, 69);
            JSONSTR_textBox.TabIndex = 155;
            // 
            // button4
            // 
            button4.Location = new System.Drawing.Point(490, 83);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(183, 23);
            button4.TabIndex = 154;
            button4.Text = "FieldValuseSet_JSONCONV";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(287, 141);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(183, 23);
            button2.TabIndex = 153;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(287, 112);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(183, 23);
            button3.TabIndex = 152;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // FieldValuseSets_JSONCONV_button
            // 
            FieldValuseSets_JSONCONV_button.Location = new System.Drawing.Point(287, 83);
            FieldValuseSets_JSONCONV_button.Name = "FieldValuseSets_JSONCONV_button";
            FieldValuseSets_JSONCONV_button.Size = new System.Drawing.Size(183, 23);
            FieldValuseSets_JSONCONV_button.TabIndex = 151;
            FieldValuseSets_JSONCONV_button.Text = "FieldValuseSets_JSONCONV";
            FieldValuseSets_JSONCONV_button.UseVisualStyleBackColor = true;
            FieldValuseSets_JSONCONV_button.Click += FieldValuseSets_JSONCONV_button_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(287, 54);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(183, 23);
            button1.TabIndex = 150;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(objectConvNew_checkBox);
            panel3.Controls.Add(GetAvailableMemory_button);
            panel3.Controls.Add(GetDRusedMemory_button);
            panel3.Controls.Add(GetSWusedMemory_button);
            panel3.Controls.Add(GetDCusedMemory_button);
            panel3.Location = new System.Drawing.Point(6, 18);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(275, 152);
            panel3.TabIndex = 149;
            // 
            // objectConvNew_checkBox
            // 
            objectConvNew_checkBox.AutoSize = true;
            objectConvNew_checkBox.Checked = true;
            objectConvNew_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            objectConvNew_checkBox.Location = new System.Drawing.Point(3, 3);
            objectConvNew_checkBox.Name = "objectConvNew_checkBox";
            objectConvNew_checkBox.Size = new System.Drawing.Size(110, 19);
            objectConvNew_checkBox.TabIndex = 148;
            objectConvNew_checkBox.Text = "objectConvNew";
            objectConvNew_checkBox.UseVisualStyleBackColor = true;
            // 
            // GetAvailableMemory_button
            // 
            GetAvailableMemory_button.Location = new System.Drawing.Point(11, 25);
            GetAvailableMemory_button.Name = "GetAvailableMemory_button";
            GetAvailableMemory_button.Size = new System.Drawing.Size(230, 23);
            GetAvailableMemory_button.TabIndex = 111;
            GetAvailableMemory_button.Text = "サーバー利用可能メモリ取得";
            toolTip1.SetToolTip(GetAvailableMemory_button, "GetAvailableMemory , GetAvailableMemory_V2");
            GetAvailableMemory_button.UseVisualStyleBackColor = true;
            GetAvailableMemory_button.Click += GetAvailableMemory_button_Click;
            // 
            // GetDRusedMemory_button
            // 
            GetDRusedMemory_button.Location = new System.Drawing.Point(11, 54);
            GetDRusedMemory_button.Name = "GetDRusedMemory_button";
            GetDRusedMemory_button.Size = new System.Drawing.Size(230, 23);
            GetDRusedMemory_button.TabIndex = 112;
            GetDRusedMemory_button.Text = "DR使用中メモリ";
            toolTip1.SetToolTip(GetDRusedMemory_button, "GetMemoryUsageWorkingSetSiz , GetMemoryUsageWorkingSetSiz_V2");
            GetDRusedMemory_button.UseVisualStyleBackColor = true;
            GetDRusedMemory_button.Click += GetDRusedMemory_button_Click;
            // 
            // GetSWusedMemory_button
            // 
            GetSWusedMemory_button.Location = new System.Drawing.Point(11, 112);
            GetSWusedMemory_button.Name = "GetSWusedMemory_button";
            GetSWusedMemory_button.Size = new System.Drawing.Size(230, 23);
            GetSWusedMemory_button.TabIndex = 115;
            GetSWusedMemory_button.Text = "SW使用中メモリ";
            toolTip1.SetToolTip(GetSWusedMemory_button, "GetMemoryUsageWorkingSetSiz , GetMemoryUsageWorkingSetSiz_V2");
            GetSWusedMemory_button.UseVisualStyleBackColor = true;
            GetSWusedMemory_button.Click += GetSWusedMemory_button_Click;
            // 
            // GetDCusedMemory_button
            // 
            GetDCusedMemory_button.Location = new System.Drawing.Point(11, 83);
            GetDCusedMemory_button.Name = "GetDCusedMemory_button";
            GetDCusedMemory_button.Size = new System.Drawing.Size(230, 23);
            GetDCusedMemory_button.TabIndex = 113;
            GetDCusedMemory_button.Text = "DC使用中メモリ";
            toolTip1.SetToolTip(GetDCusedMemory_button, "GetMemoryUsageWorkingSetSiz , GetMemoryUsageWorkingSetSiz_V2");
            GetDCusedMemory_button.UseVisualStyleBackColor = true;
            GetDCusedMemory_button.Click += GetDCusedMemory_button_Click;
            // 
            // JsonTest_button
            // 
            JsonTest_button.Location = new System.Drawing.Point(287, 21);
            JsonTest_button.Name = "JsonTest_button";
            JsonTest_button.Size = new System.Drawing.Size(375, 23);
            JsonTest_button.TabIndex = 116;
            JsonTest_button.Text = "JsonTest(SqlFieldValuexConverter)";
            JsonTest_button.UseVisualStyleBackColor = true;
            JsonTest_button.Click += JsonTest_button_Click;
            // 
            // logWindowControl
            // 
            logWindowControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            logWindowControl.Location = new System.Drawing.Point(689, 267);
            logWindowControl.Margin = new System.Windows.Forms.Padding(4);
            logWindowControl.Name = "logWindowControl";
            logWindowControl.Size = new System.Drawing.Size(639, 359);
            logWindowControl.TabIndex = 136;
            // 
            // accountUserForm
            // 
            accountUserForm.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            accountUserForm.Location = new System.Drawing.Point(0, 0);
            accountUserForm.Margin = new System.Windows.Forms.Padding(4);
            accountUserForm.Name = "accountUserForm";
            accountUserForm.parentControl = null;
            accountUserForm.Size = new System.Drawing.Size(1128, 110);
            accountUserForm.TabIndex = 135;
            accountUserForm.WriteLine = null;
            accountUserForm.Paint += accountUserForm1_Paint;
            accountUserForm.Leave += accountUserForm_Leave;
            // 
            // logwindowClear_button
            // 
            logwindowClear_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            logwindowClear_button.Location = new System.Drawing.Point(1242, 633);
            logwindowClear_button.Name = "logwindowClear_button";
            logwindowClear_button.Size = new System.Drawing.Size(75, 23);
            logwindowClear_button.TabIndex = 138;
            logwindowClear_button.Text = "区切り線";
            logwindowClear_button.UseVisualStyleBackColor = true;
            logwindowClear_button.Click += logwindowClear_button_Click;
            // 
            // panel4
            // 
            panel4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel4.Controls.Add(objectConvNew_GetFileLst_CheckBox);
            panel4.Controls.Add(label2);
            panel4.Controls.Add(GetFileList_button);
            panel4.Controls.Add(SearchPath_textBox);
            panel4.Controls.Add(ServerSourceFolderNaeme_textBox);
            panel4.Location = new System.Drawing.Point(689, 140);
            panel4.Name = "panel4";
            panel4.Size = new System.Drawing.Size(628, 113);
            panel4.TabIndex = 139;
            // 
            // objectConvNew_GetFileLst_CheckBox
            // 
            objectConvNew_GetFileLst_CheckBox.AutoSize = true;
            objectConvNew_GetFileLst_CheckBox.Checked = true;
            objectConvNew_GetFileLst_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            objectConvNew_GetFileLst_CheckBox.Location = new System.Drawing.Point(11, 3);
            objectConvNew_GetFileLst_CheckBox.Name = "objectConvNew_GetFileLst_CheckBox";
            objectConvNew_GetFileLst_CheckBox.Size = new System.Drawing.Size(110, 19);
            objectConvNew_GetFileLst_CheckBox.TabIndex = 150;
            objectConvNew_GetFileLst_CheckBox.Text = "objectConvNew";
            objectConvNew_GetFileLst_CheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(9, 27);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(111, 15);
            label2.TabIndex = 108;
            label2.Text = "ReceveFullFileName";
            // 
            // GetFileList_button
            // 
            GetFileList_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            GetFileList_button.Location = new System.Drawing.Point(519, 73);
            GetFileList_button.Name = "GetFileList_button";
            GetFileList_button.Size = new System.Drawing.Size(98, 23);
            GetFileList_button.TabIndex = 0;
            GetFileList_button.Text = "GetFileList";
            GetFileList_button.UseVisualStyleBackColor = true;
            GetFileList_button.Click += GetFileList_button_Click;
            // 
            // SearchPath_textBox
            // 
            SearchPath_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            SearchPath_textBox.Location = new System.Drawing.Point(126, 49);
            SearchPath_textBox.Name = "SearchPath_textBox";
            SearchPath_textBox.Size = new System.Drawing.Size(491, 23);
            SearchPath_textBox.TabIndex = 107;
            SearchPath_textBox.Text = "*.*";
            // 
            // ServerSourceFolderNaeme_textBox
            // 
            ServerSourceFolderNaeme_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ServerSourceFolderNaeme_textBox.Location = new System.Drawing.Point(126, 24);
            ServerSourceFolderNaeme_textBox.Name = "ServerSourceFolderNaeme_textBox";
            ServerSourceFolderNaeme_textBox.Size = new System.Drawing.Size(491, 23);
            ServerSourceFolderNaeme_textBox.TabIndex = 100;
            ServerSourceFolderNaeme_textBox.Text = "C:\\TOYOSVC";
            // 
            // object_textBox
            // 
            object_textBox.Location = new System.Drawing.Point(463, 575);
            object_textBox.Multiline = true;
            object_textBox.Name = "object_textBox";
            object_textBox.Size = new System.Drawing.Size(107, 69);
            object_textBox.TabIndex = 158;
            // 
            // TabControl07
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            Controls.Add(object_textBox);
            Controls.Add(panel4);
            Controls.Add(logwindowClear_button);
            Controls.Add(groupBox1);
            Controls.Add(logWindowControl);
            Controls.Add(accountUserForm);
            Controls.Add(groupBox4);
            Name = "TabControl07";
            Size = new System.Drawing.Size(1331, 669);
            Load += TabControl07_Load;
            VisibleChanged += TabControl07_VisibleChanged;
            groupBox4.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Object_pictureBox).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox JSONSTR_textBox;
        private System.Windows.Forms.Button FromObject_to_JSON_button;
        private System.Windows.Forms.Button FromJSON_to_Object_button;
        private System.Windows.Forms.TextBox object_textBox;
        private System.Windows.Forms.PictureBox Object_pictureBox;
    }
}
