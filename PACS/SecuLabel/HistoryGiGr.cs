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
using System.Data.OracleClient;

namespace SecuLabel
{
    public partial class HistoryGiGr : DevExpress.XtraEditors.XtraForm
    {
        string LS_SDATE="", LS_EDATE="", LS_COMPANY="", LS_DOC_TYPE="", LS_MATERIAL_CODE="", ls_plant="";
        public static Dictionary<String, String> dicVendor = new Dictionary<String, String>(); 
        public HistoryGiGr()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 数据导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "正在导出数据,请稍等...");
            PaCSGlobal.ExportGridToFile(grdView1, "Histoty Info");
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
        }


        /// <summary>
        /// 数据后台查询
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
                SecuGlobal.showNG(panelStatus, lblStatus, ex.Message );
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }




        /// <summary>
        /// 获取查询的SQL
        /// </summary>
        /// <param name="LS_SDATE"></param>
        /// <param name="LS_EDATE"></param>
        /// <param name="LS_COMPANY"></param>
        /// <param name="LS_DOC_TYPE"></param>
        /// <param name="LS_MATERIAL_CODE"></param>
        /// <param name="ls_plant"></param>
        /// <returns>可以直接执行的Sql</returns>
        private string getSql(string LS_SDATE, string LS_EDATE, string LS_COMPANY, string LS_DOC_TYPE,string LS_MATERIAL_CODE, string ls_plant)
        {
            string sql = " SELECT COMPANY,  " +
                            "        DOC_NO,  " +
                            "        DOC_SEQ,  " +
                            "        DOC_TYPE,  " +
                            "        DOC_MVT,  " +
                            "        MATERIAL_CODE,  " +
                            "        (select b.description from " + SecuGlobal.tb_fpp_itemmaster  + " b where a.material_code = b.matnr) description, " +
                            "        DOC_QTY,  " +
                            "        MOVE_COMPANY, " +
                            "        MOVE_LINE, " +
                            "        REQ_DOC,  " +
                            "        REQ_SEQ,  " +
                            "        DOC_DATE,  " +
                            "        DOC_TIME,  " +
                            "        DOC_USER,  " +
                            "        DOC_IP,  " +
                            "        BK_COMPANY,  " +
                            "        BK_DOC_NO,  " +
                            "        BK_DOC_SEQ,  " +
                            "        BARCODE_FLAG,  " +
                            "        BOXNO,  " +
                            "        ROLLNO,  " +
                            "        LOTNO,  " +
                            "        SN_FROM,  " +
                            "        SN_TO,  " +
                            "        PROD_LOTNO,  " +
                            "        REMARK,  " +
                            "        UPDATE_DATE,  " +
                            "        UPDATE_TIME,  " +
                            "        UPDATE_USER,  " +
                            "        UPDATE_IP, " +
                            "        prod_plan_date, " +
                            "        plant " +
                            " FROM " + SecuGlobal.tbSecurityDoc  + " a " +
                            " WHERE DOC_DATE BETWEEN '" + LS_SDATE + "' AND '" + LS_EDATE + "' " +
                            " AND COMPANY LIKE  '" + LS_COMPANY + "' " +
                            " AND DOC_TYPE LIKE '" + LS_DOC_TYPE  + "' " +
                            " AND MATERIAL_CODE LIKE  '" + LS_MATERIAL_CODE  + "' " +
                            " and a.plant like  '" + ls_plant  + "' and a.FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                            " ORDER BY DOC_DATE DESC,DOC_TIME DESC ";
            return sql;
        }


        /// <summary>
        /// 变量赋值
        /// </summary>
        private void getVariable()
        {

            LS_COMPANY = cbVendor.Text.Trim();

            if (!string.IsNullOrEmpty(LS_COMPANY))
            {
                if (!LS_COMPANY.Equals("ALL"))
                {
                    string[] split = LS_COMPANY.Split(new Char[] { ':' });
                    LS_COMPANY = split[0].Trim();
                    LS_COMPANY = LS_COMPANY + "%";
                }
                else
                {
                    LS_COMPANY = "%";
                }
            }


            if (cbPlant.SelectedIndex != -1)
                ls_plant  = cbPlant.Properties.Items[cbPlant.SelectedIndex].ToString();

            if (!string.IsNullOrEmpty(ls_plant))
            {
                if (!ls_plant.Equals("ALL"))
                    ls_plant = ls_plant + "%";
                else
                    ls_plant = "%";
            }


            if (cbMoveType.SelectedIndex != -1)
                LS_DOC_TYPE  = cbMoveType.Properties.Items[cbMoveType.SelectedIndex].ToString();

            if (!string.IsNullOrEmpty(LS_DOC_TYPE))
            {
                if (!LS_DOC_TYPE.Equals("ALL"))
                {
                    string[] split = LS_DOC_TYPE.Split(new Char[] { ':' });
                    LS_DOC_TYPE = split[0].Trim();
                    LS_DOC_TYPE = LS_DOC_TYPE + "%";
                }
                else
                {
                    LS_DOC_TYPE = "%";
                }
            }

            if (!string.IsNullOrEmpty(tbMeterial.Text))
                LS_MATERIAL_CODE = tbMeterial.Text.Trim() + "%";
            else
                LS_MATERIAL_CODE = "%";

            LS_SDATE = dateEditFrom.Text.Trim().Replace("-","");
            LS_EDATE = dateEditTo.Text.Trim().Replace("-", "");


        }





        /// <summary>
        /// 更改GirdView标题栏及显示顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader(DataTable dt)
        {
            //标题栏
            dt.Columns["plant"].ColumnName = "Plant";
            dt.Columns["COMPANY"].ColumnName = "厂家";
            dt.Columns["DOC_NO"].ColumnName = "单号";
            dt.Columns["DOC_SEQ"].ColumnName = "序号";
            dt.Columns["DOC_TYPE"].ColumnName = "移库方向";
            dt.Columns["DOC_MVT"].ColumnName = "移库类型";
            dt.Columns["MATERIAL_CODE"].ColumnName = "材料";
            dt.Columns["description"].ColumnName = "材料描述";
            dt.Columns["DOC_QTY"].ColumnName = "数量";
            dt.Columns["MOVE_COMPANY"].ColumnName = "接受厂家";
            dt.Columns["MOVE_LINE"].ColumnName = "接受生产LINE";
            dt.Columns["REQ_DOC"].ColumnName = "Req Doc";
            dt.Columns["REQ_SEQ"].ColumnName = "Req Seq";
            dt.Columns["prod_plan_date"].ColumnName = "生产计划日期";
            dt.Columns["DOC_DATE"].ColumnName = "Doc Date";
            dt.Columns["DOC_TIME"].ColumnName = "Doc Time";
            dt.Columns["DOC_USER"].ColumnName = "Doc User";
            dt.Columns["DOC_IP"].ColumnName = "Doc IP";
            dt.Columns["BK_COMPANY"].ColumnName = "Bk Company";
            dt.Columns["BK_DOC_NO"].ColumnName = "Bk Doc No";
            dt.Columns["BK_DOC_SEQ"].ColumnName = "Bk Doc Seq";
            dt.Columns["BARCODE_FLAG"].ColumnName = "条形码";
            dt.Columns["PROD_LOTNO"].ColumnName = "Prod LotNo";
            dt.Columns["REMARK"].ColumnName = "备注";
            dt.Columns["UPDATE_DATE"].ColumnName = "更新日期";
            dt.Columns["UPDATE_TIME"].ColumnName = "更新时间";
            dt.Columns["UPDATE_USER"].ColumnName = "更新人";
            dt.Columns["UPDATE_IP"].ColumnName = "更新IP";

            //顺序改变
            dt.Columns["Plant"].SetOrdinal(0);
            dt.Columns["厂家"].SetOrdinal(1);
            dt.Columns["单号"].SetOrdinal(2);
            dt.Columns["序号"].SetOrdinal(3);
            dt.Columns["移库方向"].SetOrdinal(4);
            dt.Columns["移库类型"].SetOrdinal(5);
            dt.Columns["材料"].SetOrdinal(6);
            dt.Columns["材料描述"].SetOrdinal(7);
            dt.Columns["数量"].SetOrdinal(8);
            dt.Columns["接受厂家"].SetOrdinal(9);
            dt.Columns["接受生产LINE"].SetOrdinal(10);
            dt.Columns["Req Doc"].SetOrdinal(11);
            dt.Columns["Req Seq"].SetOrdinal(12);
            dt.Columns["生产计划日期"].SetOrdinal(13);
            dt.Columns["Doc Date"].SetOrdinal(14);
            dt.Columns["Doc Time"].SetOrdinal(15);
            dt.Columns["Doc User"].SetOrdinal(16);
            dt.Columns["Doc IP"].SetOrdinal(17);
            dt.Columns["Bk Company"].SetOrdinal(18);
            dt.Columns["Bk Doc No"].SetOrdinal(19);
            dt.Columns["Bk Doc Seq"].SetOrdinal(20);
            dt.Columns["条形码"].SetOrdinal(21);
            dt.Columns["Prod LotNo"].SetOrdinal(22);
            dt.Columns["备注"].SetOrdinal(23);
            dt.Columns["更新日期"].SetOrdinal(24);
            dt.Columns["更新时间"].SetOrdinal(25);
            dt.Columns["更新人"].SetOrdinal(26);
            dt.Columns["更新IP"].SetOrdinal(27);

            //隐藏字段
            dt.Columns["BOXNO"].SetOrdinal(28);
            dt.Columns["ROLLNO"].SetOrdinal(29);
            dt.Columns["LOTNO"].SetOrdinal(30);
            dt.Columns["SN_FROM"].SetOrdinal(31);
            dt.Columns["SN_TO"].SetOrdinal(32);

            return dt;
        }




        public static void setDicVendor(string PacsFctCode)
        {
            try
            {
                string sql = " select vend_code4,vend_nm_cn " +
                            " from pacsm_md_vend  " +
                            " where instr(','||vend_func||',',',SECU_USE_VEND,') > 0 and fct_code = '" + PacsFctCode + "' ";

                OracleDataReader odr = OracleHelper.ExecuteReader(sql);
                dicVendor.Clear();
                if (odr.HasRows)
                {

                    while (odr.Read())
                    {
                        dicVendor.Add(odr["vend_code4"].ToString(), odr["vend_nm_cn"].ToString());
                    }
                }

            }
            catch (Exception setDicVendor)
            {
                XtraMessageBox.Show("setDicVendor-" + setDicVendor.Message);
            }
        }


        /// <summary>
        /// 后台数据查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                getVariable();
                DataTable dt = OracleHelper.ExecuteDataTable(getSql(LS_SDATE ,LS_EDATE ,LS_COMPANY ,LS_DOC_TYPE,LS_MATERIAL_CODE ,ls_plant ));
                dt = setDtHeader(dt);

                string vendorDesc = "";


                this.Invoke((MethodInvoker)delegate
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dicVendor.TryGetValue(dr["厂家"].ToString(), out vendorDesc);
                        dr["厂家"] = vendorDesc;

                        if (dr["移库方向"].ToString().Equals("GI"))
                            dr["移库方向"] = "出库";
                        else
                            dr["移库方向"] = "入库";

                    }

                    gridControl1.DataSource = dt;
                    grdView1.BestFitColumns();

                    grdView1.Columns["单号"].SummaryItem.SummaryType = SummaryItemType.Count;
                    grdView1.Columns["单号"].SummaryItem.DisplayFormat = "All : {0:f0}";

                    grdView1.Columns["Plant"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdView1.Columns["厂家"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdView1.Columns["单号"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdView1.Columns["序号"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdView1.Columns["移库方向"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdView1.Columns["移库类型"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    //grdView1.Columns["材料"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    //grdView1.Columns["材料描述"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdView1.Columns["数量"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    grdView1.Columns["接受厂家"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

                    grdView1.Columns["材料描述"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near ;
                    grdView1.Columns["数量"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far ;
                    grdView1.Columns["备注"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near ;
                    
                    //隐藏字段
                    grdView1.Columns["BOXNO"].Visible = false;
                    grdView1.Columns["ROLLNO"].Visible = false;
                    grdView1.Columns["LOTNO"].Visible = false;
                    grdView1.Columns["SN_FROM"].Visible = false;
                    grdView1.Columns["SN_TO"].Visible = false;
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                });
            }
            catch (Exception err)
            {
                //XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
                SecuGlobal.showOK(panelStatus, lblStatus, err.Message);
            }
        }




        /// <summary>
        /// Load基本信息加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HistoryGiGr_Load(object sender, EventArgs e)
        {
            setDicVendor(PaCSGlobal.LoginUserInfo.Fct_code);
            if (dicVendor.Count <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有找到VENDOR 信息，请联系管理员");
                return;
            }


            SecuGlobal.setDate(dateEditFrom, dateEditTo);
            SecuGlobal.setAllVendorInfo(PaCSGlobal.LoginUserInfo.Fct_code ,cbVendor);
            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");
        }

        private void grdView1_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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