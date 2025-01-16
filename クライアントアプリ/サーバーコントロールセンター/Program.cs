using SasaLib;
using SasaLib.VariableControlPipeServer;
using SharedClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerControlCenterApplication
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var isSwitchSystemRuntimeSerializationUseNewMaxArraySize=DotNetFrameworkAppContextSetSwitch.SwitchSystemRuntimeSerializationUseNewMaxArraySize(true);
            DebugConsole.WriteLine($"isSwitchSystemRuntimeSerializationUseNewMaxArraySize={isSwitchSystemRuntimeSerializationUseNewMaxArraySize}");

            // "ServerControlConfig.xml" を読み込み()
            SccConfigWork.ReadConfig(@"ServerControlConfig.xml");


            if (string.IsNullOrWhiteSpace(SccConfig.Config.LogFolder))
            {
                GlovalValues.Mylog = new SasaLib.Logging(SccConfigWork.GetAppConfigFolder(), $"{Environment.UserName}.log");
            }
            else
            {
                GlovalValues.Mylog = new SasaLib.Logging(SccConfig.Config.LogFolder, $"{Environment.UserName}.log");

            }

            GlovalValues.Mylog.WriteLine("■アプリケーションスタート ServerControlConfig.xml読み込み済み");


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //終了時に設定を書き出し
            SccConfig.Config.Save();

            GlovalValues.Mylog.Close();

        }
    }
}


