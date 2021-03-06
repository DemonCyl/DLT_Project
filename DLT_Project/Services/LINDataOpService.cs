﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using de.lipowsky.LIN.Devices;
using DLT_Project.Entity;
using log4net;

namespace DLT_Project.Services
{
    public class LINDataOpService
    {
        private int handle;
        BabyLin.CallBackFrameDelegate frameDelegate;
        private const uint FrameIDForAllFrames = 0xC0000000;
        private string stopCmd = "inject 0x02 ";
        private string leftCmd = "inject 0x02 ";
        private string rightCmd = "inject 0x02 ";
        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LINDataOpService(ConfigData configData)
        {
            this.stopCmd = stopCmd + configData.StopCmd + " 0 0";
            this.leftCmd = leftCmd + configData.LeftCmd + " 100 0";
            this.rightCmd = rightCmd + configData.RightCmd + " 100 0";
        }

        //初始化与检测设备的连接
        public bool Initial()
        {
            //读取DLL，获得设备版本
            try
            {
                int major = 0;
                int minor = 0;
                BabyLin.BL_getVersion(ref major, ref minor);
                string versionString = BabyLin.BL_getVersionString();
                string wrapperVersion = BabyLin.GetWrapperVersion();
            }
            catch (Exception)
            {
                throw new Exception("未找到BabyLIN.DLL，请联系软件工程师！");
            }
            // 获取设备COM口
            int[] babyLinPorts = BabyLin.GetPorts();
            if (babyLinPorts.Length == 0)
            {
                throw new Exception("与检测设备失去连接，请检查连接状态！");
            }
            //打开端口，并取得句柄
            this.handle = BabyLin.BL_open(babyLinPorts[0]);
            if (this.handle == 0)
            {
                throw new Exception("无法与检测设备建立连接");
            }
            //加载SDF文件
            //string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Actuator.sdf");
            //int ret = BabyLin.BL_loadSDF(this.handle, filePath, 1);
            //if (ret != BabyLin.BL_OK)
            //{
            //    throw new Exception("无法加载SDF文件");
            //}
            //注册回调函数FrameCallbackFunc，用于接收设备返回信息
            this.frameDelegate = new BabyLin.CallBackFrameDelegate(this.FrameCallbackFunc);
            int ret = BabyLin.BL_registerFrameCallback(this.handle, frameDelegate);

            ret = BabyLin.BL_sendCommand(this.handle, "mon_init 19200 1;");
            if (ret != BabyLin.BL_OK)
            {
                throw new Exception("Could initialize the monitor mode.");
            }

            ret = BabyLin.BL_sendCommand(this.handle, string.Format("mon_on {0} 1;", FrameIDForAllFrames));
            if (ret != BabyLin.BL_OK)
            {
                throw new Exception("Could activate the monitor mode");
            }

            return true;
        }

        public void DisConnect()
        {
            int ret;

            // unsubscribe from frame reports
            BabyLin.BL_sendCommand(this.handle, "nodisframe 255;");

            // stop the bus
            ret = BabyLin.BL_sendCommand(this.handle, "stop;");

            // unregister callbacks
            BabyLin.BL_registerFrameCallback(this.handle, null);
            BabyLin.BL_registerSignalCallback(this.handle, null);

            // close connection to BabyLIN
            ret = BabyLin.BL_close(this.handle);
            this.handle = 0;
        }

        private void FrameCallbackFunc(BabyLin.BL_frame_t frame)
        {
            // Write frame data to output
        }

        public void SendCmd(LRType type, CmdType cmdType)
        {
            int ret = 0;
            switch (cmdType)
            {
                case CmdType.start:
                    if (type == LRType.Left)
                    {
                        ret = BabyLin.BL_sendCommand(this.handle, leftCmd);
                        log.Info("左侧加热！");
                    }
                    else if (type == LRType.Right)
                    {
                        ret = BabyLin.BL_sendCommand(this.handle, rightCmd);
                        log.Info("右侧加热！");
                    }
                    else if (type == LRType.Stop)
                    {
                        ret = BabyLin.BL_sendCommand(this.handle, stopCmd);
                        log.Info("1-停止加热！");
                    }
                    break;
                case CmdType.stop:
                    ret = BabyLin.BL_sendCommand(this.handle, stopCmd);
                    log.Info("0-停止加热！");
                    break;
            }
            if (ret != BabyLin.BL_OK)
            {
                log.Error("Send Error：" + ret);
            }
        }

        public bool CheckStatus()
        {
            int ret = BabyLin.BL_sendCommand(this.handle, string.Format("mon_on {0} 1;", FrameIDForAllFrames));
            if (ret != BabyLin.BL_OK)
            {
                return false;
            }

            return true;
        }
    }
}
