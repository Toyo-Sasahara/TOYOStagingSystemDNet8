using SasaLib;
using SasaLib.PIPE;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using SharedClassLibrary;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Reflection;

namespace ToyoStageService
{
    /// <summary>
    /// メモリ情報を取得
    /// </summary>
    public static class RemoteServerCommand_GetAvailableMemory
    {
        static readonly string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        static readonly object Command_LockHandler = new object();

        /// <summary>
        ///  ■
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="namedPipeSrvStream"></param>
        public static void GetAvailableMemory(int serverId, string objectID, int taskId, NamedPipeServerStream namedPipeSrvStream, string eventLogSourceName)
        {
            try
            {
                lock (Command_LockHandler)
                {


                    // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
                    string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);


                    try
                    {
                        // "Memory" パフォーマンスカウンタを指定して PerformanceCounter インスタンスを作成
                        PerformanceCounter memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

                        // 利用可能なメモリーの量を取得
                        float availableMemory = memoryCounter.NextValue();

                        DebugConsole.WriteLine($"利用可能なメモリー: {availableMemory} MB");


                        using (BinaryWriter writer = new BinaryWriter(namedPipeSrvStream, Encoding.UTF8, true))
                        {
                            // ⑥MemoryStream送信
                            writer.WriteObject(availableMemory);
                        }

                    }
                    catch (Exception ex)
                    {
                        SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【GetAvailableMemory】に失敗しました。　例外検知 {ex.Message}", OutConsole: true);
                    }
                }

            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【GetAvailableMemory】 lock内で未ハンドリングの例外検知 {ex.Message} {ex.InnerException}", OutConsole: true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="objectID"></param>
        /// <param name="taskId"></param>
        /// <param name="namedPipeSrvStream"></param>
        /// <param name="eventLogSourceName"></param>
        public static void GetMemoryUsageWorkingSetSize(int serverId, string objectID, int taskId, NamedPipeServerStream namedPipeSrvStream, string eventLogSourceName)
        {
            try
            {
                lock (Command_LockHandler)
                {
                    // 接続元のｸﾗｲｱﾝﾄ情報を文字列化
                    string clientInfo = SasaLib.PIPE.NamedPipeClientInfo.GetClientHostAndUser(namedPipeSrvStream, serverId);


                    DebugClass.ConsoleDebugOut(9, $"■コマンドサーバー【GetMemoryUsageWorkingSetSize】実行開始 {clientInfo} {AssemblyInternalName} ");

                    StreamString stst = new StreamString(namedPipeSrvStream);

                    string asmName = null;

                    try
                    {
                        //  アセンブリ名を受信
                        asmName = stst.ReadString(StageServerConfig.Config.ReadWriteStreamStringDefaultTimeOut, null);
                        DebugClass.ConsoleDebugOut(9, $"■コマンドサーバー【GetMemoryUsageWorkingSetSize】 {clientInfo} クライアントからの入力文字列 [{asmName}]を受信しました");
                    }
                    catch (Exception ex)
                    {
                        SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【ShowPrinterQueue】 ReadString(..)にて例外検知 {ex.Message}", OutConsole: true);
                    }

                    long availableMemory = 0;

                    try
                    {
                        var process = GetProcessByName(asmName);

                        if (process != null)
                        {
                            var assembly = GetAssemblyFromProcess(process);

                            availableMemory = GetMemoryUsageWorkingSetSize(assembly);

                            DebugConsole.WriteLine($"アセンブリ\"{asmName}\" が使用しているWrokingSet メモリーサイズ: {availableMemory} バイト");

                        }
                        else
                        {
                            DebugConsole.WriteLine($"アセンブリ\"{asmName}\" が見つかりません");
                        }

                        using (BinaryWriter writer = new BinaryWriter(namedPipeSrvStream, Encoding.UTF8, true))
                        {
                            // ⑥MemoryStream送信
                            writer.WriteObject(availableMemory);
                        }

                    }
                    catch (Exception ex)
                    {
                        SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【GetAvailableMemory】に失敗しました。　例外検知 {ex.Message}", OutConsole: true);
                    }
                }

            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Error, 4001, $"※{eventLogSourceName}コマンドサーバー【GetAvailableMemory】 lock内で未ハンドリングの例外検知 {ex.Message} {ex.InnerException}", OutConsole: true);
            }

        }

        static Process GetProcessByName(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0 ? processes[0] : null;
        }

        static Assembly GetAssemblyFromProcess(Process process)
        {
            try
            {
                string assemblyPath = process.MainModule?.FileName;

                if (!string.IsNullOrEmpty(assemblyPath))
                {
                    // モジュールのファイルパスからアセンブリを取得
                    return Assembly.LoadFrom(assemblyPath);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 使用中のアセンブリのメモリサイズをバイトで返す
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static long GetMemoryUsageWorkingSetSize(Assembly assembly)
        {

            string fullFileName = assembly.Location;
            string assbelyName = assembly.GetName().Name;

            // すべてのプロセスを取得
            Process[] allProcesses = Process.GetProcesses();

            ProcessModule targetModule = null;
            long processSize = 0;
            foreach (Process process in allProcesses)
            {
                DebugConsole.WriteLine($"Process.ProcessName:\"Id:{process.Id} , {process.ProcessName}\" Process.PrivateMemorySize64:{process.PrivateMemorySize64:n0} byte");

                if (process.ProcessName.Equals(assbelyName, StringComparison.OrdinalIgnoreCase))
                {
                    processSize = process.WorkingSet64;

                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\"---------------------------------------------");
                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\" Process.PrivateMemorySize64:{process.PrivateMemorySize64:n0} byte");

                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\" Process.VirtualMemorySize64:{process.VirtualMemorySize64:n0} byte");
                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\" Process.PeakVirtualMemorySize64:{process.PeakVirtualMemorySize64:n0} byte");
                    
                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\" Process.PagedMemorySize64:{process.PagedMemorySize64:n0} byte");
                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\" Process.PeakPagedMemorySize64:{process.PeakPagedMemorySize64:n0} byte");

                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\" Process.WorkingSet64:{process.WorkingSet64:n0} byte");
                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\" Process.MaxWorkingSet:{process.MaxWorkingSet:n0} byte");
                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\" Process.MinWorkingSet:{process.MinWorkingSet:n0} byte");
                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\" Process.PeakWorkingSet64:{process.PeakWorkingSet64:n0} byte");
                    ServerLog.Logging.LogRotateWriteLine($"Id:{process.Id} , Process.ProcessName:\"{process.ProcessName}\"++++++++++++++++++++++++++++++++++++++++++++++");

                    //foreach (ProcessModule module in process.Modules)
                    //{
                    //    DebugConsole.WriteLine($"ProcessModule.FileName:\"{module.FileName}\" , ProcessModule.ModuleMemorySize:{module.ModuleMemorySize} byte");
                    //    if (module.FileName.Equals(fullFileName, StringComparison.OrdinalIgnoreCase))
                    //    {
                    //processSize = targetModule?.ModuleMemorySize ?? 0;
                    //        //break;
                    //    }
                    //}


                }
            }

            ServerLog.Logging.Flash();

            // アセンブリのメモリ使用量（バイト）を返す
            return processSize;
        }

        private static string GetAssemblyPath(Assembly assembly)
        {
            try
            {
                // アセンブリのCodeBaseプロパティから絶対ファイルパスを取得
                UriBuilder uri = new UriBuilder(new Uri(assembly.CodeBase));
                return Uri.UnescapeDataString(uri.Path);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
