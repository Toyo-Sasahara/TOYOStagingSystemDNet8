using SasaLib;
using SasaLib.PIPE;
using SharedClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
namespace ToyoStageService
{
    [SupportedOSPlatform("windows")]

    /// <summary>
    /// ToyoDRAWCAPTUREREGISTseviceから使用される
    /// </summary>
    public static class RemoteServerCommand_TitleFieldTest
    {
        static readonly string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        /// <summary>
        ///  ■接続テスト(Echo Test)
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="namedPipeSrvStream"></param>
        public static void TitleFieldTest(int serverId, NamedPipeServerStream namedPipeSrvStream, string eventLogSourceName)
        {

            // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
            string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);

            StreamString stst = new StreamString(namedPipeSrvStream);

            string forClient = null;

            // ③サーバーで保存するファイル名を受信
            string serverSaveFullfileName = stst.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut);

            SasaLib.PrintConfig.CommonPaperSize paperSize = SasaLib.PrintConfig.CommonPaperSize.A4P;
            using (var reader = new BinaryReader(namedPipeSrvStream, Encoding.UTF8, true))
            {
                // ④ 用紙サイズ受信
                paperSize = reader.ReadObject<SasaLib.PrintConfig.CommonPaperSize>();

                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Information, 4002, $"{clientInfo} ｸﾗｲｱﾝﾄからの 値 {paperSize}", OutConsole:true);
            }

            float dpi = 400.0f;
            using (var reader = new BinaryReader(namedPipeSrvStream, Encoding.UTF8, true))
            {
                // ⑤DPI受信
                dpi = reader.ReadObject<float>();

                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Information, 4002, $"{clientInfo} ｸﾗｲｱﾝﾄからの 値 {dpi}", OutConsole: true);
            }


            try
            {
                // テストイメージ作成
                TitleFieldPosition.TestTitleFied titleFieldTest = new TitleFieldPosition.TestTitleFied();
                var resultMemorySW = titleFieldTest.CreateTestDrawing(paperSize, dpi, serverSaveFullfileName);


                using (BinaryWriter writer = new BinaryWriter(namedPipeSrvStream, Encoding.UTF8, true))
                {
                    // ⑥MemoryStream送信
                    writer.WriteObject(resultMemorySW);
                }

                resultMemorySW.Close();
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【TitleFieldTest】WriteString(..) クライアントへエコー出力 [{forClient}]に失敗しました。　例外検知 {ex.Message}", OutConsole: true);
            }
        }
    }
}
