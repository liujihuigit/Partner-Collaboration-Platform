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
    public partial class UserManage : Form
    {
        public UserPresenter userPresenter = null;
        public UserAuthority ua = null;
        private string userid = "";
        private bool isExpand = false;
        public UserManage()
        {
            InitializeComponent();
            userPresenter = new UserPresenter();
        }

        private void UserManage_Load(object sender, EventArgs e)
        {
            try
            {
                groupBoxInfo.Text = "用户详细信息";
                userPresenter.AddTree(treeView);
                userPresenter.GetVendorList(cmbVender);
                tbName.Focus();
                lbStatus.Text = "";
                cmbCondition.SelectedIndex = 0;
            }
            catch (Exception funce)
            {
                MessageBox.Show(this.Parent, "System error" + funce.Message);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                groupBoxInfo.Text = "用户详细信息";
                btnCheck.Visible = false;
                if (treeView.SelectedNode != null && treeView.SelectedNode.Parent != null)
                {
                    userid = treeView.SelectedNode.Tag.ToString();
                    User user = userPresenter.GetUserById(userid);
                    if (!user.Name.Equals(""))
                    {
                        tbName.Text = user.Name;
                        tbPwd.Text = user.Password;
                        tbPwd2.Text = user.Password;
                        tbFullName.Text = user.FullName;
                        tbRemark.Text = user.Vendername;
                        tbPhone.Text = user.Phone;
                        tbMail.Text = user.Mail;
                        tbRemark.Text = user.Remark;
                        cmbVender.SelectedValue = user.Venderid;//controlid$id  id
                        ckUser.Checked = user.IsChecked.Equals("0") ? false : true;
                        GetRoleListView();
                    } 
                }
                else
                    ClearText();
            }
            catch (Exception treeViewSelect)
            {
                MessageBox.Show(this.Parent, "System error" + treeViewSelect.Message);
            }
        }

        public void GetRoleListView()
        {
            try
            {
                DataTable dt = userPresenter.GetRoleListByUserId(userid);
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
            groupBoxInfo.Text = "新增用户";
            btnCheck.Visible = true;
            ClearText();
            if (treeView.SelectedNode != null && treeView.SelectedNode.Parent != null)
                cmbVender.SelectedValue = treeView.SelectedNode.Parent.Tag.ToString();
            tbName.Focus();
        }

        private void ClearText()
        {
            tbName.Clear();
            tbPwd.Clear();
            tbPwd2.Clear();
            tbFullName.Clear();
            tbRemark.Clear();
            tbPhone.Clear();
            tbMail.Clear();
            tbRemark.Clear();
            lbStatus.Text = "";     
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!tbPwd.Text.Trim().Equals(tbPwd2.Text.Trim()))
                {
                    lbStatus.Text = "两次密码输入不一致";
                    tbPwd2.Clear();
                    tbPwd2.Focus();
                    return;
                }
                string flag = groupBoxInfo.Text;


                if (string.IsNullOrEmpty(tbName.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "请输入 用户名！";
                    tbName.Focus();
                    return;
                }
                else if (flag.Equals("新增用户"))
                {
                    int result0 = userPresenter.CheckUser(tbName.Text.Trim());
                    if (result0 > 0)
                    {
                        lbStatus.ForeColor = Color.Red;
                        lbStatus.Text = "用户名已存在，请尝试其他名称！";
                        tbName.SelectAll();
                        tbName.Focus();
                        return;
                    }
                }

                if (string.IsNullOrEmpty(tbPwd2.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "请输入 密码！";
                    tbPwd2.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(tbFullName.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "请输入 真实姓名！";
                    tbFullName.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(tbRemark.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "请输入 备注！";
                    tbRemark.Focus();
                    return;
                }

                User user = new User();
                user.Id = userid;
                user.Name = tbName.Text.Trim();
                user.Password = tbPwd2.Text.Trim();
                user.FullName = tbFullName.Text.Trim();
                user.IdentityNo = "";
                user.Venderid = cmbVender.SelectedValue.ToString();
                user.Vendername = cmbVender.GetItemText(cmbVender.SelectedItem);
                user.Mail = tbMail.Text.Trim();
                user.Sex = "";
                user.Phone = tbPhone.Text.Trim();
                user.Remark = tbRemark.Text.Trim();
                user.IsChecked = ckUser.Checked ? "1" : "0";

                int result = userPresenter.SaveNode(user, flag);
                if (result <= 0)
                    MessageBox.Show("保存失败");
                else
                {
                    if (treeView.Nodes.Count > 0)
                        treeView.Nodes.Clear();
                    userPresenter.AddTree(treeView);
                    treeView.ExpandAll();//默认展开所有节点 
                    groupBoxInfo.Text = "功能详细信息";
                    lbStatus.ForeColor = Color.Green;
                    lbStatus.Text = "";
                }
            }
            catch (Exception saveUser)
            {             
                MessageBox.Show(this.Parent,"System error[saveUser]:"+saveUser.Message);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            { 
                string text = treeView.SelectedNode.Text;

                DialogResult dr = MessageBox.Show("您确认删除 [" + text + "] 吗！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    int result = userPresenter.DelNode(userid);
                    if (result <= 0)
                        MessageBox.Show("删除失败");
                    else
                    {
                        if (treeView.Nodes.Count > 0)
                            treeView.Nodes.Clear();
                        userPresenter.AddTree(treeView);
                        treeView.ExpandAll();//默认展开所有节点 
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

        private void btnFunc_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = userPresenter.GetRoleListByUserId(userid);
                List<string> roleidList = new List<string>();
                if(dt!=null&&dt.Rows.Count>0)
                {
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                        roleidList.Add(dt.Rows[i]["roleid"].ToString());
                    }
                    ua = new UserAuthority(roleidList);
                    ua.StartPosition = FormStartPosition.CenterParent;
                    ua.ShowDialog();
                }                
            }
            catch (Exception AuthEdit)
            {
                MessageBox.Show(this.Parent, "System error" + AuthEdit.Message);
            }
        }

        private void btnRetrieve_Click(object sender, EventArgs e)
        {
            string value = tbValue.Text.Trim();
            string status = cmbCondition.SelectedItem.ToString();
            DataTable dt = userPresenter.RetrieveUserInfo(status, value);
           
            cmbResult.ValueMember = "id";
            cmbResult.DisplayMember = "fullname";
            cmbResult.DataSource = dt;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                int result = 0;
                if (!string.IsNullOrEmpty(tbName.Text))
                {
                    result = userPresenter.CheckUser(tbName.Text.Trim());
                    if (result > 0)
                    {
                        lbStatus.ForeColor = Color.Red;
                        lbStatus.Text = "用户名已存在，请尝试其他名称！";
                        tbName.SelectAll();
                        tbName.Focus();
                        return;
                    }
                    else
                    {
                        lbStatus.Text = "用户名可以使用！";
                        lbStatus.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception btnCheck_Click)
            {
                MessageBox.Show(this.Parent, "System error[btnCheck_Click]:" + btnCheck_Click.Message);
            }
        }

        private void cmbResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbResult.SelectedIndex <0)
            {
                return;
            }
            string selectValue = cmbResult.SelectedValue.ToString();
           
            User user = userPresenter.GetUserById(selectValue);

            if (!user.Name.Equals(""))
            {
                userid = user.Id;
                tbName.Text = user.Name;
                tbPwd.Text = user.Password;
                tbPwd2.Text = user.Password;
                tbFullName.Text = user.FullName;
                tbRemark.Text = user.Vendername;
                tbPhone.Text = user.Phone;
                tbMail.Text = user.Mail;
                tbRemark.Text = user.Remark;
                cmbVender.SelectedValue = user.Venderid;//controlid$id  id
                ckUser.Checked = user.IsChecked.Equals("0") ? false : true;
            }
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            //  F4展开节点，F5刷新树
            if (e.KeyCode == Keys.F5)//F5键：刷新树操作
            {
                if (treeView.Nodes.Count > 0)
                    treeView.Nodes.Clear();
                userPresenter.AddTree(treeView);
            }

            if(e.KeyCode == Keys.F4)
            {
                if (isExpand)
                {
                    isExpand = false;
                    treeView.CollapseAll();//默认展开所有节点 
                }
                else
                {
                    isExpand = true;
                    treeView.ExpandAll();
                }
            }

        }



    }
}
