using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#if NETCOREAPP
using System.Runtime.Versioning;
#endif

namespace CommonCommitLogic
{
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class CommitPreviewForm : Form
    {
        public CommitPreviewForm()
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

        private void CommitPreviewForm_Load(object sender, EventArgs e)
        {
            // ウィンド位置を指定
            this.Location = new Point(this.Owner.Location.X + (this.Owner.Width - this.Width) / 2, this.Owner.Location.Y + (this.Owner.Height - this.Height) / 2);
        }
    }
}
