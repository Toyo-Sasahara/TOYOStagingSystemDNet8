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

namespace CommonCommitLogic
{
    [SupportedOSPlatform("windows")]
    public partial class BigPreviewForm : Form
    {
        public BigPreviewForm()
        {
            InitializeComponent();
        }

        public void SetImage(Image img)
        {
            //BigPreviewPictureBox.Image = img;
            commitPreviewImage1.Image = img;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BigPreviewForm_Load(object sender, EventArgs e)
        {
            // ウィンド位置を指定
            this.Location = new Point(this.Owner.Location.X + (this.Owner.Width - this.Width) / 2, this.Owner.Location.Y + (this.Owner.Height - this.Height) / 2);
        }
    }
}
