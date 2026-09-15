using SasaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SasaLib.VariableControlPipeServer
{
    partial class AutoCloseMessageBoxForm
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
            this.components = new System.ComponentModel.Container();
            this.button3 = new System.Windows.Forms.Button();
            this.MessageTest_label = new System.Windows.Forms.Label();
            this.TimeRemaining_label = new System.Windows.Forms.Label();
            this.FormClose_timer = new System.Windows.Forms.Timer(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.MessageText_label = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Icon_picturebox = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Icon_picturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button3.Location = new System.Drawing.Point(238, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(108, 24);
            this.button3.TabIndex = 4;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MessageTest_label
            // 
            this.MessageTest_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageTest_label.Location = new System.Drawing.Point(12, 9);
            this.MessageTest_label.Name = "MessageTest_label";
            this.MessageTest_label.Size = new System.Drawing.Size(445, 112);
            this.MessageTest_label.TabIndex = 1;
            this.MessageTest_label.Text = "MessageTest_label";
            // 
            // TimeRemaining_label
            // 
            this.TimeRemaining_label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TimeRemaining_label.AutoSize = true;
            this.TimeRemaining_label.Location = new System.Drawing.Point(10, 109);
            this.TimeRemaining_label.Name = "TimeRemaining_label";
            this.TimeRemaining_label.Size = new System.Drawing.Size(111, 12);
            this.TimeRemaining_label.TabIndex = 2;
            this.TimeRemaining_label.Text = "TimeRemaining_label";
            // 
            // FormClose_timer
            // 
            this.FormClose_timer.Enabled = true;
            this.FormClose_timer.Interval = 1000;
            this.FormClose_timer.Tick += new System.EventHandler(this.FormClose_timer_Tick);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(5, 197);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(161, 24);
            this.progressBar1.TabIndex = 3;
            // 
            // MessageText_label
            // 
            this.MessageText_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageText_label.Font = new System.Drawing.Font("MS UI Gothic", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.MessageText_label.ForeColor = System.Drawing.Color.Red;
            this.MessageText_label.Location = new System.Drawing.Point(80, 9);
            this.MessageText_label.Name = "MessageText_label";
            this.MessageText_label.Size = new System.Drawing.Size(448, 171);
            this.MessageText_label.TabIndex = 5;
            this.MessageText_label.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Location = new System.Drawing.Point(171, 191);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(352, 34);
            this.panel1.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 24);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button2.Location = new System.Drawing.Point(122, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 24);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Icon_picturebox
            // 
            this.Icon_picturebox.InitialImage = null;
            this.Icon_picturebox.Location = new System.Drawing.Point(5, 9);
            this.Icon_picturebox.Name = "Icon_picturebox";
            this.Icon_picturebox.Size = new System.Drawing.Size(64, 64);
            this.Icon_picturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.Icon_picturebox.TabIndex = 7;
            this.Icon_picturebox.TabStop = false;
            // 
            // AutoCloseMessageBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.CancelButton = this.button3;
            this.ClientSize = new System.Drawing.Size(534, 232);
            this.ControlBox = false;
            this.Controls.Add(this.Icon_picturebox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.MessageText_label);
            this.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "AutoCloseMessageBoxForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "■東陽ツール メッセージ";
            this.Load += new System.EventHandler(this.AutoCloseMessageBoxForm_Load);
            this.Shown += new System.EventHandler(this.AutoCloseMessageBoxForm_Shown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Icon_picturebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MessageTest_label;
        private System.Windows.Forms.Label TimeRemaining_label;
        private System.Windows.Forms.Timer FormClose_timer;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label MessageText_label;
        private System.Windows.Forms.Panel panel1;
        private Button button1;
        private Button button2;
        private PictureBox Icon_picturebox;
    }
}