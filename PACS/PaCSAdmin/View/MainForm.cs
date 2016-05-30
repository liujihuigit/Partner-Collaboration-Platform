using DevExpress.XtraEditors;
using PaCS.Tools;
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
    public partial class MainForm : Form
    {
        private TabPage tpUser = null;
        private TabPage tpRole = null;
        private TabPage tpFunc = null;
        private UserManage userform = null;
        private RoleManage roleform = null;
        private FuncManage funcform = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
             {
                Init();
                this.Text = PaCSAdminTool.FactoryName;
                this.Show();
            }
            catch (Exception MainForm_Load)
            {
                MessageBox.Show(this, "程序启动失败，请联系管理员 [MainForm_Load]: " + MainForm_Load.Message);
                System.Environment.Exit(0);   
            }
        }

        private void Init()
        {
            userform = new UserManage();
            roleform = new RoleManage();
            funcform = new FuncManage();

            userform.TopLevel = false;
            userform.BackColor = Color.White;
            userform.Dock = DockStyle.Fill;
            //form.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            userform.FormBorderStyle = FormBorderStyle.None;
            userform.Show();

            roleform.TopLevel = false;
            roleform.BackColor = Color.White;
            roleform.Dock = DockStyle.Fill;
            //form.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            roleform.FormBorderStyle = FormBorderStyle.None;
            roleform.Show();

            funcform.TopLevel = false;
            funcform.BackColor = Color.White;
            funcform.Dock = DockStyle.Fill;
            //form.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            funcform.FormBorderStyle = FormBorderStyle.None;
            funcform.Show();
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            if (!IsExisted("user_manange", tabControl))
            {
                tpUser = new TabPage("用户管理");
                tpUser.Name = "user_manange";
                tabControl.TabPages.Add(tpUser);

                tpUser.Controls.Add(userform);
            }
            tabControl.SelectedTab = tpUser;
        }

        private void btnRole_Click(object sender, EventArgs e)
        {
            if (!IsExisted("role_manange", tabControl))
            {
                tpRole = new TabPage("角色管理");
                tpRole.Name = "role_manange";
                tabControl.TabPages.Add(tpRole);

                tpRole.Controls.Add(roleform);
            }
            tabControl.SelectedTab = tpRole;
        }

        private void btnFunc_Click(object sender, EventArgs e)
        {
            if (!IsExisted("func_manange", tabControl))
            {
                tpFunc = new TabPage("功能管理");
                tpFunc.Name = "func_manange";
                tabControl.TabPages.Add(tpFunc);

                tpFunc.Controls.Add(funcform);
            }
            tabControl.SelectedTab = tpFunc;
        }

        private Boolean IsExisted(string MainTabControlKey, TabControl objTabControl)
        {
            //遍历选项卡判断是否存在该子窗体  
            foreach (Control con in objTabControl.Controls)
            {
                TabPage tab = (TabPage)con;
                if (tab.Name == MainTabControlKey)
                {
                    return true;//存在  
                }
            }
            return false;//不存在  
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("您确认退出吗！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                PaCSAdminTool.WriteLoginLog("logout_admin");
                System.Environment.Exit(0);
            }
            else
                e.Cancel= true;
        }

       

    }
}
