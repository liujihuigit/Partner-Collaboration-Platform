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
    public partial class InventoryDetail : DevExpress.XtraEditors.XtraForm
    {
        private string vendor = "";
        private string qty = "";
        private string item = "";
        private string location = "";
        private string clickCol = "";

        PaCSGlobal global = new PaCSGlobal();

        public InventoryDetail(string vendor, string qty, string item, string location, string clickCol)
        {
            InitializeComponent();
            global.InitMenu();

            this.vendor = vendor;
            this.qty = qty;
            this.item = item;
            this.location = location;
            this.clickCol = clickCol;
        }

        private void InventoryDetail_Load(object sender, EventArgs e)
        {
            try
            {
                gridControl1.DataSource = GetData();
                gridView1.BestFitColumns();
                gridView1.Columns["桶编号"].SummaryItem.SummaryType = SummaryItemType.Count;
                gridView1.Columns["桶编号"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";
            }
            catch (Exception TonerDetail_Load)
            {
                XtraMessageBox.Show(this, "System error[TonerDetail_Load]: " + TonerDetail_Load.Message);
            }
        }

        private DataTable GetData()
        {
            StringBuilder sql = new StringBuilder(
                 " select box_label \"桶编号\",item \"材料编号\",qty \"单位\",																																									" +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss')  from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GREJH'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) \"原材料仓库入库日期\",    " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GIEJH'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') \"原材料仓库出库日期\",    " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GRCJ'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') \"厂家入库日期\",  " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'GICJ'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') \"厂家出库日期\",  " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'LINE'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') \"厂家投入日期\", " +
                "          (select to_char(to_date(min(update_date||update_time),'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') from pacsp_pm_box_prgs b where a.box_label = b.box_label and b.cancel_flag is null and b.operation_window = 'SCRAP'  and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') \"废弃日期\"   " +
                 "        from pacsd_pm_box a,                                                                                                                                                                  " +
                 "                pacsm_md_vend b,                                                                                                                                                              " +
                 "                pacsc_md_comm_code c,                                                                                                                                                         " +
                 "                pacsc_md_comm_code d,                                                                                                                                                                " +
                 "                pacsc_md_comm_code e,                                                                                                                                                         " +
                 "                pacsc_md_comm_code f                                                                                                                                                          " +
                 "        where a.final_vend_to = b.vend_code                                                                                                                                                   " +
                 "        and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                 "        and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                 "        and c.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                 "        and c.type_code(+) = 'PACS_BOX_BUFFER'                                                                                                                                                " +
                 "        and c.comm_code(+) = a.final_buffer_to                                                                                                                                                " +
                 "        and d.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                 "        and d.type_code(+) = 'PACS_BOX_LINE'                                                                                                                                                           " +
                 "        and d.comm_code(+) = a.final_line_to                                                                                                                                                  " +
                 "        and e.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                 "        and e.type_code(+) = 'PACS_BOX_CASE_STATUS'                                                                                                                                           " +
                 "        and e.comm_code(+) = a.box_case_status                                                                                                                                                " +
                 "        and f.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                 "        and f.type_code(+) = 'PACS_BOX_STOCK'                                                                                                                                                 " +
                 "        and f.comm_code(+) = a.final_stock_to                                                                                                                                                 ");

            sql.Append("        and box_case_status = '" + clickCol + "'                                                                                                                                                       ");
            sql.Append("        and b.vend_nm_cn = '" + vendor + "'                                                                                                                                                         ");
            sql.Append("        and qty = '" + qty + "'                                                                                                                                                                          ");

            if (!string.IsNullOrEmpty(item))
            {
                sql.Append("        and item = '" + item + "'                                                                                                                                                              ");
            }

            if (!string.IsNullOrEmpty(location))
            {
                sql.Append("        and nvl(coalesce(d.comm_code_nm,c.comm_code_nm,f.comm_code_nm),'BLANK') = '" + location + "'                                                                                                          ");
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
            PaCSGlobal.ExportGridToFile(gridView1, "Toner_InventoryDetail");
        }


    }
}