using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MailNotice
{
    /// <summary>
    /// 構築途中 連続して同じメールを送らないようにするスマート・トラップ
    /// </summary>
    internal class SmartMail
    {
        internal static List<buffer> buffers = new List<buffer>();

        public SmartMail()
        {

        }

        public bool _SendEmail(string MailFromAddress, string SystemServiceName, string MessageType, string Msg, MailAccount mailAccount)
        {
            if (bufferUpdate( MailFromAddress,  SystemServiceName,  MessageType,  Msg))
            {
                
            }

            return false;
        }

        private bool bufferUpdate(string MailFromAddress, string SystemServiceName, string MessageType, string Msg)
        {

            var sameItems = buffers.Find(item => item.MailFromAddress == MailFromAddress && item.SystemServiceName == SystemServiceName && item.Msg == Msg);

            //sameItems.Find(items2=> items2.DateTime )

            var buffer = new buffer()
            {
                DateTime = DateTime.Now,
                MailFromAddress = MailFromAddress,
                SystemServiceName = SystemServiceName,
                MessageType = MessageType,
                Msg = Msg
            };

            buffers.Add(buffer);

            return false;
        }
    }

    class buffer
    {
        internal int ID { get;}
        internal DateTime DateTime;
        internal string MailFromAddress;
        internal string SystemServiceName;
        internal string MessageType;
        internal string Msg;

        internal buffer()
        {
            ID = ++ID;
        }
    }
}
