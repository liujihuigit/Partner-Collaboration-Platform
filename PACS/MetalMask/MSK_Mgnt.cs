using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PaCSTools;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Data.OracleClient;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System.Collections;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;
using System.IO.Ports;

namespace MetalMask
{
    public partial class MSK_Mgnt : Form
    {
        private bool m_checkStatus = false;
        private string nodeName = "";
        private SerialPort[] ports = new SerialPort[1];
        public MSK_Mgnt()
        {
            InitializeComponent();
            ports[0] = MetalMaskGlobal.port;
        }

        private void Report_Load(object sender, EventArgs e)
        {
            gridView1.OptionsCustomization.AllowColumnMoving = false;//禁止列拖动
            InitFunction();//初始化权限列表
            InitQueryCondition();//初始化查询条件数据源

            //实现DevExpress.GridControl表头全选功能
            this.gridView1.Click += new System.EventHandler(this.gridView1_Click);
            this.gridView1.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridView1_CustomDrawColumnHeader);
            this.gridView1.DataSourceChanged += new EventHandler(gridView1_DataSourceChanged);

            PaCSGlobal.InitComPort("MetalMask", "", ports);
        }


        #region 实现DevExpress.GridControl表头全选功能
        private void gridView1_Click(object sender, EventArgs e)
        {
            if (DevControlHelper.ClickGridCheckBox(this.gridView1, "check", m_checkStatus))
            {
                m_checkStatus = !m_checkStatus;
            }
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column != null && e.Column.FieldName == "check")
            {
                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);
                DevControlHelper.DrawCheckBox(e, m_checkStatus);
                e.Handled = true;
            }
        }

        void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            GridColumn column = this.gridView1.Columns.ColumnByFieldName("check");
            if (column != null)
            {
                column.Width = 80;
                column.OptionsColumn.ShowCaption = false;
                column.ColumnEdit = new RepositoryItemCheckEdit();
            }
        } 
        #endregion

        /// <summary>
        /// 初始化查询条件
        /// </summary>
        private void InitQueryCondition()
        {
            try
            {
                MetalMaskGlobal.LoadCmbStatus(cmbStatus);
                MetalMaskGlobal.LoadCmbMaskCode(cmbMaskCode);
                MetalMaskGlobal.LoadCmbModel(cmbModel);
                MetalMaskGlobal.LoadCmbVendor(cmbVendor);

                cmbVendType.SelectedIndex = 0;
                if (!PaCSGlobal.LoginUserInfo.Venderid.Equals("C660"))
                {
                    cmbVendor.Properties.ReadOnly = true;
                    cmbVendor.Text = PaCSGlobal.LoginUserInfo.Vendername;
                }
                else
                {
                    cmbVendor.SelectedIndex = 0;
                }

                dateEditFrom.Text = PaCSGlobal.GetServerDateTime(3);
                dateEditTo.Text = PaCSGlobal.GetServerDateTime(3);
                panelControl1.Enabled = false;
            }
            catch (Exception InitQueryCombobox)
            {
                XtraMessageBox.Show(this, "System error[InitQueryCombobox]: " + InitQueryCombobox.Message);
            }
        }

        /// <summary>
        /// 初始化权限列表
        /// </summary>
        private void InitFunction()
        {
            try
            {
                int count = this.panel1.Controls.Count;
                foreach (Control btn in this.panel1.Controls)
                {
                    if (btn is SimpleButton)
                    {
                        string btnFunId = btn.Tag == null ? "" : btn.Tag.ToString();
                        btn.Enabled = PaCSGlobal.HasFunction(btnFunId);
                    }
                }
            }
            catch (Exception InitFunction)
            {
                XtraMessageBox.Show(this, "System error[InitFunction]: " + InitFunction.Message);
            }
        }


        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));
                //MetalMaskGlobal.waitForm.ShowDialog();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("查询失败", "提示");
            }
        }

        /// <summary>
        /// 生成行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        /// <summary>
        /// 获得选择的厂家
        /// </summary>
        /// <returns></returns>
        public  string GetVendor()
        {
            string vendor = "";
            if (cmbVendor.SelectedIndex != -1)
                vendor = (cmbVendor.SelectedItem as ComboxData).Value;
            else
                vendor = PaCSGlobal.LoginUserInfo.Venderid;
            return vendor;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //日期条件
            string AllorPeriod = radioGroup1.SelectedIndex == 0 ? "All" : "Period";
            string RegisterorUpdate = radioGroup2.SelectedIndex == 0 ? "Register" : "Update";
            string dateFrom = dateEditFrom.Text.Trim();
            string dateTo = dateEditTo.Text.Trim();

            string barcode = tbBarcode.Text.Trim();
            string maskcode = "";
            string model = "";
            string status = "";
            string vendor = "";
            if (cmbMaskCode.SelectedIndex != -1)
                maskcode = cmbMaskCode.Properties.Items[cmbMaskCode.SelectedIndex].ToString();
            if (cmbModel.SelectedIndex != -1)
                model = cmbModel.Properties.Items[cmbModel.SelectedIndex].ToString();
            if (cmbStatus.SelectedIndex != -1)
                status = (cmbStatus.SelectedItem as ComboxData).Value;
            if (cmbVendor.SelectedIndex != -1)
                vendor = (cmbVendor.SelectedItem as ComboxData).Value;

            StringBuilder sql = new StringBuilder("select * from vwm_rm_eqp_metalmask where fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");
            if (!string.IsNullOrEmpty(barcode))
            {
                sql.Append(" and \"BarcodeNO\" like '%" + barcode + "%'");
            }
            if (!string.IsNullOrEmpty(maskcode))
            {
                sql.Append(" and \"MaskCode\" = '" + maskcode + "'");
            }
            if (!string.IsNullOrEmpty(model))
            {
                sql.Append(" and \"ProductModel\" = '" + model + "'");
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql.Append(" and STATUS_CODE = '" + status + "'");
            }

            if (cmbVendor.Properties.ReadOnly)//社外厂家,厂家下拉框禁用
            {
                if (cmbVendType.SelectedIndex == 0)//使用厂家
                {
                    sql.Append(" and VEND_LOC_CODE = '" + PaCSGlobal.LoginUserInfo.Venderid + "'");
                }
                else//所属厂家
                {
                    sql.Append(" and VEND_CODE = '" + PaCSGlobal.LoginUserInfo.Venderid + "'");
                }
            }
            else//社内:可选厂家
            {
                if (cmbVendType.SelectedIndex == 0)//使用厂家
                {
                    sql.Append(" and VEND_LOC_CODE = '" + vendor + "'");
                }
                else//所属厂家
                {
                    sql.Append(" and VEND_CODE = '" + vendor + "'");
                }
            }

            if (AllorPeriod.Equals("Period"))
            {
                if (RegisterorUpdate.Equals("Register"))
                {
                    sql.Append(" and \"RegisteredOn\" between '" + dateFrom + "' and '" + dateTo + "'");
                }
                else if (RegisterorUpdate.Equals("Update"))
                {
                    sql.Append(" and \"UpdatedOn\" between '" + dateFrom + "' and '" + dateTo + "'");
                }
            }
            //sql.Append(" and  '" + PaCSGlobal.LoginUserInfo.Venderid + "' = 'C660' or  (VEND_CODE = '" + PaCSGlobal.LoginUserInfo.Venderid + "' or VEND_LOC_CODE = '" + PaCSGlobal.LoginUserInfo.Venderid + "')");
            sql.Append(" order by \"UpdatedOn\" desc nulls last ");
            DataTable dt = OracleHelper.ExecuteDataTable(sql.ToString());
            dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
            dt.Columns["check"].SetOrdinal(0);

            //checkbox列去掉中间状态
            RepositoryItemCheckEdit checkEdit = new RepositoryItemCheckEdit();
            checkEdit.NullStyle = StyleIndeterminate.Unchecked;

            this.Invoke((MethodInvoker)delegate
           {
               gridControl1.DataSource = dt;
               gridView1.BestFitColumns();
               //隐藏某些列
               gridView1.Columns["VEND_CODE"].Visible = false;
               gridView1.Columns["VEND_LOC_CODE"].Visible = false;
               gridView1.Columns["ToolType"].Visible = false;
               gridView1.Columns["STATUS_CODE"].Visible = false;

               gridView1.Columns["MAKE_VEND_CODE"].Visible = false;
               gridView1.Columns["MAKE_RSN_CODE"].Visible = false;
               gridView1.Columns["DSU_RSN_CODE"].Visible = false;

               gridView1.Columns["MAKE_VEND_CODE"].Visible = false;
               gridView1.Columns["MAKE_RSN_CODE"].Visible = false;
               gridView1.Columns["ATTACHID"].Visible = false;
               //调整格式
               gridView1.Columns["check"].Width = 30;
               gridView1.Columns["check"].ColumnEdit = checkEdit;
               gridView1.Columns["所属厂家"].Width = 100;
               gridView1.Columns["所属厂家"].SummaryItem.SummaryType = SummaryItemType.Count;
               gridView1.Columns["所属厂家"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";
               //冻结前5列
               gridView1.Columns["check"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
               gridView1.Columns["所属厂家"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
               gridView1.Columns["使用厂家"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
               gridView1.Columns["BarcodeNO"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
               gridView1.Columns["MaskCode"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
               gridView1.Columns["PCB Code"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
               gridView1.Columns["ProductModel"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

               foreach (GridColumn col in gridView1.Columns)
               {
                   if (col.FieldName.Equals("check"))
                   {
                       col.OptionsColumn.AllowEdit = true;
                       col.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                   }
                   else
                       col.OptionsColumn.AllowEdit = false;
               }
           });
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           // MetalMaskGlobal.waitForm.Close();
            SplashScreenManager.CloseForm();  
        }

        public List<string> barcodeList()
        {
            List<string> barcodeList = new List<string>();
            string value = "";
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                value = gridView1.GetDataRow(i)["check"].ToString();
                if (value == "True")
                {
                    barcodeList.Add(gridView1.GetRowCellValue(i, gridView1.Columns["BarcodeNO"]).ToString());
                }
            }
            return barcodeList;
        }

        /// <summary>
        /// 所属厂家列表
        /// </summary>
        /// <returns></returns>
        public List<string> vendList()
        {
            List<string> vendList = new List<string>();
            string value = "";
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                value = gridView1.GetDataRow(i)["check"].ToString();
                if (value == "True")
                {
                    vendList.Add(gridView1.GetRowCellValue(i, gridView1.Columns["VEND_CODE"]).ToString());
                }
            }
            return vendList;
        }

        /// <summary>
        /// 使用厂家列表
        /// </summary>
        /// <returns></returns>
        public List<string> venLocList()
        {
            List<string> venLocList = new List<string>();
            string value = "";
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                value = gridView1.GetDataRow(i)["check"].ToString();
                if (value == "True")
                {
                    venLocList.Add(gridView1.GetRowCellValue(i, gridView1.Columns["VEND_LOC_CODE"]).ToString());
                }
            }
            return venLocList;
        }

        /// <summary>
        /// 状态列表
        /// </summary>
        /// <returns></returns>
        public List<string> statusCodeList()
        {
            List<string> statusList = new List<string>();
            string value = "";
            //string statusNm = "";
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                value = gridView1.GetDataRow(i)["check"].ToString();
                if (value == "True")
                {
                    statusList.Add(gridView1.GetRowCellValue(i, gridView1.Columns["STATUS_CODE"]).ToString());
                }
            }
            return statusList;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                Register frmNew = new Register();
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnApply_Click(sender, e);
                }
            }
            catch (Exception btnNew_Click)
            {
                XtraMessageBox.Show(this, "System error[btnNew_Click]: " + btnNew_Click.Message);
            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string currentLoc in venLocList())
                {
                    if (currentLoc != PaCSGlobal.LoginUserInfo.Venderid)
                    {
                        XtraMessageBox.Show(MetalMaskGlobal.ReportBtnClickTip, "提示");
                        return;
                    }
                }

                foreach (string currentStatus in statusCodeList())
                {
                    if (!currentStatus.Equals("MBDSU"))
                    {
                        XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能修改", "提示");
                        return;
                    }
                }

                ModifyForm frmNew = new ModifyForm(barcodeList());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnApply_Click(sender, e);
                }
            }
            catch (Exception btnModify_Click)
            {
                XtraMessageBox.Show(this, "System error[btnModify_Click]: " + btnModify_Click.Message);
            }
        }

        private void btnGI_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string currentLoc in venLocList())
                {
                    if (currentLoc != PaCSGlobal.LoginUserInfo.Venderid)
                    {
                        XtraMessageBox.Show(MetalMaskGlobal.ReportBtnClickTip, "提示");
                        return;
                    }
                }

                foreach (string currentStatus in statusCodeList())
                {
                    if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBLIN"))
                    {
                        XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBLIN"), "提示");
                        return;
                    }
                }

                GIForm frmNew = new GIForm(barcodeList());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnApply_Click(sender, e);
                }
            }
            catch (Exception btnGI_Click)
            {
                XtraMessageBox.Show(this, "System error[btnGI_Click]: " + btnGI_Click.Message);
            }
        }

        private void btnGR_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string currentLoc in venLocList())
                {
                    if (currentLoc != PaCSGlobal.LoginUserInfo.Venderid)
                    {
                        XtraMessageBox.Show(MetalMaskGlobal.ReportBtnClickTip, "提示");
                        return;
                    }
                }

                foreach (string currentStatus in statusCodeList())
                {
                    if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBBIN"))
                    {
                        XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBBIN"), "提示");
                        return;
                    }
                }

                GRForm frmNew = new GRForm(barcodeList());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnApply_Click(sender, e);
                }
            }
            catch (Exception btnGR_Click)
            {
                XtraMessageBox.Show(this, "System error[btnGR_Click]: " + btnGR_Click.Message);
            }
        }

        private void btnCarryIn_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string currentLoc in venLocList())
                {
                    if (currentLoc != PaCSGlobal.LoginUserInfo.Venderid)
                    {
                        XtraMessageBox.Show(MetalMaskGlobal.ReportBtnClickTip, "提示");
                        return;
                    }
                }

                foreach (string currentStatus in statusCodeList())
                {
                    if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBCIN"))
                    {
                        XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBCIN"), "提示");
                        return;
                    }
                }

                CarryInForm frmNew = new CarryInForm(barcodeList());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnApply_Click(sender, e);
                }
            }
            catch (Exception btnCarryIn_Click)
            {
                XtraMessageBox.Show(this, "System error[btnCarryIn_Click]: " + btnCarryIn_Click.Message);
            }
        }


        private void btnCarryOut_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string currentLoc in venLocList())
                {
                    if (currentLoc != PaCSGlobal.LoginUserInfo.Venderid)
                    {
                        XtraMessageBox.Show(MetalMaskGlobal.ReportBtnClickTip, "提示");
                        return;
                    }
                }

                foreach (string currentStatus in statusCodeList())
                {
                    if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBCOT"))
                    {
                        XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBCOT"), "提示");
                        return;
                    }
                }

                CarryOutForm frmNew = new CarryOutForm(barcodeList());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnApply_Click(sender, e);
                }
            }
            catch (Exception btnCarryOut_Click)
            {
                XtraMessageBox.Show(this, "System error[btnCarryOut_Click]: " + btnCarryOut_Click.Message);
            }
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string currentLoc in venLocList())
                {
                    if (currentLoc != PaCSGlobal.LoginUserInfo.Venderid)
                    {
                        XtraMessageBox.Show(MetalMaskGlobal.ReportBtnClickTip, "提示");
                        return;
                    }
                }

                foreach (string currentStatus in statusCodeList())
                {
                    if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBCLN"))
                    {
                        XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBCLN"), "提示");
                        return;
                    }
                }

                CleanForm frmNew = new CleanForm(barcodeList());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnApply_Click(sender, e);
                }
            }
            catch (Exception btnClean_Click)
            {
                XtraMessageBox.Show(this, "System error[btnClean_Click]: " + btnClean_Click.Message);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string currentLoc in venLocList())
                {
                    if (currentLoc != PaCSGlobal.LoginUserInfo.Venderid)
                    {
                        XtraMessageBox.Show(MetalMaskGlobal.ReportBtnClickTip, "提示");
                        return;
                    }
                }

                foreach (string currentStatus in statusCodeList())
                {
                    if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBDEL"))
                    {
                        XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBDEL"), "提示");
                        return;
                    }
                }

                DeleteForm frmNew = new DeleteForm(barcodeList());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnApply_Click(sender, e);
                }
            }
            catch (Exception btnClean_Click)
            {
                XtraMessageBox.Show(this, "System error[btnClean_Click]: " + btnClean_Click.Message);
            }
        }

        private void btnDispose_Click(object sender, EventArgs e)
        {
            try
            {
                //只有所属厂家才能废弃 vendList()
                foreach (string vend in vendList())
                {
                    if (vend != PaCSGlobal.LoginUserInfo.Venderid)
                    {
                        XtraMessageBox.Show("只有所属厂家才能废弃", "提示");
                        return;
                    }
                }

                foreach (string currentStatus in statusCodeList())
                {
                    if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBDSU"))
                    {
                        XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBDSU"), "提示");
                        return;
                    }
                }

                DisposeForm frmNew = new DisposeForm(barcodeList());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnApply_Click(sender, e);
                }
            }
            catch (Exception btnClean_Click)
            {
                XtraMessageBox.Show(this, "System error[btnClean_Click]: " + btnClean_Click.Message);
            }
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioGroup1.SelectedIndex == 0)
            {
                panelControl1.Enabled = false;
            }
            else
                panelControl1.Enabled = true;
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            GridView detailGrid = (sender as GridView);
            GridHitInfo hitInfo = (detailGrid.CalcHitInfo((e as MouseEventArgs).Location));
            string imageId = "";
            //Leave if the user didn't double-click in a cell
            if (hitInfo.InRowCell == false)
                return;
            //Leave if the user didn't double-click in the "Person Name" column
            //if (hitInfo.Column != detailGrid.Columns["PersonName"])
            //    return;
            nodeName = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "BarcodeNO").ToString();
            imageId = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "ATTACHID").ToString();
            DetailForm frmNew = new DetailForm(nodeName, imageId);
            DialogResult dg = frmNew.ShowDialog();
            if (dg == DialogResult.OK)
            {

            }
            //XtraMessageBox.Show(nodeName);
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            try
            {
                SettingForm setcom = new SettingForm("MetalMask", "", 1);
                DialogResult dg = setcom.ShowDialog();
           
                if (dg == DialogResult.OK)
                {
                    PaCSGlobal.InitComPort("MetalMask", "", ports);
                }
            }
            catch (Exception btnNew_Click)
            {
                XtraMessageBox.Show(this, "System error[btnNew_Click]: " + btnNew_Click.Message);
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            //PaCSGlobal.ExportGridViewToExcel(gridView1, "MetalMask");
            PaCSGlobal.ExportGridToFile(gridView1, "MetalMask");
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            try
            {
                HistoryForm frmNew = new HistoryForm(this);
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {

                }
            }
            catch (Exception btnNew_Click)
            {
                XtraMessageBox.Show(this, "System error[btnNew_Click]: " + btnNew_Click.Message);
            }
        }

        private void Report_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MetalMaskGlobal.port.IsOpen)
                MetalMaskGlobal.port.Close();
        }

        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (gridView1.GetDataRow(e.RowHandle) == null)
                return;
            string usdTimes = gridView1.GetDataRow(e.RowHandle)["使用次数"].ToString();
            if (int.Parse(usdTimes) > 79000)
            {
                e.Appearance.BackColor = Color.LightPink;
            }

            if (e.RowHandle == gridView1.FocusedRowHandle)
            {
                e.Appearance.ForeColor = Color.White;
                e.Appearance.BackColor = Color.CornflowerBlue;
            }
        }




    }
}
