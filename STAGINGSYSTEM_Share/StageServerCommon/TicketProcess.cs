using SasaLib;
using SasaLib.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using SIP = System.IO.Path;
using ToyoMcMfg.Staging.DataBaseConfig;
using SharedClassLibrary;
using System.Windows.Forms;
using System.Runtime.Versioning;

namespace ToyoStageService
{
    [SupportedOSPlatform("windows")]

    /// <summary>
    /// 
    /// </summary>
    public static class TicketProcess
    {
        /// <summary>
        /// 共通属性フォーマットのインスタンスを宣言
        /// </summary>
        public static CommonTicket TicketData { get; set; }

        /// <summary>
        /// ■チケットファイル読出し＆デシリアライズ
        /// </summary>
        /// <param name="CommitFolderTicketFullFilename"></param>
        /// <returns></returns>
        public static bool DeserializeTicketFile(string CommitFolderTicketFullFilename)
        {
            try
            {
                // Ticketの設定ファイルのデシリアライズ準備
                XmlSerializer serializer = new XmlSerializer(typeof(CommonTicket));
                System.IO.StreamReader streamrw = new System.IO.StreamReader(CommitFolderTicketFullFilename, new System.Text.UTF8Encoding(false));

                // オブジェクトを書き戻す
                TicketProcess.TicketData = (CommonTicket)serializer.Deserialize(streamrw);
                streamrw.Close();

                try
                {
                    ///  バイト数に従って文字列長を切り捨てる
                    TicketData.ReplaceOrCretekeyValue("CUSTOMER", 60);
                    TicketData.ReplaceOrCretekeyValue("FIRSTCUSTOMER", 60);
                    TicketData.ReplaceOrCretekeyValue("TITLE", 125);
                    TicketData.ReplaceOrCretekeyValue("PARTNAME", 125);
                    TicketData.ReplaceOrCretekeyValue("DESCRIPTION", 125);
                }
                catch (Exception ex)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoTicketProcess", EventLogEntryType.Error, 5108, $"TicketProcess.DeserializeTicketFile(...) ReplaceOrCreateKeyValue(...) 文字列長切り捨てにて例外検知 {ex.Message}");
                }

                return true;
                // チケットオブジェクト復帰完了
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoTicketProcess", EventLogEntryType.Error, 5108, $"TicketProcess.DeserializeTicketFile({CommitFolderTicketFullFilename})にて例外検知 {ex.Message}");

                return false;
            }
        }

        /// <summary>
        /// ■チケットファイルシリアライズ
        /// </summary>
        /// <param name="CommitFolderTicketFullFilename"></param>
        /// <returns></returns>
        public static bool SerializeTicketFile(string CommitFolderTicketFullFilename)
        {
            try
            {
                // チケットファイルを書き戻す
                XmlSerializer serializerSave = new XmlSerializer(typeof(CommonTicket));

                using (StreamWriter sw = new StreamWriter(CommitFolderTicketFullFilename, false, Encoding.UTF8))
                {
                    serializerSave.Serialize(sw, TicketProcess.TicketData);
                }

                return true;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoTicketProcess", EventLogEntryType.Error, 5108, $"SerializeTicketFile({CommitFolderTicketFullFilename})にて例外検知 {ex.Message}");
                return false;
            }
        }
    }

}
