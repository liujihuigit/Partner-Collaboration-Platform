using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using PaCSTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Toner
{
    public class ComboxData
    {
        public string Text { set; get; }
        public string Value { set; get; }

        public override string ToString()
        {
            return Text;
        }
    }

    class TonerGlobal
    {
        public static SerialPort port = new SerialPort();
        public static string ScanEventTip = "";

        /// <summary>
        /// 碳粉生产厂家
        /// </summary>
        /// <param name="cmb"></param>
        public static void LoadCmbVendor(ComboBoxEdit cmb)
        {
            //cmb.Properties.Items.Clear();
            string sqlVendor = "select vend_code,vend_nm,vend_nm_cn from pacsm_md_vend where instr(','||vend_func||',',',TONER_MAKE_VEND,') > 0 and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor);

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtVendor.Rows[i]["vend_nm_cn"].ToString();
                data.Value = dtVendor.Rows[i]["vend_code"].ToString();
                cmb.Properties.Items.Add(data);
            }
        
        }

        /// <summary>
        /// 碳粉使用厂家(其他，不包括登录者厂家)
        /// </summary>
        /// <param name="cmb"></param>
        public static void LoadCmbUseVendor(ComboBoxEdit cmb,bool except)
        {
            //cmb.Properties.Items.Clear();
            StringBuilder sqlVendor = new StringBuilder("select vend_code,vend_nm,vend_nm_cn from pacsm_md_vend where instr(','||vend_func||',',',TONER_USE_VEND,') > 0 and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");
            if(except)
            {
                sqlVendor.Append(" and vend_code != '" + PaCSGlobal.LoginUserInfo.Venderid + "'");
            }
            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor.ToString());

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtVendor.Rows[i]["vend_nm_cn"].ToString();
                data.Value = dtVendor.Rows[i]["vend_code"].ToString();
                cmb.Properties.Items.Add(data);
            }
            cmb.SelectedIndex = -1;
        }

        /// <summary>
        /// 材料编号
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="vend_code"></param>
        public static void LoadItemByVendCode(ComboBoxEdit cmb, string vend_code)
        {
            cmb.Properties.Items.Clear();
            StringBuilder sqlVendor = new StringBuilder("select item from pacsm_md_toner where fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");
            if (!string.IsNullOrEmpty(vend_code))
                sqlVendor.Append(" and vend_code = '" + vend_code + "'");
            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor.ToString());

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                cmb.Properties.Items.Add(dtVendor.Rows[i]["item"].ToString());
            }
            cmb.SelectedIndex = -1;
        }

        /// <summary>
        /// 厂家Stock信息
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="vendCode"></param>
        public static void LoadStockByVendCode(ComboBoxEdit cmb)
        {
            cmb.Properties.Items.Clear();
            StringBuilder sqlVendor = new StringBuilder("select comm_code code,comm_code_nm name from pacsc_md_comm_code " +
                " where comm_code_desc = '" + PaCSGlobal.LoginUserInfo.Venderid + "' "+
                " and type_code ='PACS_BOX_STOCK' "+
                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");

            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor.ToString());

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtVendor.Rows[i]["name"].ToString();
                data.Value = dtVendor.Rows[i]["code"].ToString();
                cmb.Properties.Items.Add(data);
            }
            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// 厂家楼层信息
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="vendCode"></param>
        public static void LoadBufferByVendCode(ComboBoxEdit cmb)
        {
            cmb.Properties.Items.Clear();
            StringBuilder sqlVendor = new StringBuilder("select comm_code code,comm_code_nm name from pacsc_md_comm_code "+
                " where comm_code_desc = '" + PaCSGlobal.LoginUserInfo.Venderid + "' "+
                " and type_code ='PACS_BOX_BUFFER' "+
                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'  ");

            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor.ToString());

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtVendor.Rows[i]["name"].ToString();
                data.Value = dtVendor.Rows[i]["code"].ToString();
                cmb.Properties.Items.Add(data);
            }
            cmb.SelectedIndex = -1;
        }

        /// <summary>
        /// 厂家Line信息
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="vendCode"></param>
        public static void LoadLineByBufferCode(ComboBoxEdit cmb,string buffer)
        {
            cmb.Properties.Items.Clear();
            DataTable dtVendor = GetVendorLineByBuffer(buffer);

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtVendor.Rows[i]["line_nm"].ToString();
                data.Value = dtVendor.Rows[i]["line_code"].ToString();
                cmb.Properties.Items.Add(data);
            }
            cmb.SelectedIndex = -1;
        }

        /// <summary>
        /// Line基本信息
        /// </summary>
        /// <param name="vendCode"></param>
        /// <returns></returns>
        public static DataTable GetVendorLineByBuffer(string bufferCode)
        {
            string sqlLine = "select a.comm_code line_code,a.comm_code_nm line_nm from pacsc_md_comm_code a " +
                " where type_code = 'PACS_BOX_LINE' "+
                " and comm_code_desc = '" + PaCSGlobal.LoginUserInfo.Venderid + "' "+
                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'  " +
                " and a.mlang_id = '" + bufferCode + "'";
            DataTable dtLine = OracleHelper.ExecuteDataTable(sqlLine);
            return dtLine;
        }

        /// <summary>
        /// 运输状态
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="vendCode"></param>
        public static void LoadBoxCaseStatus(ComboBoxEdit cmb)
        {
            //cmb.Properties.Items.Clear();
            StringBuilder sqlVendor = new StringBuilder("select comm_code code,comm_code_nm name from pacsc_md_comm_code " +
                " where  type_code ='PACS_BOX_CASE_STATUS' "+
                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");

            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor.ToString());

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtVendor.Rows[i]["name"].ToString();
                data.Value = dtVendor.Rows[i]["code"].ToString();
                cmb.Properties.Items.Add(data);
            }
            cmb.SelectedIndex = -1;
        }

        /// <summary>
        /// 将操作Detail，插入Prog表
        /// </summary>
        /// <param name="docno"></param>
        /// <param name="bucketLabel"></param>
        /// <param name="move_type"></param>
        /// <param name="vend_from"></param>
        /// <param name="vend_to"></param>
        public static void InsertIntoProg(string box_label)
        {
            string sql = "insert into pacsp_pm_box_prgs(doc_no,last_doc_no,box_label,item,make_vend_code,lot_no,box_no,qty,"+
                " move_code,move_type,vend_from,vend_to,stock_to,buffer_to,line_to,erp_info,box_case_status,box_status,operation_window,"+
                " create_date,create_time,create_ip,create_user,update_date,update_time,update_user,update_ip,fct_code) " +
           " select final_doc_no,last_doc_no,a.box_label,item,make_vend_code,lot_no,box_no,qty,"+
           " final_move_code,final_move_type,final_vend_from,final_vend_to,final_stock_to,final_buffer_to,final_line_to,final_erp_info,box_case_status,box_status,operation_window," +
           " create_date,create_time,create_ip,create_user,update_date,update_time,update_user,update_ip,fct_code "+
           " from pacsd_pm_box a where a.box_label = '" + box_label + "' and a.fct_code = '"+PaCSGlobal.LoginUserInfo.Fct_code+"'";

            int i = OracleHelper.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 更新Prog表，置为cancel_flag 1
        /// </summary>
        /// <param name="docno"></param>
        /// <param name="bucketLabel"></param>
        /// <param name="move_type"></param>
        /// <param name="vend_from"></param>
        /// <param name="vend_to"></param>
        public static void UpdateProg(string docno, string box_label)
        {
            string sql = "update pacsp_pm_box_prgs set cancel_flag = '1'    "+
           " where box_label = '" + box_label + "'      "+
           " and doc_no = '" + docno + "'   "+
           " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'   ";

            int i = OracleHelper.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        /// <param name="lastdocno"></param>
        /// <param name="data"></param>
        public static void Cancel(string lastdocno, string data)
        {
            //删除Box表中原纪录
            string sql = "delete pacsd_pm_box where box_label = '" + data + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            int i = OracleHelper.ExecuteNonQuery(sql);

            //回退到上一条记录
            string sql2 = "insert into pacsd_pm_box(final_doc_no,last_doc_no,box_label,item,make_vend_code,lot_no,box_no,qty," +
                " final_move_code,final_move_type,final_vend_from,final_vend_to,final_stock_to,final_buffer_to,final_line_to," +
                " box_case_status,box_status,operation_window,create_date,create_time,create_user,create_ip,fct_code) " +
                " select doc_no,last_doc_no,box_label,item,make_vend_code,lot_no,box_no,qty,move_code,move_type,vend_from,vend_to,stock_to,buffer_to,line_to," +
                " box_case_status,box_status,operation_window,create_date,create_time,create_user,create_ip,fct_code from pacsp_pm_box_prgs " +
                " where box_label = '" + data + "'  "+
                " and doc_no = '" + lastdocno + "'  "+
                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            int i2 = OracleHelper.ExecuteNonQuery(sql2);
        }

        /// <summary>
        /// 设置GridView属性
        /// </summary>
        /// <param name="dtData"></param>
        /// <param name="gridView1"></param>
        /// <param name="gridControl1"></param>
        public static void SetGridView(DataTable dtData, GridView gridView1, GridControl gridControl1)
        {
            gridControl1.DataSource = dtData;
            gridView1.BestFitColumns();
            gridView1.OptionsCustomization.AllowColumnMoving = false;//禁止列拖动
            gridView1.OptionsView.AllowCellMerge = true;
            gridView1.OptionsBehavior.Editable = false;

            if (dtData != null)
            {
                foreach (GridColumn col in gridView1.Columns)
                {
                    if (!col.FieldName.Equals("DOCNO"))
                    {
                        col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False; ;
                    }
                }
            }
        }

        /// <summary>
        /// 获取扫描到的Toner信息
        /// </summary>
        /// <param name="box_lable"></param>
        /// <returns></returns>
        public static DataTable ScanRecordStatus(string box_lable)
        {
            string sql = "select final_doc_no currentDocno,last_doc_no lastdocno,final_move_type movetype,final_move_code movecode,operation_window,final_vend_to,final_buffer_to,final_stock_to from pacsd_pm_box" +
                " where box_label = '" + box_lable + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 自动生成DocNo号 
        /// </summary>
        /// <returns></returns>
        public static string GenerateDocNo()
        {
            string docno = "";
            string sql = "select to_char(sysdate,'yyyymmdd')||fn_gene_seq('TONER',to_char(sysdate,'yyyymmdd'),'" + PaCSGlobal.LoginUserInfo.Fct_code + "','N','N','N',4) doc_no from dual";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                docno = dt.Rows[0][0].ToString();
            return docno;
        }

        /// <summary>
        /// 获取comm_code_name
        /// </summary>322222222222222222222222222
        /// <returns></returns>
        public static string GetNameByCode(string code)
        {
            string sql = " select comm_code_nm name from pacsc_md_comm_code " +
                                " where type_code is 'PACS_BOX_MOVE_TYPE' " +
                                " and comm_code = '" + code + "' ";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            return dt.Rows[0][0].ToString();
        }


        /// <summary>
        /// 获取comm_info
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCommInfoByCode(string code)
        {
            string sql = " select rm_attr_n2_nm BOX_CASE_STATUS,rm_attr_n3_nm BOX_STATUS from pacsm_rm_comm_info " +
                                " where type_code = 'PACS_BOX_MOVE' " +
                                " and comm_code = '" + code + "' " +
                                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' "+
                                " order by comm_code";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            return dt;
        }




    }
}
