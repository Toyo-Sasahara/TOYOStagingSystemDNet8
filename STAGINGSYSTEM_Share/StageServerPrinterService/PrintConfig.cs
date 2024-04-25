using System.Collections.Generic;
using SasaLib;
using System.Drawing;
using System.Drawing.Printing;
using SasaLib.PrintConfig;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace ToyoStageService
{
    /// <summary>
    /// 1プリンタ毎に準備するシリアライズされるコンフィグファイル
    /// </summary>
    public class PrinterConfig
    {
        /// メンバ  --------------------------------------------------------------------

        /// <summary>
        /// フォーマットのバージョン
        /// </summary>
        public string VERSION;

        public string COMMENT01 = "コントロールのプリンタ名を記述";
        /// <summary>
        /// コントロールパネルのプリンタ名。選択に使う
        /// </summary>
        public string PrinterName;

        public string COMMENT02 = "別名";
        /// <summary>
        /// 別名 例: COMMIT5
        /// </summary>
        public string PrinterAliasName;

        public string COMMENT03 = "このXMLファイルにリンクするショートカットファイル名を記述。拡張子付き 例: PRINTER5.LNK";
        /// <summary>
        /// このXMLファイルにリンクするショートカットファイル名を記述。拡張子付き
        /// </summary>
        public string PrinterShortCutName;

        public string IsPrinterFailure_Comment = "このプリンターが故障中でサービス不可のときはtrue";
        public bool IsPrinterFailure = false;

        /// <summary>
        /// プリンタの説明
        /// </summary>
        public string PrinterDescription;


        /// <summary>
        /// エラーを送出する最小残りキュー数
        /// </summary>
        public int NumberOfQueuesToSendErrors = 10;

        public struct PaperSize
        {
            public string Comment;
            public CommonPaperSize CommonPaperSize;
            public string PaperName;
            public bool LandScape;
            public int Xoffset;
            public int Yoffset;
            public string SourceName;
        }
        public List<PaperSize> PrinterSettings;

        /// <summary>
        /// 印刷実行時のユーザーアカウント（ドメイン名）
        /// </summary>
        public string Print_Domain;
        /// <summary>
        /// 印刷実行時のユーザーアカウント（ユーユーザー名）
        /// </summary>
        public string Print_User;

        /// <summary>
        /// 印刷実行時のユーザーのパスワードの暗号化方式
        /// </summary>
        public string Print_UserPassword_EncryptionType = "SasaAuth3.1";

        /// <summary>
        ///  印刷実行時のユーザーのパスワード（暗号化）
        /// </summary>
        public string Print_UserPassword;

        /// メソッド --------------------------------------------------------------------
        /// <summary>
        /// コンストラクタ（シリアライズのために必要）
        /// </summary>
        public PrinterConfig() { }


        /// <summary>
        /// 基本テンプレートを作成します。このファイルをコピーし、プリンタに合わせて設定を変えます。
        /// </summary>
        /// <param name="confFIle"></param>
        public static void MakePrinterConfigTemplate(string confFIle)
        {

            // パラメータを保存する
            PrinterConfig obj = new PrinterConfig
            {
                VERSION = "VER1023",

                PrinterName = @"コントロールパネルのプリンタ名",
                //

                PrinterSettings = new List<PrinterConfig.PaperSize>()
            {
                /// CommonPaperSizeと、特定のpc3ファイルの LocalMediaNameの対応表
                new PrinterConfig.PaperSize{Comment="イメージデータがA0横向き",CommonPaperSize=CommonPaperSize.A0L,PaperName="A0 (841 x 1189 mm)",Xoffset=0,Yoffset=0,LandScape=true,SourceName="自動トレイ選択" },
                new PrinterConfig.PaperSize{Comment="イメージデータがA0縦向き",CommonPaperSize=CommonPaperSize.A0P,PaperName="A0 (841 x 1189 mm)",Xoffset=0,Yoffset=0,LandScape=false ,SourceName="自動トレイ選択" },
                new PrinterConfig.PaperSize{Comment="イメージデータがA1横向き",CommonPaperSize=CommonPaperSize.A1L,PaperName="A1 (594 x 841 mm)",Xoffset=0,Yoffset=0,LandScape=true ,SourceName="自動トレイ選択"  },
                new PrinterConfig.PaperSize{Comment="イメージデータがA1縦向き",CommonPaperSize=CommonPaperSize.A1P,PaperName="A1 (594 x 841 mm)",Xoffset=0,Yoffset=0,LandScape=false ,SourceName="自動トレイ選択"  },
                new PrinterConfig.PaperSize{Comment="イメージデータがA2横向き",CommonPaperSize=CommonPaperSize.A2L,PaperName="A2 (420 x 594 mm)",Xoffset=0,Yoffset=0,LandScape=true ,SourceName="自動トレイ選択"  },
                new PrinterConfig.PaperSize{Comment="イメージデータがA2縦向き",CommonPaperSize=CommonPaperSize.A2P,PaperName="A2 (420 x 594 mm)",Xoffset=0,Yoffset=0,LandScape=false ,SourceName="自動トレイ選択"  },
                new PrinterConfig.PaperSize{Comment="イメージデータがA3横向き",CommonPaperSize=CommonPaperSize.A3L,PaperName="A3 (297 x 420 mm)",Xoffset=0,Yoffset=0,LandScape=true ,SourceName="自動トレイ選択"  },
                new PrinterConfig.PaperSize{Comment="イメージデータがA3縦向き",CommonPaperSize=CommonPaperSize.A3P,PaperName="A3 (297 x 420 mm)",Xoffset=0,Yoffset=0,LandScape=false  ,SourceName="自動トレイ選択" },
                new PrinterConfig.PaperSize{Comment="イメージデータがA4横向き",CommonPaperSize=CommonPaperSize.A4L,PaperName="A4 (210 x 297 mm)",Xoffset=0,Yoffset=0,LandScape=true ,SourceName="自動トレイ選択"  },
                new PrinterConfig.PaperSize{Comment="イメージデータがA4縦向き",CommonPaperSize=CommonPaperSize.A4P,PaperName="A4 (210 x 297 mm)",Xoffset=0,Yoffset=0,LandScape=false  ,SourceName="自動トレイ選択" },
            }
            };


            XmlSerializer serializer = new XmlSerializer(typeof(PrinterConfig));
            using (StreamWriter sw = new StreamWriter(confFIle, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, obj);
            }

        }

    }
}
