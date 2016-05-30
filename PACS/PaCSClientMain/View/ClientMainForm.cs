using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PaCSClientMain.Presenter;
using PaCSClientMain;
using PaCSClientMain.Tools;
using System.Reflection;
using DevExpress.XtraNavBar;
using DevExpress.XtraTab;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using PaCSTools;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraBars.Alerter;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Media;
using System.Data.OracleClient;

namespace PaCSClientMain.View
{
    public partial class ClientMainForm : RibbonForm
    {
        private MainPresenter mainPresenter = null;
        private string alertText = "";

        private OracleConnection conn = null;
        private OracleCommand cmd = null;

        public ClientMainForm()
        {
            InitializeComponent();

            try
            {
                conn = new OracleConnection("Data Source=109.116.6.17/sstpop;User ID=cpopuser;PassWord=cpop2000;Integrated Security=no;");
                cmd = new OracleCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                mainPresenter = new MainPresenter();
            }
            catch (Exception)
            {
                
            }
        }

        private void ClientMainForm_Load(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(typeof(WaitLoading));
        }

        /// <summary>
        /// 初始化菜单，登录用户信息
        /// </summary>
        public void Init()
        {
            try
            {
                //登录用户 信息    
                if (!PaCSTools.PaCSGlobal.LoginUserInfo.Venderid.Equals(PaCSTools.PaCSGlobal.LoginUserInfo.Fct_code.Substring(0,4)))
                {
                    string factoryName = (PaCSTools.PaCSGlobal.LoginUserInfo.Fct_code.Equals("C660A") ? "SSDP" : "SESC");//公司
                    this.btnLoginUser.Caption = PaCSTools.PaCSGlobal.LoginUserInfo.FullName + " (" + factoryName + "-" + PaCSTools.PaCSGlobal.LoginUserInfo.Vendername + ")";//用户名
                }
                else
                    this.btnLoginUser.Caption = PaCSTools.PaCSGlobal.LoginUserInfo.FullName + " (" + PaCSTools.PaCSGlobal.LoginUserInfo.Vendername + ")";//用户名
                
                //菜单
                DataTable dt = mainPresenter.GetMenuLvlOne();//一级菜单   Page
                foreach (DataRow dr in dt.Rows)
                {
                    RibbonPage rp = new RibbonPage();
                    rp.Text = dr["name"].ToString();//资源管理
                    rp.Appearance.Font = new Font("微软雅黑", 11, FontStyle.Bold);
                    string ctlId = dr["controlid"].ToString();
                    rp.Image = (Image)Properties.Resources.ResourceManager.GetObject(ctlId);//获取文件
                    rp.ImageToTextIndent = 5;

                    string pid1 = dr["id"].ToString();
                    DataTable dt2 = mainPresenter.GetMenuLvlTwo(pid1);//二级菜单 Group
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        RibbonPageGroup rpg = new RibbonPageGroup();
                        rpg.Text = dr2["name"].ToString();//设备准备
                        rpg.ShowCaptionButton = false;

                        string pid2 = dr2["id"].ToString();
                        DataTable dt3 = mainPresenter.GetMenuLvlThree(pid2);//三级菜单 buttonitem
                        foreach (DataRow dr3 in dt3.Rows)
                        {
                            string controlId = dr3["controlid"].ToString();
                            Image image = (Image)Properties.Resources.ResourceManager.GetObject(controlId);//获取文件
                            //Image image = Image.FromFile(Application.StartupPath+"\\Images\\"+controlId+".png");//获取文件

                            BarButtonItem bbItm = new BarButtonItem();
                            bbItm.Caption = dr3["name"].ToString();//工具
                            bbItm.Name = controlId;
                            //bbItm.Tag = dr3["id"].ToString();
                            bbItm.Tag = rpg.Text;//保存上层RibbonPageGroup 设备准备
                            bbItm.LargeGlyph = image;

                            bbItm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_ItemClick);//注册事件

                            if (!PaCSTools.PaCSGlobal.HasFunction(dr3["id"].ToString()))
                            {
                                bbItm.Enabled = false;
                                bbItm.Hint = "您没有权限执行该程序！";
                            }
                            rpg.ItemLinks.Add(bbItm);
                        }
                        rp.Groups.Add(rpg);
                    }
                    ribbonControl1.Pages.Add(rp);
                }
            }
            catch (Exception funce)
            {
                XtraMessageBox.Show(this, "System error" + funce.Message);
            }
        }

        /// <summary>
        /// 动态调出窗体事件 注：动态调用窗体名需和数据库中名称完全一致
        /// </summary>
        private void barButtonItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string form_name, form_caption;
                form_name = e.Link.Item.Name.Trim();//MetalMask
                form_caption = e.Link.Item.Caption.Trim();//MetalMask管理

                if (!IsExisted(form_name, xtraTabControl1))
                {
                    if (xtraTabControl1.TabPages.Count <= 10)
                    {
                        string assemblyName = e.Link.Item.Name.Trim();
                        Form childForm = (Form)System.Reflection.Assembly.Load(assemblyName).CreateInstance(assemblyName + ".Frame");

                        this.InitFormInTabControl(childForm);

                        XtraTabPage xt = new XtraTabPage();
                        xt.Text = form_caption;
                        xt.Name = form_name;
                        xt.Controls.Add(childForm);
                        xtraTabControl1.TabPages.Add(xt);

                        /*
                        //statusbar显示路径
                        string pageName = xtraTabControl1.SelectedTabPage.Text;
                        string groupName = e.Link.Item.Tag.ToString();
                        this.formPath.Caption = pageName + " —>" + groupName + " —>" + xt.Text;
                         */
                    }
                    else
                        XtraMessageBox.Show("您已经打开太多的窗体！", "提示");
                }
                xtraTabControl1.SelectedTabPage = (XtraTabPage)xtraTabControl1.Controls.Find(form_name, false)[0];
            }
            catch (Exception barButtonItem_ItemClick)
            {
                XtraMessageBox.Show(this, "System error[barButtonItem_ItemClick]: " + barButtonItem_ItemClick.Message);
            }
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
            }
            return false;//不存在  
        }

        private void InitFormInTabControl(Form form)
        {
            form.TopLevel = false;
            form.BackColor = Color.White;
            form.Dock = DockStyle.Fill;
            //form.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Show();
        }

        /// <summary>
        /// 关闭所有窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCloseAll_Click(object sender, EventArgs e)
        {
            foreach (XtraTabPage tp in xtraTabControl1.TabPages)
            {
                xtraTabControl1.TabPages.Remove(tp);
            }
        }

        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCloseCurrent_Click(object sender, EventArgs e)
        {
            xtraTabControl1.TabPages.Remove(xtraTabControl1.SelectedTabPage);
        }

        /// <summary>
        /// 关闭其他窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCloseOther_Click(object sender, EventArgs e)
        {
            foreach (XtraTabPage tp in xtraTabControl1.TabPages)
            {
                if (tp != xtraTabControl1.SelectedTabPage)
                    xtraTabControl1.TabPages.Remove(tp);
            }
        }

        private void xtraTabControl1_CloseButtonClick(object sender, EventArgs e)
        {
            DialogResult dr = XtraMessageBox.Show("您确认关闭【 " + xtraTabControl1.SelectedTabPage.Text + " 】吗！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                //System.Environment.Exit(0);
                ClosePageButtonEventArgs a = (ClosePageButtonEventArgs)e;
                string tabpagename = a.Page.Text;
                foreach (Control xtp in xtraTabControl1.TabPages)
                {
                    if (xtp.Text == tabpagename)
                    {
                        xtp.Dispose();
                        return;
                    }
                }

                //xtraTabControl1.TabPages.Remove(xtraTabControl1.SelectedTabPage);
            }
            else
                return;
        }

        private void btnLoginUser_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!IsExisted("userManage", xtraTabControl1))
            {
                if (xtraTabControl1.TabPages.Count <= 12)
                {
                    UserInfoForm uiForm = new UserInfoForm();
                    InitFormInTabControl(uiForm);

                    XtraTabPage xt = new XtraTabPage();
                    xt.Text = "用户管理";
                    xt.Name = "userManage";
                    xt.Controls.Add(uiForm);
                    xtraTabControl1.TabPages.Add(xt);
                }
                else
                    XtraMessageBox.Show("您已经打开太多的窗体！", "提示");
            }

            xtraTabControl1.SelectedTabPage = (XtraTabPage)xtraTabControl1.Controls.Find("userManage", false)[0];
        }

        private void ClientMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = XtraMessageBox.Show("您确认退出吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                try
                {
                    mainPresenter.WriteLogOutLog();
                    if (cmd!=null)
                        cmd.Dispose();
                    if(conn.State !=ConnectionState.Closed)
                        conn.Close();
                    if (conn!=null)
                        conn.Dispose();
                }
                catch (Exception)
                {

                }
                System.Environment.Exit(0);
            }
            else
                e.Cancel = true;
        }

        private void ribbonControl1_Paint(object sender, PaintEventArgs e)
        {
            //if (IsCurrentAbout)
            // return;
            DevExpress.XtraBars.Ribbon.ViewInfo.RibbonViewInfo ribbonViewInfo = ribbonControl1.ViewInfo;
            if (ribbonViewInfo == null)
                return;
            DevExpress.XtraBars.Ribbon.ViewInfo.RibbonPanelViewInfo panelViewInfo = ribbonViewInfo.Panel;
            if (panelViewInfo == null)
                return;
            Rectangle bounds = panelViewInfo.Bounds;
            int minX = bounds.X;
            DevExpress.XtraBars.Ribbon.ViewInfo.RibbonPageGroupViewInfoCollection groups = panelViewInfo.Groups;
            if (groups == null)
                return;
            if (groups.Count > 0)
                minX = groups[groups.Count - 1].Bounds.Right;
            // Image image = DevExpress.Utils.Frames.ApplicationCaption8_1.ImageLogo;
            Image image = (Image)Properties.Resources.ResourceManager.GetObject("Logo");//获取文件
            if (bounds.Height < image.Height)
                return;
            int offset = (bounds.Height - image.Height) / 2;
            int width = image.Width + 15;
            bounds.X = bounds.Width - width;
            if (bounds.X < minX)
                return;
            bounds.Width = width;
            bounds.Y += offset;
            bounds.Height = image.Height;
            e.Graphics.DrawImage(image, bounds.Location);
        }

        private void btnChat_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                XtraMessageBox.Show("Email：xxdd.liu@samsung.com","温馨提示");
            }
            catch (Exception btnApply_Click)
            {

            }
        }


        private void ShowAlertControl()
        {
            try
            {
                DataTable dt = mainPresenter.GetNotice();
                if (dt != null && dt.Rows.Count > 0)
                {
                    alertText = dt.Rows[0][0].ToString();
                }
                if (!string.IsNullOrEmpty(alertText))
                {
                    AlertInfo info = new AlertInfo("系统消息", alertText);
                    alertControl1.Show(this, info);
                }
            }
            catch (Exception)
            {

            }
        }

        private void alertControl1_ButtonClick(object sender, AlertButtonClickEventArgs e)
        {
            if (e.ButtonName == "copyInfo")
            {
                Clipboard.SetDataObject(alertText);
                e.Button.Down = true;
                XtraMessageBox.Show("系统消息已经复制到剪贴板!", "提示");
            }

        }

        private void ClientMainForm_Shown(object sender, EventArgs e)
        {
            //ShowAlertControl();
            SplashScreenManager.CloseForm();  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Broken)
                {
                    conn.Close();
                    conn.Open();
                }
                else if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                else if(conn.State == ConnectionState.Open)
                {
                    barDateTime.Caption = ExecuteScalar(conn, cmd, "select to_char(sysdate,'yyyy-mm-dd hh24:mi:ss') from dual");
                }
            }
            catch (Exception)
            {
                conn.Close();
            }
        }


        private string ExecuteScalar(OracleConnection connection, OracleCommand command, string cmdText)
        {
            object result = null;

            try
            {
                PrepareCommand(connection,command, null, CommandType.Text, cmdText);
                result = command.ExecuteScalar();
            }
            catch
            {
                throw;
            }

            return result.ToString();
        }


        private void PrepareCommand(OracleConnection connection, OracleCommand command, OracleTransaction trans, CommandType cmdType, string cmdText)
        {
            if (connection.State != ConnectionState.Open) connection.Open();

            command.Connection = connection;
            command.CommandText = cmdText;
            command.CommandType = cmdType;

            if (trans != null) command.Transaction = trans;
        }

    
    }
}
