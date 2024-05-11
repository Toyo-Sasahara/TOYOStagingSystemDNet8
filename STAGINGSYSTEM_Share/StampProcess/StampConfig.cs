using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using SasaLib.PrintConfig;
using System;

namespace ToyoStageService
{
    // スタンプ用コンフィグ
    [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    public class StampConfig
    {
        public static StampConfig Config { set; get; }

        public string COMMENT;
        public string VERSION;

        public bool DebugMode = true;

        public string Line1_Font_Name  = "MSゴシック";
        public int Line1_FontSize = 26;
        public int Line1_X  = 55;
        public int Line1_Y  = 40;
        public int Line1_Xe = 215;

        public string Line2_Font_Name = "MSゴシック";
        public int Line2_FontSize = 28;
        public int Line2_X  = 55 - 25;
        public int Line2_Y  = 40 + 55;
        public int Line2_Xe = 215 + 25;

        public string Line3_Font_Name = "MSゴシック";
        public int Line3_FontSize = 26;
        public int Line3_X  = 55;
        public int Line3_Y  = 40 + 55 + 55;
        public int Line3_Xe = 215;


        public string AUTHOR_STAMP_POS_comment = "製図者押印ポジションおよびスケール";
        public System.Windows.Point AUTHOR_STAMP_POS;
        public double AUTHOR_STAMP_SCALE = 1.0;

        public string CHECKED_STAMP_POS_comment = "設計者押印ポジションおよびスケール";
        public System.Windows.Point CHECKED_STAMP_POS;
        public double CHECKED_STAMP_SCALE = 1.0;

        public string APPROVED_STAMP_POS_comment = "承認者押印ポジションおよびスケール";
        public System.Windows.Point APPROVED_STAMP_POS;
        public double APPROVED_STAMP_SCALE = 1.0;

        public string TEST_STAMP_POS_comment = "TEST押印ポジションおよびスケール";
        public System.Windows.Point TEST_STAMP_POS;
        public double TEST_STAMP_SCALE = 1.0;

        //public string COMMNET2 = "<TitleFieldPosition>は、表題欄の右下位置をイメージデータの右下からの相対位置で指定します。";

        //public struct TitleField
        //{
        //    public CommonPaperSize CommonPapserSize;
        //    public double BottomRightBasePositionX;
        //    public double BottomRightBasePositionY;
        //}
        //public List<TitleField> TitleFieldPosition;

        /// <summary>
        /// コンストラクタ（シリアライズのために必要）
        /// </summary>
        public StampConfig() { }


    }

}
