using SasaLib;
using SasaLib.PIPE;
using SasaLib.PrintConfig;
using SasaLibDummy;
using StageServerRemote;
using STAGINGSYSTEM_COMMANDS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using ToyoMcMfg.Staging.DataBaseConfig;
using ToyoStageService;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ServerControlCenterApplication
{
    /// <summary>
    /// デリゲートの宣言
    /// </summary>
    /// <param name="logtext"></param>

    public static class Command_ServerControl
    {
        public static int ReadStreamStringTimeOut { get; set; } = 20000;

        public static int ReadHandShakeStreamStringTimeOut { get; set; } = 10000;

        public static async void Backup(string BackupDistFolder, int level, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            if (string.IsNullOrWhiteSpace(BackupDistFolder))
            {
                WriteLine("バックアップ先未定義");
                return;
            }

            Console.WriteLine($"データベースバックアップ準備");
            RemoteClientDRAWCAPTURE remoteClientDrawCapture = new RemoteClientDRAWCAPTURE(
                SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDC);

            WriteLine($"データベースバックアップ開始.保存先フォルダ：{BackupDistFolder}");
            List<string> anser = await remoteClientDrawCapture.DatabaseBackup(BackupDistFolder);
            foreach (string ans in anser)
            {
                WriteLine($"RemoteClientDrawCapture.DatabaseBackup(..) 戻り値：{ans}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BackupSourceFolder"></param>
        /// <param name="createDate"></param>
        /// <param name="WriteLine"></param>
        public static async void Restore(string BackupSourceFolder, DateTime createDate, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            if (string.IsNullOrWhiteSpace(BackupSourceFolder))
            {
                WriteLine("バックアップソースフォルダ未定義");
                return;
            }

            Console.WriteLine($"データベースリストア準備");
            RemoteClientDRAWCAPTURE remoteClientDrawCapture = new RemoteClientDRAWCAPTURE(
                SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDC);

            WriteLine($"データベースリストア開始.ソースフォルダ：{BackupSourceFolder} 日付{createDate.ToString("yyyy-MM-dd_HHmmss")}");
            List<string> anser = await remoteClientDrawCapture.DatabaseRestore(BackupSourceFolder, createDate);
            foreach (string ans in anser)
            {
                WriteLine($"remoteClientDrawCapture.DatabaseRestore(..) 戻り値：{ans}");
            }
        }

        /// <summary>
        ///  ■SetDRAWREGISTserviceLogLevel(level)
        /// </summary>
        /// <param name="level"></param>
        public static int SetOrGet_DRAWREGISTserviceDEBUGLevel(int level, bool set = true, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientDRAWREGIST remoteDRAWREGIST = new RemoteClientDRAWREGIST(
                SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            remoteDRAWREGIST.ClientTimeOut = 8000;
            remoteDRAWREGIST.ReadStreamStringTimeOut = 8000;

            string DRAWREGISTserviceLogLevelcurrent = remoteDRAWREGIST.GetDRAWREGISTserviceLogLevel();
            WriteLine($"サーバー:{SccConfig.Config.StageServerHost} の DRAWREGISTserviceの変更前のログレベルは {DRAWREGISTserviceLogLevelcurrent} です");

            if (set == false)
            {
                int.TryParse(DRAWREGISTserviceLogLevelcurrent, out int result);
                return result;
            }


            remoteDRAWREGIST.SetDRAWREGISTserviceLogLevel(level);

            string DRAWREGISTserviceLogLevelchanged = remoteDRAWREGIST.GetDRAWREGISTserviceLogLevel();
            WriteLine($"サーバー:{SccConfig.Config.StageServerHost} の DRAWREGISTserviceの変更後のログレベルは {DRAWREGISTserviceLogLevelchanged} です");
            return int.Parse(DRAWREGISTserviceLogLevelchanged);

        }

        /// <summary>
        /// ■SetDRAWCAPTUREserviceLogLevel(level)
        /// </summary>
        /// <param name="level"></param>
        public static int SetOrGet_DRAWCAPTUREserviceDEBUGLevel(int level, bool set = true, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientDRAWCAPTURE remoteDRAWCAPTURE = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDC);

            remoteDRAWCAPTURE.ClientTimeOut = 1000;
            remoteDRAWCAPTURE.ReadStreamStringTimeOut = 5000;

            string DRAWRCAPTUREserviceLogLevelcurrent = remoteDRAWCAPTURE.GetDRAWCAPTUREserviceLogLevel();
            WriteLine($"サーバー:{SccConfig.Config.StageServerHost} の DRAWCAPTUREserviceの変更前のログレベルは {DRAWRCAPTUREserviceLogLevelcurrent} です");

            remoteDRAWCAPTURE.SetDRAWCAPTUREserviceLogLevel(level);

            if (set == false)
            {
                int.TryParse(DRAWRCAPTUREserviceLogLevelcurrent, out int result);
                return result;
            }


            string DRAWRCAPTUREserviceLogLevelchanged = remoteDRAWCAPTURE.GetDRAWCAPTUREserviceLogLevel();
            WriteLine($"サーバー:{SccConfig.Config.StageServerHost} のDRAWCAPTUREserviceの変更後のログレベルは {DRAWRCAPTUREserviceLogLevelchanged} です");
            return int.Parse(DRAWRCAPTUREserviceLogLevelchanged);
        }

        /// <summary>
        /// ■
        /// </summary>
        /// <param name="level"></param>
        /// <param name="set"></param>
        /// <param name="WriteLine"></param>
        /// <returns></returns>
        public static int SetOrGet_STAGINGSYSTEMwatchDEBUGLevel(int level, bool set = true, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientSYSTEMWATCH remoteSYSTEMWATCH = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameSW);

            remoteSYSTEMWATCH.ClientTimeOut = 1000;
            remoteSYSTEMWATCH.ReadStreamStringTimeOut = 5000;

            string STAGINGSYSTMwatchLogLevelcurrent = remoteSYSTEMWATCH.GetSTAGINGSYSTEMwatchLogLevel();
            WriteLine($"サーバー:{SccConfig.Config.StageServerHost} の STAGINGSYSTEMwatchの変更前のログレベルは {STAGINGSYSTMwatchLogLevelcurrent} です");

            remoteSYSTEMWATCH.SetSTAGINGSYSTEMwatchLogLevel(level);

            if (set == false)
            {
                int.TryParse(STAGINGSYSTMwatchLogLevelcurrent, out int result);
                return result;
            }

            string STAGINGSYSTMwatchLogLevelchanged = remoteSYSTEMWATCH.GetSTAGINGSYSTEMwatchLogLevel();
            WriteLine($"サーバー:{SccConfig.Config.StageServerHost} のSTAGINGSYSTEMwatchの変更後のログレベルは {STAGINGSYSTMwatchLogLevelcurrent} です");
            return int.Parse(STAGINGSYSTMwatchLogLevelchanged);
        }

        /// <summary>
        /// ■バージョン情報確認（DRAWREGISTservice）
        /// </summary>
        /// <param name="WriteLine"></param>
        public static string GetDRAWREGISTserviceVersion(SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientDRAWREGIST remoteDRAWREGIST = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);
            string version;

            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                WriteLine(msg);
            }

            try
            {
                version = remoteDRAWREGIST.GetDRAWREGISTserviceVersion(localLogWrite);
                WriteLine($"サーバー:{SccConfig.Config.StageServerHost} の DRAWREGISTserviceのバージョンは {version} です");
            }
            catch (Exception ex)
            {
                version = null;
                WriteLine(ex.Message);
            }
            return version;
        }

        /// <summary>
        /// ■バージョン情報確認（DRAWCAPTUREservice）
        /// </summary>
        /// <param name="WriteLine"></param>
        public static string GetDRAWCAPTUREserviceVersion(SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientDRAWCAPTURE remoteDRAWCAPTURE = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDC);
            string version;
            try
            {
                version = remoteDRAWCAPTURE.GetDRAWCAPTUREserviceVersion(SccConfig.Config.StageServerHost);
                WriteLine($"サーバー:{SccConfig.Config.StageServerHost} の DRAWCAPTUREserviceのバージョンは {version} です");
            }
            catch (Exception ex)
            {
                version = null;
                WriteLine(ex.Message);
            }
            return version;
        }

        /// <summary>
        /// ■バージョン情報確認（DRAWWATCHservice）
        /// </summary>
        /// <param name="WriteLine"></param>
        public static string GetSYSTEMWATCHserviceVersion(SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientSYSTEMWATCH remoteDRAWWATCH = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameSW);
            string version;
            try
            {
                version = remoteDRAWWATCH.GetSYSTEMWATCHserviceVersion(SccConfig.Config.StageServerHost);
                WriteLine($"サーバー:{SccConfig.Config.StageServerHost} の DRAWWATCHserviceのバージョンは {version} です");
            }
            catch (Exception ex)
            {
                version = null;
                WriteLine(ex.Message);
            }
            return version;
        }

        /// <summary>
        /// ■メモリマップドファイル読出し
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="typestr"></param>
        /// <param name="WriteLine"></param>
        public static void GetSYSTEMWATCHserviceMmap(string Label, string typestr, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientSYSTEMWATCH remoteDRAWCAPTURE = new RemoteClientSYSTEMWATCH(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                "WatchService");

            switch (typestr)
            {
                case "string":
                    string MSG = (string)remoteDRAWCAPTURE.GetSYSTEMWATCHserviceMmapvalue(SccConfig.Config.StageServerHost, Label, "string");
                    WriteLine($"GetSYSTEMWATCHserviceMmapvalue(...)\n" +
                        $"MMapラベル：{Label} 型\"string\" 取得しました：{MSG}");
                    break;

                case "bool":
                    bool result = (bool)remoteDRAWCAPTURE.GetSYSTEMWATCHserviceMmapvalue(SccConfig.Config.StageServerHost, Label, "bool");
                    WriteLine($"GetSYSTEMWATCHserviceMmapvalue(...)\n" +
                        $"MMapラベル：{Label} 型\"bool\" 取得しました：{result}");
                    break;

                case "int":
                    int resultint = (int)remoteDRAWCAPTURE.GetSYSTEMWATCHserviceMmapvalue(SccConfig.Config.StageServerHost, Label, "int");
                    WriteLine($"GetSYSTEMWATCHserviceMmapvalue(...)\n" +
                        $"MMapラベル：{Label} 型\"intl\" 取得しました：{resultint}");
                    break;

                default:

                    break;
            }
        }


        /// <summary>
        /// ■PIPEコマンド [GetPipeCommandLog] サーバーが保持している クライアントの接続記録を文字列で得る
        /// </summary>
        /// <param name="ClientDomainName"></param>
        /// <param name="ClientUserName"></param>
        /// <param name="ClientUserPassword"></param>
        /// <param name="ClsLogon"></param>
        /// <param name="StageServerHost"></param>
        /// <param name="pipeName"></param>
        /// <param name="WriteLine"></param>
        /// <returns></returns>
        public static string GetPipeCommandLog(string ClientDomainName, string ClientUserName, string ClientUserPassword, bool ClsLogon,
            string StageServerHost, string pipeName,
            SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientDRAWREGIST remoteDRAWREGIST = new RemoteClientDRAWREGIST(ClientDomainName,
                ClientUserName,
                ClientUserPassword,
                ClsLogon,
                StageServerHost,
                pipeName);
            string version;

            try
            {
                if (WriteLine == null) WriteLine = Console.WriteLine;
                int ClientTimeOut = 2000;
                //bool PipeConnectionStatus = false;
                int ReadStreamStringTimeOut = 5000;
                using (new ClsLogonDummy(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon))
                {
                    try
                    {
                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(SccConfig.Config.StageServerHost, pipeName);
                        // 待機中のサーバーへ接続
                        try
                        {
                            pipeCltStream.Connect(ClientTimeOut);
                        }
                        catch (Exception ex)
                        {
                            //PipeConnectionStatus = false;

                            WriteLine($"PIPEサーバー接続エラー {ex.Message}");
                            return "Error";
                        }
                        // サーバーからのサーバ識別文字列を受け取ります。
                        StreamString stst = new StreamString(pipeCltStream);
                        bool OperationCanceledException;
                        bool AggregateException;

                        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                        if (OperationCanceledException || AggregateException)
                        {
                            WriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                            return null;
                        }
                        if (CheckFirstMessage(input0))
                        {
                            int writeResult = stst.WriteString(CMDS.DC_DR_SW_GetPipeCommandLog);

                            if (writeResult == -1)
                                throw new Exception("PIPEコマンドを送信できませんでした");

                            string ServerVersion = stst.ReadString(ReadStreamStringTimeOut, null);

                            pipeCltStream.Close();

                            return ServerVersion;
                        }
                        else
                        {
                            WriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return "エラー";
                        }
                        // Give the client process some time to display results before exiting.
                    }
                    catch (Exception ex)
                    {
                        //PipeConnectionStatus = false;
                        WriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    }
                    return "エラー";
                }
            }
            catch (Exception ex)
            {
                version = null;
                WriteLine(ex.Message);
            }
            return version;
        }

        internal static List<ClientPreInputTICKET> AddClientPreInputTICKETCODE(string pipeName, ClientPreInputTICKET ticket, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientDRAWREGIST remoteDRAWREGIST = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                pipeName);
            List<ClientPreInputTICKET> preInputTickets;


            try
            {
                if (WriteLine == null) WriteLine = Console.WriteLine;
                int ClientTimeOut = 2000;
                //bool PipeConnectionStatus = false;
                //int ReadStreamStringTimeOut = 5000;
                using (new ClsLogonDummy(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon))
                {
                    try
                    {
                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(SccConfig.Config.StageServerHost, pipeName);
                        // 待機中のサーバーへ接続
                        try
                        {
                            pipeCltStream.Connect(ClientTimeOut);
                        }
                        catch (Exception ex)
                        {
                            //PipeConnectionStatus = false;

                            WriteLine($"PIPEサーバー接続エラー {ex.Message}");
                            return null;
                        }
                        // サーバーからのサーバ識別文字列を受け取ります。
                        StreamString stst = new StreamString(pipeCltStream);
                        bool OperationCanceledException;
                        bool AggregateException;

                        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                        if (OperationCanceledException || AggregateException)
                        {
                            WriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                            return null;
                        }
                        if (CheckFirstMessage(input0))
                        {
                            int writeResult = stst.WriteString(CMDS.DR_AddClientPreInputTICKETCODE);

                            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                            {
                                writer.WriteObject(ticket);
                            }

                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                preInputTickets = reader.ReadObject<List<ClientPreInputTICKET>>(); // ｻｰﾊﾞｰからオブジェクト受信
                            }

                            pipeCltStream.Close();

                            return preInputTickets;
                        }
                        else
                        {
                            WriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return null;
                        }
                        // Give the client process some time to display results before exiting.
                    }
                    catch (Exception ex)
                    {
                        //PipeConnectionStatus = false;
                        WriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                return null;
            }
        }

        internal static List<string> RemovePreInputTIKECTCODEs(string pipeName, List<string> ticketcodes, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientDRAWREGIST remoteDRAWREGIST = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                pipeName);
            List<string> resultRemoveSucessTickets;


            try
            {
                if (WriteLine == null) WriteLine = Console.WriteLine;
                int ClientTimeOut = 2000;
                //bool PipeConnectionStatus = false;
                //int ReadStreamStringTimeOut = 5000;
                using (new ClsLogonDummy(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon))
                {
                    try
                    {
                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(SccConfig.Config.StageServerHost, pipeName);
                        // 待機中のサーバーへ接続
                        try
                        {
                            pipeCltStream.Connect(ClientTimeOut);
                        }
                        catch (Exception ex)
                        {
                            //PipeConnectionStatus = false;

                            WriteLine($"PIPEサーバー接続エラー {ex.Message}");
                            return null;
                        }
                        // サーバーからのサーバ識別文字列を受け取ります。
                        StreamString stst = new StreamString(pipeCltStream);
                        bool OperationCanceledException;
                        bool AggregateException;

                        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                        if (OperationCanceledException || AggregateException)
                        {
                            WriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                            return null;
                        }
                        if (CheckFirstMessage(input0))
                        {
                            int writeResult = stst.WriteString(CMDS.DR_RemovePreInputTIKECTCODEs);

                            using (var writer = new BinaryWriter(pipeCltStream, Encoding.UTF8, true))
                            {
                                writer.WriteObject(ticketcodes);
                            }

                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                resultRemoveSucessTickets = reader.ReadObject<List<string>>(); // ｻｰﾊﾞｰからオブジェクト受信
                            }

                            pipeCltStream.Close();

                            return resultRemoveSucessTickets;
                        }
                        else
                        {
                            WriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return null;
                        }
                        // Give the client process some time to display results before exiting.
                    }
                    catch (Exception ex)
                    {
                        //PipeConnectionStatus = false;
                        WriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                return null;
            }
        }

        internal static List<ClientPreInputTICKET> GetCommonApprovalWaitingTicketList(string pipeName, List<string> ticketcodes, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientDRAWREGIST remoteDRAWREGIST = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                pipeName);
            List<ClientPreInputTICKET> CommonApprovalWaitingTicketList;


            try
            {
                if (WriteLine == null) WriteLine = Console.WriteLine;
                int ClientTimeOut = 2000;
                //bool PipeConnectionStatus = false;
                //int ReadStreamStringTimeOut = 5000;
                using (new ClsLogonDummy(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon))
                {
                    try
                    {
                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(SccConfig.Config.StageServerHost, pipeName);
                        // 待機中のサーバーへ接続
                        try
                        {
                            pipeCltStream.Connect(ClientTimeOut);
                        }
                        catch (Exception ex)
                        {
                            //PipeConnectionStatus = false;

                            WriteLine($"PIPEサーバー接続エラー {ex.Message}");
                            return null;
                        }
                        // サーバーからのサーバ識別文字列を受け取ります。
                        StreamString stst = new StreamString(pipeCltStream);
                        bool OperationCanceledException;
                        bool AggregateException;

                        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                        if (OperationCanceledException || AggregateException)
                        {
                            WriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                            return null;
                        }
                        if (CheckFirstMessage(input0))
                        {
                            int writeResult = stst.WriteString(CMDS.DR_GetCommonApprovalWaitingTicketList);

                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                CommonApprovalWaitingTicketList = reader.ReadObject<List<ClientPreInputTICKET>>(); // ｻｰﾊﾞｰからオブジェクト受信
                            }

                            pipeCltStream.Close();

                            return CommonApprovalWaitingTicketList;
                        }
                        else
                        {
                            WriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return null;
                        }
                        // Give the client process some time to display results before exiting.
                    }
                    catch (Exception ex)
                    {
                        //PipeConnectionStatus = false;
                        WriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// ■ 現在の接続しているクライアントを表す構造体を得る
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="WriteLine"></param>
        /// <returns></returns>
        internal static List<AcceptPipeCommand> GetAuthorizedUser(string pipeName, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null)
                WriteLine = Console.WriteLine;

            RemoteClientDRAWREGIST remoteDRAWREGIST = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                pipeName);
            
            List<AcceptPipeCommand> connectClients;

            try
            {
                if (WriteLine == null) WriteLine = Console.WriteLine;
                int ClientTimeOut = 2000;
                //bool PipeConnectionStatus = false;
                //int ReadStreamStringTimeOut = 5000;
                using (new ClsLogonDummy(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon))
                {
                    try
                    {
                        NamedPipeClientStream pipeCltStream = new NamedPipeClientStream(SccConfig.Config.StageServerHost, pipeName);
                        // 待機中のサーバーへ接続
                        try
                        {
                            pipeCltStream.Connect(ClientTimeOut);
                        }
                        catch (Exception ex)
                        {
                            //PipeConnectionStatus = false;

                            WriteLine($"PIPEサーバー接続エラー {ex.Message}");
                            return null;
                        }
                        // サーバーからのサーバ識別文字列を受け取ります。
                        StreamString stst = new StreamString(pipeCltStream);
                        bool OperationCanceledException;
                        bool AggregateException;

                        string input0 = stst.ReadString(ReadHandShakeStreamStringTimeOut, out OperationCanceledException, out AggregateException, null);
                        if (OperationCanceledException || AggregateException)
                        {
                            WriteLine($"最初のハンドシェイクにてタイムアウトが発生");
                            return null;
                        }
                        if (CheckFirstMessage(input0))
                        {
                            int writeResult = stst.WriteString(CMDS.DR_GetAuthorizedUser);

                            using (BinaryReader reader = new BinaryReader(pipeCltStream, Encoding.UTF8, true))
                            {
                                connectClients = reader.ReadObject<List<AcceptPipeCommand>>(); // ｻｰﾊﾞｰからオブジェクト受信
                            }

                            pipeCltStream.Close();

                            return connectClients;
                        }
                        else
                        {
                            WriteLine($"サーバーからの接続文字列{input0}が期待と違います");
                            pipeCltStream.Close();
                            return null;
                        }
                        // Give the client process some time to display results before exiting.
                    }
                    catch (Exception ex)
                    {
                        //PipeConnectionStatus = false;
                        WriteLine($"PIPEサーバー接続エラー{ex.Message}");
                        WriteLine($"PIPEサーバー接続エラー{ex.Message}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input0"></param>
        /// <param name="WriteLine"></param>
        /// <returns></returns>
        internal static bool CheckFirstMessage(string input0, SasaLibDelegateWriteLine WriteLine = null)
        {
            if (WriteLine == null) WriteLine = DebugConsole.WriteLine;

            if (input0 == CMDS.ConnectKeyword)
                return true;
            else if (input0 == @"BUSY")
            {
                WriteLine("サーバーが混んでいます。しばらくお待ちください");
                return false;
            }
            else if (input0 == null)
            {
                WriteLine($"サーバーに接続できませんでした(null が返されました)");

                return false;
            }
            else
            {
                WriteLine($"このクライアントはサーバーが要求するものとは違います\n{input0}");

                return false;
            }
        }

    }
}
