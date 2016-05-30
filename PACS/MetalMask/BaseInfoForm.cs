using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraSplashScreen;
using PaCSTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetalMask
{
    public partial class BaseInfoForm : XtraForm
    {
        public Dictionary<string, string> ReturnValue = new Dictionary<string, string>();//用这个公开属性传值
        public BaseInfoForm()
        {
            InitializeComponent();
        }

        private void BaseInfoForm_Load(object sender, EventArgs e)
        {      
            Init();
            btnSearch.Focus();
        }

        private void Init()
        {
            try
            {
                //string sqlMaskCode = "select tool_code from pacsm_md_tool_equip t where  t.tool_gubun_code ='M' ";
                string sqlPCBCode = "select substr(tool_code,0,11) pcbcode from pacsm_md_tool_equip t where  t.tool_gubun_code ='M' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
                string sqlModel = "select rprs_model_code from pacsm_md_tool_equip t where  t.tool_gubun_code ='M' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

                //DataTable dtMaskCode = OracleHelper.ExecuteDataTable(sqlMaskCode);
                DataTable dtPCBCode = OracleHelper.ExecuteDataTable(sqlPCBCode);
                DataTable dtModel = OracleHelper.ExecuteDataTable(sqlModel);

                //this.cmbMaskCode.Properties.NullText = "ALL";
                this.cmbPCBCode.Properties.NullText = "ALL";
                this.cmbProdModel.Properties.NullText = "ALL";

                //for (int i = 0; i < dtMaskCode.Rows.Count; i++)
                //{
                //    cmbMaskCode.Properties.Items.Add(dtMaskCode.Rows[i]["tool_code"].ToString());
                //}

                for (int i = 0; i < dtPCBCode.Rows.Count; i++)
                {
                    cmbPCBCode.Properties.Items.Add(dtPCBCode.Rows[i]["pcbcode"].ToString());
                }

                for (int i = 0; i < dtModel.Rows.Count; i++)
                {
                    cmbProdModel.Properties.Items.Add(dtModel.Rows[i]["rprs_model_code"].ToString());
                }
            }
            catch (Exception Init)
            {
                 XtraMessageBox.Show(this, "System error[Init]: " + Init.Message);
            }
           
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("查询失败", "提示");
            }     
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            StringBuilder sql = new StringBuilder("select t.tool_code \"M/Mask Code\",substr(tool_code,0,11) \"PCB Code\", t.rprs_model_code \"Product. Model\" ,t.tool_ver \"PCB Ver\"," +
" t.tb_gubun_code \"T/B\",t.tool_array_num \"Array\",t.tool_leng \"Width\",t.tool_heit \"Length\",t.tool_thic \"Thickness\" " +
" from pacsm_md_tool_equip t where  t.tool_gubun_code ='M' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");

            //if(!cmbMaskCode.Text.Equals("ALL"))
            //{
            //    sql.Append(" and tool_code = '" + cmbMaskCode.Text + "'");
            //}

            if (!cmbPCBCode.Text.Equals("ALL"))
            {
                sql.Append(" and tool_code like '%" + cmbPCBCode.Text + "%'");
            }

            if (!cmbProdModel.Text.Equals("ALL"))
            {
                sql.Append(" and rprs_model_code = '" + cmbProdModel.Text + "'");
            }

            DataTable dt = OracleHelper.ExecuteDataTable(sql.ToString());

            this.Invoke((MethodInvoker)delegate
            {
                gridControl1.DataSource = dt;
                gridView1.BestFitColumns();

                gridView1.Columns[0].Width = 130;
                gridView1.Columns[0].SummaryItem.SummaryType = SummaryItemType.Count;
                gridView1.Columns[0].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";

                gridView1.Columns[1].Width = 130;
                gridView1.Columns[2].Width = 130;
                gridView1.Columns[3].Width = 100;
                gridView1.Columns[4].Width = 100;
                gridView1.Columns[5].Width = 100;
                gridView1.Columns[6].Width = 100;
                gridView1.Columns[7].Width = 100;
                gridView1.Columns[8].Width = 100;
            });         
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();  
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            //DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hInfo = gridView1.CalcHitInfo(new Point(e.X, e.Y));  
            //判断光标是否在行范围内  
            //取得选定行信息  
            //string nodeName = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "M/Mask Code").ToString();
            string MaskCode = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "M/Mask Code").ToString();
            string PCBCode = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "PCB Code").ToString();
            string ProductModel = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "Product. Model").ToString();
            string PCBVer = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "PCB Ver").ToString();
            string TB = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "T/B").ToString();
            string Array = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "Array").ToString();
            string Width = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "Width").ToString();
            string Length = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "Length").ToString();
            string Thickness = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "Thickness").ToString();

            //XtraMessageBox.Show("test:" + MaskCode);

            ReturnValue.Add("MaskCode", MaskCode);
            ReturnValue.Add("PCBCode", PCBCode);
            ReturnValue.Add("ProductModel", ProductModel);
            ReturnValue.Add("PCBVer", PCBVer);
            ReturnValue.Add("TB", TB);
            ReturnValue.Add("Array", Array);
            ReturnValue.Add("Width", Width);
            ReturnValue.Add("Length", Length);
            ReturnValue.Add("Thickness", Thickness);

            DialogResult = DialogResult.OK;

            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
