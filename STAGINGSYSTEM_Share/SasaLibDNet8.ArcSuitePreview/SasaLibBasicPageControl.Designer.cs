using System.ComponentModel;
using System.ComponentModel.Design;

namespace SasaLib
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    partial class SasaLibBasicPageControl
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
            MainPanel = new Panel();
            PageNumberTextBox = new TextBox();
            XPlus_button = new Button();
            XMinus_button = new Button();
            MainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // MainPanel
            // 
            MainPanel.Controls.Add(PageNumberTextBox);
            MainPanel.Controls.Add(XPlus_button);
            MainPanel.Controls.Add(XMinus_button);
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.Location = new Point(0, 0);
            MainPanel.Margin = new Padding(4, 4, 4, 4);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new Size(158, 34);
            MainPanel.TabIndex = 5;
            // 
            // PageNumberTextBox
            // 
            PageNumberTextBox.Anchor = AnchorStyles.Left;
            PageNumberTextBox.Location = new Point(37, 4);
            PageNumberTextBox.Margin = new Padding(4, 4, 4, 4);
            PageNumberTextBox.Name = "PageNumberTextBox";
            PageNumberTextBox.ReadOnly = true;
            PageNumberTextBox.Size = new Size(76, 23);
            PageNumberTextBox.TabIndex = 0;
            PageNumberTextBox.Text = "00 / 00";
            PageNumberTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // XPlus_button
            // 
            XPlus_button.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            XPlus_button.Location = new Point(121, 1);
            XPlus_button.Margin = new Padding(4, 4, 4, 4);
            XPlus_button.Name = "XPlus_button";
            XPlus_button.Size = new Size(27, 28);
            XPlus_button.TabIndex = 3;
            XPlus_button.Text = ">";
            XPlus_button.UseVisualStyleBackColor = true;
            XPlus_button.Click += XPlus_button_Click;
            // 
            // XMinus_button
            // 
            XMinus_button.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            XMinus_button.Location = new Point(4, 1);
            XMinus_button.Margin = new Padding(4, 4, 4, 4);
            XMinus_button.Name = "XMinus_button";
            XMinus_button.Size = new Size(27, 28);
            XMinus_button.TabIndex = 4;
            XMinus_button.Text = "<";
            XMinus_button.UseVisualStyleBackColor = true;
            XMinus_button.Click += XMinus_button_Click;
            // 
            // SasaLibBasicPageControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(MainPanel);
            Margin = new Padding(4, 4, 4, 4);
            Name = "SasaLibBasicPageControl";
            Size = new Size(158, 34);
            MainPanel.ResumeLayout(false);
            MainPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel MainPanel;
        public System.Windows.Forms.Button XPlus_button;
        public System.Windows.Forms.Button XMinus_button;
        public System.Windows.Forms.TextBox PageNumberTextBox;
    }
}
