namespace PaCSClientMain.View
{
    partial class ClientMainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientMainForm));
            DevExpress.XtraBars.Alerter.AlertButton alertButton2 = new DevExpress.XtraBars.Alerter.AlertButton();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.bsItem = new DevExpress.XtraBars.BarStaticItem();
            this.btnLoginUser = new DevExpress.XtraBars.BarButtonItem();
            this.btnChat = new DevExpress.XtraBars.BarButtonItem();
            this.barDateTime = new DevExpress.XtraBars.BarStaticItem();
            this.ribbonStatusBar1 = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            this.ribbonLogoHelper1 = new PaCSClientMain.Tools.RibbonLogoHelper();
            this.alertControl1 = new DevExpress.XtraBars.Alerter.AlertControl(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.AllowKeyTips = false;
            this.ribbonControl1.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.bsItem,
            this.btnLoginUser,
            this.btnChat,
            this.barDateTime});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 26;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.OptionsCustomizationForm.FormIcon = ((System.Drawing.Icon)(resources.GetObject("resource.FormIcon")));
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
            this.ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.ShowToolbarCustomizeItem = false;
            this.ribbonControl1.Size = new System.Drawing.Size(707, 55);
            this.ribbonControl1.StatusBar = this.ribbonStatusBar1;
            this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bsItem
            // 
            this.bsItem.Caption = "当前用户：";
            this.bsItem.Id = 9;
            this.bsItem.Name = "bsItem";
            this.bsItem.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // btnLoginUser
            // 
            this.btnLoginUser.Id = 15;
            this.btnLoginUser.Name = "btnLoginUser";
            this.btnLoginUser.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnLoginUser_ItemClick);
            // 
            // btnChat
            // 
            this.btnChat.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnChat.Caption = "联系管理员";
            this.btnChat.Glyph = ((System.Drawing.Image)(resources.GetObject("btnChat.Glyph")));
            this.btnChat.Id = 22;
            this.btnChat.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("btnChat.LargeGlyph")));
            this.btnChat.Name = "btnChat";
            this.btnChat.RibbonStyle = ((DevExpress.XtraBars.Ribbon.RibbonItemStyles)(((DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText) 
            | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText)));
            this.btnChat.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnChat_ItemClick);
            // 
            // barDateTime
            // 
            this.barDateTime.Id = 25;
            this.barDateTime.Name = "barDateTime";
            this.barDateTime.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // ribbonStatusBar1
            // 
            this.ribbonStatusBar1.ItemLinks.Add(this.bsItem);
            this.ribbonStatusBar1.ItemLinks.Add(this.btnLoginUser);
            this.ribbonStatusBar1.ItemLinks.Add(this.btnChat);
            this.ribbonStatusBar1.ItemLinks.Add(this.barDateTime);
            this.ribbonStatusBar1.Location = new System.Drawing.Point(0, 524);
            this.ribbonStatusBar1.Name = "ribbonStatusBar1";
            this.ribbonStatusBar1.Ribbon = this.ribbonControl1;
            this.ribbonStatusBar1.Size = new System.Drawing.Size(707, 27);
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Appearance.ForeColor = System.Drawing.Color.Red;
            this.xtraTabControl1.Appearance.Options.UseForeColor = true;
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.HeaderButtons = ((DevExpress.XtraTab.TabButtons)((((DevExpress.XtraTab.TabButtons.Prev | DevExpress.XtraTab.TabButtons.Next) 
            | DevExpress.XtraTab.TabButtons.Close) 
            | DevExpress.XtraTab.TabButtons.Default)));
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 55);
            this.xtraTabControl1.LookAndFeel.SkinName = "Office 2010 Silver";
            this.xtraTabControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.Padding = new System.Windows.Forms.Padding(3);
            this.xtraTabControl1.Size = new System.Drawing.Size(707, 469);
            this.xtraTabControl1.TabIndex = 2;
            this.xtraTabControl1.CloseButtonClick += new System.EventHandler(this.xtraTabControl1_CloseButtonClick);
            // 
            // defaultLookAndFeel1
            // 
            this.defaultLookAndFeel1.LookAndFeel.SkinName = "Office 2013 Light Gray";
            // 
            // ribbonLogoHelper1
            // 
            this.ribbonLogoHelper1.Image = ((System.Drawing.Image)(resources.GetObject("ribbonLogoHelper1.Image")));
            this.ribbonLogoHelper1.RibbonControl = this.ribbonControl1;
            // 
            // alertControl1
            // 
            this.alertControl1.AppearanceCaption.ForeColor = System.Drawing.Color.Blue;
            this.alertControl1.AppearanceCaption.Options.UseForeColor = true;
            this.alertControl1.AutoFormDelay = 10000;
            alertButton2.Hint = "复制消息文本";
            alertButton2.Image = ((System.Drawing.Image)(resources.GetObject("alertButton2.Image")));
            alertButton2.Name = "copyInfo";
            this.alertControl1.Buttons.Add(alertButton2);
            this.alertControl1.FormMaxCount = 3;
            this.alertControl1.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.alertControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.alertControl1.ButtonClick += new DevExpress.XtraBars.Alerter.AlertButtonClickEventHandler(this.alertControl1_ButtonClick);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ClientMainForm
            // 
            this.AllowFormGlass = DevExpress.Utils.DefaultBoolean.True;
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 551);
            this.Controls.Add(this.xtraTabControl1);
            this.Controls.Add(this.ribbonStatusBar1);
            this.Controls.Add(this.ribbonControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClientMainForm";
            this.Ribbon = this.ribbonControl1;
            this.StatusBar = this.ribbonStatusBar1;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientMainForm_FormClosing);
            this.Load += new System.EventHandler(this.ClientMainForm_Load);
            this.Shown += new System.EventHandler(this.ClientMainForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        private DevExpress.XtraBars.BarStaticItem bsItem;
        private DevExpress.XtraBars.BarButtonItem btnLoginUser;
        private Tools.RibbonLogoHelper ribbonLogoHelper1;
        private DevExpress.XtraBars.Alerter.AlertControl alertControl1;
        private DevExpress.XtraBars.BarButtonItem btnChat;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraBars.BarStaticItem barDateTime;
        private System.Windows.Forms.Timer timer1;

    }
}

