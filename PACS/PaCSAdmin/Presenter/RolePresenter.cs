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
    public class RolePresenter
    {

        public void AddTree(DataTable dtChecked,TreeView tv, int ParentID, TreeNode pNode)
        {
            string sql = "select id, pid, name,controlid from pacs_function where delflag ='0' ";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            TreeNode tn1 = new TreeNode();
            DataView dvTree = new DataView(dt);
            //过滤ParentID,得到当前的所有子节点
            dvTree.RowFilter = "[pid] = " + ParentID;
            foreach (DataRowView Row in dvTree)
            {
                if (pNode == null)
                {    //添加根节点

                    tn1.Text = Row["name"].ToString();
                    tn1.Tag = Row["controlid"].ToString() + "$" + Row["id"].ToString();
                    tv.Nodes.Add(tn1);
                    tn1.ExpandAll();
                    AddTree(dtChecked, tv, Int32.Parse(Row["id"].ToString()), tn1);    //再次递归
                }
                else
                {   //添加当前节点的子节点
                    TreeNode tn2 = new TreeNode();
                    tn2.Text = Row["name"].ToString();
                    tn2.Tag = Row["controlid"].ToString() + "$" + Row["id"].ToString();
                    
                    if (dtChecked.Select("functionid='" + Row["id"].ToString() + "'").Length>0)  
                    { 
                        tn2.Checked = true; 
                    } 
                    
                    pNode.Nodes.Add(tn2);
                    tn1.ExpandAll();
                    AddTree(dtChecked, tv, Int32.Parse(Row["id"].ToString()), tn2);    //再次递归
                }
            }
            tv.ExpandAll();
        }

        public void AddUserTree(DataTable dtChecked, TreeView tv)
        {
            //string sql = "select id,fullname,venderid,vendername from pacs_user where delflag ='0' and fct_code = '" + PaCSAdminTool.FactoryCode + "'";
            //string sql2 = "select distinct venderid,vendername from pacs_user where delflag ='0' and fct_code = '" + PaCSAdminTool.FactoryCode + "'";

            string sql = "select id,fullname,venderid,vendername from pacs_user where delflag ='0' ";
            string sql2 = "select distinct venderid,vendername from pacs_user where delflag ='0'";

            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            DataTable dt2 = OracleHelper.ExecuteDataTable(sql2);

            DataView dvTree = new DataView(dt);
            DataView dvTree2 = new DataView(dt2);
            foreach (DataRowView Row2 in dvTree2)
            {
                TreeNode tn21 = new TreeNode();
                tn21.Text = Row2["vendername"].ToString();
                tn21.Tag = Row2["venderid"].ToString();
                tv.Nodes.Add(tn21);

                dvTree.RowFilter = "[venderid] = '" + Row2["venderid"].ToString() + "'";
                foreach (DataRowView Row in dvTree)
                {
                    TreeNode tn1 = new TreeNode();
                    tn1.Text = Row["fullname"].ToString();
                    tn1.Tag = Row["id"].ToString();

                    if (dtChecked.Select("userid='" + Row["id"].ToString() + "'").Length > 0)
                    {
                        tn1.Checked = true;
                    } 

                    tn21.Nodes.Add(tn1);
                }
                tv.ExpandAll();
            }
        }

        public void AddTree(TreeView tv)
        {
            string sql = "select id,name,remark from pacs_role where delflag ='0' and fct_code = '" + PaCSAdminTool.FactoryCode + "'";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            
            DataView dvTree = new DataView(dt);

            foreach (DataRowView Row in dvTree)
            {
                TreeNode tn1 = new TreeNode();
                tn1.Text = Row["name"].ToString();
                tn1.Tag = Row["remark"].ToString() + "$" + Row["id"].ToString();
                tv.Nodes.Add(tn1);
                tn1.ExpandAll();
            }
            tv.ExpandAll();
        }

        public int SaveRole(Role role, string flag)
        {
            int i = 0;
            if (flag.Equals("新增角色"))
            {
                string sql = "insert into pacs_role(id,name,remark,createuser,fct_code) select nvl(max(to_number(id)),0)+1,:name,:remark,:createuser,:fct_code from pacs_role";
                OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":name", OracleType.VarChar, 30),
                new OracleParameter(":remark", OracleType.VarChar,50),
                new OracleParameter(":createuser", OracleType.VarChar,20),
                 new OracleParameter(":fct_code", OracleType.VarChar,20)
                };
                cmdParam[0].Value = role.Name;
                cmdParam[1].Value = role.Remark;
                cmdParam[2].Value = PaCSAdminTool.LoginUserId;
                cmdParam[3].Value = PaCSAdminTool.FactoryCode;

                i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            }
            else
            {
                string sql = "update pacs_role set name = :name,remark = :remark,updatedate=to_char(sysdate,'yyyyMMdd'),updatetime =to_char(sysdate,'hh24miss'),updateuser='" + PaCSAdminTool.LoginUserId + "' where id=:id";
                OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":name", OracleType.VarChar, 30),
                new OracleParameter(":remark", OracleType.VarChar,50),
                new OracleParameter(":id", OracleType.VarChar,20)
                };
                cmdParam[0].Value = role.Name;
                cmdParam[1].Value = role.Remark;
                cmdParam[2].Value = role.Id;

                i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            }

            return i;
        }

        public int DelNode(string id)
        {
            int i = 0;
            string sql = "update pacs_role set delflag='1',updatedate=to_char(sysdate,'yyyyMMdd'),updatetime =to_char(sysdate,'hh24miss'),updateuser='" + PaCSAdminTool.LoginUserId + "'  where id=:id";
            //string sql = "delete pacs_role where id=:id";
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":id", OracleType.VarChar, 20)
            };
            cmdParam[0].Value = id;

            i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            return i;
        }

        /// <summary>
        /// 获取可操作功能列表
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public DataTable GetAuthList(string roleid)
        {
            string sql = "select a.roleid roleid,a.functionid functionid,b.name name from pacs_role_function a,pacs_function b where a.roleid=:roleid and a.functionid = b.id and b.delflag='0'";
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20)
            };
            cmdParam[0].Value = roleid;

            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
            return dt;
        }
        
        public DataTable GetAuthListByRoleId(string roleid)
        {
            string sql = "select roleid, functionid from pacs_role_function where roleid=:roleid";
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20)
            };
            cmdParam[0].Value = roleid;

            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
            return dt;
        }

        /// <summary>
        /// 获取包含用户列表
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public DataTable GetUserList(string roleid)
        {
            string sql = "select a.roleid roleid,a.userid userid,b.fullname fullname from PACS_USER_ROLE a,pacs_user b where b.delflag != '1' and a.roleid=:roleid and a.userid = b.id";
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20)
            };
            cmdParam[0].Value = roleid;

            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
            return dt;
        }

        public DataTable GetUserByRoleId(string roleid)
        {
            string sql = "select userid,roleid from PACS_USER_ROLE where roleid=:roleid";
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20)
            };
            cmdParam[0].Value = roleid;

            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
            return dt;
        }


        public void SaveAuth(string roleid,DataTable dt)
        {
            string sql1 = "delete pacs_role_function where roleid = :roleid";
            OracleParameter[] cmdParam1 = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20)
                };
            cmdParam1[0].Value = roleid;

            int result1 = OracleHelper.ExecuteNonQuery(sql1, cmdParam1);

            DataTable dtMade = dt.DefaultView.ToTable(true, "roleid", "functionid");


            for (int i = 0; i < dtMade.Rows.Count; i++)
            {
                string sql2 = "insert into pacs_role_function(roleid,functionid) values(:roleid,:functionid)";
                OracleParameter[] cmdParam2 = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20),
                new OracleParameter(":functionid", OracleType.VarChar,20)
                };
                cmdParam2[0].Value = roleid;
                cmdParam2[1].Value = dtMade.Rows[i]["functionid"];

                int result2 = OracleHelper.ExecuteNonQuery(sql2, cmdParam2);
            }         
        }

        public void SaveUser(string roleid, DataTable dt)
        {
            string sql1 = "delete PACS_USER_ROLE where roleid = :roleid";
            OracleParameter[] cmdParam1 = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20)
                };
            cmdParam1[0].Value = roleid;

            int result1 = OracleHelper.ExecuteNonQuery(sql1, cmdParam1);

            DataTable dtMade = dt.DefaultView.ToTable(true, "userid", "roleid");


            for (int i = 0; i < dtMade.Rows.Count; i++)
            {
                string sql2 = "insert into PACS_USER_ROLE(userid,roleid) values(:userid,:roleid)";
                OracleParameter[] cmdParam2 = new OracleParameter[] {
                new OracleParameter(":userid", OracleType.VarChar, 20),
                new OracleParameter(":roleid", OracleType.VarChar,20)
                };
                cmdParam2[0].Value = dtMade.Rows[i]["userid"];
                cmdParam2[1].Value = roleid;

                int result2 = OracleHelper.ExecuteNonQuery(sql2, cmdParam2);
            }
        }

        public int DelAuth(string roleid,string funcid)
        {
            string sql1 = "delete pacs_role_function where roleid = :roleid and functionid = :functionid";
            OracleParameter[] cmdParam1 = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20),
                new OracleParameter(":functionid", OracleType.VarChar,20)
                };
            cmdParam1[0].Value = roleid;
            cmdParam1[1].Value = funcid;
            int result1 = OracleHelper.ExecuteNonQuery(sql1, cmdParam1);
            return result1;
        }

        public int DelUser(string roleid, string userid)
        {
            string sql1 = "delete pacs_user_role where roleid = :roleid and userid = :userid";
            OracleParameter[] cmdParam1 = new OracleParameter[] {
                new OracleParameter(":roleid", OracleType.VarChar, 20),
                new OracleParameter(":userid", OracleType.VarChar,20)
                };
            cmdParam1[0].Value = roleid;
            cmdParam1[1].Value = userid;
            int result1 = OracleHelper.ExecuteNonQuery(sql1, cmdParam1);
            return result1;
        }

        public bool IsParent(TreeNode tn)
        {
            return tn.Nodes.Count > 0;
        }
    }
}
