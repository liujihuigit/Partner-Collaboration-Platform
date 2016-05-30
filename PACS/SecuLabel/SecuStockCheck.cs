using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using PaCSTools;
using DevExpress.Data;

namespace SecuLabel
{
    public partial class SecuStockCheck : DevExpress.XtraEditors.XtraForm
    {

        string ls_vendor = "", ls_sdate = "", ls_edate = "";
        public SecuStockCheck()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 记载基本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecuStockCheck_Load(object sender, EventArgs e)
        {

            SecuGlobal.setAllVendorInfo(PaCSGlobal.LoginUserInfo.Fct_code, cbVendor);
            dateEditFrom.Text = PaCSTools.PaCSGlobal.GetServerDateTime(3);
            dateEditTo.Text = PaCSTools.PaCSGlobal.GetServerDateTime(3);
            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");

        }


        private void getVariable()
        {
            if (cbVendor.SelectedIndex != -1)
                ls_vendor = cbVendor.Properties.Items[cbVendor.SelectedIndex].ToString();

            if (!string.IsNullOrEmpty(ls_vendor))
            {
                string[] split = ls_vendor.Split(new Char[] { ':' });
                ls_vendor = split[0].Trim();
            }

            ls_sdate = dateEditFrom.Text.Trim().Replace("-","");
            ls_edate = dateEditFrom.Text.Trim().Replace("-", ""); 

        }


        private string getSql(string ls_vendor)
        {
            string sql = " select to_char(sysdate,'yyyymmdd') cycle_date, to_char(sysdate,'yyyy/mm/dd hh24:mi:ss') cycle_timestamp,  " +
                        "    security_type,  item,  " +
                        "    qty, box_no, roll_no,  " +
                        "    lot_no, security_start, security_end,  " +
                        "    (select count(*) from tb_security_in_sn b where b.fct_code = a.fct_code and a.doc_no = b.doc_no and a.doc_seq = b.doc_seq and b.status = 'OUT' and b.vendor = '" + ls_vendor + "') dept_qty,  " +
                        "    (select count(*) from tb_security_in_sn b where b.fct_code = a.fct_code and a.doc_no = b.doc_no and a.doc_seq = b.doc_seq and b.status = 'INPUT' and b.vendor = '" + ls_vendor + "') line_qty,  " +
                        "    (select count(*) from tb_security_in_sn b where b.fct_code = a.fct_code and a.doc_no = b.doc_no and a.doc_seq = b.doc_seq and b.status = 'SCRAP' and b.vendor = '" + ls_vendor + "' and rtn_datetime is null) scrap_qty  " +
                        " from tb_security_in a  " +
                        " where status = 'O' and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                        " and exists (select 1 from tb_security_in_sn b where b.fct_code = a.fct_code and a.doc_no = b.doc_no and a.doc_seq = b.doc_seq and b.status in ('OUT','INPUT') and b.vendor = '" + ls_vendor + "')  " +
                        " order by security_type,dept_qty desc,line_qty desc,box_no,roll_no,security_start  ";

            return sql;
        }



        /// <summary>
        /// 查询按钮事件-SQL
        /// </summary>
        /// <param name="ls_vendor"></param>
        /// <param name="ls_sdate"></param>
        /// <param name="ls_edate"></param>
        /// <param name="ls_protect"></param>
        /// <returns>可用的SQL</returns>
        private string getsql2(string ls_vendor, string ls_sdate, string ls_edate, string ls_protect)
        {
            string sql = " select '" + ls_protect  + "' s,cycle_id, cycle_vendor, cycle_date,  " +
                        "    cycle_timestamp, security_type, item,  " +
                        "    qty, box_no, roll_no,  " +
                        "    lot_no, security_start, security_end,  " +
                        "    dept_qty, dept_on_hand, line_qty,  " +
                        "    line_on_hand, remark, update_date,  " +
                        "    update_time, update_user, update_ip, " +
                        "    scrap_qty, scrap_on_hand " +
                        " from tb_security_cycle " +
                        " where cycle_vendor = '" + ls_vendor + "' " +
                        " and cycle_date between '" + ls_sdate + "' and '" + ls_edate + "' " +
                        " order by cycle_vendor,cycle_date,cycle_timestamp asc,security_type,dept_qty desc," +
                        " line_qty desc,box_no,roll_no,security_start ";

            return sql;
        }



        private void btnStockNow_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "提示");
            }
        }




        /// <summary>
        /// 改变DataTable 标题栏及排列顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader(DataTable dt)
        {
            string[] colField = { "cycle_date", "cycle_timestamp", "security_type", "item", "qty", "box_no", 
                                    "roll_no","lot_no","security_start","security_end","dept_qty","line_qty",
                                    "scrap_qty" };

            string[] colName = { "Cycle Date", "Cycle Timestamp", "Security Type", "Item", "Qty", "Box No", 
                                    "Roll No","Lot No","Security Start","Security End","Dept Qty","Line Qty",
                                    "Scrap Qty" };

            int[] showIndex = {0,1,2,3,4,5,6,7,8,9,10,11,12 };

            for (int i = 0; i <= 12 ;i++ )
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(showIndex[i]);
 
            }

            return dt;
        }



        /// <summary>
        /// 改变DataTable 标题栏及排列顺序 查询按钮
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader2(DataTable dt)
        {
            string[] colField = { "P","cycle_id","cycle_vendor","cycle_date","cycle_timestamp","security_type","item",
                                "qty","box_no","roll_no ","lot_no","security_start","security_end","dept_qty",
                                "dept_on_hand","line_qty","line_on_hand","remark","update_date","update_time",
                                "update_user","update_ip","scrap_qty","scrap_on_hand" };

            string[] colName = { "P","Cycle Id","Cycle Vendor","Cycle Date","Cycle Timestamp","Security Type","Item",
                                "Qty","Box No","Roll No ","Lot No","Security Start","Security End","Dept qty",
                                "Dept On Hand","Line Qty","Line On Hand","Remark","Update Date","Update Time",
                                "Update User","Update Ip","Scrap Qty","Scrap On Hand" };

            int[] showIndex = { 0,1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 
                                  13,14,15,16,17,18,19,20,21,22,23 };

            for (int i = 0; i <= 23; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(showIndex[i]);
            }

            return dt;
        }


        /// <summary>
        /// 当前库存查询
        /// 后台进行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                getVariable();
                this.Invoke((MethodInvoker)delegate
                {
                    DataTable dt = null;
                    dt = OracleHelper.ExecuteDataTable(getSql(ls_vendor));

                    if (dt != null)
                        dt = setDtHeader(dt);
                    grdView1.Columns.Clear();
                    gridControl1.DataSource = dt;
                    grdView1.BestFitColumns();

                    grdView1.Columns["Cycle Timestamp"].SummaryItem.SummaryType = SummaryItemType.Count;
                    grdView1.Columns["Cycle Timestamp"].SummaryItem.DisplayFormat = "All： {0:f0} ";
                    grdView1.Columns["Dept Qty"].Width = 100;
                    grdView1.Columns["Dept Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdView1.Columns["Dept Qty"].SummaryItem.DisplayFormat = "Sum:{0:f0} ";
                    grdView1.Columns["Line Qty"].Width = 80;
                    grdView1.Columns["Line Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdView1.Columns["Line Qty"].SummaryItem.DisplayFormat = "{0:f0} ";
                    grdView1.Columns["Scrap Qty"].Width = 80;
                    grdView1.Columns["Scrap Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdView1.Columns["Scrap Qty"].SummaryItem.DisplayFormat = "{0:f0} ";

                });
            }
            catch (Exception err)
            {
                XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                getVariable();
                this.Invoke((MethodInvoker)delegate
                {
                    DataTable dt = null;
                    dt = OracleHelper.ExecuteDataTable(getsql2(ls_vendor,ls_sdate,ls_edate ,"P"));

                    if (dt != null)
                        dt = setDtHeader2(dt);

                    grdView1.Columns.Clear();
                    gridControl1.DataSource = dt;
                    grdView1.BestFitColumns();

                    grdView1.Columns["Cycle Vendor"].SummaryItem.SummaryType = SummaryItemType.Count;
                    grdView1.Columns["Cycle Vendor"].SummaryItem.DisplayFormat = "All： {0:f0} ";
                    grdView1.Columns["Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdView1.Columns["Qty"].SummaryItem.DisplayFormat = "Sum： {0:f0} ";


                });
            }
            catch (Exception err)
            {
                XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker2.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "提示");
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            PaCSGlobal.ExportGridToFile(grdView1,"Stock Info");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //dataTable1.AcceptChanges();
            //dataTable1.Merge(dataTable2);DataTable 
            //    changesTable = dataTable1.GetChanges();
            //这样可以快速获得dataTable2中存在而dataTable1中不存在的行，反之可以用dataTable2合并dataTable1。
        }






    }
}