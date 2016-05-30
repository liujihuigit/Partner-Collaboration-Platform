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
using System.Data.OracleClient;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraReports.UI;

namespace SecuLabel
{
    public partial class MaterialRequest : DevExpress.XtraEditors.XtraForm
    {

        ArrayList lstDeleteInfo = new ArrayList(); // 选中删除的信息
        string ls_company = "", meterialCode = "",remark = "",planDate = "";
        string ls_plant = "SSDP", barcode = "",user="",userIp = "",remain= "0",assyDesc="";    // PLANT 目前为SSDP 
        int iSelectionCounts = 0;
        int iReqQty = 0, iPlanQty = 0;
        bool bFirstAdd = true ;

        public MaterialRequest()
        {
            InitializeComponent();
        }




        /// <summary>
        /// 添加数据到GridView
        /// 某些列ReadOnly
        /// </summary>
        private void add()
        {
            DataTable table = gridControl1.DataSource as DataTable;

            if (table == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Fct Code", typeof(string)));
                dt.Columns.Add(new DataColumn("Plant", typeof(string)));
                dt.Columns.Add(new DataColumn("材料", typeof(string)));
                dt.Columns.Add(new DataColumn("条形码", typeof(string)));
                dt.Columns.Add(new DataColumn("申请数", typeof(string)));
                dt.Columns.Add(new DataColumn("生产计划数", typeof(string)));
                dt.Columns.Add(new DataColumn("厂家在库", typeof(string)));
                dt.Columns.Add(new DataColumn("材料描述", typeof(string)));
                dt.Columns.Add(new DataColumn("创建日期", typeof(string)));
                dt.Columns.Add(new DataColumn("创建时间", typeof(string)));
                dt.Columns.Add(new DataColumn("创建人", typeof(string)));
                dt.Columns.Add(new DataColumn("创建IP", typeof(string)));
    
                dt.Rows.Add(getField());
                gridControl1.DataSource = dt;

                GridCheckMarksSelection selection = new GridCheckMarksSelection(gridView1);
                selection.CheckMarkColumn.VisibleIndex = 0;
                selection.SelectionChanged += grdCheckSelect_SelectionChanged;
                gridView1.Columns["Fct Code"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["Plant"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["条形码"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["材料"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["申请数"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["生产计划数"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["厂家在库"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["材料描述"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["创建日期"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["创建时间"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["创建人"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["创建IP"].OptionsColumn.ReadOnly = true;
            }
            else
            {
                table.Rows.Add(getField());
                gridControl1.DataSource = table;
            }

        }



        /// <summary>
        /// GridView Check Box 点击事件
        /// </summary>
        private void grdCheckSelect_SelectionChanged(object sender, EventArgs e)
        {
            iSelectionCounts = 0;
            lstDeleteInfo.Clear();
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                for (int j = 0; j < gridView1.Columns.Count; j++)
                {
                    if ((bool)gridView1.GetRowCellValue(i, gridView1.Columns[0]))
                    {
                        iSelectionCounts += 1;
                        lstDeleteInfo.Add(gridView1.GetRowCellValue(i, gridView1.Columns["创建时间"]).ToString()); // 用创建时间作为添加删除，更加准确，毫秒级别的。
                        break;
                    }
                }
            }
            gridView1.Columns["材料"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView1.Columns["材料"].SummaryItem.DisplayFormat = "选中:" + iSelectionCounts;

        }


   


        /// <summary>
        /// 添加内容赋值
        /// </summary>
        /// <returns></returns>
        private object[] getField()
        {
            string sDate = DateTime.Now.ToShortDateString();
            string sTime = DateTime.Now.ToLongTimeString()+ " : " + DateTime.Now.Millisecond.ToString();  //Delete key    删除数据的键值
            return new object[] {PaCSGlobal.LoginUserInfo.Fct_code, ls_plant, meterialCode,barcode , iReqQty , iPlanQty , remain, assyDesc, sDate, sTime, user, userIp };
        }





        /// <summary>
        /// 获取基本的变量信息
        /// </summary>
        private void getVariable()
        {
            ;
            if (!string.IsNullOrEmpty(tbRemark.Text))
                remark = tbRemark.Text.Trim();

            planDate = dtPlanDate.Text.Trim().Replace("-", "");

            iReqQty =System.Convert.ToInt32( tbReqQty.Text.ToString());
            iPlanQty = System.Convert.ToInt32( tbPlanQty.Text.ToString());

        }





        /// <summary>
        /// 新申请的记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbRemark.Text) || string.IsNullOrEmpty(dtPlanDate.Text))
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "请填写备注信息或者确认计划日期是否选择！");
                return;
            }

            if (bFirstAdd)  // 判断是否是第一次写入数据
            {
                SecuGlobal.GridViewInitial(gridView1, gridControl1);
            }


            if (gridView1.RowCount>0)    // 一个申请单号中不允许添加不同类型的CODE
            {
                string bufAssyCode = cbMeterial.Text;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    object val = gridView1.GetRowCellValue(i, gridView1.Columns["材料"]);
                    if (val.ToString().Equals(bufAssyCode))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "已经添加的材料信息" + bufAssyCode);
                        return;
                    }
                }
            }

            getVariable();
            if (iReqQty <= 0 || iPlanQty <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "请输入数量信息！");
                return;
            }

            add();//添加数据到GridView
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            bFirstAdd = false;
        }





        /// <summary>
        /// 选中行删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridView1.RowCount == 0 || lstDeleteInfo.Count == 0)
                return;


            DialogResult result = MessageBox.Show("确定删除吗吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (object obj in lstDeleteInfo)
                {
                    for (int i = 0; i < gridView1.RowCount; i++)
                    {
                        object val = gridView1.GetRowCellValue(i, gridView1.Columns["创建时间"]);
                        if (val.ToString().Equals(obj.ToString()))
                        {
                            gridView1.DeleteRow(i);
                            break;
                        }
                    }
                }
            }

            gridView1.Columns["材料"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView1.Columns["材料"].SummaryItem.DisplayFormat = "选中: 0";
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
        }





        /// <summary>
        /// 获取VEDNOR CODE 信息
        /// </summary>
        private void getVendorCode4()
        {
            ls_company = cbVendor.Text.Trim();
            if (!string.IsNullOrEmpty(ls_company))
            {
                if (!ls_company.Equals("ALL"))
                {
                    string[] split = ls_company.Split(new Char[] { ':' });   //获取到四位VENDOR CODE - BP3A （POP） - PaCS下是L1073X这样的CODE
                    ls_company = split[0].Trim();
                }
            }
        }





        /// <summary>
        /// 加载基本信息-日期、材料编号等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeterialRequest_Load(object sender, EventArgs e)
        {
            dtPlanDate.Text = PaCSGlobal.GetServerDateTime(3);
            getMeterialCode(); //FCT_CODE 别MASTER 信息
            
            user = PaCSGlobal.LoginUserInfo.FullName  ;
            userIp = PaCSGlobal.GetClientIp();

            string bufVend = SecuGlobal.getPopVendorInfo(PaCSGlobal.LoginUserInfo.Venderid, PaCSGlobal.LoginUserInfo.Fct_code);
            if (!bufVend.Equals(""))
            {
                cbVendor.Text = bufVend;
            }
            else
            {
                cbVendor.Text = PaCSGlobal.LoginUserInfo.Venderid + ":" + PaCSGlobal.LoginUserInfo.Vendername; //SESC 法人信息;
            }

            getVendorCode4();
            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");
        }


        /// <summary>
        /// 获取Meterial Code 
        /// LOAD 事件中加载
        /// </summary>
        private void getMeterialCode()
        {
            try
            {
                string sql = " select material_code from " + SecuGlobal.tbMaster  + " where " +
                    " FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' order by material_code ";
                OracleDataReader odr = OracleHelper.ExecuteReader(sql);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        cbMeterial.Properties.Items.Add(odr["material_code"]);
                    }
                }
                else
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "没有找到MASTER 信息");
                }

            }
            catch (Exception getVendorCode)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, getVendorCode.Message );
            }
        }







        /// <summary>
        /// 获取库存信息及基本的变量信息
        /// 目前PLANT 只设置为 SSDP
        /// </summary>
        /// <param name="ls_company"></param>
        /// <param name="ls_plant"></param>
        /// <returns></returns>
        private string getSql(string ls_company, string ls_plant, string meterialCode)  //ls_company 为四位的VENDOR CODE
        {
            string sql = " select fct_code, material_code, " +
                            "        barcode_flag, " +
                            "        (select b.description from " + SecuGlobal.tb_fpp_itemmaster  + " b where a.material_code = b.matnr) description, " +
                            "        board_count, " +
                            "        (select nvl(max(c.stock_qty),0)  " +
                            "           from " + SecuGlobal.tbSecurityStock  + " c  " +
                            "          where c.company = '" + ls_company + "'  " +
                            "            and c.plant = '" + ls_plant + "' " +
                            "            and c.material_code = a.material_code and c.FCT_CODE = a.FCT_CODE ) stock_qty, " +
                            "        0 req_qty, " +
                            "        0 prod_plan_qty, " +
                            "        rpad('" + ls_plant + "',4) plant " +
                            " from " + SecuGlobal.tbMaster  + " a " +
                            " where material_code = '" + meterialCode + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                            " order by material_code ";
            return sql;
        }





        /// <summary>
        /// 查询库存信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbMeterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbMeterial.SelectedIndex != -1)
                    meterialCode = cbMeterial.Properties.Items[cbMeterial.SelectedIndex].ToString();
                else
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请选择材料信息");
                    return;
                }


                DataTable dt = OracleHelper.ExecuteDataTable(getSql(ls_company, ls_plant, meterialCode));    //ls_company ('BP3A')  ls_plant('SSDP')
                if (dt != null)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        barcode = dr["barcode_flag"].ToString();
                        assyDesc = dr["description"].ToString();
                        txtRemain.Text = dr["stock_qty"].ToString();
                        remain = txtRemain.Text.Trim();
                    }

                }
                SecuGlobal.showOK(panelStatus, lblStatus, "材料选择改变，请确认信息" + meterialCode);

            }
            catch (Exception error)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, error.Message);
            }
       
        }




        /// <summary>
        /// 获取Company 信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVendor.SelectedIndex != -1)
                ls_company = cbVendor.Properties.Items[cbVendor.SelectedIndex].ToString();

            if (!string.IsNullOrEmpty(ls_company))
            {
                if (!ls_company.Equals("ALL"))
                {
                    string[] split = ls_company.Split(new Char[] { ':' });
                    ls_company = split[0].Trim();
                }
            }

        }



        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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




        private bool bAnalysisData()
        {
            int a = 0, b = 0;

            if (gridView1.RowCount <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有要保存的数据!");
                return false;
            }

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                object valAssy = gridView1.GetRowCellValue(i, gridView1.Columns["材料"]);
                object valReq = gridView1.GetRowCellValue(i, gridView1.Columns["申请数"]);
                object valPlan = gridView1.GetRowCellValue(i, gridView1.Columns["生产计划数"]);
                if (valReq.ToString().Equals("0"))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, valAssy + ": 申请数不能为零");
                    return false ;
                }

                if (valPlan.ToString().Equals("0"))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, valAssy + "生产计划数不能为零");
                    return false;
                }


                if (!int.TryParse(valReq.ToString(), out a))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, valAssy + ": 申请数量不是有效数字");
                    return false;
                }

                if (!int.TryParse(valPlan.ToString(), out b))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, valPlan + ": 申请数量不是有效数字");
                    return false;
                }
            }
            return true;
        }




        private void btnSave_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!bAnalysisData())
                    return;

                DialogResult drg = MessageBox.Show("您确信要提交 " + gridView1.RowCount  + "  条申请单吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (drg == DialogResult.OK)
                {
                    backgroundWorker1.RunWorkerAsync();
                    SplashScreenManager.ShowForm(typeof(WaitLoading));
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnSave.Enabled = false;

                    string doc_seq = OracleHelper.ExecuteScalar("select 'SECU'||to_char(sysdate,'yyyymmdd')||" +
                                          "fn_gene_seq('SECU','REQ',to_char(sysdate,'yyyymmdd'),'N','N','N',4) doc_seq from dual").ToString();  //申请单号

                    //需要保存HEADER 信息
                    OracleHelper.ExecuteNonQuery(getSql2(doc_seq));

                    for (int i = 0; i < gridView1.RowCount; i++)
                    {
                        object valAssy = gridView1.GetRowCellValue(i, gridView1.Columns["材料"]);
                        object valReq = gridView1.GetRowCellValue(i, gridView1.Columns["申请数"]);
                        object valPlan = gridView1.GetRowCellValue(i, gridView1.Columns["生产计划数"]);
                        object barCode1 = gridView1.GetRowCellValue(i, gridView1.Columns["条形码"]);
                        OracleHelper.ExecuteNonQuery(getSql1(doc_seq, (i + 1).ToString(), valAssy.ToString(), valReq.ToString(), valPlan.ToString(), barCode1.ToString()));
                    }
                    SecuGlobal.showOK(panelStatus, lblStatus, "材料申请OK，申请单号：" + doc_seq);
                    //SecuGlobal.GridViewInitial(gridView1, gridControl1); 
                    btnSave.Enabled = true;
                    bFirstAdd = true;

                    //申请单打印
                    DialogResult result = MessageBox.Show("是否要打印机申请单信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        //printReqDoc("SECU201412170003");
                        printReqDoc(doc_seq);
                    }

                });
            }
            catch (Exception err)
            {
                SecuGlobal.showOK(panelStatus, lblStatus, err.Message);
            }
        }




        private void printReqDoc(string doc_no)
        {
            DataTable dt_Header = getDtHeader(doc_no);  // 物品搬出证表头部分
            DataTable dt_Detail = getDtDetail(doc_no, ls_company);  //物品搬出证明细

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
            string sql = " select a.req_doc, " +
                        "        '*'||a.req_doc||'*' req_doc_barcode, " +
                        "        a.req_vendor, " +
                        "        (select vendorname from  " + SecuGlobal.mv_ep_vendor + " b where b.vendorcode = a.req_vendor) req_vendor_name, " +
                        "        a.req_user, " +
                        "        a.req_date, " +
                        "        a.prod_plan_date, " +
                        "        a.remark, " +
                        "        a.plant " +
                        " from " + SecuGlobal.tbSecurityRequestH  + " a " +
                        " where req_doc = '" + reqDoc + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            dt_Header = OracleHelper.ExecuteDataTable(sql);
            return dt_Header;
        }



        /// <summary>
        /// FCT CODE 
        /// </summary>
        /// <param name="reqDoc"></param>
        /// <returns></returns>
        private DataTable getDtDetail(string reqDoc,string vend4)  //报表的信息FCT CODE 增加
        {
            string bufPlant = "SSDP%";

            DataTable dt_Detail = null;
            string sql = " select b.req_seq, " +
                        "        b.material_code, " +
                        "        (select c.description from " + SecuGlobal.tb_fpp_itemmaster  + " c where c.matnr = b.material_code and c.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') material_desc, " +
                        "        b.req_qty, " +
                        "        b.prod_plan_qty, " +
                        "        b.actual_send_qty, " +
                        "        b.gr_qty, " +
                        "        (select board_count from " + SecuGlobal.tbMaster  + " e where e.material_code = b.material_code and e.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') board_count, " +
                        "        (select nvl(max(stock_qty),0)  " +
                        "          from " + SecuGlobal.tbSecurityStock  + " d  " +
                        "         where d.company = '" + vend4  + "'  " +
                        "           and d.material_code = b.material_code and d.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                        "           and d.plant like '" + bufPlant + "') stock_qty " +
                        " from " + SecuGlobal.tbSecurityRequestD  + " b " +
                        " where b.req_doc = '" + reqDoc + "' " +
                        "   and b.plant like '" + bufPlant + "' " +
                        "   and b.status <> 'DE' and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
            dt_Detail = OracleHelper.ExecuteDataTable(sql);
            return dt_Detail;
        }





        private string getSql1(string doc_seq, string req_seq, string assyCode, string req_count, string plan_count, string barcode1)
        {
            string sql = " insert into  " + SecuGlobal.tbSecurityRequestD  + "  " +
                        " ( " +
                        " REQ_DOC,REQ_SEQ,MATERIAL_CODE,REQ_QTY,PROD_PLAN_QTY,STATUS, " +
                        " GR_QTY,BARCODE_FLAG,CREATE_DATE,CREATE_TIME,CREATE_USER,CREATE_IP,PLANT,FCT_CODE " +
                        " ) " +
                        " VALUES " +
                        " ( " +
                        " '" + doc_seq + "','" + req_seq + "','" + assyCode + "','" + req_count + "','" + plan_count + "','RQ', " +
                        " '0','" + barcode1 + "',to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24miss'),'" + user + "','" + userIp + "','" + ls_plant + "','" + PaCSGlobal.LoginUserInfo .Fct_code  + "' " +
                        " ) ";
            return sql; 
        }



        private string getSql2(string doc_seq)
        {
            string sql = " insert into  " + SecuGlobal.tbSecurityRequestH + "  " +
                        " ( " +
                        " req_doc,req_vendor,req_user,req_date,prod_plan_date,remark,plant,FCT_CODE,UPDATE_DATE,UPDATE_TIME,UPDATE_USER " +
                        " ) " +
                        " VALUES " +
                        " ( " +
                        " '" + doc_seq + "','" + ls_company   +"','" + user  + "'," +
                        " to_char(sysdate,'yyyymmdd'),'" + planDate + "','" + remark + "','" + ls_plant + "','" + PaCSGlobal.LoginUserInfo.Fct_code + "',to_char(sysdate,'yyyymmdd'),to_char(sysdate,'HH24miss'),'" + user + "' " +
                        " ) ";
            return sql;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "代码稍后上载");
        }

        private void tbReqQty_KeyPress(object sender, KeyPressEventArgs e)
        {
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar !=46 )
                e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)  //小数点
                {
                    if (tbReqQty.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(tbReqQty.Text, out oldf);
                        b2 = float.TryParse(tbReqQty.Text + e.KeyChar.ToString(), out f);
                        if (b2 == false)
                        {
                            if (b1 == true)
                                e.Handled = true;
                            else
                                e.Handled = false;
                        }
                    }
                }
        }

        private void tbPlanQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 46)  //小数点
            {
                if (tbReqQty.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(tbReqQty.Text, out oldf);
                    b2 = float.TryParse(tbReqQty.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            printReqDoc("SECU201412170003");
        }

   



    }
}