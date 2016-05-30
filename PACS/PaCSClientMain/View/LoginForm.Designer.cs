namespace PaCSClientMain.View
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.registerNewUser = new System.Windows.Forms.LinkLabel();
            this.btnLogin = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.tbPwd = new DevExpress.XtraEditors.TextEdit();
            this.tbName = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPwd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // registerNewUser
            // 
            this.registerNewUser.AutoSize = true;
            this.registerNewUser.BackColor = System.Drawing.Color.Transparent;
            this.registerNewUser.LinkColor = System.Drawing.Color.DimGray;
            this.registerNewUser.Location = new System.Drawing.Point(743, 178);
            this.registerNewUser.Name = "registerNewUser";
            this.registerNewUser.Size = new System.Drawing.Size(67, 14);
            this.registerNewUser.TabIndex = 4;
            this.registerNewUser.TabStop = true;
            this.registerNewUser.Text = "注册新用户";
            this.registerNewUser.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.registerNewUser_LinkClicked);
            // 
            // btnLogin
            // 
            this.btnLogin.Appearance.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnLogin.Appearance.Options.UseFont = true;
            this.btnLogin.Location = new System.Drawing.Point(484, 402);
            this.btnLogin.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.btnLogin.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(123, 41);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "登  录";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(619, 402);
            this.btnCancel.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(123, 41);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取  消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbPwd
            // 
            this.tbPwd.Location = new System.Drawing.Point(484, 341);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPwd.Properties.Appearance.Options.UseFont = true;
            this.tbPwd.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tbPwd.Properties.AppearanceFocused.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.tbPwd.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.tbPwd.Properties.AppearanceFocused.Options.UseBorderColor = true;
            this.tbPwd.Properties.NullValuePrompt = "在此输入密码";
            this.tbPwd.Properties.NullValuePromptShowForEmptyValue = true;
            this.tbPwd.Properties.PasswordChar = '●';
            this.tbPwd.Size = new System.Drawing.Size(258, 30);
            this.tbPwd.TabIndex = 1;
            this.tbPwd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPwd_KeyDown);
            // 
            // tbName
            // 
            this.tbName.EnterMoveNextControl = true;
            this.tbName.Location = new System.Drawing.Point(484, 288);
            this.tbName.Name = "tbName";
            this.tbName.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.tbName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbName.Properties.Appearance.Options.UseFont = true;
            this.tbName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tbName.Properties.AppearanceFocused.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.tbName.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.tbName.Properties.AppearanceFocused.Options.UseBorderColor = true;
            this.tbName.Properties.NullValuePrompt = "在此输入用户名";
            this.tbName.Properties.NullValuePromptShowForEmptyValue = true;
            this.tbName.Size = new System.Drawing.Size(258, 30);
            this.tbName.TabIndex = 0;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.None;
            this.BackgroundImageStore = global::PaCSClientMain.Properties.Resources.pacs_login3;
            this.ClientSize = new System.Drawing.Size(1200, 746);
            this.Controls.Add(this.tbPwd);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.registerNewUser);
            this.Controls.Add(this.tbName);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Partner Collaboration System";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.Shown += new System.EventHandler(this.LoginForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.tbPwd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel registerNewUser;
        private DevExpress.XtraEditors.SimpleButton btnLogin;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.TextEdit tbPwd;
        private DevExpress.XtraEditors.TextEdit tbName;
    }
}