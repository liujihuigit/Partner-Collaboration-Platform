using PaCS.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCS
{
    class LoginPresenter
    {
        private string userid;
        /// <summary>
        /// 登录验证用户 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User VerifyUser(User user)
        {
            User loginUser = new User();
            string sql = "select id,name,password,fullname,fct_code,usertype from pacs_user where name=:name and password = :password and isChecked = '1' and delflag = '0' ";

            OracleParameter[] cmdParam = new OracleParameter[] {                
                new OracleParameter(":name", OracleType.VarChar, 50),
                new OracleParameter(":password", OracleType.VarChar,50)
                };

            cmdParam[0].Value = user.Name;
            cmdParam[1].Value = user.Password;

            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
            if (dt != null && dt.Rows.Count>0)
            {
                loginUser.Id = dt.Rows[0]["id"].ToString();
                userid = loginUser.Id;
                loginUser.Fct_code = dt.Rows[0]["fct_code"] == null ? "" : dt.Rows[0]["fct_code"].ToString();
                loginUser.UserType = dt.Rows[0]["usertype"] == null ? "" : dt.Rows[0]["usertype"].ToString();
            }

            return loginUser;
        }

        /// <summary>
        /// 根据IP判断厂家
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool GetFactoryByIP()
        {
            bool flag = false;
            string[] ipBlocks = PaCSAdminTool.GetClientIp().Split('.');//
            if (ipBlocks[0].Equals("109"))
            {
                if (ipBlocks[1].Equals("116"))
                {
                    flag = false;
                }
                else if (ipBlocks[1].Equals("119"))
                {
                    flag = true;
                }
            }

            return flag;
        }
    }
}
