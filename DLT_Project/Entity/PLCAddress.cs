using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLT_Project.Entity
{
    public class PLCAddress
    {
        public string Barcode { get { return "D4500"; } }

        /// <summary>
        /// 1:加热；0:停止
        /// </summary>
        public string HeaterSignal { get { return "D5000"; } }
        /// <summary>
        /// 回馈PLC信号
        /// </summary>
        public string HeaterBack { get { return "D5001"; } }
        /// <summary>
        /// 读取电阻信号 1:读取 0:不读取
        /// </summary>
        public string ResSignal { get { return "D5002"; } }
        /// <summary>
        /// 电阻值回写PLC
        /// </summary>
        public string ResDataBack { get { return "D5003"; } }
        /// <summary>
        /// PLC传检测OK，通知PC存储检测记录
        /// </summary>
        public string OverSignal { get { return "D5005"; } }
        /// <summary>
        /// PC反馈存储结果
        /// </summary>
        public string OverBack { get { return "D5006"; } }

        public string LHeater { get { return "D4000"; } }
        public string LSBROn { get { return "D4002"; } }
        public string LSafety { get { return "D4004"; } }
        public string LWalkIn { get { return "D4006"; } }
        public string LBukle { get { return "D4008"; } }
        public string LSBROff { get { return "D4009"; } }

        public string RHeater { get { return "D4100"; } }
        public string RSBROn { get { return "D4102"; } }
        public string RSafety { get { return "D4104"; } }
        public string RWalkIn { get { return "D4106"; } }
        public string RBukle { get { return "D4108"; } }
        public string RSBROff { get { return "D4109"; } }
    }
}
