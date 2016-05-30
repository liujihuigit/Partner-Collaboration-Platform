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
using System.IO.Ports;
using DevExpress.XtraSplashScreen;
using DevExpress.Data;

namespace SecuLabel
{
    public partial class VendorInput : DevExpress.XtraEditors.XtraForm
    {

        List<string> listSN = new List<string>();

        private SerialPort[] ports = new SerialPort[1];
        private delegate void InvokeDelegate(string data);
        private string receivedata = "";

        string vendorCode = "";  // 4位
        string ls_sdate, ls_edate;
        string ls_send_sdate, ls_send_edate;
        string ls_shop;
        string ls_item; //材料编号
        string ls_work_order, ls_pre_work_order; //包装计划  和 生产计划

        string is_brand = "SAMSUNG";
        string is_secu_code = "JC68-01673A";
        string ls_excp_out, ls_excp_reason; //例外出库 和 原因

        string ls_from_sn,ls_to_sn; //SN 范围
        int ll_from_num,ll_to_num;
        string ls_from_sn1,ls_to_sn1;
        string ls_from_sn2,ls_to_sn2;

        int iSelectedCounts = 0;   //生产计划数选中的数量
        int ll_actual_out_qty = 0; // 实际发料数

        

        public VendorInput()
        {
            ports[0] = new SerialPort();
            InitializeComponent();
        }
        /// <summary>
        /// Com 口数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                System.Threading.Thread.Sleep(100); //读取速度慢，加Sleep延长读取时间, 不可缺少
                //serialPort1.DiscardInBuffer();  //如果不执行上面的代码,serialPort1_DataReceived会执行多次

                int n = ports[0].BytesToRead;
                byte[] buf = new byte[n];
                ports[0].Read(buf, 0, n);
                // ports[0].ReceivedBytesThreshold = 15; // 在检讨~~

                receivedata = System.Text.Encoding.ASCII.GetString(buf);
                receivedata = receivedata.Replace("\r\n", "");

                try
                {
                    this.Invoke(new InvokeDelegate(DoData), receivedata);
                }
                catch (Exception DataReceived)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        ////要委托的代码 
                        //lbStatus.Text = "【" + receivedata + "】：出库失败";
                        XtraMessageBox.Show(this, "System error[DataReceived]: " + DataReceived.Message);
                    }));
                }
            }
        }


        /// <summary>
        /// 扫描枪设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnComSeting_Click(object sender, EventArgs e)
        {
            SettingForm setcom = new SettingForm("SecuLabel", "VendorInput", 1);
            DialogResult dg = setcom.ShowDialog();

            if (dg == DialogResult.OK)
            {
                PaCSGlobal.InitComPort("SecuLabel", "VendorInput", ports);

                if (ports[0].IsOpen)
                    ports[0].DataReceived += new SerialDataReceivedEventHandler(DataReceived);//重新绑定
            }
        }



        /// <summary>
        /// 关闭form 时 释放COM 资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VendorInput_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ports[0].DataReceived -= new SerialDataReceivedEventHandler(DataReceived);//取消绑定

                foreach (SerialPort port in ports)
                {
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                }
            }
            catch (Exception IssueAssy_FormClosing)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, IssueAssy_FormClosing.ToString());
            }
        }







        //基本信息加载
        private void VendorInput_Load(object sender, EventArgs e)
        {
            PaCSGlobal.InitComPort("SecuLabel", "VendorInput", ports);

            if (ports[0].IsOpen)
                ports[0].DataReceived += new SerialDataReceivedEventHandler(DataReceived);//重新绑定

            if (PaCSGlobal.LoginUserInfo.Fct_code.Equals("C660A"))
                cbPlant.Text = "SSDP";
            else
                cbPlant.Text = "SESC";


            string bufVend = SecuGlobal.getPopVendorInfo(PaCSGlobal.LoginUserInfo.Venderid, PaCSGlobal.LoginUserInfo.Fct_code);
            if (!bufVend.Equals(""))
            {
                cbVendor.Text = bufVend;
            }
            else
            {
                cbVendor.Text = PaCSGlobal.LoginUserInfo.Venderid + ":" + PaCSGlobal.LoginUserInfo.Vendername;  //苏州法人
            }

            getVendorCode4();
            SecuGlobal.getLineInfo(cbShop , vendorCode);
            SecuGlobal.setDate1(dateEditFrom, dateEditTo);
            dateEdit1.Text = PaCSGlobal.GetServerDateTime(3);
            dateEdit2.Text = PaCSGlobal.GetServerDateTime(3);

            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");

        }





        /// <summary>
        /// 获取四位VENDOR CODE - EXP:　BP3A
        /// </summary>
        private void getVendorCode4()
        {
            vendorCode = cbVendor.Text.Trim();
            if (!string.IsNullOrEmpty(vendorCode))
            {
                if (!vendorCode.Equals("ALL"))
                {
                    string[] split = vendorCode.Split(new Char[] { ':' });
                    vendorCode = split[0].Trim();
                }
            }
        }




        /// <summary>
        /// 扫描枪获取数据处理
        /// </summary>
        /// <param name="data"></param>
        private void DoData(string data)
        {
            tbSn.Text = data;
            doWork(tbSn.Text);
        }





        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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



        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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

        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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




        /// <summary>
        /// 后台查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "正在查询数据，请稍等...");
                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));
   
            }
            catch (Exception ex)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, ex.Message);
            }
        }





        /// <summary>
        /// 获取变量信息
        /// </summary>
        private void getVariable()
        {

            ls_sdate = dateEditFrom.Text.Trim().Replace("-", "");
            ls_edate = dateEditTo.Text.Trim().Replace("-", "");

            ls_send_sdate = dateEdit1.Text.Trim().Replace("-", "");
            ls_send_edate = dateEdit2.Text.Trim().Replace("-", "");

            if (cbShop.SelectedIndex == -1 || cbShop.Text.Equals("ALL") || string.IsNullOrEmpty(cbShop.Text))
            {
                ls_shop = cbShop.Text.Trim() + "%";
            }
            else
            {
                ls_shop = "%";
            }

            ls_item = tbMaterial.Text.Trim() + "%";
            ls_work_order = tbPackingOrder.Text.Trim() + "%";
            ls_pre_work_order = tbProdOrder.Text.Trim() + "%";
        }







        /// <summary>
        /// 后台查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
  
                getVariable();  //变量信息加载

                this.Invoke((MethodInvoker)delegate
                {
                    btnApply.Enabled = false;
                    SecuGlobal.GridViewInitial(gridView1, gridControl2);
                    SecuGlobal.GridViewInitial(gridView2, gridControl1);
                    SecuGlobal.GridViewInitial(gridView3, gridControl3);
                    
                    showData_gridView1();  //显示计划信息
                    showData_gridView2();  //加载发料信息
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                    btnApply.Enabled = true ;
                });
            }
            catch (Exception err)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, err.Message);
            }
        }



       /// <summary>
       /// 加载计划信息
       /// </summary>
        private void  showData_gridView1()
        {
            DataTable dt = null;
            dt = OracleHelper.ExecuteDataTable(getSql(ls_sdate, ls_edate, vendorCode, ls_shop, ls_item, ls_work_order, ls_pre_work_order));
            if (dt == null)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有信息");
                return;
            }

            dt = setDtHeader(dt); //更改标题栏和显示顺序

            gridControl1.DataSource = dt;
            gridView1.BestFitColumns();

            GridCheckMarksSelection grdCheckSelect = new GridCheckMarksSelection(gridView1);
            grdCheckSelect.CheckMarkColumn.VisibleIndex = 0;
            grdCheckSelect.SelectionChanged += grdCheckSelect_SelectionChanged;

            gridView1.Columns["Pre Work Order"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView1.Columns["Pre Work Order"].SummaryItem.DisplayFormat = "All:{0:f0} ";

            //gridView1.Columns["Plan Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;  //显示计划总数没有意义，选中计划总数体现就可以了
            //gridView1.Columns["Plan Qty"].SummaryItem.DisplayFormat = "{0:f0} ";

            gridView1.Columns["Plan Qty"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far ;
            gridView1.Columns["Input Qty"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gridView1.Columns["Scan Qty"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gridView1.Columns["Input Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
            gridView1.Columns["Input Qty"].SummaryItem.DisplayFormat = "{0:f0} ";
            gridView1.Columns["Scan Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
            gridView1.Columns["Scan Qty"].SummaryItem.DisplayFormat = "{0:f0} ";

            for (int i = 1; i < gridView1.Columns.Count ; i++)
            {
                gridView1.Columns[i].OptionsColumn.ReadOnly = true;
            }

        }






        /// <summary>
        /// 加载发料信息
        /// </summary>
        private void showData_gridView2()
        {
            DataTable dt = null;
            dt = OracleHelper.ExecuteDataTable(getSql2(ls_send_sdate, ls_send_edate, vendorCode, ls_shop, is_brand, ls_work_order));

            dt = setDtHeader2(dt); //更改标题栏和显示顺序

            gridControl2.DataSource = dt;
            gridView2.BestFitColumns();

            gridView2.Columns["Work Order"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView2.Columns["Work Order"].SummaryItem.DisplayFormat = "All:{0:f0} ";
            gridView2.Columns["Qty"].Width = 60;
            gridView2.Columns["Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
            gridView2.Columns["Qty"].SummaryItem.DisplayFormat = "{0:f0} ";       
        }




        /// <summary>
        /// GridView Check Box 点击事件
        /// 全局变量，是否有选中的项目
        /// </summary>
        private void grdCheckSelect_SelectionChanged(object sender, EventArgs e)
        {
            iSelectedCounts = 0;
            int iPlanQtyAll = 0, iPlanQty = 0;

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                for (int j = 0; j < gridView1.Columns.Count; j++)
                {
                    if ((bool)gridView1.GetRowCellValue(i, gridView1.Columns[0]))
                    {
                        int iInputQty = System.Convert.ToInt32(gridView1.GetRowCellValue(i, gridView1.Columns["Input Qty"]));
                        iPlanQty = System.Convert.ToInt32(gridView1.GetRowCellValue(i, gridView1.Columns["Plan Qty"]));  //生产计划量
                       
                        if (iInputQty == iPlanQty)
                        {
                            gridView1.SetRowCellValue(i, gridView1.Columns[0], false);  //已经发料的信息不允许选中
                            break;
                        }
                        else
                        {
                            iPlanQtyAll += iPlanQty;
                            iSelectedCounts += 1;
                            break;
                        }
                        
                    }
                }
            }

            gridView1.Columns["Plan Qty"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gridView1.Columns["Plan Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
            gridView1.Columns["Plan Qty"].SummaryItem.DisplayFormat = iPlanQtyAll.ToString();

            

            SecuGlobal.GridViewInitial(gridView2, gridControl2); 
            //SecuGlobal.GridViewInitial(gridView3, gridControl3);
        }





        private string getSql2(string ls_send_sdate, string ls_send_edate, string gs_vend_code, string ls_shop, string is_brand, string ls_work_order)
        {
            string sql = " select FCT_CODE,line,sec_itemcd,sec_cnt,sec_start,sec_end,work_order,excp_out,excp_reason,sec_sign, " +
                        "        vendor,sec_sn,sec_cdate,sec_ctime,sec_buyer,sec_calc_sum,store2_in, " +
                        "        brand,pre_work_order " +
                        " from " + SecuGlobal.tbPopSecuIn  + " " +
                        " where sec_cdate between '" + ls_send_sdate + "' and '" + ls_send_edate + "' " +
                        "   and vendor = '" + gs_vend_code + "' " +
                        "   and line like '" + ls_shop + "' " +
                        "   and brand like '" + is_brand + "' " +
                        "   and work_order like '" + ls_work_order + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                        " order by sec_sn desc ";
            return sql;
        }



        /// <summary>
        /// 更改标题栏和显示顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader2(DataTable dt)
        {

            string[] colField = {"FCT_CODE", "pre_work_order", "sec_cnt", "sec_start", "sec_end", "sec_itemcd", "line","work_order","excp_out","excp_reason",
                                    "sec_sign","vendor","sec_sn","sec_cdate","sec_ctime" ,"sec_buyer","sec_calc_sum",
                                    "store2_in","brand"};

            string[] colName =  {"Fct Code", "Pre Work Order", "Qty", "Sec Start", "Sec End", "Sec ItemCd", "Line","Work Order","Excp Out","Excp Reason",
                                    "Sec Sign","Vendor","Sec Sn","Sec Cdate","Sec Ctime" ,"Sec Buyer","Sec Calc Sum",
                                    "Store2 In","Brand"};


            for (int i = 0; i < colField.Length ; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);
            }
            return dt;
        }





        /// <summary>
        /// 双击对应信息，GRIDVIEW3中显示详细的SN 信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView2_DoubleClick(object sender, EventArgs e)
        {
            string ll_sec_sn = this.gridView2.GetRowCellValue(this.gridView2.FocusedRowHandle, this.gridView2.Columns["Sec Sn"]).ToString();
            SecuGlobal.GridViewInitial(gridView3, gridControl3); //清除
            DataTable dt = OracleHelper.ExecuteDataTable(getSql3(vendorCode, ll_sec_sn));

            if (dt == null)
                return;

            dt = setDtHeader3(dt);
            gridControl3.DataSource = dt;

            gridView3.BestFitColumns();
            gridView3.Columns["SerialNo"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView3.Columns["SerialNo"].SummaryItem.DisplayFormat = "{0:f0} ";
        }



        private string getSql3(string ls_vendor, string ll_sec_sn)
        {
            string sql = " select FCT_CODE,serial_no,vendor,sec_sn,add_datetime,work_order,shop " +
                        " from " + SecuGlobal.tbSnSecuHist  + " " +
                        " where vendor = '" + ls_vendor + "' " +
                        " and sec_sn = '" + ll_sec_sn + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
            return sql;
        }



        private DataTable setDtHeader3(DataTable dt)
        {

            string[] colField = { "FCT_CODE", "serial_no", "vendor", "sec_sn", "add_datetime", "work_order", "shop" };

            string[] colName = { "Fct Code", "SerialNo", "Vendor", "Sec Sn", "Add DateTime", "Work Order", "Shop" };


            for (int i = 0; i < colField.Length; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);
            }
            return dt;
        }





        private DataTable setDtHeader(DataTable dt)
        {

            string[] colField = { "pre_work_order", "pre_plan_date", "pre_shop", "pre_item", "work_order", "plan_date","shop","item","plan_qty",
                                    "input_qty","scan_qty","pre_start_date","pre_start_time","pre_end_date" ,"pre_end_time","prod_qty",
                                    "order_type","start_date","start_time","del_flag","end_date","end_time","buyer","dest","brand","sales_order","sales_item"};

            string[] colName =  { "Pre Work Order", "Pre Plan Date", "Pre Shop", "Pre Item", "Work Order", "Plan Date","Shop","Item","Plan Qty",
                                    "Input Qty","Scan Qty","Pre Start Date","Pre Start Time","Pre End Date" ,"Pre End Time","Prod Qty",
                                    "Order Type","Start Date","Start Time","Del Flag","End Date","End Time","Buyer","Dest","Brand","Sales Order","Sales Item"};

            for (int i = 0; i < colField.Length ; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);
            }

            return dt;
        }




        /// <summary>
        /// 查询计划信息  已经修改
        /// </summary>
        /// <param name="ls_sdate"></param>
        /// <param name="ls_edate"></param>
        /// <param name="gs_vend_code"></param>
        /// <param name="ls_shop"></param>
        /// <param name="ls_item"></param>
        /// <param name="ls_work_order"></param>
        /// <param name="ls_pre_work_order"></param>
        /// <returns></returns>
        private string getSql(string ls_sdate, string ls_edate, string gs_vend_code, string ls_shop, string ls_item, string ls_work_order, string ls_pre_work_order)
        {
            string sql = " select  a.work_order,a.plan_date,a.shop,a.item, " +
                        "        a.plan_qty,a.prod_qty,a.order_type,a.start_date,a.start_time,a.del_flag,a.end_date,a.end_time,a.buyer,a.dest,a.brand,a.sales_order,a.sales_item, " +
                        "        (select count(*) from " + SecuGlobal.tbSecurityInSnTest  + " b where a.work_order = b.assign_wo ) input_qty, " +
                        "        (select count(*) from " + SecuGlobal.tbSecurityInSnTest + " b where a.work_order = b.assign_wo and b.status = 'SCAN') scan_qty, " +
                        "          b.work_order pre_work_order,        " +
                        "          b.plan_date pre_plan_date, " +
                        "          b.shop pre_shop, " +
                        "          b.item pre_item, " +
                        "          b.start_date pre_start_date, " +
                        "          b.start_time pre_start_time, " +
                        "          b.end_date pre_end_date, " +
                        "          b.end_time pre_end_time " +
                        " from " + SecuGlobal.tb_plan  + " a, " +
                        "      " + SecuGlobal.tb_plan + " b " +
                        " where nvl(b.work_order,' ') like '" + ls_pre_work_order + "' " +
                        " and exists (select 1 from gmes20_line  b where b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' and b.use_yn = 'Y' and b.del_yn = 'N' and b.vend_code = '" + PaCSGlobal.LoginUserInfo.Venderid  + "' and b.proc_type_code = '72' and a.shop = b.erp_wc_code) " +
                        " and a.del_flag = 'O' " +
                        " and a.shop like '" + ls_shop + "' " +
                        " and a.item like '" + ls_item  + "' " +
                        " and a.work_order like '" + ls_work_order + "' " +
                        " and (b.prev_wo(+) = a.work_order and b.work_order(+) <> a.work_order) " +
                        " and a.plan_date between '" + ls_sdate + "' and '" + ls_edate + "' " +
                        " order by b.plan_date,b.shop,b.start_date||b.start_time,a.brand,a.buyer ";
            return sql;
        }





        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }




        /// <summary>
        /// 大写控制/ USB扫描枪-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122)
            {
                e.KeyChar = (char)((int)e.KeyChar - 32);
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                doWork(tbSn.Text.Trim());  // 读取的信息分析处理
            }
        }



        /// <summary>
        /// 基本条件、扫描数据分析
        /// </summary>
        /// <param name="scanData"></param>
        /// <returns></returns>
        private bool bAnalysis(string scanData)
        {
            try
            {
                if (rbSamsung.Checked == false && rbXerox.Checked == false)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请选择类型SAMSUNG 或者 XEROX");
                    return false;
                }

                if (ckException.Checked)
                    ls_excp_out = "Y";
                else
                    ls_excp_out = "N";
                ls_excp_reason = tbExpReason.Text.Trim();
                if (ls_excp_out.Equals("Y") && string.IsNullOrEmpty(ls_excp_reason))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "S请输入例外出库原因!");
                    return false ;
                }

                if (vendorCode.Equals("C660"))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "SSDP 社内不适用此画面！");
                    return false ;
                }

                if (is_brand.Equals("%"))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请选择三星或施乐!");
                    return false ;
                }

                string sql = "select count(*) from " + SecuGlobal.tbSnSecuHist + " where serial_no = '" + scanData + "' " +
                             "and FCT_CODE  = '"+ PaCSGlobal.LoginUserInfo.Fct_code  +"'";
                int ll_cnt = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql));
                if (!ll_cnt.Equals(0))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "SN重复扫描！");
                    return false ;
                }

                if (is_brand.Equals("SAMSUNG") && !scanData.Length.Equals(15))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "三星防伪标签长度错误，应为15位!");
                    return false;
                }


                if (is_brand.Equals("XEROX") && !scanData.Length.Equals(9))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "施乐防伪标签长度错误，应为9位!");
                    return false ;
                }

                if (iSelectedCounts <= 0)  //是否有选中数量
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请选择生产计划!");
                    return false;
                }

                return true;
            }
            catch(Exception )
            {
                return false;
            }
        }




        /// <summary>
        /// SN范围获取OK，根据SN范围计算所有的SN LIST
        /// </summary>
        /// <param name="ls_from_sn"></param>
        /// <param name="ls_to_sn"></param>
        private void ProcessingSnRange(string ls_from_sn, string ls_to_sn)
        {
            listSN.Clear();
            switch (is_brand)
            {
                case "SAMSUNG":
                    ll_from_num = System.Convert.ToInt32(ls_from_sn.Substring(7, 8)); //取序列号部分
                    ll_to_num = System.Convert.ToInt32(ls_to_sn.Substring(7, 8));
                    string sqlCount = "select count(*) from dual where ceil('" + ll_from_num + "'/2500) = ceil('" + ll_to_num + "'/2500)";
                    int iiS = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sqlCount));

                    if (iiS.Equals(1))
                    {
                        if (ll_from_num > ll_to_num)
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "结束号应比开始号大!");
                            return;

                        }
                        wf_gene_list(ls_from_sn, ls_to_sn, 7, 8, 8);
                    }
                    else if (iiS.Equals(0) || !ls_from_sn.Substring(0, 6).Equals(ls_to_sn.Substring(0, 6)))
                    {
                        ls_from_sn1 = ls_from_sn;
                        ls_to_sn2 = ls_to_sn;
                        string sql1_Samsung = "select substr('" + ls_from_sn1 + "',1,7)||trim(lpad(ceil('" + ll_from_num + "'/2500)*2500,8,'0')) from dual";
                        ls_to_sn1 = OracleHelper.ExecuteScalar(sql1_Samsung).ToString();
                        string sql2_Samsung = "select substr('" + ls_to_sn2 + "',1,7)||trim(lpad(ceil('" + ll_to_num + "'/2500-1)*2500+1,8,'0')) from dual";
                        ls_from_sn2 = OracleHelper.ExecuteScalar(sql2_Samsung).ToString();
                        wf_gene_list(ls_from_sn1, ls_to_sn1, 7, 8, 8);
                        wf_gene_list(ls_from_sn2, ls_to_sn2, 7, 8, 8);
                    }
                    break;

                case "XEROX":
                    ll_from_num = System.Convert.ToInt32(ls_from_sn.Substring(2, 7)); //取序列号部分
                    ll_to_num = System.Convert.ToInt32(ls_to_sn.Substring(2, 7));
                    string sql1 = "select count(*) from dual where ceil('" + ll_from_num + "'/10000) = ceil('" + ll_to_num + "'/10000)";
                    int iiX = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql1));

                    if (iiX.Equals(1))
                    {
                        if (ll_from_num > ll_to_num)
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "结束号应比开始号大!");
                            return;

                        }
                        wf_gene_list(ls_from_sn, ls_to_sn, 2, 3, 7);
                    }
                    else if (iiX.Equals(0) || !ls_from_sn.Substring(0, 2).Equals(ls_to_sn.Substring(0, 2)))
                    {
                        ls_from_sn1 = ls_from_sn;
                        ls_to_sn2 = ls_to_sn;
                        string sql2 = "select substr('" + ls_from_sn1 + "',1,2)||trim(lpad(ceil('" + ll_from_num + "'/10000)*10000,7,'0')) from dual";
                        ls_to_sn1 = OracleHelper.ExecuteScalar(sql2).ToString();
                        string sql3 = "select substr('" + ls_to_sn2 + "',1,2)||trim(lpad(ceil('" + ll_to_num + "'/10000-1)*10000+1,7,'0')) from dual";
                        ls_from_sn2 = OracleHelper.ExecuteScalar(sql3).ToString();
                        wf_gene_list(ls_from_sn1, ls_to_sn1, 7, 8, 8);
                        wf_gene_list(ls_from_sn2, ls_to_sn2, 7, 8, 8);
                    }
                    break;
            }
        }




        /// <summary>
        /// 选中记录中，Plan Qty求和并且返回
        /// </summary>
        /// <returns></returns>
        private int selectPlanQty()
        {
            int iPlanQtyAll = 0, iPlanQty = 0;
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                for (int j = 0; j < gridView1.Columns.Count; j++)
                {
                    if ((bool)gridView1.GetRowCellValue(i, gridView1.Columns[0]))
                    {
                        iPlanQty = System.Convert.ToInt32(gridView1.GetRowCellValue(i, gridView1.Columns["Plan Qty"]));  //生产计划量
                        iPlanQtyAll += iPlanQty;
                        break;
                    }
                }
            }
            return iPlanQtyAll;
        }



        /// <summary>
        /// 获取SN List 然后将SN信息增加到LIST中
        /// </summary>
        /// <param name="ls_sec_start"></param>
        /// <param name="ls_sec_end"></param>
        /// <param name="ll_prefix_length"></param>
        /// <param name="ll_num_start"></param>
        /// <param name="ll_num_length"></param>
        private void  wf_gene_list(string ls_sec_start, string ls_sec_end, int ll_prefix_length, int ll_num_start, int ll_num_length)
        {
            string sql = " select substr('" + ls_sec_start + "',1," + ll_prefix_length + ")||" +
                         " lpad(to_number(substr('" + ls_sec_start + "'," + ll_num_start + "," + ll_num_length + "))+rownum-1," + ll_num_length + ",0) " +
                         " from dual " +
                         " connect by rownum <= to_number(substr('" + ls_sec_end + "'," + ll_num_start + "," + ll_num_length + "))" +
                         " - to_number(substr('" + ls_sec_start + "'," + ll_num_start + "," + ll_num_length + ")) + 1 ";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                listSN.Add(dr[0].ToString());
                object[] value = { dr[0].ToString(), "", "", "", "", "" };
                addGridView3(value);
            }
        }



        private void rbSamsung_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSamsung.Checked)
            {
                is_brand = "SAMSUNG";
                is_secu_code = "JC68-01673A";
            }
            else 
            {
                is_brand = "XEROX";
                is_secu_code = "JC68-01670A";
            }
            SecuGlobal.showOK(panelStatus, lblStatus, "当前类型： " + is_brand + " Code: " + is_secu_code );
        }




        private void rbXerox_CheckedChanged(object sender, EventArgs e)
        {
            if (rbXerox.Checked)
            {
                is_brand = "XEROX";
                is_secu_code = "JC68-01670A";
            }
            else
            {
                is_brand = "SAMSUNG";
                is_secu_code = "JC68-01673A";
            }
            SecuGlobal.showOK(panelStatus, lblStatus, "当前类型： " + is_brand + " Code: " + is_secu_code);
        }




        /// <summary>
        /// 处理SN 信息
        /// </summary>
        /// <param name="scanData"></param>
        private void doWork(string scanData)
        {
            try
            {
                if (!bAnalysis(scanData))  //分析基本条件
                    return;

                if (string.IsNullOrEmpty(tbSnFrom.Text.Trim()))   //获取SN扫描区间
                {
                    tbSnFrom.Text = scanData;
                    ls_from_sn = tbSnFrom.Text.Trim();
                    SecuGlobal.showOK(panelStatus, lblStatus, "起始序列号获取成功！");
                }
                else
                {
                    tbSnTo.Text = scanData;
                    ls_to_sn = tbSnTo.Text.Trim();
                    SecuGlobal.showOK(panelStatus, lblStatus, "序列号范围分析和拆分中，请稍等...");


                    backgroundWorker2.RunWorkerAsync();  //后台运算SN list 、拆分及DB upload  


                    SplashScreenManager.ShowForm(typeof(WaitLoading));
                }

            }
            catch (Exception doWork)
            {
                SecuGlobal.showOK(panelStatus, lblStatus, doWork.Message );
            }
            finally
            {
                tbSn.Text = "";
            }
        }


        /// <summary>
        /// 后台计算和显示SN，因为数量过多，
        /// 所以才去后台方式运行。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    SecuGlobal.GridViewInitial(gridView2, gridControl2); //数据清空 
                    SecuGlobal.GridViewInitial(gridView3, gridControl3); //必须进行数据清空，否则data table 增加是出现问题

                    ProcessingSnRange(ls_from_sn, ls_to_sn);

                    if (!bCheckData())  //判断实际SN 数量 和计划数量的关系
                    {
                        tbSnFrom.Text = ""; tbSnTo.Text = "";
                        return;
                    }
                    

                    if (!bSnCheckAndAssign())  //SN 验证和分配
                    {
                        tbSnFrom.Text = ""; tbSnTo.Text = "";
                        return;
                    }


                    DialogResult dlg = MessageBox.Show("确认发出：" + ll_actual_out_qty + "条防伪标签吗吗？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dlg == DialogResult.Cancel)
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "用户取消操作");
                        tbSnFrom.Text = ""; tbSnTo.Text = "";
                        return;
                    }

                    if (bInsertData())
                    {
                        SecuGlobal.showOK(panelStatus, lblStatus, "写入数据库OK，可以点击查询按钮刷新数据");
                        XtraMessageBox.Show("写入数据库OK，可以点击查询按钮刷新数据");
                        tbSnFrom.Text = ""; tbSnTo.Text = "";
                    }
                    else
                    {
                        //SecuGlobal.showOK(panelStatus, lblStatus, "发生异常，数据写入数据库失败，请联系管理员");
                        tbSnFrom.Text = ""; tbSnTo.Text = "";
                        return;
                    }
                    
                    //需要刷新GRID1 数据  - 如果条件没有选，则可能导致数据显示特别慢

                    //SecuGlobal.GridViewInitial(gridView1, gridControl1);
                    //showData_gridView1();  //显示计划信息

                    SecuGlobal.showOK(panelStatus, lblStatus, "OK，请重点确认数据信息");

                });

            }
            catch (Exception ex)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, ex.Message);
            }

        }




        /// <summary>
        /// 数据上传DB
        /// </summary>
        /// <returns></returns>
        private bool bInsertData()
        {
            try
            {
                for (int i = 0; i < gridView2.RowCount; i++)
                {
                    string SecSn = gridView2.GetRowCellValue(i, gridView2.Columns["Sec Sn"]).ToString();
                    string Qty = gridView2.GetRowCellValue(i, gridView2.Columns["Qty"]).ToString();
                    string SecStart = gridView2.GetRowCellValue(i, gridView2.Columns["Sec Start"]).ToString();
                    string SecEnd = gridView2.GetRowCellValue(i, gridView2.Columns["Sec End"]).ToString();
                    string Line = gridView2.GetRowCellValue(i, gridView2.Columns["Line"]).ToString();
                    string PreWorkOrder = gridView2.GetRowCellValue(i, gridView2.Columns["Pre Work Order"]).ToString();
                    string WorkOrder = gridView2.GetRowCellValue(i, gridView2.Columns["Work Order"]).ToString();
                    string ExcpOut = gridView2.GetRowCellValue(i, gridView2.Columns["Excp Out"]).ToString();
                    string ExcpReason = gridView2.GetRowCellValue(i, gridView2.Columns["Excp Reason"]).ToString();
                    string SecItemCd = gridView2.GetRowCellValue(i, gridView2.Columns["Sec ItemCd"]).ToString();
                    string Vendor = gridView2.GetRowCellValue(i, gridView2.Columns["Vendor"]).ToString();
                    string SecSign = gridView2.GetRowCellValue(i, gridView2.Columns["Sec Sign"]).ToString();

                    string sqlInset = " insert into " + SecuGlobal.tbPopSecuIn + "  " +
                    " (vendor,sec_sn,sec_cdate,sec_ctime,sec_itemcd,sec_cnt,sec_start,sec_end,sec_sign,sec_calc_sum,store2_in," +
                    " sec_buyer,line,PRE_WORK_ORDER,WORK_ORDER,BRAND,EXCP_OUT,EXCP_REASON,FCT_CODE) " +
                    " values " +
                    " ( " +
                    " '" + Vendor + "','" + SecSn + "',to_char(sysdate,'yyyymmdd'),to_char(sysdate,'HH24Miss'), " +
                    " '" + SecItemCd + "','" + Qty + "','" + SecStart + "','" + SecEnd + "', " +
                    " '" + SecSign + "','" + Qty + "','" + Qty + "','SEC','" + Line + "' ,'" + PreWorkOrder + "'," +
                    "'" + WorkOrder + "','" + is_brand + "','" + ExcpOut + "','" + ExcpReason + "','" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                    " ) ";

                   OracleHelper.ExecuteNonQuery(sqlInset);
                }

                for (int i = 0; i < gridView3.RowCount; i++)
                {
                    string SerialNo = gridView3.GetRowCellValue(i, gridView3.Columns["SerialNo"]).ToString();
                    string Vendor = gridView3.GetRowCellValue(i, gridView3.Columns["Vendor"]).ToString();
                    int SecSn =System.Convert.ToInt32 (gridView3.GetRowCellValue(i, gridView3.Columns["Sec Sn"]).ToString());
                    string AddDateTime = gridView3.GetRowCellValue(i, gridView3.Columns["Add DateTime"]).ToString();
                    string WorkOrder = gridView3.GetRowCellValue(i, gridView3.Columns["Work Order"]).ToString();
                    string Shop = gridView3.GetRowCellValue(i, gridView3.Columns["Shop"]).ToString();

                    if (string.IsNullOrEmpty(Vendor) || string.IsNullOrEmpty("Sec Sn"))
                        break;

                    string sql = " insert into " + SecuGlobal.tbSnSecuHist + " (serial_no,vendor,sec_sn,add_datetime,work_order,shop,FCT_CODE) values" +
                        "('" + SerialNo + "','" + Vendor + "','" + SecSn + "',to_char(sysdate,'yyyymmddhh24miss'),'" + WorkOrder + "','" + Shop + "','" + PaCSGlobal.LoginUserInfo .Fct_code  + "') ";
                   OracleHelper.ExecuteNonQuery(sql);


                    string sqlUpdate = "update " + SecuGlobal.tbSecurityInSnTest + " set status = 'INPUT', shop = '" + Shop + "', " +
                                     "assign_wo = '" + WorkOrder + "' where serial_no = '" + SerialNo + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                   OracleHelper.ExecuteNonQuery(sqlUpdate);
                }

                return true;
            }
            catch (Exception bInsertData)
            {
                XtraMessageBox.Show("bInsertData" + bInsertData.Message);
                return false;
            }
        }





        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }




        /// <summary>
        /// //判断实际SN 数量 和计划数量的关系
        /// </summary>
        /// <returns></returns>
        private bool bCheckData()
        {
            try
            {
                int ll_out_qty = selectPlanQty();         // 选中记录的所有计划数量总和
                if (listSN.Count >= ll_out_qty)
                {
                    ll_actual_out_qty = ll_out_qty;       //实际SN数量多于计划数量，则按计划数量分配
                }
                else
                {
                    ll_actual_out_qty = listSN.Count;
                }

                if (listSN.Count < ll_out_qty)  //实际SN数量少于计划数量，只能选取一条计划进行发料
                {
                    if (iSelectedCounts > 1)
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "扫描序列号数量比计划数小时，只能选择一条计划进行发料!");
                        return false ;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }





        /// <summary>
        /// 检查SN信息并且分配
        /// </summary>
        /// <returns></returns>
        private bool  bSnCheckAndAssign()
        {
            string  ls_work_order, ls_shop, ls_sec_start, ls_sec_end,ls_pre_work_order;
            int ll_plan_qty, ll_assign_qty;
            int ll_cumulative_qty = 0;   //累计数量
            int ll_sn_start, ll_sn_end;
            long ll_sec_sn;

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                for (int j = 0; j < gridView1.Columns.Count; j++)
                {
                    if ((bool)gridView1.GetRowCellValue(i, gridView1.Columns[0]))
                    {
                        ls_work_order = gridView1.GetRowCellValue(i, gridView1.Columns["Work Order"]).ToString();  //包装计划
                        ls_shop = gridView1.GetRowCellValue(i, gridView1.Columns["Shop"]).ToString();
                        ll_plan_qty = System.Convert.ToInt32(gridView1.GetRowCellValue(i, gridView1.Columns["Plan Qty"])); 
                        ls_pre_work_order = gridView1.GetRowCellValue(i, gridView1.Columns["Pre Work Order"]).ToString(); //产线计划

                        if (ls_excp_out.Equals("Y") || listSN.Count < ll_plan_qty) // 如果是例外出库，或者SN list数量小于计划数量
                            ll_assign_qty = listSN.Count;
                        else
                            ll_assign_qty = ll_plan_qty;

                        ll_sn_start = ll_cumulative_qty ;              //INDEX
                        ll_sn_end = ll_cumulative_qty + ll_assign_qty; //
                        ls_sec_start = listSN[ll_sn_start];                 //起始SN LIST index 从0开始
                        ls_sec_end = listSN[ll_sn_end-1];                    //终止SN

                        ll_sec_sn = System.Convert.ToInt64 (OracleHelper.ExecuteScalar("select seq_secu_id.nextval from dual"));  //获取序号

                        object[] value = { ls_pre_work_order,ll_assign_qty, ls_sec_start, ls_sec_end, is_secu_code ,ls_shop,
                                             ls_work_order,ls_excp_out,ls_excp_reason ,PaCSGlobal.LoginUserInfo.Name ,vendorCode ,ll_sec_sn,
                                             "//","//" ,"SEC",ll_assign_qty,ll_assign_qty,is_brand  };
                        addGridView2(value);  //添加到GridView2 中

                        string ls_dw3_serial_no = "";
                        for (int a = ll_sn_start; a < ll_sn_end; a++)   //验证和分配SN 信息
                        {
                            ls_dw3_serial_no = gridView3.GetRowCellValue(a, gridView3.Columns["SerialNo"]).ToString(); //获取序列号信息
                            string sql = "select count(*) from " + SecuGlobal.tbSecurityInSnTest  + " where serial_no = '" + ls_dw3_serial_no + "' and status = 'OUT' " +
                                         "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                            int iC = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql));
                            if (iC.Equals(0))
                            {
                                SecuGlobal.showNG(panelStatus, lblStatus, "已出库或不存在的序列号!");
                                return false; ;
                            }

                            gridView3.SetRowCellValue(a, gridView3.Columns["SerialNo"], ls_dw3_serial_no);
                            gridView3.SetRowCellValue(a, gridView3.Columns["Vendor"],vendorCode);
                            gridView3.SetRowCellValue(a, gridView3.Columns["Sec Sn"], ll_sec_sn );
                            gridView3.SetRowCellValue(a, gridView3.Columns["Add DateTime"], "//");
                            gridView3.SetRowCellValue(a, gridView3.Columns["Work Order"], ls_work_order);
                            gridView3.SetRowCellValue(a, gridView3.Columns["Shop"], ls_shop);
                            gridView3.MoveNext();

                        }
                        ll_cumulative_qty = ll_cumulative_qty + ll_plan_qty;
                        break; //一行信息处理完成，跳出
                    }
                }
            }
            showGrid();
            return true;
        }



        private void showGrid()
        {
            gridView3.BestFitColumns();
            gridView3.Columns["SerialNo"].Width = 120;
            gridView3.Columns["SerialNo"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView3.Columns["SerialNo"].SummaryItem.DisplayFormat = "All： {0:f0} ";


            gridView2.BestFitColumns();
            gridView2.Columns["Qty"].Width = 120;
            gridView2.Columns["Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
            gridView2.Columns["Qty"].SummaryItem.DisplayFormat = "Sum： {0:f0} ";
        }




        /// <summary>
        /// 添加数据到GridView2
        /// </summary>
        private void addGridView2( object  [] value)
        {
            DataTable table = gridControl2.DataSource as DataTable;

            if (table == null)
            {
                DataTable dt = new DataTable();
                string[] colName =  { "Pre Work Order", "Qty", "Sec Start", "Sec End", "Sec ItemCd", "Line","Work Order","Excp Out","Excp Reason",
                        "Sec Sign","Vendor","Sec Sn","Sec Cdate","Sec Ctime" ,"Sec Buyer","Sec Calc Sum",
                        "Store2 In","Brand"};

                for (int i = 0; i < colName.Length; i++)
                {
                    dt.Columns.Add(new DataColumn(colName[i], typeof(string)));
                }

                dt.Rows.Add(value);
                gridControl2.DataSource = dt;
            }
            else
            {
                table.Rows.Add(value);
                gridControl2.DataSource = table;
            }
        }







        /// <summary>
        /// 添加数据到GridView3
        /// </summary>
        private void addGridView3(object[] value)
        {
            DataTable table = gridControl3.DataSource as DataTable;

            if (table == null)
            {
                DataTable dt = new DataTable();
                string[] colName = { "SerialNo", "Vendor", "Sec Sn", "Add DateTime", "Work Order", "Shop" };

                for (int i = 0; i < colName.Length; i++)
                {
                    dt.Columns.Add(new DataColumn(colName[i], typeof(string)));
                }

                dt.Rows.Add(value);
                gridControl3.DataSource = dt;
            }
            else
            {
                table.Rows.Add(value);
                gridControl3.DataSource = table;
            }
        }





        /// <summary>
        /// 导出到EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (gridView2.RowCount <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有记录！");
                return;
            }

            PaCSGlobal.ExportGridToFile(gridView2,"Sn Detail Info");
        }

        private void Debug_CheckedChanged(object sender, EventArgs e)
        {
            DialogResult dlg = MessageBox.Show("调试模式下运行？ 仅供开发人员使用", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dlg == DialogResult.OK)
            {
                if (Debug.Checked)
                {
                    cbVendor.Text = "BP3A:成宇电子";   // 测试模式
                    getVendorCode4();
                    SecuGlobal.getLineInfo(cbShop, vendorCode);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridView2.RowCount <= 0)
            {
                SecuGlobal.showNG(panelStatus,lblStatus ,"No Data");
                return;
            }

            try
            {

                string ls_sec_start, ls_sec_end, ls_vendor;
                int ll_sec_sn, ll_sec_cnt = 0;

                ls_sec_start = this.gridView2.GetRowCellValue(this.gridView2.FocusedRowHandle, this.gridView2.Columns["Sec Start"]).ToString();
                ls_sec_end = this.gridView2.GetRowCellValue(this.gridView2.FocusedRowHandle, this.gridView2.Columns["Sec End"]).ToString();
                ls_vendor = this.gridView2.GetRowCellValue(this.gridView2.FocusedRowHandle, this.gridView2.Columns["Vendor"]).ToString();
                ll_sec_sn = System.Convert.ToInt32(this.gridView2.GetRowCellValue(this.gridView2.FocusedRowHandle, this.gridView2.Columns["Sec Sn"]).ToString());
                ll_sec_cnt = System.Convert.ToInt32(this.gridView2.GetRowCellValue(this.gridView2.FocusedRowHandle, this.gridView2.Columns["Qty"]).ToString());


                DialogResult dlg = MessageBox.Show("确认删除" + ls_sec_start + "-" + ls_sec_end + "区间的" + ll_sec_cnt.ToString() + "条发料记录吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dlg == DialogResult.Cancel)
                {
                    SecuGlobal.showOK(panelStatus, lblStatus, "用户取消");
                    return;
                }

                int ll_cnt = 0;
                string sql = "";

                sql = " select count(*) " +
                        "   from " + SecuGlobal.tbSecurityInSnTest + " a " +
                        "  where serial_no in (select serial_no from " + SecuGlobal.tbSnSecuHist + " b where vendor = '" + ls_vendor + "' and sec_sn = '" + ll_sec_sn + "') " +
                        "    and status = 'INPUT' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";

                ll_cnt = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql));
                if (ll_cnt != ll_sec_cnt)
                {
                    SecuGlobal.showOK(panelStatus, lblStatus, "区间内序列号已扫描，不允许删除");
                    return;
                }

                sql = " update " + SecuGlobal.tbSecurityInSnTest + " a " +
                        "         set status = 'OUT', " +
                        "                  shop = null, " +
                        "                  assign_wo = null " +
                        "  where serial_no in (select serial_no from " + SecuGlobal.tbSnSecuHist + " b where vendor = '" + ls_vendor + "' and sec_sn = '" + ll_sec_sn + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') ";
                OracleHelper.ExecuteNonQuery(sql);

                sql = "delete from " + SecuGlobal.tbSnSecuHist + " b  where vendor = '" + ls_vendor + "'  and sec_sn = '" + ll_sec_sn + "'";
                OracleHelper.ExecuteNonQuery(sql);

                sql = "delete  from pop_secu_in b  where vendor = '" + ls_vendor + "'  and sec_sn = '" + ll_sec_sn + "'";
                OracleHelper.ExecuteNonQuery(sql);

                SecuGlobal.showOK(panelStatus, lblStatus, "删除成功");

            }
            catch (Exception ex)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "删除失败" + ex.Message );
            }




        }





    }
}