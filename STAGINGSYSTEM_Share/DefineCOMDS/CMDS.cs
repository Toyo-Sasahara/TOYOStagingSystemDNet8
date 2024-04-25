using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace STAGINGSYSTEM_COMMANDS
{
    public static class CMDS
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ConnectKeyword = @"接続可能バージョン2018-11-21";

        /// <summary>
        /// ■
        /// </summary>
        public const string DR_Status_SERVER_ALIVE = "SERVER ALIVE";

        /// <summary>
        /// ■ PIPEへのストレステスト
        /// </summary>
        public const string DC_DR_StressTest = "StressTest";

        /// <summary>
        /// ■ToyoDRAWCAPTUIREservice PIPEへの接続テスト
        /// </summary>
        public const string DC_DR_SW_ConnectTest = "ConnectTest";

        /// <summary>
        /// ■サービスのバージョンを返す
        /// </summary>
        public const string DC_DR_SW_GetVersion = "GetVersion";

        /// <summary>
        /// ■StageServerConfig.Config のパブリック変数を外部からリモート取得
        /// </summary>
        public const string DC_DR_SW_GetValue = "GetValue";

        /// <summary>
        /// ■StageServerConfig.Config のパブリック変数を外部からリモート設定
        /// </summary>
        public const string DC_DR_SW_SetValue = "SetValue";

        /// <summary>
        /// ■StageServerConfig.Config のパブリック変数を全表示
        /// </summary>
        public const string DC_DR_SW_GetVauleList = "GetVauleList";

        /// <summary>
        /// ■コミット受付可能かを調べる
        /// </summary>
        public const string DC_CommitRecepitonState = "CommitRecepitonState";

        /// <summary>
        /// □外部から見たコミットフォルダを返します。
        /// </summary>
        public const string DC_GetCOMMITFolder = "GetCOMMITFolder";

        /// <summary>
        /// ■コミット先としてホストしているプリンタシステム名を返す(XMLプリンタ設定ファイルでのプリンタ名とエイリアス名のList<keyValuePare<string,string>>)を返す
        /// </summary>
        public const string DC_GetCommitPrinterInfo = "GetCommitPrinterInfo";

        /// <summary>
        /// ■コミット先としてホストしているプリンタシステム名を返す
        /// (XMLプリンタ設定ファイルでのプリンタ名とAlias名のList<keyValuePare<string,string>>)を返す
        /// </summary>
        public const string DC_GetCommitPrinterNameAndAlias = "GetCommitPrinterNameAndAlias";

        /// <summary>
        /// ■コミット先としてホストしているプリンタシステム名を返す
        /// (XMLプリンタ設定ファイルでのプリンタ名とショートカット名のList<keyValuePare<string,string>>)を返す
        /// </summary>
        public const string DC_GetCommitPrinterShortCutName = "GetCommitPrinterShortCutName";

        /// <summary>
        /// □コミット先としてホストしているプリンタシステム名を返す
        /// (XMLプリンタ設定ファイルでのプリンタショートカット名とDescriptionをList<keyValuePare<string,string>>)で返す
        /// </summary>
        public const string DC_GetCommitPrinterShortCutNameAndDescription = "GetCommitPrinterShortCutNameAndDescription";

        /// <summary>
        /// ■コミット先としてホストしているプリンタシステムの故障有無を返す
        /// (プリンタ名とDescriptionをList<keyValuePare<string,string>>)で返す
        /// </summary>
        public const string DC_GetCommitPrinterIsFailStatus = "GetCommitPrinterIsFailStatus";

        /// <summary>
        /// ■このサーバーでの、用紙サイズ別プリンタ設定ファイル
        /// </summary>
        public const string DC_GetCommitPrinterSettingFromPaperSize = "GetCommitPrinterSettingFromPaperSize";

        /// <summary>
        /// ■指定したテキストファイルを受信します。
        /// </summary>
        public const string DC_LoadTextFile = "LoadTextFile";

        /// <summary>
        /// ■メール送信・ログレベルのセット
        /// </summary>
        public const string DC_MAILSEND_LOGLEVEL_SET = "MAILSEND_LOGLEVEL_SET";

        /// <summary>
        /// ■指定された図面番号から、図面種類を返す
        /// </summary>
        public const string DC_CHECK_DRAWING_TYPE = "CHECK_DRAWING_TYPE";

        /// <summary>
        /// ■承認処理受付可能かを調べる
        /// </summary>
        public const string DR_ApprovalRecepitonState = "ApprovalRecepitonState";

        /// <summary>
        /// ■データベースを検索。検索条件を１つのフィールドとフィールド内容にて指定
        /// </summary>
        public const string DR_DataBaseSearch = "DataBaseSearch";

        /// <summary>
        /// ■テージサーバーデータベースを検索。検索条件を　SqlSearchStringValue 形式で１つ指定
        /// </summary>
        public const string DR_DataBaseSearch3 = "DataBaseSearch3";

        /// <summary>
        /// ■(未使用・削除候補)ステージサーバデータベースを検索。検索条件を List<SqlSearchStringValue> 形式で複数指定
        /// </summary>
        public const string DR_DataBaseSearch4 = "DataBaseSearch4";

        /// <summary>
        /// ■ステージサーバデータベースを検索。検索条件を List<SqlSearchStringValue> 形式で複数指定。.ORDER BY対応
        /// </summary>
        public const string DR_DataBaseSearch4a = "DataBaseSearch4a";

        /// <summary>
        /// ■接続中のクライアントから入力されたチケットコードを押印候補バッファに蓄える。同じ図面番号がある場合は情報を返す
        /// </summary>
        public const string DR_AddClientPreInputTICKETCODE = "AddClientPreInputTICKETCODE";

        /// <summary>
        /// □チケットコードが押印候補バッファに既存かをチェックする
        /// </summary>
        public const string DR_CheckClientPreInputTICKETCODES = "CheckClientPreInputTICKETCODES";

        /// <summary>
        /// ■チケットコードを押印候補バッファから削除する
        /// </summary>
        public const string DR_RemovePreInputTIKECTCODEs = "RemovePreInputTIKECTCODEs";

        /// <summary>
        /// □共通押印候補バッファから指定したオブジェクト(List<ClientInputTICKET>型)を削除()
        /// </summary>
        public const string DR_RemovePreInputClientInputTICKETs = "RemovePreInputClientInputTICKETs";

        /// <summary>
        /// ■サーバーが保持している各クライアントから押印候補リスト最新版を返す
        /// </summary>
        public const string DR_GetCommonApprovalWaitingTicketList = "GetCommonApprovalWaitingTicketList";

        /// <summary>
        /// ■ユーザー別の登録待ちリストを返す(承認クライアント用。検索条件を List<SqlSearchStringValue> 形式で複数指定。.ORDER BY対応
        /// </summary>
        public const string DR_GetArcSuiteAwaitingRegist = "GetArcSuiteAwaitingRegist";

        /// <summary>
        /// ■ステージサーバーデータベースを検索（NULL値を検索）
        /// </summary>
        public const string DR_DataBaseSearchNull = "DataBaseSearchNull";

        /// <summary>
        /// ■ステージサーバーデータベースからレコードと実体ファイルを削除
        /// </summary>
        public const string DR_DataBaseUpdate = "DataBaseUpdate";

        /// <summary>
        /// ■イメージ全体を読み出し（イメージをPIPEで送出）
        /// </summary>
        public const string DR_RecordAndEntityfileDelete = "RecordAndEntityfileDelete";

        /// <summary>
        ///■イメージ全体を読み出し（イメージをPIPEで送出）
        /// </summary>
        public const string DR_LoadImage = "LoadImage";

        /// <summary>
        /// □クリッピングしてImage読込（イメージをPIPEで送出）
        /// </summary>
        public const string DR_LoadClipedImage = "LoadClipedImage";

        /// <summary>
        /// ■クリッピングしてImage読込（イメージをPIPEで送出）
        /// </summary>
        public const string DR_LoadImage2 = "LoadImage2";

        /// <summary>
        /// □承認・押印ロジックスタート(押印のみ、アークスイートへは登録せず) ステージサーバーDB更新も行う
        /// </summary>
        public const string DR_Approved2b = "Approved2b";

        /// <summary>
        /// ■承認・押印ロジックスタート(押印のみ、アークスイートへは登録せず) ステージサーバーDB更新も行う(押印日時指定可能)
        /// </summary>
        public const string DR_Approved2c = "Approved2c";

        /// <summary>
        /// ■押印クリア・データベース更新（GUIDBASE64コードのList形式にて指定）　2022/08/24 時点  クライアントから呼び出しを確認 RMCmaintenance.ApprovedCancel(..)からコマンドよびだし
        /// </summary>
        public const string DR_ApprovedCancels2 = "ApprovedCancels2";

        /// <summary>
        /// ■押印クリア・データベース更新(ApprovedCancelオブジェクトのList形式にて指定)(Busyチェック付き) 2022/08/24 時点 クライアントから呼び出しを確認 RMCdataBase.ApprovedCancels3(..)からコマンドよびだし
        /// </summary>
        public const string DR_ApprovedCancels3 = "ApprovedCancels3";

        /// <summary>
        /// ■登録予定のレコードのフラグを立てる
        /// </summary>
        public const string DR_SetRegistWaitingFlag = "SetRegistWaitingFlag";

        /// <summary>
        /// ■ アークスイートから属性検索
        /// </summary>
        public const string DR_GetArcSuiteAtrtribute = "GetArcSuiteAtrtribute";

        /// <summary>
        /// ■ アークスイートから属性検索
        /// </summary>
        public const string DR_GetArcSuiteAtrtribute1 = "GetArcSuiteAtrtribute1";

        /// <summary>
        /// ■ アークスイートのユーザー属性値ZUBANのList<string> コレクションを受け取って１つづ検索し属性をArrayList形式で返す
        /// </summary>
        public const string DR_GetArcSuiteAtrtribute2 = "GetArcSuiteAtrtribute2";

        /// <summary>
        /// ■ アークスイートのユーザー属性値ZUBANのList<string> コレクション、キャビネットＩＤ，受け取って１つづ検索し属性をArrayList形式で返す
        /// </summary>
        public const string DR_GetArcSuiteAtrtribute3 = "GetArcSuiteAtrtribute3";

        /// <summary>
        /// ■ アークスイートのオブジェクトIDを１つ受け取り属性検索
        /// </summary>
        public const string DR_GetArcSuiteAtrtributeFromObjectID = "GetArcSuiteAtrtributeFromObjectID";

        /// <summary>
        /// ■ アークスイートのオブジェクトIDをリストでつ受け取り属性検索
        /// </summary>
        public const string DR_GetArcSuiteAttributeFromOBJECTIDList = "GetArcSuiteAttributeFromOBJECTIDList";

        /// <summary>
        /// ■ 検索属性のリストからコンテンツを一括でとりだし指定したフォルダ（サーバーから見えるフォルダ）に書き出す。
        /// </summary>
        public const string DR_GetArcSuiteContents = "GetArcSuiteContents";

        /// <summary>
        /// ■ クライアントから検索する図面番号を１つ受け取りアークスイート最新を検索。結果をImageで返す
        /// </summary>
        public const string DR_GetArcSuiteLatestDrawing = "GetArcSuiteLatestDrawing";

        /// <summary>
        /// ■ クライアントから検索する図面番号を１つ受け取りアークスイート最新を検索。結果をコンテントﾌｧｲﾙで返す
        /// </summary>
        public const string DR_GetArcSuiteLatestDrawingFile = "GetArcSuiteLatestDrawingFile";

        /// <summary>
        /// ■ クライアントから検索する図面番号のリストを受取りアークスイート最新を検索。結果をコンテントﾌｧｲﾙで返す
        /// </summary>
        public const string DR_GetArcSuiteLatestDrawingFiles = "GetArcSuiteLatestDrawingFiles";

        /// <summary>
        /// ■ 図番コレクションと変更する属性名と属性値、変更ユーザーとパスワードを元に一括属性変更
        /// </summary>
        public const string DR_MergeArcSuiteAtrtribute = "MergeArcSuiteAtrtribute";

        /// <summary>
        /// ■ 図番と変更する属性名と属性値、変更ユーザーとパスワードを元に属性変更
        /// </summary>
        public const string DR_MergeArcSuiteAtrtribute1 = "MergeArcSuiteAtrtribute1";

        /// <summary>
        /// ■CADTYPEを変更
        /// </summary>
        public const string DR_SetUnSetArcSuiteCADTYPEattribute = "SetUnSetArcSuiteCADTYPEattribute";

        /// <summary>
        /// ■押印のみ実行
        /// </summary>
        public const string DR_StampingGo = "StampingGo";

        /// <summary>
        ///  ■印刷実行
        /// </summary>
        public const string DR_PrintStart = "PrintStart";

        /// <summary>
        /// ■ユーザー検索
        /// </summary>
        public const string DR_UserSearch = "UserSearch";

        /// <summary>
        /// ■現在の接続しているクライアントを表す構造体を得る
        /// </summary>
        public const string DR_GetAuthorizedUser = "GetAuthorizedUser";

        /// <summary>
        ///  ■SetMemMapdFile
        /// </summary>
        public const string SW_SetMemMapdFile = "SetMemMapdFile";

        /// <summary>
        /// ■GetMemMapdFile
        /// </summary>
        public const string SW_GetMemMapdFile = "GetMemMapdFile";

        /// <summary>
        /// ■GetAvailableMemory
        /// </summary>
        public const string SW_GetAvailableMemory = "GetAvailableMemory";

        /// <summary>
        /// ■GetMemoryUsageWorkingSetSize
        /// </summary>
        public const string SW_GetMemoryUsageWorkingSetSize = "GetMemoryUsageWorkingSetSize";


        /// <summary>
        ///  ■ShowPrinterQueue
        /// </summary>
        public const string SW_ShowPrinterQueue = "ShowPrinterQueue";

        /// <summary>
        /// □不明のコマンド・サーバー側設定なし
        /// </summary>
        public const string SW_UnknownCommand = "TitleFieldTest";

        /// <summary>
        /// ■サーバーが保持している DRAWCAPTUREパイプサービスへの クライアントの接続記録を文字列で得る
        /// </summary>
        public const string DC_DR_SW_GetPipeCommandLog = "GetPipeCommandLog";

        /// <summary>
        /// ■コマンド実行中のセッションコマンドリストを得ます
        /// </summary>
        public const string DC_DR_SW_GetActiveSessionCommandList = "GetActiveSessionCommandList";

        #region ServerControlコマンド＆サブコマンド

        /// <summary>
        /// ■ サーバーコントロール・サブコマンド必要
        /// </summary>
        public const string DC_DR_SW_ServerControl = "ServerControl";

        /// <summary>
        /// ■サーバーコントロール>ログレベル取得
        /// </summary>
        public const string DC_DR_SW_SS_ServerControl_CONSOLE_LOGLEVEL_GET = "CONSOLE_LOGLEVEL_GET";

        /// <summary>
        /// ■サーバーコントロール>ログレベルセット
        /// </summary>
        public const string DC_DR_SW_SS_ServerCOntorl_CONSOLE_LOGLEVEL_SET = "CONSOLE_LOGLEVEL_SET";

        /// <summary>
        /// ■サーバーコントロール>バージョン取得
        /// </summary>
        public const string DC_DR_SW_SS_ServerCOntorl_GetServerVersion = "GetServerVersion";

        /// <summary>
        /// ■サーバーコントロール>SyncConfigサーバー SyncSystemConfig.XML 強制保存
        /// </summary>
        public const string SS_ServerControl_SAVE_SyncSystemConfig = "SAVE_SyncSystemConfig";

        /// <summary>
        /// ■サーバーコントロール>SyncConfigサーバー SyncSystemConfig.XML 再読み込み
        /// </summary>
        public const string SS_ServerControl_RELOAD_SyncSystemConfig = "RELOAD_SyncSystemConfig";

        /// <summary>
        /// ■コミット受付不可能メッセージを書き換える
        /// </summary>        
        public const string DR_ServerContorl_SET_COMMITACCEPTFALSEMSG = "SET_COMMITACCEPTFALSEMSG";

        /// <summary>
        /// ■コミット受付不可能メッセージを取得する
        /// </summary>
        public const string DR_ServerContorl_GET_COMMITACCEPTFALSEMSG = "GET_COMMITACCEPTFALSEMSG";

        /// <summary>
        /// ■承認不可能メッセージを書き換える
        /// </summary>
        public const string DR_ServerContorl_SET_APPROVINGACCEPTFALSEMSG = "SET_APPROVINGACCEPTFALSEMSG";

        /// <summary>
        /// ■承認不可能メッセージを取得する
        /// </summary>
        public const string DR_ServerContorl_GET_APPROVINGACCEPTFALSEMSG = "GET_APPROVINGACCEPTFALSEMSG";

        /// <summary>
        /// ■ｸﾗｲｱﾝﾄからｻｰﾊﾞｰへファイルを送る
        /// </summary>
        public const string DC_ServerControl_FileSend = "FileSend";

        /// <summary>
        /// ■サーバーコントロール>DCサーバーからクライアントへファイル受信
        /// </summary>
        public const string DC_ServerControl_FileRecv = "FileRecv";

        /// <summary>
        /// ■サーバーコントロール>DCｻｰﾊﾞｰからｸﾗｲｱﾝﾄへファイル一覧を送る
        /// </summary>
        public const string DC_ServerControl_GetFileList = "GetFileList";

        /// <summary>
        /// ■サーバーコントロール>ｸﾗｲｱﾝﾄからDRｻｰﾊﾞｰへファイルを送る
        /// </summary>
        public const string DR_ServerControl_FILE_SEND = "FILE_SEND";

        /// <summary>
        /// ■サーバーコントロール>
        /// </summary>
        public const string DC_ServerControl_RELOAD_STAGESERVERCONFIG = "RELOAD_STAGESERVERCONFIG";

        /// <summary>
        /// ■サーバーコントロール>
        /// </summary>
        public const string DC_ServerControl_RELOAD_STAMPCONF = "RELOAD_STAMPCONF";

        /// <summary>
        /// ■サーバーコントロール>
        /// </summary>
        public const string DC_ServerContorl_RELOAD_BARCODECONF = "RELOAD_BARCODECONF";

        /// <summary>
        /// ■サーバーコントロール>
        /// </summary>
        public const string DC_ServerControl_DATABASE_BACKUP = "DATABASE_BACKUP";

        /// <summary>
        /// ■サーバーコントロール>
        /// </summary>
        public const string DC_ServerControl_DATABASE_RESTORE = "DATABASE_RESTORE";

        /// <summary>
        /// ■サーバーコントロール>
        /// </summary>
        public const string DC_DR_SS_ServerControl_RELOAD_STAGESERVERDATABASECONFIG = "RELOAD_STAGESERVERDATABASECONFIG";

        /// <summary>
        /// ■サーバーコントロール>
        /// </summary>
        public const string DC_DR_SW_ServerControl_SAVE_STAGESERVERCONFIG = "SAVE_STAGESERVERCONFIG";

        /// <summary>
        /// ■サーバーコントロール>
        /// </summary>
        public const string DC_SW_ServerContorol_SAVE_STAGESERVERDATABASECONFIG = "SAVE_STAGESERVERDATABASECONFIG";

        #endregion ServerControlコマンド＆サブコマンド

        #region Statusコマンド＆サブコマンド

        /// <summary>
        /// ■ステータスコマンド・サブコマンド必要
        /// </summary>
        public const string DC_DR_Status = "Status";

        /// <summary>
        /// ■Master or Slave
        /// </summary>
        public const string DC_Status_CheckSERVERMODE = "CheckSERVERMODE";

        /// <summary>
        /// ■現在のサーバーモードを得る
        /// </summary>
        public const string DC_DR_Status_CURRENT_MODE = "CURRENT MODE";

        /// <summary>
        /// ■データベースサーバーのホスト名を返す
        /// </summary>
        public const string DR_Status_DATABASE_HOSTNAME = "DATABASE HOSTNAME";

        /// <summary>
        /// ■データベース名を返す
        /// </summary>
        public const string DR_Status_DATABASE_DATABASENAME = "DATABASE DATABASENAME";

        /// <summary>
        /// ■データベースを制御するユーザー名を返す
        /// </summary>
        public const string DR_Status_DATABASE_CONTROLUSER = "DATABASE CONTROLUSER";

        /// <summary>
        /// ■コミット先フォルダのUNCを返す
        /// </summary>
        public const string DR_Status_COMMITPATH = "COMMITPATH";

        /// <summary>
        /// ■ファイルストアフォルダのUNCを返す
        /// </summary>
        public const string DR_Status_FILESTOREPATH = "FILESTOREPATH";

        /// <summary>
        /// ■アークスイートのDMSサーバーのホスト名を返す
        /// </summary>
        public const string DR_Status_ARCSUITE_SERVERHOST = "ARCSUITE SERVERHOST";

        /// <summary>
        /// ■アークスイートのためのワークフォルダを返す
        /// </summary>
        public const string DR_Status_ARCSUITE_WORKFOLDER = "ARCSUITE WORKFOLDER";

        /// <summary>
        /// ■接続中PIPE一覧
        /// </summary>
        public const string DC_DR_Status_GET_PIPE_CONNECTION = "GET PIPE CONNECTION";

        #endregion Statusコマンド＆サブコマンド

        #region SystemCheck コマンド&サブコマンド

        /// <summary>
        /// ■システムチェックコマンド・サブコマンド必要
        /// </summary>
        public const string DC_DR_SystemCheck = "SystemCheck";

        /// <summary>
        /// ■ ステージングフォルダ内にあるファイルのチケットコードが、ステージングデータベースに存在するかを確認
        /// </summary>
        public const string DR_SystemCheck_FILESTORE_CHECK = "FILESTORE_CHECK";

        /// <summary>
        /// ■ ステージングフォルダ内にあるファイルのうち、テージングデータベースに存在しないファイルを強制削除
        /// </summary>
        public const string DR_SystemCheck_FILESTORE_REPARE = "FILESTORE_REPARE";

        /// <summary>
        /// ■指定したチケットコードのチケットファイルもしくはイメージファイルが、FILESTORE存在していないリストを作成し返す
        /// </summary>
        public const string DR_SystemCheck_TICKETFILE_EXIST_CHECK = "TICKETFILE_EXIST_CHECK";

        /// <summary>
        /// □ コミットフォルダの残りのチケットファイル個数をカウント
        /// </summary>
        public const string DR_SystemCheck_BEFORE_COMMIT_COUNT = "BEFORE_COMMIT_COUNT";

        /// <summary>
        /// □ ステージングフォルダのチェットファイル個数をカウント
        /// </summary>
        public const string DR_SystemCheck_FILEFOLDER_COUNT = "FILEFOLDER_COUNT";

        /// <summary>
        /// □ ステージングサーバで管理しているプリンタを返す
        /// </summary>
        public const string DR_SystemCheck_ListCommitPrinters = "ListCommitPrinters";

        /// <summary>
        /// ■ 通信テスト
        /// </summary>
        public const string DR_SystemCheck_TESTPIPE = "TESTPIPE";

        #endregion SystemCheck
    }
}

