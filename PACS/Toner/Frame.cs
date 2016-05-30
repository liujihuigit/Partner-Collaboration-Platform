using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.XtraNavBar;
using PaCSTools;
using DevExpress.XtraTab.ViewInfo;
using System.IO.Ports;

namespace Toner
{
    public partial class Frame : DevExpress.XtraEditors.XtraForm
    {
        public Frame()
        {
            InitializeComponent();
        }

        private void Frame_Load(object sender, EventArgs e)
        {
            InitFunction();
            //OpenPort();
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        private void OpenPort()
        {
            try
            {
                for (int i = 1; i < 17; i++)
                {
                    if (!string.IsNullOrEmpty(PaCSGlobal.GetRegistryValue("COM" + i, "Toner")))
                    {
                        if (TonerGlobal.port.IsOpen)
                        {
                            TonerGlobal.port.Close();
                        }

                        TonerGlobal.port.PortName = "COM" + i;
                        TonerGlobal.port.BaudRate = Convert.ToInt32(PaCSGlobal.GetRegistryValue("COM" + i, "Toner"));

                        TonerGlobal.port.Open();
                    }
                }
            }
            catch (Exception OpenPort)
            {
                XtraMessageBox.Show(this, "System error[OpenPort]: " + OpenPort.Message);
                TonerGlobal.port.Close();
            }
        }

        /// <summary>
        /// 配置权限
        /// </summary>
        private void InitFunction()
        {
            try
            {
                int count = this.menuGroup.ItemLinks.Count;
                foreach(NavBarGroup group in navBarControl1.Groups)
                {
                    foreach (NavBarItemLink itemLink in group.ItemLinks)
                    {
                        if (itemLink is NavBarItemLink)
                        {
                            string itemFunId = itemLink.Item.Tag == null ? "" : itemLink.Item.Tag.ToString();
                            itemLink.Item.Enabled = PaCSGlobal.HasFunction(itemFunId);
                            if (!itemLink.Item.Enabled)
                                itemLink.Item.Hint = "您没有权限执行该程序！";
                        }
                    }
                }
            }
            catch (Exception InitFunction)
            {
                XtraMessageBox.Show(this, "System error[InitFunction]: " + InitFunction.Message);
            }
        }

        private void navBarControl1_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            try
            {
                // if (e.Link.Item.Appearance.ForeColor == Color.Red)
                if (e.Link.Item.Enabled == false)
                    return;
                string formName, formCaption;
                formName = e.Link.Item.Name.Trim();//MetalMask/Add controlid
                formCaption = e.Link.Item.Caption.Trim();//注册页面

                if (!IsExisted(formName, xtraTabControl1))
                {
                      if (xtraTabControl1.TabPages.Count <= 10)
                    {
                        string assemblyName = "Toner";
                        Form childForm = (Form)System.Reflection.Assembly.Load(assemblyName).CreateInstance(assemblyName + "." + formName);

                        this.InitFormInControl(childForm);

                        XtraTabPage xt = new XtraTabPage();
                        xt.Text = formCaption;
                        xt.Name = formName;

                        xt.Controls.Add(childForm);
                        xtraTabControl1.TabPages.Add(xt);
                    }
                      else
                          XtraMessageBox.Show("您已经打开太多的窗体！", "提示");
                }
                xtraTabControl1.SelectedTabPage = (XtraTabPage)xtraTabControl1.Controls.Find(formName, false)[0];
            }
            catch (Exception navBarControl1_LinkClicked)
            {
                XtraMessageBox.Show(this, "System error[navBarControl1_LinkClicked]: " + navBarControl1_LinkClicked.Message);
                return;
            }
        }

        private void InitFormInControl(Form form)
        {
            form.TopLevel = false;
            form.BackColor = Color.White;
            form.Dock = DockStyle.Fill;
            //form.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Show();
        }

        private Boolean IsExisted(string MainTabControlKey, XtraTabControl objTabControl)
        {
            //遍历选项卡判断是否存在该子窗体  
            foreach (Control con in objTabControl.Controls)
            {
                XtraTabPage tab = (XtraTabPage)con;
                if (tab.Name == MainTabControlKey)
                {
                    return true;//存在  
                }
                else
                {
                    bool flag = false;
                    foreach (NavBarItemLink menuLink in menuGroup.ItemLinks)
                    {
                             if (MainTabControlKey.Equals(menuLink.ItemName))
                             {
                                 flag = true;
                             }
                    }
               
                    if(flag)
                    {
                        //判断 操作菜单下的项是否已经打开。有，则关闭并取代；无，则显示
                        foreach (NavBarItemLink menuLink in menuGroup.ItemLinks)
                        {
                            if (tab.Name.Equals(menuLink.ItemName))//说明已经打开过操作菜单
                            {
                                foreach (Control frm in tab.Controls)
                                {
                                    Form f = (Form)frm;
                                    f.Close();
                                }

                                tab.Dispose();
                            }
                        }
                    }
                }
            }
            return false;//不存在  
        }

        private void navBarControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DevExpress.XtraNavBar.NavBarControl navBar = sender as DevExpress.XtraNavBar.NavBarControl;
                DevExpress.XtraNavBar.NavBarHitInfo hitInfo = navBar.CalcHitInfo(new Point(e.X, e.Y));
                if (hitInfo.InGroupCaption && !hitInfo.InGroupButton)
                    hitInfo.Group.Expanded = !hitInfo.Group.Expanded;
            }
        }

        private void xtraTabControl1_CloseButtonClick(object sender, EventArgs e)
        {
            ClosePageButtonEventArgs a = (ClosePageButtonEventArgs)e;
            string tabpagename = a.Page.Text;
            foreach (Control xtp in xtraTabControl1.TabPages)
            {
                if (xtp.Text == tabpagename)
                {
                    foreach (Control frm in xtp.Controls)
                     {
                         Form f = (Form)frm;
                         f.Close();
                     }

                    xtp.Dispose();
                    xtraTabControl1.SelectedTabPageIndex = xtraTabControl1.TabPages.Count;
                    return;
                }
            }

        }

    }
}