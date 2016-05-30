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
using System.Data.OracleClient;
using DevExpress.XtraReports.UI;

namespace SecuLabel
{
    public partial class ReqListVendor : DevExpress.XtraEditors.XtraForm
    {
        string plantCode = "", vendorCode4 = "", vendorInfo = "", status = "",dFrom ="",dTo= "";
        string fctCode = "";
        string doc_no;
        bool bRun = false;
        int grCount = 0;


       
        public ReqListVendor()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 加载基本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReqListVendor_Load(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "正在加载数据，请稍等...");
            SecuGlobal.setDate(dateEditFrom, dateEditTo);

            
            fctCode = PaCSGlobal.LoginUserInfo.Fct_code;
            if (fctCode.Equals("C6H0A"))
                plantCode = "SESC";
            else
            {
                plantCode = "SSDP";
            }
            cbPlant.Text = plantCode;



            string bufVend = SecuGlobal.getPopVendorInfo(PaCSGlobal.LoginUserInfo.Venderid, PaCSGlobal.LoginUserInfo.Fct_code);
            if (!bufVend.Equals(""))
            {
                cbVendor.Text = bufVend;
            }
            else
            {
                cbVendor.Text = PaCSGlobal.LoginUserInfo.Venderid + ":" + PaCSGlobal.LoginUserInfo.Vendername;  //苏州法人
            }


            vendorInfo = cbVendor.Text;
            if (!string.IsNullOrEmpty(vendorInfo))
            {
                if (!vendorInfo.Equals("ALL"))
                {
                    string[] split = vendorInfo.Split(new Char[] { ':' });
                    vendorCode4 = split[0].Trim();
                }
                else
                {
                    vendorCode4 = "%";
                }
            }
            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");

        }




        /// <summary>
        /// 更改DataTable 标题和顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader(DataTable dt)
        {
            //标题栏
            string[] colField = {"FCT_CODE", "PLANT", "REQ_DOC", "REQ_VENDOR", "REQ_USER", "REQ_DATE", "PROD_PLAN_DATE", 
                                    "REMARK"};

            string[] colName = { "Fct Code","Plant", "申请单号", "申请厂家", "申请人", "申请日期", "生产计划日期", 
                                   "备注" };

            for (int i = 0; i < colField.Length ; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);

            }
            return dt;
        }


        


        /// <summary>
        /// 获取可执行的SQL 
        /// 申请单查询
        /// </summary>
        /// <param name="ls_plant"></param>
        /// <param name="is_lifnr"></param>
        /// <param name="ls_sdate"></param>
        /// <param name="ls_edate"></param>
        /// <param name="ls_status"></param>
        /// <returns></returns>
        private string getSql(string ls_plant,string is_lifnr, string ls_sdate, string ls_edate,string ls_status)
        {
    
            string sql = " SELECT FCT_CODE, REQ_DOC,  " +
                         "        REQ_VENDOR,  " +
                         "        REQ_USER,  " +
                         "        REQ_DATE,  " +
                         "        PROD_PLAN_DATE,  " +
                         "        REMARK, " +
                         "        PLANT " +
                         " FROM " + SecuGlobal.tbSecurityRequestH  + " H " +
                         " WHERE REQ_VENDOR = '" + is_lifnr + "' " +
                         "   AND REQ_DATE BETWEEN '" + ls_sdate + "' AND '" + ls_edate + "' " +
                         "   AND (('" + ls_status + "' = '1' AND NOT EXISTS (SELECT 1 FROM " + SecuGlobal.tbSecurityRequestD  + " D  " +
                         "                                           WHERE H.REQ_DOC = D.REQ_DOC and d.FCT_CODE = h.FCT_CODE " +
                         "                                             AND D.STATUS = 'GR')) OR " +
                         "        ('" + ls_status + "' = '2' AND EXISTS (SELECT 1 FROM " + SecuGlobal.tbSecurityRequestD + " D  " +
                         "                                       WHERE H.REQ_DOC = D.REQ_DOC and d.FCT_CODE = h.FCT_CODE " +
                         "                                         AND D.STATUS = 'GR')) OR " +
                         "        '" + ls_status + "' = '3' " +
                         "       ) " +
                         " and h.plant like '" + ls_plant + "' and h.FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                         " ORDER BY REQ_DOC DESC ";
            return sql;
        }





        /// <summary>
        /// 变量赋值
        /// </summary>
        private void getVariable()
        {

            if (string.IsNullOrEmpty(plantCode))  //plantCode 模糊查询
            {
                plantCode += "%";
            }
            else
            {
                plantCode = "%";
            }

            if (!string.IsNullOrEmpty(cbStatus.Text))
            {
                status = cbStatus.Text.Substring(0, 1);   // 状态CODE  1 - 3
            }

            dFrom = dateEditFrom.Text.Trim().Replace("-","");
            dTo = dateEditTo.Text.Trim().Replace("-", "");
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
                
                this.Invoke((MethodInvoker)delegate
                {
                    btnQuery.Enabled = false;

                    SecuGlobal.GridViewInitial(gridView1, gridControl1);
                    SecuGlobal.GridViewInitial(gridView2, gridControl2);

                    DataTable dt = null;
                    dt = OracleHelper.ExecuteDataTable(getSql(plantCode, vendorCode4, dFrom, dTo, status)); //vendorCode4 BP3A
                    if (dt == null)
                        return;

                    dt = setDtHeader(dt); //更改标题栏和显示顺序

                    gridControl1.DataSource = dt;
                    gridView1.BestFitColumns();

                    gridView1.Columns["申请单号"].SummaryItem.SummaryType = SummaryItemType.Count ;
                    gridView1.Columns["申请单号"].SummaryItem.DisplayFormat = "All:{0:f0} ";

                    btnQuery.Enabled = true;

                });
            }
            catch (Exception err)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, err.Message);
            }
        }




        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }




        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "正在查询数据,请稍等...");
                getVariable();
                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));
                SecuGlobal.showOK(panelStatus, lblStatus, "Ready");
            }
            catch (Exception btnQuery_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnQuery_Click.Message);
            }
        }


        private string getSql2(string LS_REQ_DOC, string ls_plant)
        {
            string sql = " SELECT FCT_CODE,REQ_DOC,  " +
                        "        REQ_SEQ,  " +
                        "        MATERIAL_CODE,  " +
                        "        (select b.description from " + SecuGlobal.tb_fpp_itemmaster  + " b where a.material_code = b.matnr and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') description, " +
                        "        (select board_count from " + SecuGlobal.tbMaster  + " c where c.material_code = a.material_code and c.fct_code = a.fct_code ) board_count, " +
                        "        REQ_QTY,  " +
                        "        PROD_PLAN_QTY, " +
                        "        ACTUAL_SEND_QTY, " +
                        "        STATUS,  " +
                        "        GR_QTY,  " +
                        "        BARCODE_FLAG, " +
                        "        PLANT " +
                        " FROM " + SecuGlobal.tbSecurityRequestD  + " a " +
                        " WHERE REQ_DOC = '" + LS_REQ_DOC  + "' " +
                        " and a.plant like '" + ls_plant  + "' and a.FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                        " ORDER BY REQ_SEQ ";
            return sql;
        }




        /// <summary>
        /// 申请单明细显示
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader1(DataTable dt)
        {
            //标题栏
            string[] colField = { "FCT_CODE","PLANT", "REQ_DOC", "REQ_SEQ", "MATERIAL_CODE", "REQ_QTY", "ACTUAL_SEND_QTY", 
                                    "board_count","GR_QTY","PROD_PLAN_QTY","description","STATUS","BARCODE_FLAG"};

            string[] colName = { "Fct Code","Plant", "申请单号", "序号", "材料", "申请数量", "实际发料数", 
                                   "包装单位","入库数量","生产计划数","材料描述","状态","条形码"};

            for (int i = 0; i < colField.Length ; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);

            }
            return dt;
        }



        private void girdView2DataLoading(DataTable dt)
        {
            gridControl2.DataSource = dt;
            gridView2.BestFitColumns();

            //MessageBox.Show(gridView2.Columns.Count.ToString());

            for (int i = 1; i < gridView2.Columns.Count; i++)
            {
                gridView2.Columns[i].OptionsColumn.ReadOnly = true;  //设置成只读状态
            }

            GridCheckMarksSelection selection = new GridCheckMarksSelection(gridView2); // 增加CHECKBOX 
            selection.CheckMarkColumn.VisibleIndex = 0;
            selection.SelectionChanged += selection_SelectionChanged;
        }



        /// <summary>
        /// 选中数计算
        /// grCount 全局变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selection_SelectionChanged(object sender, EventArgs e)
        {
            
            for (int i = 0; i < gridView2.RowCount; i++)
            {
                for (int j = 0; j < gridView2.Columns.Count; j++)
                {
                    if ((bool)gridView2.GetRowCellValue(i, gridView2.Columns[0]))
                    {
                        grCount += 1;
                        break;
                    }
                }
            }
            gridView2.Columns["申请单号"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView2.Columns["申请单号"].SummaryItem.DisplayFormat = "All:" + grCount;
        }



        /// <summary>
        /// 后台加载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    doc_no = this.gridView1.GetRowCellValue(this.gridView1.FocusedRowHandle, this.gridView1.Columns["申请单号"]).ToString();
                    SecuGlobal.GridViewInitial(gridView2, gridControl2); //清除

                    DataTable dt = OracleHelper.ExecuteDataTable(getSql2(doc_no, plantCode));
                    dt = setDtHeader1(dt);
                    girdView2DataLoading(dt); //加载数据到GridView2 中

                });
            }
            catch (Exception err)
            {
                XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
            }
        }





        /// <summary>
        ///双击GRIDVIEW ，显示对应的申请单号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (bRun)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "后台有未处理完成的数据，请稍等..");
                    return;
                }

                SecuGlobal.showOK(panelStatus, lblStatus, "正在查询数据,请稍等...");
                backgroundWorker2.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));
                SecuGlobal.showOK(panelStatus, lblStatus, "Ready");
            }
            catch (Exception btnQuery_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnQuery_Click.Message);
            }
        }



        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }



        /// <summary>
        /// 行号显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 申请单入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGr_Click(object sender, EventArgs e)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "正在进行入库，请稍等...");
                bRun = true;
                btnGr.Enabled = false;

                if (gridView2.RowCount == 0 || grCount.Equals(0))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请选择申请单信息");
                    return;
                }

                DialogResult dlg = MessageBox.Show("您确认要进行入库：" + doc_no + "？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dlg == DialogResult.OK)
                {
                    int iCount = 0;
                    for (int i = 0; i < gridView2.RowCount; i++)
                    {
                        for (int j = 0; j < gridView2.Columns.Count; j++)
                        {
                            if ((bool)gridView2.GetRowCellValue(i, gridView2.Columns[0]))
                            {
                                iCount += 1;
                                break;

                            }
                        }
                    }
                    if (iCount == 0)
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "请选中要入库的信息");
                        return;
                    }
                        

                    if (!bgrDataUpload()) //失败
                    {
                        return;
                    }
                    doc_no = "";
                    btnQuery_Click(sender, e);
                }

                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            }
            catch (Exception btnGr_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnGr_Click.Message);
            }
            finally
            {
                bRun = false;
                btnGr.Enabled = true;
            }

        }



        /// <summary>
        /// 将申请单下的所有信息分析入库处理
        /// </summary>
        private bool  bgrDataUpload()
        {

            for (int i = 0; i < gridView2.RowCount; i++)
            {
                for (int j = 0; j < gridView2.Columns.Count; j++)
                {
                    if ((bool)gridView2.GetRowCellValue(i, gridView2.Columns[0]))
                    {
                        string ls_req_doc = gridView2.GetRowCellValue(i, gridView2.Columns["申请单号"]).ToString();
                        string li_req_seq = gridView2.GetRowCellValue(i, gridView2.Columns["序号"]).ToString();
                        string ls_status = gridView2.GetRowCellValue(i, gridView2.Columns["状态"]).ToString();
                        string materialCode = "";
                        int SendQty = 0;

                        if (!ls_status.Equals("CN"))  //确认发料状态
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, li_req_seq + "：该记录不允许入库");
                            return false ;
                        }

                        string sql = "update " + SecuGlobal.tbSecurityRequestD  + " set status = 'GR', " +
                                     "gr_qty = actual_send_qty where req_doc = '" + ls_req_doc + "' and req_seq = '" + li_req_seq + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";

                        OracleHelper.ExecuteNonQuery(sql);  //更新申请单状态-GR


                        string sqlStock = "select material_code,actual_send_qty from " + SecuGlobal.tbSecurityRequestD + " where  " +
                                          "req_doc = '" + ls_req_doc + "' and req_seq = ' " + li_req_seq + " ' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                        OracleDataReader odr = OracleHelper.ExecuteReader(sqlStock);
                        if (odr.HasRows)
                        {
                            while (odr.Read())
                            {
                                materialCode = odr["material_code"].ToString();
                                SendQty = System.Convert.ToInt32(odr["actual_send_qty"]);  //实际发料数
                            }
                        }
                        else
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, ls_req_doc + "材料CODE发生异常，请联系管理员 - 【grDataUpload】");
                            return false ;
                        }

                        //判断是否入库成功---
                        if (!bSecuMove(cbPlant.Text.Trim() ,vendorCode4 ,materialCode ,SendQty))   // plant 此处不带%
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, ls_req_doc + "入库失败，请联系管理员 - 【grDataUpload】");
                            return false ;
                        }
                       

                        string sqlGr =  " insert into " + SecuGlobal.tbSecurityDoc  + " " +
                                        "        (company,doc_no,doc_seq,doc_type,doc_mvt, " +
                                        "           material_code,doc_qty,req_doc,req_seq, " +
                                        "           doc_date,doc_time,doc_user,doc_ip,barcode_flag,plant,fct_code) " +
                                        " select '" + vendorCode4  + "', " +
                                        "        'GR'||to_char(sysdate,'yyyymmdd')||fn_gene_seq('SECU','GR',to_char(sysdate,'yyyymmdd'),'N','N','N',4), " +
                                        "          1, " +
                                        "          'GR', " +
                                        "          'NOR', " +
                                        "          material_code, " +
                                        "          actual_send_qty, " +
                                        "          req_doc, " +
                                        "          req_seq, " +
                                        "          to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24MIss')," +
                                        "          '" + PaCSGlobal.LoginUserInfo.Name + "','" + PaCSGlobal.GetClientIp() + "',barcode_flag, " +
                                        "          '" + cbPlant.Text.Trim() + "',FCT_CODE " +
                                        " from " + SecuGlobal.tbSecurityRequestD + " " +
                                        " where req_doc = '" + ls_req_doc + "' " +
                                        " and req_seq = '" + li_req_seq + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";

                        OracleHelper.ExecuteNonQuery(sqlGr);

                        break;

                    }
                }
            }

            return true;

        }

 


        /// <summary>
        /// 判断是否入库成功
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="vend4"></param>
        /// <param name="materialcode"></param>
        /// <param name="actualSendQty"></param>
        /// <returns>成功： true </returns>
        private bool bSecuMove(string plant,string vend4,string materialcode,int  actualSendQty)
        {
            try
            {
                int iCount = 0;   
                string sql = "select count(*) from " + SecuGlobal.tbSecurityStock  + " where company = '" + vend4 + "' " +
                             "and material_code = '" + materialcode + "' and plant = '" + plant + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                iCount = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql));  //返回第一行一列的数-  count(*)

                if(iCount == 0)
                {
                    if (actualSendQty < 0)
                        return false;

                    string sqlInsert = " insert into " + SecuGlobal.tbSecurityStock  + " (company,material_code,stock_qty, update_date,update_time,update_user,update_ip, plant,fct_code) " +
                                        " values ('" + vend4 + "','" + materialcode + "','" + actualSendQty + "', " +
                                        "             to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24MIss'),'" + PaCSGlobal.LoginUserInfo.Name   + "'," +
                                        " '" + PaCSGlobal.GetClientIp() + "', " +
                                        "              '" + plant + "','" + PaCSGlobal.LoginUserInfo .Fct_code  + "') ";
                    OracleHelper.ExecuteNonQuery(sqlInsert);   //库存 转入
                    return true;
                }


                int li_remain_qty;
                string sqlStock = " select stock_qty + '" + actualSendQty + "' as StockCount from " + SecuGlobal.tbSecurityStock + " " +
                                    " where company = '" + vend4  + "' " +
                                    " and material_code = '" + materialcode  + "' " +
                                    " and plant = '" + plant  + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
                li_remain_qty = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sqlStock));
                if (li_remain_qty < 0)
                {
                    return false;
                }
                else
                {
                    if (iCount.Equals(1))
                    {
                        string sqlUpdate = " update " + SecuGlobal.tbSecurityStock  + " set stock_qty = stock_qty + '" + actualSendQty + "', update_date = to_char(sysdate,'yyyymmdd'), " +
                                            " update_time = to_char(sysdate,'hh24MIss'),update_user = '" + PaCSGlobal.LoginUserInfo.Name + "'," +
                                            " update_ip = '" + PaCSGlobal.GetClientIp() + "' " +
                                            " where company = '" + vend4  + "' and material_code = '" + materialcode  + "'  " +
                                            " and plant = '" + plant + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
                        OracleHelper.ExecuteNonQuery(sqlUpdate);
                        SecuGlobal.showOK(panelStatus, lblStatus, "库存更新OK");
                        return true;
                    }
                    else
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "异常iCount-请联系管理员");
                        return false ;
                    }
   
                }

            }
            catch (Exception bSecuMove)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "异常：" + bSecuMove.Message);
                return false;
            }
        }

        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "状态改变，请重新查询！");
        }

        private void dateEditFrom_EditValueChanged(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "日期改变，请重新查询！");
        }

        private void dateEditTo_EditValueChanged(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "日期改变，请重新查询！");
        }



        /// <summary>
        /// 申请单数据提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    SecuGlobal.showOK(panelStatus, lblStatus, "申请单数据上传中,请稍等...");
            //    if (gridView2.RowCount <= 0)
            //    {
            //        SecuGlobal.showNG(panelStatus, lblStatus, "没有申请单信息");
            //        return;
            //    }

            //    DialogResult dr = MessageBox.Show("确认保存吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            //    if (dr == DialogResult.OK)
            //    {
            //        if (bReqDocSave())
            //        {
            //            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
      
            //        }
            //    }

            //}
            //catch (Exception btnSave_Click)
            //{
            //    SecuGlobal.showNG(panelStatus, lblStatus, btnSave_Click.Message);
            //}
        }




        /// <summary>
        /// 申请单保存---申请单修改记录
        /// </summary>
        /// <returns></returns>
        private bool bReqDocSave()
        {
            int iC = 0;
            for (int i = 0; i < gridView2.RowCount; i++)
            {
                for (int j = 0; j < gridView2.Columns.Count; j++)
                {
                    if ((bool)gridView2.GetRowCellValue(i, gridView2.Columns[0]))
                    {
                        iC += 1;
                        string ls_req_doc = gridView2.GetRowCellValue(i, gridView2.Columns["申请单号"]).ToString();
                        string li_req_seq = gridView2.GetRowCellValue(i, gridView2.Columns["序号"]).ToString();
                        string ls_status = gridView2.GetRowCellValue(i, gridView2.Columns["状态"]).ToString();
                        int  ll_board_count = System.Convert.ToInt32 (gridView2.GetRowCellValue(i, gridView2.Columns["申请单位"]).ToString());
                        int ll_actual_send_qty = System.Convert.ToInt32 (gridView2.GetRowCellValue(i, gridView2.Columns["实际发料数"]).ToString());
                        int ll_req_qty = System.Convert.ToInt32 (gridView2.GetRowCellValue(i, gridView2.Columns["申请数量"]).ToString());

                        if (!ls_status.Equals("RQ"))  //不允许用户修改数据
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, li_req_seq + "已经出库，不允许修改记录");
                            return false ;
                        }

                        if (ll_req_qty % ll_board_count != 0) //确认： 申请时候判断这个逻辑吧？？
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, li_req_seq + "该记录请输入包装数量的整数倍!");
                            return false;
                        }

                        if (ll_req_qty == ll_actual_send_qty)   //这个逻辑在核实 还有SQL中数量信息
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, li_req_seq + "该记录内容没有变化!");
                            return false;
                        }

                        string sql =  " update " + SecuGlobal.tbSecurityRequestD  + " " +
                                        " set req_qty = '" + ll_req_qty + "', " +
                                        "     actual_send_qty = '" + ll_req_qty + "', " +
                                        "     update_date = to_char(sysdate,'yyyymmdd'), " +
                                        "     update_time = to_char(sysdate,'hh24MIss'), " +
                                        "     update_user = '" + PaCSGlobal.LoginUserInfo.Name  + "', " +
                                        "     update_ip = '" + PaCSGlobal.GetClientIp() + "' " +
                                        " where req_doc = '" + ls_req_doc + "' " +
                                        " and req_seq = '" + li_req_seq + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
                        OracleHelper.ExecuteNonQuery(sql);
                    }
                }
            }

            if (iC == 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有选中内容,请确认");
                return false;
            }
            else
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                return true;
            }
        }



        /// <summary>
        /// 申请单删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "正在删除申请单数据,请稍等...");
                if (gridView2.RowCount <= 0)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "没有申请单信息");
                    return;
                }

                DialogResult dr = MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    if (bReqDocDelete())
                    {
                        SecuGlobal.showOK(panelStatus, lblStatus, "OK");

                    }
                }

            }
            catch (Exception btnDelete_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnDelete_Click.Message);
            }
        }



        /// <summary>
        /// 申请单信息删除
        /// </summary>
        /// <returns></returns>
        private bool bReqDocDelete()
        {
            int iC = 0;
            for (int i = 0; i < gridView2.RowCount; i++)
            {
                for (int j = 0; j < gridView2.Columns.Count; j++)
                {
                    if ((bool)gridView2.GetRowCellValue(i, gridView2.Columns[0]))
                    {
                        iC += 1;
                        string ls_req_doc = gridView2.GetRowCellValue(i, gridView2.Columns["申请单号"]).ToString();
                        string li_req_seq = gridView2.GetRowCellValue(i, gridView2.Columns["序号"]).ToString();
                        string ls_status = gridView2.GetRowCellValue(i, gridView2.Columns["状态"]).ToString();
                      
                        if (!ls_status.Equals("RQ"))
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, li_req_seq + "该申请单不允许删除");
                            return false;
                        }

                        string sql = " update " + SecuGlobal.tbSecurityRequestD + " " +
                                        " set status = 'DE', " +
                                        "     update_date = to_char(sysdate,'yyyymmdd'), " +
                                        "     update_time = to_char(sysdate,'hh24MIss'), " +
                                        "     update_user = '" + PaCSGlobal.LoginUserInfo.Name + "', " +
                                        "     update_ip = '" + PaCSGlobal.GetClientIp() + "' " +
                                        " where req_doc = '" + ls_req_doc + "' " +
                                        " and req_seq = '" + li_req_seq + "' and FCT_CODE = '"+ PaCSGlobal.LoginUserInfo.Fct_code  +"' ";
                        OracleHelper.ExecuteNonQuery(sql);
                    }
                }
            }

            if (iC == 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有选中内容,请确认");
                return false;
            }
            else
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                return true;
            }
        }

        private void btnReqListPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("此页面应该不用打印申请单信息，是否开发需要在确认");
        }

        private void btnDoorPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(doc_no))
            {
                SecuGlobal.showNG(panelStatus ,lblStatus ,"请首先选择申请单信息");
                return;
            }
            printReqDoc(doc_no);
        }



        private void printReqDoc(string doc_no)
        {
            DataTable dt_Header = getDtHeader(doc_no);  // 物品搬出证表头部分
            DataTable dt_Detail = getDtDetail(doc_no, vendorCode4);  //物品搬出证明细

            if (dt_Header == null || dt_Detail == null)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有查询到可用数据");
                return;
            }


            DataSet ds = new DataSet();
            ds.Tables.Add(dt_Detail);
            ds.Tables.Add(dt_Header);

            xrpReqList report = new xrpReqList(ds);
            ReportPrintTool printTool = new ReportPrintTool(report);

            printTool.ShowPreviewDialog();
            //printTool.ShowPreview(UserLookAndFeel.Default);   //UserLookAndFeel 这种机制需要在理解
        }



        private DataTable getDtHeader(string reqDoc)
        {
            DataTable dt_Header = null;
            string sql = "  select a.req_doc, " +
                        "  '*'||a.req_doc||'*' req_doc_barcode,  " +
                        "   a.req_vendor,  " +
                        "    (select vendorname from " + SecuGlobal.mv_ep_vendor  + " b where b.vendorcode = a.req_vendor) req_vendor_name,  " +
                        "    a.req_user,  " +
                        "     a.req_date,  " +
                        "    a.prod_plan_date,  " +
                        "    a.remark,  " +
                        "    a.plant  " +
                        " from " + SecuGlobal.tbSecurityRequestH  + " a  " +
                        " where req_doc = '" + reqDoc + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            dt_Header = OracleHelper.ExecuteDataTable(sql);
            return dt_Header;
        }



        /// <summary>
        /// FCT CODE 
        /// </summary>
        /// <param name="reqDoc"></param>
        /// <returns></returns>
        private DataTable getDtDetail(string reqDoc,string vend4)
        {
            string bufPlant = "SSDP%";

            DataTable dt_Detail = null;
            string sql = " select b.req_seq, " +
                        "        b.material_code, " +
                        "        (select c.description from " + SecuGlobal.tb_fpp_itemmaster + " c where c.matnr = b.material_code) material_desc, " +
                        "        b.req_qty, " +
                        "        b.prod_plan_qty, " +
                        "        b.actual_send_qty, " +
                        "        b.gr_qty, " +
                        "        (select board_count from " + SecuGlobal.tbMaster + " e where e.material_code = b.material_code) board_count, " +
                        "        (select nvl(max(stock_qty),0)  " +
                        "          from " + SecuGlobal.tbSecurityStock + " d  " +
                        "         where d.company = '" + vend4 + "'  " +
                        "           and d.material_code = b.material_code " +
                        "           and d.plant like '" + bufPlant + "') stock_qty " +
                        " from " + SecuGlobal.tbSecurityRequestD + " b " +
                        " where b.req_doc = '" + reqDoc + "' " +
                        "   and b.plant like '" + bufPlant + "' " +
                        "   and b.status <> 'DE' ";
            dt_Detail = OracleHelper.ExecuteDataTable(sql);
            return dt_Detail;
        }








  



        /// <summary>
        /// 行号显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView2_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;//对齐方式
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

    }
}