using SasaLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ToyoStageService
{
    /// <summary>
    /// プロセス間共有変数の準備
    ///  他のプロセスから、 SharedConfigValue.mmapedFile.COMMITACCEPTFALSEMSG.ReadStr() のように使用する
    /// </summary>
    public class SharedConfigValue
    {
        /// <summary>
        /// プロセス間メモリマップドファイル用通信用のオブジェクト
        /// </summary>
        public static SharedConfigValue mmapedFile { get; set; }

        /// <summary>
        /// ①サーバーの稼働モード（Master 主側、Slave レプリケーション側）
        /// </summary>
        public MemoryMappedFileControl SERVERMODE { get; set; }

        /// <summary>
        /// ②ArcSuite登録サイクル RMapprovedProcess.ArcSuiteRegistFromDB(..) を実行するか否かの設定
        /// </summary>
        public MemoryMappedFileControl ArcSuiteRegistrationCycle { get; set; }

        /// <summary>
        /// ③コミット受付可能の場合true falseの場合コミットフォルダは受け付けません（削除）
        /// </summary>
        public MemoryMappedFileControl COMMITACCEPT { get; set; }

        /// <summary>
        /// ④コミット受付不可の場合、クライアントに返すメッセージ
        /// </summary>
        public MemoryMappedFileControl COMMITACCEPTFALSEMSG { get; set; }

        /// <summary>
        /// ⑤承認受付可能の場合true falseの場合承認は受け付けません）
        /// </summary>
        public MemoryMappedFileControl APPROVINGACCEPT { get; set; }

        /// <summary>
        /// ⑥承認受付不可の場合、クライアントに返すメッセージ
        /// </summary>
        public MemoryMappedFileControl APPROVINGACCEPTFALSEMSG { get; set; }

        /// <summary>
        /// ⑦コミット完了後すぐに印刷するか否かを切替 true=即印刷
        /// </summary>
        public MemoryMappedFileControl ImmediateryPrinting { get; set; }

        /// <summary>
        /// ⑧現在のサーバーの設定モード falseなら通常 、trueならテストモード
        /// </summary>
        public MemoryMappedFileControl TESTMODE { get; set; }

        /// <summary>
        /// ⑨RemoteServerCommand_DATABASE でイベントビューアにログを書き込むか
        /// </summary>
        public MemoryMappedFileControl RemoteServerCommand_DATABASE_EventView_Send { get; set; }

        /// <summary>
        /// ⑩REPLICATIONTOSUBHOST
        /// </summary>
        public MemoryMappedFileControl REPLICATIONTOSUBHOST { get; set; }

        /// <summary>
        /// コンストラクタ
        /// 他のプロセスから比較的スタートに近い段階で呼ばれる
        /// </summary>
        public SharedConfigValue()
        {
            // ① StageServerConfig.Config.SERVERMODE
            SERVERMODE = new MemoryMappedFileControl("SERVERMODE", (int)StageServerConfig.Config.SERVERMODE);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"①int ServerMode = {(int)StageServerConfig.Config.SERVERMODE}");
            // ② StageServerConfig.Config.ArcSuiteRegistrationCycle
            ArcSuiteRegistrationCycle = new MemoryMappedFileControl("ArcSuiteRegistrationCycle", StageServerConfig.Config.ArcSuiteRegistrationCycle);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"②int ArcSuiteRegistrationCycle = {StageServerConfig.Config.ArcSuiteRegistrationCycle}");
            // ③ StageServerConfig.Config.COMMITACCEPT
            COMMITACCEPT = new MemoryMappedFileControl("COMMITACCEPT", StageServerConfig.Config.COMMITACCEPT);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"③int COMMITACCEPT = {StageServerConfig.Config.COMMITACCEPT}");
            // ④ StageServerConfig.Config.COMMITACCEPTFALSEMSG
            COMMITACCEPTFALSEMSG = new MemoryMappedFileControl("COMMITACCEPTFALSEMSG", StageServerConfig.Config.COMMITACCEPTFALSEMSG);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"④stirng  COMMITACCEPTFALSEMSG = {StageServerConfig.Config.COMMITACCEPTFALSEMSG}");
            // ⑤ StageServerConfig.Config.APPROVINGACCEPT
            APPROVINGACCEPT = new MemoryMappedFileControl("APPROVINGACCEPT", StageServerConfig.Config.APPROVINGACCEPT);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"⑤int APPROVINGACCEPT = {StageServerConfig.Config.APPROVINGACCEPT}");
            // ⑥ StageServerConfig.Config.APPROVINGACCEPTFALSEMSG
            APPROVINGACCEPTFALSEMSG = new MemoryMappedFileControl("APPROVINGACCEPTFALSEMSG", StageServerConfig.Config.APPROVINGACCEPTFALSEMSG);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"⑥stirng  APPROVINGACCEPTFALSEMSG = {StageServerConfig.Config.APPROVINGACCEPTFALSEMSG}");
            // ⑦ StageServerConfig.Config.ImmediateryPrinting
            ImmediateryPrinting = new MemoryMappedFileControl("ImmediateryPrinting", StageServerConfig.Config.ImmediateryPrinting);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"⑦int  ImmediateryPrinting = {StageServerConfig.Config.ImmediateryPrinting}");
            // ⑧ StageServerConfig.Config.TESTMODE
            TESTMODE = new MemoryMappedFileControl("TESTMODE", StageServerConfig.Config.TESTMODE);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"⑧int  TESTMODE = {StageServerConfig.Config.TESTMODE}");
            // ⑨ StageServerConfig.Config.RemoteServerCommand_DATABASE_EventView_Send
            RemoteServerCommand_DATABASE_EventView_Send = new MemoryMappedFileControl("RemoteServerCommand_DATABASE_EventView_Send", StageServerConfig.Config.RecordOfNormaEvents_DataBaseSearch);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"⑨int  RemoteServerCommand_DATABASE_EventView_Send = {StageServerConfig.Config.RecordOfNormaEvents_DataBaseSearch}");
            // ⑩ StageServerConfig.Config.REPLICATIONTOSUBHOST
            REPLICATIONTOSUBHOST = new MemoryMappedFileControl("REPLICATIONTOSUBHOST", StageServerConfig.Config.REPLICATIONTOSUBHOST);
            SharedClassLibrary.DebugClass.ConsoleDebugOut(0, $"⑩int  REPLICATIONTOSUBHOST = {StageServerConfig.Config.REPLICATIONTOSUBHOST}");
        }



        /// <summary>
        /// メモリマップドファイルから一部の設定を読み出し,StageServerConfig.Config に反映
        /// </summary>
        public void ReadToStaticValues(object reference)
        {
            string objectName = reference.ToString();

            /// ① StageServerConfig.Config.SERVERMOD
            if (IsChanged(StageServerConfig.Config.SERVERMODE, (StageServerConfig.ServerMode)SERVERMODE.ReadInt()))
            {
                var msg =
                    GetSendingString(
                        callerMethodname: objectName,
                        publicValueName: "StageServerConfig.Config.SERVERMODE",
                        publicVaule: StageServerConfig.Config.SERVERMODE, mmapValue: (StageServerConfig.ServerMode)SERVERMODE.ReadInt());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", $"MMF変更を検知 [{objectName}]", msg);

                StageServerConfig.Config.SERVERMODE = (StageServerConfig.ServerMode)SERVERMODE.ReadInt();
            }

            /// ② StageServerConfig.Config.ArcSuiteRegistrationCycle
            if (IsChanged(StageServerConfig.Config.ArcSuiteRegistrationCycle, ArcSuiteRegistrationCycle.ReadBool()))
            {
                var msg =
                        GetSendingString(
                        callerMethodname: objectName,
                        publicValueName: "StageServerConfig.Config.ArcSuiteRegistrationCycle",
                        publicVaule: StageServerConfig.Config.ArcSuiteRegistrationCycle, mmapValue: ArcSuiteRegistrationCycle.ReadBool());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", "MMF変更を検知", msg);

                StageServerConfig.Config.ArcSuiteRegistrationCycle = ArcSuiteRegistrationCycle.ReadBool();
            }

            /// ③ StageServerConfig.Config.COMMITACCEPT
            if (IsChanged(StageServerConfig.Config.COMMITACCEPT, COMMITACCEPT.ReadBool()))
            {
                var msg =
                    GetSendingString(
                    callerMethodname: objectName,
                    publicValueName: "StageServerConfig.Config.COMMITACCEPT",
                    publicVaule: StageServerConfig.Config.COMMITACCEPT, mmapValue: COMMITACCEPT.ReadBool());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", "MMF変更を検知", msg);

                StageServerConfig.Config.COMMITACCEPT = COMMITACCEPT.ReadBool();
            }

            /// ④ StageServerConfig.Config.COMMITACCEPTFALSEMSG
            if (IsChanged(StageServerConfig.Config.COMMITACCEPTFALSEMSG, COMMITACCEPTFALSEMSG.ReadStr()))
            {
                var msg =
                    GetSendingString(
                        callerMethodname: objectName,
                        publicValueName: "StageServerConfig.Config.COMMITACCEPTFALSEMSG",
                        publicVaule: StageServerConfig.Config.COMMITACCEPTFALSEMSG, mmapValue: COMMITACCEPTFALSEMSG.ReadStr());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", "MMF変更を検知", msg);

                StageServerConfig.Config.COMMITACCEPTFALSEMSG = COMMITACCEPTFALSEMSG.ReadStr();
            }

            /// ⑤ StageServerConfig.Config.APPROVINGACCEPT
            if (IsChanged(StageServerConfig.Config.APPROVINGACCEPT, APPROVINGACCEPT.ReadBool()))
            {
                var msg =
                    GetSendingString(
                        callerMethodname: objectName,
                        publicValueName: "StageServerConfig.Config.APPROVINGACCEPT",
                        publicVaule: StageServerConfig.Config.APPROVINGACCEPT, mmapValue: APPROVINGACCEPT.ReadBool());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", "MMF変更を検知", msg);

                StageServerConfig.Config.APPROVINGACCEPT = APPROVINGACCEPT.ReadBool();
            }

            /// ⑥ StageServerConfig.Config.APPROVINGACCEPTFALSEMSG
            if (IsChanged(StageServerConfig.Config.APPROVINGACCEPTFALSEMSG, APPROVINGACCEPTFALSEMSG.ReadStr()))
            {
                var msg =
                    GetSendingString(
                        callerMethodname: objectName,
                        publicValueName: "StageServerConfig.Config.APPROVINGACCEPTFALSEMSG",
                        publicVaule: StageServerConfig.Config.APPROVINGACCEPTFALSEMSG, mmapValue: APPROVINGACCEPTFALSEMSG.ReadStr());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", "MMF変更を検知", msg);

                StageServerConfig.Config.APPROVINGACCEPTFALSEMSG = APPROVINGACCEPTFALSEMSG.ReadStr();
            }

            /// ⑦ StageServerConfig.Config.ImmediateryPrinting
            if (IsChanged(StageServerConfig.Config.ImmediateryPrinting, ImmediateryPrinting.ReadBool()))
            {
                var msg =
                        GetSendingString(
                        callerMethodname: objectName,
                        publicValueName: "StageServerConfig.Config.ImmediateryPrinting",
                        publicVaule: StageServerConfig.Config.ImmediateryPrinting, mmapValue: ImmediateryPrinting.ReadBool());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", "MMF変更を検知", msg);

                StageServerConfig.Config.ImmediateryPrinting = ImmediateryPrinting.ReadBool();
            }

            /// ⑧ StageServerConfig.Config.TESTMODE
            if (IsChanged(StageServerConfig.Config.TESTMODE, TESTMODE.ReadBool()))
            {
                var msg =
                        GetSendingString(
                        callerMethodname: objectName,
                        publicValueName: "StageServerConfig.Config.TESTMODE",
                        publicVaule: StageServerConfig.Config.TESTMODE, mmapValue: TESTMODE.ReadBool());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", "MMF変更を検知", msg);

                StageServerConfig.Config.TESTMODE = TESTMODE.ReadBool();
            }

            /// ⑨ RemoteServerCommand_DATABASE_EventView_Send
            if (IsChanged(StageServerConfig.Config.RecordOfNormaEvents_DataBaseSearch, RemoteServerCommand_DATABASE_EventView_Send.ReadBool()))
            {
                var msg =
                    GetSendingString(
                        callerMethodname: objectName,
                        publicValueName: "StageServerConfig.Config.RemoteServerCommand_DATABASE_EventView_Send",
                        publicVaule: StageServerConfig.Config.RecordOfNormaEvents_DataBaseSearch, mmapValue: RemoteServerCommand_DATABASE_EventView_Send.ReadBool());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", "MMF変更を検知", msg);

                StageServerConfig.Config.RecordOfNormaEvents_DataBaseSearch = RemoteServerCommand_DATABASE_EventView_Send.ReadBool();
            }

            /// ⑩ REPLICATIONTOSUBHOST
            if (IsChanged(StageServerConfig.Config.REPLICATIONTOSUBHOST, REPLICATIONTOSUBHOST.ReadBool()))
            {
                var msg =
                    GetSendingString(
                        callerMethodname: objectName,
                        publicValueName: "StageServerConfig.Config.REPLICATIONTOSUBHOST",
                        publicVaule: StageServerConfig.Config.REPLICATIONTOSUBHOST, mmapValue: REPLICATIONTOSUBHOST.ReadBool());

                SharedClassLibrary.DebugClass.ConsoleDebugOut(9, msg);

                ServerLog.Logging.LogRotateWriteLine($"StageServerConfig MMF変更を検知 [{objectName}] {msg}");
                //StageServerConfigWork.SendEmailFromStageServerConfig($"StageServerConfig", "MMF変更を検知", msg);


                StageServerConfig.Config.REPLICATIONTOSUBHOST = REPLICATIONTOSUBHOST.ReadBool();
            }
        }

        /// <summary>
        /// 送出メッセージを組立てる
        /// </summary>
        /// <param name="callerMethodname"></param>
        /// <param name="publicValueName"></param>
        /// <param name="publicVaule"></param>
        /// <param name="mmapValue"></param>
        /// <returns></returns>
        string GetSendingString(string callerMethodname, string publicValueName, object publicVaule, object mmapValue)
        {
            string result = $"■StageServerConfig.Configｵﾌﾞｼﾞｪｸﾄ  [{callerMethodname}] がﾌｨｰﾙﾄﾞ変数と対応するﾒﾓﾘﾏｯﾌﾟﾄﾞﾌｧｲﾙとの間に差異を検出しました。\n" +
                            $"ﾌｨｰﾙﾄﾞ変数名:\"{publicValueName}\" 変数側の値:\"{publicVaule}\" MMF側の値:\"{mmapValue}\"";
            return result;
        }

        /// <summary>
        /// StageServerConfig.Config.～の変数とメモリマップドファイルの情報と差異があればtrueを返す
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        private bool IsChanged(object source, object dist)
        {
            if (source.Equals(dist))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
