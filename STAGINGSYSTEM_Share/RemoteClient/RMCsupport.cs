using SasaLib;
using STAGINGSYSTEM_COMMANDS;
using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace StageServerRemote
{
    [SupportedOSPlatform("windows")]

    /// <summary>
    /// ●サーバから返答された識別文字列が正規のものかをチェックする
    /// このアセンブリから利用されるデリゲートメソッドを定義する
    /// </summary>
    public class RMCsupport
    {

        /// <summary>
        /// 作業中か？
        /// </summary>
        internal bool InSearchWorking { get; set; }

        /// <summary>
        /// サーバーから返答された識別文字列をチェックする
        /// </summary>
        /// <param name="input0">サーバーからの文字列を指定</param>
        /// <returns></returns>
        public bool CheckFirstMessage(string input0, SasaLibDelegateWriteLine WriteLine = null)
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

        internal string SendPipeCommandAndReceveMessage(StreamString stst, string commandName, int readWrteStringTimeOut, SasaLibDelegateWriteLine delegateWriteLine = null)
        {
            if (delegateWriteLine == null) delegateWriteLine = DebugConsole.WriteLine;

            int writeResult = stst.WriteString(commandName);

            bool _OperationCancelException; bool _AggreateExcepton;
            string receved = stst.ReadString(readWrteStringTimeOut, out _OperationCancelException, out _AggreateExcepton, delegateWriteLine);
            if (_OperationCancelException == true || _AggreateExcepton == true)
            {
                throw new Exception($"PIPEｻｰﾊﾞへｺﾏﾝﾄﾞ \"{commandName}\" を送出後 エラーが発生");
            }
            return receved;
        }
    }
}
