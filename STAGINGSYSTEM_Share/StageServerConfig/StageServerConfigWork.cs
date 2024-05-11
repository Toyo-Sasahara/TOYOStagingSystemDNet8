using SasaLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ToyoStageService
{
    /// <summary>
    /// メインコンフィグファイル制御クラス
    /// </summary>
    public class StageServerConfigWork
    {
        static string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        static SasaLib.Mail mail;

        /// <summary>
        /// 
        /// </summary>
        public static DateTime ReadTime;

        // その他から呼ばれる　メインコンフィグファイルのフォルダ
        public static string ConfigFolder = System.IO.Path.Combine(FileFolder.GetCommonApplicationData(), @"TOYOCOMMON");

        static string ConfigFileFullpath;

        /// <summary>
        /// サーバーの設定変数
        /// </summary>
        /// <param name="WriteEvent"></param>
        /// <returns></returns>
        static public bool ReadStageServerConfig(bool WriteEvent = false)
        {
            return (ReadStageServerConfig(ConfigFileFullpath, WriteEvent));
        }

        /// <summary>
        /// StageServerConfig設定ファイルを読み込む。最初に読み込まれることを想定
        /// </summary>
        /// <param name="STSConfigFileFullpath">フルパス</param>
        /// <param name="WriteEvent">true:イベントログ書出し</param>
        /// <returns></returns>
        static public bool ReadStageServerConfig(string STSConfigFileFullpath, bool WriteEvent = false)
        {
            ConfigFileFullpath = STSConfigFileFullpath;


            if (WriteEvent)
            {
                SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Information, 9000, $"図面承認・登録システム {AssemblyInternalName}\nStageServerConfigWork.ReadStageServerConfig()実行します");
            }

            System.IO.StreamReader sr = null;
            try
            {
                // 設定ファイルフォルダ "C:\\ProgramData\\TOYOCOMMON" を取得（作成）
                FileFolder.MakeDirectory(ConfigFolder);

                if (FileFolder.FileExists(STSConfigFileFullpath) != true)
                {
                    /// StageServerConfigの設定ファイルが無い場合は作成
                    MakeServerConfig(STSConfigFileFullpath);
                }

                try
                {
                    // 保存した設定ファイル内容を復元する
                    XmlSerializer serializer = new XmlSerializer(typeof(StageServerConfig));
                    sr = new System.IO.StreamReader(STSConfigFileFullpath, new System.Text.UTF8Encoding(false));
                    StageServerConfig.Config = (StageServerConfig)serializer.Deserialize(sr);
                    sr.Close();
                }
                catch (InvalidOperationException iex)
                {
                    sr.Close();

                    SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Error, 9000,
                        $"図面承認・登録システム {AssemblyInternalName}\nStageServerConfigWork.ReadStageServerConfig()にて例外検知。サービスを終了します。読込コンフィグ：{STSConfigFileFullpath}\n\n内容 {iex.InnerException}",OutConsole:true);
                    Environment.Exit(1);
                }
                catch (Exception ex)
                {
                    sr.Close();

                    SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Error, 9000,
                        $"図面承認・登録システム {AssemblyInternalName}\nStageServerConfigWork.ReadStageServerConfig()にて例外検知。サービスを終了します。\n内容 {ex.Message}", OutConsole: true);
                    SendEmailFromStageServerConfig($"StageServerConfig", "重大エラー",
                        $"図面承認・登録システム {AssemblyInternalName}\nStageServerConfigWork.ReadStageServerConfig()にて例外検知。サービスを終了します。読込コンフィグ：{STSConfigFileFullpath}\n\r内容 {ex.InnerException}");
                    Environment.Exit(1);
                }

                /// DATASOURCE, SUB_DATASOURCE が未指定の場合はそれぞれホスト名を設定する
                if (string.IsNullOrWhiteSpace(StageServerConfig.Config.DATASOURCE))
                {
                    StageServerConfig.Config.DATASOURCE = StageServerConfig.Config.DBHOST;
                    SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Information, 9000, $"StageServerConfig.XML に DATASOURCE が 存在していないため StageServerConfig.Config.DBHOST を 設定しました");
                    SaveConfig();
                }
                if (string.IsNullOrWhiteSpace(StageServerConfig.Config.SUB_DATASOURCE))
                {
                    StageServerConfig.Config.SUB_DATASOURCE = StageServerConfig.Config.SUB_DBHOST;
                    SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Information, 9000, $"StageServerConfig.XML に SUB_DATASOURCE が 存在していないため StageServerConfig.Config.SUB_DBHOST {StageServerConfig.Config.SUB_DBHOST}を 代わりに設定しました");
                    SaveConfig();
                }

                if (WriteEvent)
                {
                    SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Information, 9000,
                        $"設定ファイル：{STSConfigFileFullpath} を読み込みました\n\n" +
                        $"VERSION:{StageServerConfig.Config.VERSION}\n" +
                        $"SERVERMODE:{StageServerConfig.Config.SERVERMODE}\n" +
/* 0*/                  $"COMMITACCEPT:{StageServerConfig.Config.COMMITACCEPT}\n" +
/* 1*/                  $"EMAILALERTADDR:{StageServerConfig.Config.EMAILNOTICE_SendToADDR}\n" +
/* 2*/                  $"EMAILFROMADDR:{StageServerConfig.Config.EMAILFROMCAPTURESYSADDR}\n" +
/* 3*/                  $"EMAILFROMREGISTSYSADDR:{StageServerConfig.Config.EMAILFROMREGISTSYSADDR}\n" +
/* 4*/                  $"EMAILSERVER:{StageServerConfig.Config.EMAILSERVER}\n" +
/* 5*/                  $"EMAILSENDPORT:{StageServerConfig.Config.EMAILSENDPORT}\n" +
/* 6*/                  $"CommitFolder:{StageServerConfig.Config.CommitFolder}\n" +
/* 7*/                  $"CommitFolderUNC:{StageServerConfig.Config.CommitFolderUNC}\n" +
/* 8*/                  $"FileStoreFolder:{StageServerConfig.Config.FileStoreFolder}\n" +
/* 9*/                  $"FileStoreFolderUNC:{StageServerConfig.Config.FileStoreFolderUNC}\n" +
/*10*/                  $"DBHOST:{StageServerConfig.Config.DBHOST}\n" +
/*10*/                  $"DATASOURCE:{StageServerConfig.Config.DATASOURCE}\n" +
/*10c*/                 $"SUB_DBHOST:{StageServerConfig.Config.SUB_DBHOST}\n" +
/*10c*/                 $"SUB_DATASOURCE:{StageServerConfig.Config.SUB_DATASOURCE}\n" +
/*10d*/                 $"SUB_FileStoreForderUNC:{StageServerConfig.Config.SUB_FileStoreForderUNC}\n" +
/*11*/                  $"HandShakeKeyword:{StageServerConfig.Config.HandShakeKeyword}\n" +
/*12*/                  $"DBNAME:{StageServerConfig.Config.DBNAME}\n" +
/*13*/                  $"DBcontrolUser:{StageServerConfig.Config.DBcontrolUser}\n" +
/*15*/                  $"ArcSuiteDMS:{StageServerConfig.Config.ArcSuiteDMS}\n" +
/*16*/                  $"ArcSuiteConfigFolder:{StageServerConfig.Config.ArcSuiteConfigFolder}\n" +
/*17*/                  $"ArcSuiteTicketConfigFile:{StageServerConfig.Config.ArcSuiteTicketConfigFile}\n" +
/*18*/                  $"ArcSuiteDocumentRegistWorkFolder:{StageServerConfig.Config.ArcSuiteDocumentRegistWorkFolder}\n" +
/*19*/                  $"ArcSuiteSDKuserName:{StageServerConfig.Config.ArcSuiteSDKuserName}\n" +
///*21*/                  $"ArcSuiteDocumentRegistCommand:{StageServerConfig.Config.ArcSuiteDocumentRegistCommand}\n" +
///*22*/                  $"ArcSuiteDocumentRegistCSV:{StageServerConfig.Config.ArcSuiteDocumentRegistWorkingCSVFile}\n" +
///*23*/                  $"ArcSuiteDocumentRegistXML:{StageServerConfig.Config.ArcSuiteDocumentRegistXMLFile}\n" +
///*24*/                  $"ArcSuiteDocumentRegistAnserFileFullPath:{StageServerConfig.Config.ArcSuiteDocumentRegistAnserFileFullPath}\n" +
/*25*/                  $"ArcSuiteDocumentAttrGettWorkFolder:{StageServerConfig.Config.ArcSuiteDocumentAttrGettWorkFolder}\n" +
///*26*/                  $"ArcSuiteDocumentAttrGetCommand:{StageServerConfig.Config.ArcSuiteDocumentAttrGetCommand}\n" +
///*27*/                  $"ArcSuiteDoucmnetAttrGetCsvTemplate:{StageServerConfig.Config.ArcSuiteDoucmnetAttrGetCsvTemplate}\n" +
///*28*/                  $"ArcSuiteDocumentAttrGetXmlTemplate:{StageServerConfig.Config.ArcSuiteDocumentAttrGetXmlTemplate}\n" +
/*28A*/                 $"ArcSuiteDocumentAttrMergeWorkFolder:{StageServerConfig.Config.ArcSuiteDocumentAttrMergeWorkFolder}\n" +
///*28B*/                 $"ArcSuiteDocumentAttrMergeCommand:{StageServerConfig.Config.ArcSuiteDocumentAttrMergeCommand}\n" +
///*28C*/                 $"ArcSuiteDocumentAttrMergeFileFullPath:{StageServerConfig.Config.ArcSuiteDocumentAttrMergeAnserFileFullPath}\n" +
///*29*/                  $"ArcSuiteDocumentDeleteCommand:{StageServerConfig.Config.ArcSuiteDocumentDeleteCommand}\n" +
///*30*/                  $"ArcSuiteDocumentDeleteCSV:{StageServerConfig.Config.ArcSuiteDocumentDeleteCSV}\n" +
///*31*/                  $"ArcSuiteDoucmnetDeleteXML:{StageServerConfig.Config.ArcSuiteDoucmnetDeleteXML}\n" +
/*32*/                  $"PrintersConfigFolder:{StageServerConfig.Config.PrintersConfigFolder}\n" +
/*33*/                  $"PlotSetting_A0L:{StageServerConfig.Config.PlotSetting_A0L}\n" +
/*33*/                  $"PlotSetting_A0P:{StageServerConfig.Config.PlotSetting_A0P}\n" +
/*34*/                  $"PlotSetting_A1L:{StageServerConfig.Config.PlotSetting_A1L}\n" +
/*34*/                  $"PlotSetting_A1P:{StageServerConfig.Config.PlotSetting_A1P}\n" +
/*35*/                  $"PlotSetting_A2L:{StageServerConfig.Config.PlotSetting_A2L}\n" +
/*35*/                  $"PlotSetting_A2P:{StageServerConfig.Config.PlotSetting_A2P}\n" +
/*36*/                  $"PlotSetting_A3L:{StageServerConfig.Config.PlotSetting_A3L}\n" +
/*36*/                  $"PlotSetting_A3P:{StageServerConfig.Config.PlotSetting_A3P}\n" +
/*37*/                  $"PlotSetting_A4L:{StageServerConfig.Config.PlotSetting_A4L}\n" +
/*37*/                  $"PlotSetting_A4P:{StageServerConfig.Config.PlotSetting_A4P}\n" +
/*38*/                  $"PlotSetting_unknown:{StageServerConfig.Config.PlotSetting_unknown}\n" +
///*39*/                  $"BeforePrintingImageFullPath:{StageServerConfig.Config.BeforePrintingImageFullPath}\n" +
/*40*/                  $"CapturePollingTime:{StageServerConfig.Config.CapturePollingTime}\n" +
/*41*/                  $"ImmediateryPrinting:{StageServerConfig.Config.ImmediateryPrinting}\n" +
/*42*/                  $"ArcSuiteRegistWaitTime:{StageServerConfig.Config.ArcSuiteRegistWaitTimeMinutes}\n" +
/*43*/                  $"TESTMODE:{StageServerConfig.Config.TESTMODE}\n"
                        , false
                    );
                }


                ReadTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Error, 9000, $"図面承認・登録システム {AssemblyInternalName}\nnReadStageServerConfig(...) 【StageServerConfig.XML】 解析部で例外が発生。コミット・承認処理 共に受付を禁止に移行します。\n{ex.Message}");

                StageServerConfig.Config.COMMITACCEPT = false;
                StageServerConfig.Config.COMMITACCEPTFALSEMSG = $"【StageServerConfig.XML】 解析部で例外発生。コミット・承認処理 共に受付禁止中です。";
                StageServerConfig.Config.APPROVINGACCEPT = false;

                SendEmailFromStageServerConfig($"StageServerConfig", "警告",
                    $"図面承認・登録システム {AssemblyInternalName}\nReadStageServerConfig(...) 【StageServerConfig.XML】 解析部で例外発生。コミット・承認処理 共に受付禁止中です。");

                return false;
            }
            return true;
        }

        /// <summary>
        /// StageServer設定ファイルが存在しないとき、基本設定にてファイルを生成する
        /// </summary>
        /// <param name="confiugFullPath"></param>
        public static void MakeServerConfig(string confiugFullPath)
        {

            // パラメータを保存する
            StageServerConfig.Config = new StageServerConfig
            {
            };

            XmlSerializer serializer = new XmlSerializer(typeof(StageServerConfig));
            using (StreamWriter sw = new StreamWriter(confiugFullPath, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, StageServerConfig.Config);
                SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Information, 9000, $"設定ファイル {confiugFullPath} を強制作成しました");
            }

        }

        /// <summary>
        /// 現在の変数で保存
        /// </summary>
        public static void SaveConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(StageServerConfig));
            try
            {
                // ファイルがロックされているならなにもしない
                if (SasaLib.FileFolder.IsFileLocked(ConfigFileFullpath))
                {
                    SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Error, 9000, $"※設定ファイル {ConfigFileFullpath} を現在の変数で保存に失敗 理由:ﾌｧｲﾙがﾛｯｸされていました");
                    return;
                }
                using (StreamWriter sw = new StreamWriter(ConfigFileFullpath, false, Encoding.UTF8))
                {
                    serializer.Serialize(sw, StageServerConfig.Config);
                    SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Information, 9000, $"※設定ファイル {ConfigFileFullpath} を現在の変数で保存に成功");
                }
            }
            catch (IOException ioe)
            {
                SasaLib.Eventlog.Log.WriteEntry("StageServerConfig", EventLogEntryType.Error, 9000, $"※設定ファイル {ConfigFileFullpath} を現在の変数で保存に失敗 理由：{ioe.Message} {ioe.InnerException}");
            }
        }

        internal static bool SendEmailFromStageServerConfig(string SystemServiceName, string MessageType, string Msg)
        {
            // 送信元アドレス
            string MailFromAddress = StageServerConfig.Config.EMAILFROMSTAGESERVERCONFIGADDR;

            // SMTPサーバーアドレスを 設定ファイルより取得
            string SmtpServerAddr = StageServerConfig.Config.EMAILSERVER;
            if (string.IsNullOrWhiteSpace(SmtpServerAddr))
                return false;

            // SMTP ポート番号を 設定ファイルより取得
            int port = StageServerConfig.Config.EMAILSENDPORT;
            if (port == 0)
                return false;

            /// SMTP送信先アドレスを 設定ファイルより取得
            string SendToAddr = StageServerConfig.Config.EMAILNOTICE_SendToADDR;
            if (string.IsNullOrWhiteSpace(SendToAddr))
                return false;

            // SMTP認証ユーザーを 設定ファイルより取得
            string SMTPAUTHUSER = StageServerConfig.Config.SMTPAUTHUSER;

            // SasaLib.Encryption クラスを使い 暗号を復号化する
            SasaLib.Encryption encryption = new Encryption(StageServerConfig.Config.SMTPAUTHPASS_SasaLibEncryptionType);

            // SMTP認証ユーザー(デコードされた正式パスワード)
            string TrueSmtpAuthPass = encryption.Decoding(StageServerConfig.Config.SMTPAUTHPASS);


            if (SendToAddr != "" && MailFromAddress != "")
            {
                try
                {
                    mail = new SasaLib.Mail(SmtpServerAddr, port, SMTPAUTHUSER, TrueSmtpAuthPass);

                    mail.MsgSend(MailFromAddress, SendToAddr, $"{MessageType},{Environment.MachineName} {SystemServiceName}",
                                    Msg, eventViewVerbose: true
                                );

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
