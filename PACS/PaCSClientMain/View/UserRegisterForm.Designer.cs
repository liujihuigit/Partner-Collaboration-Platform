namespace PaCSClientMain.View
{
    partial class UserRegisterForm
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
            this.groupControl = new DevExpress.XtraEditors.GroupControl();
            this.lbStatus = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.tbFct = new DevExpress.XtraEditors.TextEdit();
            this.tbVendor = new DevExpress.XtraEditors.TextEdit();
            this.tbRemark = new DevExpress.XtraEditors.TextEdit();
            this.tbMail = new DevExpress.XtraEditors.TextEdit();
            this.tbPhone = new DevExpress.XtraEditors.TextEdit();
            this.tbFullName = new DevExpress.XtraEditors.TextEdit();
            this.tbPwd2 = new DevExpress.XtraEditors.TextEdit();
            this.tbPwd = new DevExpress.XtraEditors.TextEdit();
            this.tbName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl)).BeginInit();
            this.groupControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbFct.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVendor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRemark.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFullName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPwd2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPwd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl
            // 
            this.groupControl.Controls.Add(this.labelControl1);
            this.groupControl.Controls.Add(this.lbStatus);
            this.groupControl.Controls.Add(this.btnClose);
            this.groupControl.Controls.Add(this.btnSave);
            this.groupControl.Controls.Add(this.layoutControl1);
            this.groupControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl.Location = new System.Drawing.Point(0, 0);
            this.groupControl.Name = "groupControl";
            this.groupControl.ShowCaption = false;
            this.groupControl.Size = new System.Drawing.Size(328, 384);
            this.groupControl.TabIndex = 3;
            // 
            // lbStatus
            // 
            this.lbStatus.Location = new System.Drawing.Point(88, 263);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(0, 14);
            this.lbStatus.TabIndex = 3;
            // 
            // btnClose
            // 
            this.btnClose.Appearance.BackColor = System.Drawing.Color.Silver;
            this.btnClose.Appearance.Options.UseBackColor = true;
            this.btnClose.Location = new System.Drawing.Point(48, 297);
            this.btnClose.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
            this.btnClose.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(91, 33);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "取消";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Appearance.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Location = new System.Drawing.Point(182, 297);
            this.btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
            this.btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(91, 33);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "提交";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.tbFct);
            this.layoutControl1.Controls.Add(this.tbVendor);
            this.layoutControl1.Controls.Add(this.tbRemark);
            this.layoutControl1.Controls.Add(this.tbMail);
            this.layoutControl1.Controls.Add(this.tbPhone);
            this.layoutControl1.Controls.Add(this.tbFullName);
            this.layoutControl1.Controls.Add(this.tbPwd2);
            this.layoutControl1.Controls.Add(this.tbPwd);
            this.layoutControl1.Controls.Add(this.tbName);
            this.layoutControl1.Location = new System.Drawing.Point(15, 27);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(300, 232);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // tbFct
            // 
            this.tbFct.Location = new System.Drawing.Point(73, 101);
            this.tbFct.Name = "tbFct";
            this.tbFct.Properties.ReadOnly = true;
            this.tbFct.Size = new System.Drawing.Size(222, 20);
            this.tbFct.StyleController = this.layoutControl1;
            this.tbFct.TabIndex = 15;
            // 
            // tbVendor
            // 
            this.tbVendor.Location = new System.Drawing.Point(73, 125);
            this.tbVendor.Name = "tbVendor";
            this.tbVendor.Properties.ReadOnly = true;
            this.tbVendor.Size = new System.Drawing.Size(222, 20);
            this.tbVendor.StyleController = this.layoutControl1;
            this.tbVendor.TabIndex = 14;
            // 
            // tbRemark
            // 
            this.tbRemark.Location = new System.Drawing.Point(73, 197);
            this.tbRemark.Name = "tbRemark";
            this.tbRemark.Size = new System.Drawing.Size(222, 20);
            this.tbRemark.StyleController = this.layoutControl1;
            this.tbRemark.TabIndex = 13;
            // 
            // tbMail
            // 
            this.tbMail.Location = new System.Drawing.Point(73, 173);
            this.tbMail.Name = "tbMail";
            this.tbMail.Size = new System.Drawing.Size(222, 20);
            this.tbMail.StyleController = this.layoutControl1;
            this.tbMail.TabIndex = 12;
            // 
            // tbPhone
            // 
            this.tbPhone.Location = new System.Drawing.Point(73, 149);
            this.tbPhone.Name = "tbPhone";
            this.tbPhone.Size = new System.Drawing.Size(222, 20);
            this.tbPhone.StyleController = this.layoutControl1;
            this.tbPhone.TabIndex = 9;
            // 
            // tbFullName
            // 
            this.tbFullName.Location = new System.Drawing.Point(73, 77);
            this.tbFullName.Name = "tbFullName";
            this.tbFullName.Size = new System.Drawing.Size(222, 20);
            this.tbFullName.StyleController = this.layoutControl1;
            this.tbFullName.TabIndex = 7;
            // 
            // tbPwd2
            // 
            this.tbPwd2.Location = new System.Drawing.Point(73, 53);
            this.tbPwd2.Name = "tbPwd2";
            this.tbPwd2.Size = new System.Drawing.Size(222, 20);
            this.tbPwd2.StyleController = this.layoutControl1;
            this.tbPwd2.TabIndex = 6;
            // 
            // tbPwd
            // 
            this.tbPwd.Location = new System.Drawing.Point(73, 29);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.Size = new System.Drawing.Size(222, 20);
            this.tbPwd.StyleController = this.layoutControl1;
            this.tbPwd.TabIndex = 5;
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(73, 5);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(222, 20);
            this.tbName.StyleController = this.layoutControl1;
            this.tbName.TabIndex = 4;
            this.tbName.Leave += new System.EventHandler(this.tbName_Leave);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem5,
            this.layoutControlItem9});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
            this.layoutControlGroup1.Size = new System.Drawing.Size(300, 232);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.tbName;
            this.layoutControlItem1.CustomizationFormText = "用户名(*)";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem1.Text = "用户名(*)";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(65, 14);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.tbPwd;
            this.layoutControlItem2.CustomizationFormText = "密码(*)";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem2.Text = "密码(*)";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(65, 14);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.tbPwd2;
            this.layoutControlItem3.CustomizationFormText = "确认密码(*)";
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem3.Text = "确认密码(*)";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(65, 14);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.tbFullName;
            this.layoutControlItem4.CustomizationFormText = "真实姓名(*)";
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem4.Text = "真实姓名(*)";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(65, 14);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.tbPhone;
            this.layoutControlItem6.CustomizationFormText = "电话";
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 144);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem6.Text = "电话(*)";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(65, 14);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.tbMail;
            this.layoutControlItem7.CustomizationFormText = "Email";
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 168);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem7.Text = "Email(*)";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(65, 14);
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.tbRemark;
            this.layoutControlItem8.CustomizationFormText = "业务描述";
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 192);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(294, 34);
            this.layoutControlItem8.Text = "业务描述(*)";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(65, 14);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.tbVendor;
            this.layoutControlItem5.CustomizationFormText = "厂家";
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 120);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem5.Text = "厂家";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(65, 14);
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.tbFct;
            this.layoutControlItem9.CustomizationFormText = "公司";
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 96);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem9.Text = "公司";
            this.layoutControlItem9.TextSize = new System.Drawing.Size(65, 14);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.labelControl1.Location = new System.Drawing.Point(140, 358);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(175, 14);
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "*提交成功后，请联系管理员开通";
            // 
            // UserRegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 384);
            this.ControlBox = false;
            this.Controls.Add(this.groupControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "UserRegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "用户注册申请";
            this.Load += new System.EventHandler(this.UserRegisterForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl)).EndInit();
            this.groupControl.ResumeLayout(false);
            this.groupControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbFct.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVendor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRemark.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFullName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPwd2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPwd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit tbPhone;
        private DevExpress.XtraEditors.TextEdit tbFullName;
        private DevExpress.XtraEditors.TextEdit tbPwd2;
        private DevExpress.XtraEditors.TextEdit tbPwd;
        private DevExpress.XtraEditors.TextEdit tbName;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.TextEdit tbMail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.TextEdit tbRemark;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.LabelControl lbStatus;
        private DevExpress.XtraEditors.TextEdit tbVendor;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.TextEdit tbFct;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraEditors.LabelControl labelControl1;   
    }
}