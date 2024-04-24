using SasaLib;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using SharedClassLibrary;
using System.Threading.Tasks;

namespace ToyoStageService
{
    /// <summary>
    /// ToyoDRAWCAPTUREREGISTseviceから使用される
    /// </summary>
    public static class RemoteServerCommand_ConnectTest
    {
        static readonly string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        static readonly object Command_LockHandler = new object();

        /// <summary>
        ///  ■接続テスト(Echo Test)
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="namedPipeSrvStream"></param>
        public static void ConnectTest(int serverId, string objectID, int taskId, NamedPipeServerStream namedPipeSrvStream, string eventLogSourceName)
        {
            try
            {
                lock (Command_LockHandler)
                {
                    // コンソールカラー設定
                    //ConsoleColor color = DebugClass.GetConsoleColor(serverId);


                    // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
                    string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);

                    StreamString stst = new StreamString(namedPipeSrvStream);

                    string forClient = null;

                    try
                    {
                        // ③ 任意の文字列を受信
                        forClient = stst.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut, null);
                        DebugClass.ConsoleDebugOut(5, $"■{clientInfo} コマンドサーバー【ConnectTest】クライアントからの入力文字列 [{forClient}]を受信しました");
                    }
                    catch (Exception ex)
                    {
                        SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【ConnectTest】 ReadString(..)に失敗しました。例外検知 {ex.Message}", OutConsole: true);
                    }

                    try
                    {
                        //④ 受信した文字列を返す
                        stst.WriteString(forClient);
                        DebugClass.ConsoleDebugOut(9, $"■{clientInfo} コマンドサーバー【ConnectTest】クライアントへエコー出力 [{forClient}]を返しました 実行終了");
                    }
                    catch (Exception ex)
                    {
                        SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【ConnectTest】WriteString(..) クライアントへエコー出力 [{forClient}]に失敗しました。　例外検知 {ex.Message}", OutConsole: true);
                    }
                }

            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【ConnectTest】 lock内で未ハンドリングの例外検知 {ex.Message} {ex.InnerException}", OutConsole: true);
            }
        }
    }
}
