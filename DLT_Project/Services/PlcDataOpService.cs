using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLT_Project.Entity;
using HslCommunication;
using HslCommunication.Profinet.Melsec;
using HslCommunication.LogNet;

namespace DLT_Project.Services
{
    public class PlcDataOpService
    {
        private ConfigData configData;
        private MelsecMcNet plc;
        private ILogNet log;
        private OperateResult connect;
        private PLCAddress address = new PLCAddress();

        public PlcDataOpService(ConfigData config, ILogNet log)
        {
            this.configData = config;
            this.log = log;
        }

        public bool GetConnection()
        {
            plc = new MelsecMcNet(configData.PlcIpAdress, configData.PlcPort);
            plc.ConnectTimeOut = 2000; //超时时间

            connect = plc.ConnectServer();

            if (!connect.IsSuccess)
            {
                log.WriteError("PLC Connect Error!");
            }

            return connect.IsSuccess;
        }

        public void Close()
        {
            if (connect.IsSuccess)
            {
                plc.ConnectClose();
            }
        }

        public string ReadBarCode()
        {
            string barCode = null;
            OperateResult<string> re = plc.ReadString(address.Barcode, 40);
            if (re.IsSuccess)
            {
                barCode = re.Content.Trim();
            }
            else
            {
                log.WriteError("BarCode Read Error!");
            }
            return barCode;
        }

        public LRType ReadType()
        {
            LRType type = LRType.Null;
            OperateResult<short> re = plc.ReadInt16(address.Type);
            if (re.IsSuccess)
            {
                type = (LRType) re.Content;
            }
            else
            {
                log.WriteError("Type Read Error!");
            }
            return type;
        }

        public short ReadSignal(SignalType type)
        {
            OperateResult<short> re = null;
            switch (type)
            {
                case SignalType.HeaterSignal: // heater
                    re = plc.ReadInt16(address.HeaterSignal);
                    break;
                case SignalType.ResSignal: // res
                    re = plc.ReadInt16(address.ResSignal);
                    break;
                case SignalType.OverSignal: // insert
                    re = plc.ReadInt16(address.OverSignal);
                    break;

            }
            short signal = -1;
            if (re.IsSuccess)
            {
                signal = re.Content;
            }
            else
            {
                log.WriteError("Signal Read Error!");
            }
            return signal;
        }

        public void WriteOverBack()
        {
            OperateResult re = plc.Write(address.OverBack, (short)1);
            if (!re.IsSuccess)
            {
                log.WriteError("OverBack Write Error!");
            }
        }

        public void WriteHeaterBack()
        {
            OperateResult re = plc.Write(address.HeaterBack, (short)1);
            if (!re.IsSuccess)
            {
                log.WriteError("HeaterBack Write Error!");
            }
        }

        public void WriteRes(float data)
        {
            OperateResult re = plc.Write(address.ResDataBack, data);
            if (!re.IsSuccess)
            {
                log.WriteError("ResData Write Error!");
            }
        }

        public void WriteSignal(float data)
        {
            OperateResult re = plc.Write(address.Signal, data);
            if (!re.IsSuccess)
            {
                log.WriteError("ResData Write Error!");
            }
        }

        /// <summary>
        /// 取得存储数据
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public MesInfo GetInfo(string barCode, LRType type)
        {
            MesInfo info = new MesInfo();
            info.BarCode = barCode;
            switch (type)
            {
                case LRType.Left:
                    //Left
                    info.Type = 1;
                    OperateResult<float> reLHeater = plc.ReadFloat(address.LHeater);
                    if (reLHeater.IsSuccess) info.Heater = reLHeater.Content;

                    OperateResult<float> reLSBROn = plc.ReadFloat(address.LSBROn);
                    if (reLSBROn.IsSuccess) info.SBROn = reLSBROn.Content;

                    OperateResult<float> reLSBROff = plc.ReadFloat(address.LSBROff);
                    if (reLSBROff.IsSuccess) info.SBROff = reLSBROff.Content;

                    OperateResult<float> reLSafety = plc.ReadFloat(address.LSafety);
                    if (reLSafety.IsSuccess) info.Safety = reLSafety.Content;

                    OperateResult<float> reLWalkIn = plc.ReadFloat(address.LWalkIn);
                    if (reLWalkIn.IsSuccess) info.WalkInLight = reLWalkIn.Content;

                    OperateResult<short> reLBukle = plc.ReadInt16(address.LBukle);
                    if (reLBukle.IsSuccess) info.Bukle = reLBukle.Content;
                    break;
                case LRType.Right:
                    //Right
                    info.Type = 2;
                    OperateResult<float> reRHeater = plc.ReadFloat(address.RHeater);
                    if (reRHeater.IsSuccess) info.Heater = reRHeater.Content;

                    OperateResult<float> reRSBROn = plc.ReadFloat(address.RSBROn);
                    if (reRSBROn.IsSuccess) info.SBROn = reRSBROn.Content;

                    OperateResult<float> reRSBROff = plc.ReadFloat(address.RSBROff);
                    if (reRSBROff.IsSuccess) info.SBROff = reRSBROff.Content;

                    OperateResult<float> reRSafety = plc.ReadFloat(address.RSafety);
                    if (reRSafety.IsSuccess) info.Safety = reRSafety.Content;

                    OperateResult<float> reRWalkIn = plc.ReadFloat(address.RWalkIn);
                    if (reRWalkIn.IsSuccess) info.WalkInLight = reRWalkIn.Content;

                    OperateResult<short> reRBukle = plc.ReadInt16(address.RBukle);
                    if (reRBukle.IsSuccess) info.Bukle = reRBukle.Content;
                    break;
            }

            return info;
        }

        /// <summary>
        /// 测试用，可以删
        /// </summary>
        /// <returns></returns>
        public float test()
        {
            float t = -1f;
            OperateResult re = plc.Write("D4000", 100.41f);
            if (!re.IsSuccess)
            {
                log.WriteError("ResData Write Error!");
            }

            OperateResult<float> ret = plc.ReadFloat("D4000");
            if (ret.IsSuccess) t = ret.Content;

            return t;
        }
    }
}
