using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PaCSTools;
using System.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using System.IO.Ports;
using System.Reflection;
using System.ComponentModel;

namespace MetalMask
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

    public class MetalMaskGlobal
    {
        //扫描枪
        public static SerialPort port = new SerialPort();
        //提示
        public static string ReportBtnClickTip = "MetalMask已搬出到其他厂家，不能再对其进行操作！";
        public static string KeyDownEventTip = "barcode不存在或已搬出到其他厂家";

        public static bool CheckBarcodeExist(string barcode)
        {
            string sql = "select  * "+
                " from pacsm_rm_tool a " +
                " where a.tool_id = '"+barcode+"' " ;
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
            public static bool CheckStatus(string currentStatus, string nextStatus)
        {
            string sql = "select  * " +
                " from pacsm_rm_comm_info a " +
                " where a.type_code = 'TOOL_MOVE' " +
                " and rm_grp_n3_code = 'MM'" +
                "  and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' " +
                " and del_yn = 'N'" +
                " and  a.rm_grp_n1_code = '" + currentStatus + "'" +
                " and a.rm_grp_n2_code = '" + nextStatus + "'";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public static string GetStatusNmByCode(string statusCode)
        {
            string returnString = "";
            string sql = "select comm_code_nm" +
                                " from pacsc_md_comm_code" +
                                " where type_code in" +
                                " (" +
                                " 'TOOL_STATUS'" +
                                " )" +
                                " and grp_code = 'MB'" +
                                " and comm_code = '" + statusCode + "' "+
                                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            DataTable dtResult = OracleHelper.ExecuteDataTable(sql);
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                returnString = dtResult.Rows[0]["comm_code_nm"].ToString();
            }
            return returnString;
        }

        public static void  LoadCmbStatus(ComboBoxEdit cmb)
        {
            string sql = "            select comm_code,comm_code_nm   " +
             "   from pacsc_md_comm_code                                         " +
             "    where type_code ='TOOL_STATUS'                               " +
             "    and grp_code = 'MB'                                                    " +
             "    and comm_code not in                                                 " +
             "    (                                                                                    " +
             "        'MBDIN', 'MBLIN', 'MBTIN', 'MBTOT'                         " +
             "    )                                                                                    "+
             " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            DataTable dtStatus = OracleHelper.ExecuteDataTable(sql);

            for (int i = 0; i < dtStatus.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtStatus.Rows[i]["comm_code_nm"].ToString();
                data.Value = dtStatus.Rows[i]["comm_code"].ToString();
                cmb.Properties.Items.Add(data);
            }
        }

        public static void LoadCmbMaskCode(ComboBoxEdit cmb)
        {
            string sql = "select distinct tool_code MaskCode from pacsm_md_tool_equip where  tool_gubun_code ='M' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            DataTable dtMCode = OracleHelper.ExecuteDataTable(sql);

            for (int i = 0; i < dtMCode.Rows.Count; i++)
            {
                cmb.Properties.Items.Add(dtMCode.Rows[i]["MaskCode"].ToString());
            }
        }

        public static void LoadCmbModel(ComboBoxEdit cmb)
        {
            string sql = "select distinct rprs_model_code Model from pacsm_md_tool_equip where  tool_gubun_code ='M' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            DataTable dtModel = OracleHelper.ExecuteDataTable(sql);

            for (int i = 0; i < dtModel.Rows.Count; i++)
            {
                cmb.Properties.Items.Add(dtModel.Rows[i]["Model"].ToString());
            }
        }

        /// <summary>
        /// 主页面 厂家
        /// </summary>
        /// <param name="cmb"></param>
        public static void LoadCmbVendor(ComboBoxEdit cmb)
        {
            DataTable dtVendor = GetSpecifiedVendor();

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtVendor.Rows[i]["vend_nm"].ToString();
                data.Value = dtVendor.Rows[i]["vend_code"].ToString();
                cmb.Properties.Items.Add(data);
            }
        }

        /// <summary>
        /// 制造原因
        /// </summary>
        /// <returns></returns>
         public RepositoryItemComboBox cmbMakerReason()
        {
            RepositoryItemComboBox cmbReason = new RepositoryItemComboBox();

            DataTable dtReason = GetMakerReason();

            for (int i = 0; i < dtReason.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtReason.Rows[i]["COMM_CODE_NM"].ToString();
                data.Value = dtReason.Rows[i]["COMM_CODE"].ToString();
                cmbReason.Items.Add(data);
            }
            cmbReason.ParseEditValue += new ConvertEditValueEventHandler(repositoryItemComboBox_ParseEditValue);
            return cmbReason;
        }

         public static DataTable GetMakerReason()
         {
             string sqlReason = "SELECT COMM_CODE,COMM_CODE_NM||':'||COMM_CODE COMM_CODE_NM FROM PACSC_MD_COMM_CODE WHERE TYPE_CODE = 'MAKE_RSN' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
             DataTable dtReason = OracleHelper.ExecuteDataTable(sqlReason);
             return dtReason;
         }


        /// <summary>
        /// 废弃原因
        /// </summary>
        /// <returns></returns>
        public RepositoryItemComboBox cmbDsuReason()
        {
            RepositoryItemComboBox cmbReason = new RepositoryItemComboBox();

            string sqlReason = "SELECT COMM_CODE,COMM_CODE_NM||':'||COMM_CODE COMM_CODE_NM FROM PACSC_MD_COMM_CODE WHERE TYPE_CODE = 'DSU_RSN' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            DataTable dtReason = OracleHelper.ExecuteDataTable(sqlReason);

            for (int i = 0; i < dtReason.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtReason.Rows[i]["COMM_CODE_NM"].ToString();
                data.Value = dtReason.Rows[i]["COMM_CODE"].ToString();
                cmbReason.Items.Add(data);
            }
            cmbReason.ParseEditValue += new ConvertEditValueEventHandler(repositoryItemComboBox_ParseEditValue);
            return cmbReason;
        }

        public RepositoryItemComboBox cmbLine()
        {
            RepositoryItemComboBox cmbLine = new RepositoryItemComboBox();
            cmbLine.TextEditStyle = TextEditStyles.DisableTextEditor;

            DataTable dtLine = GetVendorLine(PaCSGlobal.LoginUserInfo.Venderid);

            for (int i = 0; i < dtLine.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtLine.Rows[i]["line_nm"].ToString();
                data.Value = dtLine.Rows[i]["line_code"].ToString();
                cmbLine.Items.Add(data);
            }
            cmbLine.ParseEditValue += new ConvertEditValueEventHandler(repositoryItemComboBox_ParseEditValue);
            return cmbLine;

        }

        /// <summary>
        /// 修改制造厂家
        /// </summary>
        /// <returns></returns>
        public RepositoryItemComboBox cmbMakerVendor()
        {
            RepositoryItemComboBox cmbVendor = new RepositoryItemComboBox();
            cmbVendor.TextEditStyle = TextEditStyles.DisableTextEditor;

            DataTable dtVendor = GetMakerVendor();

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtVendor.Rows[i]["vend_nm"].ToString() + ":" + dtVendor.Rows[i]["vend_code"].ToString();
                data.Value = dtVendor.Rows[i]["vend_code"].ToString();
                cmbVendor.Items.Add(data);
            }
            cmbVendor.ParseEditValue += new ConvertEditValueEventHandler(repositoryItemComboBox_ParseEditValue);
            return cmbVendor;

        }

        /// <summary>
        /// 搬出厂家
        /// </summary>
        /// <returns></returns>
        public RepositoryItemComboBox cmbVendor()
        {
            RepositoryItemComboBox cmbVendor = new RepositoryItemComboBox();
            cmbVendor.TextEditStyle = TextEditStyles.DisableTextEditor;

            DataTable dtVendor = GetSpecifiedVendor();

            for (int i = 0; i < dtVendor.Rows.Count; i++)
            {
                ComboxData data = new ComboxData();
                data.Text = dtVendor.Rows[i]["vend_nm"].ToString() + ":" + dtVendor.Rows[i]["vend_code"].ToString();
                data.Value = dtVendor.Rows[i]["vend_code"].ToString();
                cmbVendor.Items.Add(data);
            }
            cmbVendor.ParseEditValue += new ConvertEditValueEventHandler(repositoryItemComboBox_ParseEditValue);
            return cmbVendor;

        }

        /// <summary>
        /// Bin基本信息(新的MM注册时，获取空闲状态的Bin)
        /// </summary>
        /// <param name="vendCode"></param>
        /// <returns></returns>
        public static DataTable GetRegisterBinInfo()
        {
            string sqlBin = "select f.comm_code Bin from pacsm_rm_comm_info f where  f.type_code = 'BIN' and f.rm_grp_n2_code = '" + PaCSGlobal.LoginUserInfo.Venderid + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'  " +
                    " minus " +
                    "select t.tool_bin_code from pacsm_rm_tool t where t.status_code not in('MBDEL','MBDSU') and t.vend_loc_code = '" + PaCSGlobal.LoginUserInfo.Venderid + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'  ";

            DataTable dtBin = OracleHelper.ExecuteDataTable(sqlBin);
            return dtBin;
        }

        /// <summary>
        /// Bin基本信息(入库时，选择全部Bin)
        /// </summary>
        /// <param name="vendCode"></param>
        /// <returns></returns>
        public static DataTable GetGRBinInfo()
        {
            string sqlBin = "select f.comm_code Bin from pacsm_rm_comm_info f where  f.type_code = 'BIN' and f.rm_grp_n2_code = '" + PaCSGlobal.LoginUserInfo.Venderid + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            DataTable dtBin = OracleHelper.ExecuteDataTable(sqlBin);
            return dtBin;
        }

        /// <summary>
        /// Line基本信息
        /// </summary>
        /// <param name="vendCode"></param>
        /// <returns></returns>
        public static DataTable GetVendorLine(string vendCode)
        {
            string sqlLine = "select line_code, line_nm||':'||line_code line_nm from gmes20_line w where w.vend_code  ='" + vendCode + "' and w.proc_type_code = '22' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            DataTable dtLine = OracleHelper.ExecuteDataTable(sqlLine);
            return dtLine;
        }

        /// <summary>
        /// 多个厂家 制造厂家
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMakerVendor()
        {
            string sqlVendor = "select a.comm_code vend_code,a.comm_code_nm vend_nm from pacsm_rm_comm_info a where rm_grp_n1_code = 'MM' and type_code = 'MM_MAKE_VEND' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor);
            return dtVendor;
        }

        /// <summary>
        /// 四个厂家(SSDP,大豪，星地，韩星)
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSpecifiedVendor()
        {
            string sqlVendor = "select distinct nvl(vend_code,'C660') vend_code,(select vend_nm_cn from pacsm_md_vend b where nvl(a.vend_code,'C660') = b.vend_code and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) vend_nm from gmes20_line a where  nvl(a.vend_code,'C660') in ('L100L2','L105OA','L10740','C660') and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            DataTable dtVendor = OracleHelper.ExecuteDataTable(sqlVendor);
            return dtVendor;
        }

        public RepositoryItemComboBox cmbStoreLoc()
        {
            RepositoryItemComboBox cmbStoreLoc = new RepositoryItemComboBox();
            cmbStoreLoc.TextEditStyle = TextEditStyles.DisableTextEditor;

            DataTable dtStoreLoc = GetGRBinInfo();

            for (int i = 0; i < dtStoreLoc.Rows.Count; i++)
            {
                cmbStoreLoc.Items.Add(dtStoreLoc.Rows[i]["Bin"].ToString());
            }

            cmbStoreLoc.ParseEditValue += new ConvertEditValueEventHandler(repositoryItemComboBox_ParseEditValue);
            return cmbStoreLoc;
        }

        private void repositoryItemComboBox_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            e.Value = e.Value.ToString();
            e.Handled = true;
        }

        public static bool WriteLog(string barcode, string flag)
        {
            string sql = "  insert into pacsm_rm_tool_h                                " +
                               " (                                                                               " +
                               " tool_id,fct_code,                                                  " +
                               " tool_gubun_code, tool_code,                                                     " +
                               " status_code, TOOL_BIN_CODE, TOOL_LINE_CODE,                                             " +
                               " make_vend_code, make_rsn_code, CARYIN_RSN,                                        " +
                               " tool_tens_value,tool_use_times,tool_use_times_add,caryot_rsn,                                      " +
                               " CARYOT_RECEIVER, CARYOT_RECEIVER_CONTCT,                              " +
                               " DSU_RSN_CODE, del_yn,                                                    " +
                               " CREATE_DT, CREATE_USER,                                                       " +
                               " UPDATE_DT, UPDATE_USER,                                                       " +
                               " TOOL_SN,VEND_LOC_CODE,                                                       " +
                               " tool_ver, vend_code,                                 " +
                               " make_rsn_cont,attachid,     " +//------------------------------------------
                               " dml_type_code                    " +
                               " )                                                                               " +
                               " select                                                                          " +
                               " tool_id,fct_code,                                                  " +
                               " tool_gubun_code, tool_code,                                                     " +
                               " status_code, TOOL_BIN_CODE, TOOL_LINE_CODE,                                             " +
                               " make_vend_code, make_rsn_code, CARYIN_RSN,                                        " +
                               " tool_tens_value,tool_use_times,tool_use_times_add,caryot_rsn,                                      " +
                               " CARYOT_RECEIVER, CARYOT_RECEIVER_CONTCT,                                  " +
                               " DSU_RSN_CODE, del_yn,                                                  " +
                               " CREATE_DT, CREATE_USER,                                                       " +
                               " UPDATE_DT, UPDATE_USER,                                                       " +
                               " tool_sn, VEND_LOC_CODE,                                                       " +
                               " tool_ver, vend_code,                                 " +
                               " make_rsn_cont,attachid,      " +//------------------------------------------
                               " '" + flag + "'                           " +
                               " from pacsm_rm_tool                                                              " +
                               " where tool_gubun_code = 'MM'                                                    " +
                               " and  tool_id = '" + barcode + "'" +
                               " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            int i = OracleHelper.ExecuteNonQuery(sql);
            if (i > 0)
                return true;
            else
                return false;
        }

    }
}
