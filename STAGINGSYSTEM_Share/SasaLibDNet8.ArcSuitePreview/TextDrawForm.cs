using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SasaLib.ArcSuitePreview
{
    [SupportedOSPlatform("windows")]
    public partial class TextDrawForm : Form
    {
        ArcSuitePreviewForm arcsuitePreviewForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextDrawForm(ArcSuitePreviewForm arcsuitePreviewForm)
        {
            this.arcsuitePreviewForm = arcsuitePreviewForm;

            InitializeComponent();
        }

        private void Draw_button_Click(object sender, EventArgs e)
        {
            string text = drawString_textBox.Text;

        }

        private void TextDrawForm_Shown(object sender, EventArgs e)
        {
            arcsuitePreviewForm.WriteTextMode = true;
        }

        private void TextDrawForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            arcsuitePreviewForm.WriteTextMode = false;

            //switch (e.CloseReason)
            //{
            //    case CloseReason.UserClosing:
            //        //InventorAddInServer.logSystem.WriteLine("DockableLogWindowForm event UserClosing");
            //        e.Cancel = true;
            //        this.Hide();
            //        this.IsShow = false;
            //        break;
            //    default:
            //        break;
            //}

        }
    }
}
