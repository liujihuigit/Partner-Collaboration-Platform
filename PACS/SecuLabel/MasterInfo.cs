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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;

namespace SecuLabel
{
    
    public partial class MasterInfo : DevExpress.XtraEditors.XtraForm
    {
        public MasterInfo()
        {
            InitializeComponent();
        }


        private void MasterInfo_Load(object sender, EventArgs e)
        {
            subStart();
            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");
        }




        /// <summary>
        /// 加载材料编号信息
        /// </summary>
        private void subStart()
        {
            string sql = "select distinct(material_code) from " + SecuGlobal.tbMaster + " where FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);

            cbMeterial.Properties.Items.Clear();
            cbMeterial.Properties.Items.Add("ALL");
            for(int i = 0 ; i < dt.Rows.Count ; i++)     
            {
                string strName = dt.Rows[i]["material_code"].ToString();

                cbMeterial.Properties.Items.Add(strName);
            }  

        }


        /// <summary>
        /// INSERT OK 之后，显示对应的数据
        /// </summary>
        /// <param name="buf"></param>
        private void ShowData(string buf)
        {

            try
            {

                StringBuilder sql = new StringBuilder("select fct_code, material_code," +
                                    " (select b.description from " + SecuGlobal.tb_fpp_itemmaster  + " b where a.material_code = b.matnr) description," +
                                    "  barcode_flag, board_count,  update_date, update_time, update_user" +
                                    "  from " + SecuGlobal.tbMaster + " a  where 1=1 ");

                if (!string.IsNullOrEmpty(buf))
                {
                    sql.Append(" and material_code = '" + buf + "'");
                }

                sql.Append(" and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' order by update_date desc,update_time desc");

                SecuGlobal.GridViewInitial(grdView1, gridControl1);
                DataTable dt  = OracleHelper.ExecuteDataTable(sql.ToString());
                dt = setDtHeader(dt);
                gridControl1.DataSource = dt;
                setGirdView();
            }
            catch (Exception ShowData)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, ShowData.Message);
            }

        }





        /// <summary>
        /// 查询数据，后台执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {

            try
            {
                if (cbMeterial.Text.Equals(""))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请选择信息后查询！");
                    return;
                }

                SecuGlobal.showOK(panelStatus, lblStatus, "查询数据中,请稍等...");
                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));
                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            }
            catch (Exception ex)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, ex.Message );
            }

        }





        /// <summary>
        /// 添加新纪录
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "添加数据中,请稍等...");
                SecuGlobal.strOperater = "ADD";
                Register frmNew = new Register();
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    //ShowData(SecuGlobal.strMeterialCode );
                    ShowData(null);
                }

                SecuGlobal.strMeterialCode = "";
                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            }
            catch (Exception btnRegister_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnRegister_Click.Message);
            }
        }




        private void update()
        {
            SecuGlobal.strMeterialCode = grdView1.GetRowCellValue(grdView1.GetSelectedRows()[0], "材料").ToString();
            SecuGlobal.strBoardCount = grdView1.GetRowCellValue(grdView1.GetSelectedRows()[0], "包装单位").ToString();
            SecuGlobal.strBarcode = grdView1.GetRowCellValue(grdView1.GetSelectedRows()[0], "条码").ToString();
            SecuGlobal.strOperater = "UPDATE";
            Register frmNew = new Register();
            DialogResult dg = frmNew.ShowDialog();
            if (dg == DialogResult.OK)
            {

                ShowData(null);
                //ShowData(SecuGlobal.strMeterialCode);
                SecuGlobal.strMeterialCode = "";
                SecuGlobal.strBoardCount = "";
                SecuGlobal.strBarcode = "";
            }
        }



        /// <summary>
        /// Update 之后数据显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "更新数据中，请稍等...");
            update();
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
        }




        /// <summary>
        /// 数据删除及显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {

            SecuGlobal.showOK(panelStatus, lblStatus, "删除数据中，请稍等...");
            SecuGlobal.strMeterialCode = grdView1.GetRowCellValue(grdView1.GetSelectedRows()[0], "材料").ToString();
            string sql = "delete from " + SecuGlobal.tbMaster + " where material_code = '" + SecuGlobal.strMeterialCode + "'";
            DialogResult result = XtraMessageBox.Show("确定删除：" + SecuGlobal.strMeterialCode + "?", "删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                OracleHelper.ExecuteNonQuery(sql);
                ShowData(null);
                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            }
            else
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "用户取消，数据未删除");
                return; 
            }
        }



        /// <summary>
        /// 导出数据到EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click_1(object sender, EventArgs e)
        {
            SecuGlobal.showOK(panelStatus, lblStatus, "导出数据中,请稍等...");
            PaCSGlobal.ExportGridToFile(grdView1, "Master Info");
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
        }




        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();  
        }



        private DataTable setDtHeader(DataTable dt)
        {
            dt.Columns["fct_code"].ColumnName = "Fct Code";
            dt.Columns["material_code"].ColumnName = "材料";
            dt.Columns["description"].ColumnName = "材料描述";
            dt.Columns["barcode_flag"].ColumnName = "条码";
            dt.Columns["board_count"].ColumnName = "包装单位";
            dt.Columns["update_date"].ColumnName = "更新日期";
            dt.Columns["update_time"].ColumnName = "更新时间";
            dt.Columns["update_user"].ColumnName = "更新人";
            return dt;
        }




        /// <summary>
        /// 获取可执行的sql
        /// </summary>
        /// <param name="ls_material_code"></param>
        /// <returns></returns>
        private string  getSql()
        {

            string ls_material_code = "";
            if (cbMeterial.SelectedIndex != -1)
            {
                ls_material_code = cbMeterial.Properties.Items[cbMeterial.SelectedIndex].ToString();
            }
            else
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "请选择材料编号");
                return "";
            }

                 
            if (!string.IsNullOrEmpty(ls_material_code))
            {
                if (!ls_material_code.Equals("ALL"))
                    ls_material_code = ls_material_code + "%";
                else
                    ls_material_code = "%";
            }


            string sql =  " select fct_code ,material_code,  " +
                             "        (select b.description from " + SecuGlobal.tb_fpp_itemmaster  + " b where a.material_code = b.matnr) description, " +
                             "                barcode_flag,  " +
                             "                board_count,  " +
                             "                update_date,  " +
                             "                update_time,  " +
                             "                update_user " +
                             " from " + SecuGlobal.tbMaster  + " a " +
                             " where material_code like '" + ls_material_code + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" +
                             " order by material_code " ;
            return sql;
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


                DataTable dt = OracleHelper.ExecuteDataTable(getSql());
                if (dt != null)
                    dt = setDtHeader(dt);
                else
                    return;

                this.Invoke((MethodInvoker)delegate
                {
                    grdView1.Columns.Clear();
                    gridControl1.DataSource = dt;
                    setGirdView();
                });
            }
            catch (Exception err)
            {
               XtraMessageBox.Show(this, "System error[ShowData]: " + err.Message);
            }
            
        }


        private void setGirdView()
        {
            grdView1.BestFitColumns();
            grdView1.Columns["材料"].SummaryItem.SummaryType = SummaryItemType.Count;
            grdView1.Columns["材料"].SummaryItem.DisplayFormat = "All : {0:f0}";
        }



        /// <summary>
        /// 行号显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdView1_CustomDrawRowIndicator_1(object sender, RowIndicatorCustomDrawEventArgs e)
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
        /// 双击时间，显示需要UPDATE 的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdView1_MouseDown_1(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hInfo = grdView1.CalcHitInfo(new Point(e.X, e.Y));
            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            { //判断光标是否在行范围内 
                if (hInfo.InRow)
                {
                    update();
                }
            }
        }



    }
}