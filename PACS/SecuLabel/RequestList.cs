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
using DevExpress.Data;
using PaCSTools;

namespace SecuLabel
{
    public partial class RequestList : DevExpress.XtraEditors.XtraForm
    {
        string queryType = "";

        string ls_req_vendor = "", ls_sdate = "", ls_edate = "", ls_material_code = "", ls_plant = "";
        public RequestList()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }



        /// <summary>
        /// 后台查询数据并且显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                getVariable();
                DataTable dt = null;

                this.Invoke((MethodInvoker)delegate
                {
                    switch (queryType)
                    {
                        case "SUMMARY":
                            dt = OracleHelper.ExecuteDataTable(getSqlSummary(ls_req_vendor, ls_sdate, ls_edate, ls_material_code, ls_plant));
                            if (dt != null)
                                dt = setDtHeader(dt);
                            
                            SecuGlobal.GridViewInitial(grdView1,gridControl1 );
                            gridControl1.DataSource = dt;
                            grdView1.BestFitColumns();

                            grdView1.Columns["描述"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                            grdView1.Columns["描述"].SummaryItem.SummaryType = SummaryItemType.Count;
                            grdView1.Columns["描述"].SummaryItem.DisplayFormat = "All: {0:f0}";

                            break;
                        case "DETAIL":
                            dt = OracleHelper.ExecuteDataTable(getSqlDetail(ls_req_vendor, ls_sdate, ls_edate));

                            SecuGlobal.GridViewInitial(grdView1, gridControl1);
                            gridControl1.DataSource = dt;
                            grdView1.BestFitColumns();

                            grdView1.Columns["REQ_VENDOR"].SummaryItem.SummaryType = SummaryItemType.Count;
                            grdView1.Columns["REQ_VENDOR"].SummaryItem.DisplayFormat = "All: {0:f0}";
                            break;
                    }
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                });
            }
            catch (Exception err)
            {
                XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
            }
            
        }



        /// <summary>
        /// 查询类型为SUMMARY 时执行的SQL 语句
        /// </summary>
        /// <returns></returns>
        private string getSqlSummary(string ls_req_vendor, string ls_sdate,string ls_edate, string ls_material_code,string ls_plant )
        {
            string sql = " select a.req_doc, " +
                            "          b.req_seq, " +
                            "          a.req_vendor, " +
                            "          b.material_code, " +
                            "          (select c.description from " + SecuGlobal.tb_fpp_itemmaster  + " c where b.material_code = c.matnr and c.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') description, " +
                            "          b.req_qty, " +
                            "          b.prod_plan_qty, " +
                            "          b.actual_send_qty, " +
                            "          b.gr_qty, " +
                            "          b.status, " +
                            "          b.barcode_flag, " +
                            "          a.req_user, " +
                            "          a.req_date, " +
                            "          a.prod_plan_date, " +
                            "          a.remark, " +
                            "          a.plant " +
                            "  from " + SecuGlobal.tbSecurityRequestH  + " a," + SecuGlobal.tbSecurityRequestD  + " b " +
                            " where a.req_doc = b.req_doc " +
                            " and a.req_vendor like '" + ls_req_vendor + "' " +
                            " and a.req_date between '" + ls_sdate + "' and '" + ls_edate + "' " +
                            " and b.material_code like '" + ls_material_code + "' " +
                            " and a.plant = b.plant and a.fct_code = b.fct_code" +
                            " and a.plant like '" + ls_plant + "' and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                            " order by a.req_doc,b.req_seq ";
                       

            return sql ;
        }


        /// <summary>
        /// 改变DataTable 标题栏及排列顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader(DataTable dt)
        {

            string[] colField = { "plant","req_doc","req_seq", "req_vendor", "material_code", "description", "req_qty", "prod_plan_qty", 
                                    "actual_send_qty","gr_qty","status","barcode_flag","req_user","req_date" ,"prod_plan_date","remark"};

            string[] colName = { "Plant","申请单号","序号", "厂家", "材料编号", "描述", "申请数量", "计划数量", 
                                    "实际发料数","入库数量","状态","条形码","申请人","申请日期" ,"计划日期","备注"};


            //int[] showIndex = { 0, 1, 2, 3, 4, 5, 6, 7 };

            for (int i = 0; i < colField.Length; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);

            }

            return dt;

        }


        /// <summary>
        /// 查询类型为DETAIL 时执行的SQL 语句
        /// </summary>
        /// <returns></returns>
        private string getSqlDetail(string ls_vendor, string ls_sdate, string ls_edate)
        {
            string sql = " select a.gi_date,b.req_vendor,b.req_user,a.qty,security_start,security_end " +
                        " from " + SecuGlobal.tbSecurityOut  + " a, " +
                        "      " + SecuGlobal.tbSecurityRequestH  + " b " +
                        " where a.gi_date between '" + ls_sdate + "' and '" + ls_edate + "' " +
                        " and a.req_doc = b.req_doc and a.fct_code = b.fct_code " +
                        " and b.req_vendor like '" + ls_vendor  + "' and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                        " order by gi_date,req_vendor,security_start ";

            return sql;
        }





        private void getVariable()
        {


            ls_req_vendor = cbVendor.Text.Trim();

            if (!string.IsNullOrEmpty(ls_req_vendor))
            {
                if (!ls_req_vendor.Equals("ALL"))
                {
                    string[] split = ls_req_vendor.Split(new Char[] { ':' });
                    ls_req_vendor = split[0].Trim();
                    ls_req_vendor = ls_req_vendor + "%";
                }
                else
                {
                    ls_req_vendor = "%";
                }
            }


            if (cbPlant.SelectedIndex != -1)
                ls_plant = cbPlant.Properties.Items[cbPlant.SelectedIndex].ToString();

            if (!string.IsNullOrEmpty(ls_plant))
            {
                if (!ls_plant.Equals("ALL"))
                {
                    ls_plant = ls_plant + "%";
                }
                else
                {
                    ls_plant = "%";
                }
            }


             ls_sdate = dateEditFrom.Text.Trim().Replace("-", "");
             ls_edate = dateEditTo.Text.Trim().Replace("-", "");


            if (!string.IsNullOrEmpty(tbMeterial.Text))
                ls_material_code = tbMeterial.Text.Trim() + "%";
            else
                ls_material_code = "%";


        }



        /// <summary>
        /// 后台查询 - wait Form 显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "查询数据中,请稍等...");
                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));
            }
            catch (Exception ex)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, ex.Message);
            }
        }



        /// <summary>
        /// 查询类型获取《汇总：SUMMARY / 详细：DETAIL》
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioGroup2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioGroup2.SelectedIndex == 0)
            {
                queryType = "SUMMARY";
                SecuGlobal.showOK(panelStatus, lblStatus, "汇总查询");
            }
            else
            {
                queryType = "DETAIL";
                SecuGlobal.showOK(panelStatus, lblStatus, "明细查询");
            }

        }





        /// <summary>
        ///加载日期 、Vendor信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestList_Load(object sender, EventArgs e)
        {
            radioGroup2_SelectedIndexChanged(sender, e);  // 获取查询类型
            SecuGlobal.setDate(dateEditFrom, dateEditTo);

            if (PaCSGlobal.LoginUserInfo.Fct_code.Equals("C660A"))
                cbPlant.Text = "SSDP";
            else
                cbPlant.Text = "SESC";

            SecuGlobal.setAllVendorInfo(PaCSGlobal.LoginUserInfo.Fct_code, cbVendor); //根据FCT CODE 获取所有的VENDOR 信息

            SecuGlobal.showOK(panelStatus , lblStatus, "Ready");
        }








        /// <summary>
        /// 数据导出到EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "数据导出中..");
            PaCSGlobal.ExportGridToFile(grdView1, "Request List Info");
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
        }




        /// <summary>
        /// grdView1 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            if (e.Info.IsRowIndicator)
            {
                if (e.RowHandle >= 0)
                {
                    e.Info.DisplayText = (e.RowHandle + 1).ToString();
                }
                else if (e.RowHandle < 0 && e.RowHandle > -1000)
                {
                    e.Info.Appearance.BackColor = System.Drawing.Color.AntiqueWhite;
                    e.Info.DisplayText = "G" + e.RowHandle.ToString();
                }
            }
        }

        private void tbMeterial_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122)
            {
                e.KeyChar = (char)((int)e.KeyChar - 32);
            }
        }
    }
}