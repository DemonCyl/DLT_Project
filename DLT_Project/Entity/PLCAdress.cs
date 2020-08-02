using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLT_Project.Entity
{
    public class PLCAdress
    {
        public static string Barcode { get { return "D4500"; } }

        /// <summary>
        /// 1:加热；0:停止
        /// </summary>
        public static string HeaterSignal { get { return "D5000"; } }
        /// <summary>
        /// 回馈PLC信号
        /// </summary>
        public static string HeaterBack { get { return "D5001"; } }
        /// <summary>
        /// 读取电阻信号 1:读取 0:不读取
        /// </summary>
        public static string ResSignal { get { return "D5002"; } }
        /// <summary>
        /// 电阻值回写PLC
        /// </summary>
        public static string ResDataBack { get { return "D5003"; } }
        /// <summary>
        /// PLC传检测OK，通知PC存储检测记录
        /// </summary>
        public static string OverSignal { get { return "D5005"; } }
        /// <summary>
        /// PC反馈存储结果
        /// </summary>
        public static string OverBack { get { return "D5006"; } }

        public static string LHeater { get { return "D4000"; } }
        public static string LSBROn { get { return "D4002"; } }
        public static string LSafety { get { return "D4004"; } }
        public static string LWalkIn { get { return "D4006"; } }
        public static string LBukle { get { return "D4008"; } }

        public static string RHeater { get { return "D4100"; } }
        public static string RSBROn { get { return "D4102"; } }
        public static string RSafety { get { return "D4104"; } }
        public static string RWalkIn { get { return "D4106"; } }
        public static string RBukle { get { return "D4108"; } }
    }
}
