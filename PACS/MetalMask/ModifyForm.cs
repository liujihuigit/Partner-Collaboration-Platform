using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using PaCSTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetalMask
{
    public partial class ModifyForm : XtraForm
    {
        private string receivedata = "";
        private delegate void InvokeDelegate(string data);
        private DataTable dt = new DataTable();
        public ModifyForm(List<string> barcodeList)
        {
            InitializeComponent();
            //this.barcodeList = barcodeList;
            DataColumn dc = null;
            dc = dt.Columns.Add("BarcodeNo", Type.GetType("System.String"));
            dc = dt.Columns.Add("MaskCode", Type.GetType("System.String"));
            dc = dt.Columns.Add("ProductModel", Type.GetType("System.String"));

            dc = dt.Columns.Add("MaskVer", Type.GetType("System.String"));
            dc = dt.Columns.Add("SN", Type.GetType("System.String"));
            dc = dt.Columns.Add("Maker", Type.GetType("System.String"));
            dc = dt.Columns.Add("Reason", Type.GetType("System.String"));
            dc = dt.Columns.Add("ReasonContents", Type.GetType("System.String"));

            PassRecord(barcodeList);

            MetalMaskGlobal.port.DataReceived += new SerialDataReceivedEventHandler(serialPortModify_DataReceived);
        }

        private void PassRecord(List<string> barcodeList)
        {
            foreach(string barcode in barcodeList)
            { 
                DataTable dtData = GetData(barcode);
                DataRow dr = dt.NewRow();
                dr["BarcodeNo"] = dtData.Rows[0]["BarcodeNo"];
                dr["MaskCode"] = dtData.Rows[0]["MaskCode"];
                dr["ProductModel"] = dtData.Rows[0]["ProductModel"];

                dr["MaskVer"] = dtData.Rows[0]["MaskVer"];
                dr["SN"] = dtData.Rows[0]["SN"];
                dr["Maker"] = dtData.Rows[0]["Maker"];
                dr["Reason"] = dtData.Rows[0]["Reason"];
                dr["ReasonContents"] = dtData.Rows[0]["ReasonContents"];

                dt.Rows.Add(dr);
            }

            gridControl1.DataSource = dt;
            gridView1.BestFitColumns();
            //gridView1.Columns[0].Width = 100;
            gridView1.Columns[0].OptionsColumn.AllowEdit = false;
            //gridView1.Columns[1].Width = 120;
            gridView1.Columns[1].OptionsColumn.AllowEdit = false;
            //gridView1.Columns[2].Width = 120;
            gridView1.Columns[2].OptionsColumn.AllowEdit = false;

            gridView1.Columns[3].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
            gridView1.Columns[4].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
            gridView1.Columns[5].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
            gridView1.Columns[6].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
            gridView1.Columns[7].AppearanceCell.BackColor = PaCSGlobal.OptionColor;

            //gridView1.Columns[3].Width = 120;
            //gridView1.Columns[4].Width = 120;
            gridView1.Columns[5].Width = 150;
            gridView1.Columns[5].ColumnEdit = new MetalMaskGlobal().cmbMakerVendor();
            gridView1.Columns[6].Width = 150;
            gridView1.Columns[6].ColumnEdit = new MetalMaskGlobal().cmbMakerReason();
            gridView1.Columns[7].Width = 150;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void serialPortModify_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                System.Threading.Thread.Sleep(100); //读取速度慢，加Sleep延长读取时间, 不可缺少
                //serialPort1.DiscardInBuffer();  //如果不执行上面的代码,serialPort1_DataReceived会执行多次
                int n = MetalMaskGlobal.port.BytesToRead;
                byte[] buf = new byte[n];
                MetalMaskGlobal.port.Read(buf, 0, n);
                receivedata = System.Text.Encoding.ASCII.GetString(buf);
                receivedata = receivedata.Replace("\r\n", "");

                this.Invoke(new EventHandler(delegate
                {
                    //要委托的代码 
                    tbBarcode.Text = receivedata;
                    tbBarcode.SelectionStart = receivedata.Length;
                }));

                try
                {
                    this.Invoke(new InvokeDelegate(DoData), receivedata);
                }
                catch (Exception ex)
                {

                }
            }
        }

         private void DoData(string data)
         {
             if (IsRecordExisted(data))
             {
                 XtraMessageBox.Show("barcode已经存在", "提示");
                 return;
             }

             DataTable dtData = GetData(data);

             if (dtData == null)
             {
                 return;//barcode不存在
             }

             DataRow dr = dt.NewRow();
             dr["BarcodeNo"] = dtData.Rows[0]["BarcodeNo"];
             dr["MaskCode"] = dtData.Rows[0]["MaskCode"];
             dr["ProductModel"] = dtData.Rows[0]["ProductModel"];

             dr["MaskVer"] = dtData.Rows[0]["MaskVer"];
             dr["SN"] = dtData.Rows[0]["SN"];
             dr["Maker"] = dtData.Rows[0]["Maker"];
             dr["Reason"] = dtData.Rows[0]["Reason"];
             dr["ReasonContents"] = dtData.Rows[0]["ReasonContents"];

             dt.Rows.Add(dr);

             gridControl1.DataSource = dt;
             gridView1.BestFitColumns();

             gridView1.Columns[3].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
             gridView1.Columns[4].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
             gridView1.Columns[5].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
             gridView1.Columns[6].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
             gridView1.Columns[7].AppearanceCell.BackColor = PaCSGlobal.OptionColor;

             //gridView1.Columns[0].Width = 100;
             gridView1.Columns[0].OptionsColumn.AllowEdit = false;
             //gridView1.Columns[1].Width = 120;
             gridView1.Columns[1].OptionsColumn.AllowEdit = false;
             //gridView1.Columns[2].Width = 120;
             gridView1.Columns[2].OptionsColumn.AllowEdit = false;

             //gridView1.Columns[3].Width = 120;
             //gridView1.Columns[4].Width = 120;
             gridView1.Columns[5].Width = 150;
             gridView1.Columns[5].ColumnEdit = new MetalMaskGlobal().cmbMakerVendor();
             gridView1.Columns[6].Width = 150;
             gridView1.Columns[6].ColumnEdit = new MetalMaskGlobal().cmbMakerReason();
             gridView1.Columns[7].Width = 150;
         }

        private void teBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                receivedata = tbBarcode.Text.Trim().ToUpper();
                try
                {
                    DoData(receivedata);
                }
                catch (Exception teBarcode_KeyDown)
                {
                    XtraMessageBox.Show(this, "System error[teBarcode_KeyDown]: " + teBarcode_KeyDown.Message);
                }
            }
        }

        private bool  IsRecordExisted(string barcode)
        {
              DataColumn[] myPrimaryKey = new DataColumn[1];
              myPrimaryKey[0] = dt.Columns["BarcodeNo"];
                dt.PrimaryKey = myPrimaryKey;
                DataRow myRemoveRow = dt.Rows.Find(new string[1] { barcode });
                if (myRemoveRow != null)
                    return true;
                else
                    return false;
        }

        private DataTable GetData(string barcode)
        {
            string sql = " select tool_id BarcodeNo," +
                            " (select rprs_model_code from pacsm_md_tool_equip b where b.fct_code = 'C660A' and b.tool_gubun_code = 'M' and tool_code = a.tool_code and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) ProductModel," +
                            " tool_code MaskCode," +
                            "  status_code," +
                            " tool_ver MaskVer," +
                            " tool_sn SN," +
                            " (select COMM_CODE_NM||':'||COMM_CODE from PACSM_RM_COMM_INFO b where b.COMM_CODE = a.make_vend_code and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' )  Maker," +
                            " (select comm_code_nm||':'||comm_code from pacsc_md_comm_code b where type_code = 'MAKE_RSN' and comm_code = a.make_rsn_code and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) Reason," +
                            " make_rsn_cont ReasonContents" +
                            " from pacsm_rm_tool a " +
                            " where tool_gubun_code = 'MM'" +
                            " and tool_id = '" + barcode + "'" +
                            " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            DataTable dtResult = OracleHelper.ExecuteDataTable(sql);

            if(dtResult.Rows.Count==0)
            {
                XtraMessageBox.Show(MetalMaskGlobal.KeyDownEventTip, "提示");
                return null;
            }
            
            return dtResult;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    string BarcodeNo = gridView1.GetRowCellValue(i, gridView1.Columns["BarcodeNo"]).ToString();

                    string MaskVer = gridView1.GetRowCellValue(i, gridView1.Columns["MaskVer"]).ToString();
                    string SN = gridView1.GetRowCellValue(i, gridView1.Columns["SN"]).ToString();
                    string MakerText = gridView1.GetRowCellValue(i, gridView1.Columns["Maker"]).ToString();
                    string ReasonText = gridView1.GetRowCellValue(i, gridView1.Columns["Reason"]).ToString();
                    string MakerCode = MakerText.Split(':')[1];
                    string ReasonCode = ReasonText.Split(':')[1];
                    
                    string ReasonContents = gridView1.GetRowCellValue(i, gridView1.Columns["ReasonContents"]).ToString();

                    sql = "update pacsm_rm_tool set tool_ver = '" + MaskVer + "' ,tool_sn = '" + SN + "' ,make_vend_code = '" + MakerCode + "',make_rsn_code='" + ReasonCode + "',make_rsn_cont='" + ReasonContents + "',update_dt = to_char(sysdate,'yyyyMMddhh24miss'),update_user ='" + PaCSGlobal.LoginUserInfo.Id + "'  where  tool_gubun_code ='MM' and tool_id = '" + BarcodeNo + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
                    OracleHelper.ExecuteNonQuery(sql);
                    MetalMaskGlobal.WriteLog(BarcodeNo, "U");
                }

                XtraMessageBox.Show("保存成功", "提示");
                DialogResult = DialogResult.OK;
            }
            catch (Exception btnSave_Click)
            {
                XtraMessageBox.Show(this, "System error[btnSave_Click]: " + btnSave_Click.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dr = XtraMessageBox.Show("确认删除?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                DataColumn[] myPrimaryKey = new DataColumn[1];
                myPrimaryKey[0] = dt.Columns["BarcodeNo"];
                dt.PrimaryKey = myPrimaryKey;
                DataRow myRemoveRow = dt.Rows.Find(new string[1] { gridView1.GetRowCellValue(gridView1.GetSelectedRows()[0], "BarcodeNo").ToString() });
                if (myRemoveRow != null)
                    myRemoveRow.Delete();
                dt.AcceptChanges();
                gridControl1.DataSource = dt;
            }
            else
                return;
        }

        private void ModifyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MetalMaskGlobal.port.DataReceived -= new SerialDataReceivedEventHandler(serialPortModify_DataReceived);
        }
    }
}
