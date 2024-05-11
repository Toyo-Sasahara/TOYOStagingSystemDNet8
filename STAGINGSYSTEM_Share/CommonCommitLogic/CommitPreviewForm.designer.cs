namespace CommonCommitLogic
{
    partial class BigPreviewForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.BigPreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.commitPreviewImage1 = new CommonCommitLogic.CommitPreviewImage();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BigPreviewPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.commitPreviewImage1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(909, 453);
            this.panel1.TabIndex = 0;
            // 
            // BigPreviewPictureBox
            // 
            this.BigPreviewPictureBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BigPreviewPictureBox.Location = new System.Drawing.Point(742, 473);
            this.BigPreviewPictureBox.Name = "BigPreviewPictureBox";
            this.BigPreviewPictureBox.Size = new System.Drawing.Size(31, 29);
            this.BigPreviewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.BigPreviewPictureBox.TabIndex = 0;
            this.BigPreviewPictureBox.TabStop = false;
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.Location = new System.Drawing.Point(846, 473);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 1;
            this.CloseButton.Text = "閉じる";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // commitPreviewImage1
            // 
            this.commitPreviewImage1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commitPreviewImage1.DebugMode = false;
            this.commitPreviewImage1.ErrorImage = null;
            this.commitPreviewImage1.Image = null;
            this.commitPreviewImage1.Location = new System.Drawing.Point(4, 4);
            this.commitPreviewImage1.Name = "commitPreviewImage1";
            this.commitPreviewImage1.Size = new System.Drawing.Size(902, 446);
            this.commitPreviewImage1.TabIndex = 1;
            // 
            // BigPreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 508);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.BigPreviewPictureBox);
            this.Controls.Add(this.panel1);
            this.MinimizeBox = false;
            this.Name = "BigPreviewForm";
            this.Text = "コミット直前 プレビュー画面";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.BigPreviewForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BigPreviewPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox BigPreviewPictureBox;
        private System.Windows.Forms.Button CloseButton;
        private CommitPreviewImage commitPreviewImage1;
    }
}