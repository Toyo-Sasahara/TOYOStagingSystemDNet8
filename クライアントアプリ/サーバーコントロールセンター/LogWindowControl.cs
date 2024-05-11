using SasaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerControlCenterApplication
{
    public partial class LogWindowControl : UserControl
    {

        public LogWindowControl()
        {
            InitializeComponent();
        }


        private void LogWindowControl_Load(object sender, EventArgs e)
        {

        }

        [System.Diagnostics.DebuggerStepThrough]
        public void WriteLine(string msg)
        {
            try
            {
                    Invoke(new Action(() =>
                    {
                        /// UIを操作する処理
                        LogWindow_textBox.AppendText(msg + "\r\n");
                    }));
            }
            catch (Exception ex)
            {
                DebugConsole.WriteLine(ex.Message);
            }
        }

    }
}
