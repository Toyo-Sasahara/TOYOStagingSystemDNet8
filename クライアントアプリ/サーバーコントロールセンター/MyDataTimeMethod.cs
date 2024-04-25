using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MyDataTimeMethod
{
    /// <summary>
    /// 日付文字列をDateTime型に変換しその後文字列形式へ変換
    /// 
    /// </summary>
    /// <param name="source">入力される日付文字列例："2019-07-10 00:55:03.2950125"</param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string ConvertDateTimeStr(string source, string format = "yyyy-MM-dd HH:mm:ss.fffffff")
    {
        DateTime dt;
        if (DateTime.TryParseExact(source, format, null, DateTimeStyles.AssumeLocal, out dt))
        {
            return ($"{dt:F} ({dt.Kind})");
        }
        return "";
    }

    /// <summary>
    /// 現在時刻よりＸ日前の時刻を得ます。
    /// </summary>
    /// <param name="PreviousDay"></param>
    /// <returns></returns>
    public static string GetOldDateString(int PreviousDay)
    {
        // 現在の日時を取得します
        DateTime dateTimeNow = System.DateTime.Now;

        // 時間間隔を作成します
        TimeSpan timeSpan = new TimeSpan(PreviousDay, 0, 0, 0);

        // 現在時刻よりtimeSpan前の時刻を得ます。
        DateTime PreviousDateTime = dateTimeNow - timeSpan;

        return PreviousDateTime.ToString("yyyy-MM-dd");
    }


    public static string GetOldDateString2(int PreviousDay)
    {
        // 現在の日時を取得します
        DateTime dateTimeNow = System.DateTime.Now;

        // 時間間隔を作成します
        TimeSpan timeSpan = new TimeSpan(PreviousDay, 0, 0, 0);

        // 現在時刻よりtimeSpan前の時刻を得ます。
        DateTime PreviousDateTime = dateTimeNow - timeSpan;

        return PreviousDateTime.ToString("yyyy/MM/dd");
    }


    /// <summary>
    /// 現在時刻よりＸ日前の時刻を得ます
    /// </summary>
    /// <param name="PreviousDay"></param>
    /// <returns></returns>
    public static DateTime GetOldDate(int PreviousDay)
    {
        // 現在の日時を取得します
        DateTime dateTimeNow = System.DateTime.Now;

        // 時間間隔を作成します
        TimeSpan timeSpan = new TimeSpan(PreviousDay, 0, 0, 0);

        // 現在時刻よりtimeSpan前の時刻を得ます。
        DateTime PreviousDateTime = dateTimeNow - timeSpan;

        return PreviousDateTime;
    }
}
