using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
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

namespace MetalMask
{
    public partial class Register2 : XtraForm
    {
        public Register2()
        {
            InitializeComponent();
        }

        private void BaseInfo_Load(object sender, EventArgs e)
        {
            gridView1.BestFitColumns();
            gridView1.OptionsCustomization.AllowColumnMoving = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(cmbMaker.Text.Equals(""))
            {
                XtraMessageBox.Show("请选择 Maker", "提示");
                cmbMaker.Focus();
                return;
            }

            if (tbBarcode.Text.Equals(""))
            {
                XtraMessageBox.Show("请选择 Barcode No", "提示");
                tbBarcode.Focus();
                return;
            }

            string sql = "insert into pacsm_rm_tool(fct_code,plant_code,tool_gubun_code,proc_type_code,tool_id,tool_code,tool_ver,rfid_code,tool_tens_value,fst_reg_dt) " +
               " values( 'C660A','P631','MM','22',:tool_id,:tool_code,:tool_ver,:rfid_code,:tool_tens_value,to_char(sysdate,'yyyyMMddhh24miss'))";
            OracleParameter[] cmdParam = new OracleParameter[] {                
                new OracleParameter(":tool_id", OracleType.VarChar,50),
                new OracleParameter(":tool_code", OracleType.VarChar,30),
                new OracleParameter(":tool_ver", OracleType.VarChar,50),
                new OracleParameter(":rfid_code", OracleType.VarChar,50),
                 new OracleParameter(":tool_tens_value", OracleType.VarChar,50)
                };
            cmdParam[0].Value = tbBarcode.Text.Trim();
            cmdParam[1].Value = tbMaskCode.Text.Trim();
            cmdParam[2].Value = tbPCBVer.Text.Trim();
            cmdParam[3].Value = tbRfid.Text.Trim();
            cmdParam[4].Value = tbTension.Text.Trim();

            int i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            if(i>0)
            {
                XtraMessageBox.Show("保存成功!", "提示");
                this.Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbProdModel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                BaseInfoForm frmNew = new BaseInfoForm();
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    tbMaskCode.Text = frmNew.ReturnValue["MaskCode"];
                    tbPCBCode.Text = frmNew.ReturnValue["PCBCode"];
                    tbProdModel.Text = frmNew.ReturnValue["ProductModel"];
                    tbPCBType.Text = "MAIN";
                    tbProcType.Text = "22";
                    tbPCBVer.Text = frmNew.ReturnValue["PCBVer"];
                    tbTB.Text = frmNew.ReturnValue["TB"];
                    tbArray.Text = frmNew.ReturnValue["Array"];
                    tbWidth.Text = frmNew.ReturnValue["Width"];
                    tbLength.Text = frmNew.ReturnValue["Length"];
                    tbThickness.Text = frmNew.ReturnValue["Thickness"];
                }
            }
            catch (Exception tbProdModel_ButtonClick)
            {
                XtraMessageBox.Show(this, "System error[tbProdModel_ButtonClick]: " + tbProdModel_ButtonClick.Message);
            }
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            //DataC
            gridView1.AddNewRow();
        }

        private void btnDelRow_Click(object sender, EventArgs e)
        {

        }
    }
}
