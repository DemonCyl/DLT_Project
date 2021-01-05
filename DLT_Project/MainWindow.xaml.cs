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
using HslCommunication.LogNet;
using System.Diagnostics;
using log4net;

namespace DLT_Project
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ConfigData config;
        private PLCAddress address;
        private MesInfo mesInfo;
        private MesDataOpService mesDataOpService;
        private ResDataOpService resDataOpService;
        private PlcDataOpService plcDataOpService;
        private PlcDataOpService qPlcDataOpService;
        private LINDataOpService linDataOpService;
        private DispatcherTimer ShowTimer;
        private DispatcherTimer timer;
        private bool sPort = false;
        private bool mData = false;
        private bool mPlc = false;
        private bool qPlc = false;
        private bool babyLIN = false;
        private static BitmapImage IFalse = new BitmapImage(new Uri("/Static/01.png", UriKind.Relative));
        private static BitmapImage ITrue = new BitmapImage(new Uri("/Static/02.png", UriKind.Relative));
        private static BitmapImage logo = new BitmapImage(new Uri("/Static/logo.png", UriKind.Relative));
        private bool dataMark = false;
        private short over = 0;
        private Stopwatch sw = new Stopwatch();
        private string markBarcode = null;
        private ILog iLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            InitializeComponent();

            #region 启动时串口最大化显示
            this.WindowState = WindowState.Maximized;
            Rect rc = SystemParameters.WorkArea; //获取工作区大小
            //this.Topmost = true;
            this.Left = 0; //设置位置
            this.Top = 0;
            this.Width = rc.Width;
            this.Height = rc.Height;
            #endregion

            Init();

            //连接ok
            if (config.DebugMode) // 调试模式
            {
                iLog.Info("--------------调试模式开启-------------");
                if (mData && qPlc)
                {
                    plcDataOpService.WriteSignal(1f);
                    MessageText.Text = "调试启动正常！";
                    iLog.Info("调试启动正常！");
                    CycleDataRead();
                }
                else
                {
                    plcDataOpService.WriteSignal(2f);
                    MessageText.Text = "调试启动异常！";
                    iLog.Info("调试启动异常！");
                }
            }
            else  // 正式运行环境
            {
                iLog.Info("--------------正式模式开启-------------");
                if (mData && mPlc && qPlc && babyLIN && sPort)
                {
                    plcDataOpService.WriteSignal(1f);
                    MessageText.Text = "启动正常！";
                    iLog.Info("启动正常！");
                    CycleDataRead();
                }
                else
                {
                    plcDataOpService.WriteSignal(2f);
                    MessageText.Text = "启动异常！";
                    iLog.Info("启动异常！");
                }
            }


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

            using (var sr = File.OpenText("C:\\config\\DLTAddress.json"))
            {
                string JsonStr1 = sr.ReadToEnd();
                if (address == null)
                {
                    address = JsonConvert.DeserializeObject<PLCAddress>(JsonStr1);
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
            plcDataOpService = new PlcDataOpService(config, address);
            qPlcDataOpService = new PlcDataOpService(config, address);
            linDataOpService = new LINDataOpService(config);

            try
            {
                babyLIN = linDataOpService.Initial();
            }
            catch (Exception ex)
            {
                iLog.Error(ex.Message);
            }

            mData = mesDataOpService.GetConnection();
            sPort = resDataOpService.GetConnection();
            mPlc = plcDataOpService.GetConnectionFx5u();
            qPlc = qPlcDataOpService.GetConnectionQ();

            LinImage.Source = (babyLIN ? ITrue : IFalse);
            FxPLCImage.Source = (mPlc ? ITrue : IFalse);
            QPLCImage.Source = (qPlc ? ITrue : IFalse);
            DataImage.Source = (mData ? ITrue : IFalse);
            PortImage.Source = (sPort ? ITrue : IFalse);
            Logo.Source = logo;
        }

        private void CycleDataRead()
        {

            timer = new DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                //读取BarCode
                string barCode = qPlcDataOpService.ReadBarCode();
                //初始化状态
                if (barCode != null && !barCode.Equals("") && !barCode.Equals(markBarcode))
                {

                    markBarcode = barCode;
                    dataMark = false;
                }

                codeText.Text = barCode.Trim();
                //读取型号
                LRType type = plcDataOpService.ReadType();
                switch (type)
                {
                    case LRType.Left:
                        TypeText.Text = "左驾";
                        break;
                    case LRType.Right:
                        TypeText.Text = "右驾";
                        break;
                }


                #region 读取Heater信号
                short heater = plcDataOpService.ReadSignal(SignalType.HeaterSignal);
                if (heater != -1)
                {
                    try
                    {
                        switch (heater)
                        {
                            case 0:
                                // 0 发送停止加热 00 00 00 00 00 00 00 00
                                linDataOpService.SendCmd(type, CmdType.stop);
                                plcDataOpService.WriteHeaterBack();
                                break;
                            case 1:
                                //1 发送驱动加热 00 00 ?? ?? 00 00 00 00 // ??->E3  L  R
                                linDataOpService.SendCmd(type, CmdType.start);
                                break;
                            default: break;
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageText.Text = ex.Message;
                        iLog.Error(ex.Message);
                    }

                }
                #endregion

                #region 读取Res信号
                short res = plcDataOpService.ReadSignal(SignalType.ResSignal);
                if (res == 1)
                {
                    try
                    {
                        float fData = resDataOpService.ReadData();
                        //write PLC data
                        if (fData != -1.1f)
                        {
                            plcDataOpService.WriteRes(fData);
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageText.Text = ex.Message;
                        iLog.Error(ex.Message);
                    }
                }
                #endregion

                #region 读取存储信号
                over = plcDataOpService.ReadSignal(SignalType.OverSignal);
                if (over == 1 && !dataMark)
                {


                    if (barCode != null && barCode.Equals(""))
                    {
                        dataMark = true;
                        // 读取测试数据资料
                        mesInfo = plcDataOpService.GetInfo(barCode, type);
                        // 存储MES数据
                        bool t = mesDataOpService.DataInsert(mesInfo);
                        if (!t) // 存储失败继续下一次存储直到成功
                        {
                            dataMark = false;
                        }
                        else
                        {
                            // 回写PLC存储状态 1:成功
                            plcDataOpService.WriteOverBack();
                            MessageText.Text = "该产品检测数据已存入数据库！";
                            iLog.Info(string.Format("该产品{0}检测数据已存入数据库！", barCode.Trim()));
                        }
                    }
                }
                #endregion


            };
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Start();
        }

        private void ShowTimer1(object sender, EventArgs e)
        {
            this.TM.Text = " ";
            //获得年月日 
            this.TM.Text += DateTime.Now.ToString("yyyy年MM月dd日");   //yyyy年MM月dd日 
            this.TM.Text += "                  \r\n       ";
            //获得时分秒 
            this.TM.Text += DateTime.Now.ToString("HH:mm");
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
            plcDataOpService.WriteSignal(2f);
            mesDataOpService.Close();
            resDataOpService.Close();
            plcDataOpService.Close();
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            iLog.Info("重新连接开始");
            plcDataOpService.WriteSignal(2f);
            if (timer != null && timer.IsEnabled)
            {
                timer.Stop();
                timer = null;
            }
            mesDataOpService.Close();
            resDataOpService.Close();
            plcDataOpService.Close();
            qPlcDataOpService.Close();
            linDataOpService.DisConnect();
            mesDataOpService = null;
            resDataOpService = null;
            plcDataOpService = null;
            linDataOpService = null;
            qPlcDataOpService = null;
            config = null;

            Init();
            //连接ok
            if (config.DebugMode) // 调试模式
            {
                iLog.Info("--------------调试模式开启-------------");
                if (mData && qPlc)
                {
                    plcDataOpService.WriteSignal(1f);
                    MessageText.Text = "调试启动正常！";
                    iLog.Info("调试启动正常！");
                    CycleDataRead();
                }
                else
                {
                    plcDataOpService.WriteSignal(2f);
                    MessageText.Text = "调试启动异常！";
                    iLog.Info("调试启动异常！");
                }
            }
            else  // 正式运行环境
            {
                iLog.Info("--------------正式模式开启-------------");
                if (mData && mPlc && qPlc && babyLIN && sPort)
                {
                    plcDataOpService.WriteSignal(1f);
                    MessageText.Text = "启动正常！";
                    iLog.Info("启动正常！");
                    CycleDataRead();
                }
                else
                {
                    plcDataOpService.WriteSignal(2f);
                    MessageText.Text = "启动异常！";
                    iLog.Info("启动异常！");
                }
            }
        }

        /// <summary>
        /// 左加热
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_LeftHeater(object sender, RoutedEventArgs e)
        {
            try
            {
                if (babyLIN)
                {
                    linDataOpService.SendCmd(LRType.Left, CmdType.start);
                    MessageText.Text = "左加热报文发送成功！";

                }
                else
                {
                    MessageText.Text = "未连接Lin！";
                    iLog.Error("未连接Lin！");
                }
            }
            catch (Exception ex)
            {
                MessageText.Text = ex.Message;
            }
        }

        /// <summary>
        /// 右加热
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_RightHeater(object sender, RoutedEventArgs e)
        {
            try
            {
                if (babyLIN)
                {
                    linDataOpService.SendCmd(LRType.Right, CmdType.start);
                    MessageText.Text = "右加热报文发送成功！";

                }
                else
                {
                    MessageText.Text = "未连接Lin！";
                    iLog.Error("未连接Lin！");
                }
            }
            catch (Exception ex)
            {
                MessageText.Text = ex.Message;
            }
        }

        /// <summary>
        /// 停止加热
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_StopHeater(object sender, RoutedEventArgs e)
        {
            try
            {
                if (babyLIN)
                {
                    linDataOpService.SendCmd(LRType.Left, CmdType.stop);
                    MessageText.Text = "停止加热报文发送成功！";
                }
                else
                {
                    MessageText.Text = "未连接Lin！";
                    iLog.Error("未连接Lin！");
                }
            }
            catch (Exception ex)
            {
                MessageText.Text = ex.Message;
            }
        }

        /// <summary>
        /// 读取电阻值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_ReadRes(object sender, RoutedEventArgs e)
        {
            //float t = qPlcDataOpService.test();
            //MessageText.Text = t.ToString();
            //string test = qPlcDataOpService.ReadBarCode();
            //MessageText.Text = test.Trim();
            //iLog.Info(test.Trim() + "12");

            try
            {
                if (sPort)
                {
                    float fData = resDataOpService.ReadData();
                    MessageText.Text = "电阻值为：" + fData.ToString() + "Ω";
                }
                else
                {
                    MessageText.Text = "未连接电阻计！";
                    iLog.Error("未连接电阻计！");
                }
            }
            catch (Exception ex)
            {
                MessageText.Text = ex.Message;
            }
        }
    }
}
