using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyoStageService
{
    /// <summary>
    /// 接続してきたｸﾗｲｱﾝﾄを表す構造体
    /// </summary>
    [Serializable]
    public struct AcceptPipeCommand
    {
        /// <summary>
        /// 接続を受け付けたPIPEサーバーID
        /// </summary>
        public int ServerId;

        /// <summary>
        /// TaskIDを保持
        /// </summary>
        public int TaskID;

        /// <summary>
        /// TaskIDの等価性を保証するオブジェクトＩＤ
        /// </summary>
        public string ObjectID;

        /// <summary>
        /// 現在のリストで最初にコマンドを受け付けた時間（リストに初めて追加されたときの時間）
        /// </summary>
        public DateTime Command_Accept_AddFirst_DateTime;

        /// <summary>
        /// PIPEサーバーコマンドを受け付けた時間
        /// </summary>
        public DateTime Command_Accept_DateTime;

        /// <summary>
        /// 接続してきたクライアントホスト名
        /// </summary>
        public string ClientHost;

        /// <summary>
        /// 接続してきたクライアントユーザー名
        /// </summary>
        public string ClientUser;

        /// <summary>
        /// 接続時のコマンド名
        /// </summary>
        public string CommandName;

        /// <summary>
        /// 接続時の東陽従業員ID(あれば)
        /// </summary>
        public string toyoUSERID;

        /// <summary>
        /// 従業員IDに対応するフルネーム（IDがあれば）
        /// </summary>
        public string toyoFULLNAME;
    }
}
