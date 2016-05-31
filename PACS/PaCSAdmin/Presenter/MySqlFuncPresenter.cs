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
    public class MySqlFuncPresenter
    {
        public void AddTree(TreeView tv,int ParentID, TreeNode pNode)
        {
            string sql = "select id, pid, name,controlid,lvl from pacs_function where delflag ='0'";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            TreeNode tn1 = new TreeNode();
            DataView dvTree = new DataView(dt);
            //过滤ParentID,得到当前的所有子节点
            dvTree.RowFilter = "[pid] = " + ParentID;
            foreach (DataRowView Row in dvTree)
            {
                if (pNode == null)
                {    //'?添加根节点

                    tn1.Text = Row["name"].ToString();
                    tn1.Tag = Row["controlid"].ToString() + "$" + Row["id"].ToString() + "$" + Row["lvl"].ToString();
                    tv.Nodes.Add(tn1);
                    tn1.ExpandAll();
                    AddTree(tv,Int32.Parse(Row["id"].ToString()), tn1);    //再次递归
                }
                else
                {   //添加当前节点的子节点
                    TreeNode tn2 = new TreeNode();
                    tn2.Text = Row["name"].ToString();
                    tn2.Tag = Row["controlid"].ToString() + "$" + Row["id"].ToString() + "$" + Row["lvl"].ToString();
                    pNode.Nodes.Add(tn2);
                    tn1.ExpandAll();
                    AddTree(tv,Int32.Parse(Row["id"].ToString()), tn2);    //再次递归
                }
            }
            tv.ExpandAll();
        }

        public void GetParentNode(ComboBox cmbBox)
        {
            //string sql = "select id,name from pacs_function where id in( select distinct(pid) from pacs_function)";
            string sql = "select id,name from pacs_function where delflag ='0'";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            
            cmbBox.ValueMember = "id";
            cmbBox.DisplayMember = "name";
            cmbBox.DataSource = dt;
        }

        public int SaveNode(Function func,string flag)
        {
            int i = 0;
            if (flag.Equals("新增功能"))
            {
                string sql = "insert into pacs_function(id,pid,name,controlid,lvl,createuser) select nvl(max(id),0)+1,:pid,:name,:controlid,:lvl,:createuser from pacs_function";
                OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":pid", OracleType.VarChar, 20), 
                new OracleParameter(":name", OracleType.VarChar, 30),
                new OracleParameter(":controlid", OracleType.VarChar,50),
                new OracleParameter(":lvl", OracleType.VarChar,2),
                new OracleParameter(":createuser", OracleType.VarChar,20)
                };
                cmdParam[0].Value = func.Pid;
                cmdParam[1].Value = func.Name;
                cmdParam[2].Value = func.Controlid;
                cmdParam[3].Value = func.Lvl;
                cmdParam[4].Value = PaCSAdminTool.LoginUserId; ;

                i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            }else
            {
                string sql = "update pacs_function set pid = :pid,name = :name,controlid = :controlid,updatedate=to_char(sysdate,'yyyyMMdd'),updatetime =to_char(sysdate,'hh24miss'),updateuser='" + PaCSAdminTool.LoginUserId + "' where id=:id";
                OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":pid", OracleType.VarChar, 20), 
                new OracleParameter(":name", OracleType.VarChar, 30),
                new OracleParameter(":controlid", OracleType.VarChar,50),
                new OracleParameter(":id", OracleType.VarChar,20)
                };
                cmdParam[0].Value = func.Pid;
                cmdParam[1].Value = func.Name;
                cmdParam[2].Value = func.Controlid;
                cmdParam[3].Value = func.Id;

                i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            }
            
            return i;
        }

        public int DelNode(string id)
        {
            int i = 0;
            string sql = "update pacs_function set delflag='1',updatedate=to_char(sysdate,'yyyyMMdd'),updatetime =to_char(sysdate,'hh24miss'),updateuser='" + PaCSAdminTool.LoginUserId + "' where id=:id";
            //string sql = "delete pacs_function where id=:id";
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":id", OracleType.VarChar, 20)
            };
            cmdParam[0].Value = id;

            i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            return i;
        }

        public DataTable GetRoleListByFuncId(string funcid)
        {
            string sql = "select u.roleid roleid,v.name rolename from pacs_role_function u,pacs_role v where u.functionid=:functionid and u.roleid = v.id and v.delflag='0'";
            OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":functionid", OracleType.VarChar, 20)
            };
            cmdParam[0].Value = funcid;

            DataTable dt = OracleHelper.ExecuteDataTable(sql, cmdParam);
            return dt;
        }
    }
}
