using SasaLib.PrintConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitleFieldPosition
{
    public class TitleFieldConfig
    {
        public static TitleFieldConfig Config { set; get; }

        public string COMMENT;
        public string VERSION;

        public struct TitleField
        {
            public CommonPaperSize CommonPapserSize;
            public double BottomRightBasePositionX;
            public double BottomRightBasePositionY;
        }
        public List<TitleField> TitleFieldPosition;

        /// <summary>
        /// コンストラクタ（シリアライズのために必要）
        /// </summary>
        public TitleFieldConfig() { }

    }
}
