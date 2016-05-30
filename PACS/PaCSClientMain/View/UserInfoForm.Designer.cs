namespace PaCSClientMain.View
{
    partial class UserInfoForm
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.tbPhone = new DevExpress.XtraEditors.TextEdit();
            this.tbMail = new DevExpress.XtraEditors.TextEdit();
            this.tbRemarks = new DevExpress.XtraEditors.TextEdit();
            this.tbNewPwd2 = new DevExpress.XtraEditors.TextEdit();
            this.tbNewPwd = new DevExpress.XtraEditors.TextEdit();
            this.tbOldPwd = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lbStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRemarks.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbNewPwd2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbNewPwd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbOldPwd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.tbPhone);
            this.layoutControl1.Controls.Add(this.tbMail);
            this.layoutControl1.Controls.Add(this.tbRemarks);
            this.layoutControl1.Controls.Add(this.tbNewPwd2);
            this.layoutControl1.Controls.Add(this.tbNewPwd);
            this.layoutControl1.Controls.Add(this.tbOldPwd);
            this.layoutControl1.Location = new System.Drawing.Point(22, 35);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(292, 189);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // tbPhone
            // 
            this.tbPhone.Location = new System.Drawing.Point(75, 108);
            this.tbPhone.Name = "tbPhone";
            this.tbPhone.Size = new System.Drawing.Size(205, 20);
            this.tbPhone.StyleController = this.layoutControl1;
            this.tbPhone.TabIndex = 9;
            // 
            // tbMail
            // 
            this.tbMail.Location = new System.Drawing.Point(75, 84);
            this.tbMail.Name = "tbMail";
            this.tbMail.Size = new System.Drawing.Size(205, 20);
            this.tbMail.StyleController = this.layoutControl1;
            this.tbMail.TabIndex = 8;
            // 
            // tbRemarks
            // 
            this.tbRemarks.Location = new System.Drawing.Point(75, 132);
            this.tbRemarks.Name = "tbRemarks";
            this.tbRemarks.Size = new System.Drawing.Size(205, 20);
            this.tbRemarks.StyleController = this.layoutControl1;
            this.tbRemarks.TabIndex = 7;
            // 
            // tbNewPwd2
            // 
            this.tbNewPwd2.Location = new System.Drawing.Point(75, 60);
            this.tbNewPwd2.Name = "tbNewPwd2";
            this.tbNewPwd2.Properties.PasswordChar = '●';
            this.tbNewPwd2.Size = new System.Drawing.Size(205, 20);
            this.tbNewPwd2.StyleController = this.layoutControl1;
            this.tbNewPwd2.TabIndex = 6;
            // 
            // tbNewPwd
            // 
            this.tbNewPwd.Location = new System.Drawing.Point(75, 36);
            this.tbNewPwd.Name = "tbNewPwd";
            this.tbNewPwd.Properties.PasswordChar = '●';
            this.tbNewPwd.Size = new System.Drawing.Size(205, 20);
            this.tbNewPwd.StyleController = this.layoutControl1;
            this.tbNewPwd.TabIndex = 5;
            // 
            // tbOldPwd
            // 
            this.tbOldPwd.Location = new System.Drawing.Point(75, 12);
            this.tbOldPwd.Name = "tbOldPwd";
            this.tbOldPwd.Properties.PasswordChar = '●';
            this.tbOldPwd.Size = new System.Drawing.Size(205, 20);
            this.tbOldPwd.StyleController = this.layoutControl1;
            this.tbOldPwd.TabIndex = 4;
            this.tbOldPwd.Leave += new System.EventHandler(this.tbOldPwd_Leave);
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
            this.layoutControlItem5,
            this.layoutControlItem7,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(292, 189);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.tbOldPwd;
            this.layoutControlItem1.CustomizationFormText = "原密码";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(272, 24);
            this.layoutControlItem1.Text = "原密码";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(60, 14);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.tbNewPwd;
            this.layoutControlItem2.CustomizationFormText = "新密码";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(272, 24);
            this.layoutControlItem2.Text = "新密码";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(60, 14);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.tbNewPwd2;
            this.layoutControlItem3.CustomizationFormText = "确认新密码";
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(272, 24);
            this.layoutControlItem3.Text = "确认新密码";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(60, 14);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.tbMail;
            this.layoutControlItem5.CustomizationFormText = "E-mail";
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(272, 24);
            this.layoutControlItem5.Text = "E-mail";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(60, 14);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.tbPhone;
            this.layoutControlItem7.CustomizationFormText = "联系电话";
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 96);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(272, 24);
            this.layoutControlItem7.Text = "联系电话";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(60, 14);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.tbRemarks;
            this.layoutControlItem4.CustomizationFormText = "备注";
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 120);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(272, 49);
            this.layoutControlItem4.Text = "备注";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(60, 14);
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.lbStatus);
            this.groupControl1.Controls.Add(this.btnSave);
            this.groupControl1.Controls.Add(this.layoutControl1);
            this.groupControl1.Location = new System.Drawing.Point(12, 12);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(344, 324);
            this.groupControl1.TabIndex = 2;
            this.groupControl1.Text = "修改密码";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(131, 265);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.tbMail;
            this.layoutControlItem6.CustomizationFormText = "layoutControlItem5";
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem6.Name = "layoutControlItem5";
            this.layoutControlItem6.Size = new System.Drawing.Size(256, 24);
            this.layoutControlItem6.Text = "layoutControlItem5";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(103, 14);
            this.layoutControlItem6.TextToControlDistance = 5;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(95, 239);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(0, 12);
            this.lbStatus.TabIndex = 3;
            // 
            // UserInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 363);
            this.Controls.Add(this.groupControl1);
            this.Name = "UserInfoForm";
            this.Text = "UserInfoForm";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRemarks.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbNewPwd2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbNewPwd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbOldPwd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit tbNewPwd2;
        private DevExpress.XtraEditors.TextEdit tbNewPwd;
        private DevExpress.XtraEditors.TextEdit tbOldPwd;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.TextEdit tbRemarks;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.TextEdit tbPhone;
        private DevExpress.XtraEditors.TextEdit tbMail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private System.Windows.Forms.Label lbStatus;
    }
}