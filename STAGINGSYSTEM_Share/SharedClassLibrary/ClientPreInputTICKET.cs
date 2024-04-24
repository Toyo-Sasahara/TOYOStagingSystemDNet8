using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyoStageService
{
    /// <summary>
    /// クライアント承認ツールが今保持している押印直前のチケットを集めるクラス
    /// </summary>
    // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    [Serializable]
    public struct ClientPreInputTICKET
    {
        /// <summary>
        /// プリ入力したチケットコード(押印準備段階)
        /// </summary>
        public string PreInputTICKETCODE;

        /// <summary>
        /// チケットコードをプリ入力した日時
        /// </summary>
        public DateTime PreInputTICKETCODE_DateTime;

        /// <summary>
        /// ﾁｹｯﾄｺｰﾄﾞに対応する図面番号
        /// </summary>
        public string DRAWNUMBER;

        /// <summary>
        /// クライアントホスト名
        /// </summary>
        public string ClientHost;

        /// <summary>
        /// クライアントユーザー名
        /// </summary>
        public string ClientUser;

        /// <summary>
        /// 接続時の東陽従業員ID
        /// </summary>
        public string toyoUSERID;

        /// <summary>
        /// 従業員IDに対応するフルネーム
        /// </summary>
        public string toyoFULLNAME;
    }
}
