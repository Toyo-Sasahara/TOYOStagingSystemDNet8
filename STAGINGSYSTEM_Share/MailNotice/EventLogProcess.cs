//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;
//using System.Runtime.CompilerServices;

//namespace ToyoStageService
//{
//    /// <summary>
//    /// イベントをまとめて一括処理するためのクラス
//    /// </summary>
//    public class EventLogProcess
//    {
//        private readonly StringBuilder sb1;

//        public EventLogProcess()
//        {
//            sb1 = new StringBuilder();
//        }

//        public void clear()
//        {
//            sb1.Clear();
//        }

//        /// <summary>
//        /// イベント追加メソッド
//        /// </summary>
//        /// <param name="format">イベント内容</param>
//        /// <param name="OutConsole">true:Coneole.WriteLine()メソッドで内容を表示sます false:左記の逆</param>
//        /// <param name="CallerMemmberName">true:呼び出し元情報を取り入れます false:左記の逆</param>
//        /// <param name="memberName">使用しないこと</param>
//        /// <param name="sourceFilePath">使用しないこと</param>
//        /// <param name="sourceLineNumber">使用しないこと</param>
//        public void Add(string format, bool OutConsole = true, bool CallerMemmberName = true, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
//        {
//            string input;

//            string datetime = System.DateTime.Now.ToString("HH:mm:ss.fff");
//            sourceFilePath = SasaLib.FileFolder.GetFileName(sourceFilePath);

//            if (CallerMemmberName)
//                input = $"●[{datetime}] , [{sourceFilePath}] , 行:[{sourceLineNumber}] , メンバ:[{memberName}]\n" + string.Format(format) + "\n";
//            else
//                input = $"●{datetime}\n" + string.Format(format);

//            if (OutConsole)
//                Console.WriteLine(input);
//            sb1.AppendLine(input);
//        }

//        /// <summary>
//        /// まとめてイベントログへ送る
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="eventType"></param>
//        /// <param name="eventID"></param>
//        /// <param name="header"></param>
//        /// <param name="footer"></param>
//        public void SendEntry(string source, EventLogEntryType eventType, int eventID, string header = null, string footer = null)
//        {
//            if (header==null)
//                SasaLib.Eventlog.Log.WriteEntry(source, eventType, eventID, sb1.ToString() , true , false);
//            else
//                SasaLib.Eventlog.Log.WriteEntry(source, eventType, eventID, $"{header}\n{sb1}{footer}",true,false);
//            sb1.Clear();
//        }

//        ///// <summary>
//        ///// メールにて送信
//        ///// </summary>
//        ///// <param name="System"></param>
//        ///// <param name="Type"></param>
//        //public void SendEmailFromREGISTADDR(string System, string Type)
//        //{
//        //    MailNotice.SendEmailFromStageServer(System, Type, sb1.ToString());
//        //    sb1.Clear();
//        //}

//        ///// <summary>
//        ///// メールにて送信
//        ///// </summary>
//        ///// <param name="System"></param>
//        ///// <param name="Type"></param>
//        //public void SendEmailFromStageServer(string System, string Type)
//        //{
//        //    MailNotice.SendEmailFromREGISTADDR(System, Type, sb1.ToString());
//        //    sb1.Clear();
//        //}
//    }
//}
