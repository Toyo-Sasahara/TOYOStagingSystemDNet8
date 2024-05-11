/// ToyoSTAGINGSYSTEMwatch service用 PIPEconnectionLoop
using SasaLib;
using SasaLib.PIPE;
//using SharedClassLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ToyoStageService
{
    /// <summary>
    /// 
    /// </summary>
    public static class RemoteServerCommand_SetValue
    {
        /// <summary>
        /// ■SetValue
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="namedPipeSrvStream"></param>
        public static bool SetValue(int serverId, string ObjectID, int taskId, NamedPipeServerStream namedPipeSrvStream, Object targetObj, string eventViewerSourceName)
        {
            // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
            string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);

            StreamString stst = new StreamString(namedPipeSrvStream);

            stst.WriteString("OK. Send CommitConfig Variable name.");
            int timeout = StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut;
            string VariableName = stst.ReadString(timeout, null);

            SasaLib.Eventlog.Log.WriteEntry(eventViewerSourceName, EventLogEntryType.Information, 4002, $"{clientInfo}よりVariableNameを受信しました。{VariableName} ", false);


            Type typeOfListString = targetObj.GetType();

            FieldInfo field = typeOfListString.GetField(VariableName);

            if (field != null)
            {
                using (var writer = new BinaryWriter(namedPipeSrvStream, Encoding.UTF8, true))
                {
                    writer.WriteObject(field.FieldType);
                }
                stst.WriteString("OK. Send Object Data.");

                object receveObj;
                using (BinaryReader reader = new BinaryReader(namedPipeSrvStream, Encoding.UTF8, true))
                {
                    receveObj = reader.ReadObject<Object>();
                }
                SasaLib.Eventlog.Log.WriteEntry(eventViewerSourceName, EventLogEntryType.Information, 4002, $"ReadObject = {receveObj}", false);

                field.SetValue(targetObj, receveObj);

                SasaLib.Eventlog.Log.WriteEntry(eventViewerSourceName, EventLogEntryType.Information, 4002, $"スタティック変数 {targetObj} オブジェクトの {VariableName} に = {receveObj} （{field.FieldType} 型）を代入しました", false);

                return true;

            }
            else
            {
                SasaLib.Eventlog.Log.WriteEntry(eventViewerSourceName, EventLogEntryType.Information, 4002, $"※スタティック変数 {targetObj} オブジェクトに {VariableName} は見つかりませんでした", false);

                return false;
            }

        }
    }
}
