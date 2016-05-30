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
using System.IO.Ports;
using System.Data.OracleClient;
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;

namespace SecuLabel
{
    public partial class IssueAssy : DevExpress.XtraEditors.XtraForm
    {
        private SerialPort[] ports = new SerialPort[1];
        private delegate void InvokeDelegate(string data);
        private string receivedata = "";

        string vendorCode = "", sDate = "", eDate = "", plant = "", status = "",doc_no = "";

        string ls_item = "";
        int ll_scan_qty = 0, ll_gi_qty = 0, li_num = 0;
        int ll_req_qty = 0;
        string doc_no_f = "", doc_seq_f = "";
        string ls_req_vendor = "", ls_req_dept = "";
        string ls_sn_from = "";
        string ls_sn_to = "";

        public IssueAssy()
        {
            InitializeComponent();
            ports[0] = new SerialPort();
        }




        /// <summary>
        /// 获取四位VENDOR CODE 
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
        /// 加载基本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssueAssy_Load(object sender, EventArgs e)
        {

            SecuGlobal.setDate(dateEditFrom, dateEditTo);
            PaCSGlobal.InitComPort("SecuLabel", "IssueAssy", ports);

            if (ports[0].IsOpen)
                ports[0].DataReceived += new SerialDataReceivedEventHandler(serialPortSecuIssueAssy_DataReceived);//重新绑定

            if (PaCSGlobal.LoginUserInfo.Fct_code .Equals("C660A"))
                cbPlant.Text = "SSDP";
            else
                cbPlant.Text = "SESC";

            
            SecuGlobal.setAllVendorInfo(PaCSGlobal.LoginUserInfo.Fct_code, cbVendor);  //确认发料担当需要给所有厂家发料，
            cbVendor.Properties.Items.Add("ALL");
            //string bufVend = SecuGlobal.getPopVendorInfo(PaCSGlobal.LoginUserInfo.Venderid, PaCSGlobal.LoginUserInfo.Fct_code);
            //if (!bufVend.Equals(""))
            //{
            //    cbVendor.Text = bufVend;
            //}
            //else
            //{
            //    cbVendor.Text = PaCSGlobal.LoginUserInfo.Venderid + ":" + PaCSGlobal.LoginUserInfo.Vendername;  //苏州法人
            //}

            getVendorCode4(); //获取四位VENDOR CODE


            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");

        }







        /// <summary>
        /// 申请单信息获取后，双击：
        /// 根据单号查询的详细申请内容
        /// 一个单号下可能多条申请记录
        /// 如果没有条码，发料数等于申请数
        /// 有条形码默认是0
        /// </summary>
        /// <param name="doc_no"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        private string getSql2(string doc_no, string plant)
        {
            string sql = " SELECT FCT_CODE,REQ_DOC,  " +
                            "        REQ_SEQ,  " +
                            "        MATERIAL_CODE,  " +
                            "        (select b.description from " + SecuGlobal.tb_fpp_itemmaster  + " b where a.material_code = b.matnr) description, " +
                            "        (select board_count from " + SecuGlobal.tbMaster  + " c where c.material_code = a.material_code and c.FCT_CODE = a.FCT_CODE) board_count, " +
                            "        REQ_QTY,  " +
                            "        PROD_PLAN_QTY, " +
                            "        case when barcode_flag = 'N' then " +
                            "             nvl(actual_send_qty,req_qty) " +
                            "        else " +
                            "             (select nvl(sum(qty),0) from " + SecuGlobal.tbSecurityOut  + " b where a.req_doc = b.req_doc and a.req_seq = b.req_seq and b.FCT_CODE = a.FCT_CODE) " +
                            "        end as actual_send_qty, " +
                            "        STATUS,  " +
                            "        GR_QTY,  " +
                            "        BARCODE_FLAG, " +
                            "        PLANT " +
                            " FROM " + SecuGlobal.tbSecurityRequestD  + " a " +
                            " WHERE a.REQ_DOC = '" + doc_no + "' " +
                            " and a.plant like '" + plant + "' " +
                            " AND a.STATUS <> 'DE' and a.FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                            " ORDER BY REQ_SEQ ";
            return sql;
        }






        /// <summary>
        /// 申请单中，如果条形码是Y 的,
        /// 显示的明细信息
        /// </summary>
        /// <param name="doc_seq"></param>
        /// <param name="seq_seq"></param>
        /// <returns></returns>
        private string getSql3(string doc_seq, string seq_seq)
        {
            string sql = " select fct_code, security_start,  " +
                        "        security_end,  " +
                        "        req_doc,  " +
                        "        req_seq,  " +
                        "        scan_data,  " +
                        "        gi_date,  " +
                        "        gi_time,  " +
                        "        gi_user,  " +
                        "        gi_ip " +
                        "   from " + SecuGlobal.tbSecurityOut  + " " +
                        "  where req_doc = '" + doc_seq + "' " +
                        "    and req_seq = '" + seq_seq + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
            return sql;
        }





        /// <summary>
        /// 查询后，获取申请单信息
        /// <param name="vendor"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="plant"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private string getSql(string vendor, string sDate, string eDate, string plant, string status)
        {
            string sql = " SELECT FCT_CODE,REQ_DOC,  " +
                            "        REQ_VENDOR,  " +
                            "        REQ_USER,  " +
                            "        REQ_DATE,  " +
                            "        PROD_PLAN_DATE,  " +
                            "        REMARK, " +
                            "        PLANT, " +
                            "        REQ_DEPT " +
                            " FROM " + SecuGlobal.tbSecurityRequestH  + " H " +
                            " WHERE REQ_VENDOR like '" + vendor + "' " +
                            "   AND REQ_DATE BETWEEN '" + sDate + "' AND '" + eDate + "' " +
                            "   AND (('" + status + "' = '1' AND EXISTS (SELECT 1 FROM " + SecuGlobal.tbSecurityRequestD  + " D  " +
                            "                                      WHERE H.REQ_DOC = D.REQ_DOC  and D.FCT_CODE = H.FCT_CODE" +
                            "                                        AND D.STATUS = 'RQ')) OR " +
                            "        ('" + status + "' = '2' AND NOT EXISTS (SELECT 1 FROM " + SecuGlobal.tbSecurityRequestD + " D  " +
                            "                                          WHERE H.REQ_DOC = D.REQ_DOC  and D.FCT_CODE = H.FCT_CODE " +
                            "                                            AND D.STATUS = 'RQ')) OR " +
                            "        '" + status + "' = '3' " +
                            "       ) " +
                            "   AND EXISTS (SELECT 1 FROM " + SecuGlobal.tbSecurityRequestD + " D " +
                            "                WHERE H.REQ_DOC = D.REQ_DOC  and D.FCT_CODE = H.FCT_CODE " +
                            "                  AND D.STATUS <> 'DE' " +
                            "              ) " +
                            " and h.plant like '" + plant + "' and h.FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                            " ORDER BY REQ_DOC DESC ";
            return sql;
        }




        /// <summary>
        /// 改变DataTable 标题栏及排列顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader(DataTable dt)
        {

            string[] colField = { "FCT_CODE","PLANT", "REQ_DOC", "REQ_VENDOR", "REQ_DEPT", "REQ_USER", "REQ_DATE", 
                                    "PROD_PLAN_DATE","REMARK" };

            string[] colName = { "Fct Code","Plant", "申请单号", "申请厂家", "申请部门", "申请人", "申请日期", 
                                    "生产计划日期","备注" };


            //int[] showIndex = { 0, 1, 2, 3, 4, 5, 6, 7 };

            for (int i = 0; i < colField.Length ; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);

            }

            return dt;

        }




        private void getVariable()
        {
            vendorCode = cbVendor.Text.Trim();    // Vendor Code 模糊查询《BP3A%》

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


            plant = cbPlant.Text.Trim();    // Plant Code 模糊查询《SSDP%》
            if (!string.IsNullOrEmpty(plant))
            {
                if (!plant.Equals("ALL"))
                    plant = plant + "%";
                else
                    plant = "%";
            }



            if (cbStatus.SelectedIndex != -1)
                status = cbStatus.Properties.Items[cbStatus.SelectedIndex].ToString();

            if (!string.IsNullOrEmpty(status))
            {
                status = status.Substring(0, 1);
            }

            sDate = dateEditFrom.Text.Trim().Replace("-", "");
            eDate = dateEditTo.Text.Trim().Replace("-", "");

        }




        /// <summary>
        /// 点击查询按钮之后，后台查询数据
        /// 查询中，不允许打印出门证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                
                getVariable();  //获取变量信息

                this.Invoke((MethodInvoker)delegate
                {

                    SecuGlobal.GridViewInitial(gridView1, gridControl2);  //清空GRIDVIEW
                    SecuGlobal.GridViewInitial(gridView2, gridControl1);
                    SecuGlobal.GridViewInitial(gridView3, gridControl3);
                    btnPrint.Enabled = false;
                    DataTable dt = null;
                    dt = OracleHelper.ExecuteDataTable(getSql(vendorCode, sDate, eDate, plant, status));  //获取申请单信息，vendorCode 、plant 都是模糊查询
                    if (dt == null)
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "申请单信息-NULL");
                        return;
                    }

                    dt = setDtHeader(dt); //更改标题栏和显示顺序

                    gridControl2.DataSource = dt;
                    gridView1.BestFitColumns();

                    gridView1.Columns["申请单号"].SummaryItem.SummaryType = SummaryItemType.Count;
                    gridView1.Columns["申请单号"].SummaryItem.DisplayFormat = "All:{0:f0} ";


                    btnPrint.Enabled = true;
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                });
            }
            catch (Exception err)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, err.Message);
            }
        }





        /// <summary>
        /// 点击按钮查询数据
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

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }






        /// <summary>
        /// 改变标题栏和显示顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader1(DataTable dt)
        {

            string[] colField = { "FCT_CODE","PLANT", "REQ_DOC", "REQ_SEQ", "MATERIAL_CODE", "REQ_QTY", "actual_send_qty", 
                                    "board_count","GR_QTY","PROD_PLAN_QTY" ,"description","STATUS","BARCODE_FLAG"};

            string[] colName = { "Fct Code","Plant", "申请单号", "序号", "材料", "申请数量", "实际发料数", 
                                    "单位","入库数量","计划数量","材料描述","状态","条形码" };

            //int[] showIndex = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            for (int i = 0; i < colField.Length ; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);
            }
            return dt;
        }







        /// <summary>
        /// 申请单号删除
        /// 需要判断状态是否可以删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {

            try
            {
                if(gridView2.RowCount <= 0)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "没有可删除的数据!");
                    return;
                }

                bool b = false;
                for (int i = 0; i < gridView2.RowCount; i++)
                {
                    if ((bool)gridView2.GetRowCellValue(i, gridView2.Columns[0]))
                    {
                        b = true;
                        break;
                    }
                }
                if (!b)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请选择要删除的订单内容");
                    return;
                }

                DialogResult dlg = MessageBox.Show("你确信要删除申请单：" + doc_no + "吗？", "删除提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dlg == DialogResult.OK)
                {
                    for (int i = 0; i < gridView2.RowCount; i++)
                    {
                        if ((bool)gridView2.GetRowCellValue(i, gridView2.Columns[0]))
                        {
                          string bufDocNo = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["申请单号"]).ToString();
                          string bufSeq = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["序号"]).ToString();
                          string status = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["状态"]).ToString();
                          string sql = " update " + SecuGlobal.tbSecurityRequestD + " " +
                                        "          set status = 'DE', " +
                                        "              update_date = to_char(sysdate,'yyyymmdd'), " +
                                        "              update_time = to_char(sysdate,'HH24miss'), " +
                                        "              update_user = '" + PaCSGlobal.LoginUserInfo.Name + "', " +
                                        "              update_ip = '" + PaCSGlobal.GetClientIp() + "' " +
                                        "        where req_doc = '" + doc_no + "' and req_seq = '" + bufSeq + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";          
                          OracleHelper.ExecuteNonQuery(sql);

                        }
                    }
                    btnApply_Click(sender, e);  //刷新数据
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                }
            }
            catch (Exception deleteError)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, deleteError.Message );
            }
        }




        private void girdView2DataLoading(DataTable dt)
        {
            gridControl1.DataSource = dt;
            gridView2.BestFitColumns();
            gridView2.Columns["申请单号"].Width = 140;
            gridView2.Columns["申请单号"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView2.Columns["申请单号"].SummaryItem.DisplayFormat = "All:{0:f0} ";
            gridView2.Columns["材料"].Width = 100;

            GridCheckMarksSelection selection = new GridCheckMarksSelection(gridView2); // 增加CHECKBOX 
            selection.CheckMarkColumn.VisibleIndex = 0;
            selection.SelectionChanged += selection_SelectionChanged;

            gridView2.Columns["Fct Code"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["Plant"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["申请单号"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["序号"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["材料"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["单位"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["入库数量"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["计划数量"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["材料描述"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["状态"].OptionsColumn.ReadOnly = true;
            gridView2.Columns["条形码"].OptionsColumn.ReadOnly = true;
        }





        private void selection_SelectionChanged(object sender, EventArgs e)
        {
            //int rowHandle = gridView2.FocusedRowHandle;
            //for (int i = 0; i < gridView2.RowCount; i++)
            //{
            //    if ((bool)gridView2.GetRowCellValue(i, gridView2.Columns[0]))
            //    {
            //        if(i != rowHandle)
            //        {
            //            gridView2.SetRowCellValue(i, gridView2.Columns[0], false);
            //        }
            //    }
            //}
        }






        /// <summary>
        /// 行号显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 扫描枪设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScanSetting_Click(object sender, EventArgs e)
        {
            SettingForm setcom = new SettingForm("SecuLabel", "IssueAssy", 1);
            DialogResult dg = setcom.ShowDialog();

            if (dg == DialogResult.OK)
            {
                PaCSGlobal.InitComPort("SecuLabel", "IssueAssy", ports);

                if (ports[0].IsOpen)
                    ports[0].DataReceived += new SerialDataReceivedEventHandler(serialPortSecuIssueAssy_DataReceived);//重新绑定
            }
        }



        /// <summary>
        /// Com 口数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPortSecuIssueAssy_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                System.Threading.Thread.Sleep(100); //读取速度慢，加Sleep延长读取时间, 不可缺少
                //serialPort1.DiscardInBuffer();  //如果不执行上面的代码,serialPort1_DataReceived会执行多次

                int n = ports[0].BytesToRead;
                byte[] buf = new byte[n];
                ports[0].Read(buf, 0, n);
                //ports[0].ReceivedBytesThreshold = 31;

                receivedata = System.Text.Encoding.ASCII.GetString(buf);
                receivedata = receivedata.Replace("\r\n", "");

                try
                {
                    this.Invoke(new InvokeDelegate(DoData), receivedata);
                }
                catch (Exception serialPortSecuIssueAssy_DataReceived)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        ////要委托的代码 
                        //lbStatus.Text = "【" + receivedata + "】：出库失败";
                        //panelStatus.BackColor = Color.Red;
                        XtraMessageBox.Show(this, "System error[serialPortSecuIssueAssy_DataReceived]: " + serialPortSecuIssueAssy_DataReceived.Message);
                    }));
                }
            }
        }



        /// <summary>
        /// 从Com口获取的数据进行相应操作
        /// </summary>
        /// <param name="data"></param>
        private void DoData(string data)
        {
            tbSn.Text = data;
            //MessageBox.Show(data);
            doWork(tbSn.Text.Trim());  // 读取的信息分析处理

        }




        /// <summary>
        /// USB 扫描枪读取SN后按回车触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSn_KeyPress(object sender, KeyPressEventArgs e)
        {

            if ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122)
            {
                e.KeyChar = (char)((int)e.KeyChar - 32);
            }

            if(e.KeyChar == (char)Keys.Enter)   
            {
                doWork(tbSn.Text.Trim());  // 读取的信息分析处理
            }
        }



        /// <summary>
        /// 大写控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void tbSnFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122)
            {
                e.KeyChar = (char)((int)e.KeyChar - 32);
            }
        }





        /// <summary>
        /// 大写控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSnTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122)
            {
                e.KeyChar = (char)((int)e.KeyChar - 32);
            }
        }




        /// <summary>
        /// 获取扫描枪数据，根据数据分析属于什么类型
        /// </summary>
        /// <param name="scanData"></param>
        /// <returns></returns>
        private string getScanDataType(string scanData)
        {
            string bufType = "";

            int li_req_cnt, li_unit_cnt, li_sn_cnt;
            //SECU201411170001
            li_req_cnt = System.Convert.ToInt32(OracleHelper.ExecuteScalar("select count(*) from " + SecuGlobal.tbSecurityRequestH  + " where req_doc = '" + scanData + "' " +
                                                                           " and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'"));

            //JC68-01673A 3310 26473 26480 3C7BECDND5 20,000 SECD4AH66180001 SECD4AH66200000
            li_unit_cnt = System.Convert.ToInt32(OracleHelper.ExecuteScalar("select count(*) from " + SecuGlobal.tbSecurityInTest  +" where (scan_data = '" + scanData + "' " +
                                                         "or scan_data_gene = '" + scanData + "') and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'"));

            //SECD4AH66180001
            li_sn_cnt = System.Convert.ToInt32(OracleHelper.ExecuteScalar("select count(*) from " + SecuGlobal.tbSecurityInSnTest  + " where serial_no = '" + scanData + "' " +
                                                        " and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'"));

            if (li_req_cnt >= 1)  // 扫描申请单号码，不用手工选择，方便操作
            {
                bufType = "DocNoType";
                SecuGlobal.showOK(panelStatus, lblStatus, "扫描了申请单号！");
            }
            else if (li_unit_cnt >= 1)  //扫描二维条形码 ，判定
            {
                bufType = "2DBarcodeType";
                SecuGlobal.showOK(panelStatus, lblStatus, "扫描了二维条形码！");
            }
            else if (li_sn_cnt >= 1)   //扫描序列号范围，判定
            {
                bufType = "SnRangeType";
                SecuGlobal.showOK(panelStatus, lblStatus, "扫描了序列号！");
            }
            else
            {
                bufType = "";
                SecuGlobal.showNG(panelStatus, lblStatus, "未知的扫描号码，请再次确认扫描内容！");
            }

            return bufType;
        }




        /// <summary>
        /// 扫描枪获取的数据进行分析处理
        /// </summary>
        /// <param name="scanData"></param>
        private void doWork(string scanData)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "正在处理数据,请稍等...");
                switch (getScanDataType(scanData))
                {
                    case "DocNoType":

                        SecuGlobal.GridViewInitial(gridView2, gridControl1);
                        DataTable dt = OracleHelper.ExecuteDataTable(getSql2(scanData, cbPlant.Text.Trim()));  // plant 查询实际上没有太大意义
                        if (dt != null)
                        {
                            dt = setDtHeader1(dt);
                        }
                        
                        girdView2DataLoading(dt); //加载数据到GridView2 中
                        SecuGlobal.showOK(panelStatus, lblStatus, "申请单数据查询OK");
                        break;
                    case "2DBarcodeType":
                        if (gridView2.RowCount <= 0)
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "请首先查询申请单信息！");
                            return;
                        }
                        processingData2D(scanData); //扫描二维条形码，拆分处理数据
                        break;
                    case "SnRangeType":
                        if (gridView2.RowCount <= 0)
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "请首先查询申请单信息！");
                            return;
                        }

                        if (string.IsNullOrEmpty(tbSnFrom.Text.Trim()))
                        {
                            SecuGlobal.showOK(panelStatus, lblStatus, "起始序列号读取成功");
                            tbSnFrom.Text = scanData;
                            ls_sn_from = tbSnFrom.Text.Trim();
                        }
                        else
                        {
                            if ((ls_sn_from.Length.Equals(15) && ls_sn_from.Substring(0, 7).Equals(scanData.Substring(0, 7))) ||
                                (ls_sn_from.Length.Equals(9)) && ls_sn_from.Substring(0, 2).Equals(scanData.Substring(0, 2)))
                            {
                                tbSnTo.Text = scanData;
                                ls_sn_to = tbSnTo.Text.Trim();
                                processingDataSnRange(ls_sn_from, ls_sn_to);  // SN范围获取成功，数据分析处理
                                tbSnFrom.Text = "";
                            }
                            else
                            {
                                SecuGlobal.showNG(panelStatus, lblStatus, "序列号前几位不一致，请重新扫描!");
                            }
                        }
                        
                        break;
                    default:
                        SecuGlobal.showNG(panelStatus, lblStatus, "错误的序列号- 获取类型失败");
                        break;
                }
            }
            catch (Exception doWorkError)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, doWorkError.Message);
            }
            finally
            {
                iniTextEdit();  //全局变量信息清空
            }
        }




        /// <summary>
        /// 扫描枪获取的2D 数据分析处理
        /// </summary>
        /// <param name="data"></param>
        private void processingData2D(string data)
        {
            try
            {
                if (!getItemScanQty2D(data))  //获取扫描的2D码在数据库中的总入库数量及对应的材料CODE ls_item 信息
                    return;

                if (!getCurrentAssy())  //遍历申请单明细，找到对应的申请单材料信息及对应的单号，申请数等
                    return;

                if (!getOutQty())       //获取出库数量 发货数+扫描数 必须等于 申请数
                    return;

                if (!getSnValue2D(data))  //判断扫描的SN 获取的数量是否和数据库中扫描入库的数量一致
                    return;

                if (!getVendorInfo())   //获取申请单的VENDOR 信息
                    return;

                if (!setShowOutQty())  //实际发料数显示到GridView中
                    return;

                if (!insertData_2D(data ))
                {
                    return;
                }
                SecuGlobal.showOK(panelStatus, lblStatus, "OK-请确认实际发料数");
            }
            catch (Exception err2d)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, err2d.Message );
            }
        }




        /// <summary>
        /// 通过扫描的SN 区间查询到数据库中对应的材料和扫描区间的数量
        /// </summary>
        /// <param name="ls_sn_from"></param>
        /// <param name="ls_sn_to"></param>
        /// <returns>true / false </returns>
        private bool 
            getItemScanQty2D(string data)
        {
            try
            {
                string sql = "select max(item) LS_ITEM ,sum(qty) LL_SCAN_QTY from " + SecuGlobal.tbSecurityInTest + " where (scan_data = '" + data + "' " +
                             "or scan_data_gene = '" + data + "') and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                OracleDataReader odr = OracleHelper.ExecuteReader(sql);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ls_item = odr["LS_ITEM"].ToString();
                        ll_scan_qty = System.Convert.ToInt32(odr["LL_SCAN_QTY"].ToString());   //根据扫描的SN范围获取 材料编号和扫描的SN 数量
                    }
                    return true;
                }
                else
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "获取扫描数量NG！");
                    return false;
                }
            }
            catch (Exception e)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, e.Message);
                return false;
            }

        }








        /// <summary>
        /// 通过扫描的SN 区间查询到数据库中对应的材料和扫描区间的数量
        /// </summary>
        /// <param name="ls_sn_from"></param>
        /// <param name="ls_sn_to"></param>
        /// <returns>true / false </returns>
        private bool  getItemScanQty(string ls_sn_from,string ls_sn_to)
        {
            try
            {
                if (ls_sn_from.Length.Equals(15))
                    li_num = 8;
                else
                    li_num = 3;

                string sql = " select max(item) ls_item, " +
                            "        substr('" + ls_sn_to + "','" + li_num + "') - substr('" + ls_sn_from + "','" + li_num + "') + 1 ll_scan_qty " +
                            "   from " + SecuGlobal.tbSecurityInTest   + " a " +
                            "  where exists (select 1 from " + SecuGlobal.tbSecurityInSnTest + " b where b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                            "  and a.doc_no = b.doc_no and " +
                            "  a.doc_seq = b.doc_seq and b.serial_no = '" + ls_sn_to + "') ";
                OracleDataReader odr = OracleHelper.ExecuteReader(sql);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ls_item = odr["LS_ITEM"].ToString();
                        ll_scan_qty = System.Convert.ToInt32(odr["LL_SCAN_QTY"].ToString());   //根据扫描的SN范围获取 材料编号和扫描的SN 数量
                    }
                    return true;
                }
                else
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "获取扫描数量NG！");
                    return false;
                }
            }
            catch(Exception e)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, e.Message);
                return false;
            }

        }



        /// <summary>
        /// //遍历申请单信息，如果申请单中不存在对应的申请编号
        /// </summary>
        /// <returns>true / false </returns>
        private bool getCurrentAssy()
        {
            try 
            {
                int iFount = 0;
                for (int i = 0; i < gridView2.RowCount;i++ )
                {
                    string meterialCode = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["材料"]).ToString();
                    if (meterialCode.Equals(ls_item))
                    {
                        doc_no_f = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["申请单号"]).ToString();
                        doc_seq_f = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["序号"]).ToString();
                        ll_req_qty = System.Convert.ToInt32(this.gridView2.GetRowCellValue(i, this.gridView2.Columns["申请数量"]).ToString());  //申请的数量
                        iFount += 1;
                    }
                }

                if (!iFount.Equals(1))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "错误的材料编号：" + ls_item);
                    iniTextEdit();
                    return false ;
                }

                return true;
            }
            catch (Exception getCurrentAssy)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, getCurrentAssy.Message + ls_item);
                return false ;
            }
        }







        /// <summary>
        /// //查询出库的数量，如果出库数量 + 扫描的数量大于申请数量 
        /// </summary>
        /// <returns>true / false </returns>
        private bool getOutQty()
        {
            try
            {
                string sql1 = " select nvl(sum(qty),0) ll_gi_qty from " + SecuGlobal.tbSecurityOut + " where " +
                              "req_doc = '" + doc_no_f + "' and req_seq = '" + doc_seq_f + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                OracleDataReader odr1 = OracleHelper.ExecuteReader(sql1);
                if (odr1.HasRows)
                {
                    while (odr1.Read())
                    {
                        ll_gi_qty = System.Convert.ToInt32(odr1["ll_gi_qty"].ToString());
                    }
                }

                if ((ll_scan_qty + ll_gi_qty) > ll_req_qty)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "出库数超过申请数!");
                    return false;
                }
                return true;
            }
            catch (Exception )
            {
                return false; 
            }
        }



        /// <summary>
        ///  查询扫描的SN信息
        /// </summary>
        /// <returns>true / false </returns>
        private bool getSnValue2D(string data)
        {
            try
            {
                int ll_in_cnt = 0;
                string sql2 = "select count(*) ll_in_cnt from " + SecuGlobal.tbSecurityInSnTest + " a where exists " + 
                              "(select 1 from " + SecuGlobal.tbSecurityInTest  + " b where b.FCT_CODE = a.FCT_CODE and a.doc_no = b.doc_no and a.doc_seq = b.doc_seq and " +
                              "(b.scan_data = '" + data + "' or b.scan_data_gene = '" + data + "')) and a.status = 'IN' and a.FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                OracleDataReader odr2 = OracleHelper.ExecuteReader(sql2);
                if (odr2.HasRows)
                {
                    while (odr2.Read())
                    {
                        ll_in_cnt = System.Convert.ToInt32(odr2["ll_in_cnt"].ToString());  //2d 码截取的SN 数量信息
                    }
                }

                if (!ll_in_cnt.Equals(ll_scan_qty))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "扫描数量和在库数量不一致，请确认是否为已出库的产品！");
                    return false;
                }
                return true;
            }
            catch (Exception getSnValue2D)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, getSnValue2D.Message + "getSnValue2D");
                return false;
            }
        }





        /// <summary>
        ///  查询扫描的SN信息
        /// </summary>
        /// <returns>true / false </returns>
        private bool getSnValue(string ls_sn_from, string ls_sn_to)
        {
            try
            {
                int ll_in_cnt = 0;
                string sql2 = " select count(*) ll_in_cnt from " + SecuGlobal.tbSecurityInSnTest + " a where serial_no between '" + ls_sn_from + "' " +
                              "and '" + ls_sn_to + "' and status = 'IN' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code   + "'";
                OracleDataReader odr2 = OracleHelper.ExecuteReader(sql2);
                if (odr2.HasRows)
                {
                    while (odr2.Read())
                    {
                        ll_in_cnt = System.Convert.ToInt32(odr2["ll_in_cnt"].ToString());
                    }
                }

                if (!ll_in_cnt.Equals(ll_scan_qty))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "出库序列号数量错误，请联系管理员!");
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        /// <summary>
        /// 获取申请者基本信息
        /// </summary>
        /// <returns></returns>
        private bool getVendorInfo()
        {
            try
            {
                string sql3 = "select req_vendor,nvl(req_dept,'-') req_dept from " + SecuGlobal.tbSecurityRequestH + " where req_doc = '" + doc_no_f + "' " +
                              " and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                OracleDataReader odr3 = OracleHelper.ExecuteReader(sql3);
                if (odr3.HasRows)
                {
                    while (odr3.Read())
                    {
                        ls_req_vendor = odr3["req_vendor"].ToString();
                        ls_req_dept = odr3["req_dept"].ToString();
                    }
                }

                if (!ls_req_vendor.Equals("") && !ls_req_dept.Equals(""))
                    return true;
                else
                {
                    SecuGlobal.showOK(panelStatus, lblStatus, "获取Vendor信息失败，请联系管理员");
                    return false;
                }
                    
            }
            catch (Exception getVendorInfo)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, getVendorInfo.Message );
                return false;
            }
        }



        /// <summary>
        /// //扫描数量显示到GRIDVIEW 实际发料数中
        /// </summary>
        /// <returns></returns>
        private bool setShowOutQty()
        {
            try
            {
                int count = 0;
                string bufAssyCode = "";
                string bufSeq = "";
                for (int i = 0; i < gridView2.RowCount; i++)
                {
                    bufAssyCode = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["材料"]).ToString();
                    bufSeq = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["序号"]).ToString();

                    if (bufSeq.Equals(doc_seq_f) && bufAssyCode.Equals(ls_item))
                    {
                        this.gridView2.SetRowCellValue(i, this.gridView2.Columns["实际发料数"], ll_scan_qty + ll_gi_qty);
                        count += 1;
                    }
                }

                if (count.Equals(1))
                    return true;
                else
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "实际发料数显示错误，请联系管理员");
                    return false;
                }
                    
            }
            catch (Exception setShowOutQty)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, setShowOutQty.Message );
                return false;
            }
        }





        /// <summary>
        /// 材料出库，数据更新
        /// </summary>
        /// <param name="sndata"></param>
        /// <returns></returns>
        private bool insertData_2D(string sndata)
        {
            try
            {
                string sqlInsert =  "  insert " +
                                    "  into " + SecuGlobal.tbSecurityOut  + " " +
                                    "       ( " +
                                    "        security_start,security_end,qty, " +
                                    "        req_doc,req_seq,scan_data,gi_date,  " +
                                    "        gi_time,gi_user,gi_ip,FCT_CODE " +
                                    "       ) " +
                                    "  select security_start,security_end,qty, " +
                                    "       :ls_req_doc,:li_req_seq,:ls_scan_data,to_char(sysdate,'yyyymmdd'), " +
                                    "       to_char(sysdate,'hh24miss'),:Gs_Userid,:Gs_ssdpip,FCT_CODE " +
                                    "  from " + SecuGlobal.tbSecurityInTest  + " " +
                                    "  where scan_data = :ls_scan_data or scan_data_gene = :ls_scan_data ";
                OracleParameter[] cmdParam = new OracleParameter[] {                
                    new OracleParameter(":ls_req_doc", OracleType.VarChar),
                    new OracleParameter(":li_req_seq", OracleType.VarChar),
                    new OracleParameter(":ls_scan_data", OracleType.VarChar),
                    new OracleParameter(":Gs_Userid", OracleType.VarChar),
                    new OracleParameter(":Gs_ssdpip", OracleType.VarChar)
                    };
                cmdParam[0].Value = doc_no_f ;
                cmdParam[1].Value = doc_seq_f ;
                cmdParam[2].Value = sndata;
                cmdParam[3].Value = PaCSTools.PaCSGlobal.LoginUserInfo.Name;
                cmdParam[4].Value = PaCSTools.PaCSGlobal.GetClientIp();
                OracleHelper.ExecuteNonQuery(sqlInsert, cmdParam);

                string sqlUpdate = "update " + SecuGlobal.tbSecurityInSnTest + " a set status = 'OUT', " +
                            "vendor = '" + ls_req_vendor + "', dept = '" + ls_req_dept + "'  where exists " +
                            "(select 1 from " + SecuGlobal.tbSecurityInTest + " b where b.FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' and  a.doc_no = b.doc_no and a.doc_seq = b.doc_seq " +
                            "and (b.scan_data = '" + sndata + "' or b.scan_data_gene = '" + sndata + "'))";
  

                OracleHelper.ExecuteNonQuery(sqlUpdate);
                SecuGlobal.showOK(panelStatus, lblStatus, ll_scan_qty.ToString() + "个防伪标签出库成功,请继续");

                return true;
            }
            catch (Exception insertData)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "Insert Exception - : " + insertData.Message);
                return false;
            }
        }







        private bool insertData(string sndata)
        {
            try
            {
                string sqlInsert = "insert into " + SecuGlobal.tbSecurityOut  +
                    " (security_start,security_end,qty,req_doc,req_seq,scan_data,gi_date, gi_time,gi_user,gi_ip,FCT_CODE) " +
                    " VALUES(:ls_sn_from,:ls_sn_to,:ll_scan_qty,:ls_req_doc,:li_req_seq,:ls_scan_data," +
                    " to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24miss'),:Gs_Userid,:Gs_ssdpip,:FCT_CODE)";
                OracleParameter[] cmdParam = new OracleParameter[] {                
                    new OracleParameter(":ls_sn_from", OracleType.VarChar),
                    new OracleParameter(":ls_sn_to", OracleType.VarChar),
                    new OracleParameter(":ll_scan_qty", OracleType.Number),
                    new OracleParameter(":ls_req_doc", OracleType.VarChar),
                    new OracleParameter(":li_req_seq", OracleType.Number,6),
                    new OracleParameter(":ls_scan_data", OracleType.VarChar),
                    new OracleParameter(":Gs_Userid", OracleType.VarChar),
                    new OracleParameter(":Gs_ssdpip", OracleType.VarChar),
                    new OracleParameter(":FCT_CODE", OracleType.VarChar)
                    };
                cmdParam[0].Value = ls_sn_from;
                cmdParam[1].Value = ls_sn_to;
                cmdParam[2].Value = ll_scan_qty;
                cmdParam[3].Value = doc_no_f;
                cmdParam[4].Value = doc_seq_f;
                cmdParam[5].Value = sndata;
                cmdParam[6].Value = PaCSGlobal.LoginUserInfo.Name;
                cmdParam[7].Value = PaCSGlobal.GetClientIp();
                cmdParam[8].Value = PaCSGlobal.LoginUserInfo.Fct_code ;
                OracleHelper.ExecuteNonQuery(sqlInsert, cmdParam);

                 string    sqlUpdate = "update " + SecuGlobal.tbSecurityInSnTest  +" a set status = 'OUT', " +
                             " vendor = '" + ls_req_vendor + "', dept = '" + ls_req_dept + "' " +
                             " where serial_no between '" + ls_sn_from + "' and '" + ls_sn_to + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";

                OracleHelper.ExecuteNonQuery(sqlUpdate);

                SecuGlobal.showOK(panelStatus, lblStatus, ll_scan_qty.ToString() + "个防伪标签出库成功,请继续");

                return true ;
            }
            catch (Exception insertData)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, insertData.Message);
                return false;
            }
        }




        /// <summary>
        /// 根据SN范围进行数据处理
        /// </summary>
        /// <param name="data"></param>
        private void processingDataSnRange(string ls_sn_from, string ls_sn_to)
        {
            if (!getItemScanQty(ls_sn_from, ls_sn_to)) //根据SN区间，查询入库数量和材料编号
               return;

            if (!getCurrentAssy())   //遍历申请单名字，判断是否存在对应的材料
                return;

            if (!getOutQty())  //获取出库的数量。扫描数量+出库数量必须小于申请数量
                return;

            if (!getSnValue(ls_sn_from, ls_sn_to)) //获取材料入库的数量，如果扫描数量不等于入库数量等报错
                return;

            if (!getVendorInfo()) //获取VENDOR信息
                return;

            if (!setShowOutQty())  //出库成功，实际发料数显式
                return;

            if (!insertData(ls_sn_to))  
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "数据上传数据库失败，请确认SN 信息是否正确！");
                return;
            }
        }



        private void iniTextEdit()
        {
            //tbSnFrom.Text = "";
            tbSnTo.Text = "";
            tbSn.Text = "";
            ls_item = "";
            ll_scan_qty = 0; ll_gi_qty = 0; li_num = 0;
            ll_req_qty = 0;
            doc_no_f = ""; doc_seq_f = "";
            ls_req_vendor = ""; ls_req_dept = "";
        }


        /// <summary>
        /// 窗体关闭时关闭COM口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssueAssy_FormClosing(object sender, FormClosingEventArgs e)
        {


            try
            {
                ports[0].DataReceived -= new SerialDataReceivedEventHandler(serialPortSecuIssueAssy_DataReceived);//取消绑定

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
                SecuGlobal.showNG(panelStatus,lblStatus ,IssueAssy_FormClosing.ToString());
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIssueAssy_Click(object sender, EventArgs e)
        {
            try
            {
                btnIssueAssy.Enabled = false;
                SecuGlobal.showOK(panelStatus, lblStatus, "正在上传发料数据,请稍等...");
                if (gridView2.RowCount <= 0)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "没有申请单信息");
                    return;
                }

                DialogResult dr = MessageBox.Show("确认发料吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    if (bIssueAssy())  //确认发料，数据上传
                    {
                        SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                        btnApply_Click(sender, e);
                    }   
                }

            }
            catch (Exception btnIssueAssy_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnIssueAssy_Click.Message);
            }

        }




        /// <summary>
        /// 确认发料
        /// </summary>
        /// <returns></returns>
        private bool bIssueAssy()
        {
            try
            { 
                string bufDocNo, bufSeq, barcode;
                int reqQty = 0, sendQty = 0;
                for (int i = 0; i < gridView2.RowCount; i++)
                {
                    bufDocNo = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["申请单号"]).ToString();
                    bufSeq = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["序号"]).ToString();
                    barcode = this.gridView2.GetRowCellValue(i, this.gridView2.Columns["条形码"]).ToString();
                    reqQty = System.Convert.ToInt32(this.gridView2.GetRowCellValue(i, this.gridView2.Columns["申请数量"]).ToString());
                    sendQty = System.Convert.ToInt32(this.gridView2.GetRowCellValue(i, this.gridView2.Columns["实际发料数"]).ToString());

                    if (barcode.Equals("Y"))
                    {
                        if (!reqQty.Equals(sendQty))
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "有条码的防伪标签请扫描出库!");
                            return false;
                        }
                    }

                    if (sendQty > reqQty)
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "实际发料数不能大于申请数!");
                        return false;
                    }

                    string sql = "update " + SecuGlobal.tbSecurityRequestD  + " set status = 'CN', " +
                                "actual_send_qty = '" + sendQty + "' where req_doc = '" + bufDocNo + "' and req_seq = '" + bufSeq + "' " +
                                "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                    OracleHelper.ExecuteNonQuery(sql);
                }
                return true;
            }
            catch (Exception bIssueAssy)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, bIssueAssy.Message);
                return false;
            }
        }





        /// <summary>
        /// 双击获取申请单信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            doc_no = this.gridView1.GetRowCellValue(this.gridView1.FocusedRowHandle, this.gridView1.Columns["申请单号"]).ToString();   //全局变量，当前选中的DOC NO
            string plantCode = this.gridView1.GetRowCellValue(this.gridView1.FocusedRowHandle, this.gridView1.Columns["Plant"]).ToString();

            SecuGlobal.GridViewInitial(gridView2, gridControl1); //清除
            SecuGlobal.GridViewInitial(gridView3, gridControl3); //清除

            DataTable dt = OracleHelper.ExecuteDataTable(getSql2(doc_no, plantCode));
            dt = setDtHeader1(dt);
            girdView2DataLoading(dt); //加载数据到GridView2 中

            if (bCheckStatus())
            {
                btnIssueAssy.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                btnIssueAssy.Enabled = true   ;
                btnDelete.Enabled = true ;
            }

            SecuGlobal.showOK(panelStatus, lblStatus, "当前选中的申请单是:" + doc_no);
        }



        /// <summary>
        /// 检查申请单状态，如果状态都是CN - 发料状态
        /// 则返回TRUE
        /// </summary>
        /// <returns></returns>
        private bool bCheckStatus()
        {
            for (int i = 0; i < gridView2.RowCount; i++)
            {
                if (gridView2.GetRowCellValue(i, gridView2.Columns["状态"]).ToString().Equals("CN"))  //发料状态
                {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// 打印出门证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {

            //string reqDoc = "SECU201412030002";


            DataTable dt_Header = getDtHeader(doc_no);  // 物品搬出证表头部分
            DataTable dt_Detail = getDtDetail(doc_no);  //物品搬出证明细

            if (dt_Header == null || dt_Detail == null)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有查询到可用数据");
                return;
            }


            DataSet ds = new DataSet() ;
            ds.Tables.Add(dt_Detail);
            ds.Tables.Add(dt_Header);

            rptSecu report = new rptSecu(ds);
            ReportPrintTool printTool = new ReportPrintTool(report);

            printTool.ShowPreviewDialog();
            //printTool.ShowPreview(UserLookAndFeel.Default);   //UserLookAndFeel 这种机制需要在理解
        }


        private DataTable getDtHeader(string reqDoc)   
        {
            DataTable dt_Header = null;
            string sql = " select a.req_doc, " +
                        "        (select lifnr_desc from " + SecuGlobal.tb_lifnr  + " where lifnr_code = req_vendor)||' '||nvl(req_dept,' ') req_vendor, " +
                        "        a.req_user,a.update_date,a.update_time,a.remark " +
                        " from "+ SecuGlobal.tbSecurityRequestH  +" a " +
                        " where a.req_doc = '" + reqDoc + "' and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            dt_Header = OracleHelper.ExecuteDataTable(sql);
            return dt_Header;
        }



        /// <summary>
        /// FCT CODE 
        /// </summary>
        /// <param name="reqDoc"></param>
        /// <returns></returns>
        private DataTable getDtDetail(string reqDoc)
        {
            DataTable dt_Detail = null;
            string sql = " select b.req_seq,b.material_code, " +
                        "        b.actual_send_qty,barcode_flag, " +
                        "        c.security_start,c.security_end,c.qty, " +
                        "        (select nvl(max(box_no),'-') from " + SecuGlobal.tbSecurityInSnTest  + " e," + SecuGlobal.tbSecurityInTest  + " f where " +
                        "           c.security_start = e.serial_no and e.doc_no = f.doc_no and e.doc_seq = f.doc_seq and e.fct_code = f.fct_code and  e.FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ) box_no, " +
                        "        (select nvl(max(roll_no),'-') from " + SecuGlobal.tbSecurityInSnTest + " e," + SecuGlobal.tbSecurityInTest + " f where " +
                        "           c.security_start = e.serial_no and e.doc_no = f.doc_no and e.doc_seq = f.doc_seq and e.FCT_CODE = f.FCT_CODE  and e.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') roll_no " +
                        " from " + SecuGlobal.tbSecurityRequestD  + " b," + SecuGlobal.tbSecurityOut  + " c " +
                        " where b.req_doc = '" + reqDoc  + "' " +
                        " and b.req_doc = c.req_doc(+) " +
                        " and b.req_seq  = c.req_seq(+) and b.FCT_CODE = c.FCT_CODE(+) and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                        " order by req_seq,security_start ";
            dt_Detail = OracleHelper.ExecuteDataTable(sql);
            return dt_Detail;
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

        private void gridView2_Click_1(object sender, EventArgs e)
        {
            string doc_no = this.gridView2.GetRowCellValue(this.gridView2.FocusedRowHandle, this.gridView2.Columns["申请单号"]).ToString();
            string doc_seq = this.gridView2.GetRowCellValue(this.gridView2.FocusedRowHandle, this.gridView2.Columns["序号"]).ToString();
            string barcode = this.gridView2.GetRowCellValue(this.gridView2.FocusedRowHandle, this.gridView2.Columns["条形码"]).ToString();
            if (barcode.Equals("Y"))
            {
                DataTable dt = OracleHelper.ExecuteDataTable(getSql3(doc_no, doc_seq)); //如果申请单某项barcode是Y ，则显示明细信息
                gridControl3.DataSource = dt;
                gridView3.BestFitColumns();

            }
            else
            {
                SecuGlobal.GridViewInitial(gridView3, gridControl3);
            }
        }


        /// <summary>
        /// 社内发料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSSDPIssue_Click(object sender, EventArgs e)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "社内发料，请稍等...");
                FaLiao frmNew = new FaLiao();
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                }

                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            }
            catch (Exception btnRegister_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnRegister_Click.Message);
            }
        }
    }
}