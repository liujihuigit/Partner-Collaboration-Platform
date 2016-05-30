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

namespace SecuLabel
{
    public partial class StockQuery : DevExpress.XtraEditors.XtraForm
    {
        String ls_plant = "", company = "", ls_material_code= "";

        public static Dictionary<String, String> dicVendor = new Dictionary<String, String>(); 

        public StockQuery()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "数据导出中, 请稍等...");
            PaCSGlobal.ExportGridToFile(grdView1, "Stock Query");
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
        }




        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "数据查询中, 请稍等...");
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
        /// 返回可执行SQL 
        /// </summary>
        /// <param name="company"></param>
        /// <param name="ls_material_code"></param>
        /// <param name="ls_plant"></param>
        /// <returns></returns>
        private string getSql(string company, string ls_material_code,string ls_plant)
        {
            string sql = " select plant, company , material_code  , " +
                         " (select b.description from " + SecuGlobal.tb_fpp_itemmaster  + " b where b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' and a.material_code = b.matnr) description, " +
                         "  stock_qty , trans_qty " +
                         " from " + SecuGlobal.tbSecurityStock  + " a " +
                         " where company like '" + company + "' " +
                         " and material_code like '" + ls_material_code + "' " +
                         " and a.plant like '" + ls_plant + "' and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                         " and not exists (select 1 from " + SecuGlobal.tbMaster  + " b where a.material_code = b.material_code and b.barcode_flag = 'Y') " +
                         " order by company,material_code ";

            return sql;
        }



        /// <summary>
        /// 变量赋值
        /// </summary>
        private void getVariable()
        {
            company = cbVendor.Text.Trim();

            if (!string.IsNullOrEmpty(company))
            {
                if (!company.Equals("ALL"))
                {
                    string[] split = company.Split(new Char[] { ':' });
                    company = split[0].Trim();
                    company = company + "%";
                }
                else
                {
                    company = "%";
                }
            }


            if (cbPlant.SelectedIndex != -1)
                ls_plant = cbPlant.Properties.Items[cbPlant.SelectedIndex].ToString();

            if (!string.IsNullOrEmpty(ls_plant))
            {
                if (!ls_plant.Equals("ALL"))
                    ls_plant = ls_plant + "%";
                else
                    ls_plant = "%";
            }

            if (!string.IsNullOrEmpty(tbMeterial.Text))
                ls_material_code = tbMeterial.Text.Trim() + "%";
            else
                ls_material_code = "%";


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

                getVariable();
                DataTable dt = OracleHelper.ExecuteDataTable(getSql(company,ls_material_code,ls_plant));

                if (dt != null)
                {
                    dt.Columns["plant"].ColumnName = "厂家";
                    dt.Columns["material_code"].ColumnName = "材料";
                    dt.Columns["description"].ColumnName = "材料描述";
                    dt.Columns["stock_qty"].ColumnName = "厂家库存";
                    dt.Columns["trans_qty"].ColumnName = "厂家转移库存";
                }

                string vendorDesc="";
                foreach(DataRow dr in dt.Rows)
                {
                    dicVendor.TryGetValue(dr["COMPANY"].ToString(),out vendorDesc);
                    dr["COMPANY"] = vendorDesc;
                }

                this.Invoke((MethodInvoker)delegate
                {
                    gridControl1.DataSource = dt;

                    grdView1.BestFitColumns();
                    grdView1.Columns["材料"].SummaryItem.SummaryType = SummaryItemType.Count;
                    grdView1.Columns["材料"].SummaryItem.DisplayFormat = "All:{0:f0}";

                    grdView1.Columns["厂家库存"].SummaryItem.SummaryType = SummaryItemType.Sum ;
                    grdView1.Columns["厂家库存"].SummaryItem.DisplayFormat = "{0:f0}";


                    grdView1.Columns["厂家转移库存"].SummaryItem.SummaryType = SummaryItemType.Sum;
                    grdView1.Columns["厂家转移库存"].SummaryItem.DisplayFormat = "{0:f0}";

                    //指定列对齐方式
                    grdView1.Columns["材料描述"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    grdView1.Columns["厂家库存"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far ;
                    grdView1.Columns["厂家转移库存"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far ;
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
        /// 加载Vendor信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockQuery_Load(object sender, EventArgs e)
        {

            setDicVendor(PaCSGlobal.LoginUserInfo.Fct_code);
            if (dicVendor.Count <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有找到VENDOR 信息，请联系管理员");
                return;
            }


            SecuGlobal.setAllVendorInfo(PaCSGlobal.LoginUserInfo.Fct_code, cbVendor);
            SecuGlobal.showOK(panelStatus , lblStatus, "Ready");
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
        /// GRIDVIEW 行号
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



    }
}