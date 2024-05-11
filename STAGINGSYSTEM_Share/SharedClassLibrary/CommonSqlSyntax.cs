using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyoMcMfg.Staging.DataBaseConfig;

namespace SharedClassLibrary
{
    public static  class CommonSqlSyntax
    {
        /// <summary>
        /// ■ArcSuite通常登録一覧リスト検索用SQL構文。
        /// </summary>
        /// <returns></returns>
        public static List<SqlSearchStringValue> ArcSuiteNormalRegistration(string pasttTImeString)
        {
            List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>(){
                    // REGISTWAITINGFLAG が　1であり、なおかつREGISTWAITINGFLAGGEDTIME時刻が、pasttTImeStringより前のものをリスト化する
                    new SqlSearchStringValue{Field = "REGISTWAITINGFLAG",      Ooperator = "=",   Value="1",   Logic="AND"},
                    new SqlSearchStringValue{Field = "REGISTWAITINGFLAGGEDTIME",    Ooperator = "<",   Value=$"'{pasttTImeString}'"}
            };
            return sql;
        }

        /// <summary>
        /// ■ArcSuite優先登録一覧リスト検索用SQL構文。
        /// </summary>
        /// <returns></returns>
        public static List<SqlSearchStringValue> ArcSuitePriorityRegistration()
        {
            List<SqlSearchStringValue> sql = new List<SqlSearchStringValue>(){
                    // REGISTWAITINGFLAG が　1であり、なおかつREGISTWAITINGFLAGGEDTIME時刻が現在時刻より後のものを
                    new SqlSearchStringValue{Field = "REGISTWAITINGFLAG",      Ooperator = "=",   Value="1",   Logic="AND"},
                    new SqlSearchStringValue{Field = "PRIORITYREGISTFLAG",      Ooperator = "=",   Value="1"}
            };
            return sql;
        }

    }
}
