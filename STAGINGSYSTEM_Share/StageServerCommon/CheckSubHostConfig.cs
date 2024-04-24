using SasaLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyoStageService
{
    public static  class CheckSubHostConfig
    {
        /// <summary>
        /// サブホストに指定されたホストのサーバーモードを得る
        /// </summary>
        /// <returns></returns>
        public static  StageServerConfig.ServerMode ServerMode(EventsSummary evt)
        {
            if (string.IsNullOrWhiteSpace(StageServerConfig.Config.SUB_DBHOST) == false)
            {
                PIPEClient pIPEClient = new PIPEClient("", "", "", false, StageServerConfig.Config.SUB_DBHOST, StageServerConfig.Config.PipeNameDC);

                var subHostmode = pIPEClient.Get_Status_Servemode(StageServerConfig.Config.PipeClientStrmeConnectDefaultTimeOut, StageServerConfig.Config.ReadWriteHandShakeStreamStringTimeOut, evt);

                return subHostmode;
            } // ■サブDBホストのサーバーモードを取得 Maser or Slave;
            else
            {
                return StageServerConfig.ServerMode.Unknown;
            }

        }

    }
}
