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
//    public static class RemoteServerCommand_MakePrinterConfigTemplate
//    {
//        static readonly string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

//        /// <summary>
//        ///  ■
//        /// </summary>
//        /// <param name="serverId"></param>
//        /// <param name="namedPipeSrvStream"></param>
//        public static void Execute(int serverId, NamedPipeServerStream namedPipeSrvStream, string eventLogSourceName, PrinterStatuses printerStatuses)
//        {
//            // コンソールカラー設定
//            //ConsoleColor color = DebugClass.GetConsoleColor(serverId);

//            int ReadStreamStringTimeOut = StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut;

//            // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
//            string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);

//            DebugClass.ConsoleDebugOut(9, $"■コマンドサーバー【MakePrinterConfigTemplate】実行開始 {clientInfo} {AssemblyInternalName} ");

//            StreamString stst = new StreamString(namedPipeSrvStream);

//            string printerTemplateFullFlename = null;

//            try
//            {
//                // ③ プリンタテンプレートフルファイル名を受信
//                printerTemplateFullFlename = stst.ReadString(ReadStreamStringTimeOut, null);
//                DebugClass.ConsoleDebugOut(9, $"■コマンドサーバー【MakePrinterConfigTemplate】{clientInfo} クライアントからの入力文字列 [{printerTemplateFullFlename}]を受信しました");
//            }
//            catch (Exception ex)
//            {
//                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【ShowPrinterQueue】 ReadString(..)にて例外検知 {ex.Message}", OutConsole: true);
//            }



//            PrinterConfig.MakePrinterConfigTemplate(printerTemplateFullFlename);

//            try
//            {
               
//                stst.WriteString($"プリンターテンプレートファイル \"{printerTemplateFullFlename}\" を作成");
//                DebugClass.ConsoleDebugOut(1, $"■コマンドサーバー【MakePrinterConfigTemplate】{clientInfo} クライアントへエコー出力 [{printerTemplateFullFlename}]を返しました 実行終了");
//            }
//            catch (Exception ex)
//            {
//                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【MakePrinterConfigTemplate】WriteString(..)にて例外検知 {ex.Message}", OutConsole: true);
//            }
//        }
//    }
//}
