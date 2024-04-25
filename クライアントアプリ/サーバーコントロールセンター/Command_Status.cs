using RemoteClient;
using SasaLib;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyoStageService;
using static ServerControlCenterApplication.Command_ServerControl;

namespace ServerControlCenterApplication
{
    public static class Command_Status
    {
        static readonly string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        public static int ClientTimeOut = 1000;

        /// <summary>
        /// ■DRサーバーのServer Mode問い合わせ
        /// Status CURRENT MODE
        /// </summary>
        /// <returns></returns>
        public static string DR_ServerMode(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            RemoteClientServerControl rmClientDR = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            string curSERVERMODE = rmClientDR.Check_Status_CurrentMode(delegateWriteLine);

            return curSERVERMODE;

        }

        /// <summary>
        /// ■DRサーバー ConnectTest
        /// </summary>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public static bool DR_ConnectTest(string testMessage, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                delegateWriteLine(msg);
            }

            RemoteClientDRAWREGIST rmClientDR = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            bool ans = rmClientDR.ConnectTest(testMessage, localLogWrite);

            return ans;

        }

        public static bool DC_ConnectTest(string testMessage, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            ///　？ローカルメソッド
            void localLogWrite(string msg)
            {
                delegateWriteLine(msg);
            }

            RemoteClientDRAWCAPTURE rmClientDC = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDC);

            bool ans = rmClientDC.ConnectTest(testMessage, localLogWrite);

            return ans;

        }

        /// <summary>
        /// ■DCサーバーの待機状態問い合わせ
        /// Status CURRENT MODE
        /// </summary>
        /// <param name="delegateWriteLine"></param>
        /// <returns></returns>
        public static string DC_ServerMode(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            RemoteClientServerControl rmClientDC = new RemoteClientServerControl(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            string curSERVERMODE = rmClientDC.Check_Status_CurrentMode(delegateWriteLine);

            return curSERVERMODE;

        }



        //public static string DR_GETFALESMSG(string ForcedDomainName, string ForcedUserName, string ForcedUserPassword, bool ForcedAccountFlag, string PipeServerName, string PipeName,SasaLibDelegateWriteLine delegateWriteLine = null)
        //{
        //    try
        //    {

        //        if (string.IsNullOrWhiteSpace(PipeServerName) == false)
        //        {
        //            PIPEClient pIPEClient = new PIPEClient(ForcedDomainName, ForcedUserName, ForcedUserPassword, ForcedAccountFlag, PipeServerName, PipeName);

        //            //StageServerConfig.ServerMode subHostmode = pIPEClient.Get_Status_Servemode();

        //        } // ■サブDBホストのサーバーモードを取得 Maser or Slave;


        //    }
        //    catch (Exception ex)
        //    {
        //        delegateWriteLine($"CheckDRAWREGISTServerMode(...) \nPipeServerName={SccConfig.Config.StageServerHost}\n {ex.Message}");
        //        Console.WriteLine($"CheckDRAWREGISTServerMode(...) \nPipeServerName={SccConfig.Config.StageServerHost}\n {ex.Message}");
        //    }
        //    return "エラー";
        //}
        //public static string DR_GETFALESMSG(SasaLibDelegateWriteLine delegateWriteLine = null)
        //{
        //    if (string.IsNullOrWhiteSpace(StageServerConfig.Config.SUB_DBHOST) == false)
        //    {
        //        PIPEClient pIPEClient = new PIPEClient("", "", "", false, StageServerConfig.Config.SUB_DBHOST, StageServerConfig.Config.PipeNameDC);

        //        StageServerConfig.ServerMode subHostmode = pIPEClient.Get_Status_Servemode();

        //    } // ■サブDBホストのサーバーモードを取得 Maser or Slave;


        //    DateTime dt1 = DateTime.Now;
        //    bool PipeConnectionStatus;
        //    using (new ClsLogon(
        //        SccConfig.Config.ClientDomainName,
        //        SccConfig.Config.ClientUserName,
        //        SccConfig.Config.ClientUserPassword,
        //        SccConfig.Config.ClsLogon
        //        ))
        //    {
        //        try
        //        {
        //            NamedPipeClientStream pipeCliStream = new NamedPipeClientStream(
        //                SccConfig.Config.StageServerHost,
        //                SccConfig.Config.PipeNameDR);


        //            // 待機中のサーバーへ接続
        //            try
        //            {
        //                pipeCliStream.Connect(ClientTimeOut);
        //                PipeConnectionStatus = pipeCliStream.IsConnected;
        //                delegateWriteLine($"CheckServerMode():パイプクライアントストリームに接続しました");

        //            }
        //            catch (Exception ex)
        //            {
        //                PipeConnectionStatus = false;
        //                delegateWriteLine($"CheckServerMode(...) \nPIPEサーバー接続エラー {SccConfig.Config.StageServerHost}\n {ex.Message}");
        //                Console.WriteLine($"CheckServerMode(...) \nPIPEサーバー接続エラー  PipeServerName={SccConfig.Config.StageServerHost} IsConnected={pipeCliStream.IsConnected} ClientTImeOut {ClientTimeOut}\n {ex.Message}");
        //                pipeCliStream.Close();
        //                return "Error";
        //            }

        //            // サーバーからのサーバ識別文字列を受け取ります。
        //            StreamString ss = new StreamString(pipeCliStream);
        //            string input0 = ss.ReadString();
        //            delegateWriteLine($"サーバーから識別文字列を受け取りました:{input0}");

        //            if (StageServerRemote.RMCsupport.CheckFirstMessage(input0))
        //            {
        //                delegateWriteLine($"CheckServerMode():{dt1}:サーバーからの接続文字列{input0}は期待値です");

        //                ss.WriteString("Status");
        //                ss.WriteString("CURRENT MODE");
        //                delegateWriteLine($"CheckServerMode():\"Status\" \"CURRENT MODE\" を送信しました");

        //                string AnserMessage = ss.ReadString();
        //                delegateWriteLine($"CheckServerMode():【{AnserMessage}】を受信しました");

        //                pipeCliStream.Close();
        //                delegateWriteLine($"CheckServerMode():パイプクライアントストリームを閉じました");

        //                return AnserMessage;
        //            }
        //            else
        //            {
        //                PipeConnectionStatus = false;
        //                Console.WriteLine($"■CheckServerMode():{dt1}:サーバーからの接続文字列{input0}が期待と違います");
        //                pipeCliStream.Close();
        //                return "エラー";
        //            }
        //            // Give the client process some time to display results before exiting.
        //        }
        //        catch (Exception ex)
        //        {
        //            PipeConnectionStatus = false;
        //            delegateWriteLine($"CheckDRAWREGISTServerMode(...) \nPipeServerName={SccConfig.Config.StageServerHost}\n {ex.Message}");
        //            Console.WriteLine($"CheckDRAWREGISTServerMode(...) \nPipeServerName={SccConfig.Config.StageServerHost}\n {ex.Message}");
        //        }
        //        return "エラー";
        //    }
        //    return null;
        //}

        public static string GetDRAWREGISTserviceLogLevel(SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            //    bool PipeConnectionStatus;

            //    using (new ClsLogon(
            //        SccConfig.Config.ClientDomainName,
            //        SccConfig.Config.ClientUserName,
            //        SccConfig.Config.ClientUserPassword,
            //        SccConfig.Config.ClsLogon
            //        ))
            //    {
            //        try
            //        {
            //            NamedPipeClientStream pipeCliStream = new NamedPipeClientStream(
            //                SccConfig.Config.StageServerHost,
            //                SccConfig.Config.PipeNameDR);
            //            try
            //            {
            //                pipeCliStream.Connect(ClientTimeOut);
            //                PipeConnectionStatus = pipeCliStream.IsConnected;
            //                delegateWriteLine($"CheckServerMode():パイプクライアントストリームに接続しました");

            //            }
            //            catch (Exception ex)
            //            {
            //                PipeConnectionStatus = false;
            //                delegateWriteLine($"CheckServerMode(...) \nPIPEサーバー接続エラー {SccConfig.Config.StageServerHost}\n {ex.Message}");
            //                Console.WriteLine($"CheckServerMode(...) \nPIPEサーバー接続エラー  PipeServerName={SccConfig.Config.StageServerHost} IsConnected={pipeCliStream.IsConnected} ClientTImeOut {ClientTimeOut}\n {ex.Message}");
            //                pipeCliStream.Close();
            //                return "Error";
            //            }
            //            // サーバーからのサーバ識別文字列を受け取ります。
            //            StreamString ss = new StreamString(pipeCliStream);
            //            string input0 = ss.ReadString();
            //            if (StageServerRemote.RMCsupport.CheckFirstMessage(input0))
            //            {
            //                // Console.WriteLine($"サーバーからの接続文字列{input0}は期待値です");
            //                ss.WriteString("ServerControl");
            //                ss.WriteString(CMDS.ServerControl_CONSOLE_LOGLEVEL_GET);

            //                string currentLogLevel = ss.ReadString();

            //                pipeCliStream.Close();

            //                return currentLogLevel;

            //            }
            //            else
            //            {
            //                Console.WriteLine($"サーバーからの接続文字列{input0}が期待と違います");
            //                pipeCliStream.Close();
            //                return $"DRAWREGISTserviveからログレベルの取得に失敗！！";
            //            }
            //            // Give the client process some time to display results before exiting.
            //        }
            //        catch (Exception ex)
            //        {
            //            PipeConnectionStatus = false;
            //            delegateWriteLine($"CheckDRAWREGISTServerMode(...) \nPipeServerName={SccConfig.Config.StageServerHost}\n {ex.Message}");
            //            Console.WriteLine($"CheckDRAWREGISTServerMode(...) \nPipeServerName={SccConfig.Config.StageServerHost}\n {ex.Message}");
            //        }
            //        return "エラー";
            //    }

            return null;
        }

    }
}
