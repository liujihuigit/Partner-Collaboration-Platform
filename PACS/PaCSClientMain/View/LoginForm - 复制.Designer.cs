namespace PaCSClientMain.View
{
    partial class LoginForm2
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
            this.lbStatus = new DevExpress.XtraEditors.LabelControl();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.tbPwd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // registerNewUser
            // 
            this.registerNewUser.AutoSize = true;
            this.registerNewUser.BackColor = System.Drawing.Color.Transparent;
            this.registerNewUser.Location = new System.Drawing.Point(517, 264);
            this.registerNewUser.Name = "registerNewUser";
            this.registerNewUser.Size = new System.Drawing.Size(65, 12);
            this.registerNewUser.TabIndex = 4;
            this.registerNewUser.TabStop = true;
            this.registerNewUser.Text = "注册新用户";
            this.registerNewUser.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.registerNewUser_LinkClicked);
            // 
            // btnLogin
            // 
            this.btnLogin.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnLogin.Location = new System.Drawing.Point(392, 198);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(76, 27);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "登录";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnCancel.Location = new System.Drawing.Point(475, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 27);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbPwd
            // 
            this.tbPwd.Location = new System.Drawing.Point(385, 168);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tbPwd.Properties.AppearanceFocused.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.tbPwd.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.tbPwd.Properties.AppearanceFocused.Options.UseBorderColor = true;
            this.tbPwd.Properties.NullValuePrompt = "在此输入密码";
            this.tbPwd.Properties.NullValuePromptShowForEmptyValue = true;
            this.tbPwd.Properties.PasswordChar = '●';
            this.tbPwd.Size = new System.Drawing.Size(165, 20);
            this.tbPwd.TabIndex = 1;
            this.tbPwd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPwd_KeyDown);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(385, 138);
            this.tbName.Name = "tbName";
            this.tbName.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.tbName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tbName.Properties.AppearanceFocused.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.tbName.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.tbName.Properties.AppearanceFocused.Options.UseBorderColor = true;
            this.tbName.Properties.NullValuePrompt = "在此输入用户名";
            this.tbName.Properties.NullValuePromptShowForEmptyValue = true;
            this.tbName.Size = new System.Drawing.Size(165, 20);
            this.tbName.TabIndex = 0;
            this.tbName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbName_KeyDown);
            // 
            // lbStatus
            // 
            this.lbStatus.Appearance.ForeColor = System.Drawing.Color.Green;
            this.lbStatus.Location = new System.Drawing.Point(395, 237);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(0, 14);
            this.lbStatus.TabIndex = 5;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(609, 328);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.tbPwd);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.registerNewUser);
            this.Controls.Add(this.tbName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统登录";
            this.Load += new System.EventHandler(this.LoginForm_Load);
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
        private DevExpress.XtraEditors.LabelControl lbStatus;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}