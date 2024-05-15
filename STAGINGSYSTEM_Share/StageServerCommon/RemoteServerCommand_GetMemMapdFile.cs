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
    public static class RemoteServerCommand_GetMemMapdFile
    {
        /// <summary>
        /// メモリマップドファイルを読み出すリモートコマンド
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="namedPipeSrvStream"></param>
        public static void GetMemMapdFile(int serverId, NamedPipeServerStream namedPipeSrvStream, string eventLogSourceName)
        {
            // コンソールカラー設定
            //ConsoleColor color = DebugClass.GetConsoleColor(serverId);

            // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
            string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);

            StreamString stst = new StreamString(namedPipeSrvStream);

            string LabelName;

            LabelName = stst.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut, null); // mmapdラベル名を受信

            MemoryMapdFile mmf = new MemoryMapdFile(LabelName);

            var typestr = stst.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut, null);// mmapd型の種類を受信
            switch (typestr)
            {
                case "string":
                    var mmfResultstr = mmf.ReadStr();

                    // ④結果をクライアントに送出
                    stst.WriteString(mmfResultstr);

                    if (StageServerConfig.Config.RecordOfNormaEvents_RelatedToMMAP)
                        SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Information, 4001, $"RemoteServerCommand_GetMemMapdFile.GetMemMapdFile(..)\n{clientInfo} :mmf:{LabelName} の内容をｸﾗｲｱﾝﾄに送信 {typestr} = \"{mmfResultstr}\"", false);

                    break;

                case "bool":
                    var mmfResultbool = mmf.ReadBool();

                    using (var writer = new BinaryWriter(namedPipeSrvStream, Encoding.UTF8, true))
                    {
                        // ④結果をクライアントに送出
                        writer.WriteObject(mmfResultbool);
                        if (StageServerConfig.Config.RecordOfNormaEvents_RelatedToMMAP)
                            SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Information, 4001, $"RemoteServerCommand_GetMemMapdFile.GetMemMapdFile(..)\n{clientInfo} :mmf:{LabelName} の内容をｸﾗｲｱﾝﾄに送信 {typestr} = {mmfResultbool}", false);
                    }
                    break;

                case "int":
                    var mmfResultint = mmf.ReadInt();
                    using (var writer = new BinaryWriter(namedPipeSrvStream, Encoding.UTF8, true))
                    {
                        // ④結果をクライアントに送出
                        writer.WriteObject(mmfResultint);
                        if (StageServerConfig.Config.RecordOfNormaEvents_RelatedToMMAP)
                            SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Information, 4001, $"RemoteServerCommand_GetMemMapdFile.GetMemMapdFile(..)\n{clientInfo} :mmf:{LabelName} の内容をｸﾗｲｱﾝﾄに送信 {typestr} = {mmfResultint}", false);
                    }

                    break;
                default:
                    SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"RemoteServerCommand_GetMemMapdFile.GetMemMapdFile(..)\n{clientInfo} : エラー　型名 \"{typestr}\"は設定されていない", false);

                    break;
            }
        }
    }
}
