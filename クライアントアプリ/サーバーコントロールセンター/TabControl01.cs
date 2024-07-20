using SasaLib;
using SharedClassLibrary;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToyoStageService;
#if NETCOREAPP
using System.Runtime.Versioning;
#endif

namespace ServerControlCenterApplication
{
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class TabControl01 : UserControl
    {
        Form1 mainForm;

        int printerSelIndex1;
        int printerSelIndex2;




        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="form"></param>
        public TabControl01(Form1 form)
        {
            this.mainForm = form;
            InitializeComponent();

            accountUserForm.parentControl = this;


            // カスタムイベントのハンドラを追加
            accountUserForm.AccountChanged += accountUserForm_AccountChanged;
            accountUserForm.HostChanged += accountUserForm_HostChanged;
            DebugForm_PictureBox.AllowDrop = true;
        }

        private async void accountUserForm_AccountChanged(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("accountUserForm_AccountChanged(..)実行・・・\r\n");

            await Task.Run(() =>
            {
                MethodInvoker method = () =>
                {
                    accountUserForm.SetToControls();


                }; if (InvokeRequired) { Invoke(method); } else { method(); }
            });



        }

        private async void accountUserForm_HostChanged(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("accountUserForm_AccountChanged(..)実行・・・\r\n");

            await Task.Run(() =>
            {
                MethodInvoker method = () =>
                {
                    accountUserForm.SetToControls();

                    mainForm.CommitPrinters.GetData(objectConvNew_CheckBox.Checked);

                }; if (InvokeRequired) { Invoke(method); } else { method(); }
            });

            mainForm.CommitPrinters.SetComboBox(ref PrinterSel_comboBox);
            mainForm.CommitPrinters.SetComboBox(ref PrinterSel_comboBox2);

            if (mainForm.CommitPrinters.resultGetCommitPrinterShortCutName != null && mainForm.CommitPrinters.resultGetCommitPrinterShortCutName.Count > 0)
            {
                PrinterSel_comboBox.DataSource = mainForm.CommitPrinters.resultGetCommitPrinterShortCutName;
                PrinterSel_comboBox.DisplayMember = "key";
                PrinterSel_comboBox.ValueMember = "value";

                PrinterSel_comboBox2.DataSource = mainForm.CommitPrinters.resultGetCommitPrinterShortCutName;
                PrinterSel_comboBox2.DisplayMember = "key";
                PrinterSel_comboBox2.ValueMember = "value";
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl01_Load(object sender, EventArgs e)
        {
            accountUserForm.StageServerHostName_comboBox_SetText(SccConfig.Config.StageServerHost);

            accountUserForm.WriteLine = logWindowControl.WriteLine;
        }

        /// <summary>
        /// ■このタブコントロールの表示状態が変わったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TabControl01_VisibleChanged(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("TabControl01_VisibleChanged(..)実行・・・\r\n");

            await Task.Run(() =>
            {
                MethodInvoker method = () =>
                {
                    accountUserForm.SetToControls();

                    mainForm.CommitPrinters.GetData(objectConvNew_CheckBox.Checked);

                }; if (InvokeRequired) { Invoke(method); } else { method(); }
            });

            mainForm.CommitPrinters.SetComboBox(ref PrinterSel_comboBox);
            mainForm.CommitPrinters.SetComboBox(ref PrinterSel_comboBox2);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reloadPrinter_button_Click(object sender, EventArgs e)
        {
            logWindowControl.Clear();
            mainForm.CommitPrinters.GetData(objectConvNew_CheckBox.Checked, logWindowControl.WriteLine);
        }



        /// <summary>
        /// ■コミットテストボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitTestButton_Click(object sender, EventArgs e)
        {
            string TemplateFile = TESTIMAGE_comboBox.Text;

            string PARTNUMBER = TESTIMAGEDraw_TexstTextBox.Text;
            string AUTHOR = AUTHORUSER_textBox.Text;
            string AUTHDATE = AUTHDATE_textBox.Text;
            string TITLE = DESCRIPTION_textBox.Text;
            string PARTSNAME = DESCRIPTION_textBox.Text;
            string DESCRIPTION = DESCRIPTION_textBox.Text;
            string MATERIAL = MATERIAL_textBox.Text;




            List<CommonTicket.Param> prms = new List<CommonTicket.Param>();
            CommitTest.UpdateComonTicketParam(ref prms, "PARTNUMBER", PARTNUMBER);
            CommitTest.UpdateComonTicketParam(ref prms, "SANITIZEDPARTNUMBER", null);
            CommitTest.UpdateComonTicketParam(ref prms, "REV", null);

            CommitTest.UpdateComonTicketParam(ref prms, "AUTHOR", AUTHOR);
            CommitTest.UpdateComonTicketParam(ref prms, "AUTHORDATE", AUTHDATE);

            CommitTest.UpdateComonTicketParam(ref prms, "DESIGNER", null);
            CommitTest.UpdateComonTicketParam(ref prms, "CHECKDATE", null);

            CommitTest.UpdateComonTicketParam(ref prms, "DRAWINGTYPE", null);

            CommitTest.UpdateComonTicketParam(ref prms, "ORDERNUMBER", null);

            CommitTest.UpdateComonTicketParam(ref prms, "TITLE", TITLE);
            CommitTest.UpdateComonTicketParam(ref prms, "DESCRIPTION", DESCRIPTION);
            CommitTest.UpdateComonTicketParam(ref prms, "PARTSNAME", PARTSNAME);
            CommitTest.UpdateComonTicketParam(ref prms, "MATERIAL", MATERIAL);
            CommitTest.UpdateComonTicketParam(ref prms, "MATERIALCODE", null);

            CommitTest.UpdateComonTicketParam(ref prms, "MACHINETYPE", null);

            CommitTest.UpdateComonTicketParam(ref prms, "FIRSTCUSTOMER", "〇１製薬");
            CommitTest.UpdateComonTicketParam(ref prms, "CUSTOMER", "〇２製薬");

            List<CommonTicket.Variant> variant = new List<CommonTicket.Variant>();

            if (VARIANT_Type_checkBox.Checked)
            {
                // TextBox.Text を改行で分割して List<string> に変換
                List<string> lines = VARIANT_PARTNUMBER_textBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                CommitTest.UpdateComonTicketVariant(ref variant, lines, true);
            }

            string printerName = (string)PrinterSel_comboBox.SelectedValue;

            ComitTestStart(TemplateFile, prms, variant, printerName);
        }

        private void CommitContinuouslyTest_Button_Click(object sender, EventArgs e)
        {

            string PARTNUMBER = TESTIMAGEDraw_TexstTextBox.Text;
            string AUTHOR = AUTHORUSER_textBox.Text;
            string AUTHDATE = AUTHDATE_textBox.Text;
            string TITLE = DESCRIPTION_textBox.Text;
            string PARTSNAME = DESCRIPTION_textBox.Text;
            string DESCRIPTION = DESCRIPTION_textBox.Text;
            string MATERIAL = MATERIAL_textBox.Text;

            List<CommonTicket.Param> prms = new List<CommonTicket.Param>();
            CommitTest.UpdateComonTicketParam(ref prms, "SANITIZEDPARTNUMBER", null);
            CommitTest.UpdateComonTicketParam(ref prms, "REV", null);

            CommitTest.UpdateComonTicketParam(ref prms, "AUTHOR", AUTHOR);
            CommitTest.UpdateComonTicketParam(ref prms, "AUTHORDATE", AUTHDATE);

            CommitTest.UpdateComonTicketParam(ref prms, "DESIGNER", null);
            CommitTest.UpdateComonTicketParam(ref prms, "CHECKDATE", null);

            CommitTest.UpdateComonTicketParam(ref prms, "DRAWINGTYPE", null);

            CommitTest.UpdateComonTicketParam(ref prms, "ORDERNUMBER", null);

            CommitTest.UpdateComonTicketParam(ref prms, "TITLE", TITLE);
            CommitTest.UpdateComonTicketParam(ref prms, "DESCRIPTION", DESCRIPTION);
            CommitTest.UpdateComonTicketParam(ref prms, "PARTSNAME", PARTSNAME);
            CommitTest.UpdateComonTicketParam(ref prms, "MATERIAL", MATERIAL);
            CommitTest.UpdateComonTicketParam(ref prms, "MATERIALCODE", null);

            CommitTest.UpdateComonTicketParam(ref prms, "MACHINETYPE", null);

            CommitTest.UpdateComonTicketParam(ref prms, "FIRSTCUSTOMER", "〇１製薬");
            CommitTest.UpdateComonTicketParam(ref prms, "CUSTOMER", "〇２製薬");


            List<CommonTicket.Variant> variantparams = new List<CommonTicket.Variant>();


            foreach (var pp in TESTIMAGEDraw_TexstTextBox2.Lines)
            {
                if (string.IsNullOrWhiteSpace(pp) == false)
                {
                    string TemplateFile = TESTIMAGE_comboBox.Text;
                    CommitTest.UpdateComonTicketParam(ref prms, "PARTNUMBER", pp);

                    string printerName;

                    if (CommtiNoPrintout_checkBox.Checked)
                    {
                        printerName = KWD.NoPrintCommitPrinter;
                    }
                    else
                    {
                        printerName = (string)PrinterSel_comboBox2.SelectedValue;
                    }


                    var ticketcode = ComitTestStart(TemplateFile, prms, variantparams, printerName);

                    TICKETCODS_textBox.AppendText($"{ticketcode}\r\n");
                }

            }
        }

        string ComitTestStart(string TemplateFile, List<CommonTicket.Param> prms, List<CommonTicket.Variant> variantparams, string printerName)
        {

            string CurrentProcessPath = FileFolder.GetCurrentProcessePath();

            string TemplateTiffFullpath
                = FileFolder.GetFolderName(CurrentProcessPath) + System.IO.Path.DirectorySeparatorChar + @"TestImages" + System.IO.Path.DirectorySeparatorChar + TemplateFile + ".TIF";


            string TIFFpositonChangeTemplateConfigFullFileName = SccConfigWork.GetAppConfigFolder() + System.IO.Path.DirectorySeparatorChar + "TIFFpositonChangeTemplate.XML";
            logWindowControl.WriteLine($"テスト図面用オフセット設定ﾌｧｲﾙは  {TIFFpositonChangeTemplateConfigFullFileName}が指定されます");

            logWindowControl.WriteLine($"プリンタ設定ファイルは  \"{printerName}\"が指定されます");


            string TICKETCODE;

            /// テスト用イメージとチケットファイル作成
            bool ans = CommitTest.CreateTicketAndTiffImage(SccConfig.Config.CommitPath, TemplateTiffFullpath, out TICKETCODE, prms, variantparams, TIFFpositonChangeTemplateConfigFullFileName, printerName, PrintOutOnly: PrintOUtOnly_checkBox.Checked, WriteLine: LogWindowWriteLine);

            if (ans)
            {
                TestImageOffsetValue_Label.Text = $"用紙ｻｲｽﾞ{StaticCommonVars.currentPaperSize} ｵﾌｾｯﾄ位置 (X={StaticCommonVars.Offset.X},Y={StaticCommonVars.Offset.Y})";
                TicketCodeReadOnly_TextBox.Text = StaticCommonVars.TICKETCODE;

                #region 【クリッピングして表示】
                System.Drawing.Bitmap bmporg = new System.Drawing.Bitmap(StaticCommonVars.currentImage);

                bmporg = ImageUtil.ChangePixelFormat((Bitmap)bmporg, PixelFormat.Format32bppArgb);

                int ww = 2100;
                int hh = 800;
                int w = ww;
                int h = hh;
                int xx = bmporg.Width - ww;
                int yy = bmporg.Height - hh;

                System.Drawing.Bitmap clipBmp = SasaLib.ImageUtil.ImageRoi(bmporg, new Rectangle(xx, yy, w, h));

                DebugForm_PictureBox.Image = clipBmp;
                bmporg.Dispose();
                #endregion

            }

            return TICKETCODE;
        }

        /// <summary>
        /// ■
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowPrinterQueue_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH rmc_SYSTEMWATCH = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameSW);
            var ans = rmc_SYSTEMWATCH.ShowPrinterQueue(SccConfig.Config.StageServerHost, PrinterSel_comboBox.Text, logWindowControl.WriteLine);

            logWindowControl.WriteLine(ans);
        }

        public void LogWindowWriteLine(string msg)
        {
            try
            {
                if (this.InvokeRequired)
                {//https://qiita.com/taiyakisun/items/15b57df979eae7562aef
                    Invoke(new Action<string>(this.UpdateText), msg);
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
            logWindowControl.WriteLine($"{msg}\r\n");
        }

        private void DebugFormPictureBox_DragDrop(object sender, DragEventArgs e)
        {
            List<string> filelist = new List<string>();

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i];

                logWindowControl.WriteLine($"ピクチャーボックスにドラッグされました {fileName}\r\n");

                filelist.Add(fileName);
            }

            foreach (var aa in filelist)
            {
                DateTime LastWriteTimeBefore = System.IO.File.GetLastWriteTime(aa);
                logWindowControl.WriteLine($"実行します {aa} LastWriteTime:{LastWriteTimeBefore}\r\n");

                //int ret = SasaLib.OScommand.ExcuteBatchCMD2(@"C:\Program Files\paint.net\paintdotnet.exe", aa);

                string stdoutText = null;
                string stderrText = null;
                int ret = SasaLib.OScommand.ExcuteBatchCMD2B(@"C:\Program Files\paint.net\paintdotnet.exe", aa, out stdoutText, out stderrText);

                DateTime LastWriteTimeAfter = System.IO.File.GetLastWriteTime(aa);
                logWindowControl.WriteLine($"実行完了 {aa} 戻り値:{ret} LastWriteTime:{LastWriteTimeBefore}\r\n");
            }
        }

        private void DebugFormPictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }


        private void TESTIMAGEDraw_TexstTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void VARIANT_Type_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            VARIANT_PARTNUMBER_textBox.Enabled = VARIANT_Type_checkBox.Checked;

            List<string> lines = VARIANT_PARTNUMBER_textBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            var oneline = ConvertToRangeString(lines);
            TESTIMAGEDraw_TexstTextBox.Text = oneline;
        }

        /// <summary>
        /// コレクション
        ///  "XX-12345-060",
        ///  "XX-12345-061",
        ///  "XX-12345-062",
        ///  から、文字列　"XX-12345-060~062"を得る
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public string ConvertToRangeString(List<string> inputList)
        {
            if (inputList == null || inputList.Count == 0)
            {
                return string.Empty;
            }

            inputList.Sort(); // 文字列をソート

            string currentPrefix = null;
            int? startNumber = null;
            int? endNumber = null;
            string prefix = null;

            foreach (var item in inputList)
            {
                string[] parts = item.Split('-');
                if (parts.Length != 3)
                {
                    continue; // フォーマットが異なる場合は無視
                }

                prefix = $"{parts[0]}-{parts[1]}";
                int number;
                if (!int.TryParse(parts[2], out number))
                {
                    continue; // 数字部分が変換できない場合は無視
                }

                if (currentPrefix == null)
                {
                    currentPrefix = prefix;
                    startNumber = number;
                    endNumber = number;
                }
                else if (prefix == currentPrefix && number == endNumber + 1)
                {
                    endNumber = number;
                }
                else
                {
                    currentPrefix = prefix;
                    startNumber = number;
                    endNumber = number;
                }
            }

            if (startNumber != null && endNumber != null)
            {
                if (startNumber == endNumber)
                {
                    return $"{currentPrefix}-{startNumber:D3}";
                }
                else
                {
                    return $"{currentPrefix}-{startNumber:D3}~{endNumber:D3}";
                }
            }

            return string.Empty;
        }


        private void TabControl01_Paint(object sender, PaintEventArgs e)
        {
            mainForm.CommitPrinters.SetComboBox(ref PrinterSel_comboBox);
            mainForm.CommitPrinters.SetComboBox(ref PrinterSel_comboBox2);
        }

        private void PrinterSel_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            printerSelIndex1 = PrinterSel_comboBox.SelectedIndex;
        }

        private void PrinterSel_comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            printerSelIndex2 = PrinterSel_comboBox2.SelectedIndex;

        }

        private void GetPrinterStatus_button_Click(object sender, EventArgs e)
        {
            StringBuffer stringBuffer = new StringBuffer();


            List<PrinterInfo> resultGetCommitPrinterInfo = Task.Run(() =>
            {
                RemoteClientDRAWCAPTURE rmc_DRAWCAPTURE = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDC);
                rmc_DRAWCAPTURE.ClientTimeOut = 10000;

                List<PrinterInfo> result = rmc_DRAWCAPTURE.GetCommitPrinterInfo(objectConvNew: objectConvNew2_CheckBox.Checked, WriteLine: stringBuffer.AppendLine);

                return result;
            }).Result;

            stringBuffer.AppendLine($"----------------------------------------------------------------------");

            foreach (var x in resultGetCommitPrinterInfo)
            {
                stringBuffer.AppendLine(
                    $"{x.PrinterName} Ready: {x.Ready}\r\n" +
                    $"\tIsPrinterFailure: {x.IsPrinterFailure}\r\n" +
                    $"\tPrinterXmlFileName: {x.PrinterXmlFileName}\r\n" +
                    $" \tPrinterDescription: {x.PrinterDescription}\r\n" +
                    $"----------------------------------------------------------------------");

            }

            logWindowControl.WriteLine(stringBuffer.GetBufferContents());

        }

        private void GetCommitPrinterStatusButton_Click(object sender, EventArgs e)
        {
            StringBuffer stringBuffer = new StringBuffer();



            List<SharedClassLibrary.PrinterStatus> printerStatuses = Task.Run(() =>
            {
                RemoteClientSYSTEMWATCH rmc_SYSTEMWATCH = new RemoteClientSYSTEMWATCH(SccConfig.Config.StageServerHost, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameSW);
                rmc_SYSTEMWATCH.ClientTimeOut = 10000;

                var result2 = rmc_SYSTEMWATCH.GetJSON_CommitPrinterStatuss(SccConfig.Config.StageServerHost, WriteLine: stringBuffer.AppendLine);
                return result2;
            }).Result;

            if (printerStatuses != null)
            {
                foreach (var x in printerStatuses)
                {
                    stringBuffer.AppendLine($"{x.PrinterName} NumberOfJobs : {x.NumberOfJobs}");
                }

                logWindowControl.WriteLine(stringBuffer.GetBufferContents());
            }


        }

        private void logwindowClear_button_Click(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("----------------------------------------------------------------------------------------------------------------------------");

        }

        private void PrinterSel_comboBox_DropDown(object sender, EventArgs e)
        {
            logWindowControl.Clear();
            mainForm.CommitPrinters.GetData(objectConvNew: true, logWindowControl.WriteLine);

            mainForm.CommitPrinters.SetComboBox(ref PrinterSel_comboBox);

        }
    }


}
