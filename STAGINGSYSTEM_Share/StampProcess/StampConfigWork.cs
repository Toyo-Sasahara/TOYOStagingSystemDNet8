using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SasaLib.PrintConfig;
using System.Diagnostics;
using SasaLib;

namespace ToyoStageService
{
    /// <summary>
    /// スタンプ用コンフィグ 初期作成
    /// </summary> 
    public class StampConfigWork
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        public static DateTime ReadTime;

        public static string ConfigFileFullpath { get; set; }

        /// <summary>
        /// 用紙サイズ毎のスタンプ設定のデシリアライズ
        /// </summary>
        /// <param name="FullPath"></param>
        public static bool ReadStampSetting(string FullPath, bool WriteEvent = false)
        {
            ReadTime = DateTime.Now;

            ConfigFileFullpath = FullPath;

            // 設定ファイルの存在確認
            if (FileFolder.FileExists(ConfigFileFullpath) != true)
            {
                MakeStampConfig(ConfigFileFullpath);
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPconfig", EventLogEntryType.Information, 5000,
                        $"{AssemblyInternalName}\nReadStampSetting(...) スタンプ設定ファイル：{ConfigFileFullpath}新規作成しました\n"
                        , false, true
                );
            }

            System.IO.StreamReader sr = null;
            try
            {
                // 用紙サイズ毎のスタンプ設定のデシリアライズ
                XmlSerializer serializer = new XmlSerializer(typeof(StampConfig));
                sr = new System.IO.StreamReader(FullPath, new System.Text.UTF8Encoding(false));
                // オブジェクトをインスタンスに書き戻す
                StampConfig.Config = (StampConfig)serializer.Deserialize(sr);

                sr.Close();

                return true;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoStampProcess", EventLogEntryType.Error, 5000,
                    $"{AssemblyInternalName} ReadStampSetting(...) スタンプ設定ファイル：{ConfigFileFullpath}のデシアライズで例外発生\n{ex.Message} {ex.InnerException}"
                    , false, true
                    );
                sr.Close();

                return false;
            }
        }

        static public void MakeStampConfig(string stampConfigName)
        {
            StampConfig.Config = new StampConfig
            {
                COMMENT = "承認印を押す位置を設定する。用紙サイズ別に設定した表題欄右下位置からの相対位置をmmで指定します",
                VERSION = "VER1.1",
                AUTHOR_STAMP_POS = new System.Windows.Point(104, 0),
                CHECKED_STAMP_POS = new System.Windows.Point(124, 0),
                APPROVED_STAMP_POS = new System.Windows.Point(144, 0),
                TEST_STAMP_POS = new System.Windows.Point(104, 50),
                //TitleFieldPosition = new List<StampConfig.TitleField>()
                //{
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A0L, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A0P, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A1L, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A1P, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A2L, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A2P, BottomRightBasePositionX=10.7, BottomRightBasePositionY=9.3},
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A3L, BottomRightBasePositionX=4.7, BottomRightBasePositionY=9.3},
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A3P, BottomRightBasePositionX=4.7, BottomRightBasePositionY=9.3},
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A4L, BottomRightBasePositionX=4.7, BottomRightBasePositionY=9.3},
                //    new StampConfig.TitleField{CommonPapserSize=CommonPaperSize.A4P, BottomRightBasePositionX=4.7, BottomRightBasePositionY=9.3}
                //}
            };



            XmlSerializer serializer = new XmlSerializer(typeof(StampConfig));
            using (StreamWriter sw = new StreamWriter(stampConfigName, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, StampConfig.Config);
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPconfig", EventLogEntryType.Information, 5000, $"{AssemblyInternalName}\nMakeStampConfig(...)設定ファイル {stampConfigName} を強制作成しました");
            }
        }

        public static void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(StampConfig));
            using (StreamWriter sw = new StreamWriter(ConfigFileFullpath, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, StampConfig.Config);
                SasaLib.Eventlog.Log.WriteEntry("ToyoSTAMPconfig", EventLogEntryType.Information, 5000,
                    $"{AssemblyInternalName}\nSave(...) 設定ファイル {ConfigFileFullpath} を現在の変数で保存しました");
            }

        }

    }

}
