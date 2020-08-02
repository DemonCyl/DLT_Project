using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLT_Project.Entity;
using HslCommunication;
using HslCommunication.Profinet.Melsec;

namespace DLT_Project.Services
{
    public class PlcDataOpService
    {
        private ConfigData configData;
        private MelsecMcNet plc;
        private OperateResult connect;

        public PlcDataOpService(ConfigData config)
        {
            this.configData = config;
        }

        public bool GetConnection()
        {
            plc = new MelsecMcNet(configData.PlcIpAdress, configData.PlcPort);
            plc.ConnectTimeOut = 2000; //超时时间

            connect = plc.ConnectServer();

            return connect.IsSuccess;
        }

        public void Close()
        {
            if (connect.IsSuccess)
            {
                plc.ConnectClose();
            }
        }
    }
}
