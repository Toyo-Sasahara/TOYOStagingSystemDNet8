using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedClassLibrary
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PrinterInfo
    {
        /// <summary>
        /// サーバー側のコントロールパネルでのプリンタ名(C:\ProgramData\TOYOCOMMON\Printersフォルダの、各XMLﾌｧｲﾙの名前ではない)
        /// </summary>
        public string PrinterName;

        /// <summary>
        /// C:\ProgramData\TOYOCOMMON\PrintersフォルダのXMLﾌｧｲﾙに記述されたエイリアス
        /// </summary>
        public string PrinterAliasName;

        /// <summary>
        /// 
        /// </summary>
        public string PrinterShortCutName { get; set; }

        /// <summary>
        ///  C:\ProgramData\TOYOCOMMON\PrintersフォルダのXMLﾌｧｲﾙに記述されたDescription
        /// </summary>
        public string PrinterDescription;

        /// <summary>
        /// C:\ProgramData\TOYOCOMMON\PrintersフォルダのXMLﾌｧｲﾙ名
        /// </summary>
        public string PrinterXmlFileName;

        /// <summary>
        /// プリンター使用準備完了ならtrue
        /// </summary>
        public bool Ready;

        /// <summary>
        /// プリンターが故障中ならtrue
        /// </summary>
        public bool IsPrinterFailure;

        /// <summary>
        /// 
        /// </summary>
        public int NumberOfQueuesToSendErrors;

        /// <summary>
        /// プリンタドライバがポーズ中なら true
        /// </summary>
        public bool IsDriverPaused;

    }
}
