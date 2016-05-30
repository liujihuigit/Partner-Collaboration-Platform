using PaCSClientMain.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PaCSTools;

namespace PaCSClientMain.Presenter
{
    class LoginPresenter
    {
        /// <summary>
        /// 登录验证用户 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User VerifyUser(User user)
        {
            User loginUser = new User();
            string sql = "select id,name,password,fullname,identityno,mail,phone,venderid,vendername,remark,fct_code from pacs_user where name=:name and password = :password and isChecked = '1' and delflag = '0' ";

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
                loginUser.Name = dt.Rows[0]["name"].ToString();
                loginUser.Password = dt.Rows[0]["password"].ToString();
                loginUser.FullName = dt.Rows[0]["fullname"].ToString();
                loginUser.IdentityNo = dt.Rows[0]["identityno"].ToString();
                loginUser.Mail = dt.Rows[0]["mail"].ToString();
                loginUser.Phone = dt.Rows[0]["phone"].ToString();
                loginUser.Venderid = dt.Rows[0]["venderid"].ToString();
                loginUser.Vendername = dt.Rows[0]["vendername"].ToString();
                loginUser.Remark = dt.Rows[0]["remark"].ToString();
                loginUser.Fct_code = dt.Rows[0]["fct_code"].ToString();
            }

            return loginUser;
        }

        /// <summary>
        /// 注册时，检测用户名是否存在
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int CheckUser(string name)
        {
            int flag=0;
            string sql = "select count(*) from pacs_user where name=:name";

            OracleParameter[] cmdParam = new OracleParameter[] {                
                new OracleParameter(":name", OracleType.VarChar, 50)
                };

            cmdParam[0].Value = name;

            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
            if (dt != null && dt.Rows[0] != null)
            {
                flag = int.Parse(dt.Rows[0][0].ToString());
            }

            return flag;
        }

        public List<string> GetFuncListByUser(User user)
        {
            string sql = "select b.functionid functionid from pacs_user_role a,pacs_role_function b,pacs_role c " +
            " where a.userid=:id and a.roleid =b.roleid and a.roleid = c.id and c.fct_code = '" + user .Fct_code+ "' ";

            OracleParameter[] cmdParam = new OracleParameter[] {                
                new OracleParameter(":id", OracleType.VarChar,20)
                };

            cmdParam[0].Value = user.Id;
            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);

            List<string> funcList = new List<string>();

            foreach(DataRow dr in dt.Rows)
            {
                funcList.Add(dr[0].ToString());
            }
            return funcList;
        }

        public List<string> GetFactoryListByUser(User user)
        {
            string sql = "select distinct fct_code from pacs_user_role a,pacs_role b " +
            " where a.userid = '" + user.Id + "'  and a.roleid = b.id ";

            DataTable dt = OracleHelper.ExecuteDataTable(sql);

            List<string> fctList = new List<string>();

            foreach (DataRow dr in dt.Rows)
            {
                fctList.Add(dr[0].ToString());
            }
            return fctList;
        }

        /// <summary>
        /// 根据IP判断厂家
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public DataTable GetVendorByIP(string ip,string fctCode)
        {
            string sql = "select t.vend_nm_cn,t.vend_code from PACSM_MD_VEND t " +
                " where t.vend_ip like '%" + ip + "%' and fct_code ='" + fctCode + "' ";

            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            return dt;
        }

        public void GetVenderList(ComboBox cmbBox)
        {
            //string sql = "select lifnr_code,lifnr_desc from tb_lifnr";
            //DataTable dt = OracleHelper.ExecuteDataTable(sql);
            DataTable dt = PaCSGlobal.GetVendor();

            cmbBox.ValueMember = "vend_code";
            cmbBox.DisplayMember = "vend_nm";
            cmbBox.DataSource = dt;
        }

        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int SaveUser(User user)
        {
            int i = 0;
            string sql = "insert into pacs_user(id,name,password,fullname,mail,phone,venderid,vendername,remark,fct_code,ip) " +
            " select nvl(max(id),0)+1,:name,:password,:fullname,:mail,:phone,:venderid,:vendername,:remark,:fct_code,:ip from pacs_user";
            OracleParameter[] cmdParam = new OracleParameter[] {                
            new OracleParameter(":name", OracleType.VarChar, 50),
            new OracleParameter(":password", OracleType.VarChar, 50),
            new OracleParameter(":fullname", OracleType.VarChar,50),
            new OracleParameter(":mail", OracleType.VarChar,50),
            new OracleParameter(":phone", OracleType.VarChar,30),
            new OracleParameter(":venderid", OracleType.VarChar,50),
            new OracleParameter(":vendername", OracleType.VarChar,50),
            new OracleParameter(":remark", OracleType.VarChar,100),
            new OracleParameter(":fct_code", OracleType.VarChar,50),
              new OracleParameter(":ip", OracleType.VarChar,50)
            };
            cmdParam[0].Value = user.Name;
            cmdParam[1].Value = user.Password;
            cmdParam[2].Value = user.FullName;
            cmdParam[3].Value = user.Mail;
            cmdParam[4].Value = user.Phone;
            cmdParam[5].Value = user.Venderid;
            cmdParam[6].Value = user.Vendername;
            cmdParam[7].Value = user.Remark;
            cmdParam[8].Value = user.Fct_code;
            cmdParam[9].Value = user.Ip;

            i = OracleHelper.ExecuteNonQuery(sql, cmdParam);

            return i;
        }

        /// <summary>
        /// 登录日志
        /// </summary>
        /// <returns></returns>
        public int WriteLoginLog()
        {
            int i = 0;
            string sql = "insert into PACS_LOG(LOG_TYPE,CREATE_DATE,CREATE_TIME,CREATE_IP,CREATE_USER) " +
            " values('login',to_char(sysdate,'yyyyMMdd'),to_char(sysdate,'hh24miss'),:CREATE_IP,:CREATE_USER) ";
            OracleParameter[] cmdParam = new OracleParameter[] {                
            new OracleParameter(":CREATE_IP", OracleType.VarChar,100),
            new OracleParameter(":CREATE_USER", OracleType.VarChar,10)
            };
            cmdParam[0].Value = PaCSGlobal.GetClientIp();
            cmdParam[1].Value = PaCSGlobal.LoginUserInfo.Id;

            i = OracleHelper.ExecuteNonQuery(sql, cmdParam);

            return i;
        }
    }
}
