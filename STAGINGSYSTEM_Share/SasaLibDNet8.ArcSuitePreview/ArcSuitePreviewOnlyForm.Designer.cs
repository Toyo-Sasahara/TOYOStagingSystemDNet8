
namespace SasaLib.ArcSuitePreview
{
    partial class ArcSuitePreviewOnlyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            PreviewPanel = new Panel();
            ArcSuite_Status_label = new Label();
            Debug_panel = new Panel();
            FindTimeStamp_label = new Label();
            SCALE_numericUpDown = new NumericUpDown();
            Y_numericUpDown = new NumericUpDown();
            X_numericUpDown = new NumericUpDown();
            button1 = new Button();
            label2 = new Label();
            lblDst = new Label();
            label1 = new Label();
            ArcsuitePreviewForm_Msg_label = new Label();
            ArcSuitePreviewPictureBox = new PictureBox();
            groupBox1 = new GroupBox();
            DrawingInfoLabel3 = new Label();
            DrawingInfoLabel2 = new Label();
            modelcreationonorder_label = new Label();
            DrawingInfoLabel4 = new Label();
            ArcSuiteCreatedOn_label = new Label();
            TitleBlockScale_button = new Button();
            Hide_button = new Button();
            ArcSuiteWebSearchAndView_button = new Button();
            toolTip1 = new ToolTip(components);
            ClipBoardTextSearch_button = new Button();
            label4 = new Label();
            PreviewPanel.SuspendLayout();
            Debug_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SCALE_numericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Y_numericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)X_numericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ArcSuitePreviewPictureBox).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // PreviewPanel
            // 
            PreviewPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PreviewPanel.Controls.Add(label4);
            PreviewPanel.Controls.Add(ArcSuite_Status_label);
            PreviewPanel.Controls.Add(Debug_panel);
            PreviewPanel.Controls.Add(ArcsuitePreviewForm_Msg_label);
            PreviewPanel.Controls.Add(ArcSuitePreviewPictureBox);
            PreviewPanel.Location = new Point(10, 11);
            PreviewPanel.Margin = new Padding(0);
            PreviewPanel.Name = "PreviewPanel";
            PreviewPanel.Size = new Size(499, 422);
            PreviewPanel.TabIndex = 12;
            // 
            // ArcSuite_Status_label
            // 
            ArcSuite_Status_label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ArcSuite_Status_label.BackColor = Color.Transparent;
            ArcSuite_Status_label.Enabled = false;
            ArcSuite_Status_label.Font = new Font("Microsoft Sans Serif", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            ArcSuite_Status_label.ForeColor = Color.Red;
            ArcSuite_Status_label.Location = new Point(76, 279);
            ArcSuite_Status_label.Margin = new Padding(4, 0, 4, 0);
            ArcSuite_Status_label.Name = "ArcSuite_Status_label";
            ArcSuite_Status_label.Size = new Size(379, 80);
            ArcSuite_Status_label.TabIndex = 2;
            ArcSuite_Status_label.Text = "...";
            ArcSuite_Status_label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Debug_panel
            // 
            Debug_panel.Controls.Add(FindTimeStamp_label);
            Debug_panel.Controls.Add(SCALE_numericUpDown);
            Debug_panel.Controls.Add(Y_numericUpDown);
            Debug_panel.Controls.Add(X_numericUpDown);
            Debug_panel.Controls.Add(button1);
            Debug_panel.Controls.Add(label2);
            Debug_panel.Controls.Add(lblDst);
            Debug_panel.Controls.Add(label1);
            Debug_panel.Location = new Point(14, 142);
            Debug_panel.Margin = new Padding(4);
            Debug_panel.Name = "Debug_panel";
            Debug_panel.Size = new Size(467, 120);
            Debug_panel.TabIndex = 21;
            // 
            // FindTimeStamp_label
            // 
            FindTimeStamp_label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            FindTimeStamp_label.Font = new Font("MS UI Gothic", 9F);
            FindTimeStamp_label.Location = new Point(8, 91);
            FindTimeStamp_label.Margin = new Padding(4, 0, 4, 0);
            FindTimeStamp_label.Name = "FindTimeStamp_label";
            FindTimeStamp_label.Size = new Size(258, 24);
            FindTimeStamp_label.TabIndex = 34;
            FindTimeStamp_label.Text = "-";
            // 
            // SCALE_numericUpDown
            // 
            SCALE_numericUpDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SCALE_numericUpDown.DecimalPlaces = 3;
            SCALE_numericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
            SCALE_numericUpDown.Location = new Point(401, 62);
            SCALE_numericUpDown.Margin = new Padding(4);
            SCALE_numericUpDown.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            SCALE_numericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 196608 });
            SCALE_numericUpDown.Name = "SCALE_numericUpDown";
            SCALE_numericUpDown.Size = new Size(62, 23);
            SCALE_numericUpDown.TabIndex = 33;
            SCALE_numericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // Y_numericUpDown
            // 
            Y_numericUpDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Y_numericUpDown.Location = new Point(346, 62);
            Y_numericUpDown.Margin = new Padding(4);
            Y_numericUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            Y_numericUpDown.Minimum = new decimal(new int[] { 10000, 0, 0, int.MinValue });
            Y_numericUpDown.Name = "Y_numericUpDown";
            Y_numericUpDown.Size = new Size(48, 23);
            Y_numericUpDown.TabIndex = 32;
            // 
            // X_numericUpDown
            // 
            X_numericUpDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            X_numericUpDown.Location = new Point(289, 62);
            X_numericUpDown.Margin = new Padding(4);
            X_numericUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            X_numericUpDown.Minimum = new decimal(new int[] { 10000, 0, 0, int.MinValue });
            X_numericUpDown.Name = "X_numericUpDown";
            X_numericUpDown.Size = new Size(48, 23);
            X_numericUpDown.TabIndex = 31;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.BackColor = Color.Transparent;
            button1.Location = new Point(289, 88);
            button1.Margin = new Padding(4);
            button1.Name = "button1";
            button1.Size = new Size(174, 22);
            button1.TabIndex = 30;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label2.Font = new Font("MS UI Gothic", 9F);
            label2.Location = new Point(7, 66);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(258, 24);
            label2.TabIndex = 29;
            label2.Text = "-";
            // 
            // lblDst
            // 
            lblDst.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDst.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblDst.Location = new Point(8, 8);
            lblDst.Margin = new Padding(4, 0, 4, 0);
            lblDst.Name = "lblDst";
            lblDst.Size = new Size(455, 24);
            lblDst.TabIndex = 20;
            lblDst.Text = "ﾎｲｰﾙﾎﾞﾀﾝﾄﾞﾗｯｸﾞで移動。ﾎｲｰﾙ回転で拡縮. ";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.Font = new Font("MS UI Gothic", 9F);
            label1.Location = new Point(7, 32);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(258, 24);
            label1.TabIndex = 28;
            label1.Text = "-";
            // 
            // ArcsuitePreviewForm_Msg_label
            // 
            ArcsuitePreviewForm_Msg_label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ArcsuitePreviewForm_Msg_label.BackColor = Color.Transparent;
            ArcsuitePreviewForm_Msg_label.Enabled = false;
            ArcsuitePreviewForm_Msg_label.Font = new Font("メイリオ", 18F, FontStyle.Regular, GraphicsUnit.Point, 128);
            ArcsuitePreviewForm_Msg_label.Location = new Point(14, 19);
            ArcsuitePreviewForm_Msg_label.Margin = new Padding(4, 0, 4, 0);
            ArcsuitePreviewForm_Msg_label.Name = "ArcsuitePreviewForm_Msg_label";
            ArcsuitePreviewForm_Msg_label.Size = new Size(470, 195);
            ArcsuitePreviewForm_Msg_label.TabIndex = 1;
            ArcsuitePreviewForm_Msg_label.Text = "しばらくお待ちください";
            ArcsuitePreviewForm_Msg_label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ArcSuitePreviewPictureBox
            // 
            ArcSuitePreviewPictureBox.Dock = DockStyle.Fill;
            ArcSuitePreviewPictureBox.Location = new Point(0, 0);
            ArcSuitePreviewPictureBox.Margin = new Padding(4);
            ArcSuitePreviewPictureBox.Name = "ArcSuitePreviewPictureBox";
            ArcSuitePreviewPictureBox.Size = new Size(499, 422);
            ArcSuitePreviewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            ArcSuitePreviewPictureBox.TabIndex = 0;
            ArcSuitePreviewPictureBox.TabStop = false;
            ArcSuitePreviewPictureBox.MouseDown += pictureBox1_MouseDown;
            ArcSuitePreviewPictureBox.MouseMove += pictureBox1_MouseMove;
            ArcSuitePreviewPictureBox.MouseUp += pictureBox1_MouseUp;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox1.Controls.Add(DrawingInfoLabel3);
            groupBox1.Controls.Add(DrawingInfoLabel2);
            groupBox1.Controls.Add(modelcreationonorder_label);
            groupBox1.Controls.Add(DrawingInfoLabel4);
            groupBox1.Controls.Add(ArcSuiteCreatedOn_label);
            groupBox1.Location = new Point(14, 438);
            groupBox1.Margin = new Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4);
            groupBox1.Size = new Size(442, 174);
            groupBox1.TabIndex = 25;
            groupBox1.TabStop = false;
            groupBox1.Text = "ｱｰｸｽｲｰﾄでの属性値";
            // 
            // DrawingInfoLabel3
            // 
            DrawingInfoLabel3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DrawingInfoLabel3.Font = new Font("MS UI Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            DrawingInfoLabel3.Location = new Point(7, 84);
            DrawingInfoLabel3.Margin = new Padding(4, 0, 4, 0);
            DrawingInfoLabel3.Name = "DrawingInfoLabel3";
            DrawingInfoLabel3.Size = new Size(428, 22);
            DrawingInfoLabel3.TabIndex = 19;
            DrawingInfoLabel3.Text = "---";
            // 
            // DrawingInfoLabel2
            // 
            DrawingInfoLabel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DrawingInfoLabel2.Font = new Font("MS UI Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            DrawingInfoLabel2.Location = new Point(7, 55);
            DrawingInfoLabel2.Margin = new Padding(4, 0, 4, 0);
            DrawingInfoLabel2.Name = "DrawingInfoLabel2";
            DrawingInfoLabel2.Size = new Size(428, 22);
            DrawingInfoLabel2.TabIndex = 16;
            DrawingInfoLabel2.Text = "---";
            // 
            // modelcreationonorder_label
            // 
            modelcreationonorder_label.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modelcreationonorder_label.Font = new Font("MS UI Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            modelcreationonorder_label.Location = new Point(7, 29);
            modelcreationonorder_label.Margin = new Padding(4, 0, 4, 0);
            modelcreationonorder_label.Name = "modelcreationonorder_label";
            modelcreationonorder_label.Size = new Size(428, 22);
            modelcreationonorder_label.TabIndex = 17;
            modelcreationonorder_label.Text = "---";
            // 
            // DrawingInfoLabel4
            // 
            DrawingInfoLabel4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DrawingInfoLabel4.Font = new Font("MS UI Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            DrawingInfoLabel4.Location = new Point(7, 141);
            DrawingInfoLabel4.Margin = new Padding(4, 0, 4, 0);
            DrawingInfoLabel4.Name = "DrawingInfoLabel4";
            DrawingInfoLabel4.Size = new Size(428, 22);
            DrawingInfoLabel4.TabIndex = 18;
            DrawingInfoLabel4.Text = "---";
            // 
            // ArcSuiteCreatedOn_label
            // 
            ArcSuiteCreatedOn_label.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ArcSuiteCreatedOn_label.Font = new Font("MS UI Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            ArcSuiteCreatedOn_label.Location = new Point(7, 112);
            ArcSuiteCreatedOn_label.Margin = new Padding(4, 0, 4, 0);
            ArcSuiteCreatedOn_label.Name = "ArcSuiteCreatedOn_label";
            ArcSuiteCreatedOn_label.Size = new Size(428, 22);
            ArcSuiteCreatedOn_label.TabIndex = 21;
            ArcSuiteCreatedOn_label.Text = "---";
            // 
            // TitleBlockScale_button
            // 
            TitleBlockScale_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            TitleBlockScale_button.Location = new Point(395, 441);
            TitleBlockScale_button.Margin = new Padding(4);
            TitleBlockScale_button.Name = "TitleBlockScale_button";
            TitleBlockScale_button.Size = new Size(114, 35);
            TitleBlockScale_button.TabIndex = 25;
            TitleBlockScale_button.Text = "右下部拡大";
            TitleBlockScale_button.UseVisualStyleBackColor = true;
            TitleBlockScale_button.Click += TitleBlockScale_button_Click;
            // 
            // Hide_button
            // 
            Hide_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Hide_button.Location = new Point(395, 579);
            Hide_button.Margin = new Padding(4);
            Hide_button.Name = "Hide_button";
            Hide_button.Size = new Size(114, 35);
            Hide_button.TabIndex = 26;
            Hide_button.Text = "閉じる";
            Hide_button.UseVisualStyleBackColor = true;
            Hide_button.Click += Hide_button_Click;
            // 
            // ArcSuiteWebSearchAndView_button
            // 
            ArcSuiteWebSearchAndView_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ArcSuiteWebSearchAndView_button.Location = new Point(395, 492);
            ArcSuiteWebSearchAndView_button.Margin = new Padding(4);
            ArcSuiteWebSearchAndView_button.Name = "ArcSuiteWebSearchAndView_button";
            ArcSuiteWebSearchAndView_button.Size = new Size(114, 35);
            ArcSuiteWebSearchAndView_button.TabIndex = 27;
            ArcSuiteWebSearchAndView_button.Text = "Web版で再検索";
            ArcSuiteWebSearchAndView_button.UseVisualStyleBackColor = true;
            ArcSuiteWebSearchAndView_button.Click += ArcSuiteWebSearchAndView_button_Click;
            // 
            // ClipBoardTextSearch_button
            // 
            ClipBoardTextSearch_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ClipBoardTextSearch_button.Font = new Font("MS UI Gothic", 9F);
            ClipBoardTextSearch_button.Location = new Point(395, 531);
            ClipBoardTextSearch_button.Margin = new Padding(4);
            ClipBoardTextSearch_button.Name = "ClipBoardTextSearch_button";
            ClipBoardTextSearch_button.Size = new Size(114, 41);
            ClipBoardTextSearch_button.TabIndex = 29;
            ClipBoardTextSearch_button.Text = "ｸﾘｯﾌﾟﾎﾞｰﾄﾞ\r\n文字列を検索";
            ClipBoardTextSearch_button.UseVisualStyleBackColor = false;
            ClipBoardTextSearch_button.Click += ClipBoardTextSearch_button_Click;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label4.BackColor = Color.Transparent;
            label4.Location = new Point(404, 359);
            label4.Name = "label4";
            label4.Size = new Size(92, 53);
            label4.TabIndex = 37;
            label4.Text = "マウス中ボタンで移動・ホイール操作で拡大縮小";
            // 
            // ArcSuitePreviewOnlyForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(520, 626);
            ControlBox = false;
            Controls.Add(ClipBoardTextSearch_button);
            Controls.Add(ArcSuiteWebSearchAndView_button);
            Controls.Add(Hide_button);
            Controls.Add(TitleBlockScale_button);
            Controls.Add(groupBox1);
            Controls.Add(PreviewPanel);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4);
            Name = "ArcSuitePreviewOnlyForm";
            Text = "■東陽ﾂｰﾙ ArcSuite登録済み図面";
            FormClosing += ArcSuitePreviewForm_FormClosing;
            Load += ArcSuitePreviewForm_Load;
            Shown += ArcSuitePreviewForm_Shown;
            PreviewPanel.ResumeLayout(false);
            Debug_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SCALE_numericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)Y_numericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)X_numericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)ArcSuitePreviewPictureBox).EndInit();
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel PreviewPanel;
        private System.Windows.Forms.Label ArcsuitePreviewForm_Msg_label;
        public System.Windows.Forms.Label ArcSuite_Status_label;
        private System.Windows.Forms.PictureBox ArcSuitePreviewPictureBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label DrawingInfoLabel3;
        private System.Windows.Forms.Label DrawingInfoLabel2;
        private System.Windows.Forms.Label modelcreationonorder_label;
        private System.Windows.Forms.Label DrawingInfoLabel4;
        private System.Windows.Forms.Label ArcSuiteCreatedOn_label;
        private System.Windows.Forms.Button TitleBlockScale_button;
        private System.Windows.Forms.Button Hide_button;
        private System.Windows.Forms.Button ArcSuiteWebSearchAndView_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDst;
        private System.Windows.Forms.Panel Debug_panel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown SCALE_numericUpDown;
        private System.Windows.Forms.NumericUpDown Y_numericUpDown;
        private System.Windows.Forms.NumericUpDown X_numericUpDown;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label FindTimeStamp_label;
        private System.Windows.Forms.Button ClipBoardTextSearch_button;
        private Label label4;
    }
}