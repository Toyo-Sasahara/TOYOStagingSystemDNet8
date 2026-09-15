namespace SasaLib.ArcSuitePreview
{
    partial class TextDrawForm
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
            this.panel1 = new Panel();
            this.FontSize_numericUpDown = new NumericUpDown();
            this.drawString_textBox = new TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.FontSize_numericUpDown).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.panel1.Controls.Add(this.FontSize_numericUpDown);
            this.panel1.Controls.Add(this.drawString_textBox);
            this.panel1.Location = new Point(14, 15);
            this.panel1.Margin = new Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(1101, 55);
            this.panel1.TabIndex = 1;
            // 
            // FontSize_numericUpDown
            // 
            this.FontSize_numericUpDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.FontSize_numericUpDown.Font = new Font("MS UI Gothic", 18F, FontStyle.Regular, GraphicsUnit.Point, 128);
            this.FontSize_numericUpDown.Location = new Point(1024, 4);
            this.FontSize_numericUpDown.Margin = new Padding(4, 4, 4, 4);
            this.FontSize_numericUpDown.Maximum = new decimal(new int[] { 72, 0, 0, 0 });
            this.FontSize_numericUpDown.Minimum = new decimal(new int[] { 40, 0, 0, 0 });
            this.FontSize_numericUpDown.Name = "FontSize_numericUpDown";
            this.FontSize_numericUpDown.Size = new Size(74, 31);
            this.FontSize_numericUpDown.TabIndex = 2;
            this.FontSize_numericUpDown.Value = new decimal(new int[] { 40, 0, 0, 0 });
            // 
            // drawString_textBox
            // 
            this.drawString_textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.drawString_textBox.Font = new Font("MS UI Gothic", 18F, FontStyle.Regular, GraphicsUnit.Point, 128);
            this.drawString_textBox.Location = new Point(4, 4);
            this.drawString_textBox.Margin = new Padding(4, 4, 4, 4);
            this.drawString_textBox.Name = "drawString_textBox";
            this.drawString_textBox.Size = new Size(1013, 31);
            this.drawString_textBox.TabIndex = 1;
            // 
            // TextDrawForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1129, 85);
            Controls.Add(this.panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 4, 4, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TextDrawForm";
            RightToLeftLayout = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "文字列描画";
            FormClosing += TextDrawForm_FormClosing;
            Shown += TextDrawForm_Shown;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.FontSize_numericUpDown).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.TextBox drawString_textBox;
        internal System.Windows.Forms.NumericUpDown FontSize_numericUpDown;
    }
}