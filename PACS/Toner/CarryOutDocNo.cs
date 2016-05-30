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
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

namespace Toner
{
    public partial class CarryOutDocNo : DevExpress.XtraEditors.XtraForm
    {
        private string dateFrom = "";
        private string dateTo = "";
        private string vendorDest = "";
        private int index = -1;

        public CarryOutDocNo(int index)
        {
            InitializeComponent();
            this.index = index;
        }

        private void CarryOutDocNo_Load(object sender, EventArgs e)
        {
            cmbDestVendor.Properties.BeginUpdate();
            TonerGlobal.LoadCmbUseVendor(cmbDestVendor, true);
            cmbDestVendor.Properties.EndUpdate();

            cmbDestVendor.SelectedIndex = index;

            dateEditFrom.Text = PaCSGlobal.GetServerDateTime(3);
            dateEditTo.Text = PaCSGlobal.GetServerDateTime(3);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbDestVendor.SelectedIndex == -1)
                {
                    XtraMessageBox.Show("请选择目的厂家！", "提示"); ;
                    cmbDestVendor.Focus();
                    return;
                }

                dateFrom = dateEditFrom.Text.Trim().Replace("-", "");//2014-08-05
                dateTo = dateEditTo.Text.Trim().Replace("-", "");//2014-08-06
                vendorDest = (cmbDestVendor.SelectedItem as ComboxData).Value;//目的厂家

                gridControl1.DataSource = GetData("", dateFrom, dateTo, vendorDest);
                gridView1.BestFitColumns();
                gridView1.OptionsCustomization.AllowColumnMoving = false;//禁止列拖动

                if (gridView1.Columns.Count>0)
                    gridView1.Columns[2].Visible = false;
            }
            catch (Exception btnApply_Click)
            {
                XtraMessageBox.Show(this, "System error[btnApply_Click]: " + btnApply_Click.Message);
            }
        }

        private DataTable GetData(string docno,string date1,string date2,string destVend)
        {
            StringBuilder sql = new StringBuilder(" select a.final_doc_no DocNo,count(*) \"数量\",a.final_vend_to, " +
               " (select t.vend_nm_cn from pacsm_md_vend t where t.vend_code = a.final_vend_to and t.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) \"目的厂家\"," +
               " to_char(to_date(a.update_date,'yyyymmdd'),'yyyy-mm-dd') \"出库日期\",(select u.fullname  from pacs_user u  where u.id = a.update_user) \"出库人\",a.update_ip \"出库IP\" " +
               " from pacsd_pm_box a " +
               " where  operation_window = 'GIEJH' " +
               //" and update_date between to_char(sysdate-1,'yyyyMMdd') " +
               //" and to_char(sysdate,'yyyyMMdd') " +
               " and update_date between '"+date1+"' " +
               " and '" + date2 + "' " +
               " and a.final_vend_to = '" + destVend + "' " +
               " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");

            if (!string.IsNullOrEmpty(docno))
            {
                sql.Append(" and a.final_doc_no like '%" + docno + "%'");
            }
            sql.Append(" group by a.final_doc_no,a.final_vend_to,a.update_date,a.update_user,a.update_ip");
            sql.Append(" order by a.final_doc_no desc,a.update_date desc nulls last");

            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());

            if (dtResult.Rows.Count == 0)
            {
                return null;
            }
            return dtResult;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string docnos = "";
            string vendto = "";
            string vendtoCode = "";
            List<string> vendtoList  = new List<string>();
            int[] rowhandles = gridView1.GetSelectedRows();
            if (rowhandles.Length==0)
            {
                XtraMessageBox.Show("请选择DocNo！","提示");
                return;
            }

            foreach (int i in rowhandles)
            {
                docnos+=gridView1.GetRowCellValue(i, gridView1.Columns["DOCNO"]).ToString()+",";
                vendtoCode = gridView1.GetRowCellValue(i, gridView1.Columns[2]).ToString();
                vendto = gridView1.GetRowCellValue(i, gridView1.Columns[3]).ToString();
            }
            docnos = docnos.Remove(docnos.Length - 1, 1);

            TonerOutSheet report = new TonerOutSheet(docnos, vendto, vendtoCode);
            ReportPrintTool pts = new ReportPrintTool(report);
            pts.PrintingSystem.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            pts.ShowPreview();
            //pts.Print();
        }


        

      
    }
}