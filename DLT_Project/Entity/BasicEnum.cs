using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLT_Project.Entity
{
    /// <summary>
    /// 信号类型
    /// </summary>
    public enum SignalType
    {
        HeaterSignal = 1,
        ResSignal = 2,
        OverSignal = 3
    }

    /// <summary>
    /// 座椅左右型号
    /// </summary>
    public enum LRType
    {
        Null = 0,
        Left = 1,
        Right = 2,
        Stop = 3
    }

    /// <summary>
    /// 加热信号
    /// </summary>
    public enum CmdType
    {
        stop = 0,
        start = 1
    }
}
