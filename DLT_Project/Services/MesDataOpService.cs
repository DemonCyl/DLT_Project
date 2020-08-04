using DLT_Project.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HslCommunication.LogNet;

namespace DLT_Project.Services
{
    public class MesDataOpService
    {
        private ConfigData configData;
        private SqlConnection conn;
        private SqlCommand cmd;
        private ILogNet log;
        private string sql = @"insert into Data values (@Barcode,@Type,@WalkInLight,@Heater,@Bukle,@Safety,@SBROff,@SBROn)";
        //,@Type,@WalkInLight,@Heater,@Bukle,@Safety,@SBROff,@SBROn

        public MesDataOpService(ConfigData data, ILogNet log)
        {
            this.configData = data;
            this.log = log;
        }
        public bool GetConnection()
        {
            bool mark = false;
            StringBuilder sb = new StringBuilder("server=" + configData.DataIpAdress +
                ";database=" + configData.DataBaseName + "; uid=" + configData.Uid + ";pwd=" + configData.Pwd + "");
            if (conn == null)
            {
                try
                {
                    conn = new SqlConnection(sb.ToString());
                    conn.Open();
                    log.WriteInfo("数据库连接成功");
                    mark = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("数据库连接失败！原因为： " + ex.Message);
                    log.WriteError("数据库连接失败！原因为： " + ex.Message);
                    mark = false;
                }
            }
            return mark;
        }

        public bool DataInsert(MesInfo mesInfo)
        {

            bool mark = false;
            cmd = new SqlCommand(sql, conn);
            ///Parameters Set
            cmd.Parameters.AddWithValue("@Barcode", mesInfo.BarCode);
            cmd.Parameters.AddWithValue("@Type", mesInfo.Type);
            cmd.Parameters.AddWithValue("@WalkInLight", mesInfo.WalkInLight);
            cmd.Parameters.AddWithValue("@Heater", mesInfo.Heater);
            cmd.Parameters.AddWithValue("@Bukle", mesInfo.Bukle);
            cmd.Parameters.AddWithValue("@Safety", mesInfo.Safety);
            cmd.Parameters.AddWithValue("@SBROff", mesInfo.SBROff);
            cmd.Parameters.AddWithValue("@SBROn", mesInfo.SBROn);

            try
            {
                if (cmd.ExecuteNonQuery() != 0)
                {
                    mark = true;
                }
            }
            catch
            {
                log.WriteError("存储失败：" + mesInfo.ToString());
                mark = false;
            }
            return mark;
        }

        public void Close()
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}
