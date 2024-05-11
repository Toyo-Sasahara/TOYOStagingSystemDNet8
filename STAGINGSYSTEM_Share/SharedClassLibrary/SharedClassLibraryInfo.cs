using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class SharedClassLibraryInfo
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
    public static string GetAssmblyVersion()
    {
        System.Reflection.Assembly assembly = Assembly.GetExecutingAssembly();
        System.Reflection.AssemblyName asmName = assembly.GetName();
        System.Version version = asmName.Version;

        return version.ToString();
    }


    public static string GetAssemblyFileName()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        return ver.FileName;
    }

    public static string GetAssemblyProductVersion()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        return ver.ProductVersion;
    }

    public static DateTime GetAssemblyLastWriteTime()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        DateTime dt = System.IO.File.GetLastWriteTime(ver.FileName);
        return dt;
    }
    public static DateTime GetAssemblyCreationTime()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        DateTime dt = System.IO.File.GetCreationTime(ver.FileName);
        return dt;
    }

    public static string GetAssemblyFileVersion()
    {
        System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
        return ver.FileVersion;
    }
}
