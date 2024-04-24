using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyoStageService;

namespace SharedClassLibrary
{
    public static class GlovalValues
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        /// <summary>
        /// ロギング
        /// </summary>
        static public SasaLib.Logging Mylog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string ServerVersion { get; set; }

        /// <summary>
        /// 全体ログレベル
        /// </summary>
        public static int ConsoleWriteLevel { get; set; }

        /// <summary>
        /// メールログ送信レベル
        /// </summary>
        public static int MaillogSendMode { get; set; }

    }
}
