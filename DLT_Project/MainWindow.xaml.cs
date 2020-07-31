using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace DLT_Project
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Int32 PlcHand;

        public MainWindow()
        {
            InitializeComponent();

            //string SqlConnectionStatement = "server=localhost;database=CylTest;integrated security=SSPI";
            //SqlConnection conn = new SqlConnection(SqlConnectionStatement);//声明一个SqlConnection对象
            //conn.Open();
            //string sql = @"insert into Data values ('testBarcode1',4,5,4,5,10,4,2)";
            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = sql;
            //cmd.Connection = conn;
            //cmd.ExecuteNonQuery();
            //conn.Close();
        }

    }
}
