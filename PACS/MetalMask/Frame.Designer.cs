namespace MetalMask
{
    partial class Frame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frame));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.menuGroup = new DevExpress.XtraNavBar.NavBarGroup();
            this.MSK_Mgnt = new DevExpress.XtraNavBar.NavBarItem();
            this.settingGroup = new DevExpress.XtraNavBar.NavBarGroup();
            this.MSK_CODE = new DevExpress.XtraNavBar.NavBarItem();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.navBarItem1 = new DevExpress.XtraNavBar.NavBarItem();
            this.imageList2 = new System.Windows.Forms.ImageList();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.splitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.navBarControl1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.AppearanceCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.splitContainerControl1.Panel2.AppearanceCaption.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainerControl1.Panel2.AppearanceCaption.ForeColor = System.Drawing.Color.Gray;
            this.splitContainerControl1.Panel2.AppearanceCaption.Options.UseBackColor = true;
            this.splitContainerControl1.Panel2.AppearanceCaption.Options.UseFont = true;
            this.splitContainerControl1.Panel2.AppearanceCaption.Options.UseForeColor = true;
            this.splitContainerControl1.Panel2.AppearanceCaption.Options.UseTextOptions = true;
            this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
            this.splitContainerControl1.Panel2.ShowCaption = true;
            this.splitContainerControl1.Size = new System.Drawing.Size(1011, 655);
            this.splitContainerControl1.SplitterPosition = 195;
            this.splitContainerControl1.TabIndex = 0;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.menuGroup;
            this.navBarControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(221)))), ((int)(((byte)(238)))));
            this.navBarControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.navBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.menuGroup,
            this.settingGroup});
            this.navBarControl1.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.MSK_Mgnt,
            this.MSK_CODE});
            this.navBarControl1.LargeImages = this.imageList1;
            this.navBarControl1.LinkInterval = 8;
            this.navBarControl1.Location = new System.Drawing.Point(0, 0);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 195;
            this.navBarControl1.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.ExplorerBar;
            this.navBarControl1.Size = new System.Drawing.Size(195, 651);
            this.navBarControl1.SmallImages = this.imageList1;
            this.navBarControl1.StoreDefaultPaintStyleName = true;
            this.navBarControl1.TabIndex = 0;
            this.navBarControl1.Text = "navBarControl1";
            this.navBarControl1.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarControl1_LinkClicked);
            this.navBarControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.navBarControl1_MouseDown);
            // 
            // menuGroup
            // 
            this.menuGroup.Appearance.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuGroup.Appearance.Options.UseFont = true;
            this.menuGroup.Appearance.Options.UseTextOptions = true;
            this.menuGroup.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.menuGroup.Caption = "操作菜单";
            this.menuGroup.Expanded = true;
            this.menuGroup.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.MSK_Mgnt)});
            this.menuGroup.Name = "menuGroup";
            this.menuGroup.SmallImage = ((System.Drawing.Image)(resources.GetObject("menuGroup.SmallImage")));
            // 
            // MSK_Mgnt
            // 
            this.MSK_Mgnt.Caption = "MetalMask管理";
            this.MSK_Mgnt.Name = "MSK_Mgnt";
            this.MSK_Mgnt.SmallImageIndex = 1;
            this.MSK_Mgnt.Tag = "60039";
            // 
            // settingGroup
            // 
            this.settingGroup.Appearance.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            this.settingGroup.Appearance.Options.UseFont = true;
            this.settingGroup.Appearance.Options.UseTextOptions = true;
            this.settingGroup.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.settingGroup.Caption = "基本信息";
            this.settingGroup.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.MSK_CODE)});
            this.settingGroup.Name = "settingGroup";
            this.settingGroup.SmallImage = ((System.Drawing.Image)(resources.GetObject("settingGroup.SmallImage")));
            // 
            // MSK_CODE
            // 
            this.MSK_CODE.Caption = "基本信息管理";
            this.MSK_CODE.Name = "MSK_CODE";
            this.MSK_CODE.SmallImageIndex = 1;
            this.MSK_CODE.Tag = "60040";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "item3.png");
            this.imageList1.Images.SetKeyName(1, "bullet_right.png");
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.AppearancePage.HeaderActive.BackColor = System.Drawing.Color.DimGray;
            this.xtraTabControl1.AppearancePage.HeaderActive.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xtraTabControl1.AppearancePage.HeaderActive.ForeColor = System.Drawing.Color.Blue;
            this.xtraTabControl1.AppearancePage.HeaderActive.Options.UseBackColor = true;
            this.xtraTabControl1.AppearancePage.HeaderActive.Options.UseFont = true;
            this.xtraTabControl1.AppearancePage.HeaderActive.Options.UseForeColor = true;
            this.xtraTabControl1.CausesValidation = false;
            this.xtraTabControl1.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeader;
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.HeaderButtons = ((DevExpress.XtraTab.TabButtons)((DevExpress.XtraTab.TabButtons.Close | DevExpress.XtraTab.TabButtons.Default)));
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.Size = new System.Drawing.Size(807, 651);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.CloseButtonClick += new System.EventHandler(this.xtraTabControl1_CloseButtonClick);
            // 
            // navBarItem1
            // 
            this.navBarItem1.Caption = "碳粉详细信息";
            this.navBarItem1.Name = "navBarItem1";
            this.navBarItem1.SmallImageIndex = 0;
            this.navBarItem1.Tag = "60034";
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "icon_cog.png");
            this.imageList2.Images.SetKeyName(1, "icon_datareport.png");
            this.imageList2.Images.SetKeyName(2, "icon_pens.png");
            // 
            // Frame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 655);
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "Frame";
            this.Text = "Frame";
            this.Load += new System.EventHandler(this.Frame_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup menuGroup;
        private DevExpress.XtraNavBar.NavBarItem MSK_Mgnt;
        private DevExpress.XtraNavBar.NavBarGroup settingGroup;
        private DevExpress.XtraNavBar.NavBarItem MSK_CODE;
        private System.Windows.Forms.ImageList imageList1;
        private DevExpress.XtraNavBar.NavBarItem navBarItem1;
        private System.Windows.Forms.ImageList imageList2;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
    }
}