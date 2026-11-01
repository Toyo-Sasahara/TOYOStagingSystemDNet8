namespace NetworkPrintServiceTestTool
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelEndpoint = new Label();
            textBoxEndpoint = new TextBox();
            labelImagePath = new Label();
            textBoxImagePath = new TextBox();
            buttonBrowseImage = new Button();
            labelDeviceName = new Label();
            textBoxDeviceName = new TextBox();
            labelDocumentName = new Label();
            textBoxDocumentName = new TextBox();
            buttonSend = new Button();
            textBoxResult = new TextBox();
            SuspendLayout();
            // 
            // labelEndpoint
            // 
            labelEndpoint.AutoSize = true;
            labelEndpoint.Location = new Point(12, 15);
            labelEndpoint.Name = "labelEndpoint";
            labelEndpoint.Size = new Size(79, 15);
            labelEndpoint.TabIndex = 0;
            labelEndpoint.Text = "Endpoint URL";
            // 
            // textBoxEndpoint
            // 
            textBoxEndpoint.Location = new Point(112, 12);
            textBoxEndpoint.Name = "textBoxEndpoint";
            textBoxEndpoint.Size = new Size(750, 23);
            textBoxEndpoint.TabIndex = 1;
            textBoxEndpoint.Text = "http://localhost:5230/print";
            // 
            // labelImagePath
            // 
            labelImagePath.AutoSize = true;
            labelImagePath.Location = new Point(12, 47);
            labelImagePath.Name = "labelImagePath";
            labelImagePath.Size = new Size(60, 15);
            labelImagePath.TabIndex = 2;
            labelImagePath.Text = "Image File";
            // 
            // textBoxImagePath
            // 
            textBoxImagePath.Location = new Point(112, 44);
            textBoxImagePath.Name = "textBoxImagePath";
            textBoxImagePath.Size = new Size(636, 23);
            textBoxImagePath.TabIndex = 3;
            // 
            // buttonBrowseImage
            // 
            buttonBrowseImage.Location = new Point(754, 43);
            buttonBrowseImage.Name = "buttonBrowseImage";
            buttonBrowseImage.Size = new Size(108, 25);
            buttonBrowseImage.TabIndex = 4;
            buttonBrowseImage.Text = "参照...";
            buttonBrowseImage.UseVisualStyleBackColor = true;
            buttonBrowseImage.Click += buttonBrowseImage_Click;
            // 
            // labelDeviceName
            // 
            labelDeviceName.AutoSize = true;
            labelDeviceName.Location = new Point(12, 79);
            labelDeviceName.Name = "labelDeviceName";
            labelDeviceName.Size = new Size(73, 15);
            labelDeviceName.TabIndex = 5;
            labelDeviceName.Text = "DeviceName";
            // 
            // textBoxDeviceName
            // 
            textBoxDeviceName.Location = new Point(112, 76);
            textBoxDeviceName.Name = "textBoxDeviceName";
            textBoxDeviceName.Size = new Size(289, 23);
            textBoxDeviceName.TabIndex = 6;
            textBoxDeviceName.Text = "DocuCentre-V5080.XML";
            // 
            // labelDocumentName
            // 
            labelDocumentName.AutoSize = true;
            labelDocumentName.Location = new Point(12, 111);
            labelDocumentName.Name = "labelDocumentName";
            labelDocumentName.Size = new Size(93, 15);
            labelDocumentName.TabIndex = 11;
            labelDocumentName.Text = "DocumentName";
            // 
            // textBoxDocumentName
            // 
            textBoxDocumentName.Location = new Point(112, 108);
            textBoxDocumentName.Name = "textBoxDocumentName";
            textBoxDocumentName.Size = new Size(592, 23);
            textBoxDocumentName.TabIndex = 12;
            // 
            // buttonSend
            // 
            buttonSend.Location = new Point(710, 107);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(152, 25);
            buttonSend.TabIndex = 13;
            buttonSend.Text = "送信して印刷実行";
            buttonSend.UseVisualStyleBackColor = true;
            buttonSend.Click += buttonSend_Click;
            // 
            // textBoxResult
            // 
            textBoxResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxResult.Location = new Point(12, 156);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.ReadOnly = true;
            textBoxResult.ScrollBars = ScrollBars.Both;
            textBoxResult.Size = new Size(1284, 436);
            textBoxResult.TabIndex = 14;
            textBoxResult.WordWrap = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1310, 604);
            Controls.Add(textBoxResult);
            Controls.Add(buttonSend);
            Controls.Add(textBoxDocumentName);
            Controls.Add(labelDocumentName);
            Controls.Add(textBoxDeviceName);
            Controls.Add(labelDeviceName);
            Controls.Add(buttonBrowseImage);
            Controls.Add(textBoxImagePath);
            Controls.Add(labelImagePath);
            Controls.Add(textBoxEndpoint);
            Controls.Add(labelEndpoint);
            Name = "Form1";
            Text = "NetworkPrintService Test Tool";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelEndpoint;
        private TextBox textBoxEndpoint;
        private Label labelImagePath;
        private TextBox textBoxImagePath;
        private Button buttonBrowseImage;
        private Label labelDeviceName;
        private TextBox textBoxDeviceName;
        private Label labelDocumentName;
        private TextBox textBoxDocumentName;
        private Button buttonSend;
        private TextBox textBoxResult;
    }
}
