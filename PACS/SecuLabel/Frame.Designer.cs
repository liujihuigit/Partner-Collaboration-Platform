namespace SecuLabel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frame));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.menuGroup = new DevExpress.XtraNavBar.NavBarGroup();
            this.MaterialRequest = new DevExpress.XtraNavBar.NavBarItem();
            this.IssueAssy = new DevExpress.XtraNavBar.NavBarItem();
            this.ReqListVendor = new DevExpress.XtraNavBar.NavBarItem();
            this.MaterialGi = new DevExpress.XtraNavBar.NavBarItem();
            this.BarcodeGr = new DevExpress.XtraNavBar.NavBarItem();
            this.SsdpInput = new DevExpress.XtraNavBar.NavBarItem();
            this.VendorInput = new DevExpress.XtraNavBar.NavBarItem();
            this.chartGroup = new DevExpress.XtraNavBar.NavBarGroup();
            this.HistoryGiGr = new DevExpress.XtraNavBar.NavBarItem();
            this.StockQuery = new DevExpress.XtraNavBar.NavBarItem();
            this.SecuReport = new DevExpress.XtraNavBar.NavBarItem();
            this.RequestList = new DevExpress.XtraNavBar.NavBarItem();
            this.SecuStockCheck = new DevExpress.XtraNavBar.NavBarItem();
            this.VendorSelfCheck = new DevExpress.XtraNavBar.NavBarItem();
            this.settingGroup = new DevExpress.XtraNavBar.NavBarGroup();
            this.MasterInfo = new DevExpress.XtraNavBar.NavBarItem();
            this.MaterialUpload = new DevExpress.XtraNavBar.NavBarItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.navBarItem1 = new DevExpress.XtraNavBar.NavBarItem();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
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
            this.splitContainerControl1.LookAndFeel.UseDefaultLookAndFeel = false;
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
            this.splitContainerControl1.SplitterPosition = 202;
            this.splitContainerControl1.TabIndex = 0;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.menuGroup;
            this.navBarControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.navBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.menuGroup,
            this.chartGroup,
            this.settingGroup});
            this.navBarControl1.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.MaterialRequest,
            this.IssueAssy,
            this.ReqListVendor,
            this.MaterialGi,
            this.StockQuery,
            this.HistoryGiGr,
            this.MasterInfo,
            this.BarcodeGr,
            this.MaterialUpload,
            this.SecuReport,
            this.RequestList,
            this.SecuStockCheck,
            this.VendorSelfCheck,
            this.SsdpInput,
            this.VendorInput});
            this.navBarControl1.LargeImages = this.imageList1;
            this.navBarControl1.LinkInterval = 8;
            this.navBarControl1.Location = new System.Drawing.Point(0, 0);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 202;
            this.navBarControl1.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.ExplorerBar;
            this.navBarControl1.Size = new System.Drawing.Size(202, 651);
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
            this.menuGroup.Caption = "入库/出库管理";
            this.menuGroup.Expanded = true;
            this.menuGroup.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.MaterialRequest),
            new DevExpress.XtraNavBar.NavBarItemLink(this.IssueAssy),
            new DevExpress.XtraNavBar.NavBarItemLink(this.ReqListVendor),
            new DevExpress.XtraNavBar.NavBarItemLink(this.MaterialGi),
            new DevExpress.XtraNavBar.NavBarItemLink(this.BarcodeGr),
            new DevExpress.XtraNavBar.NavBarItemLink(this.SsdpInput),
            new DevExpress.XtraNavBar.NavBarItemLink(this.VendorInput)});
            this.menuGroup.Name = "menuGroup";
            this.menuGroup.SmallImage = ((System.Drawing.Image)(resources.GetObject("menuGroup.SmallImage")));
            // 
            // MaterialRequest
            // 
            this.MaterialRequest.Caption = "材料申请";
            this.MaterialRequest.Name = "MaterialRequest";
            this.MaterialRequest.SmallImageIndex = 1;
            this.MaterialRequest.Tag = "60058";
            // 
            // IssueAssy
            // 
            this.IssueAssy.Caption = "确认发料";
            this.IssueAssy.Name = "IssueAssy";
            this.IssueAssy.SmallImageIndex = 1;
            this.IssueAssy.Tag = "60052";
            // 
            // ReqListVendor
            // 
            this.ReqListVendor.Caption = "申请单厂家处理";
            this.ReqListVendor.Name = "ReqListVendor";
            this.ReqListVendor.SmallImageIndex = 1;
            this.ReqListVendor.Tag = "60053";
            // 
            // MaterialGi
            // 
            this.MaterialGi.Caption = "材料出库";
            this.MaterialGi.Name = "MaterialGi";
            this.MaterialGi.SmallImageIndex = 1;
            this.MaterialGi.Tag = "60059";
            // 
            // BarcodeGr
            // 
            this.BarcodeGr.Caption = "防伪标签扫描入库";
            this.BarcodeGr.Name = "BarcodeGr";
            this.BarcodeGr.SmallImageIndex = 1;
            this.BarcodeGr.Tag = "60045";
            // 
            // SsdpInput
            // 
            this.SsdpInput.Caption = "<社内>防伪标签投入";
            this.SsdpInput.Name = "SsdpInput";
            this.SsdpInput.SmallImageIndex = 1;
            this.SsdpInput.Tag = "60056";
            // 
            // VendorInput
            // 
            this.VendorInput.Caption = "<社外>防伪标签投入";
            this.VendorInput.Name = "VendorInput";
            this.VendorInput.SmallImageIndex = 1;
            this.VendorInput.Tag = "60057";
            // 
            // chartGroup
            // 
            this.chartGroup.Appearance.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            this.chartGroup.Appearance.Options.UseFont = true;
            this.chartGroup.Appearance.Options.UseTextOptions = true;
            this.chartGroup.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.chartGroup.Caption = "数据报表";
            this.chartGroup.Expanded = true;
            this.chartGroup.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.HistoryGiGr),
            new DevExpress.XtraNavBar.NavBarItemLink(this.StockQuery),
            new DevExpress.XtraNavBar.NavBarItemLink(this.SecuReport),
            new DevExpress.XtraNavBar.NavBarItemLink(this.RequestList),
            new DevExpress.XtraNavBar.NavBarItemLink(this.SecuStockCheck),
            new DevExpress.XtraNavBar.NavBarItemLink(this.VendorSelfCheck)});
            this.chartGroup.LargeImage = ((System.Drawing.Image)(resources.GetObject("chartGroup.LargeImage")));
            this.chartGroup.Name = "chartGroup";
            // 
            // HistoryGiGr
            // 
            this.HistoryGiGr.Caption = "入出库履历";
            this.HistoryGiGr.Name = "HistoryGiGr";
            this.HistoryGiGr.SmallImageIndex = 1;
            this.HistoryGiGr.Tag = "60047";
            // 
            // StockQuery
            // 
            this.StockQuery.Caption = "库存查询";
            this.StockQuery.Name = "StockQuery";
            this.StockQuery.SmallImageIndex = 1;
            this.StockQuery.Tag = "60046";
            // 
            // SecuReport
            // 
            this.SecuReport.Caption = "防伪标签报表";
            this.SecuReport.Name = "SecuReport";
            this.SecuReport.SmallImageIndex = 1;
            this.SecuReport.Tag = "60049";
            // 
            // RequestList
            // 
            this.RequestList.Caption = "申请List查看";
            this.RequestList.Name = "RequestList";
            this.RequestList.SmallImageIndex = 1;
            this.RequestList.Tag = "60048";
            // 
            // SecuStockCheck
            // 
            this.SecuStockCheck.Caption = "防伪标签盘点";
            this.SecuStockCheck.Name = "SecuStockCheck";
            this.SecuStockCheck.SmallImageIndex = 1;
            this.SecuStockCheck.Tag = "60050";
            // 
            // VendorSelfCheck
            // 
            this.VendorSelfCheck.Caption = "厂家自身盘点";
            this.VendorSelfCheck.Name = "VendorSelfCheck";
            this.VendorSelfCheck.SmallImageIndex = 1;
            this.VendorSelfCheck.Tag = "60051";
            // 
            // settingGroup
            // 
            this.settingGroup.Appearance.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            this.settingGroup.Appearance.Options.UseFont = true;
            this.settingGroup.Appearance.Options.UseTextOptions = true;
            this.settingGroup.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.settingGroup.Caption = "基本信息";
            this.settingGroup.Expanded = true;
            this.settingGroup.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.MasterInfo),
            new DevExpress.XtraNavBar.NavBarItemLink(this.MaterialUpload)});
            this.settingGroup.Name = "settingGroup";
            this.settingGroup.SmallImage = ((System.Drawing.Image)(resources.GetObject("settingGroup.SmallImage")));
            // 
            // MasterInfo
            // 
            this.MasterInfo.Caption = "材料信息管理";
            this.MasterInfo.Name = "MasterInfo";
            this.MasterInfo.SmallImageIndex = 1;
            this.MasterInfo.Tag = "60042";
            // 
            // MaterialUpload
            // 
            this.MaterialUpload.Caption = "防伪标签明细上载";
            this.MaterialUpload.Name = "MaterialUpload";
            this.MaterialUpload.SmallImageIndex = 1;
            this.MaterialUpload.Tag = "60060";
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
            this.xtraTabControl1.Size = new System.Drawing.Size(793, 651);
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
        private DevExpress.XtraNavBar.NavBarItem MaterialRequest;
        private DevExpress.XtraNavBar.NavBarItem IssueAssy;
        private DevExpress.XtraNavBar.NavBarItem ReqListVendor;
        private DevExpress.XtraNavBar.NavBarItem MaterialGi;
        private DevExpress.XtraNavBar.NavBarGroup chartGroup;
        private DevExpress.XtraNavBar.NavBarItem StockQuery;
        private DevExpress.XtraNavBar.NavBarItem HistoryGiGr;
        private DevExpress.XtraNavBar.NavBarGroup settingGroup;
        private DevExpress.XtraNavBar.NavBarItem MasterInfo;
        private System.Windows.Forms.ImageList imageList1;
        private DevExpress.XtraNavBar.NavBarItem BarcodeGr;
        private DevExpress.XtraNavBar.NavBarItem navBarItem1;
        private System.Windows.Forms.ImageList imageList2;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraNavBar.NavBarItem MaterialUpload;
        private DevExpress.XtraNavBar.NavBarItem SecuReport;
        private DevExpress.XtraNavBar.NavBarItem RequestList;
        private DevExpress.XtraNavBar.NavBarItem SecuStockCheck;
        private DevExpress.XtraNavBar.NavBarItem VendorSelfCheck;
        private DevExpress.XtraNavBar.NavBarItem SsdpInput;
        private DevExpress.XtraNavBar.NavBarItem VendorInput;
    }
}