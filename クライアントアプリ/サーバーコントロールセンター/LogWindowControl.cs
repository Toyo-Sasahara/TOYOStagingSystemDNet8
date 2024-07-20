using SasaLib;
using System;
using System.Windows.Forms;
#if NETCOREAPP
using System.Runtime.Versioning;
#endif

namespace ServerControlCenterApplication
{
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class LogWindowControl : UserControl
    {

        public LogWindowControl()
        {
            InitializeComponent();
        }


        private void LogWindowControl_Load(object sender, EventArgs e)
        {

        }

        public void Clear()
        {
            LogWindow_textBox.Clear();
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
