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
    public partial class Register : XtraForm
    {
        public Register()
        {
            InitializeComponent();
        }

        private void BaseInfo_Load(object sender, EventArgs e)
        {
            LoadCombobox();
        }

        private  void LoadCombobox()
        {
            try
            {
                DataTable dtReason = MetalMaskGlobal.GetMakerReason();
                DataTable dtStoreLoc = MetalMaskGlobal.GetRegisterBinInfo();
                DataTable dtMaker = MetalMaskGlobal.GetMakerVendor();

                for (int i = 0; i < dtReason.Rows.Count; i++)
                {
                    ComboxData data = new ComboxData();
                    data.Text = dtReason.Rows[i]["COMM_CODE_NM"].ToString();
                    data.Value = dtReason.Rows[i]["COMM_CODE"].ToString();
                    cmbReason.Properties.Items.Add(data);
                }

                for (int i = 0; i < dtStoreLoc.Rows.Count; i++)
                {
                    cmbStore.Properties.Items.Add(dtStoreLoc.Rows[i]["Bin"].ToString());
                }

                for (int i = 0; i < dtMaker.Rows.Count; i++)
                {
                    ComboxData data = new ComboxData();
                    data.Text = dtMaker.Rows[i]["vend_nm"].ToString();
                    data.Value = dtMaker.Rows[i]["vend_code"].ToString();
                    cmbMaker.Properties.Items.Add(data);
                }
                
                  ComboxData dataOwer = new ComboxData();
                    dataOwer.Text = PaCSGlobal.LoginUserInfo.Vendername;
                    dataOwer.Value = PaCSGlobal.LoginUserInfo.Venderid;
                    cmbOwerVend.Properties.Items.Add(dataOwer);

                if(!PaCSGlobal.LoginUserInfo.Venderid.Equals("C660"))
                {
                    ComboxData dataOwer2 = new ComboxData();
                    dataOwer2.Text = "SSDP";
                    dataOwer2.Value = "C660";
                    cmbOwerVend.Properties.Items.Add(dataOwer2);
                }

                    cmbOwerVend.SelectedIndex = 0;
            }
            catch (Exception LoadCombobox)
            {
                XtraMessageBox.Show(this, "System error[LoadCombobox]: " + LoadCombobox.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbProdModel.Text.Equals(""))
                {
                    XtraMessageBox.Show("请选择 Prod.Model", "提示");
                    tbProdModel.Focus();
                    return;
                }

                if (cmbStore.SelectedIndex == -1)
                {
                    XtraMessageBox.Show("请选择 Store Loc.", "提示");
                    cmbStore.Focus();
                    return;
                }

                if (cmbMaker.SelectedIndex == -1)
                {
                    XtraMessageBox.Show("请选择 Maker", "提示");
                    cmbMaker.Focus();
                    return;
                }

                if (tbBarcode.Text.Equals(""))
                {
                    XtraMessageBox.Show("请输入 Barcode No", "提示");
                    tbBarcode.Focus();
                    return;
                }

                if (MetalMaskGlobal.CheckBarcodeExist(tbBarcode.Text.Trim()))
                  {
                      XtraMessageBox.Show("此 BarcodeNo 已存在！", "提示");
                      tbBarcode.Focus();
                      return;
                  }

                string sql = "insert into pacsm_rm_tool(tool_gubun_code,del_yn,CREATE_DT,status_code,tool_id,tool_code,tool_ver,tool_tens_value,tool_use_times,CREATE_USER,TOOL_BIN_CODE,make_vend_code,make_rsn_code,make_rsn_cont,TOOL_SN,vend_code,vend_loc_code,attachid) " +
                   " values( 'MM','N',to_char(sysdate,'yyyyMMddhh24miss'),'MBNIN',:tool_id,:tool_code,:tool_ver,:tool_tens_value,:tool_use_times,:CREATE_USER,:TOOL_BIN_CODE,:make_vend_code,:make_rsn_code,:make_rsn_cont,:TOOL_SN,:vend_code,:vend_loc_code,:attachid)";
                OracleParameter[] cmdParam = new OracleParameter[] {                
                new OracleParameter(":tool_id", OracleType.VarChar,50),
                new OracleParameter(":tool_code", OracleType.VarChar,30),
                new OracleParameter(":tool_ver", OracleType.VarChar,50),
                new OracleParameter(":tool_tens_value", OracleType.VarChar,50),
                new OracleParameter(":CREATE_USER", OracleType.VarChar,50),
                new OracleParameter(":TOOL_BIN_CODE", OracleType.VarChar,50),
                new OracleParameter(":make_vend_code", OracleType.VarChar,50),
                new OracleParameter(":make_rsn_code", OracleType.VarChar,50),
                new OracleParameter(":make_rsn_cont", OracleType.VarChar,100),
                new OracleParameter(":TOOL_SN", OracleType.VarChar,50),
                new OracleParameter(":vend_code", OracleType.VarChar,50),
                new OracleParameter(":vend_loc_code", OracleType.VarChar,50),
                new OracleParameter(":attachid", OracleType.VarChar,100),
                new OracleParameter(":tool_use_times", OracleType.VarChar,100)
                };
                cmdParam[0].Value = tbBarcode.Text.Trim();
                cmdParam[1].Value = tbMaskCode.Text.Trim();
                cmdParam[2].Value = tbMMVer.Text.Trim();
                cmdParam[3].Value = tbTension.Text.Trim();
                cmdParam[4].Value = PaCSGlobal.LoginUserInfo.Id;
                string storeLoc = "";
                if (cmbStore.SelectedIndex != -1)
                    storeLoc = cmbStore.Properties.Items[cmbStore.SelectedIndex].ToString();
                else
                    storeLoc = cmbStore.Text == "-请选择-" ? "" : cmbStore.Text;
                cmdParam[5].Value = storeLoc;
                cmdParam[6].Value = cmbMaker.SelectedIndex == -1?"":(cmbMaker.SelectedItem as ComboxData).Value;
                cmdParam[7].Value = cmbReason.SelectedIndex == -1?"":(cmbReason.SelectedItem as ComboxData).Value;
                cmdParam[8].Value = tbContents.Text.Trim();
                cmdParam[9].Value = tbSN.Text.Trim();
                cmdParam[10].Value = cmbOwerVend.SelectedIndex == -1 ? "" : (cmbOwerVend.SelectedItem as ComboxData).Value;
                cmdParam[11].Value = PaCSGlobal.LoginUserInfo.Venderid;
                cmdParam[12].Value = btnImage.Text;
                cmdParam[13].Value = tbUseTimes.Text.Trim();

                int i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
                if (i > 0)
                {
                    XtraMessageBox.Show("保存成功!", "提示");
                    MetalMaskGlobal.WriteLog(tbBarcode.Text.Trim(), "I");//履历

                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception btnSave_Click)
            {
                 XtraMessageBox.Show(this, "System error[btnSave_Click]: " + btnSave_Click.Message);
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

        private void btnImage_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (tbBarcode.Text.Equals(""))
                {
                    XtraMessageBox.Show("请输入 Barcode No", "提示");
                    tbBarcode.Focus();
                    return;
                }

                ImageUploadForm frmNew = new ImageUploadForm(tbBarcode.Text.Trim());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    btnImage.Text = frmNew.ReturnValue;
                }
            }
            catch (Exception btnImage_ButtonClick)
            {
                XtraMessageBox.Show(this, "System error[btnImage_ButtonClick]: " + btnImage_ButtonClick.Message);
            }
        }

        private void cmbStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            string storeLoc = "";
            if (cmbStore.SelectedIndex != -1)
                storeLoc = cmbStore.Properties.Items[cmbStore.SelectedIndex].ToString();
            string sql = "select lpad(trim(to_char(to_number(nvl(substr(max(tool_id),9,2),'00')) + 1,'99')),2,'0') hist"+
                                " from pacsm_rm_tool "+
                                " where substr(tool_id,1,8) = '" + storeLoc + "'";

            DataTable dtResult = OracleHelper.ExecuteDataTable(sql);
            tbBarcode.Text = storeLoc + dtResult.Rows[0]["hist"].ToString();
        }

        private void cmbStore_MouseClick(object sender, MouseEventArgs e)
        {
            if(cmbStore.Properties.Items.Count==0)
            {
                XtraMessageBox.Show("没有可用的Bin位置，请联系管理员！", "提示");
                return;
            }
        }
    }
}
