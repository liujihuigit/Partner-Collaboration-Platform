using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using PaCSTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toner
{
    public partial class BatchScrap : XtraForm
    {
        //public Dictionary<string, string> ReturnValue = new Dictionary<string, string>();//用这个公开属性传值
        private bool m_checkStatus = false;
        public BatchScrap()
        {
            InitializeComponent();
        }

        private void BatchScrap_Load(object sender, EventArgs e)
        {      
            Init();
            schboxno.Focus();
        }

        private void Init()
        {
            try
            {
                //实现DevExpress.GridControl表头全选功能
                this.gridView1.Click += new System.EventHandler(this.gridView1_Click);
                this.gridView1.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridView1_CustomDrawColumnHeader);
                this.gridView1.DataSourceChanged += new EventHandler(gridView1_DataSourceChanged);

                //checkbox列去掉中间状态
                RepositoryItemCheckEdit checkEdit = new RepositoryItemCheckEdit();
                checkEdit.NullStyle = StyleIndeterminate.Unchecked;

                gridControl1.DataSource = GetData();
                gridView1.BestFitColumns();

                gridView1.Columns["check"].Width = 30;
                gridView1.Columns["check"].ColumnEdit = checkEdit;
                gridView1.Columns[1].SummaryItem.SummaryType = SummaryItemType.Count;
                gridView1.Columns[1].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";

                foreach (GridColumn col in gridView1.Columns)
                {
                    if (col.FieldName.Equals("check"))
                    {
                        col.OptionsColumn.AllowEdit = true;
                        col.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                    }
                    else
                        col.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception Init)
            {
                 XtraMessageBox.Show(this, "System error[Init]: " + Init.Message);
            }        
        }

        private DataTable GetData()
        {
              //取得可废弃的碳粉记录
            StringBuilder sql = new StringBuilder(" select  " +
            " box_label \"桶编号\",item \"材料编号\",    " +
            " (select vend_nm_cn from pacsm_md_vend b where a.make_vend_code = b.vend_code and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "')  \"生产厂家\", " +

            " qty \"数量\",box_status \"开封状态\",box_case_status \"运输状态\" " +

            " from pacsd_pm_box a where 1=1 ");
            sql.Append(" and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'  ");
            sql.Append(" and final_vend_to = '" + PaCSGlobal.LoginUserInfo.Venderid + "'  ");
            sql.Append(" and operation_window ='LINE'  ");

            sql.Append(" order by create_date||create_time||update_date||update_time desc nulls last");
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());
            dtResult.Columns.Add("check", System.Type.GetType("System.Boolean"));
            dtResult.Columns["check"].SetOrdinal(0);

            return dtResult;
        }

        public List<string> tonerList()
        {
            List<string> list = new List<string>();
            string value = "";
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                value = gridView1.GetDataRow(i)["check"].ToString();
                if (value == "True")
                {
                    list.Add(gridView1.GetRowCellValue(i, gridView1.Columns["桶编号"]).ToString());
                }
            }
            return list;
        }

        private void btnScrap_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = XtraMessageBox.Show("确认废弃?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    foreach (string toner in tonerList())
                    {
                        Dispose(toner);
                    }
                }
                else
                    return;
            }
            catch (Exception btnScrap_Click)
            {
                    XtraMessageBox.Show(this, "System error[btnScrap_Click]: " + btnScrap_Click.Message);
            }
        }

        /// <summary>
        /// 厂家 废弃
        /// </summary>
        private void Dispose(string data)
        {
            string sql = "update pacsd_pm_box set final_move_type = '551',final_move_code = 'MOVE0601',final_doc_no = :final_doc_no," +
                " last_doc_no = final_doc_no,operation_window = 'SCRAP',box_case_status = :box_case_status,box_status = :box_status," +
                " update_date = to_char(sysdate,'yyyyMMdd'),update_time = to_char(sysdate,'hh24miss'),update_user = :update_user,update_ip = :update_ip " +
                " where box_label = '" + data + "' " +
                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            OracleParameter[] cmdParam = new OracleParameter[] {
                    new OracleParameter(":final_doc_no", OracleType.VarChar, 50),
                    new OracleParameter(":update_user", OracleType.VarChar, 50), 
                    new OracleParameter(":update_ip", OracleType.VarChar, 50),
                    new OracleParameter(":box_case_status", OracleType.VarChar, 50),
                    new OracleParameter(":box_status", OracleType.VarChar, 50)
                    };
            cmdParam[0].Value = TonerGlobal.GenerateDocNo();
            cmdParam[1].Value = PaCSGlobal.LoginUserInfo.Id;
            cmdParam[2].Value = PaCSGlobal.GetClientIp();

            DataTable dtStatus = TonerGlobal.GetCommInfoByCode("MOVE0601");
            if (dtStatus.Rows.Count > 0)
            {
                cmdParam[3].Value = dtStatus.Rows[0]["BOX_CASE_STATUS"].ToString();
                cmdParam[4].Value = dtStatus.Rows[0]["BOX_STATUS"].ToString();
            }
            else
            {
                cmdParam[3].Value = "";
                cmdParam[4].Value = "";
            }

            int i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            //插入prog表
            TonerGlobal.InsertIntoProg(data);
            //提示成功语音
            PaCSGlobal.PlayWavOk();
            XtraMessageBox.Show("废弃成功！", "提示");
        }

        #region 实现DevExpress.GridControl表头全选功能
        private void gridView1_Click(object sender, EventArgs e)
        {
            if (DevControlHelper.ClickGridCheckBox(this.gridView1, "check", m_checkStatus))
            {
                m_checkStatus = !m_checkStatus;
            }
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column != null && e.Column.FieldName == "check")
            {
                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);
                DevControlHelper.DrawCheckBox(e, m_checkStatus);
                e.Handled = true;
            }
        }

        void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            GridColumn column = this.gridView1.Columns.ColumnByFieldName("check");
            if (column != null)
            {
                column.Width = 80;
                column.OptionsColumn.ShowCaption = false;
                column.ColumnEdit = new RepositoryItemCheckEdit();
            }
        }
        #endregion

    }
}
