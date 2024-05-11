using SasaLib.ArcSuitePreview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCommitLogic
{
    public class ArcSuiteSearchResult
    {
        /// <summary>
        /// コミットしようとした部品番号
        /// </summary>
        public string commitTarget_partnumber;
        
        /// <summary>
        /// コミットしようとした部品番号の部品名・表題・デスクリプション
        /// </summary>
        public string commitTarget_Description;

        /// <summary>
        /// 登録済みかを検索した番号
        /// </summary>
        public List<string> searchTargets;

        /// <summary>
        /// アークスイートの検索結果（検索対象図番の枝番後部文字列を排除後の結果）
        /// </summary>
        public List<ArcsuitePreview> arcSuitePreviews;

        /// <summary>
        /// コミットしようとした図番と同一のものが１件のみヒットした場合の詳細調査結果
        /// </summary>
        public enum DESCRIPTION_ComparResult_MessagetypeEnum
        {
            /// <summary>
            /// 
            /// </summary>
            Null,

            /// <summary>
            /// 
            /// </summary>
            検索結果１件ArcSuiteに同番図面有り表題は合致,

            /// <summary>
            /// 
            /// </summary>
            検索結果１件ArcSuiteに同番図面有り表題は相違,

            /// <summary>
            /// 
            /// </summary>
            検索結果１件ArcSuiteに同番図面有り表題がArcSuite側に無し,

            /// <summary>
            /// 
            /// </summary>
            検索結果１件ArcSuiteに同番図面有り表題がCAD側に無し,

            /// <summary>
            /// 
            /// </summary>
            検索結果１件ArcSuiteに同番図面有り表題がはどちらも無し,

            /// <summary>
            /// 
            /// </summary>
            検索結果複数ArcSuiteに同番図面あり類番図面あり,

            /// <summary>
            /// 
            /// </summary>
            検索結果１件ArcSuiteに同番図面なし類番図面あり,

            /// <summary>
            /// 
            /// </summary>
            検索結果複数ArcSuiteに同番図面なし類番図面があり,


            /// <summary>
            /// 
            /// </summary>
            ArcSuiteに同番図面なし類番図面もなし,

            /// <summary>
            /// 
            /// </summary>
            ArcSuite停止中検査されていない,

            /// <summary>
            /// 
            /// </summary>
            エラー
        }
        public DESCRIPTION_ComparResult_MessagetypeEnum dESCRIPTION_ComparResult_MessagetypeEnum;
    }
}
