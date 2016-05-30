namespace PaCS
{
    partial class RoleManage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoleManage));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.treeViewRole = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.btnRoleDel = new System.Windows.Forms.Button();
            this.btnRoleAdd = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnUserDel = new System.Windows.Forms.Button();
            this.btnUserAdd = new System.Windows.Forms.Button();
            this.userListView = new System.Windows.Forms.ListView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAuthDel = new System.Windows.Forms.Button();
            this.btnAuthEdit = new System.Windows.Forms.Button();
            this.authListView = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.btnRoleSave = new System.Windows.Forms.Button();
            this.tbDesc = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.btnRoleDel);
            this.splitContainer1.Panel1.Controls.Add(this.btnRoleAdd);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(724, 550);
            this.splitContainer1.SplitterDistance = 235;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.treeViewRole);
            this.groupBox4.Location = new System.Drawing.Point(3, 42);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(229, 504);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "角色列表";
            // 
            // treeViewRole
            // 
            this.treeViewRole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewRole.ImageIndex = 1;
            this.treeViewRole.ImageList = this.imageList1;
            this.treeViewRole.Location = new System.Drawing.Point(3, 17);
            this.treeViewRole.Name = "treeViewRole";
            this.treeViewRole.SelectedImageIndex = 1;
            this.treeViewRole.Size = new System.Drawing.Size(223, 484);
            this.treeViewRole.TabIndex = 0;
            this.treeViewRole.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewRole_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "user.png");
            this.imageList1.Images.SetKeyName(1, "角色.png");
            this.imageList1.Images.SetKeyName(2, "用户组.png");
            this.imageList1.Images.SetKeyName(3, "钥匙.png");
            // 
            // btnRoleDel
            // 
            this.btnRoleDel.Location = new System.Drawing.Point(119, 13);
            this.btnRoleDel.Name = "btnRoleDel";
            this.btnRoleDel.Size = new System.Drawing.Size(75, 23);
            this.btnRoleDel.TabIndex = 2;
            this.btnRoleDel.Text = "删除";
            this.btnRoleDel.UseVisualStyleBackColor = true;
            this.btnRoleDel.Click += new System.EventHandler(this.btnRoleDel_Click);
            // 
            // btnRoleAdd
            // 
            this.btnRoleAdd.Location = new System.Drawing.Point(25, 13);
            this.btnRoleAdd.Name = "btnRoleAdd";
            this.btnRoleAdd.Size = new System.Drawing.Size(75, 23);
            this.btnRoleAdd.TabIndex = 1;
            this.btnRoleAdd.Text = "添加";
            this.btnRoleAdd.UseVisualStyleBackColor = true;
            this.btnRoleAdd.Click += new System.EventHandler(this.btnRoleAdd_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(485, 550);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(477, 524);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "角色信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.btnUserDel);
            this.groupBox3.Controls.Add(this.btnUserAdd);
            this.groupBox3.Controls.Add(this.userListView);
            this.groupBox3.Location = new System.Drawing.Point(293, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(181, 508);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "包含用户";
            // 
            // btnUserDel
            // 
            this.btnUserDel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUserDel.Location = new System.Drawing.Point(101, 477);
            this.btnUserDel.Name = "btnUserDel";
            this.btnUserDel.Size = new System.Drawing.Size(75, 23);
            this.btnUserDel.TabIndex = 2;
            this.btnUserDel.Text = "移除";
            this.btnUserDel.UseVisualStyleBackColor = true;
            this.btnUserDel.Click += new System.EventHandler(this.btnUserDel_Click);
            // 
            // btnUserAdd
            // 
            this.btnUserAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUserAdd.Location = new System.Drawing.Point(8, 477);
            this.btnUserAdd.Name = "btnUserAdd";
            this.btnUserAdd.Size = new System.Drawing.Size(75, 23);
            this.btnUserAdd.TabIndex = 1;
            this.btnUserAdd.Text = "编辑";
            this.btnUserAdd.UseVisualStyleBackColor = true;
            this.btnUserAdd.Click += new System.EventHandler(this.btnUserAdd_Click);
            // 
            // userListView
            // 
            this.userListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userListView.Location = new System.Drawing.Point(8, 20);
            this.userListView.Name = "userListView";
            this.userListView.Size = new System.Drawing.Size(168, 451);
            this.userListView.TabIndex = 0;
            this.userListView.UseCompatibleStateImageBehavior = false;
            this.userListView.View = System.Windows.Forms.View.List;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnAuthDel);
            this.groupBox2.Controls.Add(this.btnAuthEdit);
            this.groupBox2.Controls.Add(this.authListView);
            this.groupBox2.Location = new System.Drawing.Point(6, 200);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(281, 316);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "可操作功能";
            // 
            // btnAuthDel
            // 
            this.btnAuthDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAuthDel.Location = new System.Drawing.Point(200, 284);
            this.btnAuthDel.Name = "btnAuthDel";
            this.btnAuthDel.Size = new System.Drawing.Size(75, 23);
            this.btnAuthDel.TabIndex = 2;
            this.btnAuthDel.Text = "移除";
            this.btnAuthDel.UseVisualStyleBackColor = true;
            this.btnAuthDel.Click += new System.EventHandler(this.btnAuthDel_Click);
            // 
            // btnAuthEdit
            // 
            this.btnAuthEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAuthEdit.Location = new System.Drawing.Point(107, 284);
            this.btnAuthEdit.Name = "btnAuthEdit";
            this.btnAuthEdit.Size = new System.Drawing.Size(75, 23);
            this.btnAuthEdit.TabIndex = 1;
            this.btnAuthEdit.Text = "编辑";
            this.btnAuthEdit.UseVisualStyleBackColor = true;
            this.btnAuthEdit.Click += new System.EventHandler(this.btnAuthEdit_Click);
            // 
            // authListView
            // 
            this.authListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authListView.Location = new System.Drawing.Point(6, 20);
            this.authListView.Name = "authListView";
            this.authListView.Size = new System.Drawing.Size(269, 256);
            this.authListView.TabIndex = 0;
            this.authListView.UseCompatibleStateImageBehavior = false;
            this.authListView.View = System.Windows.Forms.View.List;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lbStatus);
            this.groupBox1.Controls.Add(this.btnRoleSave);
            this.groupBox1.Controls.Add(this.tbDesc);
            this.groupBox1.Controls.Add(this.tbName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 175);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "角色详细信息";
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.ForeColor = System.Drawing.Color.Red;
            this.lbStatus.Location = new System.Drawing.Point(33, 130);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(17, 12);
            this.lbStatus.TabIndex = 9;
            this.lbStatus.Text = "ll";
            // 
            // btnRoleSave
            // 
            this.btnRoleSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRoleSave.Location = new System.Drawing.Point(189, 125);
            this.btnRoleSave.Name = "btnRoleSave";
            this.btnRoleSave.Size = new System.Drawing.Size(75, 23);
            this.btnRoleSave.TabIndex = 8;
            this.btnRoleSave.Text = "保存";
            this.btnRoleSave.UseVisualStyleBackColor = true;
            this.btnRoleSave.Click += new System.EventHandler(this.btnRoleSave_Click);
            // 
            // tbDesc
            // 
            this.tbDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDesc.Location = new System.Drawing.Point(74, 56);
            this.tbDesc.Multiline = true;
            this.tbDesc.Name = "tbDesc";
            this.tbDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDesc.Size = new System.Drawing.Size(190, 44);
            this.tbDesc.TabIndex = 7;
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Location = new System.Drawing.Point(74, 19);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(190, 21);
            this.tbName.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "描述:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名称(*):";
            // 
            // RoleManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 550);
            this.Controls.Add(this.splitContainer1);
            this.Name = "RoleManage";
            this.Text = "角色管理";
            this.Load += new System.EventHandler(this.RoleManage_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView userListView;
        private System.Windows.Forms.Button btnUserAdd;
        private System.Windows.Forms.Button btnUserDel;
        private System.Windows.Forms.Button btnRoleAdd;
        private System.Windows.Forms.Button btnRoleDel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbDesc;
        private System.Windows.Forms.Button btnRoleSave;
        private System.Windows.Forms.ListView authListView;
        private System.Windows.Forms.Button btnAuthDel;
        private System.Windows.Forms.Button btnAuthEdit;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TreeView treeViewRole;
        private System.Windows.Forms.Label lbStatus;
    }
}