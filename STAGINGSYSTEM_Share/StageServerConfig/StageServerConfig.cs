using STAGINGSYSTEM_COMMANDS;
using System;
using System.Xml.Serialization;

/// <summary>
/// StageServeサービスのための主設定
/// </summary>
namespace ToyoStageService
{
    public class StageServerConfig
    {
        /// <summary>
        /// ステージサーバー共通設定ファイル StageServerConfig.XML
        /// </summary>
        public static StageServerConfig Config { set; get; }

        public string SERVERLOGFOLDER = @"D:\STAGING\serverlog";

        /// <summary>
        /// この設置ファイルの書式バージョン
        /// </summary>
        public string VERSION = "VER1.1-1019";

        /// <summary>
        /// サービスをコンソールより起動したときに表示されるログレベル
        /// </summary>
        public string DebugWriteLevel_c = @"コンソールデバッグメッセージ表示レベル";
        public int DebugWriteLevel_DRAWCAPTUREservice = 5;
        public int DebugWriteLevel_DRAWREGISTservice = 5;
        public int DebugWriteLevel_SYSTEMWATCHservice = 5;

        public string ConfigFileAutoBackup_c = @"設定ファイルを起動時に複製するか";
        /// <summary>
        /// 設定ファイルを起動時に複製するか
        /// </summary>
        public bool ConfigFileAutoBackup = true;

        public string ConfigFileDeserialzeTime_c = @"一部の設定ファイルのデシアライズリピート間隔(秒)。";
        /// <summary>
        /// 一部の設定ファイルデシアライズのリピート間隔(秒).
        /// </summary>
        public int ConfigFileDeserialzeTime = 10;

        /// <summary>
        /// サーバーの稼働モード（Master 主側、Slave レプリケーション側）
        /// </summary>
        public enum ServerMode { Master, Slave, Unknown }

        public string SERVERMODE_c = @"サーバーの稼働モード（Master 主側、Slave レプリケーション側）";
        /// <summary>
        /// サーバー稼働モード
        /// </summary>
        public ServerMode SERVERMODE;

        //public string WindowsAdministratorUserName_c = @"Windows管理者アカウント";
        //public string WindowsAdministratorUserName = "Administrator";
        //public string WindowsAdministratorPassword_c = @"Windows管理者アカウントのパスワード（暗号化されています）";
        //public string WindowsAdministratorPassword = "xM5xd6waInuJTpVgovXUqfs04gnSvhndETa3FLvJS2c=";
        //public string WindowsAdministratorPassword_SasaLibEncryptionType_c = @"Windows管理者アカウントのパスワード 暗号化規格名";
        //public string WindowsAdministratorPassword_SasaLibEncryptionType = "SasaAuth3.1";


        public string ArcSuiteRegistrationCycle_c = @"ArcSuite登録サイクル RMapprovedProcess.ArcSuiteRegistFromDB(..) を実行するか否かの設定 メインホスト・サブホストどちらかのみをtrueにすること";
        /// <summary>
        /// ArcSuite登録サイクル RMapprovedProcess.ArcSuiteRegistFromDB(..) を実行するか否かの設定
        /// </summary>
        public bool ArcSuiteRegistrationCycle = false;

        public string COMMITACCEPT_c = @"コミット受付可能の場合true (falseの場合コミットフォルダは受け付けません（コミットパスに書き込まれたファイルは削除されます）";
        /// <summary>
        /// コミット受付可能の場合true falseの場合コミットフォルダは受け付けません（削除）
        /// </summary>
        public bool COMMITACCEPT = true;

        /// <summary>
        /// データベースバックアップ時などにtrueにする
        /// </summary>
        [XmlIgnore()] public bool CommitHaltFlagForInternalSystem;


        public string COMMITACCEPTFALSEMSG_c = @"コミット受付不可の場合、クライアントに返すメッセージ（"" の場合メソッドで指定されたメッセージ）";
        /// <summary>
        /// コミット受付不可の場合、クライアントに返すメッセージ（"" の場合メソッドで指定されたメッセージ）
        /// </summary>
        public string COMMITACCEPTFALSEMSG = "";

        public string APPROVINGACCEPT_c = @"承認受付可能の場合true falseの場合承認は受け付けません）";
        /// <summary>
        /// 承認受付可能の場合true falseの場合承認は受け付けません）
        /// </summary>
        public bool APPROVINGACCEPT = true;

        public string APPROVINGACCEPTFALSEMSG_c = @"承認受付不可の場合、クライアントに返すメッセージ";
        /// <summary>
        /// 承認受付不可の場合、クライアントに返すメッセージ
        /// </summary>
        public string APPROVINGACCEPTFALSEMSG = "";

        public string REPLICATIONTOSUBHOST_c = @"サブホストへの複製を実行するか否か";
        /// <summary>
        /// サブホストへの複製を実行するか否か
        /// </summary>
        public bool REPLICATIONTOSUBHOST = true;

        public string ImmediateryPrinting_c = @"コミット完了後すぐに印刷するか否かを切替 true=即印刷";
        /// <summary>
        /// Debug用 コミット完了後すぐに印刷するか否かを切替 true=即印刷
        /// </summary>
        public bool ImmediateryPrinting = true;

        public string TESTMODE_c = @"現在のサーバーの設定モード falseなら通常 、trueならテストモード";
        /// <summary>
        /// Debug用 現在のサーバーの設定モード Falseなら通常 、Trueならテストモード
        /// </summary>
        public bool TESTMODE = false;


        public string EMAILNOTICE_SendToADDR_c = @"警告メール送信先アドレス";
        /// <summary>
        /// 警告メール送信先アドレス
        /// </summary>
        public string EMAILNOTICE_SendToADDR = "y-sasahara@softbank.ne.jp";

        public string EMAILFROMCAPTURESYSADDR_c = @"メール送信元アドレス（キャプチャリングサービス）";
        /// <summary>
        /// メール送信元アドレス（キャプチャリングサービス）
        /// </summary>
        public string EMAILFROMCAPTURESYSADDR = "DarwCAPTUREservice@toyo-mc-mfg.co.jp";

        public string EMAILFROMREGISTSYSADDR_c = @"メール送信元アドレス（承認・登録サービス）";
        /// <summary>
        /// メール送信元アドレス(承認・登録システム)
        /// </summary>
        public string EMAILFROMREGISTSYSADDR = "DrawREGISTservice@toyo-mc-mfg.co.jp";

        public string EMAILFROMSYSWATCHADDR_c = @"メール送信元アドレス（システム監視サービス）";
        /// <summary>
        /// メール送信元アドレス(システム監視サービス)
        /// </summary>
        public string EMAILFROMSYSWATCHADDR = "SystemWATCHservice@toyo-mc-mfg.co.jp";

        public string EMAILFROMSTAGESERVERCONFIGADDR_c = @"メール送信元アドレス（ステージサーバーコンフィギュサービス）";
        /// <summary>
        /// メール送信元アドレス(ステージサーバーコンフィギュサービス)
        /// </summary>
        public string EMAILFROMSTAGESERVERCONFIGADDR = "StageServerConfig@toyo-mc-mfg.co.jp";

        public string EMAILFROMPRINTERSTATUSADDR_c = @"メール送信元アドレス（プリンタステータス監視）";
        /// <summary>
        /// メール送信元アドレス（プリンタステータス監視）
        /// </summary>
        public string EMAILFROMPRINTERSTATUSADDR = "PrinterStatus@toyo-mc-mfg.co.jp";

        public string EMAILFROMCOMMONSERVICEADDR_c = @"メール送信元アドレス（共通サービス）";
        /// <summary>
        /// メール送信元アドレス(共通サービス)
        /// </summary>
        public string EMAILFROMCOMMONSERVICEADDR = "CommonLibrary@toyo-mc-mfg.co.jp";

        public string EMAILSERVER_c = @"メールサーバーアドレス";
        /// <summary>
        /// メールサーバーアドレス
        /// </summary>
        public string EMAILSERVER = "";

        public string EMAILSENDPORT_c = @"メールサーバーポート";
        /// <summary>
        /// メールサーバーポート
        /// </summary>
        public int EMAILSENDPORT;

        public string SMTPAUTHUSER_c = @"SMTP認証ユーザー名";
        /// <summary>
        /// SMTP認証ユーザー名
        /// </summary>
        public string SMTPAUTHUSER = "";

        public string SMTPAUTHPASS_c = @"SMTP認証接続パスワード(暗号化されています)";
        /// <summary>
        /// SMTP認証ユーザー名
        /// </summary>
        public string SMTPAUTHPASS = "";

        public string SMTPAUTHPASS_SasaLibEncryptionType_c = @"SMTP認証接続パスワード 暗号化規格名";
        /// <summary>
        /// SMTP認証接続パスワード 暗号化規格名
        /// </summary>
        public string SMTPAUTHPASS_SasaLibEncryptionType = "SasaAuth2.1";

        [XmlIgnore()] public string CapturePollingTime_c = @"キャプチャサービスのコミットフォルダポーリング間隔(msec)";
        /// <summary>
        /// キャプチャサービスのポーリング間隔(msec)
        /// </summary>
        [XmlIgnore()] public int CapturePollingTime = 1000;

        public string CommitWaitingFileThreshold_c = @"コミット待ち許容数（この数値を超えたらｺﾐｯﾄ処理が滞っていると判断する）";
        /// <summary>
        /// コミット待ち許容数（この数値を超えたら印刷が滞っていると判断する
        /// </summary>
        public int CommitWaitingFileThreshold = 10;

        public string ElapsedSecondSinceCreation_c = @"コミット待ちのうち指定秒数を超えた古いチケットを検索する";
        /// <summary>
        /// コミット待ち許容数を超えたとき、そのファイルの中で作成から次の秒数を超えたものを通告する
        /// </summary>
        public int ElapsedSecondSinceCreation = 300;

        public string RecordOfErrEvents_CreatedTimeElapsedMinutes_c = @"コミットフォルダのファイル中で作成から次の秒数を超えたものをeventビューアーに記録する";
        /// <summary>
        /// コミットフォルダのファイル中で作成から次の秒数を超えたものをeventビューアーに記録する
        /// </summary>
        public int RecordOfErrEvents_CreatedTimeElapsedMinutes = 6800;

        public string CommitFolder_c = @"コミット受付フォルダ（サーバー側ローカル）の指定";
        /// <summary>
        /// コミット受付フォルダ（サーバー側ローカル）の指定
        /// </summary>
        public string CommitFolder = @"D:\COMMIT";

        public string CommitFolderUNC_c = @"コミットフォルダの指定(UNC形式)";
        /// <summary>
        /// コミットフォルダの指定(UNC形式)
        /// </summary>
        public string CommitFolderUNC = @"\\localhost\COMMIT$";

        public string FileStoreFolder_c = @"ステージングフォルダの場所（サーバー側ローカル）";
        /// <summary>
        /// ステージングフォルダの場所（サーバー側ローカル）
        /// </summary>
        public string FileStoreFolder = @"D:\STAGING\FILESTORE";

        public string FileStoreFolderUNC_c = @"ステージングフォルダの場所(外部から見たUNC形式)";
        /// <summary>
        /// ステージングフォルダの場所(外部から見たUNC形式)
        /// </summary>
        public string FileStoreFolderUNC = @"\\localhost\STAGING$\FILESTORE";


        public string DBHOST_c = @"プライマリデータベースサーバー名 例:CS1";
        /// <summary>
        /// プライマリデータベースサーバー名。例:CS1
        /// </summary>
        public string DBHOST = "localhost";

        public string DATASOURCE_c = @"プライマリデータベース データソース文字列 例:CS1 , CS1,1433";
        /// <summary>
        /// 
        /// </summary>
        public string DATASOURCE = "localhost";

        public string SUB_DBHOST_c = @"冗長性確保のためのセカンダリデータベースホスト名。例:CS2";
        /// <summary>
        /// 冗長性確保のためのセカンダリデータベースホスト名。例:CS2
        /// </summary>
        public string SUB_DBHOST = "";

        public string SUB_DATASOURCE_c = @"セカンダリデータベース データソース文字列";
        /// <summary>
        /// 
        /// </summary>
        public string SUB_DATASOURCE = "";

        public string SUB_FileStoreForderUNC_c = @"冗長性確保のためのコピー先 例:\\CS2\FILESTORE$. 相手先フォルダのセキュリティ・共有フォルダのセキュリテイにはサーバーアカウントを指定してください。";
        /// <summary>
        /// 冗長性確保のためのコピー先 例:\\CS2\FILESTORE$. 相手先フォルダのセキュリティ・共有フォルダのセキュリテイにはサーバーアカウントを指定してください。
        /// </summary>
        public string SUB_FileStoreForderUNC = @"";

        /// <summary>
        /// ToyoDRAWREGISTservice接続用パイプ名
        /// </summary>
        [XmlIgnore()] public string PipeNameDR { get; set; } = "ApprovalServer";

        [XmlIgnore()] public string PipeNameARCSDKCTL { get; set; } = "ARCSDKCTLServer";

        public string PipeNameDR_NumberOfTasks_c = @"PipeNameDRの待受けﾀｽｸ個数";
        /// <summary>
        /// PipeNameDRの待受けﾀｽｸ個数
        /// </summary>
        public int PipeNameDR_NumberOfTasks = 200;
        public int PipeNameDR_CheckNumberOfTasks = 100;

        /// <summary>
        ///  ToyoDRAWCAPTUREservice接続用パイプ名
        /// </summary>
        [XmlIgnore()] public string PipeNameDC { get; set; } = "CaptureService";

        public string PipeNameDC_NumberOfTasks_c = @"PipeNameDCの待受けﾀｽｸ個数";
        /// <summary>
        /// PipeNameDCの待受けﾀｽｸ個数
        /// </summary>
        public int PipeNameDC_NumberOfTasks = 200;
        public int PipeNameDC_CheckNumberOfTasks = 100;

        /// <summary>
        ///  ToyoDRAWREGISTservice接続用パイプ名
        /// </summary>
        [XmlIgnore()] public string PipeNameSW { get; set; } = "WatchService";

        public string PipeNameSW_NumberOfTasks_c = @"PipeNameSWの待受けﾀｽｸ個数";
        /// <summary>
        /// PipeNameSWの待受けﾀｽｸ個数
        /// </summary>
        public int PipeNameSW_NumberOfTasks = 200;
        public int PipeNameSW_CheckNumberOfTasks = 100;

        /// <summary>
        /// クライアントサーバー間PIPE通信でハンドシェイクに用いるキーワード
        /// </summary>
        [XmlIgnore()] public string HandShakeKeyword = CMDS.ConnectKeyword;


        public string PipeClientStrmeConnectDefaultTimeOut_c = @"PipeServer Connecton がデフォルトとでタイムアウトになるmsec";
        /// <summary>
        /// PipeServer Connecton がデフォルトとでタイムアウトになるmsec
        /// </summary>
        public int PipeClientStrmeConnectDefaultTimeOut = 12000;

        public string ReadWriteStreamStringDefaultTimeOut_c = @"Stream string がﾃﾞﾌｫﾙﾄでタイムアウトになる msec";
        /// <summary>
        ///Stream string がﾃﾞﾌｫﾙﾄでタイムアウトになる msec
        /// </summary>
        public int ReadWriteStreamStringDefaultTimeOut = 40000;

        public string ReadWriteHandShakeStreamStringTimeOut_c =  @"ハンドシェイクでStream string がﾃﾞﾌｫﾙﾄでタイムアウトになるmsec";
        /// <summary>
        ///  ハンドシェイクでStream string がﾃﾞﾌｫﾙﾄでタイムアウトになるmsec
        /// </summary>
        public int ReadWriteHandShakeStreamStringTimeOut = 14000;

        public string IsLoggingPIPE_NormalStatus_c = @"正常実行時のPIPEコマンドに関するログを記録するか否か";
        /// <summary>
        /// 正常実行時のPIPEコマンドに関するログを記録するか否か
        /// </summary>
        public bool IsLoggingPIPE_NormalStatus = true;

        /// <summary>
        /// PIPECONNECTIONLOOP で BUSYだった回数
        /// </summary>
        [XmlIgnore()] public int PipeServer_BUSY_Count = 0;

        public string DBNAME_c = @"データベース名";
        /// <summary>
        /// データベース名
        /// </summary>
        public string DBNAME = "DATABASE1";

        public string DBcontrolUser_c = @" データベース接続ユーザー名";
        /// <summary>
        /// データベース接続ユーザー名
        /// </summary>
        public string DBcontrolUser = "sa";

        public string DBcontrolUserPass_c = @"データベース接続ユーザー用パスワード(暗号化されています)";
        /// <summary>
        ///  データベース接続ユーザー用パスワード
        /// </summary>
        public string DBcontrolUserPass = "";

        public string DBcontrolUserPass_SasaLibEncryptionType_c = @"データベース接続ユーザー用パスワード 暗号化規格名";
        /// <summary>
        ///  データベース接続ユーザー用パスワード暗号化規格名
        /// </summary>
        public string DBcontrolUserPass_SasaLibEncryptionType = "SasaAuth2.1";

        public string ArcSuiteDMS_c = @"ArcSuiteデータベースサーバー名";
        /// <summary>
        /// ArcSuiteデータベースサーバー名
        /// </summary>
        public string ArcSuiteDMS = "ASS1";

        public string ArcSuiteSDKuserName_c = @"ArcSuiteドキュメント登録に使用するユーザー名";
        /// <summary>
        /// ArcSuiteドキュメント登録に使用するユーザー名
        /// </summary>
        public string ArcSuiteSDKuserName = "AutoSystemUser";

        public string ArcSuiteSDKuserPasswd_c = @"ArcSuiteSDKに使用するパスワード(暗号化されています)";
        /// <summary>
        /// ArcSuiteSDKに使用するパスワード(暗号化されています)
        /// </summary>
        public string ArcSuiteSDKuserPasswd = "";

        public string ArcSuiteSDKuserPasswd_SasaLibEncryptionType_c = @"ArcSuiteドキュメント登録に使用するパスワード 暗号化規格名";
        /// <summary>
        ///  ArcSuiteドキュメント登録に使用するパスワード 暗号化規格名
        /// </summary>
        public string ArcSuiteSDKuserPasswd_SasaLibEncryptionType = "SasaAuth2.1";


        public string ArcSuiteConfigFolder_c = @"ArcSuiteSDKで使用するスクリプトファイルを保存するフォルダ(サーバー側ローカル)";
        /// <summary>
        ///  ArcSuiteSDKで使用するスクリプトファイルを保存するフォルダ(サーバー側ローカル)
        /// </summary>
        public string ArcSuiteConfigFolder = @"C:\ProgramData\TOYOCOMMON\ArcSuite";

        public string ArcSuiteTicketConfigFile_c = @"ステージサーバデータベースと、ArcSuite登録時に使用するフィールド名の結びつけの設定ファイル";
        /// <summary>
        /// ステージサーバデータベースと、ArcSuite登録時に使用するフィールド名の結びつけの設定ファイル
        /// </summary>
        public string ArcSuiteTicketConfigFile = "ArcSuiteTicketConfig.XML";

        public string ArcSuiteDocumentRegistWorkFolder_c = @"ArcSuiteSDKで使用するコマンドファイルのフォルダ";
        //// <summary>
        /// ArcSuiteSDKで使用するコマンドファイル（登録・操作作業オブジェクト毎に内容がちがうもの）のフォルダ
        /// 送信したTIFFイメージも保存
        /// </summary>
        public string ArcSuiteDocumentRegistWorkFolder = @"D:\STAGING\ARCSUITESEND";

        public string ArcSuiteDocumentAttrGett_DebugMode_c = @"ArcSuiteSDKで データベースから情報取得時にデバッグモードＯＮ";
        /// <summary>
        /// 
        /// </summary>
        public bool ArcSuiteDocumentAttrGett_DebugMode = true;

        public string ArcSuiteDocumentAttrGettWorkFolder_c = @"ArcSuiteSDKで データベースから情報を得るスクリプトのワークフォルダ";
        /// <summary>
        /// ArcSuiteSDKで データベースから情報を得るスクリプトのワークフォルダ
        /// </summary>
        public string ArcSuiteDocumentAttrGettWorkFolder = @"D:\STAGING\ARCSUITEDRGET";


        public string ArcSuiteDocumentAttrMergeWorkFolder_c = @"ArcSuiteSDKで 属性マージスクリプトのワークフォルダ";
        /// <summary>
        /// ArcSuiteSDKで 属性マージスクリプトのワークフォルダ
        /// </summary>
        public string ArcSuiteDocumentAttrMergeWorkFolder = @"D:\STAGING\ARCSUITEDRMERGE";


        public string ArcSuiteGetContentWorkFolder_c = @"ArcSuiteからTIFFﾄﾞｷｭﾒﾝﾄを取得するための作業フォルダ";
        /// <summary>
        /// ArcSuiteからTIFFﾄﾞｷｭﾒﾝﾄを取得するための作業フォルダ
        /// </summary>
        public string ArcSuiteGetContentWorkFolder = @"D:\STAGING\ARCSUITEGETTIFFCONTENT";

        public string ArcSuiteGetContent_DebugMode_c = @"GetContentでのデバッグモード";
        /// <summary>
        /// GetContentでのデバッグモード
        /// </summary>
        public bool ArcSuiteGetContent_DebugMode = true;



        public string PrintersConfigFolder_c = @"印刷関連の設定フォルダ(サーバー側ローカル)";
        /// <summary>
        /// 印刷関連の設定フォルダ(サーバー側ローカル)
        /// </summary>
        public string PrintersConfigFolder = @"C:\ProgramData\TOYOCOMMON\Printers";

        public string PlotSetting_A0_c = @"用紙サイズA0用のプリンタ設定";
        public string PlotSetting_A0P = @"SII Teriostar LP-1030.XML";
        public string PlotSetting_A0L = @"SII Teriostar LP-1030.XML";

        public string PlotSetting_A1_c = @"用紙サイズA1用のプリンタ設定";
        public string PlotSetting_A1P = @"SII Teriostar LP-1030.XML";
        public string PlotSetting_A1L = @"SII Teriostar LP-1030.XML";

        public string PlotSetting_A2_c = @"用紙サイズA2用のプリンタ設定";
        public string PlotSetting_A2P = @"SII Teriostar LP-1030.XML";
        public string PlotSetting_A2L = @"SII Teriostar LP-1030.XML";

        public string PlotSetting_A3_c = @"用紙サイズA3用のプリンタ設定";
        public string PlotSetting_A3P = @"RICOH SPC830M-B.XML";
        public string PlotSetting_A3L = @"RICOH SPC830M-B.XML";

        public string PlotSetting_A4_c = @"用紙サイズA4用のプリンタ設定";
        public string PlotSetting_A4P = @"RICOH SPC830M-B.XML";
        public string PlotSetting_A4L = @"RICOH SPC830M-B.XML";

        public string PlotSetting_unknown_c = @"用紙サイズ不明のプリンタ設定";
        public string PlotSetting_unknown = @"RICOH SPC830M-B.XML";

        public string PrintDebug_BeforePrintingImageSaveFolder_c = @"プリント直前にイメージをファイル保存するフォルダ null のとき何もしない";
        /// <summary>
        /// プリント直前にイメージをファイル保存するフォルダ null のとき何もしない
        /// </summary>
        public string PrintDebug_BeforePrintingImageSaveFolder = "";


        public string WatchDocTimeSec_c = @"生存監視タスクのリピート間隔(秒)。";
        /// <summary>
        /// 生存監視タスクのリピート間隔(秒).
        /// </summary>
        public int WatchDocTimeSec = 60;

        public string RecordOfErrEvent_ServerFailerSendRemainingSec_c = @"生存監視タスクのサーバー接続不能エラーをイベントビューアに記録する間隔(sec)";
        /// <summary>
        /// サーバー接続不能エラーをイベントビューアに記録する間隔(sec)
        /// </summary>
        public int RecordOfErrEvent_ServerFailerSendRemainingSec = 180;

        public string EMAILNOTICE_ServerFailerSendRemainingSeccomment = @"生存監視タスクのサーバー接続不能エラーをメールで再送する間隔(sec)";
        /// <summary>
        /// サーバー接続不能エラーをメールで再送する間隔(sec)
        /// </summary>
        public int EMAILNOTICE_ServerFailerSendRemainingSec = 1300;

        public string LocalPrinterWaitJobThreshold_c = @"ローカルプリンター印刷ジョブ合計許容数（この数値を超えたらローカルプリンターの処理が滞っていると判断する）";
        /// <summary>
        /// ローカルプリンター印刷ジョブ合計許容数
        /// </summary>
        public int LocalPrinterWaitJobThreshold = 20;

        public string PrinterSystemErrorReportMailSend_c = @"プリンタ監視システムからのエラーレポートをメールで送信するか否か";
        /// <summary>
        /// プリンタ監視システムからのエラーレポートをメールで送信するか否か
        /// </summary>
        public bool PrinterSystemErrorReportMailSend = true;

        public string WaitingRepeatSecTime_c = "ArcSuite登録最短サイクル(秒)";
        /// <summary>
        /// 
        /// </summary>
        public int WaitingRepeatTimeSec = 60;

        public string ArcSuiteRegistWaitTime_c = @"登録指示後にアークスイートへ登録するまでの通常登録の待機時間(分)";
        /// <summary>
        /// 登録指示後にアークスイートへ登録するまでの待機時間(min)
        /// </summary>
        public int ArcSuiteRegistWaitTimeMinutes = 15;

        public string MaxRegistCount_c = @"登録指示後にArcSuiteへ図面を一度に登録可能な枚数";
        /// <summary>
        /// ArcSuiteへ図面を一度に登録可能な枚数
        /// </summary>
        public int MaxRegistCount = 60;


        public string NumberOfCriticalErrorsBeforeTheProcessIsStopped_DRAWCAPTUREservice_c = @"プロセスが停止するまでのクリティカルエラーの数(DRAWCAPTUREservice)";
        /// <summary>
        /// プロセスが停止するまでのクリティカルエラーの数(DRAWCAPTUREservice)（エラーの都度マイナス１）
        /// </summary>
        public int NumberOfCriticalErrorsBeforeTheProcessIsStopped_DRAWCAPTUREservice = 30;

        public string NumberOfCriticalErrorsBeforeTheProcessIsStopped_DRAWREGISTservice_c = @"プロセスが停止するまでのクリティカルエラーの数(DRAWREGISTservice)";
        /// <summary>
        /// プロセスが停止するまでのクリティカルエラーの数(DRAWREGISTservice)（エラーの都度マイナス１）
        /// </summary>
        public int NumberOfCriticalErrorsBeforeTheProcessIsStopped_DRAWREGISTservice = 30;

        public string ArcSuiteRegistErrCount_c = @"ｱｰｸｽｲｰﾄ登録ｴﾗｰの度にマイナス。ゼロのとき登録ルーチンを止める";
        /// <summary>
        /// ｱｰｸｽｲｰﾄ登録ｴﾗｰの度にマイナス。ゼロのとき登録ルーチンを止める
        /// </summary>
        public int ArcSuiteRegistErrCount = 3;


        public string RecordOfNormaEvents_PrinterStatus_c = @"プリンタ設定ファイル(.XML)の再読み込みに関するイベントビューアへの送出をコントロールするフラグ";
        /// <summary>
        /// 
        /// </summary>
        public bool RecordOfNormaEvents_ReloadPrinters = false;

        public string RecordOfNormaEvents_ReloadPrinters_IntervalSecond_c = "プリンタ設定ファイル(.XML)の再読み込みを行う間隔";
        /// <summary>
        /// 
        /// </summary>
        public int RecordOfNormaEvents_ReloadPrinters_IntervalSecond = 40;

        public string RecordOfNormaEvents_DataBaseSearch_c = @"RemoteServerCommand_DATABASEクラスの各メソッドに関係するイベントを書き込むか否かのフラグ";
        /// <summary>
        /// DataBaseSearch クラスの各メソッドに存在するイベントビューア―への情報送出をコントロールするフラグ
        /// </summary>
        public bool RecordOfNormaEvents_DataBaseSearch = false;

        public string RecordOfNormaEvents_RelatedToMMAP_c = @" MMAP に関係するイベントを書き込むか否かのフラグ";
        /// <summary>
        /// MMAPに関係するイベントを書き込むか否かのフラグ
        /// </summary>
        public bool RecordOfNormaEvents_RelatedToMMAP = false;

        public string RecordOfNormaEvents_RemoteServerCommand_GetFilesAndFileRecv_c = @"リモートサーバーコマンド GetFiles , FileRecv について 正常終了イベントを書き込むか否かのフラグ";
        /// <summary>
        /// リモートサーバーコマンド GetFiles , FileRecv について 正常終了イベントを書き込むか否かのフラグ
        /// </summary>
        public bool RecordOfNormaEvents_RemoteServerCommand_GetFilesAndFileRecv = false;

        public string RecordOfNormaEvents_ArcSuiteSDKdrGet_c = @"  ArcSuiteSDKdrGet に関係するイベントを書き込むか否かのフラグ";
        /// <summary>
        /// ArcSuiteSDKdrGet に関係するイベントを書き込むか否かのフラグ
        /// </summary>
        public bool RecordOfNormaEvents_ArcSuiteSDKdrGet = false;

        public string RecordOfNormaEvents_ArcSuiteSDKdrGetContents_c = @"  ArcSuiteSDKdrGetContents に関係するイベントを書き込むか否かのフラグ";
        /// <summary>
        /// ArcSuiteSDKdrGetContents に関係するイベントを書き込むか否かのフラグ
        /// </summary>
        public bool RecordOfNormaEvents_ArcSuiteSDKdrGetContents = false;

        public string RecordOfNormaEvents_RemoteServerCommand_ArcsuiteSdk_c = @"  RemoteServerCommand_ArcsuiteSdk に関係するイベントを書き込むか否かのフラグ";
        /// <summary>
        /// RemoteServerCommand_ArcsuiteSdk に関係するイベントを書き込むか否かのフラグ
        /// </summary>
        public bool RecordOfNormaEvents_RemoteServerCommand_ArcsuiteSdk = false;

        //

        public string PipeCommand_ConnectionEventLog_WriteIntervalSecond_c = @"PipeCommand_ConnectionEventLog_WriteIntervalSecond 特定のPIPEコマンドの接続ログをイベントログに書き出す間隔秒数";
        /// <summary>
        /// 特定のPIPEコマンドの接続ログをイベントログに書き出す間隔秒数
        /// </summary>
        public int PipeCommand_ConnectionEventLog_WriteIntervalSecond = 3600;

        public string PipeCommand_ConnectionEventLog_HoldSecond_c = @"PipeCommand_ConnectionEventLog_HoldSecond 特定のPIPEコマンドの接続ログを現時刻から保持する秒数";
        /// <summary>
        /// 特定のPIPEコマンドの接続ログを現時刻から保持する秒数
        /// </summary>
        public int PipeCommand_ConnectionEventLog_HoldSecond = 60;

        /// <summary>
        /// 指定値以下に成ったら警告を発生するメモリ残量（ﾒｶﾞﾊﾞｲﾄ）
        /// </summary>
        public int TriggerAlertRemaingMemoryMegaByte = 200;

        /// <summary>
        /// List<ConnectedApprovalCient> CurrentConnectedClients のクライアントの接続時刻から指定秒数経過したものを削除する
        /// </summary>
        [XmlIgnore()] public int List_Client_AcceptCommand_HoldSecond = 1200;

        /// <summary>
        /// List_Client_GetArcSuiteAwaitingRegis 記録レコードから指定時間以上経過したアイテムを削除
        /// </summary>
        [XmlIgnore()] public int List_Client_GetArcSuiteAwaitingRegist_HoldSecond = 30;

        /// <summary>
        /// Client_CommonInputTicketLis記録レコードから指定時間以上経過したアイテムを削除
        /// </summary>
        [XmlIgnore()] public int Client_CommonInputTicketList_HoldSecond = 120;

        /// <summary>
        /// 重要ループで例外を強制発生させるテストフラグ
        /// </summary>
        [XmlIgnore()] public bool ExceptionOccurrenceTestSwitch = false;

        //シリアライズのためにはコンストラクタは必要
        public StageServerConfig() { }
    }
}
