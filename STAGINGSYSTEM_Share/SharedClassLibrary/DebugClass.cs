using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// クライアント・サーバー間で利用されるライブラリ
/// C:\Users\sasahara\source\repos\TOYOSTAGINGSYSTEM\SharedClassLibrary\DebugClass.cs
/// </summary>
namespace SharedClassLibrary
{
    public static class DebugClass
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        /// <summary>
        /// 指定したloglevelが全体ログレベルより小さい時コンソール出力を実行する。
        /// </summary>
        /// <param name="loglevel">1から9。</param>
        /// <param name="value"></param>
        [System.Diagnostics.DebuggerStepThrough]
        public static void ConsoleDebugOut(int loglevel, string value, ConsoleColor consoleColor = (ConsoleColor)1, bool viewLvele = true)
        {
            Task.Run(() =>
            {
                if (consoleColor != (ConsoleColor)1)
                    Console.ForegroundColor = consoleColor;
                //
                if (loglevel <= GlovalValues.ConsoleWriteLevel)
                {
                    if (viewLvele == true)
                        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] " + value + $" (表示ﾛｸﾞﾚﾍﾞﾙ:{loglevel}以上/現在:{GlovalValues.ConsoleWriteLevel})");
                    else
                        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] " + value);
                }
                //
                if (consoleColor != (ConsoleColor)1)
                    Console.ResetColor();
            });
        }

        /// <summary>
        /// ServerID毎のテキストカラーを設定
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public static ConsoleColor GetConsoleColor(int serverId)
        {
            ConsoleColor co;

            switch (serverId)
            {
                case 1:
                    co = ConsoleColor.White;
                    break;
                case 2:
                    co = ConsoleColor.DarkGray;
                    break;
                case 3:
                    co = ConsoleColor.Magenta;
                    break;
                case 4:
                    co = ConsoleColor.DarkMagenta;
                    break;
                case 5:
                    co = ConsoleColor.Cyan;
                    break;
                case 6:
                    co = ConsoleColor.DarkCyan;
                    break;
                case 7:
                    co = ConsoleColor.Green;
                    break;
                case 8:
                    co = ConsoleColor.DarkGreen;
                    break;
                case 9:
                    co = ConsoleColor.Yellow;
                    break;
                case 10:
                    co = ConsoleColor.DarkYellow;
                    break;

                default:
                    co = ConsoleColor.White;
                    break;
            }
            return co;
        }
    }
}


