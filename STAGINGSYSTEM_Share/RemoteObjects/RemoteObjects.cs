/// 
/// パイプでオブジェクト送受信するための器を作成
///
using System;
using ToyoMcMfg.Staging.DataBaseConfig;

// ネームスペースは変更しないこと。シリアル化・逆シリアル化に影響あり
namespace ToyoMcMfg.Staging.RemoteObjects
{
    /// <summary>
    /// パイプにて送受信される承認情報をカプセル化するクラス
    /// </summary>
    [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    public class ApprovedStatus
    {
        /// <summary>
        /// データベースからの検索結果が1件の場合Trueとする
        /// </summary>
        public bool DataBaseGetSucess { get; set; }

        /// <summary>
        ///  GUIDBASE64
        /// </summary>
        public string GUIDBASE64 { get; set; }

        /// <summary>
        /// PARTNUMBER
        /// </summary>
        public string PARTNUMBER { get; set; }

        /// <summary>
        /// チケットコード。バーコードとして印刷されるコードです
        /// </summary>
        public string TICKETCODE { get; set; }

        /// <summary>
        /// 製図担当者押印可能ならtrue
        /// </summary>
        public bool AuthorSignable { get; set; }

        /// <summary>
        /// 設計者押印可能ならtrue
        /// </summary>
        public bool DesignerSignable { get; set; }

        /// <summary>
        /// 最終承認者押印可能ならtrue
        /// </summary>
        public bool ApprovedSignable { get; set; }

        /// <summary>
        /// 製図者名
        /// </summary>
        public string AUTHOR { get; set; }

        /// <summary>
        /// 製図日
        /// </summary>
        public string AUTHORDATE { get; set; }

        /// <summary>
        /// 設計者名
        /// </summary>
        public string DESIGNER { get; set; }

        /// <summary>
        /// 設計日
        /// </summary>
        public string CHECKDATE { get; set; }

        /// <summary>
        /// 最終承認者名
        /// </summary>
        public string APPROVEDUSER { get; set; }

        /// <summary>
        /// 承認日
        /// </summary>
        public string APPROVEDDATE { get; set; }

        /// <summary>
        /// ArcSuite登録を指示したユーザー
        /// </summary>
        public string REGISTEDUSER { get; set; }

        /// <summary>
        /// ArcSuite登録を指示した日時
        /// </summary>
        public string REGISTEDTIME { get; set; }

        /// <summary>
        /// Arcsuiteに登録した際のオブジェクトID
        /// </summary>
        public string ARCSUITEID { get; set; }

        /// <summary>
        /// 承認作業の成功状態
        /// </summary>
        public bool ApprovedSucess { get; set; }

        /// <summary>
        /// 承認作業で返されたメッセージ
        /// </summary>
        public string ApprovedMessage { get; set; }

        /// <summary>
        /// コンストラクタ（何もしない）
        /// </summary>
        public ApprovedStatus() { }

        /// <summary>
        /// コンストラクタ（別のオブジェクトからメンバーをコピー）
        /// </summary>
        /// <param name="a"></param>
        public ApprovedStatus(ApprovedStatus a)
        {
            APPROVEDDATE = a.APPROVEDDATE;
            ApprovedMessage = a.ApprovedMessage;
            ApprovedSignable = a.ApprovedSignable;
            ApprovedSucess = a.ApprovedSucess;
            APPROVEDUSER = a.APPROVEDUSER;
            ARCSUITEID = a.ARCSUITEID;
            AUTHOR = a.AUTHOR;
            AUTHORDATE = a.AUTHORDATE;
            AuthorSignable = a.AuthorSignable;

            CHECKDATE = a.CHECKDATE;
            DataBaseGetSucess = a.DataBaseGetSucess;

            DESIGNER = a.DESIGNER;
            DesignerSignable = a.DesignerSignable;
            GUIDBASE64 = a.GUIDBASE64;
            PARTNUMBER = a.PARTNUMBER;
            REGISTEDTIME = a.REGISTEDTIME;
            REGISTEDUSER = a.REGISTEDUSER;
            TICKETCODE = a.TICKETCODE;
        }
    }

    /// <summary>
    /// 押印取り消しを行う対象と、押印種別をカプセル化するクラス
    /// </summary>
    [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    public class ApprovedCancel
    {
        public FieldValueSet FieldValueSet { get; set; }
        /// <summary>
        /// 製図担当者 をキャンセル対象とするか
        /// </summary>
        public bool AUTHOR { get; set; }

        /// <summary>
        /// 設計者 をキャンセル対象とするか
        /// </summary>
        public bool DESIGNER { get; set; }

        /// <summary>
        /// 承認者 をキャンセル対象とするか
        /// </summary>
        public bool APPROVED { get; set; }

        /// <summary>
        /// 最終押印を実行した実行ユーザー をキャンセル対象とするか
        /// </summary>
        public bool APPROVEDPCUSER { get; set; }

        /// <summary>
        /// 最終押印を実行した実行ホスト をキャンセル対象とするか
        /// </summary>
        public bool APPROVEDHOST { get; set; }
    }


}
