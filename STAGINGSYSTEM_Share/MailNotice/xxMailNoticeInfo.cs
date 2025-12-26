//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//public static class MailNoticeInfo
//{
//    public static System.Diagnostics.FileVersionInfo GetAssemblyInfo()
//    {
//        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
//        return ver;
//    }

//    /// <summary>
//    /// アセンブリバージョンを取得
//    /// </summary>
//    /// <returns></returns>
//    public static string GetAssmblyVersion()
//    {
//        System.Reflection.Assembly assembly = Assembly.GetExecutingAssembly();
//        System.Reflection.AssemblyName asmName = assembly.GetName();
//        System.Version version = asmName.Version;

//        return version.ToString();
//    }

//}
