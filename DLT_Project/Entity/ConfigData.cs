﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLT_Project.Entity
{
    public class ConfigData
    {

        /// <summary>
        /// 调试模式：1: 打开  0:关闭
        /// </summary>
        public bool DebugMode { get; set; }

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
        public string Fx5uPlcIpAdress { get; set; }

        public int Fx5uPlcPort { get; set; }

        public string QPlcIpAdress { get; set; }

        public int QPlcPort { get; set; }
        #endregion

        public string StopCmd { get; set; }
        public string LeftCmd { get; set; }
        public string RightCmd { get; set; }
    }
}
