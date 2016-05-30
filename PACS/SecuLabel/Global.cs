using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using PaCSTools;
using DevExpress.XtraGrid;
using System.Drawing;

namespace SecuLabel
{
    class SecuGlobal
    {
        //public static Dictionary<String, String> dicVendor = new Dictionary<String, String>();   //PaCS - Pop Vendor对应关系  tb_security_cycle

        //基本信息
        public static string tbMaster = "TB_SECURITY_MASTER";   

        //保安标签批量上载
        public static string tbSecurityInvoice = "TB_SECURITY_INVOICE";  

        //保安标签扫描入库
        public static string tbSecurityInTest = "TB_SECURITY_IN";
        public static string tbSecurityInSnTest = "TB_SECURITY_IN_SN";


        //材料申请Header / Detail table
        public static string tbSecurityRequestD = "TB_SECURITY_REQUEST_D";
        public static string tbSecurityRequestH = "TB_SECURITY_REQUEST_H";

        //确认发料
        public static string tbSecurityOut = "TB_SECURITY_OUT";

        //申请单厂家处理
        public static string tbSecurityDoc = "TB_SECURITY_DOC";
        public static string tbSecurityStock = "TB_SECURITY_STOCK";  //库存

        //扫描到生产线
        public static string tbSnSecuHist = "TB_SN_SECU_HIST";
        public static string tbPopSecuIn = "POP_SECU_IN";


        //Comm DB 
        public static string tb_fpp_itemmaster = "TB_FPP_ITEMMASTER";
        public static string tb_code = "TB_CODE";
        public static string tb_lifnr = "TB_LIFNR";
        public static string tb_line_mapping = "TB_LINE_MAPPING";
        public static string tb_plan = "TB_PLAN";
        public static string pacsm_md_vend = "pacsm_md_vend";


        public static string mv_ep_dept = "mv_ep_dept";
        public static string mv_dept = "mv_dept";
        public static string mv_ep_vendor = "mv_ep_vendor";

        public static string strOperater = "";
        public static string strMeterialCode = "";
        public static string strBoardCount = "";
        public static string strBarcode = "";


        public static string getPopVendorInfo(string PacsVendorID, string PacsFctCode)
        {
            try
            {
                string sql = " select vend_code4||':'||vend_nm_cn " +
                             " from( " +
                             " select vend_code,vend_code4,vend_nm,vend_nm_cn " +
                             " from pacsm_md_vend  " +
                             " where instr(','||vend_func||',',',SECU_USE_VEND,') > 0 and fct_code = '" + PacsFctCode + "' " +
                             " ) " +
                             " where vend_code = '" + PacsVendorID + "' ";
                string buf = OracleHelper.ExecuteScalar(sql).ToString();
                if (!string.IsNullOrEmpty(buf))
                {
                    return buf;
                }

                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }


        public static void  setAllVendorInfo(string PacsFctCode, DevExpress.XtraEditors.ComboBoxEdit cbComBox)
        {
            try
            {
                string sql = " select vend_code4||':'||vend_nm_cn as vendorInfo " +
                            " from( " +
                            " select vend_code,vend_code4,vend_nm,vend_nm_cn " +
                            " from pacsm_md_vend  " +
                            " where instr(','||vend_func||',',',SECU_USE_VEND,') > 0 and fct_code = '" + PacsFctCode + "' " +
                            " ) ";

                OracleDataReader odr = OracleHelper.ExecuteReader(sql);

                if (odr.HasRows)
                {
                    cbComBox.Properties.Items.Clear();
                    while (odr.Read())
                    {
                        cbComBox.Properties.Items.Add(odr["vendorInfo"]);
                    }
                }
                else
                {
                    XtraMessageBox.Show("没有找到信息");
                }
            }
            catch (Exception getAllVendorInfo)
            {
                XtraMessageBox.Show("getAllVendorInfo-" + getAllVendorInfo.Message );
            }
        }




        static Dictionary<string, string> dicMoveMaster()
        {
            Dictionary<string, string> dicMove = new Dictionary<string, string>();

            dicMove.Add("DEF", "不良返退至SSDP");
            dicMove.Add("INI", "期初库存");
            dicMove.Add("LOS", "LOSS出库（丢失情况）");
            dicMove.Add("NOR", "正常入出库");
            dicMove.Add("RT1", "出库返退（剩余)");
            dicMove.Add("RT2", "出库返退（原材料不良）");
            dicMove.Add("RT3", "出库返退（作业不良）");
            dicMove.Add("RTN", "出库返退（单号取消）");
            dicMove.Add("TRS", "厂家间转移");

            return dicMove;
        }


        public static string getMoveType(string mvtType)
        {
            try
            {
                string buf = "";
                Dictionary<string, string> myDictionary = dicMoveMaster();
                buf = myDictionary[mvtType];

                if (string.IsNullOrEmpty(buf))
                {
                    return buf;
                }
                return buf;
            }
            catch (Exception)
            {
                return "";
            }
        }







        public static void showOK(DevExpress.XtraEditors.PanelControl panelControl,DevExpress.XtraEditors.LabelControl lblStatus,string str)
        {
            lblStatus.Text = str;
            panelControl.BackColor = Color.Yellow;
            panelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            //lblStatus.ForeColor = System.Drawing.Color.White;
        }

        public static void showNG(DevExpress.XtraEditors.PanelControl panelControl, DevExpress.XtraEditors.LabelControl lblStatus, string str)
        {
            lblStatus.Text = str;
            panelControl.BackColor = Color.Red;
            panelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            //lblStatus.ForeColor = System.Drawing.Color.Red;
        }



        public static void getMeterialCode(DevExpress.XtraEditors.ComboBoxEdit cbComBox)
        {
            try
            {

                string sql = " SELECT TRIM(material_code) FROM TB_security_master  where barcode_flag = 'Y'  ORDER BY material_code ";
                OracleDataReader odr = OracleHelper.ExecuteReader(sql);

                if (odr.HasRows)
                {
                    cbComBox.Properties.Items.Clear();
                    cbComBox.Properties.Items.Add("ALL");
                    while (odr.Read())
                    {
                        cbComBox.Properties.Items.Add(odr["TRIM(material_code)"]);
                    }
                }
                else
                {
                    XtraMessageBox.Show("没有找到信息");
                }

            }
            catch (Exception getVendorCode)
            {
                XtraMessageBox.Show(getVendorCode.Message);
            }

        }


        public static void setDate(DevExpress.XtraEditors.DateEdit dFrom, DevExpress.XtraEditors.DateEdit dTo)
        {
            try
            {
                string localdate = DateTime.Now.ToString("yyyy-mm-dd");
                string sqlString = "select to_char(sysdate-10,'yyyy-mm-dd') dFrom,to_char(sysdate,'yyyy-mm-dd') dTo from dual";
                OracleDataReader odr = OracleHelper.ExecuteReader(sqlString);
                if (odr.Read())
                {
                    dFrom.Text = odr["dFrom"].ToString();
                    dTo.Text = odr["dTo"].ToString();
                }
                else
                {
                    dFrom.Text = localdate;
                    dTo.Text = localdate;
                }

            }
            catch (Exception setDate)
            {
                XtraMessageBox.Show(setDate.Message);
            }
        }



        public static void setDate1(DevExpress.XtraEditors.DateEdit dFrom, DevExpress.XtraEditors.DateEdit dTo)
        {
            try
            {
                string localdate = DateTime.Now.ToString("yyyy-mm-dd");
                string sqlString = "select to_char(sysdate+1,'yyyy-mm-dd') dFrom,to_char(sysdate+3,'yyyy-mm-dd') dTo from dual";
                OracleDataReader odr = OracleHelper.ExecuteReader(sqlString);
                if (odr.Read())
                {
                    dFrom.Text = odr["dFrom"].ToString();
                    dTo.Text = odr["dTo"].ToString();
                }
                else
                {
                    dFrom.Text = localdate;
                    dTo.Text = localdate;
                }

            }
            catch (Exception setDate)
            {
                XtraMessageBox.Show(setDate.Message);
            }
        }





        public static void GridViewInitial(GridView gridView,GridControl gridControl)
        {
            gridView.ClearSelection();
            gridView.ClearColumnsFilter();
            gridView.ClearDocument();
            gridView.Columns.Clear();

            gridControl.DataSource = null;
            gridView.RefreshData();
        }



        public static void getLineInfo(DevExpress.XtraEditors.ComboBoxEdit cbComBox,string vend4 )
        {
            try
            {

                string sql = " select line_code||':'||line_desc as lineInfo from tb_line_mapping where pop_line_type = 'MAINS' and lifnr = '" + vend4 + "' order by line_code ";
                OracleDataReader odr = OracleHelper.ExecuteReader(sql);

                if (odr.HasRows)
                {
                    cbComBox.Properties.Items.Clear();
                    while (odr.Read())
                    {
                        cbComBox.Properties.Items.Add(odr["lineInfo"]);
                    }
                }
                else
                {
                    XtraMessageBox.Show("没有找到信息");
                }

            }
            catch (Exception getLineInfo)
            {
                XtraMessageBox.Show(getLineInfo.Message);
            }

        }





        //static Dictionary<string, string> dictVendor()
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>();

        //    dic.Add("ALL", "ALL");
        //    dic.Add("C660", "C660:SSDP");
        //    dic.Add("L1073S", "BP3A:成宇电子");
        //    dic.Add("L1073X", "D5G9:阿科帝斯");

        //    return dic;
        //}



        //public static string getVendorInfo(string vendID)
        //{
        //    try 
        //    {
        //        string buf = "";
        //        Dictionary<string, string> myDictionary = dictVendor();
        //        buf = myDictionary[vendID];

        //        if(string.IsNullOrEmpty(buf))
        //        {
        //            return buf;
        //        }
        //        return buf;
        //    }
        //    catch(Exception )
        //    {
        //        return "";
        //    }
        //}




        //public static void getVendorCode(DevExpress.XtraEditors.ComboBoxEdit cbComBox)
        //{
        //    try
        //    {

        //        string sql = " select vendorcode,vendorname  " +
        //                        "   from mv_ep_vendor " +
        //                        "  where trim(vendorip) is not null " +
        //                        "    and vendorcode in ('BP3A','BWW6','D5G9') ";
        //        OracleDataReader odr = OracleHelper.ExecuteReader(sql);

        //        if (odr.HasRows)
        //        {
        //            cbComBox.Properties.Items.Clear();
        //            cbComBox.Properties.Items.Add("ALL");
        //            cbComBox.Properties.Items.Add("C660 : SSDP");
        //            while (odr.Read())
        //            {
        //                cbComBox.Properties.Items.Add(odr["vendorcode"] + " : " + odr["vendorname"]);
        //                dicVendor.Add(odr["vendorcode"].ToString(), odr["vendorname"].ToString());

        //            }
        //        }
        //        else
        //        {
        //            XtraMessageBox.Show("没有找到信息");
        //        }

        //    }
        //    catch (Exception getVendorCode)
        //    {
        //        XtraMessageBox.Show(getVendorCode.Message);
        //    }
        //}


        //public static void setCombox( DevExpress.XtraEditors.ComboBoxEdit   cbVendor)
        //{
        //    Dictionary<string, string> myDictionary = dictVendor();
        //    foreach (var item in myDictionary)
        //    {
        //        cbVendor.Properties.Items.Add(item.Value);
        //        //(item.Key + item.Value);

        //    }

        //}



        // public static void getVendorCode(string flag,DevExpress.XtraEditors.ComboBoxEdit cbComBox)
        //{
        //    try
        //    {
        //        if(flag.Equals("A"))  //向后考虑 目前用不到
        //        {

        //        }

        //        string sql = " select vendorcode,vendorname  " +
        //                        "   from mv_ep_vendor " +
        //                        "  where vendorcode in ('BP3A','D5G9','C660') ";
        //        OracleDataReader odr = OracleHelper.ExecuteReader(sql);

        //        if (odr.HasRows)
        //        {
        //            cbComBox.Properties.Items.Clear();
        //            while (odr.Read())
        //            {
        //                cbComBox.Properties.Items.Add(odr["vendorcode"] + " : " + odr["vendorname"]);
        //                dicVendor.Add(odr["vendorcode"].ToString(), odr["vendorname"].ToString()); 

        //            }
        //        }
        //        else
        //        {
        //            XtraMessageBox.Show("没有找到信息");
        //        }

        //    }
        //    catch (Exception getVendorCode)
        //    {
        //        XtraMessageBox.Show(getVendorCode.Message);
        //    }


        //}




        //public static string  getVendorCode4(string vendId)
        //{
        //    try
        //    {

        //        string sql = " select vend_code from  " +
        //                        " ( " +
        //                        "     select vend_code,vend_code4,vend_nm,vend_nm_cn " +
        //                        "     from pacsm_md_vend  " +
        //                        "     where instr(','||vend_func||',',',SECU_USE_VEND,') > 0 and fct_code = 'C660A' " +
        //                        " ) " +
        //                        " where vend_code = '" + vendId + "' ";
        //        object  obj = OracleHelper.ExecuteScalar(sql);

        //        if (string.IsNullOrEmpty(obj.ToString()))
        //            return obj.ToString();
        //        else
        //            return "";
        //    }
        //    catch (Exception )
        //    {
        //        return "";
        //    }
        //}



    }
}
