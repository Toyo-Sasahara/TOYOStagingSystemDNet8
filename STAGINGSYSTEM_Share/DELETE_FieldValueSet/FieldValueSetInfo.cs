using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class FieldValueSetInfo
{
    public static System.Diagnostics.FileVersionInfo GetAssemblyInfo()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        return ver;
    }

    /// <summary>
    /// アセンブリバージョンを取得
    /// </summary>
    /// <returns></returns>
    public static string GetAssemblyVersion()
    {
        System.Reflection.Assembly assembly = Assembly.GetExecutingAssembly();
        System.Reflection.AssemblyName asmName = assembly.GetName();
        System.Version version = asmName.Version;

        return version.ToString();
    }

    public static string GetAssemblyProductVersion()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        return ver.ProductVersion;
    }


    public static string GetAssemblyFileVersion()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        return ver.FileVersion;
    }


    public static string GetAssemblyFileName()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        return ver.FileName;
    }

    public static DateTime GetAssemblyLastWriteTime()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        DateTime dt = System.IO.File.GetLastWriteTime(ver.FileName);
        return dt;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string GetAssemblyFileMD5()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string md5 = SasaLib.FileFolder.GetMD5FileHash(ver.FileName);
        return md5;
    }


}
