//using SasaLib;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Runtime.Versioning;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Xml.Serialization;
//using SIP = System.IO.Path;
//[SupportedOSPlatform("windows")]


//public static class ImagePositonConfigWork
//{
//    static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

//    /// <summary>
//    /// イメージオフセット情報の読込
//    /// </summary>
//    /// <returns></returns>
//    public static bool ReadImagePositionConfig(string ConfigFileFullpath, SasaLibDelegateWriteLine logtext = null)
//    {
//        if (logtext != null) logtext($"{AssemblyInternalName} ReadImagePositionConfig({ConfigFileFullpath},)を実行開始します");


//        if (FileFolder.FileExists(ConfigFileFullpath) != true)
//        {
//            /// ImageExportConfigの設定ファイルが無い場合は作成
//            var ans = MakeIMAGEPOSXML(ConfigFileFullpath);

//            if (ans)
//            {
//                if (logtext != null) logtext($"{AssemblyInternalName} {ConfigFileFullpath}を新たに作成しました");
//                SasaLib.Eventlog.Log.WriteEntry("GenerateTIFFdrawing", EventLogEntryType.Warning, 7200, $"{AssemblyInternalName} {ConfigFileFullpath}を新たに作成しました");
//            }
//        }

//        // 保存した設定ファイル内容を復元する
//        XmlSerializer serializer = new XmlSerializer(typeof(ImagePositonConfig));
//        System.IO.StreamReader sr = new System.IO.StreamReader(ConfigFileFullpath, new System.Text.UTF8Encoding(false));

//        try
//        {
//            ImagePositonConfig.Config = (ImagePositonConfig)serializer.Deserialize(sr);
//            sr.Close();


//            if (logtext != null) logtext($"{AssemblyInternalName} メージオフセット設定ファイル {ConfigFileFullpath} を読み込みました\n");

//            return true;

//        }
//        catch (InvalidOperationException iex)
//        {
//            sr.Close();
//            //MessageBox.Show($"メージオフセット設定ファイル {ConfigFileFullpath} の読み込みを失敗しました。\n内容 {iex.InnerException}");
//            if (logtext != null) logtext($"{AssemblyInternalName} メージオフセット設定ファイル {ConfigFileFullpath} の読み込みを失敗しました。\n内容 {iex.InnerException}");
//            SasaLib.Eventlog.Log.WriteEntry("GenerateTIFFdrawing", EventLogEntryType.Error, 7200, $"{AssemblyInternalName} メージオフセット設定ファイル {ConfigFileFullpath} の読み込みを失敗しました。\n内容 {iex.InnerException}");
//            return false;
//        }
//        catch (Exception ex)
//        {
//            //MessageBox.Show($"エラー：メージオフセット設定ファイルの読込で例外発生{ex.Message}");
//            //LogsSW.DebugWriteLine($"エラー：ReadImagePositionConfigで例外発生 {ex.Message}");
//            SasaLib.Eventlog.Log.WriteEntry("GenerateTIFFdrawing", EventLogEntryType.Error, 7200, $"{AssemblyInternalName} エラー：ImageExportConfigWork.ReadImagePositionConfig()で例外発生 {ex.Message} ");
//            return false;
//        }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public static void CreateImagePos()
//    {
//    }

//    /// <summary>
//    /// IMAGEPOS.XMLLを強制作成
//    /// </summary>
//    /// <param name="confFIle"></param>
//    /// <returns></returns>
//    public static bool MakeIMAGEPOSXML(string confFile)
//    {
//        ImagePositonConfig.Config = new ImagePositonConfig
//        {
//            VERSION = "これは CAD種類を問わないﾃﾞﾌｫﾙﾄ値です.CADに合わせて編集が必要です",

//            ImageEffects = new List<Setting>()
//                {
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A0L,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = -2, OffsetY = 1 },
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A0P,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = 0, OffsetY = 0 },
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A1L,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = 0, OffsetY = 2 },
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A1P,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = 0, OffsetY = 0 },
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A2L,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = -3, OffsetY = 3 },
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A2P,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = 0, OffsetY = 0 },
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A3L,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = 1, OffsetY = 2 },
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A3P,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = 0, OffsetY = 0 },
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A4L,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = 0, OffsetY = 0 },
//                    new Setting { CommonPaperSize = SasaLib.PrintConfig.CommonPaperSize.A4P,
//                        ImageRotation = System.Drawing.RotateFlipType.RotateNoneFlipNone, OffsetX = 1, OffsetY = -3 }
//                },

//        };

//        XmlSerializer serializerIMAGEPOSConfig = new XmlSerializer(typeof(ImagePositonConfig));

//        try
//        {
//            using (StreamWriter sw = new StreamWriter(confFile, false, Encoding.UTF8))
//            {
//                serializerIMAGEPOSConfig.Serialize(sw, ImagePositonConfig.Config);
//                sw.Close();
//            }
//            return true;
//        }
//        catch (InvalidOperationException iex)
//        {

//            MessageBox.Show($"{AssemblyInternalName} 設定ファイル {confFile} を作成失敗\n内容 {iex.Message}");
//            return false;
//        }

//        catch (Exception ex)
//        {

//            MessageBox.Show($"{AssemblyInternalName} 設定ファイル {confFile} を作成失敗\n内容 {ex.Message}");
//            return false;
//        }

//    }

//    public static bool MakeIMAGEPOSXML(string confFile , string SYSTEM, string VERSION,List<Setting> ImageEffects)
//    {
//        ImagePositonConfig.Config = new ImagePositonConfig
//        {
//            SYSTEM = SYSTEM,

//            VERSION = VERSION,

//            ImageEffects = ImageEffects,
//        };

//        XmlSerializer serializerIMAGEPOSConfig = new XmlSerializer(typeof(ImagePositonConfig));

//        try
//        {
//            using (StreamWriter sw = new StreamWriter(confFile, false, Encoding.UTF8))
//            {
//                serializerIMAGEPOSConfig.Serialize(sw, ImagePositonConfig.Config);
//                sw.Close();
//            }
//            return true;
//        }
//        catch (InvalidOperationException iex)
//        {

//            MessageBox.Show($"{AssemblyInternalName} 設定ファイル {confFile} を作成失敗\n内容 {iex.Message}");
//            return false;
//        }

//        catch (Exception ex)
//        {

//            MessageBox.Show($"{AssemblyInternalName} 設定ファイル {confFile} を作成失敗\n内容 {ex.Message}");
//            return false;
//        }

//    }

//}
