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
    public partial class RoleManage : Form
    {
        public RolePresenter rolePresenter = null;
        public RoleAuthority ra =null;
        public RoleUser ru = null;
        private string roleid="";
        public RoleManage()
        {
            InitializeComponent();
            rolePresenter = new RolePresenter();
        }

        private void RoleManage_Load(object sender, EventArgs e)
        {
            lbStatus.Text = "";
            groupBox1.Text = "角色详细信息";
            rolePresenter.AddTree(treeViewRole);
        }

        private void treeViewRole_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                groupBox1.Text = "角色详细信息";
                if (treeViewRole.SelectedNode != null)
                {
                    roleid = treeViewRole.SelectedNode.Tag.ToString().Split('$')[1];//remark$id  id  
                    tbName.Text = treeViewRole.SelectedNode.Text;
                    tbDesc.Text = treeViewRole.SelectedNode.Tag.ToString().Split('$')[0];//remark$id  remark
                    GetAuthListView();
                    GetUserListView();
                }
            }
            catch (Exception treeViewSelect)
            {

                MessageBox.Show(this.Parent, "System error" + treeViewSelect.Message);
            }
        }

        private void btnRoleAdd_Click(object sender, EventArgs e)
        {
            groupBox1.Text = "新增角色";
            tbName.Text = "";
            tbDesc.Text = "";
        }

        private void btnRoleDel_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("您确认删除吗！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    int result = rolePresenter.DelNode(roleid);
                    if (result <= 0)
                        MessageBox.Show("删除失败");
                    else
                    {
                        if (treeViewRole.Nodes.Count > 0)
                            treeViewRole.Nodes.Clear();
                        rolePresenter.AddTree(treeViewRole);
                    }
                }
                else
                    return;
            }
            catch (Exception treeViewDel)
            {
                MessageBox.Show(this.Parent, "System error" + treeViewDel.Message);
            }
        }

        private void btnRoleSave_Click(object sender, EventArgs e)
        {
            try
            {
                string flag = groupBox1.Text;
                string name = tbName.Text.Trim();
                string desc = tbDesc.Text.Trim();

                if(string.IsNullOrEmpty(name))
                {
                    lbStatus.Text = "请输入 名称！";
                    tbName.Focus();
                    return;
                }

                Role role = new Role();
                role.Id = roleid;
                role.Name = name;
                role.Remark = desc;
                int result = rolePresenter.SaveRole(role, flag);
                if (result <= 0)
                    MessageBox.Show("保存失败");
                else
                {
                    if (treeViewRole.Nodes.Count > 0)
                        treeViewRole.Nodes.Clear();
                    rolePresenter.AddTree(treeViewRole);
                    lbStatus.Text = "";
                }
            }
            catch (Exception RoleSave)
            {
                
                MessageBox.Show(this.Parent, "System error" + RoleSave.Message);
            }
        }

        private void btnAuthEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ra = new RoleAuthority(roleid);
                ra.StartPosition = FormStartPosition.CenterParent;
                ra.ShowDialog();
                if (ra.DialogResult == DialogResult.OK)
                {
                    GetAuthListView();
                }
            }
            catch (Exception AuthEdit)
            {
                MessageBox.Show(this.Parent, "System error" + AuthEdit.Message);
            }
        }

        private void btnAuthDel_Click(object sender, EventArgs e)
        {
            string key = authListView.SelectedItems[0].ImageKey;
            string text = authListView.SelectedItems[0].Text;
            DialogResult dr = MessageBox.Show("您确认删除 [" + text + "] 吗！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
             if (dr == DialogResult.OK)
             {
                 int flag = rolePresenter.DelAuth(roleid, key);
                 if (flag > 0)
                     GetAuthListView();
             }
             else
                 return;
        }

        

        public void GetAuthListView()
        {
            try
            {
                DataTable dt = rolePresenter.GetAuthList(roleid);
                authListView.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem(dt.Rows[i]["name"].ToString(), dt.Rows[i]["functionid"].ToString());
                    //string[]的长度和listview的列数要一致
                    authListView.Items.Add(lvi);
                }
            }
            catch (Exception GetAuthListView)
            {           
                 MessageBox.Show(this.Parent, "System error" + GetAuthListView.Message);
            }
        }

        public void GetUserListView()
        {
            try
            {
                DataTable dt = rolePresenter.GetUserList(roleid);
                userListView.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem(dt.Rows[i]["fullname"].ToString(),dt.Rows[i]["userid"].ToString() );
                    //string[]的长度和listview的列数要一致
                    userListView.Items.Add(lvi);
                }
            }
            catch (Exception GetAuthListView)
            {
                MessageBox.Show(this.Parent, "System error" + GetAuthListView.Message);
            }
        }

        private void btnUserAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ru = new RoleUser(roleid);
                ru.StartPosition = FormStartPosition.CenterParent;
                ru.ShowDialog();
                if (ru.DialogResult == DialogResult.OK)
                {
                    GetUserListView();
                }
            }
            catch (Exception AuthEdit)
            {
                MessageBox.Show(this.Parent, "System error" + AuthEdit.Message);
            }
        }

        private void btnUserDel_Click(object sender, EventArgs e)
        {
            string key = userListView.SelectedItems[0].ImageKey;
            string text = userListView.SelectedItems[0].Text;
            DialogResult dr = MessageBox.Show("您确认删除 [" + text + "] 吗！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                int flag = rolePresenter.DelUser(roleid, key);
                if (flag > 0)
                    GetUserListView();
            }
            else
                return;
        }
    
    }
}
