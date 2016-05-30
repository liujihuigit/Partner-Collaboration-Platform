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

namespace PCBCode
{
    public partial class Report : DevExpress.XtraEditors.XtraForm
    {
        public Report()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            try
            {
                gridControl1.DataSource = GetData();
                gridView1.BestFitColumns();
                gridView1.OptionsBehavior.Editable = false;

                gridView1.Columns[0].Width = 130;
                gridView1.Columns[0].SummaryItem.SummaryType = SummaryItemType.Count;
                gridView1.Columns[0].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";
            }
            catch (Exception Init)
            {
                XtraMessageBox.Show(this, "System error[Init]: " + Init.Message);
            }
        }

        private DataTable GetData()
        {
            StringBuilder sql = new StringBuilder("select substr(tool_code,0,11) \"PCB Code\",rprs_model_code \"Prod.Model\",tb_gubun_code \"T/B\"," +
                " to_char(to_date(fst_reg_dt,'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') \"创建时间\","+
                " (select u.fullname  from pacs_user u  where u.name = a.fst_reger_id) \"创建人\" from pacsm_md_tool_equip a ");
            sql.Append(" order by a.fst_reg_dt desc nulls last");

            return OracleHelper.ExecuteDataTable(sql.ToString());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string code = tbCode.Text.Trim();
            string model = tbModel.Text.Trim();
            string TB = cmbTB.SelectedItem.ToString().Substring(0, 1);

            if(string.IsNullOrEmpty(code))
            {
                XtraMessageBox.Show("请输入 PCB Code", "提示");
                tbCode.Focus();
                return;
            }

            if (string.IsNullOrEmpty(model))
            {
                XtraMessageBox.Show("请输入 Prod.Model", "提示");
                tbModel.Focus();
                return;
            }
            else if(!IsValid(model))
            {
                XtraMessageBox.Show("Prod.Model【" + model + "】不存在，请检查输入！", "提示");
                tbModel.Focus();
                return;
            }

            string mask_code = code + "-" + TB + "-00";

            if (IsExist(mask_code))
            {
                XtraMessageBox.Show("Mask Code【" + mask_code + "】已存在，不能重复输入！", "提示");
                tbModel.Focus();
                return;
            }

            Save(mask_code,model,TB);
            Init();
        }

        /// <summary>
        /// 是否有效Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsValid(string model)
        {
            string sql = "select * from tb_fpp_itemmaster x "+
            " where x.werks = 'P631' "+
            " and regexp_substr(x.matnr,'[^/]+') = '"+model+"' ";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 是否有效Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsExist(string code)
        {
            string sql = "select * from pacsm_md_tool_equip x " +
            " where x.fct_code = 'C660A' " +
            " and tool_gubun_code = 'M' "+
            " and tool_code = '" + code + "'";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="tool_code"></param>
        /// <param name="model"></param>
        /// <param name="TB"></param>
        /// <returns></returns>
        private void Save(string tool_code, string model, string TB)
        {
            string sql = "insert into pacsm_md_tool_equip(fct_code,tool_gubun_code,tool_code,rprs_model_code,tb_gubun_code,fst_reger_id)"+
                " values('C660A','M',:tool_code,:rprs_model_code,:tb_gubun_code,:fst_reger_id)";
            OracleParameter[] cmdParam = new OracleParameter[] {                
                new OracleParameter(":tool_code", OracleType.VarChar,30),
                new OracleParameter(":rprs_model_code", OracleType.VarChar,50),
                new OracleParameter(":tb_gubun_code", OracleType.VarChar,50),
                new OracleParameter(":fst_reger_id", OracleType.VarChar,100)
                };
            cmdParam[0].Value = tool_code;
            cmdParam[1].Value = model;
            cmdParam[2].Value = TB;
            cmdParam[3].Value = PaCSGlobal.LoginUserInfo.Name;

            int i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            
        }
    }
}