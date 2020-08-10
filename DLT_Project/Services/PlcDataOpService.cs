using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLT_Project.Entity;
using HslCommunication;
using HslCommunication.Profinet.Melsec;
using HslCommunication.LogNet;
using log4net;

namespace DLT_Project.Services
{
    public class PlcDataOpService
    {
        private ConfigData configData;
        private MelsecMcNet plc;
        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OperateResult connect;
        private PLCAddress address;

        public PlcDataOpService(ConfigData config, PLCAddress plcAddress)
        {
            this.configData = config;
            this.address = plcAddress;
        }

        public bool GetConnectionFx5u()
        {
            plc = new MelsecMcNet(configData.Fx5uPlcIpAdress, configData.Fx5uPlcPort);
            plc.ConnectTimeOut = 2000; //超时时间

            connect = plc.ConnectServer();

            if (!connect.IsSuccess)
            {
                log.Error("检测PLC Connect Error!");
            }

            return connect.IsSuccess;
        }

        public bool GetConnectionQ()
        {
            plc = new MelsecMcNet(configData.QPlcIpAdress, configData.QPlcPort);
            plc.ConnectTimeOut = 2000; //超时时间
            plc.NetworkNumber = 0x00;  // 网络号
            plc.NetworkStationNumber = 0x00; // 网络站号

            connect = plc.ConnectServer();

            if (!connect.IsSuccess)
            {
                log.Error("主线PLC Connect Error!");
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
            OperateResult<string> re = plc.ReadString(address.Barcode, address.BarcodeLength);
            if (re.IsSuccess)
            {
                barCode = re.Content;
                barCode = barCode.Replace("\0","").Trim();
            }
            else
            {
                log.Error("BarCode Read Error!");
            }
            return barCode;
        }

        public LRType ReadType()
        {
            LRType type = LRType.Null;
            OperateResult<short> re = plc.ReadInt16(address.Type);
            if (re.IsSuccess)
            {
                type = (LRType)re.Content;
            }
            else
            {
                log.Error("Type Read Error!");
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
                log.Error("Signal Read Error!");
            }
            return signal;
        }

        public void WriteOverBack()
        {
            OperateResult re = plc.Write(address.OverBack, (short)1);
            if (!re.IsSuccess)
            {
                log.Error("OverBack Write Error!");
            }
        }

        public void WriteHeaterBack()
        {
            OperateResult re = plc.Write(address.HeaterBack, (short)1);
            if (!re.IsSuccess)
            {
                log.Error("HeaterBack Write Error!");
            }
        }

        public void WriteRes(float data)
        {
            OperateResult re = plc.Write(address.ResDataBack, data);
            if (!re.IsSuccess)
            {
                log.Error("ResData Write Error!");
            }
        }

        public void WriteSignal(float data)
        {
            OperateResult re = plc.Write(address.Signal, data);
            if (!re.IsSuccess)
            {
                log.Error("Signal Write Error!");
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
            //OperateResult re = plc.Write("D88", 100.41f);
            //if (!re.IsSuccess)
            //{
            //    log.Error("ResData Write Error!");
            //}

            OperateResult<float> ret = plc.ReadFloat("D88");
            if (ret.IsSuccess) t = ret.Content;

            return t;
        }
    }
}
