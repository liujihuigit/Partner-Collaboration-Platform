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

namespace Toner
{
    public partial class AgingDetail : DevExpress.XtraEditors.XtraForm
    {
        private string vendor = "";
        private string qty = "";
        private string type = "";
        private string item = "";
        private string location = "";
        private string d1 = "";
        private string d2 = "";

        PaCSGlobal global = new PaCSGlobal();

        public AgingDetail(string vendor, string qty,string type, string item, string location, string d1,string d2)
        {
            InitializeComponent();
            global.InitMenu();

            this.vendor = vendor;
            this.qty = qty;
            this.type = type;
            this.item = item;
            this.location = location;
            this.d1 = d1;
            this.d2 = d2;
        }

        private void AgingDetail_Load(object sender, EventArgs e)
        {
            try
            {
                gridControl1.DataSource = GetData();
                gridView1.BestFitColumns();
                gridView1.Columns["桶编号"].SummaryItem.SummaryType = SummaryItemType.Count;
                gridView1.Columns["桶编号"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";
            }
            catch (Exception AgingDetail_Load)
            {
                XtraMessageBox.Show(this, "System error[AgingDetail_Load]: " + AgingDetail_Load.Message);
            }
        }

        private DataTable GetData()
        {
            StringBuilder sql = new StringBuilder(
                 " select box_label \"桶编号\",item \"材料编号\",qty \"单位\",																																															 " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss')  from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GREJH'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) \"原材料仓库入库日期\",    " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GIEJH'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) \"原材料仓库出库日期\",    " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GRCJ'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) \"厂家入库日期\",  " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GICJ'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) \"厂家出库日期\",  " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'LINE'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) \"厂家投入日期\", " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'SCRAP'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) \"废弃日期\"   " +
                "      from                                                                                                                                                                                                                      "+
                "      (                                                                                                                                                                                                                         "+
                "           select box_label,item,qty,final_vend_to,vend_nm_cn,final_stock_to,final_stock_to_nm,final_buffer_to,final_buffer_to_nm,final_line_to,line_nm,box_case_status,box_case_status_nm,                                     "+
                "                    coalesce(line_nm,final_buffer_to_nm,final_stock_to_nm) stock_location,                                                                                                                                      "+
                "                    case when vendor_input_date is null then 'STOCK' else 'INPUT' end as aging_type,                                                                                                                            "+
                "                    case when vendor_input_date is null then ceil(sysdate - to_date(coalesce(cj_gr_date,ejh_gi_date,ejh_gr_date),'yyyymmddhh24miss')) else ceil(sysdate - to_date(vendor_input_date,'yyyymmddhh24miss')) end as aging                            "+
                "            from                                                                                                                                                                                                                "+
                "            (                                                                                                                                                                                                                   "+
                "                 select box_label,item,qty,final_vend_to,b.vend_nm_cn,final_stock_to,f.comm_code_nm final_stock_to_nm,final_buffer_to,c.comm_code_nm final_buffer_to_nm,														 "+
                "						   final_line_to,d.comm_code_nm line_nm,box_case_status,e.comm_code_nm box_case_status_nm,																														 " +
                "                          (select min(update_date||update_time) from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GREJH'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) ejh_gr_date,                               " +
                "                          (select min(update_date||update_time) from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GIEJH'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) ejh_gi_date,                          " +
                "                          (select min(update_date||update_time) from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GRCJ'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) cj_gr_date,                          " +
                "                          (select min(update_date||update_time) from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'LINE'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) vendor_input_date                           " +
                "                  from pacsd_pm_box a,                                                                                                                                                                                          "+
                "                          pacsm_md_vend b,                                                                                                                                                                                      "+
                "                          pacsc_md_comm_code c,                                                                                                                                                                                 "+
                "                          pacsc_md_comm_code d,                                                                                                                                                                                        " +
                "                          pacsc_md_comm_code e,                                                                                                                                                                                 "+
                "                          pacsc_md_comm_code f                                                                                                                                                                                  "+
                "                  where a.final_vend_to = b.vend_code                                                                                                                                                                           "+
                "                  and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                "                  and a.final_move_type <> '551'                                                                                                                                                " +
                "                  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                "                  and c.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                "                  and c.type_code(+) = 'PACS_BOX_BUFFER'                                                                                                                                                                        "+
                "                  and c.comm_code(+) = a.final_buffer_to                                                                                                                                                                        "+
                "                  and d.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                "                  and d.type_code(+) = 'PACS_BOX_LINE'                                                                                                                                                                                   " +
                "                  and d.comm_code(+) = a.final_line_to                                                                                                                                                                          " +
                "                  and e.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                "                  and e.type_code(+) = 'PACS_BOX_CASE_STATUS'                                                                                                                                                                   "+
                "                  and e.comm_code(+) = a.box_case_status                                                                                                                                                                        "+
                "                  and f.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                "                  and f.type_code(+) = 'PACS_BOX_STOCK'                                                                                                                                                                         "+
                "                  and f.comm_code(+) = a.final_stock_to                                                                                                                                                                         "+           
                "           )                                                                                                                                                                                                                    "+
                "     ) a                                                                                                                                                                                                                        "+
                "  where 1=1                                                                                                                                                                                                     "+
                "      and aging_type = '"+type+"'                                                                                                                                                                                                 " +
                "      and aging between '" + d1 + "' and '" + d2+ "'                                                                                                                                                                                                 " +
                "      and vend_nm_cn = '"+vendor+"'                                                                                                                                                                                               "+
                "      and qty = '"+qty+"'                                                                                                                                                                                                              ");

            if (!string.IsNullOrEmpty(item))
            {
                sql.Append("        and item = '" + item + "'                                                                                                                                                              ");
            }

            if (!string.IsNullOrEmpty(location))
            {
                sql.Append("        and nvl(coalesce(line_nm,final_buffer_to_nm,final_stock_to_nm),'BLANK') = '"+location+"'                                                                                                          ");
            }
            else
            {
                sql.Append("        and line_nm is null and final_buffer_to_nm is null and final_stock_to_nm is null                                                                                                          ");
            }

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
            PaCSGlobal.ExportGridToFile(gridView1, "Toner_AgingDetail");
        }


    }
}