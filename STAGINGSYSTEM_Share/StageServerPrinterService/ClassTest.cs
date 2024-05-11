#region TEST CODE
using SasaLib;
using System.Diagnostics;

namespace ToyoStageService
{
    public static class ClassTest
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        static public EventsSummary safeEvt = new EventsSummary();

        public static void Run()
        {
            PrinterConfig.MakePrinterConfigTemplate(@"D:\TEMPLATE.XML");
            bool IsError;
            PrinterService printService = new PrinterService(@"D:\A4.TIF", @"D:\RICOH SPC830M-A.XML", safeEvt, out IsError);
            safeEvt.SendEntry("ToyoDRAWCAPTUREservice", EventLogEntryType.Information, 7000, $"{AssemblyInternalName} 印刷テスト情報", "印刷テスト情報");


            printService.PrintExecute("",@"D:\印刷前.bmp");
        }
    }


}
#endregion

