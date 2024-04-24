using SasaLib;
using SharedClassLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ToyoStageService.ClientPreInputTICKET;

namespace ToyoStageService
{
    /// <summary>
    /// 入力中のチケットコードをｸﾗｲｱﾝﾄから受取一時保存するクラス
    /// </summary>
    public class List_Client_InputTICKET
    {
        /// <summary>
        /// 各クライアントから受信した、押印しようとするチケット情報のリスト
        /// </summary>
        List<ClientPreInputTICKET> Client_CommonInputTicketList = new List<ClientPreInputTICKET>();
        readonly object Client_CommonInputTicketList_LockHandler = new object();

        string eventLogSourceName;

        public List_Client_InputTICKET(string eventLogSourceName, List_Client_GetArcSuiteAwaitingRegist status_AuthorizedUserConnection)
        {
            this.eventLogSourceName = eventLogSourceName;
            try
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        lock (Client_CommonInputTicketList_LockHandler)
                        {
                            var removeCount = Client_CommonInputTicketList.RemoveAll(w => w.PreInputTICKETCODE_DateTime.AddSeconds(StageServerConfig.Config.Client_CommonInputTicketList_HoldSecond) < DateTime.Now);
                            if (removeCount > 0)
                            {
                                DebugClass.ConsoleDebugOut(0, $"{StageServerConfig.Config.Client_CommonInputTicketList_HoldSecond} 秒経過した ログ(List<StageServerConfig.ConfigClient_CommonInputTicketList> Client_CommonInputTicketList)を {removeCount} 個消去しました", ConsoleColor.Magenta);
                            }
                        }

                        await Task.Delay(1 * 1000);
                    } //チケットコードスキャン時から指定時間以上経過したものを削除する
                });

                Task.Run(async () =>
                {
                    while (true)
                    {
                        // CommonApprovalWaitingTicketList から クライアント名のみを抽出
                        List<string> CommonApprovalWaitingTicketList_hosts = Client_CommonInputTicketList.ConvertAll(item => item.ClientHost);

                        // 現在接続中の(GetCurrentConnectClients()で取得)したクライアントを抽出
                        List<AcceptPipeCommand> ConnectClients = status_AuthorizedUserConnection.GetCurrentConnectClients();

                        // ConnectClients オブジェクトからホストを抽出してList化
                        List<string> ClientHosts = ConnectClients.ConvertAll(item => item.ClientHost);

                        lock (Client_CommonInputTicketList_LockHandler)
                        {
                            // 削除対象として現在接続していないホストを抽出
                            List<string> removeHosts = CommonApprovalWaitingTicketList_hosts.Except(ClientHosts).ToList();
                            foreach (var removeHost in removeHosts)
                            {
                                Client_CommonInputTicketList.RemoveAll(w => w.ClientHost.ToUpper() == removeHost.ToUpper());
                            } // 現在接続されていないホストからのClientTicketを削除
                        }

                        await Task.Delay(1 * 10000);
                    } // １0秒間隔でﾁｪｯｸする
                });
            }
            catch (Exception ex)
            {
                DebugClass.ConsoleDebugOut(0, $"List_Client_InputTICKET() ので例外 {ex.Message} {ex.InnerException}", ConsoleColor.Red);
            }
        }

        public List<ClientPreInputTICKET> GetCommonApprovalWaitingTicketList()
        {
            return Client_CommonInputTicketList;
        }

        /// <summary>
        /// 指定図面番号をもつ指定TICKETCODE以外の押印候補チケット情報を検索
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="clientInputTickets"></param>
        /// <returns></returns>
        public List<ClientPreInputTICKET> GetTicketsSpecDRAWNNUMBERFromCommonApprovalWaitingList(string partNumber, string TICKETCODE)
        {
            lock (Client_CommonInputTicketList_LockHandler)
            {

                return Client_CommonInputTicketList
                .Where((item => item.DRAWNUMBER.ToUpper() == partNumber.ToUpper())).Where(item => item.PreInputTICKETCODE != TICKETCODE).ToList();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="partNumbers"></param>
        /// <param name="tickets"></param>
        /// <returns></returns>
        public List<ClientPreInputTICKET> GetContainPARTSNUMBER(List<string> partNumbers, List<ClientPreInputTICKET> tickets)
        {
            List<ClientPreInputTICKET> results = new List<ClientPreInputTICKET>();
            foreach (var partNumber in partNumbers)
            {
                List<ClientPreInputTICKET> a = tickets.Where(item => item.DRAWNUMBER.ToUpper() == partNumber.ToUpper()).ToList();
                results.AddRange(a);
            }
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        public List<ClientPreInputTICKET> GetExceptInputTicketsFromCommonApprovalWaitingList(List<ClientPreInputTICKET> tickets)
        {
            lock (Client_CommonInputTicketList_LockHandler)
            {

                List<ClientPreInputTICKET> newTickets = Client_CommonInputTicketList.Except(tickets).ToList();
                return newTickets;
            }

        }

        /// <summary>
        /// 図面番号が同じものをすべて削除する
        /// </summary>
        /// <param name="drawingNumber"></param>
        public void RemoveAllDrawingNumberFromCommonApprovalWaitingList(string drawingNumber)
        {
            lock (Client_CommonInputTicketList_LockHandler)
            {

                Client_CommonInputTicketList.RemoveAll(item => item.DRAWNUMBER.ToUpper() == drawingNumber.ToUpper());
            }
        }

        public int Remove(string TICKETCODE)
        {
            lock (Client_CommonInputTicketList_LockHandler)
            {

                var result = Client_CommonInputTicketList.RemoveAll(item => item.PreInputTICKETCODE == TICKETCODE);
                return result;
            }
        }
        public bool Remove(ClientPreInputTICKET ticket)
        {
            lock (Client_CommonInputTicketList_LockHandler)
            {

                var result = Client_CommonInputTicketList.Remove(ticket);
                return result;
            }
        }


        /// <summary>
        /// クライアントがスキャンしたチケットコードをバッファに追加する.同じチケットコードの場合は追加しない
        /// </summary>
        /// <param name="TTICKET"></param>
        public void Add(ClientPreInputTICKET ticket)
        {
            List<ClientPreInputTICKET> result = Client_CommonInputTicketList.FindAll(item => item.PreInputTICKETCODE == ticket.PreInputTICKETCODE);

            lock (Client_CommonInputTicketList_LockHandler)
            {
                if (result.Count == 0)
                {
                    Client_CommonInputTicketList.Add(ticket);
                } // 指定したチケット番号がバッファに無ければ追加する
                else
                {
                    foreach (ClientPreInputTICKET item in result)
                        Client_CommonInputTicketList.Remove(item);
                    Client_CommonInputTicketList.Add(ticket);
                } // 指定したチケット番号がバッファに存在する場合はいちど同じチケット番号のオブジェクトを全消去したのち指定したチケットを追加する
            }
        }
    }
}
