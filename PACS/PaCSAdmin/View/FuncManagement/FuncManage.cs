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
    public partial class FuncManage : Form
    {
        public FuncPresenter funcPresenter = null;
        public FuncManage()
        {
            InitializeComponent();
            funcPresenter = new FuncPresenter();
        }       

        private void FuncManage_Load(object sender, EventArgs e)
        {
            try
            {
                lbStatus.Text = "";
                groupBox2.Text = "功能详细信息";
                funcPresenter.AddTree(treeView1, -1, (TreeNode)null);
                treeView1.ExpandAll();//默认展开所有节点  
                funcPresenter.GetParentNode(cmbBox);
            }
            catch (Exception funce)
            {
                MessageBox.Show(this.Parent,"System error"+funce.Message);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                groupBox2.Text = "功能详细信息";
               
                if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null)
                {
                    btnDel.Enabled = true;
                    tbName.Text = treeView1.SelectedNode.Text;
                    tbId.Text = treeView1.SelectedNode.Tag.ToString().Split('$')[0];//controlid$id$lvl  controlid
                    cmbBox.SelectedValue = treeView1.SelectedNode.Parent.Tag.ToString().Split('$')[1];//controlid$id$lvl  id
                    tbFuncId.Text = treeView1.SelectedNode.Tag.ToString().Split('$')[1];//controlid$id$lvl  id

                    GetRoleListView(treeView1.SelectedNode.Tag.ToString().Split('$')[1]);
                }else
                {
                    btnDel.Enabled = false;
                }
            }
            catch (Exception treeViewSelect)
            {               
                MessageBox.Show(this.Parent,"System error"+treeViewSelect.Message);
            }
        }

        public void GetRoleListView(string funcid)
        {
            try
            {
                DataTable dt = funcPresenter.GetRoleListByFuncId(funcid);
                roleListView.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem(dt.Rows[i]["rolename"].ToString(), dt.Rows[i]["roleid"].ToString());
                    //string[]的长度和listview的列数要一致
                    roleListView.Items.Add(lvi);
                }
            }
            catch (Exception GetAuthListView)
            {
                MessageBox.Show(this.Parent, "System error" + GetAuthListView.Message);
            }
        }   

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {             
                groupBox2.Text = "新增功能";
                tbName.Text = "";
                tbId.Text = "";
                cmbBox.SelectedValue = treeView1.SelectedNode.Tag.ToString().Split('$')[1];//controlid$id$lvl  id  
                lbLvl.Text = (int.Parse(treeView1.SelectedNode.Tag.ToString().Split('$')[2]) + 1).ToString();//controlid$id$lvl  lvl  
            }
            catch (Exception treeViewAdd)
            {              
                MessageBox.Show(this.Parent,"System error"+treeViewAdd.Message);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {      
                string id = treeView1.SelectedNode.Tag.ToString().Split('$')[1];//controlid$id  id  
                string text = treeView1.SelectedNode.Text;
                DialogResult dr = MessageBox.Show("您确认删除 [" + text + "] 吗！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    int result = funcPresenter.DelNode(id);
                    if (result <= 0)
                        MessageBox.Show("删除失败");
                    else
                    {
                        if (treeView1.Nodes.Count > 0)
                            treeView1.Nodes.Clear();
                        funcPresenter.AddTree(treeView1, -1, (TreeNode)null);
                        treeView1.ExpandAll();//默认展开所有节点 
                        funcPresenter.GetParentNode(cmbBox);
                    }
                }
                else
                    return;
            }
            catch (Exception treeViewDel)
            {
                
                MessageBox.Show(this.Parent,"System error"+treeViewDel.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string id = "";
                if (treeView1.SelectedNode != null)
                    id = treeView1.SelectedNode.Tag.ToString().Split('$')[1];//controlid$id  id  
                string pId = cmbBox.SelectedValue.ToString();
                string nodeName = tbName.Text.Trim();
                string ctrlId = tbId.Text.Trim();
                string flag = groupBox2.Text;
                string level = lbLvl.Text;

                if (string.IsNullOrEmpty(nodeName))
                {
                    lbStatus.Text = "请输入 功能名称！";
                    tbName.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ctrlId))
                {
                    lbStatus.Text = "请输入 功能控件ID！";
                    tbId.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(pId))
                {
                    lbStatus.Text = "请选择 上层功能！";
                    cmbBox.Focus();
                    return;
                }

                Function func = new Function();
                func.Id = id;
                func.Name = nodeName;
                func.Pid = pId;
                func.Controlid = ctrlId;
                func.Lvl = level;

                int result = funcPresenter.SaveNode(func, flag);
                if (result <= 0)
                    MessageBox.Show("保存失败");
                else
                {
                    if (treeView1.Nodes.Count > 0)
                        treeView1.Nodes.Clear();
                    funcPresenter.AddTree(treeView1, -1, (TreeNode)null);
                    treeView1.ExpandAll();//默认展开所有节点 
                    groupBox2.Text = "功能详细信息";
                    lbStatus.Text = "";
                    funcPresenter.GetParentNode(cmbBox);
                }
            }
            catch (Exception treeViewSave)
            {
                
                MessageBox.Show(this.Parent,"System error"+treeViewSave.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count > 0)
                treeView1.Nodes.Clear();
            funcPresenter.AddTree(treeView1, -1, (TreeNode)null);
        }

        private void label3_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("组名为Page名.组名;\r模块名称为单个单词，按钮为单词.Add", label3,2000);
        }
    }
}
