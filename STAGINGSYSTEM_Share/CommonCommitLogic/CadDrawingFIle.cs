using SasaLib.NumberingSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCommitLogic
{
    public class CadDrawingFile
    {
        /// <summary>
        /// コミットしようとした部品番号
        /// </summary>
        public string commitTarget_partnumber;

        public string ActiveDocFullFilename;

        public string CadDrawingFileNameWithoutExtension
        {
            get
            {
                if (ActiveDocFullFilename != null)
                {
                    return System.IO.Path.GetFileNameWithoutExtension(ActiveDocFullFilename);
                }
                else
                {
                    return null;
                }
            }
        }

        public bool WarrningButtonEnabled;

        public string CompareResultMsg;

    }
}
