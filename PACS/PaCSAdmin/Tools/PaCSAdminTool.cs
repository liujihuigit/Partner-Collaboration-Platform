using PaCS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaCS.Tools
{
    public class PaCSAdminTool
    {
        public static string LoginUserId = "";
        public static string FactoryCode = "";
        public static string FactoryName = "";
        public static DataTable GetVendor()
        {
            string sqlVendor = "select distinct nvl(vend_code,'C660') vend_code,(select vend_nm_cn from pacsm_md_vend b where nvl(a.vend_code,'C660') = b.vend_code and fct_code = '"+PaCSAdminTool.FactoryCode+"') vend_nm from gmes20_line a";
            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor);
            return dtVendor;
        }

        /// <summary>
        /// 登录日志
        /// </summary>
        /// <returns></returns>
        public static int WriteLoginLog(string type)
        {
            int i = 0;
            string sql = "insert into PACS_LOG(LOG_TYPE,CREATE_DATE,CREATE_TIME,CREATE_IP,CREATE_USER) " +
            " values('" + type + "',to_char(sysdate,'yyyyMMdd'),to_char(sysdate,'hh24miss'),:CREATE_IP,:CREATE_USER) ";
            OracleParameter[] cmdParam = new OracleParameter[] {                
            new OracleParameter(":CREATE_IP", OracleType.VarChar,100),
            new OracleParameter(":CREATE_USER", OracleType.VarChar,10)
            };
            cmdParam[0].Value = GetClientIp();
            cmdParam[1].Value = LoginUserId;

            i = OracleHelper.ExecuteNonQuery(sql, cmdParam);

            return i;
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
    }
}
