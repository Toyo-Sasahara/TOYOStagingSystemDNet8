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

namespace ToyoStageService
{
    /// <summary>
    /// 承認クライアントからの接続リストを保持するクラス
    /// </summary>
    public class List_Client_AcceptCommand
    {
        /// <summary>
        /// コマンドログ
        /// </summary>
        private List<AcceptPipeCommand> _Client_AcceptCommandList = new List<AcceptPipeCommand>();
        private readonly object _client_AcceptCommandList_LockHandler = new object();

        /// <summary>
        /// 
        /// </summary>
        private string _eventLogSourceName;

        /// <summary>
        /// 
        /// </summary>
        private int _purgeBeforeSecond;

        /// <summary>
        /// 
        /// </summary>
        private int _logWriteIntervalSecond;


        /// <summary>
        /// Pipeサーバーコマンドログ定期調査 起動
        /// </summary>
        /// <param name="eventLogSourceName">イベントログソース名</param>
        /// <param name="pipeCommandName">このイベントログ記録のキック元になったPIPEコマンド名</param>
        /// <param name="purgeBeforeSecond">ログ情報をパージする間隔（秒）</param>
        /// <param name="logWriteIntervalSecond">ログへ記録する間隔（秒）</param>
        public List_Client_AcceptCommand(string eventLogSourceName, int purgeBeforeSecond = 60, int logWriteIntervalSecond = 60)
        {
            this._eventLogSourceName = eventLogSourceName;
            this._purgeBeforeSecond = purgeBeforeSecond;
            this._logWriteIntervalSecond = logWriteIntervalSecond;

            Task.Run(() => { RemoveOldRecord(); });
            Task.Run(() => { LogWrite(); });
        }

        /// <summary>
        /// 
        /// </summary>
        private async void RemoveOldRecord()
        {
            while (true)
            {
                try
                {
                    //lock (_client_AcceptCommandList_LockHandler)
                    //{
                    var removeCount = _Client_AcceptCommandList.RemoveAll(w => w.Command_Accept_DateTime < DateTime.Now.AddSeconds(-this._purgeBeforeSecond));

                    if (removeCount > 0)
                    {
                        DebugClass.ConsoleDebugOut(5, $"{StageServerConfig.Config.List_Client_AcceptCommand_HoldSecond} 秒経過した コマンドログ(List<AcceptPipeCommand> Client_AcceptCommandList)を {removeCount} 個消去しました", ConsoleColor.Magenta);
                    }
                    //}
                }
                catch (Exception ex)
                {
                    ServerLog.PiperServerLogging.LogRotateWriteLine($"removeOldRecord()内にて例外検知  {ex.Message} {ex.InnerException}", FlashSync: true);
                }

                await Task.Delay(1 * 1000);
            }
        }

        /// <summary>
        /// サーバが保持している直近のクライアント接続ログを出力
        /// </summary>
        private async void LogWrite()
        {
            while (true)
            {
                string outmsg = GetPipeCommandLog($"");

                //if (StageServerConfig.Config.IsLoggingPIPE_NormalStatus)
                ServerLog.PiperServerLogging.LogRotateWriteLine($"{outmsg}", FlashSync: true);

                await Task.Delay(this._logWriteIntervalSecond * 1000);
            }
        }

        /// <summary>
        /// サーバーが保持している クライアントの接続記録を文字列で得る
        /// </summary>
        /// <returns></returns>
        public string GetPipeCommandLog(string headderMsg)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(headderMsg);
            sb.AppendLine($"{_eventLogSourceName} 受信記録 直近 {_purgeBeforeSecond} 秒以内の接続");

            var _Client_AcceptCommandList_ListCopy = new List<AcceptPipeCommand>(_Client_AcceptCommandList); //  コピーコレクションを準備

            foreach (var _Client_AcceptCommand in _Client_AcceptCommandList_ListCopy)
            {
                if (string.IsNullOrWhiteSpace(_Client_AcceptCommand.toyoUSERID))
                    sb.AppendLine($"[ServerId: {_Client_AcceptCommand.ServerId} TaskID:{_Client_AcceptCommand.TaskID}] [接続時刻:\"{_Client_AcceptCommand.Command_Accept_DateTime.ToString()}\"] [接続ホスト:\"{_Client_AcceptCommand.ClientHost}\"] [ユーザー:\"{_Client_AcceptCommand.ClientUser}\"] [ｺﾏﾝﾄﾞ:\"{_Client_AcceptCommand.CommandName}\"]");
                else
                    sb.AppendLine($"[ServerId: {_Client_AcceptCommand.ServerId} TaskID:{_Client_AcceptCommand.TaskID}] [接続時刻:\"{_Client_AcceptCommand.Command_Accept_DateTime.ToString()}\"] [接続ホスト:\"{_Client_AcceptCommand.ClientHost}\"] [ユーザー:\"{_Client_AcceptCommand.ClientUser}\"] [ｺﾏﾝﾄﾞ:\"{_Client_AcceptCommand.CommandName}\"] [東陽UserID:{_Client_AcceptCommand.toyoUSERID}]");
            }
            string outmsg = sb.ToString();

            return outmsg;
        }

        /// <summary>
        /// 接続毎に記録する
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="commandName"></param>
        /// <param name="toyoUSERID"></param>
        /// <param name="ImpersonationUserName"></param>
        /// <param name="computerName"></param>
        public void SetConnection(int serverId, string objectID, int taskId, string commandName, string toyoUSERID, string ImpersonationUserName, string computerName)
        {
            try
            {
                // 接続してきたクライアントのユーザ名

                //lock (_client_AcceptCommandList_LockHandler)
                //{
                var result = _Client_AcceptCommandList.Where(value => value.ClientUser == ImpersonationUserName).Where(value => value.ClientHost == computerName).Where(value => value.CommandName == commandName);

                if (result.Count() == 0)
                {

                    _Client_AcceptCommandList.Add(new AcceptPipeCommand { ServerId = serverId, ClientHost = computerName, ClientUser = ImpersonationUserName, CommandName = commandName, toyoUSERID = toyoUSERID, Command_Accept_DateTime = DateTime.Now });

                    if (StageServerConfig.Config.IsLoggingPIPE_NormalStatus)
                        ServerLog.PiperServerLogging.LogRotateWriteLine($"[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{taskId}], PipeCommandLog.SetConnection(..) , userName:{ImpersonationUserName} , ClientHost:{computerName} CommandName:{commandName}リストに登録", FlashSync: true);
                }
                else
                {
                    if (StageServerConfig.Config.IsLoggingPIPE_NormalStatus)
                        ServerLog.PiperServerLogging.LogRotateWriteLine($"[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{taskId}], PipeCommandLog.SetConnection(..) , userName:{ImpersonationUserName} , ClientHost:{computerName} CommandName:{commandName}はリストに登録済み", FlashSync: true);
                }
                //}

            }
            catch (Exception ex)
            {
                ServerLog.PiperServerLogging.LogRotateWriteLine($"※[PIPEID:{serverId:D3} \"{objectID}\"], [Task.ID:{taskId}], PipeCommandLog.SetConnection(..) にて例外  接続元:({computerName}) , ユーザー名: {ImpersonationUserName}]  , コマンド名: {commandName}].   例外発生   {ex.Message} {ex.InnerException}", FlashSync: true);
            }
        }


    }
}
