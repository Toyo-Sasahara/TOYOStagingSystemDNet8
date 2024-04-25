namespace ClientApp.Forms
{
    partial class PreviewImageForm
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
            this.PreviewArcSuitePictureBox = new System.Windows.Forms.PictureBox();
            this.Hide_button = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ArcSuitePARTNAMEandDESCRIPTION_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ArcSuiteEditionNumber_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewArcSuitePictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PreviewArcSuitePictureBox
            // 
            this.PreviewArcSuitePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreviewArcSuitePictureBox.Location = new System.Drawing.Point(0, 0);
            this.PreviewArcSuitePictureBox.Name = "PreviewArcSuitePictureBox";
            this.PreviewArcSuitePictureBox.Size = new System.Drawing.Size(675, 416);
            this.PreviewArcSuitePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PreviewArcSuitePictureBox.TabIndex = 0;
            this.PreviewArcSuitePictureBox.TabStop = false;
            this.PreviewArcSuitePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BigPreviewPictureBox_MouseDown);
            this.PreviewArcSuitePictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BigPreviewPictureBox_MouseMove);
            this.PreviewArcSuitePictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BigPreviewPictureBox_MouseUp);
            // 
            // Hide_button
            // 
            this.Hide_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Hide_button.Location = new System.Drawing.Point(612, 473);
            this.Hide_button.Name = "Hide_button";
            this.Hide_button.Size = new System.Drawing.Size(75, 23);
            this.Hide_button.TabIndex = 1;
            this.Hide_button.Text = "非表示";
            this.Hide_button.UseVisualStyleBackColor = true;
            this.Hide_button.Click += new System.EventHandler(this.Hide_button_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.PreviewArcSuitePictureBox);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(675, 416);
            this.panel1.TabIndex = 2;
            // 
            // ArcSuitePARTNAMEandDESCRIPTION_textBox
            // 
            this.ArcSuitePARTNAMEandDESCRIPTION_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ArcSuitePARTNAMEandDESCRIPTION_textBox.Location = new System.Drawing.Point(304, 475);
            this.ArcSuitePARTNAMEandDESCRIPTION_textBox.Name = "ArcSuitePARTNAMEandDESCRIPTION_textBox";
            this.ArcSuitePARTNAMEandDESCRIPTION_textBox.ReadOnly = true;
            this.ArcSuitePARTNAMEandDESCRIPTION_textBox.Size = new System.Drawing.Size(299, 19);
            this.ArcSuitePARTNAMEandDESCRIPTION_textBox.TabIndex = 6;
            this.ArcSuitePARTNAMEandDESCRIPTION_textBox.Visible = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(225, 478);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "部品名/説明:";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.ForeColor = System.Drawing.Color.Fuchsia;
            this.label3.Location = new System.Drawing.Point(12, 442);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(337, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "マウス中央ボタンで 拡大・縮小・画面移動 可能です";
            // 
            // ArcSuiteEditionNumber_textBox
            // 
            this.ArcSuiteEditionNumber_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ArcSuiteEditionNumber_textBox.Location = new System.Drawing.Point(169, 475);
            this.ArcSuiteEditionNumber_textBox.Name = "ArcSuiteEditionNumber_textBox";
            this.ArcSuiteEditionNumber_textBox.ReadOnly = true;
            this.ArcSuiteEditionNumber_textBox.Size = new System.Drawing.Size(27, 19);
            this.ArcSuiteEditionNumber_textBox.TabIndex = 4;
            this.ArcSuiteEditionNumber_textBox.Visible = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 478);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "上図面のｱｰｸｽｲｰﾄ改版番号:";
            this.label1.Visible = false;
            // 
            // PreviewImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 501);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ArcSuitePARTNAMEandDESCRIPTION_textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ArcSuiteEditionNumber_textBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Hide_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximumSize = new System.Drawing.Size(750, 540);
            this.MinimizeBox = false;
            this.Name = "PreviewImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "コミットされたステージサーバー上の図面イメージ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreviewArcSuiteForm_FormClosing);
            this.Load += new System.EventHandler(this.PreviewArcSuiteForm_Load);
            this.Shown += new System.EventHandler(this.PreviewArcSuiteForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.PreviewArcSuitePictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Hide_button;
        private System.Windows.Forms.PictureBox PreviewArcSuitePictureBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox ArcSuitePARTNAMEandDESCRIPTION_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ArcSuiteEditionNumber_textBox;
        private System.Windows.Forms.Label label1;
    }
}