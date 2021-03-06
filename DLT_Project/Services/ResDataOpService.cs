﻿using DLT_Project.Entity;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HslCommunication.LogNet;
using log4net;

namespace DLT_Project.Services
{

    public class ResDataOpService
    {

        private SerialPort serialPort;
        private ConfigData config;
        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string cmd = @":FETCH?";

        public ResDataOpService(ConfigData config)
        {
            this.config = config;
        }

        public bool GetConnection()
        {
            bool mark = false;
            if (serialPort == null)
            {
                serialPort = new SerialPort(config.PortName, config.BaudRate, Parity.None, 8, StopBits.One);
                serialPort.DtrEnable = true;
                serialPort.RtsEnable = true;
                serialPort.ReadTimeout = 200;
                mark = OpenPort();
            }
            else
            {
                mark = OpenPort();
            }
            return mark;
        }

        private bool OpenPort()
        {
            string message = null;
            try//这里写成异常处理的形式以免串口打不开程序崩溃
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (serialPort.IsOpen)
            {
                log.Info("电阻计连接成功！");
                return true;
            }
            else
            {
                //MessageBox.Show("电阻计打开失败!原因为： " + message);
                log.Error("电阻计打开失败!原因为： " + message);
                return false;
            }
        }

        public void Close()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public float ReadData()
        {
            string re = null;
            float data = 0;
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                try
                {
                    serialPort.WriteLine(cmd);
                    re = serialPort.ReadLine();
                    data = TransformData(re);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    throw new Exception("1+" + ex.Message);
                }
            }
            return data;
        }

        public void Check()
        {
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                try
                {
                    serialPort.WriteLine(cmd);
                    serialPort.ReadLine();
                }
                catch (TimeoutException ex1)
                {
                    this.Close();
                    this.GetConnection();
                }
                catch (Exception ex)
                {
                    log.Error("Check Error:" + ex.Message);
                }
            }
            else
            {
                this.GetConnection();
            }
        }

        public float TransformData(string strData)
        {
            float data = 0f;
            // 00.000E-03  e.g. 00 000 -03
            log.Info("Read: " + strData);
            string[] sArray1 = strData.Split('E');
            switch (sArray1[1].Trim())
            {
                case "-03":
                    data = float.Parse(sArray1[0]) * 0.001f;
                    break;
                case "+00":
                    data = float.Parse(sArray1[0]);
                    break;
                case "+03":
                    data = float.Parse(sArray1[0]) * 1000f;
                    break;
                case "+06":
                    data = float.Parse(sArray1[0]) * 1000000f;
                    break;
                case "+18":
                case "+19":
                case "+20":
                case "+28":
                case "+29":
                case "+30":
                    data = 2000000f;
                    break;
                default:
                    data = -1.1f;
                    break;
            }
            return data;
        }
    }
}
