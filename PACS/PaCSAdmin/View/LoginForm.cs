using DevExpress.XtraEditors;
using PaCS.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PaCS
{
    public partial class LoginForm : XtraForm
    {
        private LoginPresenter lp = null;
        private MySqlLoginPresenter mlp = null;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                lp = new LoginPresenter();
                mlp = new MySqlLoginPresenter();
                toggleSwitch1.IsOn = lp.GetFactoryByIP();
            }
            catch (Exception)
            {
                
            }
        }

        private void tbPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//ENTER键，回车键：确定操作
            {
                btnLogin_Click(sender, e);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
              System.Environment.Exit(0);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                btnLogin.Enabled = false;

                string name = tbName.Text.Trim();
                string pwd = tbPwd.Text.Trim();
                string fct_code = toggleSwitch1.IsOn ? "C6H0A" : "C660A";
                string fct_name = toggleSwitch1.IsOn ? "SESC" : "SSDP";

                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("请输入 用户名!", "提示");
                    tbName.Focus();
                    btnLogin.Enabled = true;
                    return;
                }

                if (string.IsNullOrEmpty(pwd))
                {
                    MessageBox.Show("请输入 密码!", "提示");
                    tbPwd.Focus();
                    btnLogin.Enabled = true;
                    return;
                }

                User user = new User();//登录人信息
                user.Name = name;
                user.Password = pwd;

                User verfiedUser = mlp.VerifyUser(user);//验证后信息
                if (!string.IsNullOrEmpty(verfiedUser.Id))
                {
                    if (verfiedUser.UserType.Equals("Admin"))
                    {
                        if (!verfiedUser.Fct_code.Equals(fct_code))
                        {
                            MessageBox.Show("您没有登录" + fct_name + "的权限!", "提示");
                            btnLogin.Enabled = true;
                            return;
                        }
                    }
                    else if (verfiedUser.UserType.Equals("SuperAdmin"))
                    {

                    }
                    else
                    {
                        MessageBox.Show("您没有管理员权限!", "提示");
                        btnLogin.Enabled = true;
                        return;
                    }
                   
                    PaCSAdminTool.FactoryCode = fct_code;
                    PaCSAdminTool.FactoryName = fct_name;
                    PaCSAdminTool.LoginUserId = verfiedUser.Id;
                    PaCSAdminTool.WriteLoginLog("login_admin");

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("用户名或密码错误!", "提示");
                    tbPwd.Text = "";
                    tbPwd.Focus();
                    btnLogin.Enabled = true;
                    return;
                }
            }
            catch (Exception btnLogin_Click)
            {
                MessageBox.Show(this, "System error[btnLogin_Click]: " + btnLogin_Click.Message);
            }
        }

        
    }
}
