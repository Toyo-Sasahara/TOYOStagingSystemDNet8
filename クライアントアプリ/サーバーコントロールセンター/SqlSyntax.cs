using ServerControlCenterApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyoMcMfg.Staging.DataBaseConfig;

public static class SqlSyntax
{
    /// <summary>
    /// チケットコードで検索
    /// </summary>
    /// <param name="TICKETCODE"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> TICKETCODE(string TICKETCODE)
    {
        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Field = "TICKETCODE",    Ooperator = "=",   Value=$"'{TICKETCODE}'"},          // TICKETCODE
           };
        return sql;
    }

    /// <summary>
    /// 部品番号で検索
    /// </summary>
    /// <param name="Partnumber"></param>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> PARTNUMBER(string Partnumber, int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Field = "PARTNUMBER",    Ooperator = "LIKE",   Value=$"'{Partnumber}%'"},          // COMMITUSERに{Username} であるもの
                new SqlSearchStringValue{Logic="AND"},
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'" }
           };
        return sql;
    }

    /// <summary>
    /// ArcSuite登録済みを検索
    /// </summary>
    /// <param name="REGISTEDUSER"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> ArcSuiteRegisteredList(int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "ARCSUITEID",Ooperator = "IS", Value="NOT NULL"},
                new SqlSearchStringValue{Logic=")"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "APPROVEDDATE",Ooperator = ">",  Value=$"'{tmestring}%'" }// APPROVEDDATE は nvarchar(50) であるので文字列として扱う
            };
        return sql;
    }

    /// <summary>
    /// 承認ユーザーがnull、かつ ArcSuiteオブジェクトIDがnullでないもの。なおかつREGISTWAITINFFLAGがゼロのもの
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> ARCSUITEIDisNull_And_APPROVEDUSERisNotNull(int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "APPROVEDUSER",Ooperator = "IS",   Value="NOT NULL" , Logic="AND"},
                new SqlSearchStringValue{Field = "ARCSUITEID",Ooperator = "IS", Value="NULL",   Logic="AND"},
                    new SqlSearchStringValue{Logic="("},
                        new SqlSearchStringValue{Field = "REGISTWAITINGFLAG",Ooperator = "IS",   Value="NULL" , Logic="OR"},
                        new SqlSearchStringValue{Field = "REGISTWAITINGFLAG",Ooperator = "=",   Value="0" },
                    new SqlSearchStringValue{Logic=")"},
                new SqlSearchStringValue{Logic=")"},
                new SqlSearchStringValue{Logic="AND"},
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'" }
            };
        return sql;
    }

    /// <summary>
    /// 承認日時がnullではなく、登録時刻がnullかつ、ArcSuite登録待ちが立っていないものを検索
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> APPROVEDDATEisNotNull_And_REGISTEDTIMEisZero_And_REGISTWAITINGFLAGisZero(int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "APPROVEDDATE",Ooperator = "IS",   Value="NOT NULL" , Logic="AND"},
                new SqlSearchStringValue{Field = "REGISTEDTIME",Ooperator = "IS", Value="NULL", Logic="AND"},
                    new SqlSearchStringValue{Logic="("},
                        new SqlSearchStringValue{Field = "REGISTWAITINGFLAG",Ooperator = "IS",   Value="NULL" , Logic="OR"},
                        new SqlSearchStringValue{Field = "REGISTWAITINGFLAG",Ooperator = "=",   Value="0" },
                    new SqlSearchStringValue{Logic=")"},
                new SqlSearchStringValue{Logic=")"},
                new SqlSearchStringValue{Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'" }
            };
        return sql;
    }

    /// <summary>
    /// 設計承認が未達のレコードを検索
    /// </summary>
    /// <returns></returns>
    public static List<SqlSearchStringValue> ApprovableList(int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);
        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "AUTHOR",      Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "DESIGNER",    Ooperator = "IS",   Value="NULL",       Logic="or"},    // 設計者がNULL  または、
                new SqlSearchStringValue{Field = "DESIGNER",    Ooperator = "=",   Value="''",       Logic="AND"},      // 最終承認者がNULLまたは
                new SqlSearchStringValue{Field = "APPROVEDUSER",Ooperator = "IS",   Value="NULL" },    // 最終承認者が''であるものを探す
                new SqlSearchStringValue{Logic=")"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'" }
            };
        return sql;
    }

    /// <summary>
    /// 最終承認可能なレコードを検索
    /// </summary>
    /// <returns></returns>
    public static List<SqlSearchStringValue> ApprovableFinulList(int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "AUTHOR",      Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},
                new SqlSearchStringValue{Field = "DESIGNER",    Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},
                new SqlSearchStringValue{Field = "APPROVEDUSER",Ooperator = "IS",   Value="NULL", Logic="OR"},
                new SqlSearchStringValue{Field = "APPROVEDUSER",Ooperator = "=",   Value="''"},
                new SqlSearchStringValue{Logic=")"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'"}
            };
        return sql;
    }

    /// <summary>
    /// ArcSuite未登録を検索
    /// </summary>
    /// <param name="REGISTEDUSER"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> ArcSuiteNotRegisteredList(int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'" ,   Logic="AND"},
                new SqlSearchStringValue{Ooperator = "IS", Field = "ARCSUITEID",  Value="NULL"},
            };
        return sql;
    }

    /// <summary>
    /// アークスイート登録予定
    /// </summary>
    /// <returns></returns>
    public static List<SqlSearchStringValue> ArcSuiteRegistrationScheduled(int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "AUTHOR",      Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},
                new SqlSearchStringValue{Field = "DESIGNER",    Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},
                new SqlSearchStringValue{Field = "APPROVEDUSER",Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},
                new SqlSearchStringValue{Field = "REGISTWAITINGFLAG",Ooperator = "=", Value="1"}, // SqlServerのbit型は IS NULL OR = 0 か = 1 で検索する
                new SqlSearchStringValue{Logic=")"},
                new SqlSearchStringValue{Logic="AND"},
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'"}
            };
        return sql;
    }

    /// <summary>
    /// 無条件ですべてを検索
    /// </summary>
    /// <param name="timespanDay"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> LISTALL(int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Field = "TICKETCODE",      Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'"}

            };
        return sql;
    }

    /// <summary>
    /// 承認ユーザーを検索
    /// </summary>
    /// <param name="Username"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> ApprovalUser(string Username, int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);
        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "AUTHOR",      Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},   //AUTHORが空でなく、かつ
                new SqlSearchStringValue{Field = "APPROVEDUSER",    Ooperator = "LIKE",   Value=$"'{Username}%'"},          // APPROVEDUSERに{Username} であるもの
                new SqlSearchStringValue{Logic=")"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "APPROVEDDATE",Ooperator = ">",  Value=$"'{tmestring}%'" }// APPROVEDDATE は nvarchar(50) であるので文字列として扱う
           };
        return sql;
    }


    /// <summary>
    /// コミットユーザーを検索
    /// </summary>
    /// <param name="Username"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> CommitUser(string Username, int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "AUTHOR",      Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},   //AUTHORが空でなく、かつ
                new SqlSearchStringValue{Field = "COMMITUSER",    Ooperator = "LIKE",   Value=$"'{Username}%'"},          // COMMITUSERに{Username} であるもの
                new SqlSearchStringValue{Logic=")"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'" }
           };
        return sql;
    }

    /// <summary>
    /// コミットユーザーを検索
    /// </summary>
    /// <param name="Username"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> PaperSize(string PaperSizeName, int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "PAPERSIZE",    Ooperator = "LIKE",   Value=$"'{PaperSizeName}%'"},          // COMMITUSERに{Username} であるもの
                new SqlSearchStringValue{Logic=")"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'" }
           };
        return sql;
    }


    /// <summary>
    /// コミットホスト名で検索
    /// </summary>
    /// <param name="Username"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> CommitHost(string CommitHost, int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "COMMITHOST",    Ooperator = "LIKE",   Value=$"'{CommitHost}%'"},          // COMMITUSERに{Username} であるもの
                new SqlSearchStringValue{Logic=")"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'" }
           };
        return sql;
    }


    /// <summary>
    /// 製図者が空欄でなくかつ承認者名に指定した文字列が含まれるものを検索
    /// </summary>
    /// <param name="REGISTEDUSER"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> UserApprovalvleNameList(string REGISTEDUSER)
    {
        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Field = "AUTHOR",      Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},
                new SqlSearchStringValue{Field = "REGISTEDUSER",    Ooperator = "LIKE",   Value=$"'%{REGISTEDUSER}%'"}
            };
        return sql;
    }

    /// <summary>
    /// コミットユーザーを検索・アークスイート登録済みを除く　（まだ未完成）
    /// </summary>
    /// <param name="Username"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> CommitUserExcludeArcSuiteRegisted(string Username, int timeSpan)
    {
        string tmestring = MyDataTimeMethod.GetOldDateString(timeSpan);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
             new SqlSearchStringValue{Logic="("},
                new SqlSearchStringValue{Field = "ARCSUITEID",      Ooperator = "IS",   Value="NULL",   Logic="AND"},   //
                new SqlSearchStringValue{Field = "AUTHOR",      Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},   //AUTHORが空でなく、かつ
                new SqlSearchStringValue{Field = "COMMITUSER",    Ooperator = "LIKE",   Value=$"'{Username}%'"},          // COMMITUSERに{Username} であるもの
                new SqlSearchStringValue{Logic=")"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Logic="AND"},   // 製図者がNULLでなく、かつ
                new SqlSearchStringValue{Field = "TIMESTAMP",Ooperator = ">",  Value=$"'{tmestring}'" }
            };
        return sql;
    }


    /// <summary>
    /// ARCSUITEIDが空欄でなくCREATESOFTWARE指定値であるものを検索
    /// </summary>
    /// <param name="Username"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> CreatedCADSerch(string Cadname)
    {
        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Field = "ARCSUITEID",      Ooperator = "IS",   Value="NOT NULL",   Logic="AND"},
                new SqlSearchStringValue{Field = "CREATESOFTWARE",    Ooperator = "LIKE",   Value=$"'%{Cadname}%'"}
            };
        return sql;
    }

    /// <summary>
    /// 優先登録フラグがONのものを検索
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> PRIORITYREGSTFRAG(int flag)
    {
        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Field = "PRIORITYREGISTFLAG",      Ooperator = "=",   Value=flag.ToString()},
            };
        return sql;
    }

    /// <summary>
    /// 承認済みのものすべて
    /// </summary>
    /// <param name="timespanDay"></param>
    /// <returns></returns>
    public static List<SqlSearchStringValue> LISTApproved(int timespanDay)
    {
        string oldDate = MyDataTimeMethod.GetOldDateString(timespanDay);

        List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>()
            {
                new SqlSearchStringValue{Field = "APPROVEDDATE",Ooperator = ">",  Value=$"'{oldDate}%'" ,   Logic="AND"},
                new SqlSearchStringValue{Field = "TICKETCODE",      Ooperator = "IS",   Value="NOT NULL"},
            };
        return sql;
    }



}

