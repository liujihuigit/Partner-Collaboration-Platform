using PaCSClientMain.Tools;
using PaCSTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCSClientMain.Presenter
{
    class MainPresenter
    {
        public void AddTree(List<string> list, TreeView tv, int ParentID, TreeNode pNode)
        {
            string sql = "select * from pacs_function where lvl !=  '3'";
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
                    AddTree(list, tv, Int32.Parse(Row["id"].ToString()), tn1);    //再次递归
                }
                else
                {   //添加当前节点的子节点
                    TreeNode tn2 = new TreeNode();
                    tn2.Text = Row["name"].ToString();
                    tn2.Tag = Row["controlid"].ToString() + "$" + Row["id"].ToString();

                    if (!list.Contains(Row["id"].ToString()))
                    {
                        tn2.ForeColor = SystemColors.Control;
                    }

                    pNode.Nodes.Add(tn2);
                    tn1.ExpandAll();
                    AddTree(list, tv, Int32.Parse(Row["id"].ToString()), tn2);    //再次递归
                }
            }
            tv.ExpandAll();
        }
        /// <summary>
        /// 一级菜单 :资源管理 Page
        /// </summary>
        /// <returns></returns>
        public DataTable GetMenuLvlOne()
        {
            string sql = "select id,name,controlid from pacs_function where lvl =  '1' and delflag ='0' order by decode(id,60004,60000,id)";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            return dt;
        }
        /// <summary>
        /// 二级菜单 :设备准备 Group
        /// </summary>
        /// <returns></returns>
        public DataTable GetMenuLvlTwo(string pid)
        {
            string sql = "select  id,name,controlid  from pacs_function where  lvl =  '2' and delflag ='0' and pid = '" + pid + "' order by 1 ";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            return dt;
        }
        /// <summary>
        /// 三级菜单 :JIG管理 MetalMask管理 Button
        /// </summary>
        /// <returns></returns>
        public DataTable GetMenuLvlThree(string pid)
        {
            string sql = "select  id,name,controlid  from pacs_function where  lvl =  '3' and delflag ='0' and pid = '" + pid + "' order by 1";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 登出日志
        /// </summary>
        /// <returns></returns>
        public int WriteLogOutLog()
        {
            int i = 0;
            string sql = "insert into PACS_LOG(LOG_TYPE,CREATE_DATE,CREATE_TIME,CREATE_IP,CREATE_USER) " +
            " values('logout',to_char(sysdate,'yyyyMMdd'),to_char(sysdate,'hh24miss'),:CREATE_IP,:CREATE_USER) ";
            OracleParameter[] cmdParam = new OracleParameter[] {                
            new OracleParameter(":CREATE_IP", OracleType.VarChar,100),
            new OracleParameter(":CREATE_USER", OracleType.VarChar,10)
            };
            cmdParam[0].Value = PaCSGlobal.GetClientIp();
            cmdParam[1].Value = PaCSGlobal.LoginUserInfo.Id;

            i = OracleHelper.ExecuteNonQuery(sql, cmdParam);

            return i;
        }

        public DataTable GetNotice()
        {
            string sql = "select content from pacs_notice where id =  1 and useflag = 1";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            return dt;
        }

    }
}
