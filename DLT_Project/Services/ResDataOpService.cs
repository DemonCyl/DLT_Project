using DLT_Project.Entity;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DLT_Project.Services
{

    public class ResDataOpService
    {

        private SerialPort serialPort;
        private ConfigData config;
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
                serialPort.ReadTimeout = 100;
                //serialPort.NewLine = "\r\n";
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
                return true;
            }
            else
            {
                MessageBox.Show("串口打开失败!原因为： " + message);
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

        public void ReadData()
        {
            string re = null;
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                try
                {
                    serialPort.WriteLine(cmd);
                    re = serialPort.ReadLine();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
