using SasaLib;
using StageServerRemote;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace AdditinalConfig
{
    /// <summary>
    /// ｱﾄﾞｲﾝメイン設定ﾌｧｲﾙで使用されるパブリックフィールドの変更（メモリ上）
    /// </summary>
    public struct SetConfigValue
    {
        public string FieldName;
        public string TypeName;
        public System.Xml.XmlNode Value;
    }

    public enum WriteMode
    {
        NoOverwrite, ForceOverwrite,
    }

    public enum Attribute
    {
        ReadOnly, ReadWrite
    }

    public struct GetFulFileName
    {
        public string Comment;
        public string ServerSideFullFileName;
        public string LocalSideFullFileName;
        /// <summary>
        /// 既存ﾌｧｲﾙが存在した場合の対象包　
        /// </summary>
        public WriteMode LocalSideFileWriteMode;
        public bool UnZip;
        public string UnZipLocalFolder;
        public bool DeleteZipFileAfterUnzipped;
    }

    public struct SetAttributeFile
    {
        public string Commnent;
        public string LocalSideFullFileName;
        public Attribute Attribute;
    }

    public struct RemoveFile
    {
        public string Commnent;
        public string LocalSideFullFileName;
    }

    public struct SetRegistry
    {
        public string Commnent;
        public string KeyName;
        public string ValueName;
        public Microsoft.Win32.RegistryValueKind RegistryValueKind;
        public string Value;
    }

    public struct RemoveRegistry
    {
        public string Commnent;
        public string OpenKeyName;
        public string SubKeyName;
        public string ValueName;
    }

    public struct ExecuteProcess
    {
        public string Commnent;
        public string FileName;
        public string Arguments;
        public string UserName;
        public string Password;
        public string EncryptedPassword;
        public string Domain;
        public bool StdOut;
        public bool StdErr;
    }

    public class OSCommandExecutionSupport
    {

        public bool DisplayLogger { get; set; } = false;


        SasaLibDelegateWriteLine WriteLine;

        string StageServerHost;
        string PipeNameDC;

        string ClientDomainName;
        string ClientUserName;
        string ClientUserPassword;
        bool ClsLogon = false;


        public OSCommandExecutionSupport(string StageServerHost, string PipeNameDC, bool ClsLogon, string ClientDomainName = null, string ClientUserName = null,string  ClientUserPassword = null, SasaLibDelegateWriteLine WriteLine = null)
        {
            this.StageServerHost = StageServerHost;
            this.PipeNameDC = PipeNameDC;
            this.ClientDomainName = ClientDomainName;
            this.ClientUserName = ClientUserName;
            this.ClientUserPassword = ClientUserPassword;
            this.ClsLogon = ClsLogon;
            this.WriteLine = WriteLine;
        }


    }

}
