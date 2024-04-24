using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyoMcMfg.Staging.DataBaseConfig;

namespace ToyoStageService
{
    public static class UserDataBase
    {
        public static FieldValueSet SearchFrom_USERID(string userId)
        {
            DataBaseSearch dbSearchObj;
            dbSearchObj = new DataBaseSearch("USERSTORE", "USERID", userId, new List<string>() { "*" }, "AuthorizedUserConnectionStatus", EventLog: StageServerConfig.Config.RecordOfNormaEvents_DataBaseSearch);
            FieldValueSet result = dbSearchObj.DBresultOne;
            return result;
        }

    }
}
