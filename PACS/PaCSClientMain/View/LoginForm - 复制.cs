using PaCSClientMain;
using PaCSClientMain.Presenter;
using PaCSClientMain.Tools;
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
    public partial class LoginForm2 : Form
    {
        private LoginPresenter lp = null;
        private ClientMainForm clientMain = null;
        private User verfiedUser = null;
        private string name = "";
        public LoginForm2(ClientMainForm clientMain)
        {
            InitializeComponent();
            this.clientMain = clientMain;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            lp = new LoginPresenter();
            LoadStoredUser();
        }

        /// <summary>
        /// 从注册表中获取登录成功过的用户
        /// </summary>
        private void LoadStoredUser()
        {
             tbName.Text = PaCSGlobal.GetRegistryValue("loginName", "");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);   
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                name = tbName.Text.Trim();
                string pwd = tbPwd.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    XtraMessageBox.Show("请输入 用户名!", "提示");
                    tbName.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(pwd))
                {
                    XtraMessageBox.Show("请输入 密码!", "提示");
                    tbPwd.Focus();
                    return;
                }

                User user = new User();//登录人信息
                user.Name = name;
                user.Password = pwd;

                verfiedUser = lp.VerifyUser(user);//验证后信息
                if (!string.IsNullOrEmpty(verfiedUser.Id))
                {
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    XtraMessageBox.Show("用户名或密码错误!", "提示");
                    return;
                }
            }
            catch (Exception btnLogin_Click)
            {
                XtraMessageBox.Show(this, "System error[btnLogin_Click]: " + btnLogin_Click.Message);
            }
        }

        private void tbName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//ENTER键，回车键：确定操作
            {
                tbPwd.Focus();
            }
        }

        private void tbPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//ENTER键，回车键：确定操作
            {
                btnLogin_Click(sender,e);
            }
        }

        private void registerNewUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                UserRegisterForm frmRegiser = new UserRegisterForm();
                DialogResult dg = frmRegiser.ShowDialog();
            }
            catch (Exception registerNewUser_LinkClicked)
            {
                XtraMessageBox.Show(this, "System error[registerNewUser_LinkClicked]: " + registerNewUser_LinkClicked.Message);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
               this.Invoke((MethodInvoker)delegate
           {
                  lbStatus.Text = "正在登陆...";
           });
            //获取用户权限列表，保存登录用户信息
            PaCSGlobal.FunctionDict = lp.GetFuncListByUser(verfiedUser);
            PaCSGlobal.LoginUserInfo = verfiedUser;
            //保存用户信息到注册表
            PaCSGlobal.SetRegistryValue("loginName", name, "");
            //初始化主界面菜单
            clientMain.Init();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lbStatus.Text = "登陆成功";
            });
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
