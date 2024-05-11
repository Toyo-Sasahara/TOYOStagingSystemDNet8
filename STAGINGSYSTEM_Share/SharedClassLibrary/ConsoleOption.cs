using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace ToyoStageService
{
    public static class ConsoleOption
    {
        public static void UnsetEayEditMode()
        {
            // 簡易編集モードフラグをリセット
            var hConsole = Win32.GetStdHandle(Win32.STD_INPUT_HANDLE);
            uint mode;
            Win32.GetConsoleMode(hConsole, out mode);
            Win32.SetConsoleMode(hConsole, mode & ~Win32.ENABLE_QUICK_EDIT_MODE);

        }

        // Win32 API定義クラス
        private static class Win32
        {
            public const int STD_INPUT_HANDLE = -10;

            public const uint ENABLE_QUICK_EDIT_MODE = 0x0040;

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        }
    }
}