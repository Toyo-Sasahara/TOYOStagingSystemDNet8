using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SasaLib;
using StageServerRemote;

namespace ServerControlCenterApplication
{
    public class RemoteClientMemoryMapdFile
    {
        public enum ServerMode { Master, Slave }

        public ServerMode SERVERMODE { get; set; }           // ①

        public bool ArcSuiteRegistrationCycle { get; set; }  // ②

        public bool COMMITACCEPT { get; set; }               // ③

        public string COMMITACCEPTFALSEMSG { get; set; }     // ④

        public bool APPROVINGACCEPT { get; set; }            // ⑤

        public string APPROVINGACCEPTFALSEMSG { get; set; }  // ⑥

        public bool ImmediateryPrinting { get; set; }        // ⑦

        public bool TESTMODE { get; set; }                   // ⑧

        public bool RemoteServerCommand_DATABASE_EventView_Send { get; set; }

        private string ClientDomainName { get; set; }
        public string StageServerHost { get; set; }
        private string ClientUserName { get; set; }
        private  string ClientUserPassword { get; set; }
        private bool ClsLogonDummy { get; set; }

        public RemoteClientMemoryMapdFile(string ForcedDomainName, string ForcedUserName, string ForcedUserPassword, bool ForcedAccountFlag, string PipeServerName, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            ClientDomainName = ForcedDomainName;
            StageServerHost = PipeServerName;

            ClientUserName =ForcedUserName;
            ClientUserPassword = ForcedUserPassword;
            ClsLogonDummy = ForcedAccountFlag;
        }

        /// <summary>
        /// メモリマップドファイルを読み出し該当するプロパティへ値をセットする
        /// </summary>
        /// <param name="writeLine"></param>
        /// <returns></returns>
        public bool ReadMMPFAndSetInTheProperties(SasaLibDelegateWriteLine writeLine = null)
        {
            if (writeLine == null)
            {
                writeLine = Console.WriteLine;
            }
            bool ans;
            object SERVERMODE;
            ans = readMMPFValue("SERVERMODE", "int", out SERVERMODE, writeLine);
            if (ans)
                this.SERVERMODE = (ServerMode)SERVERMODE;

            object ArcSuiteRegistrationCycle;
            ans = readMMPFValue("ArcSuiteRegistrationCycle", "bool", out ArcSuiteRegistrationCycle, writeLine);
            if (ans)
                this.ArcSuiteRegistrationCycle = (bool)ArcSuiteRegistrationCycle;

            object COMMITACCEPT;
            ans = readMMPFValue("COMMITACCEPT", "bool", out COMMITACCEPT, writeLine);
            if (ans)
                this.COMMITACCEPT = (bool)COMMITACCEPT;

            object COMMITACCEPTFALSEMSG;
            ans = readMMPFValue("COMMITACCEPTFALSEMSG", "string", out COMMITACCEPTFALSEMSG, writeLine);
            if (ans)
                this.COMMITACCEPTFALSEMSG = (string)COMMITACCEPTFALSEMSG;

            object APPROVINGACCEPT;
            ans = readMMPFValue("APPROVINGACCEPT", "bool", out APPROVINGACCEPT, writeLine);
            if (ans)
                this.APPROVINGACCEPT = (bool)APPROVINGACCEPT;

            object APPROVINGACCEPTFALSEMSG;
            ans = readMMPFValue("APPROVINGACCEPTFALSEMSG", "string", out APPROVINGACCEPTFALSEMSG, writeLine);
            if (ans)
                this.APPROVINGACCEPTFALSEMSG = (string)APPROVINGACCEPTFALSEMSG;

            object ImmediateryPrinting;
            ans = readMMPFValue("ImmediateryPrinting", "bool", out ImmediateryPrinting, writeLine);
            if (ans)
                this.ImmediateryPrinting = (bool)ImmediateryPrinting;

            object TESTMODE;
            ans = readMMPFValue("TESTMODE", "bool", out TESTMODE, writeLine);
            if (ans)
                this.TESTMODE = (bool)TESTMODE;

            object RemoteServerCommand_DATABASE_EventView_Send;
            ans = readMMPFValue("RemoteServerCommand_DATABASE_EventView_Send", "bool", out RemoteServerCommand_DATABASE_EventView_Send, writeLine);
            if (ans)
                this.RemoteServerCommand_DATABASE_EventView_Send = (bool)RemoteServerCommand_DATABASE_EventView_Send;

            return ans;
        }

        /// <summary>
        /// プロパティを読み出し該当するメモリマップドファイルへ値をセットする
        /// </summary>
        /// <param name="writeLine"></param>
        /// <returns></returns>
        public bool ReadPropertiesAndSetInTheMMPF(SasaLibDelegateWriteLine writeLine)
        {
            bool ans1 = writeMMPFValue("SERVERMODE", "int",(int)SERVERMODE);
            writeLine($"SetValue(\"SERVERMODE\", \"int\",(int){SERVERMODE})の戻り値 {ans1}");

            bool ans2 = writeMMPFValue("ArcSuiteRegistrationCycle", "bool", (bool)ArcSuiteRegistrationCycle);
            writeLine($"SetValue(\"ArcSuiteRegistrationCycle\", \"bool\",(bool){ArcSuiteRegistrationCycle})の戻り値 {ans2}");

            bool ans3 = writeMMPFValue("COMMITACCEPT", "bool", (bool)COMMITACCEPT);
            writeLine($"SetValue(\"COMMITACCEPT\", \"bool\",(bool){COMMITACCEPT})の戻り値 {ans3}");

            bool ans4 = writeMMPFValue("COMMITACCEPTFALSEMSG", "string", (string)COMMITACCEPTFALSEMSG);
            writeLine($"SetValue(\"COMMITACCEPTFALSEMSG\", \"string\",(string){COMMITACCEPTFALSEMSG})の戻り値 {ans4}");

            bool ans5 = writeMMPFValue("APPROVINGACCEPT", "bool", (bool)APPROVINGACCEPT);
            writeLine($"SetValue(\"APPROVINGACCEPT\", \"bool\",(bool){APPROVINGACCEPT})の戻り値 {ans5}");

            bool ans6 = writeMMPFValue("APPROVINGACCEPTFALSEMSG", "string", (string)APPROVINGACCEPTFALSEMSG);
            writeLine($"SetValue(\"APPROVINGACCEPTFALSEMSG\", \"string\",(string){APPROVINGACCEPTFALSEMSG})の戻り値 {ans6}");

            bool ans7 = writeMMPFValue("ImmediateryPrinting", "bool", (bool)ImmediateryPrinting);
            writeLine($"SetValue(\"ImmediateryPrinting\", \"bool\",(bool){ImmediateryPrinting})の戻り値 {ans7}");

            bool ans8 = writeMMPFValue("TESTMODE", "bool", (bool)TESTMODE);
            writeLine($"SetValue(\"TESTMODE\", \"bool\",(bool){TESTMODE})の戻り値 {ans8}");

            bool ans9 = writeMMPFValue("RemoteServerCommand_DATABASE_EventView_Send", "bool", (bool)RemoteServerCommand_DATABASE_EventView_Send);
            writeLine($"SetValue(\"RemoteServerCommand_DATABASE_EventView_Send\", \"bool\",(bool){RemoteServerCommand_DATABASE_EventView_Send})の戻り値 {ans9}");

            return ans1 & ans2 & ans3 & ans4 & ans5 & ans6 & ans7 & ans8 & ans9;
        }

        /// <summary>
        /// 指定した名前のメモリマップドファイルを読み出す
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="typestr"></param>
        /// <param name="Result"></param>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        bool readMMPFValue(string Label, string typestr, out object Result, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            RemoteClientSYSTEMWATCH remoteDRAWCAPTURE = new RemoteClientSYSTEMWATCH(
                ClientDomainName,
                ClientUserName,
                ClientUserPassword,
                ClsLogonDummy,
                StageServerHost,
                "WatchService");

            if (delegateWriteLine == null)
                delegateWriteLine = Console.WriteLine;

            //delegateWriteLine($"\r\nSccConfig.Config.StageServerHost = {SccConfig.Config.StageServerHost}\r\n");

            switch (typestr)
            {
                case "string":
                    Result = remoteDRAWCAPTURE.GetSYSTEMWATCHserviceMmapvalue(StageServerHost, Label, "string");
                    if (Result != null)
                    {
                        delegateWriteLine($"GetSYSTEMWATCHserviceMmapvalue(...)\n" +
                            $"MMapラベル：{Label} 型\"string\" 取得しました：{Result}");
                    }
                    break;

                case "bool":
                    Result = remoteDRAWCAPTURE.GetSYSTEMWATCHserviceMmapvalue(StageServerHost, Label, "bool");
                    if (Result != null)
                    {
                        delegateWriteLine($"GetSYSTEMWATCHserviceMmapvalue(...)\n" +
                            $"MMapラベル：{Label} 型\"bool\" 取得しました：{Result}");
                    }
                    break;

                case "int":
                    Result = remoteDRAWCAPTURE.GetSYSTEMWATCHserviceMmapvalue(StageServerHost, Label, "int");
                    if (Result != null)
                    {
                        delegateWriteLine($"GetSYSTEMWATCHserviceMmapvalue(...)\n" +
                        $"MMapラベル：{Label} 型\"int\" 取得しました：{Result}");
                    }
                    break;

                default:
                    Result = null;
                    break;
            }
            if (Result == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 指定した名前のメモリマップドファイルへ書き出す
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="typestr"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool writeMMPFValue(string Label, string typestr, object output)
        {
            RemoteClientSYSTEMWATCH remoteDRAWCAPTURE = new RemoteClientSYSTEMWATCH(ClientDomainName,
                ClientUserName,
                ClientUserPassword,
                ClsLogonDummy,
                StageServerHost,
                "WatchService");

            bool sucess;

            switch (typestr)
            {
                case "string":
                    sucess = remoteDRAWCAPTURE.SetDRAWWATCHserviceMmapvalue(StageServerHost, Label, "string", output);
                    if (sucess != false)
                    {
                    }
                    break;

                case "bool":
                    sucess = remoteDRAWCAPTURE.SetDRAWWATCHserviceMmapvalue(StageServerHost, Label, "bool", output);
                    if (sucess != false)
                    {
                    }
                    break;

                case "int":
                    sucess = remoteDRAWCAPTURE.SetDRAWWATCHserviceMmapvalue(StageServerHost, Label, "int", output);
                    if (sucess != false)
                    {
                    }
                    break;

                default:
                    sucess = false;
                    break;
            }
            return sucess;
        }

    }
}
