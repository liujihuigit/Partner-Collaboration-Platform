using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCS
{
    public partial class RoleAuthority : Form
    {
        public FuncPresenter funcPresenter = null;
        public RolePresenter rolePresenter = null;
        private string roleid = "";
        private DataTable dt = null;

        public RoleAuthority(string roleid)
        {
            InitializeComponent();
            funcPresenter = new FuncPresenter();
            rolePresenter = new RolePresenter();
            dt = new DataTable();
            this.roleid = roleid;
        }

        private void RoleAuthority_Load(object sender, EventArgs e)
        {
            try
            {
                //选择role_function中权限
                dt = rolePresenter.GetAuthListByRoleId(roleid);
                rolePresenter.AddTree(dt,treeView1, -1, (TreeNode)null);
                treeView1.ExpandAll();//默认展开所有节点                 
            }
            catch (Exception funce)
            {
                MessageBox.Show(this.Parent, "System error" + funce.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            rolePresenter.SaveAuth(roleid, dt);
            this.Dispose();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ckBox_CheckedChanged(object sender, EventArgs e)
        {
            treeView1.Nodes[0].Checked = ckBox.Checked;
            CheckAllChildNodes(treeView1.Nodes[0],ckBox.Checked);
        }


        public void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;

                if (node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        /// <summary>
        /// 选择checkbox后，操作数据源DataTable（新增或移除）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByMouse)
            {
                if (e.Node.Checked)
                {
                    //勾选，若是父节点，则勾选该父节点下全部子节点
                    setNodeCheckedState(e.Node, true);
                    dt = dt.DefaultView.ToTable(true, "roleid", "functionid");
                }
                else
                {
                    //取消节点选中状态之后，取消所有父节点的选中状态
                    setNodeCheckedState(e.Node, false);
                    //如果节点存在父节点，取消父节点的选中状态
                    dt = dt.DefaultView.ToTable(true, "roleid", "functionid");
                }
            }
        }


        //选中节点之后，选中节点的所有子节点
        private void setNodeCheckedState(TreeNode currNode, bool state)
        {
            if (state == true)
            {
                DataRow newRow = dt.NewRow();
                newRow["roleid"] = roleid;//remark$id  id
                //newRow["name"] = currNode.Text;//remark$id  id
                newRow["functionid"] = currNode.Tag.ToString().Split('$')[1];//remark$id  id
                dt.Rows.Add(newRow);
            }
            else
            {
                DataColumn[] myPrimaryKey = new DataColumn[2];
                myPrimaryKey[0] = dt.Columns["roleid"];
                myPrimaryKey[1] = dt.Columns["functionid"];
                dt.PrimaryKey = myPrimaryKey;
                DataRow myRemoveRow = dt.Rows.Find(new string[2] { roleid, currNode.Tag.ToString().Split('$')[1] });//remark$id  id funcid
                if (myRemoveRow != null)
                    myRemoveRow.Delete();
                dt.AcceptChanges();
            }
        }
    }
}
