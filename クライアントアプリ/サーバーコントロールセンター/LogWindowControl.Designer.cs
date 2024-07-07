namespace ServerControlCenterApplication
{
    partial class LogWindowControl
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
            label4 = new System.Windows.Forms.Label();
            LogWindow_textBox = new System.Windows.Forms.TextBox();
            panel1 = new System.Windows.Forms.Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(4, 0);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(134, 15);
            label4.TabIndex = 136;
            label4.Text = "CurrentStageServer.label";
            // 
            // LogWindow_textBox
            // 
            LogWindow_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LogWindow_textBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            LogWindow_textBox.Location = new System.Drawing.Point(6, 19);
            LogWindow_textBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            LogWindow_textBox.Multiline = true;
            LogWindow_textBox.Name = "LogWindow_textBox";
            LogWindow_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            LogWindow_textBox.Size = new System.Drawing.Size(508, 232);
            LogWindow_textBox.TabIndex = 135;
            // 
            // panel1
            // 
            panel1.Controls.Add(label4);
            panel1.Controls.Add(LogWindow_textBox);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(518, 255);
            panel1.TabIndex = 137;
            // 
            // LogWindowControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel1);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "LogWindowControl";
            Size = new System.Drawing.Size(518, 255);
            Load += LogWindowControl_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox LogWindow_textBox;
        private System.Windows.Forms.Panel panel1;
    }
}
