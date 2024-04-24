using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;

namespace CommonCommitLogic
{

    /// <summary>
    /// コミットツールの固定的な設定を保持するクラス（コミットの都度変化するデータは含まない）
    /// </summary>
    public class CommitParam
    {

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■ //

        /// <summary>
        /// コミット先サーバー名
        /// </summary>
        public string StageServerHost;

        public string ClientDomainName;
        public string ClientUserName;
        public bool ClsLogon;
        public string PipeConnection31Password;
        public string PipeNameDR;
        public string PipeNameDC;
        public int CommitRecepitonStateConnectTimeOut;
        public int CheckCommitRecepitonStateFalseCount;


        /// <summary>
        /// TIFFとチケットファイルを保存する出力先URLフォルダ
        /// </summary>
        public string CommitPath;

        /// <summary>
        /// コミット受け付け共有フォルダ内に残存するファイルの個数がこの値を変えたらコミットさせない
        /// </summary>
        public int CommitQueueThreshold;

        public string PrinterDriverName;

        public string PC3FileName;

        public string CREATESOFTWARE;
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■ //

        /// <summary>
        /// ArcSuite接続に必要なユーザ名
        /// </summary>
        public string ArcSuiteUserName;

        /// <summary>
        /// ArcSuite接続に必要なパスワード（暗号化済み）
        /// </summary>
        public string ArcSuiteCrypt31UserPass;

        /// <summary>
        /// ArcSuiteにWebコマンドにて図面番号を検索するURL
        /// </summary>
        public string ArcSuiteSearchURL;

        /// <summary>
        /// ArcSuiteに図面番号を検索するURL(コンテント同時オープンも含む)
        /// </summary>
        public string ArcSuiteSearchAndContentOpenURL;

        public bool ArcSuiteDrawingDownloadMode;


        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■ //

        /// <summary>
        /// 採番システムサーバーのホスト名またはＩＰアドレス
        /// </summary>
        public string NumberingServerName;
        public string NumberingServerWebAddr;
        public int NumberingServerDbPort;
        public string NumberingServerDB_Username;
        public string NumberingServerDB_Crypt31Password;



        /// <summary>
        /// 部品図に関して採番システムに問い合わせるURL。 {SEARCHNUMBER} が図番に置き換わる
        /// </summary>
        public string NumberingPartDrawingUpdateAddress = "{NUMBERINGWEBADD}/numbering/search.php?keyword={SEARCHNUMBER}&from=index";

        /// <summary>
        /// 組立図に関して採番システムに問い合わせるURL。 {SEARCHNUMBER} が図番に置き換わる
        /// </summary>
        public string NumberingAssyDrawingUpdateAddress = "{NUMBERINGWEBADD}/ksearch.php?keyword={SEARCHNUMBER}&from=index";

        /// <summary>
        /// レイアウト図等技術図書に関して採番システムに問い合わせるURL。 {SEARCHNUMBER} が図番に置き換わる
        /// </summary>
        public string NumberingLayoutDrawingUpdateAddress = "{NUMBERINGWEBADD}/numbering/lsearch.php?keyword={SEARCHNUMBER}";
    }
}
