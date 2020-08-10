using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLT_Project.Entity
{
    public class PLCAddress
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }

        public ushort BarcodeLength { get; set; }

        /// <summary>
        /// Left & Right 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 准备信号
        /// </summary>
        public string Signal { get; set; }

        /// <summary>
        /// 1:加热；0:停止
        /// </summary>
        public string HeaterSignal { get; set; }
        /// <summary>
        /// 回馈PLC信号
        /// </summary>
        public string HeaterBack { get; set; }
        /// <summary>
        /// 读取电阻信号 1:读取 0:不读取
        /// </summary>
        public string ResSignal { get; set; }
        /// <summary>
        /// 电阻值回写PLC
        /// </summary>
        public string ResDataBack { get; set; }
        /// <summary>
        /// PLC传检测OK，通知PC存储检测记录
        /// </summary>
        public string OverSignal { get; set; }
        /// <summary>
        /// PC反馈存储结果
        /// </summary>
        public string OverBack { get; set; }

        public string LHeater { get; set; }
        public string LSBROn { get; set; }
        public string LSafety { get; set; }
        public string LWalkIn { get; set; }
        public string LBukle { get; set; }
        public string LSBROff { get; set; }

        public string RHeater { get; set; }
        public string RSBROn { get; set; }
        public string RSafety { get; set; }
        public string RWalkIn { get; set; }
        public string RBukle { get; set; }
        public string RSBROff { get; set; }
    }
}
