using CommonCommitLogic;
using SasaLib.ArcSuitePreview;
using SasaLib.NumberingSupport;
using SasaLib;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Versioning;

namespace CommonCommitLogic
{
    [SupportedOSPlatform("windows")]
    public class CommitSupportCadDrawingFile
    {

        /// <summary>
        /// 
        /// </summary>
        CommitParam commitParam;

        /// <summary>
        /// 
        /// </summary>
        public string CadDocumentFullfileName { get; private set; }

        public string CadDocumentFileName
        {
            get
            {
                if (CadDocumentFullfileName != null)
                    return System.IO.Path.GetFileName(CadDocumentFullfileName);
                else
                    return null;
            }
        }

        private string CadDocumentFileNameWithoutExtension
        {
            get
            {
                if (CadDocumentFullfileName != null)
                    return System.IO.Path.GetFileNameWithoutExtension(CadDocumentFullfileName);
                else
                    return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        SasaLibDelegateWriteLine WriteLine = DebugConsole.WriteLine;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<CadDrawingFile> CadDrwingFileChanged;



        /// <summary>
        /// 
        /// </summary>
        private CadDrawingFile _cadDrawingFile;


        /// <summary>
        /// 
        /// </summary>
        public CadDrawingFile CadDrawingFile
        {
            get { return _cadDrawingFile; }
            set
            {
                if (_cadDrawingFile != value)
                {
                    _cadDrawingFile = value;
                    OnCadDrwingFileChanged(CadDrawingFile);
                }

            }
        }



        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="commitParam"></param>
        /// <param name="WriteLine"></param>
        public CommitSupportCadDrawingFile(CommitParam commitParam, string cadDocumentFullfileName,  SasaLibDelegateWriteLine WriteLine)
        {
            this.commitParam = commitParam;
            this.CadDocumentFullfileName = cadDocumentFullfileName;
            this.WriteLine = WriteLine;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnCadDrwingFileChanged(CadDrawingFile e)
        {
            CadDrwingFileChanged?.Invoke(this, e);
        }




        /// <summary>
        /// CADファイル名が表題欄図面番号と正しいかをチェックする
        /// </summary>
        /// <param name="_partnumber"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        internal async void CheckCadFileName(string _partnumber, IWin32Window owner)
        {

            WriteLine($"■CommitSupportCadDrawingFile.CheckCadFileName(..) 採番システム【{commitParam.NumberingServerName}】へ【{_partnumber}】の採番実績を問い合わせています");

            await Task.Run(() =>
            {
                string resultMsg = "未定義";
                bool warningButton = false;

                try
                {
                    if (_partnumber.ToUpper() != CadDocumentFileNameWithoutExtension.ToUpper())
                    {
                        resultMsg = $"CADﾌｧｲﾙ名 \"{CadDocumentFileName}\" が\r\n表題欄部品(図面)番号 \"{_partnumber}\" と違います確認してください";
                        warningButton = true;
                    }
                    else
                    {
                        resultMsg = $"CADﾌｧｲﾙ名 \"{CadDocumentFileName}\" と\r\n表題欄部品(図面)番号 \"{_partnumber}\"  はマッチしています";
                        warningButton = false;

                    }
                }
                catch (Exception ex)
                {
                    SasaLib.Eventlog.Log.WriteEntry("CommonCommitLogic", EventLogEntryType.Error, 0, $"※CheckArcSuiteRegistedで例外{ex.Message}");
                }
                CadDrawingFile = new CadDrawingFile() { commitTarget_partnumber = _partnumber ,ActiveDocFullFilename = CadDocumentFullfileName , CompareResultMsg  = resultMsg, WarrningButtonEnabled = warningButton };
            });

        }

    }
}
