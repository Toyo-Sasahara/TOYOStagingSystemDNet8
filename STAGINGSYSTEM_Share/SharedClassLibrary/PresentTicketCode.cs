using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SasaLib;

namespace ToyoStageService
{
    /// <summary>
    /// GUIDコードをクライアントに創出する予定
    /// </summary>
    public class PresentTicketCode
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        GUIDExtensions GUIDExtensions = new GUIDExtensions(true);

        public string CreateTICKETCODE()
        {
            return GUIDExtensions.B64FnameString;
        }
    }
}
