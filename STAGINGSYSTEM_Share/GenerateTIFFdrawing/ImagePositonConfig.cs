
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using SasaLib;
using SasaLib.PrintConfig;
[SupportedOSPlatform("windows")]

/// <summary>
/// TIFF書き出し設定(標準用紙サイズに対するオフセット値)を保持するクラス
/// </summary>
[Serializable]
public class ImagePositonConfig
{
    public static ImagePositonConfig Config { set; get; }


    /// <summary>
    /// この設定ファイルを使用するシステム名
    /// </summary>
    public string SYSTEM;

    /// <summary>
    /// この設定ファイルの書式バージョン
    /// </summary>
    public string VERSION;

    /// <summary>
    /// 
    /// </summary>
    public List<Setting> ImageEffects { get; set; }

    /// <summary>
    /// コンフィグファイルを読み込んだかを確認するフラグ
    /// </summary>
    public bool Already = true;

    #region メソッド


    /// <summary>
    /// List<Offset> Params から CommonPaperSizeを検索し対応するオフセット値を返す
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public Point GetOffset(CommonPaperSize a)
    {
        foreach (Setting b in ImageEffects)
        {
            if (b.CommonPaperSize == a)
            {
                return new Point(b.OffsetX, b.OffsetY);
            }
        }
        return new Point(0, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="CommonPaperSize"></param>
    /// <returns></returns>
    public RotateFlipType GetImageRotation(CommonPaperSize CommonPaperSize)
    {
        foreach (Setting imgeffect in ImageEffects)
        {
            if (imgeffect.CommonPaperSize == CommonPaperSize)
            {
                return imgeffect.ImageRotation;
            }
        }
        return RotateFlipType.RotateNoneFlipNone;
    }

    #endregion

    //シリアライズのためにはコンストラクタは必要
    public ImagePositonConfig() { }
}

[Serializable]
public struct Setting
{
    public CommonPaperSize CommonPaperSize; // 画面表示状態での用紙サイズと向き
    public int OffsetX;    //
    public int OffsetY;    //
    public RotateFlipType ImageRotation; //プロットで初期作成後に回転する角度
}


