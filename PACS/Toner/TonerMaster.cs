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
    public partial class TonerMaster : DevExpress.XtraEditors.XtraForm
    {
        PaCSGlobal global = new PaCSGlobal();
        public TonerMaster()
        {
            InitializeComponent();
            global.InitMenu();
        }

        private void TonerMaster_Load(object sender, EventArgs e)
        {
            gridView1.OptionsCustomization.AllowColumnMoving = false;//禁止列拖动
            //        gridView1.OptionsBehavior.Editable = false;
        }

        void link_OpenLink(object sender, OpenLinkEventArgs e)
        {
            if (e.EditValue.ToString().Equals("0"))
                return;

            string vendor = gridView1.GetFocusedRowCellValue("厂家").ToString();
            string qty = gridView1.GetFocusedRowCellValue("单位").ToString();

            string item = "";
            if(ckItem.Checked)
            {
                item = gridView1.GetFocusedRowCellValue("材料编号").ToString();
            }

            string location = "";
            if (ckLoc.Checked)
            {
                location = gridView1.GetFocusedRowCellValue("位置").ToString();
            }
           
            string clickCol = gridView1.FocusedColumn.FieldName;

            InventoryDetail frmNew = new InventoryDetail(vendor, qty, item,location,clickCol);
            DialogResult dg = frmNew.ShowDialog();
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

            //    gridView1.Columns["STOCK"].AppearanceCell.ForeColor = Color.Blue;
            //    gridView1.Columns["TRANSIT"].AppearanceCell.ForeColor = Color.Blue;
            //    gridView1.Columns["INPUT"].AppearanceCell.ForeColor = Color.Blue;

            //    RepositoryItemHyperLinkEdit linkStock = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit linkTransit = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit linkInput = new RepositoryItemHyperLinkEdit();

            //    linkStock.OpenLink += link_OpenLink;
            //    linkTransit.OpenLink += link_OpenLink;
            //    linkInput.OpenLink += link_OpenLink;

            //    gridView1.Columns["STOCK"].ColumnEdit = linkStock;
            //    gridView1.Columns["TRANSIT"].ColumnEdit = linkTransit;
            //    gridView1.Columns["INPUT"].ColumnEdit = linkInput;
            //}
            //catch (Exception ckLoc_CheckedChanged)
            //{
            //    XtraMessageBox.Show(this, "System error[ckLoc_CheckedChanged]: " + ckLoc_CheckedChanged.Message);
            //}
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

            //    gridView1.Columns["STOCK"].AppearanceCell.ForeColor = Color.Blue;
            //    gridView1.Columns["TRANSIT"].AppearanceCell.ForeColor = Color.Blue;
            //    gridView1.Columns["INPUT"].AppearanceCell.ForeColor = Color.Blue;

            //    RepositoryItemHyperLinkEdit linkStock = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit linkTransit = new RepositoryItemHyperLinkEdit();
            //    RepositoryItemHyperLinkEdit linkInput = new RepositoryItemHyperLinkEdit();

            //    linkStock.OpenLink += link_OpenLink;
            //    linkTransit.OpenLink += link_OpenLink;
            //    linkInput.OpenLink += link_OpenLink;

            //    gridView1.Columns["STOCK"].ColumnEdit = linkStock;
            //    gridView1.Columns["TRANSIT"].ColumnEdit = linkTransit;
            //    gridView1.Columns["INPUT"].ColumnEdit = linkInput;
            //}
            //catch (Exception ckItem_CheckedChanged)
            //{
            //    XtraMessageBox.Show(this, "System error[ckItem_CheckedChanged]: " + ckItem_CheckedChanged.Message);
            //}
        }

        private DataTable GetData(bool itemFlag, bool locFlag)
        {
            StringBuilder sql = new StringBuilder(
            " select vend_nm_cn \"厂家\",qty \"单位\"," + (itemFlag ? " item \"材料编号\" ," : "") + (locFlag ? "stock_location \"位置\" ," : "") + "																					" +
            "          max(decode(box_case_status_nm, '在库', cnt, 0)) stock,                                                                   " +
            "          max(decode(box_case_status_nm, '在途', cnt, 0)) transit,                                                                 " +
            "          max(decode(box_case_status_nm, '投入', cnt, 0)) input                                                                    " +
            " from                                                                                                                              " +
            " (                                                                                                                                 " +
            "     select vend_nm_cn,qty" + (itemFlag ? ",item" : "") + (locFlag ? ",stock_location" : "") + ",box_case_status_nm,count(*)  cnt " +
            "     from                                                                                                                          " +
            "     (                                                                                                                             " +
            "         select box_label,item,qty,final_vend_to,b.vend_nm_cn,final_stock_to,f.comm_code_nm final_stock_to_nm,final_buffer_to,     " +
            "              c.comm_code_nm final_buffer_to_nm,final_line_to,d.comm_code_nm line_nm,box_case_status,e.comm_code_nm box_case_status_nm,coalesce(d.comm_code_nm,c.comm_code_nm,f.comm_code_nm) stock_location          " +
            "          from pacsd_pm_box a,                                                                                                     " +
            "                  pacsm_md_vend b,                                                                                                 " +
            "                  pacsc_md_comm_code c,                                                                                            " +
            "                  pacsc_md_comm_code d,                                                                                                   " +
            "                  pacsc_md_comm_code e,                                                                                            " +
            "                  pacsc_md_comm_code f                                                                                             " +
            "          where 1=1                                                                                       " +
            "          and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "          and a.final_vend_to = b.vend_code " +
           // "          and (a.final_vend_to = '" + PaCSGlobal.LoginUserInfo.Venderid + "' or  '" + PaCSGlobal.LoginUserInfo.Venderid + "' = 'C660')  " +
            "          and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "          and c.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "          and c.type_code(+) = 'PACS_BOX_BUFFER'                                                                                   " +
            "          and c.comm_code(+) = a.final_buffer_to                                                                                   " +
            "          and d.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "          and d.type_code(+) = 'PACS_BOX_LINE'                                                                                              " +
            "          and d.comm_code(+) = a.final_line_to                                                                                     " +
            "          and e.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "          and e.type_code(+) = 'PACS_BOX_CASE_STATUS'                                                                              " +
            "          and e.comm_code(+) = a.box_case_status                                                                                   " +
            "          and f.fct_code(+) = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'       " +
            "          and f.type_code(+) = 'PACS_BOX_STOCK'                                                                                    " +
            "          and f.comm_code(+) = a.final_stock_to                                                                                    " +
            "          and a.box_case_status in ('STOCK','TRANSIT','INPUT')                                                      " +
            "     )                                                                                                                             " +
            "     group by vend_nm_cn,qty" + (itemFlag ? ",item" : "") + (locFlag ? ",stock_location" : "") + ",box_case_status_nm                        " +
            " )                                                                                                                                 " +
            " group by vend_nm_cn,qty" + (itemFlag ? ",item" : "") + (locFlag ? ",stock_location" : "") + ""+
            " order by 1");

            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());
            return dtResult;
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
            PaCSGlobal.ExportGridToFile(gridView1, "Toner_Master");
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

                    gridView1.Columns["STOCK"].Width = 70;
                    gridView1.Columns["STOCK"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    gridView1.Columns["STOCK"].SummaryItem.DisplayFormat = "{0:f0}";

                    gridView1.Columns["TRANSIT"].Width = 70;
                    gridView1.Columns["TRANSIT"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    gridView1.Columns["TRANSIT"].SummaryItem.DisplayFormat = "{0:f0}";

                    gridView1.Columns["INPUT"].Width = 70;
                    gridView1.Columns["INPUT"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    gridView1.Columns["INPUT"].SummaryItem.DisplayFormat = "{0:f0}";

                    RepositoryItemHyperLinkEdit linkStock = new RepositoryItemHyperLinkEdit();
                    RepositoryItemHyperLinkEdit linkTransit = new RepositoryItemHyperLinkEdit();
                    RepositoryItemHyperLinkEdit linkInput = new RepositoryItemHyperLinkEdit();

                    linkStock.LinkColor = Color.Maroon;
                    linkTransit.LinkColor = Color.Maroon;
                    linkInput.LinkColor = Color.Maroon;

                    linkStock.OpenLink += link_OpenLink;
                    linkTransit.OpenLink += link_OpenLink;
                    linkInput.OpenLink += link_OpenLink;

                    gridView1.Columns["STOCK"].ColumnEdit = linkStock;
                    gridView1.Columns["TRANSIT"].ColumnEdit = linkTransit;
                    gridView1.Columns["INPUT"].ColumnEdit = linkInput;

                    gridView1.Columns["厂家"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["单位"].OptionsColumn.AllowEdit = false;
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