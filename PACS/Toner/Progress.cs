using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraSplashScreen;
using PaCSTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toner
{
    public partial class Progress : XtraForm
    {
        private DataTable dt = new DataTable();
        PaCSGlobal global = new PaCSGlobal();

        public Progress()
        {
            InitializeComponent();
            global.InitMenu();

            dateEditFrom.Text = PaCSGlobal.GetServerDateTimeRange("-", -7);
            dateEditTo.Text = PaCSGlobal.GetServerDateTime(3);
        }

        private void Progress_Load(object sender, EventArgs e)
        {
            cmbVendor.Properties.BeginUpdate();
            ComboxData data = new ComboxData();
            data.Text = "";
            data.Value = "";
            cmbVendor.Properties.Items.Add(data);
            TonerGlobal.LoadCmbVendor(cmbVendor);
            cmbVendor.Properties.EndUpdate();
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

         private DataTable GetData(string dateFrom1, string dateTo1, string boxlabel, string vendor, string item)
        {
            StringBuilder sql = new StringBuilder(" select  " +
            " box_label \"桶编号\",item \"材料编号\",    " +
            " (select vend_nm_cn from pacsm_md_vend b where a.make_vend_code = b.vend_code and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "')  \"生产厂家\", " +

            " lot_no LOTNO,box_no BOXNO,qty \"单位\",box_status \"开封状态\",box_case_status \"运输状态\"," +
            " (select comm_code_nm from pacsm_rm_comm_info b where a.move_code = b.comm_code and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "')  \"操作名称\",  " +
            " (select comm_code_nm from pacsc_md_comm_code b where b.type_code = 'PACS_BOX_MOVE_TYPE' and a.move_type = b.comm_code and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') \"操作类型\", " +

            " (select vend_nm_cn from pacsm_md_vend b where a.vend_from = b.vend_code and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "')  \"来自厂家\",                        " +
            " (select vend_nm_cn from pacsm_md_vend b where a.vend_to = b.vend_code and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "')  \"去向厂家\",                        " +
            " (select comm_code_nm from pacsc_md_comm_code t where t.type_code ='PACS_BOX_STOCK' and t.comm_code = a.stock_to and t.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "')  \"仓库位置\",                        " +
            " (select b.comm_code_nm line_nm from pacsc_md_comm_code b where b.comm_code=a.buffer_to and b.type_code='PACS_BOX_BUFFER' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') \"楼层\",     " +
            " (select b.comm_code_nm line_nm from pacsc_md_comm_code b where b.comm_code=a.line_to and b.type_code='PACS_BOX_LINE' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') \"生产线\", "+
            " decode(cancel_flag,'1','取消') \"是否取消\",                         " +

            " to_char(to_date(update_date,'yyyymmdd'),'yyyy-mm-dd')  \"更新日期\", " +
            " to_char(to_date(update_time,'hh24miss'),'hh24:mi:ss')  \"更新时间\",                 " +
            " (select u.fullname  from pacs_user u  where u.id = a.update_user)  \"更新人\" ,                                              " +
            " update_ip \"更新人IP\","+

            " to_char(to_date(create_date,'yyyymmdd'),'yyyy-mm-dd')  \"创建日期\", " +
            " to_char(to_date(create_time,'hh24miss'),'hh24:mi:ss')  \"创建时间\", " +
            " (select u.fullname  from pacs_user u  where u.id = a.create_user) \"创建人\", " +
            " create_ip \"创建人IP\", " +

            " doc_no DOCNO " +

            " from pacsp_pm_box_prgs a where 1=1 ");

          //  sql.Append(" and (vend_to = '" + PaCSGlobal.LoginUserInfo.Venderid + "' or  '" + PaCSGlobal.LoginUserInfo.Venderid + "' = 'C660') ");
            sql.Append(" and  fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");
            sql.Append(" and  create_date between '" + dateFrom1 + "' and '" + dateTo1 + "' ");

            if (!string.IsNullOrEmpty(boxlabel))
                sql.Append(" and  box_label like '%" + boxlabel + "%'");
            if (!string.IsNullOrEmpty(vendor))
                sql.Append(" and  make_vend_code = '" + vendor + "'");
            if (!string.IsNullOrEmpty(item))
                sql.Append(" and  item like '%" + item + "%'");

            sql.Append(" order by create_date||create_time,update_date||update_time nulls last");
            Console.WriteLine(sql.ToString());
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());

            return dtResult;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            PaCSGlobal.ExportGridToFile(gridView1, "Toner_History");
        }

        private void SetGridView(DataTable dtData)
        {
            tbItem.DataSource = dtData;
            gridView1.BestFitColumns();
            gridView1.OptionsCustomization.AllowColumnMoving = false;//禁止列拖动
            gridView1.OptionsView.AllowCellMerge = true;
            gridView1.OptionsBehavior.Editable = false;

            gridView1.Columns["桶编号"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView1.Columns["桶编号"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";

            gridView1.Columns["桶编号"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            gridView1.Columns["材料编号"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            gridView1.Columns["生产厂家"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            gridView1.Columns["单位"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

            if (dtData != null)
            {
                foreach (GridColumn col in gridView1.Columns)
                {
                    if (!col.FieldName.Equals("桶编号"))
                    {
                        col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    }
                }
            }
        }

        private void gridView1_MouseUp(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = this.gridView1.CalcHitInfo(e.Location);
            if (hi.InRow && e.Button == MouseButtons.Right)
            {
                global.CallMenu(gridView1).ShowPopup(Control.MousePosition);
            } 
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            tbItem.Text = "";
            tbBoxid.Text = "";
            cmbVendor.SelectedIndex = -1;

            dateEditFrom.Text = PaCSGlobal.GetServerDateTimeRange("-", -7);
            dateEditTo.Text = PaCSGlobal.GetServerDateTime(3);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string dateFrom = dateEditFrom.Text.Trim().Replace("-", "");//2014-08-05
                string dateTo = dateEditTo.Text.Trim().Replace("-", "");//2014-08-06
                string boxlabel = tbBoxid.Text.Trim();
                string vendor = "";
                if (cmbVendor.SelectedIndex > -1)
                    vendor = (cmbVendor.SelectedItem as ComboxData).Value;//制造厂家
                string item = tbItem.Text.Trim();
                  this.Invoke((MethodInvoker)delegate
                {
                    SetGridView(GetData(dateFrom, dateTo, boxlabel, vendor, item));
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
