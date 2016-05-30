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
    public partial class RoleUser : Form
    {
        public RolePresenter rolePresenter = null;
        private string roleid = "";
        private DataTable dt = null;

        public RoleUser(string roleid)
        {
            InitializeComponent();
            rolePresenter = new RolePresenter();
            dt = new DataTable();
            this.roleid = roleid;
        }

        private void RoleUser_Load(object sender, EventArgs e)
        {
            try
            {
                //选择role_function中权限
                dt = rolePresenter.GetUserByRoleId(roleid);
                rolePresenter.AddUserTree(dt, treeView1);
                treeView1.ExpandAll();//默认展开所有节点                 
            }
            catch (Exception funce)
            {
                MessageBox.Show(this.Parent, "System error" + funce.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            rolePresenter.SaveUser(roleid, dt);
            this.Dispose();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (TreeNode treeNode in treeView1.Nodes)
            {
                CheckAllChildNodes(treeNode, ckBox.Checked);
            }
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
                    setChildNodeCheckedState(e.Node, true);
                    dt = dt.DefaultView.ToTable(true, "userid", "roleid");
                }
                else
                {
                    //取消节点选中状态之后，取消所有父节点的选中状态
                    setChildNodeCheckedState(e.Node, false);
                    //如果节点存在父节点，取消父节点的选中状态
                    if (e.Node.Parent != null)
                    {
                        setParentNodeCheckedState(e.Node, false);
                    }
                    dt = dt.DefaultView.ToTable(true, "userid", "roleid");
                }
            }
        }

        //取消节点选中状态之后，取消所有父节点的选中状态
        private void setParentNodeCheckedState(TreeNode currNode, bool state)
        {
            TreeNode parentNode = currNode.Parent;

            parentNode.Checked = state;
            if (currNode.Parent.Parent != null)
            {
                setParentNodeCheckedState(currNode.Parent, state);
            }
        }

        //选中节点之后，选中节点的所有子节点
        private void setChildNodeCheckedState(TreeNode currNode, bool state)
        {
            TreeNodeCollection nodes = currNode.Nodes;
            if (nodes.Count > 0)
            {
                foreach (TreeNode tn in nodes)
                {
                    tn.Checked = state;
                    if (state == true)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["roleid"] = roleid;//remark$id  id
                        //newRow["name"] = tn.Text;//remark$id  id
                        newRow["userid"] = tn.Tag.ToString();//remark$id  id
                        dt.Rows.Add(newRow);
                    }
                    else
                    {
                        DataColumn[] myPrimaryKey = new DataColumn[2];
                        myPrimaryKey[0] = dt.Columns["userid"];
                        myPrimaryKey[1] = dt.Columns["roleid"];
                        dt.PrimaryKey = myPrimaryKey;
                        object[] findTheseVals = new object[2];
                        findTheseVals[0] = tn.Tag.ToString();
                        findTheseVals[1] = roleid;
                        //string expr = "roleid='" + findTheseVals[0] + "' and functionid='" + findTheseVals[1] + "'";
                        //DataRow[] myRemoveRow = dt.Select(expr);
                        DataRow myRemoveRow = dt.Rows.Find(findTheseVals);
                        if(myRemoveRow!=null)
                            myRemoveRow.Delete();
                        dt.AcceptChanges();
                    }
                    if(tn.Nodes.Count>0)
                        setChildNodeCheckedState(tn, state);
                }
            }
            else
            {
                if (state == true)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["roleid"] = roleid;//remark$id  id
                    //newRow["name"] = currNode.Text;//remark$id  id
                    newRow["userid"] = currNode.Tag.ToString();//remark$id  id
                    dt.Rows.Add(newRow);
                }
                else
                {
                    DataColumn[] myPrimaryKey = new DataColumn[2];
                    myPrimaryKey[0] = dt.Columns["userid"];
                    myPrimaryKey[1] = dt.Columns["roleid"];
                    dt.PrimaryKey = myPrimaryKey;
                    DataRow myRemoveRow = dt.Rows.Find(new string[2] { currNode.Tag.ToString(),roleid  });//remark$id  id funcid
                    if (myRemoveRow != null)
                        myRemoveRow.Delete();
                    dt.AcceptChanges();
                }
            }
        }

        /// <summary>
        /// 查询treeNode节点下有多少节点被选中（递归实现，不受级数限制）
        /// </summary>
        /// <param name="tv"></param>
        /// <returns></returns>
        private int GetNodeChecked(TreeNode tv)
        {
            int x = 0;
            if (tv.Checked)
            {
                x++;
            }
            foreach (TreeNode item in tv.Nodes)
            {
                x += GetNodeChecked(item);

            }
            return x;

        }

        /// <summary>
        /// 查询TreeView下节点被checked的数目
        /// </summary>
        /// <param name="treev"></param>
        /// <returns></returns>
        private int GetTreeViewNodeChecked(TreeView treev)
        {
            int k = 0;
            foreach (TreeNode item in treev.Nodes)
            {
                k += GetNodeChecked(item);
            }
            return k;
        }


    }
}
