//using SasaLib;
//using System;
//using System.Diagnostics;
//using System.IO.Pipes;
//using SharedClassLibrary;
//using System.Reflection;
//using System.Runtime.Versioning;

//namespace ToyoStageService
//{
//    [SupportedOSPlatform("windows")]

//    /// <summary>
//    /// ToyoDRAWCAPTUREREGISTseviceから使用される
//    /// </summary>
//    public static class RemoteServerCommand_ShowPrinterQueue
//    {
//        static readonly string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

//        /// <summary>
//        ///  ■
//        /// </summary>
//        /// <param name="serverId"></param>
//        /// <param name="namedPipeSrvStream"></param>
//        public static void ShowPrinterQueue(int serverId, NamedPipeServerStream namedPipeSrvStream, string eventLogSourceName, PrinterStatuses printerStatuses)
//        {
//            // コンソールカラー設定
//            //ConsoleColor color = DebugClass.GetConsoleColor(serverId);


//            // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
//            string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);

//            DebugClass.ConsoleDebugOut(9, $"■コマンドサーバー【ShowPrinterQueue】実行開始 {clientInfo} {AssemblyInternalName} ");

//            StreamString stst = new StreamString(namedPipeSrvStream);

//            string printerDriverName = null;

//            try
//            {
//                // ③ プリンタ名を受信
//                printerDriverName = stst.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut, null);
//                DebugClass.ConsoleDebugOut(9, $"■コマンドサーバー【ShowPrinterQueue】 {clientInfo} クライアントからの入力文字列 [{printerDriverName}]を受信しました");
//            }
//            catch (Exception ex)
//            {
//                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【ShowPrinterQueue】 ReadString(..)にて例外検知 {ex.Message}", OutConsole:true);
//            }

//            try
//            {
//                //④ 現在のキューを返す
//                var numberOfJobs = SasaLib.PrinterStatus.GetPrintQueue(printerDriverName).NumberOfJobs;
//                stst.WriteString($"プリンター\"{printerDriverName}\" 残りの書類 {numberOfJobs} 件");
//                DebugClass.ConsoleDebugOut(9, $"■コマンドサーバー【ShowPrinterQueue】 {clientInfo} クライアントへエコー出力 [{printerDriverName}]を返しました 実行終了");
//            }
//            catch (Exception ex)
//            {
//                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【ShowPrinterQueue】WriteString(..)にて例外検知 {ex.Message}", OutConsole: true);
//            }
//        }
//    }
//}
