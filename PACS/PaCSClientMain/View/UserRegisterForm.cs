using PaCSClientMain.Presenter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PaCSTools;
using DevExpress.XtraEditors;

namespace PaCSClientMain.View
{
    public partial class UserRegisterForm : Form
    {
        private LoginPresenter loginPresenter = null;
        private string ip = "";
        public UserRegisterForm()
        {
            InitializeComponent();
            loginPresenter = new LoginPresenter();
        }

        private void UserRegisterForm_Load(object sender, EventArgs e)
        {
            try
            {
                tbName.Focus();

                ip = PaCSGlobal.GetClientIp();

                string[] ipBlocks = ip.Split('.');//
                if(ipBlocks[0].Equals("109"))
                {
                    if (ipBlocks[1].Equals("116"))
                    {
                        tbFct.Text = "SSDP";
                        tbFct.Tag = "C660A";

                        int thirdBlock = int.Parse(ipBlocks[2]);//
                        if (thirdBlock < 199 || thirdBlock == 201 || thirdBlock == 223)
                        {
                            tbVendor.Text = "SSDP";
                            tbVendor.Tag = "C660";
                        }
                        else if (thirdBlock == 214 || thirdBlock == 232)
                        {
                            tbVendor.Text = "阿科帝斯";
                            tbVendor.Tag = "L1073X";
                        }
                        else
                        {
                            DataTable dt = loginPresenter.GetVendorByIP("109.116." + ipBlocks[2] + ".", tbFct.Tag == null ? "" : tbFct.Tag.ToString());
                            if (dt.Rows.Count > 0)
                            {
                                tbVendor.Text = dt.Rows[0]["vend_nm_cn"].ToString();
                                tbVendor.Tag = dt.Rows[0]["vend_code"].ToString();
                            }
                        }
                    }
                    else if(ipBlocks[1].Equals("119"))
                    {
                        tbFct.Text = "SESC";
                        tbFct.Tag = "C6H0A";

                        DataTable dt = loginPresenter.GetVendorByIP("109.119." + ipBlocks[2] + ".", tbFct.Tag == null ? "" : tbFct.Tag.ToString());
                        if (dt.Rows.Count > 0)
                        {
                            tbVendor.Text = dt.Rows[0]["vend_nm_cn"].ToString();
                            tbVendor.Tag = dt.Rows[0]["vend_code"].ToString();
                        }
                        else
                        { 
                            tbVendor.Text = "SESC";
                            tbVendor.Tag = "C6H0";
                        }
                    }
                }

                //loginPresenter.GetVenderList(cmbVender);
            }
            catch (Exception UserRegisterForm_Load)
            {              
                XtraMessageBox.Show(this.Parent, "System error[UserRegisterForm_Load]:" + UserRegisterForm_Load.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!tbPwd.Text.Trim().Equals(tbPwd2.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "两次密码输入不一致";
                    tbPwd2.Text = "";
                    tbPwd2.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(tbName.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "请输入 用户名！";
                    tbName.Focus();
                    return;
                }
                else
                {
                    int result0 = loginPresenter.CheckUser(tbName.Text.Trim());
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
                if (string.IsNullOrEmpty(tbPhone.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "请输入 电话！";
                    tbPhone.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(tbMail.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "请输入 Email！";
                    tbMail.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(tbRemark.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "请输入 业务描述！";
                    tbRemark.Focus();
                    return;
                }

                User user = new User();
                user.Name = tbName.Text.Trim();
                user.Password = tbPwd2.Text.Trim();
                user.FullName = tbFullName.Text.Trim();
                user.IdentityNo = "";
                user.Venderid = tbVendor.Tag == null ? "" : tbVendor.Tag.ToString();
                user.Vendername = tbVendor.Text;
                user.Mail = tbMail.Text.Trim();
                user.Phone = tbPhone.Text.Trim();
                user.Remark = tbRemark.Text.Trim();
                user.Ip = ip;
                user.Fct_code = tbFct.Tag == null ? "" : tbFct.Tag.ToString();

                int result = loginPresenter.SaveUser(user);
                if (result > 0)
                {
                    lbStatus.ForeColor = Color.Green;
                    lbStatus.Text = "提交成功！";
                    XtraMessageBox.Show("您的申请已提交，请等待审核！");
                    this.Close();
                }               
                else
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "提交失败！";
                }
            }
            catch (Exception saveUser)
            {
                XtraMessageBox.Show(this.Parent, "System error[saveUser]:" + saveUser.Message);
            }
        }

        private void tbName_Leave(object sender, EventArgs e)
        {
            try
            {
                int result = 0;
                if (!string.IsNullOrEmpty(tbName.Text))
                {
                    result = loginPresenter.CheckUser(tbName.Text.Trim());
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
                XtraMessageBox.Show(this.Parent, "System error[btnCheck_Click]:" + btnCheck_Click.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
