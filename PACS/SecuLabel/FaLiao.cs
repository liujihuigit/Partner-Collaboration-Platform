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
using System.Collections;
using DevExpress.Data;
using System.Data.OracleClient;
using DevExpress.XtraSplashScreen;

namespace SecuLabel
{
    public partial class FaLiao : DevExpress.XtraEditors.XtraForm
    {
        ArrayList lstDeleteInfo = new ArrayList(); // 选中删除的信息
        bool bFirstAdd = true;
        string remark, meterialCode, dept, user;
        string ls_plant = "SSDP";

        int reqQty = 0;

        int iSelectionCounts = 0;
        public FaLiao()
        {
            InitializeComponent();
        }




        /// <summary>
        /// 获取材料编号
        /// </summary>
        private void getMeterialCode()
        {
            try
            {
                string sql = " select material_code from " + SecuGlobal.tbMaster + " where " +
                    " FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' order by material_code ";
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
                SecuGlobal.showNG(panelStatus, lblStatus, getVendorCode.Message);
            }
        }





        /// <summary>
        /// 窗体Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FaLiao_Load(object sender, EventArgs e)
        {
            this.Text = "社内发料";
            getMeterialCode();

            string sql = "select cname from " + SecuGlobal.mv_dept  + " order by nlssort(cname,'NLS_SORT=SCHINESE_PINYIN_M')";
            OracleDataReader odr = OracleHelper.ExecuteReader(sql);
            if (odr.HasRows)
            {
                cbDept.Properties.Items.Clear();
                while (odr.Read())
                {
                    cbDept.Properties.Items.Add(odr["cname"]);
                }
            }

            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");
        }



        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbRemark.Text) || string.IsNullOrEmpty(cbDept.Text) || string.IsNullOrEmpty(cbMeterial.Text) || string.IsNullOrEmpty(tbUser.Text))
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "请填写基本信息");
                return;
            }

            if (bFirstAdd)  // 判断是否是第一次写入数据
            {
                SecuGlobal.GridViewInitial(gridView1, gridControl1);
            }


            if (gridView1.RowCount > 0)    // 一个申请单号中不允许添加不同类型的CODE
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

            if (reqQty <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "请确认申请数量");
                return;
            }

            add();//添加数据到GridView
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            bFirstAdd = false;
        }




        /// <summary>
        /// 获取变量信息
        /// </summary>
        private void getVariable()
        {
            if (!string.IsNullOrEmpty(tbRemark.Text))
                remark = tbRemark.Text.Trim();

            meterialCode = cbMeterial.Text.Trim();
            dept = cbDept.Text.Trim();
            user = tbUser.Text;

            reqQty =System.Convert.ToInt32(tbQty.Text.Trim());
        }




       /// <summary>
       /// 添加信息
       /// </summary>
        private void add()
        {
            DataTable table = gridControl1.DataSource as DataTable;

            if (table == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Fct Code", typeof(string)));
                dt.Columns.Add(new DataColumn("材料", typeof(string)));
                dt.Columns.Add(new DataColumn("申请数量", typeof(string)));
                dt.Columns.Add(new DataColumn("领料人", typeof(string)));
                dt.Columns.Add(new DataColumn("所在部门", typeof(string)));
                dt.Columns.Add(new DataColumn("创建日期", typeof(string)));
                dt.Columns.Add(new DataColumn("创建时间", typeof(string)));
                dt.Columns.Add(new DataColumn("备注", typeof(string)));

                dt.Rows.Add(getField());
                gridControl1.DataSource = dt;

                GridCheckMarksSelection selection = new GridCheckMarksSelection(gridView1);
                selection.CheckMarkColumn.VisibleIndex = 0;
                selection.SelectionChanged += grdCheckSelect_SelectionChanged;
                gridView1.Columns["Fct Code"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["材料"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["领料人"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["申请数量"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["所在部门"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["创建日期"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["创建时间"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["备注"].OptionsColumn.ReadOnly = true;
            }
            else
            {
                table.Rows.Add(getField());
                gridControl1.DataSource = table;
            }

        }



        /// <summary>
        /// 选中信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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




        private object[] getField()
        {
            string sDate = DateTime.Now.ToShortDateString();
            string sTime = DateTime.Now.ToLongTimeString() + " : " + DateTime.Now.Millisecond.ToString();  //Delete key    删除数据的键值
            return new object[] { PaCSGlobal.LoginUserInfo.Fct_code, meterialCode,  reqQty , user, dept, sDate, sTime, remark  };
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





        private bool bAnalysisData()
        {
            int a = 0;

            if (gridView1.RowCount <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有要保存的数据!");
                return false;
            }

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                object valAssy = gridView1.GetRowCellValue(i, gridView1.Columns["材料"]);
                object valReq = gridView1.GetRowCellValue(i, gridView1.Columns["申请数量"]);
                if (valReq.ToString().Equals("0"))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, valAssy + ": 发料数不能为零");
                    return false;
                }

                if (!int.TryParse(valReq.ToString(), out a))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, valAssy + ": 发料数量不是有效数字");
                    return false;
                }

            }
            return true;
        }




        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!bAnalysisData())
                    return;

                DialogResult drg = MessageBox.Show("您确信要提交 " + gridView1.RowCount + "  条申请单吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
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
                        object valReq = gridView1.GetRowCellValue(i, gridView1.Columns["申请数量"]);

                        OracleHelper.ExecuteNonQuery(getSql1(doc_seq, (i + 1).ToString(), valAssy.ToString(), valReq.ToString(), "0","Y"));
                    }
                    SecuGlobal.showOK(panelStatus, lblStatus, "材料申请OK，申请单号：" + doc_seq);
                    btnSave.Enabled = true;
                    bFirstAdd = true;

                });
            }
            catch (Exception err)
            {
                SecuGlobal.showOK(panelStatus, lblStatus, err.Message);
            }
        }

        private string getSql2(string doc_seq)
        {
            string ls_company = "C660";
            string sql = " insert into  " + SecuGlobal.tbSecurityRequestH + "  " +
                        " ( " +
                        " req_doc,req_vendor,req_user,req_date,prod_plan_date,remark,plant,update_date,update_time,update_user,FCT_CODE " +
                        " ) " +
                        " VALUES " +
                        " ( " +
                        " '" + doc_seq + "','" + ls_company + "','" + user + "'," +
                        " to_char(sysdate,'yyyymmdd'),to_char(sysdate,'yyyymmdd'),'" + remark + "','" + ls_plant + "',to_char(sysdate,'yyyymmdd')," +
                        " to_char(sysdate,'HHMiss'),'" + PaCSGlobal.LoginUserInfo.Name  + "','" + PaCSGlobal.LoginUserInfo.Fct_code + "' " +
                        " ) ";
            return sql;
        }


        private string getSql1(string doc_seq, string req_seq, string assyCode, string req_count, string plan_count, string barcode1)
        {
            string userIp = PaCSGlobal.GetClientIp();
            string sql = " insert into  " + SecuGlobal.tbSecurityRequestD + "  " +
                        " ( " +
                        " REQ_DOC,REQ_SEQ,MATERIAL_CODE,REQ_QTY,PROD_PLAN_QTY,STATUS, " +
                        " GR_QTY,BARCODE_FLAG,CREATE_DATE,CREATE_TIME,CREATE_USER,CREATE_IP,PLANT,FCT_CODE " +
                        " ) " +
                        " VALUES " +
                        " ( " +
                        " '" + doc_seq + "','" + req_seq + "','" + assyCode + "','" + req_count + "','" + plan_count + "','RQ', " +
                        " '0','" + barcode1 + "',to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24miss'),'" + PaCSGlobal.LoginUserInfo.Name  + "','" + userIp + "','" + ls_plant + "','" + PaCSGlobal.LoginUserInfo.Fct_code + "' " +
                        " ) ";
            return sql;
        }

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

        private void tbQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 46)  //小数点
            {
                if (tbQty.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(tbQty.Text, out oldf);
                    b2 = float.TryParse(tbQty.Text + e.KeyChar.ToString(), out f);
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




    }
}