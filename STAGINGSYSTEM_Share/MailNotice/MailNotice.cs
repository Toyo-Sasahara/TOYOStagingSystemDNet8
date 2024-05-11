using MailNotice;
using SasaLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyoStageService;

namespace ToyoStageService
{
    public static class MailNotice
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        // DRAWREGISTserviceからのメール送信
        public static void SendEmailFromDRAWREGISTservice(string MessageType, string Msg , MailAccount mailAccount , string FromAddr)
        {
            string SystemServiceName = "ToyoDRAWREGISTservice";

            var ans = _SendEmail(FromAddr, SystemServiceName, MessageType, Msg , mailAccount);

            if (ans == false)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoMailNotice", EventLogEntryType.Information, 9000,
                    $"{AssemblyInternalName} SendEmailFromDRAWREGISTservice(..)送信失敗" +
                    $"");
            }
        }

        // DRAWCAPTUREserviceからのメール送信
        public static void SendEmailFromDRAWCAPTUREservice(string MessageType, string Msg , MailAccount mailAccount, string FromAddr)
        {
            string SystemServiceName = "SendEmailFromDRAWCAPTUREservice";

            var ans = _SendEmail(FromAddr, SystemServiceName, MessageType, Msg , mailAccount);

            if (ans==false)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoMailNotice", EventLogEntryType.Information, 9000,
                    $"{AssemblyInternalName} SendEmailFromDRAWCAPTUREservice(..)送信失敗" +
                    $"");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MessageType"></param>
        /// <param name="Msg"></param>
        /// <param name="mailAccount"></param>
        /// <param name="FromAddr"></param>
        public static void SendEmailFromSYSTEMWATCHservice(string MessageType, string Msg , MailAccount mailAccount, string FromAddr)
        {
            string SystemServiceName  = "ToyoSYSTEMWATCHservice";

            var ans = _SendEmail(FromAddr, SystemServiceName, MessageType, Msg , mailAccount);

            if (ans == false)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoMailNotice", EventLogEntryType.Information, 9000,
                    $"{AssemblyInternalName} SendEmailFromSYSTEMWATCHservice(..)送信失敗" +
                    $"");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MessageType"></param>
        /// <param name="Msg"></param>
        /// <param name="mailAccount"></param>
        /// <param name="FromAddr"></param>
        //public static void SendEmailFromSYNCSYSTEMservice(string MessageType, string Msg , MailAccount mailAccount, string FromAddr)
        //{
        //    string SystemServiceName = "ToyoSYNCSYSTEMservice";

        //    var ans = _SendEmail(FromAddr, SystemServiceName, MessageType, Msg , mailAccount);




        //    if (ans == false)
        //    {
        //        SasaLib.Eventlog.Log.WriteEntry("ToyoMailNotice", EventLogEntryType.Information, 9000,
        //            $"{AssemblyInternalName} SendEmailFromSYSTEMWATCHservice(..)送信失敗" +
        //            $"");
        //    }
        //}

        public static void SendEmailFromPrinterStatusSystem(string MessageType, string Msg , MailAccount mailAccount, string FromAddr)
        {
            string SystemServiceName = "PrintService";

            var ans = _SendEmail(FromAddr, SystemServiceName, MessageType, Msg , mailAccount);

            if (ans == false)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoMailNotice", EventLogEntryType.Information, 9000, $"{AssemblyInternalName} SendEmailFromPrinterStatusSystem(..)送信失敗" +
                    $"");
            }
        }

        // 共通ライブラリからのメール送信
        public static void SendEmailFromCommonLibrary(string SystemServiceName, string MessageType, string Msg , MailAccount mailAccount, string FromAddr)
        {
            //string FromAddr = StageServerConfig.Config.EMAILFROMCOMMONSERVICEADDR;  // 承認・登録サービスからのメールの送信元アドレス

            var ans = _SendEmail(FromAddr, SystemServiceName, MessageType, Msg , mailAccount);

            if (ans == false)
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoMailNotice", EventLogEntryType.Information, 9000, $"{AssemblyInternalName} SendEmailFromCommonLibrary(..)送信失敗" +
                    $"");
            }
        }


        /// <summary>
        /// メール送信本体
        /// </summary>
        /// <param name="MailFromAddress"></param>
        /// <param name="SystemServiceName"></param>
        /// <param name="MessageType"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        private static bool _SendEmail(string MailFromAddress, string SystemServiceName, string MessageType, string Msg, MailAccount mailAccount)
        {
            // SMTPサーバーアドレスを 設定ファイルより取得
            string SmtpServerAddr = mailAccount.EMAILSERVER;
            if (string.IsNullOrWhiteSpace(SmtpServerAddr))
                return false;

            // SMTP ポート番号を 設定ファイルより取得
            int port = mailAccount.EMAILSENDPORT;
            if (port ==0)
                return false;

            /// SMTP送信先アドレスを 設定ファイルより取得
            string SendToAddr = mailAccount.EMAILNOTICE_SendToADDR;
            if (string.IsNullOrWhiteSpace(SendToAddr))
                return false;

            // SMTP認証ユーザーを 設定ファイルより取得
            string SMTPAUTHUSER = mailAccount.SMTPAUTHUSER;

            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(mailAccount.SMTPAUTHPASS_SasaLibEncryptionType);

            // SMTP認証ユーザー(デコードされた正式パスワード)
            string TrueSmtpAuthPass = encryption.Decoding(mailAccount.SMTPAUTHPASS);


            if (SendToAddr != "" && MailFromAddress != "")
            {
                try
                {
                    SasaLib.Mail mail = new SasaLib.Mail(SmtpServerAddr, port, SMTPAUTHUSER, TrueSmtpAuthPass);

                    mail.MsgSend(MailFromAddress, SendToAddr, $"{MessageType},{Environment.MachineName} {SystemServiceName}",
                                    Msg, eventViewVerbose: true
                                );
                    //mail.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    SasaLib.Eventlog.Log.WriteEntry("ToyoMailNotice", EventLogEntryType.Error, 9000, $"{AssemblyInternalName} SendEmail(...) 例外発生 {ex.Message}" +
                         $"SendToAddr={SendToAddr} , FromAddr={MailFromAddress} , serverAddr={SmtpServerAddr} , System={SystemServiceName} , Type={MessageType} , Msg={Msg}\n\n");
                    return false;
                }
            }
            else
            {
                SasaLib.Eventlog.Log.WriteEntry("ToyoMailNotice", EventLogEntryType.Information, 9000, $"{AssemblyInternalName} SendEmail(...)\n送り先もしくは送信元アドレスが未設定のためメールは送信しません\n" +
                    $"SendToAddr={SendToAddr} , FromAddr={MailFromAddress} , serverAddr={SmtpServerAddr} , System={SystemServiceName} , Type={MessageType} , Msg={Msg}\n\n");
                return false;
            }
        }
    }
}