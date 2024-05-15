using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ToyoStageService
{

    /// <summary>
    /// 
    /// </summary>
    public struct LockObject { public string name; public int Id; }

    /// <summary>
    /// 特定のフラグを上げ下げしてメソッドの実行を停止・再開させる静的クラス
    /// </summary>
    public static class LockHundring
    {
        [SupportedOSPlatform("windows")]

        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        /// <summary>
        /// 
        /// </summary>
        private static List<LockObject> LockList = new List<LockObject>();

        /// <summary>
        /// 
        /// </summary>
        private static List<string> LockFileList = new List<string>();

        /// <summary>
        /// 名前とID指定しロック開始。Removeされるまで待ち続ける
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Id"></param>
        public static void WaitLoop(string name, int Id)
        {
            while (LockHundring.ContainsFind(new LockObject { name = name, Id = Id }))
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoFILElockcontrol", EventLogEntryType.Information, 5106, $"{AssemblyInternalName} LockHundring.WaitLoop({name} , {Id})実行中");
                Task.Delay(2000);
            }
            LockHundring.Add(name, Id);
        }

        /// <summary>
        /// 指定した
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool Remove(string name)
        {
            var lockObject = LockList.Find(x => x.name == name);

            return LockList.Remove(lockObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public static bool Remove(string name, int serverId)
        {
            var lockObject = new LockObject { name = name, Id = serverId };

            return LockList.Remove(lockObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockObject"></param>
        /// <returns></returns>
        public static bool Remove(LockObject lockObject)
        {
            return LockList.Remove(lockObject);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serverID"></param>
        private static void Add(string name, int serverID)
        {
            //SasaLib.Eventlog.Log.WriteEntry("ToyoFILElockcontrol", EventLogEntryType.Information, 5106, $"{AssemblyInternalName} LockHundring.Add({name} ,{serverID}を実行)");
            LockList.Add(new LockObject { name = name, Id = serverID });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static int FindLockObjectId(string name)
        {
            int Id = LockList.Find(x => x.name == name).Id;
            return Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <returns></returns>
        private static LockObject FindLockObject(string GUIDBASE64)
        {
            var ans = LockList.Find(x => x.name == GUIDBASE64);
            return ans;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockObject"></param>
        /// <returns></returns>
        private static bool ContainsFind(LockObject lockObject)
        {
            return LockList.Contains(lockObject);
        }

        /// <summary>
        /// Listの中にGUIDBASE64が1件以上あったらtrue;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool ContainsFind(string name)
        {
            var lockObject = LockList.Find(x => x.name == name);
            return LockList.Contains(lockObject);
        }

    }

    /// <summary>
    /// 作業中のファイルを記録する静的クラス
    /// </summary>
    public static class LockFileTable
    {
        [SupportedOSPlatform("windows")]

        private static List<string> LockFileList = new List<string>();

        /// <summary>
        /// ロック開始
        /// </summary>
        /// <param name="filePath"></param>
        public static void Add(string filePath, [CallerMemberName] string callMemberName = "", [CallerFilePath] string callsouceFullFilename = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (LockFileList.Contains(filePath))
            {
                var callsouceFilename = SasaLib.FileFolder.GetFileName(callsouceFullFilename);
                Console.ForegroundColor = ConsoleColor.Yellow;
                SharedClassLibrary.DebugClass.ConsoleDebugOut(5, $"{filePath}はすでにロック済み。 呼び出し元:ソースファイル:{callsouceFilename},{sourceLineNumber}行,メンバー名：{callMemberName}");
                Console.ResetColor();
            }
            else
            {
                LockFileList.Add(filePath);
            }
        }

        /// <summary>
        /// Listの中ファイルパスが1件以上あったらtrue;
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ContainsFind(string filePath)
        {
            return LockFileList.Contains(filePath);

        }

        /// <summary>
        /// ロック解除
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Remove(string filePath)
        {
            bool ans = LockFileList.Remove(filePath);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"{filePath}をロックリストから削除します");
            return ans;
        }

        /// <summary>
        /// 指定したファイルパスのロックが外れるまで待機する
        /// </summary>
        /// <param name="name">ファイルパス</param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        public static async void WaitLoop(string name, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            sourceFilePath = SasaLib.FileFolder.GetFileName(sourceFilePath);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(5, $"WaitLoop({name})突入。{sourceFilePath},{sourceLineNumber}行,メンバー名：{memberName}");

            while (LockFileTable.ContainsFind(name))
            {
                Console.BackgroundColor = ConsoleColor.Green;
                SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"WaitLoop({name})  {name}は使用中。待機します",ConsoleColor.Red);
                Console.ResetColor();
                await Task.Delay(4000);
            }
            LockFileTable.Add(name);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(5, $"WaitLoop({name})　Add({name})を実行しました");
        }

        /// <summary>
        /// ロック中のファイルを一覧表示（デバッグ用）
        /// </summary>
        public static void ShowLockList()
        {

            if (LockFileList.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (var filepath in LockFileList)
                {
                    SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"ロック中：{filepath}");
                }
                Console.ResetColor();
            }
        }
    }
}
