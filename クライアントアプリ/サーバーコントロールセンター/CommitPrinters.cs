using SasaLib.PrintConfig;
using SharedClassLibrary;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerControlCenterApplication
{
    internal class CommitPrinters
    {
        public List<KeyValuePair<string, string>> resultGetCommitPrinterShortCutName;

        public List<KeyValuePair<string, string>> resultGetCommitPrinterNameAndAlias;

        public List<KeyValuePair<string, bool>> resultGetCommitPrinterIsFailStatus;

        public List<PrinterInfo> resultGetCommitPrinterInfo;

        public List<KeyValuePair<string, string>> resultGetCommitPrinterSettingFromPaperSize;



        public CommitPrinters()
        {
        }


        public async void GetData()
        {
            RemoteClientDRAWCAPTURE remoteClientDRAWCAPTURE = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDC);
            remoteClientDRAWCAPTURE.ClientTimeOut = 10000;

            
            resultGetCommitPrinterShortCutName =  Task.Run(() =>
            {
                var result = remoteClientDRAWCAPTURE.GetCommitPrinterShortCutName();
                return result;
            }).Result;

            resultGetCommitPrinterNameAndAlias = Task.Run(() =>
            {
                var result = remoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias();
                return result;
            }).Result;

            resultGetCommitPrinterIsFailStatus = Task.Run(() =>
            {
                var result = remoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus();
                return result;
            }).Result;

            resultGetCommitPrinterInfo = Task.Run(() =>
            {
                var result = remoteClientDRAWCAPTURE.GetCommitPrinterInfo();
                return result;
            }).Result;

            var resultTask = await Task.Run(() =>
            {
                var result = remoteClientDRAWCAPTURE.GetCommitPrinterSettingFromPaperSize();
                return result;
            });

            resultGetCommitPrinterSettingFromPaperSize = resultTask;

        }

        /// <summary>
        /// コンボボックスに List<KeyValuePair<string, string>>　のオブジェクトを設定。表示に KeyValuePair の Keyを、値に Valueを使用する
        /// </summary>
        /// <param name="comboBox"></param>
        public void SetComboBox(ref ComboBox comboBox)
        {
            if (resultGetCommitPrinterShortCutName != null && resultGetCommitPrinterShortCutName.Count > 0)
            {
                comboBox.DataSource = resultGetCommitPrinterShortCutName;
                comboBox.DisplayMember = "key";
                comboBox.ValueMember = "value";
                //comboBox.SelectedIndex = 0;
            }

        }


    }
}
