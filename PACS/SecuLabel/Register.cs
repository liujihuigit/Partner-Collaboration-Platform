using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PaCSTools;

namespace SecuLabel
{
    public partial class Register : DevExpress.XtraEditors.XtraForm
    {
        public Register()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (bAnalysisCondition() != true )
                return;
            
            switch (SecuGlobal.strOperater) 
            {
                case "ADD":
                    if (bExistRecord())
                        return;

                    InsertData();
                    SecuGlobal.strMeterialCode = txtMeterialCode.Text.Trim();
                    break;

                case "UPDATE":
                    if (txtQty.Text.Equals(SecuGlobal.strBoardCount) && cbBarcode.Text.Equals(SecuGlobal.strBarcode))
                    {
                        XtraMessageBox.Show("数据没有变化，请再次确认", "提示");
                        return;
                    }
                    UpdateData();
                    break;
            };
        }





        /// <summary>
        /// 判断材料编号是否已经存在
        /// </summary>
        /// <returns>True / False </returns>
        private bool bExistRecord()
        {
            string sql = "select * from " + SecuGlobal.tbMaster + " where material_code = '" + txtMeterialCode.Text.Trim() +"' " +
                         "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                XtraMessageBox.Show("编号以存在，请再次确认", "提示");
                return true;
            }
                
            return false;
        }





        /// <summary>
        /// 用户输入信息检查-（格式/长度/数字等)
        /// </summary>
        /// <returns>True / False </returns>
        private Boolean bAnalysisCondition()
        {
            try
            {
                if (txtMeterialCode.Text.Equals(""))
                {
                    XtraMessageBox.Show("请输入基本信息", "提示");
                    txtMeterialCode.Focus();
                    return false;
                }

                int i = txtMeterialCode.Text.IndexOf('-');
                if (txtMeterialCode.Text.Length != 11 || txtMeterialCode.Text.Contains('-') == false || i != 4)
                {
                    XtraMessageBox.Show("材料编号输入错误，请重新确认", "提示");
                    txtMeterialCode.Focus();
                    return false;
                }

                if (txtQty.Text.Equals("") || (int.Parse(txtQty.Text) <= 0))
                {
                    XtraMessageBox.Show("请确认包装数量", "提示");
                    txtQty.Focus();
                    return false;
                }

                if (cbBarcode.Text.Equals(""))
                {
                    XtraMessageBox.Show("请选择条码信息", "提示");
                    cbBarcode.Focus();
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                XtraMessageBox.Show(e.Message, "异常错误");
                return false;
            }
        }






        /// <summary>
        /// 数据写入DB (tb_security_master)
        /// </summary>
        private void InsertData()
        {
            try
            {
                string sql = "insert into " + SecuGlobal.tbMaster +
                    " (MATERIAL_CODE,BARCODE_FLAG,BOARD_COUNT,BOX_COUNT,UPDATE_DATE,UPDATE_TIME,UPDATE_USER,FCT_CODE) " +
                    " VALUES(:MATERIAL_CODE,:BARCODE_FLAG,:BOARD_COUNT,'',to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24miss'),:UPDATE_USER,:FCT_CODE)";
                OracleParameter[] cmdParam = new OracleParameter[] {                
                new OracleParameter(":MATERIAL_CODE", OracleType.VarChar,18),
                new OracleParameter(":BARCODE_FLAG", OracleType.VarChar,1),
                new OracleParameter(":BOARD_COUNT", OracleType.VarChar,6),
                new OracleParameter(":UPDATE_USER", OracleType.VarChar,16),
                new OracleParameter(":FCT_CODE", OracleType.VarChar,10)
                };
                cmdParam[0].Value = txtMeterialCode.Text.Trim();
                cmdParam[1].Value = cbBarcode.Text.Trim();
                cmdParam[2].Value = txtQty.Text.Trim();
                cmdParam[3].Value = PaCSTools.PaCSGlobal.LoginUserInfo.Name ;  // 当前登录用户
                cmdParam[4].Value = PaCSTools.PaCSGlobal.LoginUserInfo.Fct_code ;  // 当前登录用户

                int i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
                if (i > 0)
                {
                    XtraMessageBox.Show("保存成功!", "提示");
                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ee)
            {
                XtraMessageBox.Show(ee.Message, "失败");
            }
        }







        /// <summary>
        /// 数据UPDATE (tb_security_master)
        /// </summary>
        private void UpdateData()
        {
            try
            {
                string sql = "update " + SecuGlobal.tbMaster +
                    " set BOARD_COUNT = '" + txtQty.Text.Trim() + "',BARCODE_FLAG = '" + cbBarcode.Text.Trim()  + "', " + 
                    " UPDATE_DATE = to_char(sysdate,'yyyymmdd'),UPDATE_TIME = to_char(sysdate,'hh24miss'), " +
                    " UPDATE_USER = '" + PaCSTools.PaCSGlobal.LoginUserInfo.FullName + "' " +
                    " where MATERIAL_CODE = '" + txtMeterialCode.Text.Trim() + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
 
                int i = OracleHelper.ExecuteNonQuery(sql);
                if (i > 0)
                {
                    XtraMessageBox.Show("数据更新成功!", "提示");
                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ee)
            {
                XtraMessageBox.Show(ee.Message, "失败");
            }
        }







        private void tbnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 资材号码强制限制输入大写
        /// </summary>
        private void txtMeterialCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122)
            {
                e.KeyChar = (char)((int)e.KeyChar - 32);
            }
        }


        private void Register_Load(object sender, EventArgs e)
        {
            if (SecuGlobal.strOperater.Equals("UPDATE"))
            {
                txtMeterialCode.Enabled = false;
                txtMeterialCode.Text = SecuGlobal.strMeterialCode;
                txtQty.Text = SecuGlobal.strBoardCount;
                cbBarcode.Text = SecuGlobal.strBarcode;
            }

        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 46)  //小数点
            {
                if (txtQty.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(txtQty.Text, out oldf);
                    b2 = float.TryParse(txtQty.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void txtMeterialCode_Validated(object sender, EventArgs e)
        {
            string fctCode = PaCSGlobal.LoginUserInfo.Fct_code;
            if (fctCode.Equals("C660A"))  // ssdp 可以验证FPP ITEM MASTER 信息
            {
                string sql = "select COUNT(*) from " + SecuGlobal.tb_fpp_itemmaster  + " where matnr = '" + txtMeterialCode.Text.Trim() + "' and fct_code = '" + fctCode + "'";
                int i = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql.ToString()));

                if (i != 1)
                {
                    XtraMessageBox.Show("错误的编号");
                }
 
            }
        }
    }
}