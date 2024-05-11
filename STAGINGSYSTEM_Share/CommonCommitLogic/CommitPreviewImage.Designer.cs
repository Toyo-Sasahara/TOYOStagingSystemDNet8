namespace CommonCommitLogic
{
    partial class CommitPreviewImage
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
            this.MiniPreviewPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MiniPreviewPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MiniPreviewPictureBox
            // 
            this.MiniPreviewPictureBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.MiniPreviewPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MiniPreviewPictureBox.Location = new System.Drawing.Point(0, 0);
            this.MiniPreviewPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.MiniPreviewPictureBox.Name = "MiniPreviewPictureBox";
            this.MiniPreviewPictureBox.Size = new System.Drawing.Size(547, 311);
            this.MiniPreviewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MiniPreviewPictureBox.TabIndex = 17;
            this.MiniPreviewPictureBox.TabStop = false;
            this.MiniPreviewPictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MiniPreviewPictureBox_MouseDoubleClick);
            this.MiniPreviewPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BigPreviewPictureBox_MouseDown);
            this.MiniPreviewPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BigPreviewPictureBox_MouseMove);
            this.MiniPreviewPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BigPreviewPictureBox_MouseUp);
            // 
            // CommitPreviewImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MiniPreviewPictureBox);
            this.Name = "CommitPreviewImage";
            this.Size = new System.Drawing.Size(547, 311);
            this.Load += new System.EventHandler(this.PreviewArcSuiteForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MiniPreviewPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox MiniPreviewPictureBox;
    }
}
