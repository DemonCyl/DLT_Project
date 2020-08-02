using DLT_Project.Entity;
using DLT_Project.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HslCommunication;
using HslCommunication.Profinet.Melsec;

namespace DLT_Project
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ConfigData config;
        private MesInfo mesInfo;
        private MesDataOpService mesDataOpService;
        private ResDataOpService resDataOpService;
        private DispatcherTimer ShowTimer;
        private bool sPort = false;
        private bool mData = false;
        private bool mPlc = false;
        private static BitmapImage IFalse = new BitmapImage(new Uri("/Static/01.png", UriKind.Relative));
        private static BitmapImage ITrue = new BitmapImage(new Uri("/Static/02.png", UriKind.Relative));
        private DispatcherTimer timer;
        private bool dataMark = false;
        private int over = 0;
        private string markBarcode = null;

        private MelsecMcNet plc;


        public MainWindow()
        {
            InitializeComponent();

            Init();

            //连接ok
            if (sPort && mData && mPlc)
            {
                CycleDataRead();
            }

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += (s, e) =>
            {
                // start 存储
                if (over == 1 && dataMark)
                {
                    bool t = mesDataOpService.DataInsert(mesInfo);
                }


            };
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(2000);
            dispatcherTimer.Start();


        }

        /// <summary>
        /// 读取本地配置文件
        /// </summary>
        private void LoadJsonData()
        {
            using (var sr = File.OpenText("C:\\config\\DLTConfig.json"))
            {
                string JsonStr = sr.ReadToEnd();
                if (config == null)
                {
                    config = JsonConvert.DeserializeObject<ConfigData>(JsonStr);
                }
            }
        }

        private void Init()
        {
            LoadJsonData();

            #region 时间定时器
            ShowTimer = new System.Windows.Threading.DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowTimer1);
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1);
            ShowTimer.Start();
            #endregion

            mesDataOpService = new MesDataOpService(config);
            resDataOpService = new ResDataOpService(config);

            mData = mesDataOpService.GetConnection();
            sPort = resDataOpService.GetConnection();
            //PLC connection
            //Lin connection

            PLCImage.Source = (mPlc ? ITrue : IFalse);
            DataImage.Source = (mData ? ITrue : IFalse);
            PortImage.Source = (sPort ? ITrue : IFalse);
        }

        private void CycleDataRead()
        {

            timer = new DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                //读取BarCode
                string barCode = null;
                //初始化状态
                if (barCode != null && !barCode.Equals("") && !barCode.Equals(markBarcode))
                {
                    markBarcode = barCode;
                    dataMark = false;
                }
                //判断型号

                #region 读取Heater信号
                int heater = 1;
                switch (heater)
                {
                    // 0 发送停止加热 00 00 00 00 00 00 00 00
                    case 0:
                        break;
                    //1 发送驱动加热 00 00 ?? ?? 00 00 00 00 // ??->E3  L  R
                    case 1:
                        break;
                    default: break;
                }
                #endregion

                #region 读取Res信号
                int res = 0;
                if (res == 1)
                {
                    resDataOpService.ReadData();
                    //write PLC data

                }
                #endregion

                #region 读取存储信号
                over = 0;

                if (over == 1 && !dataMark)
                {
                    dataMark = true;
                    // 读取测试数据资料
                    mesInfo = new MesInfo();
                    // 存储MES数据
                    bool t = mesDataOpService.DataInsert(mesInfo);
                    if (!t) // 存储失败继续下一次存储直到成功
                    {
                        dataMark = false;
                    }else
                    {
                        // 回写PLC存储状态 1:成功
                    }

                }
                #endregion

            };
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Start();
        }

        private void ShowTimer1(object sender, EventArgs e)
        {
            this.TM.Text = " ";
            //获得年月日 
            this.TM.Text += DateTime.Now.ToString("yyyy年MM月dd日");   //yyyy年MM月dd日 
            this.TM.Text += "\r\n       ";
            //获得时分秒 
            this.TM.Text += DateTime.Now.ToString("HH:mm:ss");
            this.TM.Text += "              ";
            this.TM.Text += DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            this.TM.Text += " ";
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mesDataOpService.Close();
            resDataOpService.Close();
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mesDataOpService.Close();
            resDataOpService.Close();
            mesDataOpService = null;
            resDataOpService = null;
            config = null;
            if(timer != null && timer.IsEnabled)
            {
                timer.Stop();
                timer = null;
            }

            Init();
            //连接ok
            if (sPort && mData && mPlc)
            {
                CycleDataRead();
            }
        }

    }
}
