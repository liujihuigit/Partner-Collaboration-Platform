using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PaCSTools;
using DevExpress.Data;
using DevExpress.XtraSplashScreen;

namespace Toner
{
    public partial class TonerDetail : DevExpress.XtraEditors.XtraForm
    {
        PaCSGlobal global = new PaCSGlobal();
        private string dateFrom = "";
        private string dateTo = "";
        private string boxlabel = "";
        private string makeVendor = "";
        private string item = "";
        private string vendor = "";
        private string status = "";
        public TonerDetail()
        {
            InitializeComponent();
            global.InitMenu();
        }

        private void TonerDetail_Load(object sender, EventArgs e)
        {
            cmbMakeVendor.Properties.BeginUpdate();

            ComboxData data = new ComboxData();
            data.Text = "";
            data.Value = "";
            cmbMakeVendor.Properties.Items.Add(data);
            TonerGlobal.LoadCmbVendor(cmbMakeVendor);

            cmbMakeVendor.Properties.EndUpdate();



            cmbVendor.Properties.BeginUpdate();

            ComboxData data2 = new ComboxData();
            data2.Text = "";
            data2.Value = "";
            cmbVendor.Properties.Items.Add(data2);
            TonerGlobal.LoadCmbUseVendor(cmbVendor, false);

            cmbVendor.Properties.EndUpdate();

            //if (!PaCSGlobal.LoginUserInfo.Venderid.Equals("C660"))
            //{
            //    cmbVendor.Properties.ReadOnly = true;
            //    cmbVendor.Text = PaCSGlobal.LoginUserInfo.Vendername;
            //}
            //else
            //{
            //    cmbVendor.SelectedIndex = 0;
            //}

            cmbStatus.Properties.BeginUpdate();

            ComboxData data3 = new ComboxData();
            data3.Text = "";
            data3.Value = "";
            cmbStatus.Properties.Items.Add(data3);
            TonerGlobal.LoadBoxCaseStatus(cmbStatus);

            cmbStatus.Properties.EndUpdate();

            dateEditFrom.Text = PaCSGlobal.GetServerDateTimeRange("-", -7 );
            dateEditTo.Text = PaCSGlobal.GetServerDateTime(3);
        }

        private DataTable GetData()
        {
            StringBuilder sql = new StringBuilder(
            "   select box_label \"桶编号\",item \"材料编号\",make_vendor_nm  \"生产厂家\",lot_no LOTNO,box_no BOXNO,qty \"单位\",box_status \"开封状态\",transit_status \"运输状态\",vend_loc_nm  \"当前所在厂家\",stock_loc \"仓库位置\",buffer_loc \"楼层\",line_loc \"生产线\"," +
            "     ejh_gr_date \"原材料仓库入库日期\",ejh_gi_date \"原材料仓库出库日期\",cj_gr_date \"厂家入库日期\", cj_gi_date \"厂家出库日期\",cj_input_date \"厂家投入日期\",scrap_date \"废弃日期\"                      " +
            "    from (select a.box_label,a.item,a.make_vend_code,(select vend_nm_cn from pacsm_md_vend b where b.vend_code = a.make_vend_code and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "')  make_vendor_nm,a.lot_no,a.box_no,a.qty,a.box_status,"+
            "          (select comm_code_nm from pacsc_md_comm_code b where b.comm_code=a.box_case_status and b.type_code='PACS_BOX_CASE_STATUS' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) transit_status,a.box_case_status,   " +//(select vend_nm_cn from pacsm_md_vend b where a.final_vend_from = b.vend_code)  \"来自厂家\",
            "          (select vend_nm_cn from pacsm_md_vend b where a.final_vend_to = b.vend_code and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') vend_loc_nm,a.final_vend_to,(select comm_code_nm from pacsc_md_comm_code t where t.type_code ='PACS_BOX_STOCK' and t.comm_code = a.final_stock_to and t.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "')  stock_loc, " +
            "          (select b.comm_code_nm line_nm from pacsc_md_comm_code b where b.comm_code=a.final_buffer_to and b.type_code='PACS_BOX_BUFFER' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') buffer_loc, (select b.comm_code_nm line_nm from pacsc_md_comm_code b where b.comm_code=a.final_line_to and b.type_code='PACS_BOX_LINE' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') line_loc,    " +
            "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss')  from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GREJH' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') ejh_gr_date,    " +
            "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GIEJH' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') ejh_gi_date,    " +
            "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GRCJ' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') cj_gr_date,  " +
            "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GICJ' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') cj_gi_date,  " +
            "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'LINE' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') cj_input_date, " +
            "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'SCRAP' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') scrap_date   " +
            "      from                                                                                                             " +
            "       (select box_label,update_date,update_time,operation_window,cancel_flag     " +
            "            from pacsp_pm_box_prgs  " +
            "          where update_date between '" + dateFrom + "'  and   '" + dateTo + "'  " +
            "             and cancel_flag is null  " +
            "             and operation_window = decode( '" + cmbDate.SelectedIndex + "','0','GREJH','1','GIEJH','2','GRCJ','3','GICJ','4','LINE','5','SCRAP','GREJH')  " +
            "             and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'  "+
            "      ) B , pacsd_pm_box a   " +
            "     where B.BOX_LABEL = A.BOX_LABEL  ");
            //"        and  ( a.final_vend_to = '" + PaCSGlobal.LoginUserInfo.Venderid + "'  or  '" + PaCSGlobal.LoginUserInfo.Venderid + "' = 'C660'        )   "  );
            sql.Append(" and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'  ");

            if (!string.IsNullOrEmpty(boxlabel))
            {
                sql.Append(" and a.box_label like '%" + boxlabel + "%' ");
            }

            if (!string.IsNullOrEmpty(makeVendor))
            {
                sql.Append(" and a.make_vend_code = '" + makeVendor + "' ");
            }

            if (!string.IsNullOrEmpty(item))
            {
                sql.Append(" and a.item like '%" + item + "%' ");
            }

            if (!string.IsNullOrEmpty(vendor))
            {
                sql.Append(" and a.final_vend_to = '" + vendor + "' ");
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql.Append(" and a.box_case_status = '" + status + "' ");
            }


             sql.Append(  " and  1=1  )  "        );                                                                                                                 
            //if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            //{
            //    string dateType = "";
            //    switch(cmbDate.SelectedIndex)
            //    {
            //        case 0://原材料仓库入库日期
            //            dateType = "ejh_gr_date";
            //            break;
            //        case 1://原材料仓库出库日期
            //                     dateType = "ejh_gi_date";
            //            break;
            //        case 2://厂家入库日期
            //                     dateType = "cj_gr_date";
            //            break;
            //        case 3://厂家出库日期
            //                     dateType = "cj_gi_date";
            //            break;
            //        case 4://厂家投入日期
            //                     dateType = "cj_input_date";
            //            break;
            //        case 5://废弃日期
            //                     dateType = "scrap_date";
            //            break;
            //    }
            //       sql.Append(" and "+dateType+" between '" + dateFrom+" 00:00:00" + "' and '" + dateTo +" 23:59:59"+ "'");
            //}

            sql.Append(" order by vend_loc_nm,ejh_gr_date ");

            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());
            return dtResult;
        }

        private void gridView1_MouseUp(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = this.gridView1.CalcHitInfo(e.Location);
            if (hi.InRow && e.Button == MouseButtons.Right)
            {
                global.CallMenu(gridView1).ShowPopup(Control.MousePosition);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            PaCSGlobal.ExportGridToFile(gridView1, "Toner_Detail");
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));
            }
            catch (Exception btnApply_Click)
            {
                XtraMessageBox.Show(this, "System error[btnApply_Click]: " + btnApply_Click.Message);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dateEditFrom.Text = PaCSGlobal.GetServerDateTimeRange("-", -7);
            dateEditTo.Text = PaCSGlobal.GetServerDateTime(3);
           
            cmbMakeVendor.SelectedIndex = -1;
            cmbVendor.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
            cmbDate.SelectedIndex = 0;

            tbBucket.Text = "";
            tbItem.Text = "";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                dateFrom = dateEditFrom.Text.Trim().Replace("-","");//2014-08-05
                dateTo = dateEditTo.Text.Trim().Replace("-", "");//2014-08-06
                boxlabel = tbBucket.Text.Trim();
                if (cmbMakeVendor.SelectedIndex > -1)
                    makeVendor = (cmbMakeVendor.SelectedItem as ComboxData).Value;//制造厂家

                item = tbItem.Text.Trim();
                //string item = "";
                //if (cmbVendor.SelectedIndex > -1)
                //    item = cmbItem.SelectedItem.ToString();

                //if (!PaCSGlobal.LoginUserInfo.Venderid.Equals("C660"))
                //{
                //    vendor = PaCSGlobal.LoginUserInfo.Venderid;
                //}
                //else
                //{
                if (cmbVendor.SelectedIndex > -1)
                    vendor = (cmbVendor.SelectedItem as ComboxData).Value;//当前厂家
                //}

                if (cmbStatus.SelectedIndex > -1)
                    status = (cmbStatus.SelectedItem as ComboxData).Value;//运输状态

                this.Invoke((MethodInvoker)delegate
                {
                    gridControl1.DataSource = GetData();
                    gridView1.BestFitColumns();
                    gridView1.Columns["桶编号"].SummaryItem.SummaryType = SummaryItemType.Count;
                    gridView1.Columns["桶编号"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";

                    gridView1.Columns["单位"].Width = 70;
                    gridView1.Columns["单位"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    gridView1.Columns["单位"].SummaryItem.DisplayFormat = "{0:f0}";
                });
            }
            catch (Exception backgroundWorker1_DoWork)
            {
                XtraMessageBox.Show(this, "System error[backgroundWorker1_DoWork]: " + backgroundWorker1_DoWork.Message);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();  
        }
       
    }
}