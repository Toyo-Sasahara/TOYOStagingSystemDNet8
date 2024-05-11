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
using System.Text;
using System.Threading.Tasks;

namespace ToyoStageService
{
    /// <summary>
    /// 
    /// </summary>
    static public class RemoteServerCommand_GetVauleList
    {
        /// <summary>
        /// ■GetValue
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="namedPipeSrvStream"></param>
        /// <param name="targetObj"></param>
        public static void GetVauleList(int serverId, string ObjectID, int taskId, NamedPipeServerStream namedPipeSrvStream, Object targetObj, string eventViewerSourceName)
        {
            List<string> datas = ListValueSrings(targetObj);

            // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
            string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);


            using (var writer = new BinaryWriter(namedPipeSrvStream, Encoding.UTF8, true))
            {
                writer.WriteObject(datas);
            }

            DebugConsole.WriteLine($"◆[ﾊﾟｲﾌﾟｻｰﾊﾞ] [ID:{serverId}] List<string>型 を  {clientInfo}  へ送信しました");

        }

        static List<string> ListValueSrings(Object targetObj)
        {
            Type targetObjSystemType = targetObj.GetType();

            List<string> result = new List<string>();

            FieldInfo[] myFieldInfos;
            Type myType = targetObjSystemType;

            // Get the type and fields of FieldInfoClass.
            myFieldInfos = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo myFieldInfo in myFieldInfos)
            {
                string valuestr;
                try
                {

                    Type type = myFieldInfo.FieldType;

                    var valule = myFieldInfo.GetValue(targetObj);

                    valuestr = (string)Convert.ChangeType(valule, TypeCode.String);

                }
                catch (Exception ex)
                {
                    DebugConsole.WriteLine($"例外検知 {ex.Message} {myFieldInfo.Name} = {myFieldInfo.GetValue(targetObj)}");
                    valuestr = "例外発生";
                }

                result.Add($"変数名: {myFieldInfo.Name}\r\n" +
                    $"\t 値: {valuestr}\r\n" +
                    $"\t 型: {myFieldInfo.FieldType}\r\n" +
                    $"\tメンバータイプ: {myFieldInfo.MemberType}\r\n" +
                    $"\tIsPublic: {myFieldInfo.IsPublic} IsFamily: {myFieldInfo.IsFamily}\r\n");

            }
            return result;
        }

    }
}
