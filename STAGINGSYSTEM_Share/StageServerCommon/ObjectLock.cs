using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ToyoMcMfg.Staging
{
    public struct LockObject { public string name; public int Id; }

    public static class LockHundring
    {
        private static List<LockObject> LockList = new List<LockObject>();
        private static List<string> LockFileList = new List<string>();

        public static void Add(string GUIDBASE64, int serverID)
        {
            LockList.Add(new LockObject { name = GUIDBASE64, Id = serverID });
        }

        public static int FindLockObjectId(string GUIDBASE64)
        {
            int Id = LockList.Find(x => x.name == GUIDBASE64).Id;
            return Id;
        }

        public static LockObject FindLockObject(string GUIDBASE64)
        {
            var ans = LockList.Find(x => x.name == GUIDBASE64);
            return ans;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockObject"></param>
        /// <returns></returns>
        public static bool ContainsFind(LockObject lockObject)
        {
            return LockList.Contains(lockObject);
        }

        /// <summary>
        /// Listの中にGUIDBASE64が1件以上あったらtrue;
        /// </summary>
        /// <param name="GUIDBASE64"></param>
        /// <returns></returns>
        public static bool ContainsFind(string GUIDBASE64)
        {
            var lockObject = LockList.Find(x => x.name == GUIDBASE64);
            return LockList.Contains(lockObject);

        }
        public static bool Remove(string GUIDBASE64)
        {
            var lockObject = LockList.Find(x => x.name == GUIDBASE64);

            return LockList.Remove(lockObject);
        }

        public static bool Remove(string GUIDBASE64, int serverId)
        {
            var lockObject = new LockObject { name = GUIDBASE64, Id = serverId };

            return LockList.Remove(lockObject);
        }

        public static bool Remove(LockObject lockObject)
        {
            return LockList.Remove(lockObject);
        }

        public static void WaitLoop(string name, int Id)
        {
            while (LockHundring.ContainsFind(new LockObject { name = name, Id = Id }))
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine($"{name}はserverID{Id}で使用中。アンロックさえるまで停止します");
                Console.ResetColor();
                Task.Delay(2000);
            }
            LockHundring.Add(name, Id);
        }


    }

    /// <summary>
    /// 
    /// </summary>
    public static class LockFileTable
    {
        private static List<string> LockFileList = new List<string>();

        /// <summary>
        /// ロック開始
        /// </summary>
        /// <param name="filePath"></param>
        public static void Add(string filePath, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (LockFileList.Contains(filePath))
            {
                sourceFilePath = SasaLib.FileFolder.GetFileName(sourceFilePath);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{filePath}はロック済み。無視します 呼び出し元:ソースファイル:{sourceFilePath},{sourceLineNumber}行,メンバー名：{memberName}");
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
            Console.WriteLine($"{filePath}をロックリストから削除します");
            return ans;
        }

        public static async void WaitLoop(string name, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            sourceFilePath = SasaLib.FileFolder.GetFileName(sourceFilePath);
            Console.WriteLine($"WaitLoop({name})突入。{sourceFilePath},{sourceLineNumber}行,メンバー名：{memberName}");

            while (LockFileTable.ContainsFind(name))
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine($"{name}は使用中。待機します");
                Console.ResetColor();
                await Task.Delay(4000);
            }
            LockFileTable.Add(name);
            Console.WriteLine($"WaitLoop({name})　Add({name})を実行しました");
        }

        public static void ShowLockList()
        {
            if (LockFileList.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (var filepath in LockFileList)
                {
                    Console.WriteLine($"ロック中：{filepath}");
                }
                Console.ResetColor();
            }
        }
    }
}
