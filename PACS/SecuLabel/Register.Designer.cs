namespace SecuLabel
{
    partial class Register
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Register));
            this.tbnExit = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.txtQty = new DevExpress.XtraEditors.TextEdit();
            this.txtMeterialCode = new DevExpress.XtraEditors.TextEdit();
            this.cbBarcode = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtQty.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMeterialCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbBarcode.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tbnExit
            // 
            this.tbnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbnExit.Image = ((System.Drawing.Image)(resources.GetObject("tbnExit.Image")));
            this.tbnExit.Location = new System.Drawing.Point(202, 211);
            this.tbnExit.Name = "tbnExit";
            this.tbnExit.Size = new System.Drawing.Size(87, 27);
            this.tbnExit.TabIndex = 6;
            this.tbnExit.Text = "关闭";
            this.tbnExit.Click += new System.EventHandler(this.tbnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(100, 211);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 27);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.txtQty);
            this.panelControl1.Controls.Add(this.txtMeterialCode);
            this.panelControl1.Controls.Add(this.cbBarcode);
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.labelControl4);
            this.panelControl1.Location = new System.Drawing.Point(7, 9);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(366, 175);
            this.panelControl1.TabIndex = 19;
            // 
            // txtQty
            // 
            this.txtQty.Location = new System.Drawing.Point(139, 75);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(130, 20);
            this.txtQty.TabIndex = 28;
            this.txtQty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQty_KeyPress);
            // 
            // txtMeterialCode
            // 
            this.txtMeterialCode.Location = new System.Drawing.Point(139, 22);
            this.txtMeterialCode.Name = "txtMeterialCode";
            this.txtMeterialCode.Size = new System.Drawing.Size(130, 20);
            this.txtMeterialCode.TabIndex = 27;
            this.txtMeterialCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMeterialCode_KeyPress);
            this.txtMeterialCode.Validated += new System.EventHandler(this.txtMeterialCode_Validated);
            // 
            // cbBarcode
            // 
            this.cbBarcode.Location = new System.Drawing.Point(139, 132);
            this.cbBarcode.Name = "cbBarcode";
            this.cbBarcode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbBarcode.Properties.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cbBarcode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbBarcode.Size = new System.Drawing.Size(72, 20);
            this.cbBarcode.TabIndex = 26;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(63, 81);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(56, 14);
            this.labelControl3.TabIndex = 24;
            this.labelControl3.Text = "包装单位 :";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(63, 135);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(63, 14);
            this.labelControl2.TabIndex = 23;
            this.labelControl2.Text = "条码(Y/N) :";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(63, 25);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(56, 14);
            this.labelControl4.TabIndex = 19;
            this.labelControl4.Text = "资材编号 :";
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 256);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.tbnExit);
            this.Controls.Add(this.btnSave);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Register";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "基本信息注册";
            this.Load += new System.EventHandler(this.Register_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtQty.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMeterialCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbBarcode.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton tbnExit;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.ComboBoxEdit cbBarcode;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtQty;
        private DevExpress.XtraEditors.TextEdit txtMeterialCode;
    }
}