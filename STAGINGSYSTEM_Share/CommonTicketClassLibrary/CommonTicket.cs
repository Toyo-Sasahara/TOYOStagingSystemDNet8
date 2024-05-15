///
/// チケットコードを取扱共通クラス
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Versioning;
[SupportedOSPlatform("windows")]

[Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
public class CommonTicket
{
    /// メンバ  --------------------------------------------------------------------

    public string TYPE = "共通チケットフォーマット";

    /// <summary>
    /// フォーマットのバージョン
    /// </summary>
    public double VERSION = 1.4;

    /// <summary>
    /// 書き出しに使ったTOYOACAD2015COMMITTOOL.dllのアセンブリバージョン
    /// </summary>
    public string EXPORTERDLLVER;

    /// <summary>
    /// コミット・バーコード押印せず印刷のみを行うフラグ
    /// </summary>
    public bool PrintOutOnly;

    /// <summary>
    /// GUID情報
    /// </summary>
    public Guid GUID;

    /// <summary>
    /// GUIDをBase64エンコードしたもの
    /// </summary>
    public string GUIDBASE64;

    /// <summary>
    /// この図面をコミットしたユーザー
    /// </summary>
    public string COMMITUSER;

    /// <summary>
    /// この図面をコミットしたＰＣ
    /// </summary>
    public string COMMITHOST;

    /// <summary>
    /// オリジナル作成元（のCAD）
    /// </summary>
    public string CREATESOFTWARE;

    /// <summary>
    /// 元CADが認識していたファイル名
    /// </summary>
    public string DOCUMENTNAME;

    /// <summary>
    /// TIFF書き出し時のタイムスタンプ
    /// </summary>
    public DateTime TIMESTAMP;

    /// <summary>
    /// 印刷レイアウトにおける用紙サイズと向き
    /// </summary>
    public String PLOTPAPERSIZE;

    /// <summary>
    /// CAD上での表示状態における用紙サイズと向き
    /// </summary>
    public String PAPERSIZE;

    /// <summary>
    /// チケットファイル名 GUIDをBase64でエンコードし'/'を'_'に変換
    /// </summary>
    public string TICKETCODE;

    /// <summary>
    /// 印刷希望先("",存在しないものはおまかせとする)
    /// </summary>
    public string REQUESTPRINTER;

    /// <summary>
    /// 印刷実行日時（現時刻以前は即印刷とする）
    /// </summary>
    public DateTime PRINTINGTIME;

    public string COMMENT01 = "<Params></Params>タグ 内のパラメータはサーバー側の準備が出来ていないとエラーとなります";
    public string COMMENT02 = "コミット受付サーバーでは、<Params></Params>タグ 内の<Key></Key>と対応する<Value></Value>の値をデータベースへ取りこみます。";
    public string COMMENT03 = "<Params></Params>タグ 内のその他のタグは無視されます（各CADアドインでのみ使用します）";
    /// <summary>
    /// パラメータのList定義
    /// </summary>
    public List<Param> Params;

    /// <summary>
    /// 表図面の場合 Variantで定義
    /// </summary>
    public List<Param> Variant;


    /// <summary>
    /// 属性パラメータの構造体。すべてstring属性とする
    /// </summary>
    ///     
    [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    public struct Param
    {
        /// <summary>
        /// 共通キー名を格納します。このキーをDBと一致させます.新たに共通キーを追加する場合は事前にデータベース側にその名前の列(タイプはvarchar)を追加しておく必要があります。
        /// 存在しない列名を共通キーで追加した場合は 
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 共通キー名に格納する文字列
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 共通キー名に該当するAUTOCAD属性名
        /// </summary>
        public string Acad_attr { get; set; }
        /// <summary>
        /// 共通キー名に該当するSolidWorksの属性名
        /// </summary>
        public string SW_porp { get; set; }

        /// <summary>
        /// 共通キー名に該当するInventorの属性名
        /// </summary>
        public string Inventor_ipropSet { get; set; }

        /// <summary>
        /// 共通キー名に該当するInventorのiProperty名(ディスプレイ名ではありません)
        /// </summary>
        public string Inventor_iprop { get; set; }

        public List<BlockAttr> SW_BlockAttr { get; set; }

    }

    [Serializable] // パイプでオブジェクトを送受信するため、シリアル化のマークが必要
    public struct BlockAttr
    {
        public string BlockName;
        public string Attr;
    }

    /// メソッド --------------------------------------------------------------------
    /// <summary>
    /// コンストラクタ（シリアライズのために必要）
    /// </summary>
    public CommonTicket()
    { }

    /// <summary>
    /// key名を指定し格納されているValueを読み出し
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetParamKeyValue(string key)
    {
        foreach (Param b in Params)
        {
            if (b.Key == key)
            {
                return b.Value;
            }
        }
        Console.WriteLine($"public string GetParamKeyValue(string key),{key}に対するパラメータはありません");
        return null;
    }

    public void UpdateComonTicketParam(string KEY, string VALUE)
    {
        Param newparam = new Param() { Key = KEY, Value = VALUE };
        if (Params.Where(p => p.Key == KEY).Count() > 0)
            Params.RemoveAll(x => x.Key == KEY);
        Params.Add(newparam);
    }

    /// <summary>
    ///【Use SolidworksTOYOaddin】 key名を指定し、valueをセット。key名が無い場合は新規作成してからvalueをセット
    /// </summary>
    /// <param name="Key"></param>
    /// <param name="Value"></param>
    public void ReplaceOrCreateKeyValue(string Key, string Value)
    {
        try
        {
            Console.WriteLine($"SetKeyValue(...)   {Key}={Value}");
            if (Value == null)
            {
                Value = "";
            }

            // LINQ使用。もしParamsリストのKeyに指定したものがあればTrue
            if (Params.Any(x => x.Key == Key))
            {
                // ある場合は置き換え
                for (int i = 0; i < Params.Count; i++)
                {
                    //
                    if (Params[i].Key == Key)
                    {
                        Param tmpData = Params[i];
                        tmpData.Value = Value;
                        Params[i] = tmpData;
                    }
                }
            }
            else
            {
                // ない場合は追加
                Params.Add(new CommonTicket.Param { Key = Key, Value = Value });
                Console.WriteLine($"SetKeyValue 追加しました   {Key}={Value}");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"ReplaceOrCreateKeyValue(..)例外発生 {ex.Message}");
        }
    }

    /// <summary>
    /// string文字列をsjisに変換し指定したバイト数以内に短くする。
    /// </summary>
    /// <param name="Key"></param>
    /// <param name="numberOfcolumns"></param>
    public void ReplaceOrCretekeyValue(string Key, int sizeOfByte)
    {
        try
        {
            string orgValue = (string)GetParamKeyValue(Key);
            
            if (string.IsNullOrWhiteSpace(orgValue)) return; // Keyの文字列が IsNullOrWhiteSpace なら 何もしない

            System.Text.Encoding sjis = System.Text.Encoding.GetEncoding("shift_jis");　// TODO: Encoding.GetEncoding(932)は .NET Core にて例外が出てしまう
            int orginalSize = sjis.GetByteCount(orgValue);
            if (sizeOfByte < orginalSize)
            {
                byte[] b = sjis.GetBytes(orgValue);
                string result = sjis.GetString(b, 0, sizeOfByte);
                ReplaceOrCreateKeyValue(Key, result);
            }
            else
            {
                return;
            }
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry("CommonTicket", $"CommonTicket.ReplaceOrCretekeyValue({Key}{sizeOfByte}) のエラー{ex.Message}");

            return;
        }
    }

    /// <summary>
    /// ユニバーサル用
    /// key名を指定し格納されているacad_attrを読み出し
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetParamAcadtAttrName(string key)
    {
        foreach (Param b in Params)
        {
            if (b.Key == key)
            {
                return b.Acad_attr;
            }
        }
        return null;
    }

    /// <summary>
    /// AutoCAD用
    /// Params[]オブジェクトからkeyとしてAutoCAD属性名acad_attrを探索し,valueに文字列を保存
    /// 存在しないAutoCAD属性に対しては""とする。
    /// </summary>
    /// <param name="acad_attr">DWGに保存されている属性名 </param>
    /// <param name="value">AutoCAD属性名に対応する文字列</param>
    public void SetAcadAttr(string acad_attr, string value)
    {
        if (value == null)
        {
            value = "";
        }
        for (int i = 0; i < Params.Count; i++)
        {
            // IsMatch(string input, string pattern)
            if (Regex.IsMatch(acad_attr, Params[i].Acad_attr))
            {
                Param tmpData = Params[i];
                tmpData.Value = value;
                Params[i] = tmpData;
            }
        }
    }

    /// <summary>
    /// SolidWorks用
    /// Params[]オブジェクトからkeyとしてSolidworksプロパティ名sw_propを探索し,valueに文字列を保存
    /// 存在しないSolidworksプロパティ名に対しては""とする。
    /// </summary>
    /// <param name="sw_prop">"SolidWorksのカスタムプロパティ名"</param>
    /// <param name="value">"M-10201-010RL"</param>
    public void SetSWprop(string sw_prop, string value)
    {
        if (value == null)
        {
            value = "";
        }

        for (int i = 0; i < Params.Count; i++)
        {
            //
            if (sw_prop == Params[i].SW_porp)
            {
                Console.WriteLine($"SetSwprop(...)   モデルにプロパティ{sw_prop}がみつかりました");
                Console.WriteLine($"SetSwprop(...)   {sw_prop}={value} ");
                Param tmpData = Params[i];
                tmpData.Value = value;
                Params[i] = tmpData;
                return;
            }
        }
        Console.WriteLine($"SetSwprop(...)   {sw_prop}={value} みつかりません");
    }

    /// <summary>
    /// Inventor用
    /// 指定したiPropertyNameをParam[x].Inventor_iprop から探し、該当するParam[x].Valueに文字列をセットする。
    /// </summary>
    /// <param name="iPropertyName">iProperty名（例："図面番号"）</param>
    /// <param name="value">実データ。例:"M-10201-012"</param>
    public void SetINVprop(string iPropertyName, string value)
    {

        if (value == null)
        {
            value = "";
        }

        for (int i = 0; i < Params.Count; i++)
        {
            //
            if (iPropertyName == Params[i].Inventor_iprop)
            {
                Console.WriteLine($"SetINVprop(...)   モデルにプロパティ{iPropertyName}がみつかりました");
                Console.WriteLine($"SetINVprop(...)   {iPropertyName}={value} ");
                Param tmpData = Params[i];
                tmpData.Value = value;
                Params[i] = tmpData;
                return;
            }
        }
        Console.WriteLine($"SetINVprop(...)   {iPropertyName}={value} みつかりません");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iPropertySet"></param>
    /// <param name="iPropertyName"></param>
    /// <param name="value"></param>
    public void SetINVprop(string iPropertySet, string iPropertyName, string value)
    {

        if (value == null)
        {
            value = "";
        }

        for (int i = 0; i < Params.Count; i++)
        {
            //
            if (iPropertySet == Params[i].Inventor_ipropSet)
            {
                if (iPropertyName == Params[i].Inventor_iprop)
                {
                    Console.WriteLine($"SetINVprop(...)   モデルにプロパティ{iPropertyName}がみつかりました");
                    Console.WriteLine($"SetINVprop(...)   {iPropertyName}={value} ");
                    Param tmpData = Params[i];
                    tmpData.Value = value;
                    Params[i] = tmpData;
                    return;
                }
            }
        }
        Console.WriteLine($"SetINVprop(...)   {iPropertyName}={value} みつかりません");
    }

    /// <summary>
    /// 【Use InventorTOYOaddin】指定した共通key名に対応するiProperty名を現在のチケットテンプレートから検索
    /// </summary>
    /// <param name="keyname">検索する共通キー。例: " PARNUMBER"</param>
    /// <returns>対応するiPropertyName、見つからないkeynameの場合nullを返す</returns>
    public string GetINViPropertyName(string keyname)
    {
        for (int i = 0; i < Params.Count; i++)
        {
            //
            if (keyname == Params[i].Key)
            {
                Console.WriteLine($"GetINViPropertyName(...)   {keyname}がみつかりました");
                Console.WriteLine($"GetINViPropertyName(...)   {keyname} = {Params[i].Inventor_iprop}");
                return Params[i].Inventor_iprop;
            }
        }
        return null;
    }

    /// <summary>
    /// 【Use SolidworksTOYOaddin】指定した共通key名に対応するSolidworksでのファイルプロパティ名を現在のチケットテンプレートから検索
    /// </summary>
    /// <param name="keyname"></param>
    /// <returns></returns>
    public string GetSWpropName(string keyname)
    {
        for (int i = 0; i < Params.Count; i++)
        {
            //
            if (keyname == Params[i].Key)
            {
                Console.WriteLine($"GetSWpropName(...)   {keyname}がみつかりました");
                Console.WriteLine($"GetSWpropName(...)   {keyname} = {Params[i].Inventor_iprop}");
                return Params[i].SW_porp;
            }
        }
        return null;
    }

    /// <summary>
    /// 【Use SolidworksTOYOaddin】
    /// </summary>
    /// <param name="keyname"></param>
    /// <returns></returns>
    public List<BlockAttr> GetSWblockAndAttrName(string keyname)
    {
        for (int i = 0; i < Params.Count; i++)
        {
            //
            if (keyname == Params[i].Key)
            {
                Console.WriteLine($"GetSWblockAndAttrName(...)   {keyname}がみつかりました");
                Console.WriteLine($"GetSWblockAndAttrName(...)   {keyname} = {Params[i].Inventor_iprop}");
                return Params[i].SW_BlockAttr;
            }
        }
        return null;
    }

    /// <summary>
    /// チケットファイルで使用する 標準キー名 を CommonTicket.Param オブジェクトから探す。
    /// </summary>
    /// <param name="CommonKeyname">チケットファイルで使用する 標準キー名 </param>
    /// <param name="param"></param>
    /// <returns></returns>
    public bool GetHitParam(string CommonKeyname, ref CommonTicket.Param param)
    {

        for (int i = 0; i < Params.Count; i++)
        {
            //
            if (CommonKeyname == Params[i].Key)
            {
                Console.WriteLine($"GetINViPropertyName(...)   {CommonKeyname}がみつかりました");
                Console.WriteLine($"GetINViPropertyName(...)   {CommonKeyname} = {Params[i].Inventor_iprop}");

                param = Params[i];
                return true;
            }
        }

        return false;
    }

    public static string GetSererTestModeSanitizedPartNumber(string input)
    {
            return "TEST-" + input;
    }


}


