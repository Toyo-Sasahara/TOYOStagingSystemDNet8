using StageServerRemote;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToyoMcMfg.Staging.DataBaseConfig;
using ToyoMcMfg.Staging.RemoteObjects;
using ToyoStageService;
using SasaLib;
using System.IO;
using System.Linq;
using System.Threading;
using static System.Resources.ResXFileRef;
using System.Xml.Linq;
using System.Windows.Media.TextFormatting;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Reflection;

#if NETCOREAPP
using System.Runtime.Versioning;
#endif

namespace ServerControlCenterApplication
{
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

            FileRecvTest_Source_ServerFullFIleName_textBox.Text = FileSendTest_Dist_ServerFuleFileName_textBox.Text;

            FileRecvTest_Dist_Server_FullFileName_textBox.Text = System.IO.Path.Combine(@"D:\", System.IO.Path.GetFileName(FileRecvTest_Source_ServerFullFIleName_textBox.Text));

        }

        private void TabControl07_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                System.Windows.Forms.MethodInvoker method = () =>
                {
                    accountUserForm.SetToControls();

                }; if (InvokeRequired) { Invoke(method); } else { method(); }
            });
            accountUserForm.WriteLine = logWindowControl.WriteLine;
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
            if (result == DialogResult.OK)
            {
                FileSendTest_Souce_LocalFullFileName_textBox.Text = openFileDialog1.FileName;

                FileSendTest_Souce_LocalFullFileName_textBox.Text = FileSendTest_Souce_LocalFullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            }
        }

        private void SetSamePath_button_Click(object sender, EventArgs e)
        {
            FileSendTest_Souce_LocalFullFileName_textBox.Text = FileSendTest_Souce_LocalFullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            FileSendTest_Dist_ServerFuleFileName_textBox.Text = System.IO.Path.Combine(@"D:\", System.IO.Path.GetFileName(FileSendTest_Souce_LocalFullFileName_textBox.Text));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSendStart_button_Click(object sender, EventArgs e)
        {
            FileSendTest_Souce_LocalFullFileName_textBox.Text = FileSendTest_Souce_LocalFullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            FileSendTest_Dist_ServerFuleFileName_textBox.Text = FileSendTest_Dist_ServerFuleFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');

            RemoteClientDRAWCAPTURE rmc_DRAWCAPTURE = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameDC
                                                );
            if (System.IO.File.Exists(FileSendTest_Souce_LocalFullFileName_textBox.Text))
            {
                string resultMsg;
                var ans = rmc_DRAWCAPTURE.FileSend(
                    FileSendTest_Souce_LocalFullFileName_textBox.Text, FileSendTest_Dist_ServerFuleFileName_textBox.Text,
                    resultMsg: out resultMsg, objectConvNew: FileSendWriteObjectConvNew_checkBox.Checked, WriteLine: logWindowControl.WriteLine, verbose: true);

                logWindowControl.WriteLine($"■FileSend(..)  実行結果戻り値 = {ans},  out resultMsg = {resultMsg}");
            }
            else
            {
                MessageBox.Show($"{FileSendTest_Souce_LocalFullFileName_textBox.Text} が存在しません");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetSamePathServer_button_Click(object sender, EventArgs e)
        {
            FileRecvTest_Source_ServerFullFIleName_textBox.Text = FileRecvTest_Source_ServerFullFIleName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            FileRecvTest_Dist_Server_FullFileName_textBox.Text = System.IO.Path.Combine(@"D:\", System.IO.Path.GetFileName(FileRecvTest_Source_ServerFullFIleName_textBox.Text));

        }

        private void FileReceveStart_button_Click(object sender, EventArgs e)
        {
            FileRecvTest_Source_ServerFullFIleName_textBox.Text = FileRecvTest_Source_ServerFullFIleName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            FileRecvTest_Dist_Server_FullFileName_textBox.Text = FileRecvTest_Dist_Server_FullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');

            RemoteClientDRAWCAPTURE rmc_DRAWCAPTURE = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName,
                                    SccConfig.Config.ClientUserName,
                                    SccConfig.Config.ClientUserPassword,
                                    SccConfig.Config.ClsLogon,
                                     SccConfig.Config.StageServerHost,
                                     SccConfig.Config.PipeNameDC
                                    );


            string msg;
            var ans = rmc_DRAWCAPTURE.FileRecv(FileRecvTest_Source_ServerFullFIleName_textBox.Text, FileRecvTest_Dist_Server_FullFileName_textBox.Text, out msg, objectConvNew: ReceveFileObjectConvNew_checkBox.Checked, WriteLine: LogWindowWriteLine, Verbose: true);

        }

        private void GetFileList_button_Click(object sender, EventArgs e)
        {
            FileRecvTest_Source_ServerFullFIleName_textBox.Text = FileRecvTest_Source_ServerFullFIleName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            FileRecvTest_Dist_Server_FullFileName_textBox.Text = FileRecvTest_Dist_Server_FullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');

            RemoteClientDRAWCAPTURE rmc_DRAWCAPTURE = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName,
                                    SccConfig.Config.ClientUserName,
                                    SccConfig.Config.ClientUserPassword,
                                    SccConfig.Config.ClsLogon,
                                     SccConfig.Config.StageServerHost,
                                     SccConfig.Config.PipeNameDC
                                    );


            string msg;
            List<string> GetFileLists;
            string ResultAndMsg;
            var ans = rmc_DRAWCAPTURE.GetFileList(ServerSourceFolderNaeme_textBox.Text, SearchPath_textBox.Text, out GetFileLists, out ResultAndMsg, objectConvNew: objectConvNew_GetFileLst_CheckBox.Checked, true, logWindowControl.WriteLine);

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
            RemoteClientSYSTEMWATCH rmc_SYSTEMWATCH = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameSW
                                                );
            float availableMemory;
            var result = rmc_SYSTEMWATCH.GetAvailableMemory(out availableMemory, objectConvNew: objectConvNew_checkBox.Checked, WriteLine: LogWindowWriteLine);

        }

        private void GetDRusedMemory_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH rmc_SYSTEMWATCH = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameSW
                                                );
            long availableMemory;
            var result = rmc_SYSTEMWATCH.GetUsedMemory("ToyoDRAWREGISTservice", out availableMemory, objectConvNew: objectConvNew_checkBox.Checked, WriteLine: LogWindowWriteLine);

        }

        private void GetDCusedMemory_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH rmc_SYSTEMWATCH = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameSW
                                                );
            long availableMemory;
            var result = rmc_SYSTEMWATCH.GetUsedMemory("ToyoDRAWCAPTUREservice", out availableMemory, objectConvNew: objectConvNew_checkBox.Checked, WriteLine: LogWindowWriteLine);

        }

        private void GetSWusedMemory_button_Click(object sender, EventArgs e)
        {
            RemoteClientSYSTEMWATCH rmc_SYSTEMWATCH = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                                                SccConfig.Config.ClientUserName,
                                                SccConfig.Config.ClientUserPassword,
                                                SccConfig.Config.ClsLogon,
                                                 SccConfig.Config.StageServerHost,
                                                 SccConfig.Config.PipeNameSW
                                                );
            long availableMemory;
            var result = rmc_SYSTEMWATCH.GetUsedMemory("ToyoSTAGINGSYSTEMwatch", out availableMemory, objectConvNew: objectConvNew_checkBox.Checked, WriteLine: LogWindowWriteLine);

        }

        public struct ParamX
        {
            public string Field { get; set; }
            public object Value { get; set; }
            public SqlDbType SqlDBType { get; set; }
        }

        public class SqlFieldValuex
        {
            /// <summary>
            /// フィールド
            /// </summary>
            public string Field { get; set; }

            /// <summary>
            /// データタイプ
            /// </summary>
            public SqlDbType SqlDBType { get; set; }

            public List<ParamX> ParamXs = new List<ParamX>();
        }


        private void JsonTest_button_Click(object sender, EventArgs e)
        {

            SqlFieldValuex fieldValue = new SqlFieldValuex
            {
                Field = "FieldName",
                SqlDBType = SqlDbType.VarChar,
                ParamXs = new List<ParamX>
                {
                    new ParamX
                    {
                        Field = "ParamField1",
                        Value = "ParamValue1",
                        SqlDBType = SqlDbType.VarChar
                    },
                    new ParamX
                    {
                        Field = "ParamField2",
                        Value = 123,  // 例として数値を設定
                        SqlDBType = SqlDbType.Int
                    }
                    // 追加の ParamX を必要に応じて初期化することができます
                }
            };

            List<SqlFieldValuex> sqlFieldValues = new List<SqlFieldValuex>() { fieldValue, fieldValue, fieldValue };

            var options = new JsonSerializerOptions { Converters = { new SqlFieldValuexConverter() }, WriteIndented = true };

            var objectConverter = new SasaLib.PIPE.ObjectConverter<List<SqlFieldValuex>>();

            string jsontxt;
            var a = objectConverter.FromObjectToByteArrayViaJsonSerializer(sqlFieldValues, out jsontxt, options: options);

            string jsontxt2;
            List<SqlFieldValuex> fieldValueSetRet = objectConverter.FromByteArrayToObjectViaJsonSerializer(a, out jsontxt2, options);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<SqlSearchStringValue> sqlSearchStringValues = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue {Field = "0", Logic = "A", Ooperator ="D" , Value = "E"},
                new SqlSearchStringValue {Field = "1", Logic = "B", Ooperator ="E" , Value = "R"},
                new SqlSearchStringValue {Field = "2", Logic = "C", Ooperator ="F" , Value = "T"},
            };


            var objectConverter = new SasaLib.PIPE.ObjectConverter<List<SqlSearchStringValue>>();

            string jsonText;
            byte[] bytes = objectConverter.FromObjectToByteArrayViaJsonSerializer(sqlSearchStringValues, out jsonText);

            string jsontxt2;
            List<SqlSearchStringValue> fieldValueSetRet = objectConverter.FromByteArrayToObjectViaJsonSerializer(bytes, out jsontxt2);

        }

        private void FieldValuseSets_JSONCONV_button_Click(object sender, EventArgs e)
        {
            List<FieldValueSet> fieldValueSets = new List<FieldValueSet>()
            {
                new FieldValueSet
                {
                    Message = "AA", Sucess = true,
                    Params = new List<FieldValueSet.Param>
                    {
                        new FieldValueSet.Param
                        {Field = "", SqlDBType = SqlDbType.Int, Value = 100
                        }
                    }
                },
                new FieldValueSet
                {
                    Message = "BB", Sucess = true,
                    Params = new List<FieldValueSet.Param>
                    {
                        new FieldValueSet.Param
                        {Field = "", SqlDBType = SqlDbType.Int, Value = 100
                        }
                    }
                }

            };

            var options = new JsonSerializerOptions { Converters = { new FieldValueSetJsonConverter() }, WriteIndented = true };

            var objectConverter = new SasaLib.PIPE.ObjectConverter<List<FieldValueSet>>();

            string outtext1;
            byte[] bytes = objectConverter.FromObjectToByteArrayViaJsonSerializer(fieldValueSets, out outtext1, options: options);

            string outtext2;
            List<FieldValueSet> fieldValueSetRet = objectConverter.FromByteArrayToObjectViaJsonSerializer(bytes, out outtext2, options);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            // サンプルデータの作成
            List<ApprovedCancel> approvedCancels = new List<ApprovedCancel>
            {
                new ApprovedCancel
                {
                    FieldValueSet = new FieldValueSet
                    {
                        Message = "Sample Message",
                        Params = new List<FieldValueSet.Param>
                        {
                            new FieldValueSet.Param { Field = "Field1", Value = "Value1", SqlDBType = SqlDbType.VarChar },
                            new FieldValueSet.Param { Field = "Field2", Value = 123, SqlDBType = SqlDbType.Int }
                        }
                    },
                    AUTHOR = true
                },
                new ApprovedCancel
                {
                    FieldValueSet = new FieldValueSet
                    {
                        Message = "Another Message",
                        Params = new List<FieldValueSet.Param>
                        {
                            new FieldValueSet.Param { Field = "Field3", Value = DateTime.Now, SqlDBType = SqlDbType.DateTime }
                        }
                    },
                    AUTHOR = false
            }
        };
            var options = new JsonSerializerOptions { Converters = { new ApprovedCancelJsonConverter() }, WriteIndented = true };

            var objectConverter = new SasaLib.PIPE.ObjectConverter<List<ApprovedCancel>>();

            string outtext1;
            byte[] bytes = objectConverter.FromObjectToByteArrayViaJsonSerializer(approvedCancels, out outtext1, options: options);

            string outtext2;
            List<ApprovedCancel> fieldValueSetRet = objectConverter.FromByteArrayToObjectViaJsonSerializer(bytes, out outtext2, options: options);

            if (approvedCancels == fieldValueSetRet)
            {
                MessageBox.Show("等しい");
            }
            else
            {
                MessageBox.Show("等しくない");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            FieldValueSet fieldValueSet_in = new FieldValueSet
            {
                Message = "AA",
                Sucess = true,
                Params = new List<FieldValueSet.Param>
                    {
                        new FieldValueSet.Param
                        {Field = "", SqlDBType = SqlDbType.Int, Value = 100
                        }
                    }
            };


            var options = new JsonSerializerOptions { Converters = { new FieldValueSetJsonConverter() }, WriteIndented = true };
            var objectConverter = new SasaLib.PIPE.ObjectConverter<FieldValueSet>();

            string outtext1;
            byte[] bytes = objectConverter.FromObjectToByteArrayViaJsonSerializer(fieldValueSet_in, out outtext1, options: null);

            string outtext2;
            FieldValueSet fieldValue_out = objectConverter.FromByteArrayToObjectViaJsonSerializer(bytes, out outtext2, null);


            if (fieldValueSet_in == fieldValue_out)
            {
                MessageBox.Show("等しい");
            }
            else
            {
                MessageBox.Show("等しくない");
            }

        }

        private void logwindowClear_button_Click(object sender, EventArgs e)
        {
            logWindowControl.WriteLine("----------------------------------------------------------------------------------------------------------------------------");

        }

        private void SourceFromLocalFullFileName_textBox_TextChanged(object sender, EventArgs e)
        {
            FileSendTest_Souce_LocalFullFileName_textBox.Text = FileSendTest_Souce_LocalFullFileName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            FileSendTest_Dist_ServerFuleFileName_textBox.Text = System.IO.Path.Combine(@"D:\", System.IO.Path.GetFileName(FileSendTest_Souce_LocalFullFileName_textBox.Text));
        }

        private void FileSendTest_Dist_ServerFuleFileName_textBox_TextChanged(object sender, EventArgs e)
        {
            FileRecvTest_Source_ServerFullFIleName_textBox.Text = FileSendTest_Dist_ServerFuleFileName_textBox.Text;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void SetSamePathServer_button_Click_1(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                FileRecvTest_Source_ServerFullFIleName_textBox.Text = openFileDialog1.FileName;

                FileRecvTest_Source_ServerFullFIleName_textBox.Text = FileRecvTest_Source_ServerFullFIleName_textBox.Text.TrimStart('\"').TrimEnd('\"');
            }

        }

        private void FromJSON_to_Object_button_Click(object sender, EventArgs e)
        {

        }

        private void FromObject_to_JSON_button_Click(object sender, EventArgs e)
        {
        }
    }
}
