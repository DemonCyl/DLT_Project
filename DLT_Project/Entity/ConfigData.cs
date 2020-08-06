using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLT_Project.Entity
{
    public class ConfigData
    {


        #region 数据库配置
        public string DataIpAdress { get; set; }

        public string DataBaseName { get; set; }

        public string Uid { get; set; }

        public string Pwd { get; set; }
        #endregion

        #region 电阻计配置
        public string PortName { get; set; }

        public int BaudRate { get; set; }
        #endregion

        #region 三菱Plc配置
        public string PlcIpAdress { get; set; }

        public int PlcPort { get; set; }
        #endregion

        public string StopCmd { get; set; }
        public string LeftCmd { get; set; }
        public string RightCmd { get; set; }
    }
}
