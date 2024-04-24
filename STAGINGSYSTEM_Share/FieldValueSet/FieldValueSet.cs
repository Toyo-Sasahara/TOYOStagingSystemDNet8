using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ネームスペースは変更しないこと。シリアル化・逆シリアル化に影響あり
namespace ToyoMcMfg.Staging.DataBaseConfig
{
    /// <summary>
    ///■【2019-06-29】 SqlServerデータベースのフィールド名と値、フィールドの型を構造体としてListに格納できるクラス
    /// 主に検索結果を返すのに使う
    /// </summary>
    [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    public class FieldValueSet : IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        public string Message;
        /// <summary>
        /// 成功・失敗を保持
        /// </summary>
        public bool Sucess;

        /// <summary>
        /// SQLServerのデータのための、Key(フィールド名) Value 値 SqlDBType 型 をカプセル化したParam構造体
        /// </summary>
        [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマーク必要
        public struct Param
        {
            public string Field { get; set; }
            public object Value { get; set; }
            public SqlDbType SqlDBType { get; set; }
        }

        /// <summary>
        /// Param構造体をリスト化したもの
        /// </summary>
        public List<Param> Params = new List<Param>();

        /// <summary>
        /// keyに最初に合致する
        /// </summary>
        /// <param name="key">検索するkey名</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough]
        public string SearchKey(string key)
        {
            try
            {
                Param xx = Params.Find(a => a.Field == key);
                
                    return xx.Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                SasaLib.Eventlog.Log.WriteEntry("ToyoFieldValueSet", EventLogEntryType.Error, 9700, $"FieldValueSetクラス　SearchKey({key})で例外 {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// Keyに最初に合致するParam構造体を得る。
        /// </summary>
        /// <param name="Key"></param>
        /// <returns>Param型の戻り値</returns>
        public Param GetObjectFromKey(string FieldName)
        {
            var xx = Params.Find(a => a.Field == FieldName);
            return xx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Params).GetEnumerator();
        }
    }

    /// <summary>
    /// SqlServerデータベースのフィールド名と値、フィールドの型を構造体としてパック
    /// </summary>
    [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    public struct SqlFieldValue
    {
        /// <summary>
        /// フィールド
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 値
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// データタイプ
        /// </summary>
        public SqlDbType SqlDBType { get; set; }
    }

    [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    public struct SqlSearchStringValue
    {
        /// <summary>
        /// DBのフィールド（カラム）名　例:"PARTNUMBER"
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 演算子 例：　"LIKE" "=" "IS NULL" 
        /// </summary>
        public string Ooperator { get; set; }
        public string Value { get; set; }
        public string Logic { get; set; }
    }

    [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    public struct SqlSearchValue
    {
        /// <summary>
        /// DBのフィールド（カラム）名　例:"PARTNUMBER"
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 演算子 例：　"LIKE" "=" "IS NULL" 
        /// </summary>
        public string Ooperator { get; set; }

        public object Value { get; set; }
        /// <summary>
        /// データタイプ
        /// </summary>
        public SqlDbType SqlDBType { get; set; }
        public string Logic { get; set; }
    }

}
