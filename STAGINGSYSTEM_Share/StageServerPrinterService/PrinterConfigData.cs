using System.Collections.Generic;
using System.Drawing.Printing;
using SasaLib.PrintConfig;
using System;
using System.Xml.Serialization;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using SasaLib;

namespace ToyoStageService
{
    /// <summary>
    /// プリンタ設定を読み出すクラス（jプリンタ毎にインスタンス化される）
    /// </summary>
    public class PrinterConfigData
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        // プリンタは故障中ではないか
        internal bool IsPrinterFailure { get; private set; }

        // プリンタの準備完了か
        internal bool Ready { get; private set; } = false;

        // プリンター毎の設定ファイル
        internal PrinterConfig configobj = new PrinterConfig();

        // 現在のプリンター名（Windowsが認識しているドライバ名）
        internal string PrinterName { get; private set; }

        /// <summary>
        /// このプリンタ設定の別名
        /// </summary>
        internal string PrinterAliasName { get; private set; }

        internal string PrinterShortCutName { get; private set; }

        /// <summary>
        /// このプリンタ設定の説明文
        /// </summary>
        internal string PrinterDescription { get; private set; }

        internal string PrinterSeetingConfigXMLFile { get; private set; }

        // プリンタに指定する用紙サイズのコレクション
        List<PaperSize> globalPaperSizeCollection;

        // プリンタに指定する用紙供給元のコレクション
        List<PaperSource> globalPaperSourceColelection;

        // 印刷時のオフセット値
        internal struct Offset
        {
            public int X;
            public int Y;
        }

        // 印刷時の実行アカウント（ドメイン）
        internal string Print_Domain { get; private set; }

        // 印刷時の実行アカウント（ユーザー）
        internal string Print_User { get; private set; }

        // 印刷時の実行アカウント（パスワード）
        internal string Print_UserPlanePassword { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// PrinerConfigFileを読込ます。
        /// </summary>
        /// <param name="PrinterConfigFilePath"></param>
        public PrinterConfigData(string PrinterConfigFilePath)
        {
            /// PrinterConfigFilePath　存在確認
            if (SasaLib.FileFolder.FileExists(PrinterConfigFilePath) == false)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERconfig", EventLogEntryType.Error, 7001, $"{AssemblyInternalName} PrinterConfigData(...)　{PrinterConfigFilePath}が見つかりません");
                Ready = false;
                return;
            }

            /// PrinterConfigFilePath から プリンタ設定を読込
            if (ReadPrinterSeeting(SasaLib.FileFolder.GetTargetPath(PrinterConfigFilePath)) == false)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERconfig", EventLogEntryType.Error, 7001, $"{AssemblyInternalName} ReadPrinterSeeting({PrinterConfigFilePath})がfalseを返しました");
                Ready = false;
                return;
            }

            /// 
            if (string.IsNullOrWhiteSpace(configobj.PrinterName))
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERconfig", EventLogEntryType.Error, 7001, $"{AssemblyInternalName} PrinterConfigData({PrinterConfigFilePath}) PrinterName が未設定です");
            }
            PrinterName = configobj.PrinterName;

            if (string.IsNullOrWhiteSpace(configobj.PrinterAliasName))
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERconfig", EventLogEntryType.Error, 7001, $"{AssemblyInternalName} PrinterConfigData({PrinterConfigFilePath}) PrinterAliasName が未設定です");
            }
            PrinterAliasName = configobj.PrinterAliasName;

            if (string.IsNullOrWhiteSpace(configobj.PrinterDescription))
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERconfig", EventLogEntryType.Error, 7001, $"{AssemblyInternalName} PrinterConfigData({PrinterConfigFilePath}) PrinterDescription が未設定です");
            }
            PrinterDescription = configobj.PrinterDescription;

            if (string.IsNullOrWhiteSpace(configobj.PrinterShortCutName))
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERconfig", EventLogEntryType.Error, 7001, $"{AssemblyInternalName} PrinterConfigData({PrinterConfigFilePath}) PrinterShortCutName が未設定です");
            }
            PrinterShortCutName = configobj.PrinterShortCutName;

            IsPrinterFailure = configobj.IsPrinterFailure;

            Print_Domain = configobj.Print_Domain;

            Print_User = configobj.Print_User;
            // パスワードをデコードする
            SasaLib.Encryption encryption = new Encryption(configobj.Print_UserPassword_EncryptionType);
            Print_UserPlanePassword = encryption.Decoding(configobj.Print_UserPassword);

            /// 指定したプリンタで選択できる用紙サイズ情報をすべて取得する
            var paperSizeObjects = SasaLib.Printing.GetPaperSizeObjects(PrinterName);
            if (paperSizeObjects == null)
            {
                Ready = false;
                return;
            }
            else
            {
                globalPaperSizeCollection = paperSizeObjects;
            }

            /// 指定したプリンタで選択できる出力先情報をすべて取得する
            var papserSouceObjects = SasaLib.Printing.GetPapserSouceObjects(PrinterName);
            if (papserSouceObjects == null)
            {
                Ready = false;
                return;
            }
            else
            {
                globalPaperSourceColelection = papserSouceObjects;
            }

            // 準備完了フラグ
            Ready = true;
        }

        /// <summary>
        /// XML設定ファイルを読み込む
        /// </summary>
        public bool ReadPrinterSeeting(string PrinterConfigFileName)
        {
            PrinterSeetingConfigXMLFile = PrinterConfigFileName;

            try
            {
                // プリンタ毎設定ファイルのデシリアライズ準備
                XmlSerializer serializer = new XmlSerializer(typeof(PrinterConfig));
                System.IO.StreamReader sr = new System.IO.StreamReader(PrinterConfigFileName, new System.Text.UTF8Encoding(false));
                // オブジェクトをインスタンスに書き戻す
                configobj = (PrinterConfig)serializer.Deserialize(sr);
                sr.Close();

            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoPRINTERconfig", EventLogEntryType.Error, 7001, $"{AssemblyInternalName} ReadPrinterSeeting()　{PrinterConfigFileName} プリンタ設定読込ミスError:{ex.Message}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 現在のプリンタ設定から 指定したCommonPaperSizeに一致したSystem.Drawing.Printing.PaperSize オブジェクトを返す。
        /// </summary>
        /// <param name="paperSize">CommonPaperSize paperSize</param>
        /// <returns></returns>
        public PaperSize GetPaperSize(CommonPaperSize paperSize)
        {
            foreach (var a in configobj.PrinterSettings)
            {
                if (a.CommonPaperSize == paperSize)
                {
                    foreach (var b in globalPaperSizeCollection)
                    {
                        if (b.PaperName == a.PaperName)
                        {
                            return b;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 現在のプリンタ設定から 指定したCommonPaperSizeに一致したSystem.Drawing.Printing.PaperSource オブジェクトを返す。
        /// </summary>
        /// <param name="paperSize"></param>
        /// <returns></returns>
        public PaperSource GetPaperSource(CommonPaperSize paperSize)
        {
            foreach (var a in configobj.PrinterSettings)
            {
                if (a.CommonPaperSize == paperSize)
                {
                    foreach (var b in globalPaperSourceColelection)
                    {
                        if (b.SourceName == a.SourceName)
                        {
                            return b;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 現在のプリンタ設定から 指定したCommonPaperSizeに一致したLandScapeを返す
        /// </summary>
        /// <param name="paperSize"></param>
        /// <returns></returns>
        public bool GetLandScape(CommonPaperSize paperSize)
        {
            foreach (var a in configobj.PrinterSettings)
            {
                if (a.CommonPaperSize == paperSize)
                {
                    return a.LandScape;
                }
            }
            return false;
        }

        /// <summary>
        /// 印刷時の用紙オフセット
        /// </summary>
        /// <param name="paperSize"></param>
        /// <returns></returns>
        internal Offset GetOffset(CommonPaperSize paperSize)
        {
            foreach (var a in configobj.PrinterSettings)
            {
                if (a.CommonPaperSize == paperSize)
                {
                    Offset offset = new Offset { X = a.Xoffset, Y = a.Yoffset };
                    return offset;
                }
            }
            return new Offset { X = 0, Y = 0 };

        }
    }

}
