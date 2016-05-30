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
    public partial class Register3 : XtraForm
    {
        public Register3()
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
                string sqlReason = "SELECT COMM_CODE,COMM_CODE_NM FROM PACSC_MD_COMM_CODE WHERE TYPE_CODE = 'MAKE_RSN'";
                string sqlStoreLoc = "select distinct(TOOL_BIN_CODE) storeloc from  pacsm_rm_tool w where w.tool_gubun_code  ='MM' and vend_loc_code = '" + PaCSGlobal.LoginUserInfo.Venderid + "' and status_code not in('MBNIN','MBBIN','MBCIN','MBTIN') ";///not in
                string sqlMaker = "select distinct nvl(vend_code,'C660') vend_code,(select vend_nm_cn from pacsm_md_vend b where nvl(a.vend_code,'C660') = b.vend_code) vend_nm from gmes20_line a";

                DataTable dtReason = OracleHelper.ExecuteDataTable(sqlReason);
                DataTable dtStoreLoc = OracleHelper.ExecuteDataTable(sqlStoreLoc);
                DataTable dtMaker = OracleHelper.ExecuteDataTable(sqlMaker);

                for (int i = 0; i < dtReason.Rows.Count; i++)
                {
                    ComboxData data = new ComboxData();
                    data.Text = dtReason.Rows[i]["COMM_CODE_NM"].ToString();
                    data.Value = dtReason.Rows[i]["COMM_CODE"].ToString();
                    cmbReason.Properties.Items.Add(data);
                }

                for (int i = 0; i < dtStoreLoc.Rows.Count; i++)
                {
                    cmbStore.Properties.Items.Add(dtStoreLoc.Rows[i]["storeloc"].ToString());
                }

                for (int i = 0; i < dtMaker.Rows.Count; i++)
                {
                    ComboxData data = new ComboxData();
                    data.Text = dtMaker.Rows[i]["vend_nm"].ToString();
                    data.Value = dtMaker.Rows[i]["vend_code"].ToString();
                    cmbMaker.Properties.Items.Add(data);
                }
            
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
                    XtraMessageBox.Show("请输入 Prod.Model", "提示");
                    tbProdModel.Focus();
                    return;
                }

                if (tbMaskCode.Text.Equals(""))
                {
                    XtraMessageBox.Show("请输入 M/Mask Code", "提示");
                    tbMaskCode.Focus();
                    return;
                }

                if (tbPCBCode.Text.Equals(""))
                {
                    XtraMessageBox.Show("请输入 PCB Code", "提示");
                    tbPCBCode.Focus();
                    return;
                }

                if (tbTB.Text.Equals(""))
                {
                    XtraMessageBox.Show("请输入 T/B", "提示");
                    tbTB.Focus();
                    return;
                }

                if (tbBarcode.Text.Equals(""))
                {
                    XtraMessageBox.Show("请输入 Barcode No", "提示");
                    tbBarcode.Focus();
                    return;
                }

                if (cmbMaker.SelectedIndex == -1)
                {
                    XtraMessageBox.Show("请选择 Maker", "提示");
                    cmbMaker.Focus();
                    return;
                }

                if (MetalMaskGlobal.CheckBarcodeExist(tbBarcode.Text.Trim()))
                  {
                      XtraMessageBox.Show("此 BarcodeNo 已存在！", "提示");
                      tbBarcode.Focus();
                      return;
                  }

                string sql = "insert into pacsm_rm_tool(fct_code,plant_code,tool_gubun_code,use_yn,del_yn,CREATE_DT,status_code,tool_id,tool_code,tool_ver,tool_tens_value,CREATE_USER,TOOL_BIN_CODE,make_vend_code,make_rsn_code,make_rsn_cont,TOOL_SN,vend_code,vend_loc_code) " +
                   " values( 'C660A','P631','MM','Y','N',to_char(sysdate,'yyyyMMddhh24miss'),'MBNIN',:tool_id,:tool_code,:tool_ver,:tool_tens_value,:CREATE_USER,:TOOL_BIN_CODE,:make_vend_code,:make_rsn_code,:make_rsn_cont,:TOOL_SN,:vend_code,:vend_loc_code)";
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
                new OracleParameter(":vend_loc_code", OracleType.VarChar,50)
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
                cmdParam[10].Value = PaCSGlobal.LoginUserInfo.Venderid;
                cmdParam[11].Value = PaCSGlobal.LoginUserInfo.Venderid;

                int i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
                if (i > 0)
                {
                    XtraMessageBox.Show("保存成功!", "提示");
                    MetalMaskGlobal.WriteLog(tbBarcode.Text.Trim(), "I");//履历

                    //如果MetalMask基本信息没有，则保存到基本信息表中
                    string sql2 = "select * from pacsm_md_tool_equip where tool_gubun_code = 'M' and tool_code = '" + tbMaskCode.Text.Trim() + "'";
                    DataTable dtResult = OracleHelper.ExecuteDataTable(sql2);
                    if (dtResult.Rows.Count==0)
                    {
                        string sql3 = "insert into pacsm_md_tool_equip(fct_code,tool_gubun_code,pcb_gubun_code,del_yn,use_yn,fst_reg_dt,fst_reger_id," +
                            " tool_code,rprs_model_code,tb_gubun_code,"+
                            " tool_arry_num,tool_ver,tool_leng,tool_heit,tool_thic" +
                            " ) " +//2
                            " values('C660A','M','M','N','Y',to_char(sysdate,'yyyyMMddhh24miss'),'" + PaCSGlobal.LoginUserInfo.Name + "',"+
                            " :tool_code,:rprs_model_code,:tb_gubun_code," +
                            " :tool_arry_num,:tool_ver,:tool_leng,:tool_heit,:tool_thic)";
                        OracleParameter[] cmdParam2 = new OracleParameter[] {                
                            new OracleParameter(":tool_code", OracleType.VarChar,30),
                            new OracleParameter(":rprs_model_code", OracleType.VarChar,50),
                            new OracleParameter(":tb_gubun_code", OracleType.VarChar,50),

                            new OracleParameter(":tool_arry_num", OracleType.Number,50),
                            new OracleParameter(":tool_ver", OracleType.VarChar,50),
                            new OracleParameter(":tool_leng", OracleType.Number,50),
                            new OracleParameter(":tool_heit", OracleType.Number,50),
                            new OracleParameter(":tool_thic", OracleType.Number,50)
                            };

                        cmdParam[0].Value = tbMaskCode.Text.Trim();
                        cmdParam[1].Value = tbProdModel.Text.Trim();
                        cmdParam[2].Value = tbTB.Text.Trim();
                        cmdParam[3].Value = tbArray.Text.Trim();
                        cmdParam[4].Value = tbPCBVer.Text.Trim();
                        cmdParam[5].Value = tbLength.Text.Trim();
                        cmdParam[6].Value = tbWidth.Text.Trim();
                        cmdParam[7].Value = tbThickness.Text.Trim();

                        OracleHelper.ExecuteNonQuery(sql3, cmdParam2);
                    }

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
    }
}
