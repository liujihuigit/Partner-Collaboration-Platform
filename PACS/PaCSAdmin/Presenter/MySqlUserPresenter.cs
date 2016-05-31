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
    public class MySqlUserPresenter
    {
        public void AddTree(TreeView tv)
        {
            string sql = "select id,fullname,venderid,vendername from pacs_user where delflag ='0' and fct_code = '"+PaCSAdminTool.FactoryCode+"'";
            string sql2 = "select distinct venderid,vendername from pacs_user where delflag ='0' and fct_code = '" + PaCSAdminTool.FactoryCode + "'";

            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            DataTable dt2 = OracleHelper.ExecuteDataTable(sql2);

            DataView dvTree = new DataView(dt);
            DataView dvTree2 = new DataView(dt2);
            foreach (DataRowView Row2 in dvTree2)
            {
                TreeNode tn21 = new TreeNode();
                tn21.Text = Row2["vendername"].ToString();
                tn21.Tag = Row2["venderid"].ToString();
                tn21.ImageIndex = 2;
                tn21.SelectedImageIndex = 2;
                tv.Nodes.Add(tn21);

                dvTree.RowFilter = "[venderid] = '" + Row2["venderid"].ToString()+"'";
                foreach (DataRowView Row in dvTree)
                {
                    TreeNode tn1 = new TreeNode();
                    tn1.Text = Row["fullname"].ToString();
                    tn1.Tag = Row["id"].ToString();
                    tn1.ImageIndex = 0;
                    tn1.SelectedImageIndex = 0;
                    tn21.Nodes.Add(tn1);
                }
            }
            //tv.ExpandAll();
        }

        public User GetUserById(string id)
        {
            string sql = "select id,name,password,fullname,sex,identityno,mail,phone,venderid,vendername,remark,isChecked from pacs_user where id=:id and delflag ='0' and fct_code = '" + PaCSAdminTool.FactoryCode + "'";
           
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":id", OracleType.VarChar, 20)
                };
            cmdParam[0].Value = id;
            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
            User user = new User();
            if(dt!=null&&dt.Rows[0]!=null)
            {
                user.Id = dt.Rows[0]["id"].ToString();
                user.Name = dt.Rows[0]["name"].ToString();
                user.Password = dt.Rows[0]["password"].ToString();
                user.FullName = dt.Rows[0]["fullname"].ToString();
                user.Sex = dt.Rows[0]["sex"].ToString();
                user.IdentityNo = dt.Rows[0]["identityno"].ToString();
                user.Mail = dt.Rows[0]["mail"].ToString();
                user.Phone = dt.Rows[0]["phone"].ToString();
                user.Venderid = dt.Rows[0]["venderid"].ToString();
                user.Vendername = dt.Rows[0]["vendername"].ToString();
                user.Remark = dt.Rows[0]["remark"].ToString();
                user.IsChecked = dt.Rows[0]["isChecked"].ToString();
            } 
            return user;
        }

        public void GetVendorList(ComboBox cmbBox)
        {
            DataTable dt = PaCSAdminTool.GetVendor();

            cmbBox.ValueMember = "vend_code";
            cmbBox.DisplayMember = "vend_nm";
            cmbBox.DataSource = dt;
        }

        /// <summary>
        /// 新增用户时，检测用户名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int CheckUser(string name)
        {
            int flag = 0;
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

        public int SaveNode(User user,string flag)
        {
            int i = 0;
            if (flag.Equals("新增用户"))
            {
                string sql = "insert into pacs_user(id,name,password,fullname,mail,phone,venderid,vendername,remark,isChecked,fct_code,createuser) " +
                " select nvl(max(id),0)+1,:name,:password,:fullname,:mail,:phone,:venderid,:vendername,:remark,:isChecked,:fct_code,:createuser from pacs_user";
                OracleParameter[] cmdParam = new OracleParameter[] {                
                new OracleParameter(":name", OracleType.VarChar, 50),
                new OracleParameter(":password", OracleType.VarChar, 50),
                new OracleParameter(":fullname", OracleType.VarChar,50),
                new OracleParameter(":mail", OracleType.VarChar,50),
                new OracleParameter(":phone", OracleType.VarChar,30),
                new OracleParameter(":venderid", OracleType.VarChar,50),
                new OracleParameter(":vendername", OracleType.VarChar,50),
                new OracleParameter(":remark", OracleType.VarChar,100),
                new OracleParameter(":isChecked", OracleType.VarChar,10),
                new OracleParameter(":fct_code", OracleType.VarChar,10),
                   new OracleParameter(":createuser", OracleType.VarChar,10)
                };
                cmdParam[0].Value = user.Name;
                cmdParam[1].Value = user.Password;
                cmdParam[2].Value = user.FullName;
                cmdParam[3].Value = user.Mail;
                cmdParam[4].Value = user.Phone;
                cmdParam[5].Value = user.Venderid;
                cmdParam[6].Value = user.Vendername;
                cmdParam[7].Value = user.Remark;
                cmdParam[8].Value = user.IsChecked;
                cmdParam[9].Value = PaCSAdminTool.FactoryCode;
                cmdParam[10].Value = PaCSAdminTool.LoginUserId;

                i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            }else
            {
                string sql = "update pacs_user set name = :name,password = :password,fullname = :fullname,"+
                    " mail=:mail,phone=:phone,venderid=:venderid,vendername=:vendername,remark=:remark," +
                    " UPDATEDATE=to_char(sysdate,'yyyyMMdd'),UPDATETIME=to_char(sysdate,'hh24miss'),"+
                    " UPDATEUSER=:UPDATEUSER ,isChecked = :isChecked where id=:id";
                OracleParameter[] cmdParam = new OracleParameter[] {
                 new OracleParameter(":name", OracleType.VarChar, 50),
                new OracleParameter(":password", OracleType.VarChar, 50),
                new OracleParameter(":fullname", OracleType.VarChar,50),
                new OracleParameter(":mail", OracleType.VarChar,50),
                new OracleParameter(":phone", OracleType.VarChar,30),
                new OracleParameter(":venderid", OracleType.VarChar,50),
                new OracleParameter(":vendername", OracleType.VarChar,50),
                new OracleParameter(":remark", OracleType.VarChar,100),
                new OracleParameter(":UPDATEUSER", OracleType.VarChar,20),
                  new OracleParameter(":isChecked", OracleType.VarChar,10),
                new OracleParameter(":id", OracleType.VarChar,20),
                };
                cmdParam[0].Value = user.Name;
                cmdParam[1].Value = user.Password;
                cmdParam[2].Value = user.FullName;
                cmdParam[3].Value = user.Mail;
                cmdParam[4].Value = user.Phone;
                cmdParam[5].Value = user.Venderid;
                cmdParam[6].Value = user.Vendername;
                cmdParam[7].Value = user.Remark;
                cmdParam[8].Value = PaCSAdminTool.LoginUserId;
                cmdParam[9].Value = user.IsChecked;
                cmdParam[10].Value = user.Id;

                i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            }
            
            return i;
        }

        public int DelNode(string id)
        {
            int i = 0;
            string sql = "update pacs_user set delflag='1',updatedate=to_char(sysdate,'yyyyMMdd'),updatetime =to_char(sysdate,'hh24miss'),updateuser='" + PaCSAdminTool.LoginUserId + "' where id=:id";
            //string sql = "delete pacs_user where id=:id";
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":id", OracleType.VarChar, 20)
            };
            cmdParam[0].Value = id;

            i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            return i;
        }

        public DataTable GetRoleListByUserId(string userid)
        {
            string sql = "select u.roleid roleid,v.name rolename from pacs_user_role u,pacs_role v where u.userid=:userid and u.roleid = v.id and v.delflag='0' and v.fct_code = '" + PaCSAdminTool.FactoryCode + "' ";
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":userid", OracleType.VarChar, 20)
            };
            cmdParam[0].Value = userid;

            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
            return dt;
        }

        public DataTable GetAuthListByRoleId(List<string> roleidList)
        {
            DataTable dtReturn = new DataTable();
            foreach(string roleid in roleidList)
            {
                string sql = "select roleid, functionid from pacs_role_function where roleid=:roleid";
                OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20)
            };
                cmdParam[0].Value = roleid;

                DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
                dtReturn.Merge(dt);
            }

            return dtReturn;
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public DataTable RetrieveUserInfo(string status, string value)
        {
            StringBuilder sql = new StringBuilder("select id,name,password,fullname,sex,identityno,mail,phone,venderid,vendername,remark,isChecked from pacs_user where delflag ='0' and fct_code = '" + PaCSAdminTool.FactoryCode + "' ");

            switch (status)
            {
                case "用户名":
                    sql.Append("  and name like '%" + value + "%'");
                    break;
                case "真实姓名":
                    sql.Append("  and fullname like '%" + value + "%'");
                    break;
                case "用户状态":
                    sql.Append("  and ischecked like '%" + value + "%'");
                    break;
                default:
                    break;
            }

            DataTable dt = OracleHelper.ExecuteDataTable(sql.ToString());
            return dt;
        }
    }
}
