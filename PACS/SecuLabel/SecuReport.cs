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
using DevExpress.XtraSplashScreen;
using DevExpress.Data;
using System.Collections;

namespace SecuLabel
{
    public partial class SecuReport : DevExpress.XtraEditors.XtraForm
    {

        string ls_item = "", ls_sec_boxno = "", ls_sec_rollno = "", vendorCode = "", ls_status;
        string ls_sec_start = "", ls_sec_end = "";
        int iSelectionCounts = 0; // GridView选中数
        ArrayList snList = new ArrayList();



        public SecuReport()
        {
            InitializeComponent();
        }



        /// <summary>
        /// TAB显示不同查询内容
        /// 颜色标记，提示用户输入那些内容即可
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xtraTabControl1_Click(object sender, EventArgs e)
        {
            cbMeterial.Enabled = true;
            cbVendor.Enabled = true;
            ckStatus.Enabled = true;
            tbBoxNo.Enabled = true;
            tbRollNo.Enabled = true;
            tbSecuStart.Enabled = true;
            tbSecuEnd.Enabled = true;

            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                //部门库存查询，只需要Vendor Code ，模糊查询
                cbMeterial.Enabled = false ;
                ckStatus.Enabled = false;
                tbBoxNo.Enabled = false;
                tbRollNo.Enabled = false;
                tbSecuStart.Enabled = false;
                tbSecuEnd.Enabled = false;

            }
            else if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
            {
                //每卷使用明细查询 <需要材料编号 、 BOX NO 、 ROLL NO 防伪区间号，状态（O OR %） >
                //ls_item,ls_sec_boxno,ls_sec_rollno,ls_security_sn,ls_lifnr,ls_status vendorcode
 
            }
            else if (xtraTabControl1.SelectedTabPage == xtraTabPage3)
            {
                //范围段明细 <:ls_sec_start and :ls_sec_end >
                cbMeterial.Enabled = false ;
                ckStatus.Enabled = false;
                tbBoxNo.Enabled = false;
                tbRollNo.Enabled = false;
            }
            else
            {
                //报废未返回SN 处理Vendor 信息
                cbMeterial.Enabled = false;
                ckStatus.Enabled = false;
                tbBoxNo.Enabled = false;
                tbRollNo.Enabled = false;
                tbSecuStart.Enabled = false;
                tbSecuEnd.Enabled = false;
          
            }
        }




        /// <summary>
        /// 加载材料编号，日期，厂家CODE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecuReport_Load(object sender, EventArgs e)
        {
            SecuGlobal.setAllVendorInfo(PaCSGlobal.LoginUserInfo.Fct_code, cbVendor);
            SecuGlobal.getMeterialCode(cbMeterial);
            xtraTabControl1_Click(sender, e);
            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");
        }



        private void getVariable()
        {

            vendorCode = cbVendor.Text.Trim();

            if (!string.IsNullOrEmpty(vendorCode))
            {
                if (!vendorCode.Equals("ALL"))
                {
                    string[] split = vendorCode.Split(new Char[] { ':' });
                    vendorCode = split[0].Trim();
                    vendorCode = vendorCode + "%";
                }
                else
                {
                    vendorCode = "%";
                }
            }



            ls_item = cbMeterial.Text.Trim();

            if (!string.IsNullOrEmpty(ls_item))
            {
                if (!ls_item.Equals("ALL"))
                    ls_item = ls_item + "%";
                else
                    ls_item = "%";
            }

            if (!string.IsNullOrEmpty(tbBoxNo.Text))
                ls_sec_boxno = tbBoxNo.Text.Trim() + "%";
            else
                ls_sec_boxno = "%";

            if (!string.IsNullOrEmpty(tbRollNo.Text))
                ls_sec_rollno = tbRollNo.Text.Trim() + "%";
            else
                ls_sec_rollno = "%";


            if (ckStatus.Checked == true)
                ls_status = "O%";
            else
                ls_status = "%";


            if (!string.IsNullOrEmpty(tbSecuStart.Text))
                ls_sec_start = tbSecuStart.Text.Trim();

            if (!string.IsNullOrEmpty(tbSecuEnd.Text))
                ls_sec_end = tbSecuEnd.Text.Trim();


        }




        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                getVariable();

                if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                {
                    DialogResult dr = MessageBox.Show("当前页面 - < 部门库存查询 >，继续请按OK", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.OK)
                    {
                        backgroundWorker1.RunWorkerAsync();
                        SplashScreenManager.ShowForm(typeof(WaitLoading));
                    }
                }
                else if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
                {
                    DialogResult dr = MessageBox.Show("当前页面 - < 每卷使用明细查询 >，继续请按OK", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.OK)
                    {
                        backgroundWorker2.RunWorkerAsync();
                        SplashScreenManager.ShowForm(typeof(WaitLoading));
                    }
                }
                else if (xtraTabControl1.SelectedTabPage == xtraTabPage3)
                {

                    DialogResult dr = MessageBox.Show("当前页面 - < 范围段明细查询 >，继续请按OK", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.OK)
                    {
                        backgroundWorker3.RunWorkerAsync();
                        SplashScreenManager.ShowForm(typeof(WaitLoading));
                    }
                }
                else
                {
                    DialogResult dr = MessageBox.Show("当前页面 - < 报废未返回序列号 >，继续请按OK", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.OK)
                    {
                        backgroundWorker4.RunWorkerAsync();
                        SplashScreenManager.ShowForm(typeof(WaitLoading));
                    }
                }
            }
            catch (Exception ex)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, ex.Message);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }

        private void backgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }


        private void backgroundWorker5_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }



        /// <summary>
        /// 改变DataTable 标题栏及排列顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader(DataTable dt)
        {

            string[] colField = { "CENTER","ITEM", "STOCK" };

            string[] colName = { "部门信息", "材料编号", "库存数量信息" };

            for (int i = 0; i < colField.Length; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);

            }

            return dt;

        }


        /// <summary>
        /// 后台部门库存信息查询
        /// Vendor Code 模糊查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnQuery.Enabled = false;

                    DataTable dt = null;
                    dt = OracleHelper.ExecuteDataTable(getSql1(vendorCode));

                    if (dt == null) return;

                    gridControl1.DataSource = setDtHeader(dt);
                    grdDept.BestFitColumns();

                    //grdDept.Columns["CENTER"].Width = 150;
                    //grdDept.Columns["ITEM"].Width = 150;
                    //grdDept.Columns["STOCK"].Width = 150;
                    grdDept.Columns["材料编号"].SummaryItem.SummaryType = SummaryItemType.Count;
                    grdDept.Columns["材料编号"].SummaryItem.DisplayFormat = "All： {0:f0} ";
                    grdDept.Columns["库存数量信息"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdDept.Columns["库存数量信息"].SummaryItem.DisplayFormat = "Sum： {0:f0} ";

                    grdDept.Columns["库存数量信息"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far ;

                    btnQuery.Enabled = true;
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");

                });
            }
            catch (Exception err)
            {
                XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
            }

        }





        /// <summary>
        /// 部门库存信息查询SQL
        /// </summary>
        /// <returns></returns>
        private string getSql1(string vendorCode)
        {

            string  sql = " select center,item,count(*) stock " +
                        " from " +
                        " ( " +
                        " select nvl(vendor,'C660')||dept||shop center,serial_no,a.status status,b.item " +
                        " from " + SecuGlobal.tbSecurityInSnTest  + " a," + SecuGlobal.tbSecurityInTest  + " b " +
                        " where a.status in ('IN','OUT','INPUT') " +
                        " and nvl(vendor,'C660') like '" + vendorCode + "' " +
                        " and a.doc_no = b.doc_no " +
                        " and a.doc_seq = b.doc_seq and a.fct_code = b.fct_code and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                        " ) " +
                        " group by center,item " +
                        " order by center,item desc ";

            return(sql);
        }




        /// <summary>
        /// 每卷使用明细查询SQL
        /// </summary>
        /// <returns></returns>
        private string getSql2(string ls_item, string ls_sec_boxno, string ls_sec_rollno, string ls_sec_start, string ls_status)
        {
            string  sql =" select doc_no, doc_seq, scan_data,  " +
            "          (select count(*) from " + SecuGlobal.tbSecurityInSnTest + " b where b.fct_code = a.fct_code and b.serial_no between a.security_start and a.security_end and b.status = 'IN') in_cnt, " +
            "          (select count(*) from " + SecuGlobal.tbSecurityInSnTest + " b where b.fct_code = a.fct_code and b.serial_no between a.security_start and a.security_end and b.status = 'OUT') out_cnt, " +
            "          (select count(*) from " + SecuGlobal.tbSecurityInSnTest + " b where b.fct_code = a.fct_code and b.serial_no between a.security_start and a.security_end and b.status = 'INPUT') input_cnt, " +
            "          (select count(*) from " + SecuGlobal.tbSecurityInSnTest + " b where b.fct_code = a.fct_code and b.serial_no between a.security_start and a.security_end and b.status = 'SCAN') scan_cnt, " +
            "          (select count(*) from " + SecuGlobal.tbSecurityInSnTest + " b where b.fct_code = a.fct_code and b.serial_no between a.security_start and a.security_end and b.status = 'DEFECT') defect_cnt, " +
            "          (select count(*) from " + SecuGlobal.tbSecurityInSnTest + " b where b.fct_code = a.fct_code and b.serial_no between a.security_start and a.security_end and b.status = 'SCRAP') scrap_cnt, " +
            "          security_type, unit, item,  " +
            "          qty, box_no, roll_no,  " +
            "          lot_no, security_start, security_end,  " +
            "          gr_date, gr_time, gr_user,  " +
            "          gr_ip " +
            " from " + SecuGlobal.tbSecurityInTest  + " a " +
            " where 1= 1 " +
            " and item like '" + ls_item + "' " +
            " and nvl(box_no ,' ') like '" + ls_sec_boxno + "' " +
            " and nvl(roll_no,' ') like '" + ls_sec_rollno + "' " +
            " and nvl('" + ls_sec_start + "',security_start) between security_start and security_end " +
            " and exists (select 1 from tb_security_in_sn b where b.fct_code = a.fct_code and b.serial_no between a.security_start and a.security_end and nvl(b.vendor,'C660') like '" + vendorCode  + "') " +
            " and status like '" + ls_status + "' and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
            " order by security_start,security_end ";

            return (sql);
        }





        /// <summary>
        /// 范围段明细查询SQL
        /// </summary>
        /// <returns></returns>
        private string getSql3(string ls_sec_start, string ls_sec_end)
        {
           
            string sql = " select serial_no, " +
                        "        status, " +
                        "        vendor, " +
                        "        dept, " +
                        "        shop, " +
                        "        (select substr(max(add_datetime),1,8) from " + SecuGlobal.tbSnSecuHist + " b where b.fct_code = a.fct_code and a.serial_no = b.serial_no) input_date, " +
                        "        (select substr(max(add_datetime),9,6) from " + SecuGlobal.tbSnSecuHist + " b where b.fct_code = a.fct_code and a.serial_no = b.serial_no) input_time, " +
                        "        (select substr(max(scan_date||scan_time),1,8) from tb_subassy_prod b where a.serial_no = b.security_sn) scan_date, " +
                        "        (select substr(max(scan_date||scan_time),9,6) from tb_subassy_prod b where a.serial_no = b.security_sn) scan_time, " +
                        "        remark, " +
                        "        prodc_sn " +
                        "   from " + SecuGlobal.tbSecurityInSnTest  + " a " +
                        "  where serial_no between '" + ls_sec_start + "' and '" + ls_sec_end + "' and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";

            return (sql);
        }





        /// <summary>
        /// 报废未返回SN 处理SQL
        /// </summary>
        /// <returns></returns>
        private string getSql4(string vendorCode)
        {
            string sql = " select serial_no, doc_no, doc_seq,  " +
                         "    status, remark, shop,  " +
                         "    vendor, dept, update_datetime,  " +
                         "    update_user, prodc_sn, assign_wo,  " +
                         "    scan_wo, rtn_datetime  " +
                         " from " + SecuGlobal.tbSecurityInSnTest  + " a " +
                         " where status = 'SCRAP'  " +
                         " and rtn_datetime is null and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                         " and vendor like '" + vendorCode + "' " +
                         " and exists (select 1 from " + SecuGlobal.tbSecurityInTest  + " b where b.fct_code = a.fct_code and  a.doc_no = b.doc_no and a.doc_seq = b.doc_seq and b.status = 'O') " +
                         " order by vendor,serial_no ";

            return (sql);
        }



        /// <summary>
        /// 改变DataTable 标题栏及排列顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader2(DataTable dt)
        {
            //标题栏
            dt.Columns["doc_no"].ColumnName = "Doc No";
            dt.Columns["doc_seq"].ColumnName = "Doc Seq";
            dt.Columns["scan_data"].ColumnName = "Scan Data";
            dt.Columns["in_cnt"].ColumnName = "仓库库存";
            dt.Columns["out_cnt"].ColumnName = "部门库存";
            dt.Columns["input_cnt"].ColumnName = "投入生产线库存";
            dt.Columns["scan_cnt"].ColumnName = "已扫描数";
            dt.Columns["defect_cnt"].ColumnName = "扫描后删除数";
            dt.Columns["scrap_cnt"].ColumnName = "报废数";
            dt.Columns["security_type"].ColumnName = "Security Type";
            dt.Columns["unit"].ColumnName = "Unit";
            dt.Columns["item"].ColumnName = "Item";
            dt.Columns["qty"].ColumnName = "Qty";
            dt.Columns["box_no"].ColumnName = "Box No";
            dt.Columns["roll_no"].ColumnName = "Roll No";
            dt.Columns["lot_no"].ColumnName = "Lot No";
            dt.Columns["security_start"].ColumnName = "Security Start";
            dt.Columns["security_end"].ColumnName = "Security End";
            dt.Columns["gr_date"].ColumnName = "Gr Date";
            dt.Columns["gr_time"].ColumnName = "Gr Time";
            dt.Columns["gr_user"].ColumnName = "Gr User";
            dt.Columns["gr_ip"].ColumnName = "Gr Ip";

            //顺序改变
            dt.Columns["Security Type"].SetOrdinal(0);
            dt.Columns["Box No"].SetOrdinal(1);
            dt.Columns["Roll No"].SetOrdinal(2);
            dt.Columns["Security Start"].SetOrdinal(3);
            dt.Columns["Security End"].SetOrdinal(4);
            dt.Columns["Qty"].SetOrdinal(5);
            dt.Columns["仓库库存"].SetOrdinal(6);
            dt.Columns["部门库存"].SetOrdinal(7);
            dt.Columns["投入生产线库存"].SetOrdinal(8);
            dt.Columns["已扫描数"].SetOrdinal(9);
            dt.Columns["扫描后删除数"].SetOrdinal(10);
            dt.Columns["报废数"].SetOrdinal(11);
            dt.Columns["Unit"].SetOrdinal(12);
            dt.Columns["Item"].SetOrdinal(13);
            dt.Columns["Doc No"].SetOrdinal(14);
            dt.Columns["Doc Seq"].SetOrdinal(15);
            dt.Columns["Scan Data"].SetOrdinal(16);
            dt.Columns["Lot No"].SetOrdinal(17);
            dt.Columns["Gr Date"].SetOrdinal(18);
            dt.Columns["Gr Time"].SetOrdinal(19);
            dt.Columns["Gr User"].SetOrdinal(20);
            dt.Columns["Gr Ip"].SetOrdinal(21);

            return dt;
        }




        /// <summary>
        /// 每卷使用明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnQuery.Enabled = false;
                    DataTable dt = null;
                    dt = OracleHelper.ExecuteDataTable(getSql2(ls_item, ls_sec_boxno, ls_sec_rollno, ls_sec_start, ls_status));
                    if (dt == null)
                    {
                        btnQuery.Enabled = true;
                        return;
                    }
                        

                    dt = setDtHeader2(dt); //更改标题栏和显示顺序

                    gridControl2.DataSource = dt;
                    grdRoll.BestFitColumns();

                    grdRoll.Columns["Qty"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    grdRoll.Columns["Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdRoll.Columns["Qty"].SummaryItem.DisplayFormat = "Sum:{0:f0} ";

                    grdRoll.Columns["Qty"].Width = 100;
                    //grdRoll.Columns["Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    //grdRoll.Columns["Qty"].SummaryItem.DisplayFormat = "Sum:{0:f0} ";
                    grdRoll.Columns["仓库库存"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far ;
                    grdRoll.Columns["仓库库存"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdRoll.Columns["仓库库存"].SummaryItem.DisplayFormat = "{0:f0} ";

                    grdRoll.Columns["部门库存"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    grdRoll.Columns["部门库存"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdRoll.Columns["部门库存"].SummaryItem.DisplayFormat = "{0:f0} ";

                    grdRoll.Columns["投入生产线库存"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    grdRoll.Columns["投入生产线库存"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdRoll.Columns["投入生产线库存"].SummaryItem.DisplayFormat = "{0:f0} ";

                    grdRoll.Columns["已扫描数"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    grdRoll.Columns["已扫描数"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdRoll.Columns["已扫描数"].SummaryItem.DisplayFormat = "{0:f0} ";

                    grdRoll.Columns["扫描后删除数"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    grdRoll.Columns["扫描后删除数"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdRoll.Columns["扫描后删除数"].SummaryItem.DisplayFormat = "{0:f0} ";

                    grdRoll.Columns["报废数"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    grdRoll.Columns["报废数"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdRoll.Columns["报废数"].SummaryItem.DisplayFormat = "{0:f0} ";

                    //冻结
                    grdRoll.Columns["Security Type"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdRoll.Columns["Box No"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdRoll.Columns["Roll No"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdRoll.Columns["Security Start"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdRoll.Columns["Security End"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdRoll.Columns["Qty"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

                    btnQuery.Enabled = true;
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");

                });
            }
            catch (Exception err)
            {
                //XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
                SecuGlobal.showNG(panelStatus, lblStatus, err.Message );
            }

        }





        /// <summary>
        /// 改变DataTable 标题栏及排列顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader3(DataTable dt)
        {

            dt.Columns["serial_no"].ColumnName = "Serial No";
            dt.Columns["status"].ColumnName = "Status";
            dt.Columns["vendor"].ColumnName = "Vendor";
            dt.Columns["dept"].ColumnName = "Dept";
            dt.Columns["shop"].ColumnName = "Shop";
            dt.Columns["input_date"].ColumnName = "Input Date";
            dt.Columns["input_time"].ColumnName = "Input Time";
            dt.Columns["scan_date"].ColumnName = "Scan Date";
            dt.Columns["scan_time"].ColumnName = "Scan Time";
            dt.Columns["remark"].ColumnName = "Remark";
            dt.Columns["prodc_sn"].ColumnName = "Prodc Sn";

            //顺序改变
            dt.Columns["Serial No"].SetOrdinal(0);
            dt.Columns["Prodc Sn"].SetOrdinal(1);
            dt.Columns["Status"].SetOrdinal(2);
            dt.Columns["Vendor"].SetOrdinal(3);
            dt.Columns["Dept"].SetOrdinal(4);
            dt.Columns["Shop"].SetOrdinal(5);
            dt.Columns["Input Date"].SetOrdinal(6);
            dt.Columns["Input Time"].SetOrdinal(7);
            dt.Columns["Scan Date"].SetOrdinal(8);
            dt.Columns["Remark"].SetOrdinal(9);

            return dt;
        }




        /// <summary>
        /// 范围段明细查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnQuery.Enabled = false;

                    if (ls_sec_start.Equals("") || ls_sec_end.Equals(""))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "请输入必要条件-区间范围！");
                        btnQuery.Enabled = true;
                        return;
                    }
                        

                    DataTable dt = null;
                    dt = OracleHelper.ExecuteDataTable(getSql3(ls_sec_start, ls_sec_end));
                    if (dt == null)
                        return;
                    dt = setDtHeader3(dt);

                    gridControl3.DataSource = dt;
                    grdDetail.BestFitColumns();

                    grdDetail.Columns["Prodc Sn"].SummaryItem.SummaryType = SummaryItemType.Count;
                    grdDetail.Columns["Prodc Sn"].SummaryItem.DisplayFormat = "All： {0:f0} ";

                    btnQuery.Enabled = true;
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");

                });
            }
            catch (Exception err)
            {
                XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
                //SecuGlobal.showNG(panelStatus, lblStatus, err.Message);
            }


        }






        /// <summary>
        /// 改变DataTable 标题栏及排列顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader4(DataTable dt)
        {   
            dt.Columns["serial_no"].ColumnName = "Serial No";
            dt.Columns["doc_no"].ColumnName = "Doc No";
            dt.Columns["doc_seq"].ColumnName = "Doc Seq";
            dt.Columns["status"].ColumnName = "Status";
            dt.Columns["remark"].ColumnName = "Remark";
            dt.Columns["shop"].ColumnName = "Shop";
            dt.Columns["vendor"].ColumnName = "Vendor";
            dt.Columns["dept"].ColumnName = "Dept";
            dt.Columns["update_datetime"].ColumnName = "Update DataTime";
            dt.Columns["update_user"].ColumnName = "Update User";
            dt.Columns["prodc_sn"].ColumnName = "Prodc Sn";
            dt.Columns["assign_wo"].ColumnName = "Assign Wo";
            dt.Columns["scan_wo"].ColumnName = "Scan Wo";
            dt.Columns["rtn_datetime"].ColumnName = "Rtn DateTime";

            return dt;
        }




        /// <summary>
        /// GridView Check Box 点击事件
        /// </summary>
        private void grdCheckSelect_SelectionChanged(object sender, EventArgs e)
        {
            iSelectionCounts = 0;
            for (int i = 0; i < grdSN.RowCount; i++)
            {
                for (int j = 0; j < grdSN.Columns.Count; j++)
                {
                    if ((bool)grdSN.GetRowCellValue(i, grdSN.Columns[0]))
                    {
                        iSelectionCounts += 1;
                        break;
                    }
                }
            }
            grdSN.Columns["Doc No"].SummaryItem.SummaryType = SummaryItemType.Count;
            grdSN.Columns["Doc No"].SummaryItem.DisplayFormat = "选中:" + iSelectionCounts;

        }






        /// <summary>
        /// 报废未返回序列号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnQuery.Enabled = false;

                    DataTable dt = null;
                    dt = OracleHelper.ExecuteDataTable(getSql4(vendorCode));
                    dt = setDtHeader4(dt);

                    gridControl4.DataSource = dt;
                    grdSN.BestFitColumns();

                    GridCheckMarksSelection grdCheckSelect = new GridCheckMarksSelection(grdSN);
                    grdCheckSelect.CheckMarkColumn.VisibleIndex = 0;
                    grdCheckSelect.SelectionChanged += grdCheckSelect_SelectionChanged;

                    for (int i = 1; i < grdSN.Columns.Count ; i++)
                    {
                        grdSN.Columns[i].OptionsColumn.ReadOnly = true;
                    }


                        grdSN.Columns["Serial No"].SummaryItem.SummaryType = SummaryItemType.Count;
                    grdSN.Columns["Serial No"].SummaryItem.DisplayFormat = "All： {0:f0} ";

                    btnQuery.Enabled = true;
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                });
            }
            catch (Exception err)
            {
                //XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
                SecuGlobal.showNG(panelStatus, lblStatus, err.Message);
            }
        }





        /// <summary>
        /// 后台进行SN 报废返回处理 
        /// 完成后GRIDVIEW 数据更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                snList.Clear();
                for (int i = 0; i < grdSN.RowCount; i++)
                {
                    for (int j = 0; j < grdSN.Columns.Count; j++)
                    {
                        if ((bool)grdSN.GetRowCellValue(i, grdSN.Columns[0]))
                        {
                            object val = grdSN.GetRowCellValue(i, grdSN.Columns["Serial No"]);
                            OracleHelper.ExecuteNonQuery(updateDataSql(val.ToString()));  //更新数据后记录对应SN
                            snList.Add(val);
                            break;
                        }
                    }
                }


                this.Invoke((MethodInvoker)delegate
                {
                    // 移除对应SN，这样效率更高
                    foreach (object obj in snList)
                    {
                        for (int i = 0; i < grdSN.RowCount; i++)
                        {
                            object val = grdSN.GetRowCellValue(i, grdSN.Columns["Serial No"]);
                            if (val.ToString().Equals(obj.ToString()))
                            {
                                grdSN.DeleteRow(i);
                                break;
                            }
                        }
                    }
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                });
            }
            catch (Exception err)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, err.Message);
            }




        }








        /// <summary>
        /// 报废未处理SN在利用信息更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReUse_Click(object sender, EventArgs e)
        {

            try 
            {
                if (iSelectionCounts == 0)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请首先选择要返回的报废标签数据");
                    return;
                }

                DialogResult drg = MessageBox.Show("您确信要返回 " + iSelectionCounts + "  条报废标签吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (drg == DialogResult.OK)
                    {
                        backgroundWorker5.RunWorkerAsync();
                        SplashScreenManager.ShowForm(typeof(WaitLoading));
                    }

            }
            catch (Exception ex)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, ex.Message);
            }
        }





        /// <summary>
        /// 获取需要报废SN 的UPDATE 语句
        /// </summary>
        /// <param name="ls_serial_no"></param>
        /// <returns></returns>

        private string  updateDataSql(string ls_serial_no)
        {
            string gs_userid  = PaCSTools.PaCSGlobal.LoginUserInfo.Name ;

            string sql = "update " + SecuGlobal.tbSecurityInSnTest + "  set rtn_datetime = to_char(sysdate,'yyyymmddHH24miss'), " +
                         "update_user = '" + gs_userid + "' where serial_no = '" + ls_serial_no + "' and status = 'SCRAP' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";

            //string sql = "update tb_security_in_sn_test  set rtn_datetime = to_char(sysdate,'yyyymmddHH24miss'), " + 
            //            "update_user = '" + gs_userid + "' where serial_no = 'SECD4AR66740407' and status = 'SCRAP'";

            return sql;
        }





        /// <summary>
        /// 设置gridView 行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setRowNumber(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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





        private void grdView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            setRowNumber(sender, e);
        }


        private void grdRoll_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            setRowNumber(sender, e);
        }

        private void grdDetail_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            setRowNumber(sender, e);
        }

        private void grdSN_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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

        private void btnExport_Click(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus ,lblStatus, "数据导出中,请稍等...");

            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                PaCSGlobal.ExportGridToFile(grdDept, "Dept Info");
            }
            else if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
            {
                PaCSGlobal.ExportGridToFile(grdRoll, "Roll Info");
            }
            else if (xtraTabControl1.SelectedTabPage == xtraTabPage3)
            {
                PaCSGlobal.ExportGridToFile(grdDetail, "Detial Info");
            }
            else
            {
                PaCSGlobal.ExportGridToFile(grdSN , "Sn Info");
            }
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
        }



    }
}