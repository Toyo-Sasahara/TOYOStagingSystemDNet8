/// ToyoSTAGINGSYSTEMwatch service用 PIPEconnectionLoop
using SasaLib;
using SasaLib.PIPE;
using SharedClassLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ToyoStageService
{
    [SupportedOSPlatform("windows")]

    /// <summary>
    /// 
    /// </summary>
    public static class RemoteServerCommand_SetMemMapdFile
    {
        /// <summary>
        /// リモートでメモリマップドファイルを設定
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="namedPipeSrvStream"></param>
        public static void SetMemMapdFile(int serverId, NamedPipeServerStream namedPipeSrvStream, string eventLogSourceName)
        {
            // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
            string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);

            // コンソールカラー設定
            //ConsoleColor color = DebugClass.GetConsoleColor(serverId);

            StreamString stst = new StreamString(namedPipeSrvStream);

            string LabelName;

            LabelName = stst.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut, null);

            MemoryMapdFile mmf = new MemoryMapdFile(LabelName);

            var typestr = stst.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut, null);// mmapd型の種類を受信

            switch (typestr)
            {
                case "string":
                    var resultStr = stst.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut, null);
                    mmf.Write(resultStr);

                    if (StageServerConfig.Config.RecordOfNormaEvents_RelatedToMMAP)
                        SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Information, 4002, $"RemoteServerCommand_SetMemMapdFile.SetMemMapdFile(..)\n{clientInfo} ｸﾗｲｱﾝﾄからの文字列 \"{resultStr}\" を mmf:{LabelName}に格納", false);

                    break;

                case "bool":
                    using (var reader = new BinaryReader(namedPipeSrvStream, Encoding.UTF8, true))
                    {
                        var resultBool = reader.ReadObject<bool>();
                        mmf.Write(resultBool);
                        if (StageServerConfig.Config.RecordOfNormaEvents_RelatedToMMAP)
                            SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Information, 4002, $"RemoteServerCommand_SetMemMapdFile.SetMemMapdFile(..)\n{clientInfo} ｸﾗｲｱﾝﾄからのbool値 {resultBool} を mmf:{LabelName}に格納", false);
                    }
                    break;

                case "int":
                    using (var reader = new BinaryReader(namedPipeSrvStream, Encoding.UTF8, true))
                    {
                        var resultInt = reader.ReadObject<int>();
                        mmf.Write(resultInt);
                        if (StageServerConfig.Config.RecordOfNormaEvents_RelatedToMMAP)
                            SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Information, 4002, $"RemoteServerCommand_SetMemMapdFile.SetMemMapdFile(..)\n{clientInfo} ｸﾗｲｱﾝﾄからのint値 {resultInt} を mmf:{LabelName}に格納", false);
                    }
                    break;

                default:
                    SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4002, $"RemoteServerCommand_SetMemMapdFile.SetMemMapdFile(..)\n{clientInfo} : エラー　型名\"{typestr}\"は設定されていない", false);
                    break;
            }
        }
    }
}
