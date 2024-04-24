using SasaLib.NumberingSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCommitLogic
{
    public class TypeNumber
    {
        /// <summary>
        /// コミットしようとした部品番号
        /// </summary>
        public string commitTarget_partnumber;

        public NumberTypeConfig.DrawingType drawingType;
        public string TypeName;
        public bool isVariant;
        public string VariantSuffixMIN;
        public string VariantSuffixMAX;
    }
}
