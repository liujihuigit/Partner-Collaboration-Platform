using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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

namespace Toner
{
    public partial class AppendScan : XtraForm
    {
        public Dictionary<string, string> ReturnValue = new Dictionary<string, string>();//用这个公开属性传值
        private string frmName = "";
        public AppendScan(string frmName)
        {
            InitializeComponent();
            this.frmName = frmName;
        }

        private void AppendScan_Load(object sender, EventArgs e)
        {      
            Init();
            schDocno.Focus();
        }

        private void Init()
        {
            try
            {
                gridControl1.DataSource = GetData("");
                gridView1.BestFitColumns();

                gridView1.Columns[0].Width = 130;
                gridView1.Columns[0].SummaryItem.SummaryType = SummaryItemType.Count;
                gridView1.Columns[0].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";
            }
            catch (Exception Init)
            {
                 XtraMessageBox.Show(this, "System error[Init]: " + Init.Message);
            }
           
        }

        private DataTable GetData(string docno)
        {
              //取得可追加扫描的Docno（符合的条件为：pacsp_pm_box_prgs 表中last_doc_no 不含此docno）
                StringBuilder sql = new StringBuilder("select distinct doc_no DOCNO from pacsp_pm_box_prgs a where  (a.create_date between to_char(sysdate-1,'yyyyMMdd') and to_char(sysdate,'yyyyMMdd')) ");
                //sql.Append(" and not exists (select 1 from pacsp_pm_box_prgs b where b.last_doc_no=a.doc_no and cancel_flag !='1' ) ");
                //sql.Append(" and update_ip = '"+PaCSGlobal.GetClientIp()+"' ");
                sql.Append(" and not exists (select 1 from pacsp_pm_box_prgs b where nvl(b.last_doc_no,' ')=a.doc_no and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "') ");
                sql.Append(" and nvl(cancel_flag,'0') !='1' ");
                sql.Append(" and operation_window = '" + frmName + "' ");
                sql.Append(" and update_ip = '" + PaCSGlobal.GetClientIp() + "' ");
                sql.Append(" and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");

                if (!string.IsNullOrEmpty(docno))
                {
                    sql.Append(" and a.doc_no like '%" + docno + "%'");
                }
                sql.Append(" order by 1 desc");

                return OracleHelper.ExecuteDataTable(sql.ToString());
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            //DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hInfo = gridView1.CalcHitInfo(new Point(e.X, e.Y));  
            //判断光标是否在行范围内  
            //取得选定行信息  
            //string nodeName = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "M/Mask Code").ToString();

            GridView detailGrid = (sender as GridView);
            GridHitInfo hitInfo = (detailGrid.CalcHitInfo((e as MouseEventArgs).Location));
            //Leave if the user didn't double-click in a cell
            if (hitInfo.InRowCell == false)
                return;

            string docno = gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "DOCNO").ToString();

            ReturnValue.Add("DOCNO", docno);

            DialogResult = DialogResult.OK;

            this.Close();
        }

    }
}
