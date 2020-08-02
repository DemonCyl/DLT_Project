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

namespace DLT_Project
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ConfigData config;
        private MesDataOpService mesDataOpService;
        private ResDataOpService resDataOpService;
        private DispatcherTimer ShowTimer;
        private bool sPort = false;
        private bool mData = false;
        private bool mPlc = false;
        private static BitmapImage IFalse = new BitmapImage(new Uri("/Images/01.png", UriKind.Relative));
        private static BitmapImage ITrue = new BitmapImage(new Uri("/Images/02.png", UriKind.Relative));

        public MainWindow()
        {
            InitializeComponent();

            Init();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += (s, e) =>
            {
                resDataOpService.ReadData();
            };
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(2000);
            dispatcherTimer.Start();


        }

        /// <summary>
        /// 读取本地配置文件
        /// </summary>
        private void LoadJsonData()
        {
            using (var sr = File.OpenText("C:\\config\\config2.json"))
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

            PLCImage.Source = (mPlc ? ITrue : IFalse);
            DataImage.Source = (mData ? ITrue : IFalse);
            PortImage.Source = (sPort ? ITrue : IFalse);


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
            Init();
        }

        private void PlcData()
        {

            if (true)
            {

            }
        }
    }
}
