using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCSTools
{
    public class ComboxData
    {
        public string Text { set; get; }
        public string Value { set; get; }

        public override string ToString()
        {
            return Text;
        }
    }
    public partial class PaCSGlobal
    {
        public static User LoginUserInfo = new User();
        public static List<string> FunctionDict = new List<string>();

        /// <summary> 
        /// 看用户是否具有某个功能
        /// </summary>
        /// <param name="controlID"></param>
        /// <returns></returns>

        public static bool HasFunction(string funcID)
        {
            bool result = false;

            if (FunctionDict.Contains(funcID))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 获取厂家
        /// </summary>
        /// <returns></returns>
        public static DataTable GetVendor()
        {
            string sqlVendor = "select distinct nvl(vend_code,'C660') vend_code,(select vend_nm_cn from pacsm_md_vend b where nvl(a.vend_code,'C660') = b.vend_code and fct_code = '"+PaCSGlobal.LoginUserInfo.Fct_code+"') vend_nm from gmes20_line a";
            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor);
            return dtVendor;
        }


        /// <summary>
        /// 获取厂家
        /// </summary>
        /// <returns></returns>
        public static string GetVendorNameByCode(string code)
        {
            string sqlVendor = "select vend_nm_cn from pacsm_md_vend where vend_code = '" + code + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'";
            System.Data.DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor);
            return dtVendor.Rows[0][0].ToString();
        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static string GetServerDateTime(int Type)
        {
            string sqlString = "";
            switch (Type)
            {
                case 1: sqlString = "select to_char(sysdate,'yyyymmddhh24miss') from dual"; break;
                case 2: sqlString = "select to_char(sysdate,'yyyymmdd') from dual"; break;
                case 3: sqlString = "select to_char(sysdate,'yyyy-mm-dd') from dual"; break;
                case 4: sqlString = "select to_char(sysdate,'yyyy/mm/dd') from dual"; break;
                case 5: sqlString = "select to_char(sysdate,'hh24miss') from dual"; break;
                case 6: sqlString = "select to_char(sysdate,'yyyy/mm/dd hh24:mi:ss') from dual"; break;
                case 7: sqlString = "select to_char(sysdate,'yyyy-mm-dd hh24:mi:ss') from dual"; break;
            }
            OracleDataReader odr = OracleHelper.ExecuteReader(sqlString);
            if (odr.Read())
            {
                return odr.GetString(0);
            }
            else
                return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// 获取服务器时间 + -  天数
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>

        public static string GetServerDateTimeRange(string format, int range)
        {
            string sqlString = "";
            sqlString = " select to_char(sysdate " + range + ", 'yyyy" + format + "mm" + format + "dd') from dual ";

            OracleDataReader odr = OracleHelper.ExecuteReader(sqlString);
            if (odr.Read())
            {
                return odr.GetString(0);
            }
            else
                return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// 获取主机IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            string ip = "";
            string sql = " select sys_context('USERENV', 'IP_ADDRESS') vendIP from dual";
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql);
            ip = dtResult.Rows[0]["vendIP"].ToString();
            return ip;
        }

        public static void InitComPort(string module_name, string form_name,SerialPort[] ports)
        {
            int errorPort = 0;
            int m = ports.Length ; //com口个数
            int n = 0;
            try
            {
                string regPath = module_name + "\\" + form_name;

                foreach (SerialPort port in ports)
                {
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                }

                for (int i = 1; i < 17; i++)
                {
                    if (!string.IsNullOrEmpty(PaCSGlobal.GetRegistryValue("COM" + i, regPath)))
                    {
                        errorPort = i;

                        if (n >= m)
                        {
                            break;
                        }
                        ports[n].PortName = "COM" + i;
                        if (PaCSGlobal.GetRegistryValue("COM" + i, regPath).IndexOf(',')!=-1)
                            ports[n].BaudRate = int.Parse(PaCSGlobal.GetRegistryValue("COM" + i, regPath).Split(',')[0]);
                        else
                            ports[n].BaudRate = int.Parse(PaCSGlobal.GetRegistryValue("COM" + i, regPath));
                        ports[n].Open();

                        n++;
                    }
                }
            }
            catch (Exception e)
            {
                XtraMessageBox.Show("端口COM" + errorPort + "打开错误", "提示");
            }
        }


        /// <summary>
        /// 获取服务器与本地时间差
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetDateTimeDiff()
        {
            TimeSpan diff;

            DateTime date1 = DateTime.Parse(GetServerDateTime(7));//服务器时间
            DateTime date2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//本地时间
            diff = date1 - date2;
            return diff;
        }
     }
}
