using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PaCSTools;
using DevExpress.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;

namespace Toner
{
    public partial class TonerAging : DevExpress.XtraEditors.XtraForm
    {
        PaCSGlobal global = new PaCSGlobal();
        public TonerAging()
        {
            InitializeComponent();
            global.InitMenu();
            gridView1.OptionsCustomization.AllowColumnMoving = false;//禁止列拖动
        }

        private void TonerAging_Load(object sender, EventArgs e)
        {

        }

        void link_OpenLink(object sender, OpenLinkEventArgs e)
        {
            if (e.EditValue.ToString().Equals("0"))
                return;

            string vendor = gridView1.GetFocusedRowCellValue("厂家").ToString();
            string qty = gridView1.GetFocusedRowCellValue("单位").ToString();
            string type = gridView1.GetFocusedRowCellValue("类型").ToString();

            string item = "";
            if (ckItem.Checked)
            {
                item = gridView1.GetFocusedRowCellValue("材料编号").ToString();
            }

            string location = "";
            if (ckLoc.Checked)
            {
                location = gridView1.GetFocusedRowCellValue("位置").ToString();
            }

            string d1 = "";
            string d2 = "";

            string clickCol = gridView1.FocusedColumn.FieldName;
            switch(clickCol)
            {
                case "一周":
                    d1 = "1";
                    d2 = "7";
                    break;
                case "二周":
                      d1 = "8";
                    d2 = "14";
                    break;
                case "三周":
                      d1 = "15";
                    d2 = "21";
                    break;
                case "四周":
                      d1 = "22";
                    d2 = "28";
                    break;
                case "大于四周":
                    d1 = "29";
                    d2 = "999999";
                    break;
                default:
                    break;
            }

            AgingDetail frmNew = new AgingDetail(vendor, qty, type,item, location, d1,d2);
            DialogResult dg = frmNew.ShowDialog();
        }


        private DataTable GetData(bool itemFlag, bool locFlag)
        {
            StringBuilder sql = new StringBuilder(
            " select vend_nm_cn \"厂家\",qty \"单位\"," + (itemFlag ? " item \"材料编号\" ," : "") + (locFlag ? "stock_location \"位置\" ," : "") + "aging_type \"类型\",																																										" +
            "          max(decode(aging_week, '1', aging_cnt, 0)) \"一周\",                                                                                                                                                          " +
            "          max(decode(aging_week, '2', aging_cnt, 0)) \"二周\",                                                                                                                                                          " +
            "          max(decode(aging_week, '3', aging_cnt, 0)) \"三周\",                                                                                                                                                          " +
            "          max(decode(aging_week, '4', aging_cnt, 0)) \"四周\",                                                                                                                                                          " +
            "          max(decode(aging_week, 'OTHER', aging_cnt, 0)) \"大于四周\"                                                                                                                                                   " +
            " from                                                                                                                                                                                                                      "+
            " (                                                                                                                                                                                                                         "+
            "     select vend_nm_cn,qty" + (itemFlag ? ",item" : "") + (locFlag ? ",stock_location" : "") + ",aging_type,aging_week,count(aging) aging_cnt                                                                                                                                  " +
            "     from                                                                                                                                                                                                                  "+
            "     (                                                                                                                                                                                                                     "+
            "         select box_label,item,qty,final_vend_to,vend_nm_cn,final_stock_to,final_stock_to_nm,final_buffer_to,final_buffer_to_nm,final_line_to,line_nm,box_case_status,box_case_status_nm,stock_location,aging_type,aging,  "+
            "                  case when aging between 0 and 7 then '1'                                                                                                                                                                 "+
            "                          when aging between 8 and 14 then '2'                                                                                                                                                             "+
            "                          when aging between 15 and 21 then '3'                                                                                                                                                            "+
            "                          when aging between 22 and 28 then '4'                                                                                                                                                            "+
            "                          else 'OTHER'                                                                                                                                                                                     "+
            "                  end as aging_week                                                                                                                                                                                        "+
            "         from                                                                                                                                                                                                              "+
            "         (                                                                                                                                                                                                                 "+
            "           select box_label,item,qty,final_vend_to,vend_nm_cn,final_stock_to,final_stock_to_nm,final_buffer_to,final_buffer_to_nm,final_line_to,line_nm,box_case_status,box_case_status_nm,                                "+
            "                    coalesce(line_nm,final_buffer_to_nm,final_stock_to_nm) stock_location,                                                                                                                                 "+
            "                    case when vendor_input_date is null then 'STOCK' else 'INPUT' end as aging_type,                                                                                                                       "+
            "                    case when vendor_input_date is null then ceil(sysdate - to_date(coalesce(cj_gr_date,ejh_gi_date,ejh_gr_date),'yyyymmddhh24miss')) else ceil(sysdate - to_date(vendor_input_date,'yyyymmddhh24miss')) end as aging                       "+
            "            from                                                                                                                                                                                                           "+
            "            (                                                                                                                                                                                                              "+
            "                 select box_label,item,qty,final_vend_to,b.vend_nm_cn,final_stock_to,f.comm_code_nm final_stock_to_nm,final_buffer_to,c.comm_code_nm final_buffer_to_nm,final_line_to,d.comm_code_nm line_nm,                           " +
            "						   box_case_status,e.comm_code_nm box_case_status_nm,                                                                                                                                               "+
            "                          (select min(update_date||update_time) from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GREJH' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') ejh_gr_date,                          " +
            "                          (select min(update_date||update_time) from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GIEJH' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') ejh_gi_date,                          " +
            "                          (select min(update_date||update_time) from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GRCJ' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') cj_gr_date,                          " +
            "                          (select min(update_date||update_time) from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'LINE' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') vendor_input_date                      " +
            "                  from pacsd_pm_box a,                                                                                                                                                                                     "+
            "                          pacsm_md_vend b,                                                                                                                                                                                 "+
            "                          pacsc_md_comm_code c,                                                                                                                                                                            "+
            "                          pacsc_md_comm_code d,                                                                                                                                                                                   " +
            "                          pacsc_md_comm_code e,                                                                                                                                                                            "+
            "                          pacsc_md_comm_code f                                                                                                                                                                             "+
            "                  where 1=1                                                                                       " +

            "                  and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "                  and a.final_vend_to = b.vend_code " +
            "                  and a.final_move_type <> '551'                                                                                                                                           " +
        // "                  and (a.final_vend_to = '" + PaCSGlobal.LoginUserInfo.Venderid + "' or  '" + PaCSGlobal.LoginUserInfo.Venderid + "' = 'C660')  " +
            "                  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "                  and c.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "                  and c.type_code(+) = 'PACS_BOX_BUFFER'                                                                                                                                                                   "+
            "                  and c.comm_code(+) = a.final_buffer_to                                                                                                                                                                   "+
            "                  and d.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "                  and d.type_code(+) = 'PACS_BOX_LINE'                                                                                                                                                                              " +
            "                  and d.comm_code(+) = a.final_line_to                                                                                                                                                                     " +
            "                  and e.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "                  and e.type_code(+) = 'PACS_BOX_CASE_STATUS'                                                                                                                                                              "+
            "                  and e.comm_code(+) = a.box_case_status                                                                                                                                                                   "+
            "                  and f.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "                  and f.type_code(+) = 'PACS_BOX_STOCK'                                                                                                                                                                    "+
            "                  and f.comm_code(+) = a.final_stock_to                                                                                                                                                                    "+     
            "            )                                                                                                                                                                                                              "+
            "         )                                                                                                                                                                                                                 "+
            "     )                                                                                                                                                                                                                     "+
            "     group by vend_nm_cn,qty" + (itemFlag ? ",item" : "") + (locFlag ? ",stock_location" : "") + ",aging_type,aging_week                                                                                                                                                     " +
            " )                                                                                                                                                                                                                         "+
            " group by vend_nm_cn,qty" + (itemFlag ? ",item" : "") + (locFlag ? ",stock_location" : "") + ",aging_type           order by 1                                             ");

            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());
            return dtResult;
        }

        private void ckItem_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    gridView1.Columns.Clear();
            //    gridControl1.DataSource = GetData(ckItem.Checked, ckLoc.Checked);
            //    gridView1.BestFitColumns();
            //    gridView1.Columns["厂家"].Width = 120;
            //    gridView1.Columns["厂家"].SummaryItem.SummaryType = SummaryItemType.Count;
            //    gridView1.Columns["厂家"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";

            //    RepositoryItemHyperLinkEdit link1 = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit link2 = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit link3 = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit link4 = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit link5 = new RepositoryItemHyperLinkEdit();

            //    link1.LinkColor = Color.Maroon;
            //    link2.LinkColor = Color.Maroon;
            //    link3.LinkColor = Color.Maroon;
            //    link4.LinkColor = Color.Maroon;
            //    link5.LinkColor = Color.Maroon;

            //    link1.OpenLink += link_OpenLink;
            //    link2.OpenLink += link_OpenLink;
            //    link3.OpenLink += link_OpenLink;
            //    link4.OpenLink += link_OpenLink;
            //    link5.OpenLink += link_OpenLink;

            //    gridView1.Columns["一周"].ColumnEdit = link1;
            //    gridView1.Columns["二周"].ColumnEdit = link2;
            //    gridView1.Columns["三周"].ColumnEdit = link3;
            //    gridView1.Columns["四周"].ColumnEdit = link4;
            //    gridView1.Columns["大于四周"].ColumnEdit = link5;
            //}
            //catch (Exception ckItem_CheckedChanged)
            //{
            //    XtraMessageBox.Show(this, "System error[ckItem_CheckedChanged]: " + ckItem_CheckedChanged.Message);
            //}
        }

        private void ckLoc_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    gridView1.Columns.Clear();
            //    gridControl1.DataSource = GetData(ckItem.Checked, ckLoc.Checked);
            //    gridView1.BestFitColumns();
            //    gridView1.Columns["厂家"].Width = 120;
            //    gridView1.Columns["厂家"].SummaryItem.SummaryType = SummaryItemType.Count;
            //    gridView1.Columns["厂家"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";

            //    RepositoryItemHyperLinkEdit link1 = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit link2 = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit link3 = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit link4 = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit link5 = new RepositoryItemHyperLinkEdit();

            //    link1.LinkColor = Color.Maroon;
            //    link2.LinkColor = Color.Maroon;
            //    link3.LinkColor = Color.Maroon;
            //    link4.LinkColor = Color.Maroon;
            //    link5.LinkColor = Color.Maroon;

            //    link1.OpenLink += link_OpenLink;
            //    link2.OpenLink += link_OpenLink;
            //    link3.OpenLink += link_OpenLink;
            //    link4.OpenLink += link_OpenLink;
            //    link5.OpenLink += link_OpenLink;

            //    gridView1.Columns["一周"].ColumnEdit = link1;
            //    gridView1.Columns["二周"].ColumnEdit = link2;
            //    gridView1.Columns["三周"].ColumnEdit = link3;
            //    gridView1.Columns["四周"].ColumnEdit = link4;
            //    gridView1.Columns["大于四周"].ColumnEdit = link5;
            //}
            //catch (Exception ckLoc_CheckedChanged)
            //{
            //    XtraMessageBox.Show(this, "System error[ckLoc_CheckedChanged]: " + ckLoc_CheckedChanged.Message);
            //}
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

        private void btnExport_Click(object sender, EventArgs e)
        {
            PaCSGlobal.ExportGridToFile(gridView1, "Toner_Aging");
        }

        private void gridView1_MouseUp(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = this.gridView1.CalcHitInfo(e.Location);
            if (hi.InRow && e.Button == MouseButtons.Right)
            {
                global.CallMenu(gridView1).ShowPopup(Control.MousePosition);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                  this.Invoke((MethodInvoker)delegate
                {
                    gridView1.Columns.Clear();

                    gridControl1.DataSource = GetData(ckItem.Checked, ckLoc.Checked);
                    gridView1.BestFitColumns();
                    gridView1.Columns["厂家"].Width = 120;
                    gridView1.Columns["厂家"].SummaryItem.SummaryType = SummaryItemType.Count;
                    gridView1.Columns["厂家"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";

                    gridView1.Columns["一周"].Width = 70;
                    gridView1.Columns["一周"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    gridView1.Columns["一周"].SummaryItem.DisplayFormat = "{0:f0}";

                    gridView1.Columns["二周"].Width = 70;
                    gridView1.Columns["二周"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    gridView1.Columns["二周"].SummaryItem.DisplayFormat = "{0:f0}";

                    gridView1.Columns["三周"].Width = 70;
                    gridView1.Columns["三周"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    gridView1.Columns["三周"].SummaryItem.DisplayFormat = "{0:f0}";

                    gridView1.Columns["四周"].Width = 70;
                    gridView1.Columns["四周"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    gridView1.Columns["四周"].SummaryItem.DisplayFormat = "{0:f0}";

                    gridView1.Columns["大于四周"].Width = 70;
                    gridView1.Columns["大于四周"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    gridView1.Columns["大于四周"].SummaryItem.DisplayFormat = "{0:f0}";

                    RepositoryItemHyperLinkEdit link1 = new RepositoryItemHyperLinkEdit();
                    RepositoryItemHyperLinkEdit link2 = new RepositoryItemHyperLinkEdit();
                    RepositoryItemHyperLinkEdit link3 = new RepositoryItemHyperLinkEdit();
                    RepositoryItemHyperLinkEdit link4 = new RepositoryItemHyperLinkEdit();
                    RepositoryItemHyperLinkEdit link5 = new RepositoryItemHyperLinkEdit();

                    link1.LinkColor = Color.Maroon;
                    link2.LinkColor = Color.Maroon;
                    link3.LinkColor = Color.Maroon;
                    link4.LinkColor = Color.Maroon;
                    link5.LinkColor = Color.Maroon;

                    link1.OpenLink += link_OpenLink;
                    link2.OpenLink += link_OpenLink;
                    link3.OpenLink += link_OpenLink;
                    link4.OpenLink += link_OpenLink;
                    link5.OpenLink += link_OpenLink;

                    gridView1.Columns["一周"].ColumnEdit = link1;
                    gridView1.Columns["二周"].ColumnEdit = link2;
                    gridView1.Columns["三周"].ColumnEdit = link3;
                    gridView1.Columns["四周"].ColumnEdit = link4;
                    gridView1.Columns["大于四周"].ColumnEdit = link5;

                    gridView1.Columns["厂家"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["单位"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["类型"].OptionsColumn.AllowEdit = false;

                    if (gridView1.Columns.Contains(gridView1.Columns["材料编号"]))
                        gridView1.Columns["材料编号"].OptionsColumn.AllowEdit = false;
                    if (gridView1.Columns.Contains(gridView1.Columns["位置"]))
                        gridView1.Columns["位置"].OptionsColumn.AllowEdit = false;
                });
            }
            catch (Exception btnApply_Click)
            {
                XtraMessageBox.Show(this, "System error[btnApply_Click]: " + btnApply_Click.Message);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();  
        }


    }
}