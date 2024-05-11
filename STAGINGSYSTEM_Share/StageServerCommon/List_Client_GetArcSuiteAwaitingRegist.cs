using SasaLib.PIPE;
using SasaLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using SharedClassLibrary;
using System.Drawing;
using System.IO;
using System.Data;
using ToyoMcMfg.Staging.DataBaseConfig;
using System.Xml.XPath;
using System.Diagnostics.Eventing.Reader;
using STAGINGSYSTEM_COMMANDS;

namespace ToyoStageService
{
    /// <summary>
    /// 承認クライアントからの接続情報を扱うクラス
    /// </summary>
    public class List_Client_GetArcSuiteAwaitingRegist
    {
        /// <summary>
        /// 接続してきたｸﾗｲｱﾝﾄ を保持する
        /// </summary>
        List<AcceptPipeCommand> Client_GetArcSuiteAwaitingRegistList = new List<AcceptPipeCommand>();
        //readonly object Client_GetArcSuiteAwaitingRegistList_LockHandler = new object();

        /// <summary>
        /// 
        /// </summary>
        string eventLogSourceName;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="eventLogSourceName"></param>
        /// <param name="commandName"></param>
        /// <param name="purgeBeforeSecond"></param>
        /// <param name="eventLogWriteIntervalSecond"></param>
        public List_Client_GetArcSuiteAwaitingRegist(string eventLogSourceName)
        {
            this.eventLogSourceName = eventLogSourceName;
            try
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        //lock (Client_GetArcSuiteAwaitingRegistList_LockHandler)
                        //{

                        var removeCount = Client_GetArcSuiteAwaitingRegistList.RemoveAll(w => w.Command_Accept_DateTime.AddSeconds(StageServerConfig.Config.List_Client_GetArcSuiteAwaitingRegist_HoldSecond) < DateTime.Now);
                        if (removeCount > 0)
                        {
                            DebugClass.ConsoleDebugOut(0, $"{StageServerConfig.Config.List_Client_GetArcSuiteAwaitingRegist_HoldSecond} 秒経過した クライアントからの入力チケットリスト(List<ConnectedApprovalCient> CurrentConnectedClients)を {removeCount} 個消去しました", ConsoleColor.Magenta);
                        }
                        //}
                        await Task.Delay(1 * 1000);
                    } // １秒間隔で コマンドが指定時間以上経過した接続を削除する
                });

                Task.Run(async () =>
                {
                    while (true)
                    {
                        string outmsg = GetSendSummaryEventData();

                        SasaLib.Eventlog.Log.WriteEntry(eventLogSourceName, EventLogEntryType.Information, 9700, $"{outmsg}", OutConsole: true);

                        await Task.Delay(StageServerConfig.Config.PipeCommand_ConnectionEventLog_WriteIntervalSecond * 1000);
                    } // PipeCommand_ConnectionEventLog_WriteIntervalSecond で指定した秒数間隔でイベントログに現在の接続情報を記録
                });

            }
            catch (Exception ex)
            {
                DebugClass.ConsoleDebugOut(0, $"List_Client_GetArcSuiteAwaitingRegist() の で例外 {ex.Message} {ex.InnerException}", ConsoleColor.Red);
            }

        }

        /// <summary>
        /// イベントログにサマリーを出力
        /// </summary>
        /// <returns></returns>
        private string GetSendSummaryEventData()
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendLine($"{eventLogSourceName} AuthorizedUserConnectionStatus 受信記録 直近 {StageServerConfig.Config.List_Client_AcceptCommand_HoldSecond} 秒以内の接続");

            var Client_GetArcSuiteAwaitingRegist_ListCopy = new List<AcceptPipeCommand>(Client_GetArcSuiteAwaitingRegistList); //  コピーコレクションを準備
            foreach (var Client_GetArcSuiteAwaitingRegist in Client_GetArcSuiteAwaitingRegist_ListCopy)
            {
                if (string.IsNullOrWhiteSpace(Client_GetArcSuiteAwaitingRegist.toyoUSERID))
                    sb.AppendLine($"[ServerId: {Client_GetArcSuiteAwaitingRegist.ServerId} TaskID:{Client_GetArcSuiteAwaitingRegist.TaskID}] [接続時刻:\"{Client_GetArcSuiteAwaitingRegist.Command_Accept_DateTime.ToString()}\"] [接続ホスト:\"{Client_GetArcSuiteAwaitingRegist.ClientHost}\"] [ユーザー:\"{Client_GetArcSuiteAwaitingRegist.ClientUser} [コマンド名:\"{Client_GetArcSuiteAwaitingRegist.CommandName}");
                else
                    sb.AppendLine($"[ServerId: {Client_GetArcSuiteAwaitingRegist.ServerId} TaskID:{Client_GetArcSuiteAwaitingRegist.TaskID}] [接続時刻:\"{Client_GetArcSuiteAwaitingRegist.Command_Accept_DateTime.ToString()}\"] [接続ホスト:\"{Client_GetArcSuiteAwaitingRegist.ClientHost}\"] [ユーザー:\"{Client_GetArcSuiteAwaitingRegist.ClientUser} [コマンド名:\"{Client_GetArcSuiteAwaitingRegist.CommandName} [toyoUSERID:\"{Client_GetArcSuiteAwaitingRegist.toyoUSERID}\"]");
            }
            string outmsg = sb.ToString();

            return outmsg;
        }

        /// <summary>
        /// 現在の接続しているクライアントを返す
        /// </summary>
        /// <returns></returns>
        public List<AcceptPipeCommand> GetCurrentConnectClients()
        {
            return Client_GetArcSuiteAwaitingRegistList;
        }

        public bool IsContainClientHost(string clientHost)
        {
            var result = Client_GetArcSuiteAwaitingRegistList.Where(item => item.ClientHost.ToUpper() == clientHost.ToUpper());
            if (result != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 接続毎に記録する
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="objectID"></param>
        /// <param name="taskId"></param>
        /// <param name="toyoUSERID"></param>
        /// <param name="ImpersonationUserName"></param>
        /// <param name="computerName"></param>
        public void SetConnection(int serverId, string objectID, int taskId, string toyoUSERID, string ImpersonationUserName, string computerName)
        {
            string commandName = CMDS.DR_GetArcSuiteAwaitingRegist;
            string ClientHost = null;
            string ClientUser = null;


            try
            {
                FieldValueSet result = UserDataBase.SearchFrom_USERID(toyoUSERID);
                string toyoLASTNAME = result.SearchKey("LASTNAME");
                string toyoFIRSTNAME = result.SearchKey("FIRSTNAME");

                // 接続してきたクライアントのホスト名
                string clientComputerName = computerName;
                ClientHost = Net.DnsGetHostNameOrIP(clientComputerName); // IPアドレスの場合ホスト名を返す

                // 接続してきたクライアントのユーザ名
                ClientUser = ImpersonationUserName;

                IEnumerable<AcceptPipeCommand> recetRecorde;
                List<DateTime> Command_Accept_AddFirst_DateTimes = new List<DateTime>();

                try
                {

                    // 同じホスト名 かつ 同じユーザー かつ 同じコマンド のレコードを抽出する
                    recetRecorde = Client_GetArcSuiteAwaitingRegistList.ToList().Where(value => (value.ClientHost == ClientHost) && (value.toyoUSERID == toyoUSERID) && (value.CommandName == commandName));

                    // 同じホスト名 かつ 同じユーザー かつ 同じコマンド において Command_Accept_DateTime のみを集めた List<DateTime>を作成する。
                    Command_Accept_AddFirst_DateTimes = recetRecorde.ToList().ConvertAll(item => item.Command_Accept_AddFirst_DateTime);

                }
                catch (Exception ex)
                {
                    SasaLib.Eventlog.Log.WriteEntry("StageServerCommon", EventLogEntryType.Error, 9700, $"※DRAWREGISTservice_PipeCommand.authorizedUserConnectionStatus.SetConnection(..) にて例外その１ [PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{taskId}] PC:{commandName}, User:\"{ImpersonationUserName}\" , {ex.Message} {ex.InnerException} ");
                }

                DateTime Command_Accept_DateTime = DateTime.Now;

                DateTime Accept_AddFirst_DateTime;

                if (Command_Accept_AddFirst_DateTimes.Count > 0)
                {
                    Accept_AddFirst_DateTime = Command_Accept_AddFirst_DateTimes.Min(); //一番小さい（若い）DateTimeを 最初のアクセス日時とする
                }
                else
                {
                    Accept_AddFirst_DateTime = Command_Accept_DateTime;
                }

                try
                {

                    //lock (Client_GetArcSuiteAwaitingRegistList_LockHandler)
                    //{
                    // 同じホスト名かつ同じユーザIDかつ同じコマンドかつ受け入れ時刻が最後の時刻以下のレコードを一括削除
                    var removeCount = Client_GetArcSuiteAwaitingRegistList.RemoveAll(value =>
                        (value.ClientHost == ClientHost) &&
                        (value.toyoUSERID == toyoUSERID) &&
                        (value.CommandName == commandName)
                        && (value.Command_Accept_DateTime < Command_Accept_DateTime)
                    );


                    Client_GetArcSuiteAwaitingRegistList.Add(new AcceptPipeCommand { ServerId = serverId, ClientHost = ClientHost, ClientUser = ClientUser, CommandName = commandName, toyoUSERID = toyoUSERID, toyoFULLNAME = $"{toyoLASTNAME} {toyoFIRSTNAME}", Command_Accept_DateTime = Command_Accept_DateTime, Command_Accept_AddFirst_DateTime = Accept_AddFirst_DateTime });
                    //}
                }
                catch (Exception ex)
                {
                    SasaLib.Eventlog.Log.WriteEntry("StageServerCommon", EventLogEntryType.Error, 9700, $"※DRAWREGISTservice_PipeCommand.authorizedUserConnectionStatus.SetConnection(..) にて例外その１ [PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{taskId}] PC:{commandName}, User:\"{ImpersonationUserName}\" , {ex.Message} {ex.InnerException} ");
                }


            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("StageServerCommon", EventLogEntryType.Error, 9700, $"※DRAWREGISTservice_PipeCommand.authorizedUserConnectionStatus.SetConnection(..) にて例外 [PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{taskId}] PC:{commandName}, User:\"{ImpersonationUserName}\" , {ex.Message} {ex.InnerException} ");
            }
        }


    }
}
