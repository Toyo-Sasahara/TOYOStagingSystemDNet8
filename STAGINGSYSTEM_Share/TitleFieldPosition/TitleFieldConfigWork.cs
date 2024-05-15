using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SasaLib.PrintConfig;
using System.Diagnostics;
using SasaLib;
using TitleFieldPosition;
using System.Runtime.Versioning;

namespace ToyoStageService
{
    [SupportedOSPlatform("windows")]

    /// <summary>
    /// スタンプ用コンフィグ 初期作成
    /// </summary> 
    public class TitleFieldConfigWork
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        public static DateTime ReadTime;

        public static string ConfigFileFullpath { get; set; }

        /// <summary>
        /// 用紙サイズ毎の表題欄位置定義ファイルのデシリアライズ
        /// </summary>
        /// <param name="FullPath"></param>
        public static bool ReadTitleFieldConfig(string FullPath, bool WriteEvent = false)
        {
            ReadTime = DateTime.Now;

            ConfigFileFullpath = FullPath;

            // 設定ファイルの存在確認
            if (FileFolder.FileExists(ConfigFileFullpath) != true)
            {
                MakeTitleFieldConfig(ConfigFileFullpath);
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPconfig", EventLogEntryType.Information, 5000,
                        $"{AssemblyInternalName}\nReadTitleFieldConfig(...) 表題欄位置定義ファイル： \"{ConfigFileFullpath}\" 新規作成しました\n"
                        , false, true
                );
            }

            System.IO.StreamReader sr = null;
            try
            {
                // 用紙サイズ毎のスタンプ設定のデシリアライズ
                XmlSerializer serializer = new XmlSerializer(typeof(TitleFieldConfig));
                sr = new System.IO.StreamReader(FullPath, new System.Text.UTF8Encoding(false));
                // オブジェクトをインスタンスに書き戻す
                TitleFieldConfig.Config = (TitleFieldConfig)serializer.Deserialize(sr);

                sr.Close();

                return true;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Error, 5000,
                    $"{AssemblyInternalName} ReadTitleFieldConfig(...) 表題欄位置定義ファイル：{ConfigFileFullpath}のデシアライズで例外発生\n{ex.Message} {ex.InnerException}"
                    , false, true
                    );
                sr.Close();

                return false;
            }
        }

        static public void MakeTitleFieldConfig(string stampConfigName)
        {
            TitleFieldConfig.Config = new TitleFieldConfig
            {
                COMMENT = "表題欄右下位置を用紙サイズ別に設定する。単位:mm",
                VERSION = "VER1.1",
                TitleFieldPosition = new List<TitleFieldConfig.TitleField>()
                {
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A0L, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A0P, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A1L, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A1P, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A2L, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A2P, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A3L, BottomRightBasePositionX=4.7, BottomRightBasePositionY=9.3},
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A3P, BottomRightBasePositionX=4.7, BottomRightBasePositionY=9.3},
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A4L, BottomRightBasePositionX=4.7, BottomRightBasePositionY=9.3},
                    new TitleFieldConfig.TitleField{CommonPapserSize=CommonPaperSize.A4P, BottomRightBasePositionX=4.7, BottomRightBasePositionY=9.3}
                }
            };



            XmlSerializer serializer = new XmlSerializer(typeof(TitleFieldConfig));
            using (StreamWriter sw = new StreamWriter(stampConfigName, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, TitleFieldConfig.Config);
                SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Information, 5000, $"{AssemblyInternalName}\nMakeStampConfig(...)設定ファイル {stampConfigName} を強制作成しました");
            }
        }

        public static void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TitleFieldConfig));
            using (StreamWriter sw = new StreamWriter(ConfigFileFullpath, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, TitleFieldConfig.Config);
                SasaLib.Eventlog.Log.WriteEntry("TOYOCOMMON", EventLogEntryType.Information, 5000,
                    $"{AssemblyInternalName}\nSave(...) 設定ファイル {ConfigFileFullpath} を現在の変数で保存しました");
            }

        }

    }

}
