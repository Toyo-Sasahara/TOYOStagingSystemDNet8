/// ToyoSTAGINGSYSTEMwatch service用 PIPEconnectionLoop
using SasaLib;
using SasaLib.PIPE;
//using SharedClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ToyoStageService
{
    [SupportedOSPlatform("windows")]

    /// <summary>
    /// 
    /// </summary>
    static public class RemoteServerCommand_GetVaule
    {
        /// <summary>
        /// ■GetValue
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="namedPipeSrvStream"></param>
        /// <param name="targetObj"></param>
        public static void GetVaule(int serverId, string ObjectID, int taskId, NamedPipeServerStream namedPipeSrvStream, Object targetObj, string eventViewerSourceName)
        {
            // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
            string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);


            StreamString stst = new StreamString(namedPipeSrvStream);

            stst.WriteString("OK. Send  Variable name.");

            int timeout = StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut;

            string VariableName = stst.ReadString(timeout, null);

            SasaLib.Eventlog.Log.WriteEntry(eventViewerSourceName, EventLogEntryType.Information, 4002, $"CommandSubAnalyze_CommitConfigVaule(..) {clientInfo} よりVariableNameを受信しました。{VariableName} ", false);

            Type targetObjSystemType = targetObj.GetType();

            FieldInfo field = targetObjSystemType.GetField(VariableName);
            using (var writer = new BinaryWriter(namedPipeSrvStream, Encoding.UTF8, true))
            {
                writer.WriteObject(field.FieldType);
            }
            SasaLib.Eventlog.Log.WriteEntry(eventViewerSourceName, EventLogEntryType.Information, 4002, $"変数 {VariableName} の 型;{field.FieldType} を  {clientInfo}  へ送信しました", false);

            var valule = field.GetValue(targetObj);
            using (var writer = new BinaryWriter(namedPipeSrvStream, Encoding.UTF8, true))
            {
                writer.WriteObject(valule, timeout);
            }
            SasaLib.Eventlog.Log.WriteEntry(eventViewerSourceName, EventLogEntryType.Information, 4002, $"変数 {VariableName} の 価;{valule}　を  {clientInfo}  へ送信しました", false);

            SasaLib.Eventlog.Log.WriteEntry(eventViewerSourceName, EventLogEntryType.Information, 4002, $"RemoteServerCommand_GetVersion.GetVersion(..)\n{clientInfo}よりサーバーバージョンを要求されました。", false);

        }
    }
}
