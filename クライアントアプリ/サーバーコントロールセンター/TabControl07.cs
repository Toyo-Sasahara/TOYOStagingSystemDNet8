using SasaLib.PIPE;
using StageServerRemote;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
#if NETCOREAPP
using System.Runtime.Versioning;
#endif

namespace ServerControlCenterApplication
{
    /// <summary>
    /// 
    /// </summary>
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class TabControl07 : UserControl
    {
        Form1 mainForm;


        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="form"></param>
        public TabControl07(Form1 form)
        {
            this.mainForm = form;
            InitializeComponent();
        }

        private void TabControl07_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                MethodInvoker method = () =>
                {
                    accountUserForm.SetToControls();

                }; if (InvokeRequired) { Invoke(method); } else { method(); }
            });
        }

        /// <summary>
        /// ■このタブコントロールの表示状態が変わったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl07_VisibleChanged(object sender, EventArgs e)
        {
            accountUserForm.SetToControls();

            logWindowControl.WriteLine("TabControl07_VisibleChanged(..)実行・・・\r\n");
        }

        /// <summary>
        /// ■アカウントパネルからフォーカスが移動したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountUserForm_Leave(object sender, EventArgs e)
        {
            accountUserForm.ControlChanged(sender, e);
        }

        private void accountUserForm1_Paint(object sender, PaintEventArgs e)
        {

        }


        /// <summary>
        /// メインスレッド外からの呼び出しも考慮したﾛｸﾞｳｨﾝﾄﾞｳ変更メソッド
        /// </summary>
        /// <param name="msg"></param>
        public void LogWindowWriteLine(string msg)
        {
            try
            {
                if (this.InvokeRequired)
                {//https://qiita.com/taiyakisun/items/15b57df979eae7562aef
                    this.Invoke(new Action<string>(this.UpdateText), msg);
                }
                else
                {
                    UpdateText($"{msg}\n");
                }
            }
            catch (Exception ex)
            {
                logWindowControl.WriteLine($"例外検知{ex.Message}\r\n");

            }
        }
        private void UpdateText(string msg)
        {
            //this.LogWindow_textBox.AppendText($"{msg}\r\n");
            logWindowControl.WriteLine($"{msg}");
        }

        private void SelectSouceFileName_button_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();
            SourceFromLocalFullFileName_textBox.Text = openFileDialog1.FileName;
            SetSamePath_button_Click(sender, e);
        }

        private void SetSamePath_button_Click(object sender, EventArgs e)
        {
            SourceFromLocalFullFileName_textBox.Text = SourceFromLocalFullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            SendToServerRullFileName.Text = System.IO.Path.Combine(@"D:\", System.IO.Path.GetFileName(SourceFromLocalFullFileName_textBox.Text));
        }

        private void FileSendStart_button_Click(object sender, EventArgs e)
        {
            SourceFromLocalFullFileName_textBox.Text = SourceFromLocalFullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            SendToServerRullFileName.Text = SendToServerRullFileName.Text.TrimStart('\"').TrimEnd('\"');

            RemoteClientDRAWCAPTURE remoteClientDC = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameDC
                                                );
            string msg;
            var ans = remoteClientDC.FileSend(SourceFromLocalFullFileName_textBox.Text, SendToServerRullFileName.Text, out msg, WriteLine:LogWindowWriteLine) ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetSamePathServer_button_Click(object sender, EventArgs e)
        {
            ReceveFullFileName_textBox.Text = ReceveFullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            ReceveToLocalFullFileName_textBox.Text = System.IO.Path.Combine(@"D:\", System.IO.Path.GetFileName(ReceveFullFileName_textBox.Text));

        }

        private void FileReceveStart_button_Click(object sender, EventArgs e)
        {
            ReceveFullFileName_textBox.Text = ReceveFullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            ReceveToLocalFullFileName_textBox.Text = ReceveToLocalFullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');

            RemoteClientDRAWCAPTURE remoteClientDC = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName,
                                    SccConfig.Config.ClientUserName,
                                    SccConfig.Config.ClientUserPassword,
                                    SccConfig.Config.ClsLogon,
                                     SccConfig.Config.StageServerHost,
                                     SccConfig.Config.PipeNameDC
                                    );


            string msg;
            var ans = remoteClientDC.FileRecv(ReceveFullFileName_textBox.Text, ReceveToLocalFullFileName_textBox.Text, out msg, WriteLine: LogWindowWriteLine);

        }

        private void ReceveFullFileName_textBox_TextChanged(object sender, EventArgs e)
        {
            SetSamePathServer_button_Click(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetAvailableMemory_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH remoteClientSW = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameSW
                                                );
            float availableMemory;
            var result = remoteClientSW.GetAvailableMemory(out availableMemory, WriteLine: LogWindowWriteLine);

        }

        private void GetDRusedMemory_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH remoteClientSW = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameSW
                                                );
            long availableMemory;
            var result = remoteClientSW.GetUsedMemory("ToyoDRAWREGISTservice", out availableMemory, WriteLine: LogWindowWriteLine);

        }

        private void GetDCusedMemory_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH remoteClientSW = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameSW
                                                );
            long availableMemory;
            var result = remoteClientSW.GetUsedMemory("ToyoDRAWCAPTUREservice", out availableMemory, WriteLine: LogWindowWriteLine);

        }

        private void GetSWusedMemory_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH remoteClientSW = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameSW
                                                );
            long availableMemory;
            var result = remoteClientSW.GetUsedMemory("ToyoSTAGINGSYSTEMwatch", out availableMemory, WriteLine: LogWindowWriteLine);

        }

        private void ObjecttoByteViaJsonSerializer_Click(object sender, EventArgs e)
        {
            Bitmap orgObject = Properties.Resources.イメージ読込中;

            DebugForm_PictureBox.Image = null;

            // オブジェクトをバイト配列に変換
            var converter = new ObjectConverter<Bitmap>();
            long sz;
            Exception ex;

            string jsontxt;
            var bytes = converter.ToByteArrayViaJsonSerializer(orgObject);
            Bitmap anserobject = converter.FromByteArrayViaJsonSerializer(bytes);

            DebugForm_PictureBox.Image = anserobject;

        }

        private void ObjecttoBytebutton_Click(object sender, EventArgs e)
        {
            Image orgObject = Properties.Resources.イメージ読込中;

            DebugForm_PictureBox.Image = null;

            // オブジェクトをバイト配列に変換
            var converter = new ObjectConverter<Image>();
            long sz;
            Exception ex;

            var bytes = converter.ToByteArray(orgObject);

            Image image = converter.FromByteArray(bytes);

            DebugForm_PictureBox.Image = image;
        }

        private void Direct2_button_Click(object sender, EventArgs e)
        {
            Image orgObject = Properties.Resources.イメージ読込中;

            DebugForm_PictureBox.Image = null;

            // オブジェクトをバイト配列に変換
            var converter = new ObjectConverter<Image>();
            long sz;
            Exception ex;

            var bytes = converter.ToByteArrayViaDirect2((Bitmap)orgObject);

            Bitmap input = (Bitmap)converter.FromByteArrayViaDirect2(bytes);

            DebugForm_PictureBox.Image = input;

        }

        private void FromByteArrayToBitmap_button_Click(object sender, EventArgs e)
        {
            DebugForm_PictureBox.Image = null;

            Image orgObject = Properties.Resources.イメージ読込中;

            // オブジェクトをバイト配列に変換
            var converter = new ObjectConverter<Image>();
            long sz;
            Exception ex;

            var bytes = converter.ToByteArrayFromBitmap((Bitmap)orgObject, System.Drawing.Imaging.ImageFormat.Jpeg);

            Bitmap input = (Bitmap)converter.FromByteArrayToBitmap(bytes);

            DebugForm_PictureBox.Image = input;

        }
    }
}
