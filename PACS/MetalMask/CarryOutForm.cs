using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using PaCSTools;
using System;
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
    public partial class CarryOutForm : XtraForm
    {
        private string receivedata = "";
        private delegate void InvokeDelegate(string data);
        private DataTable dt = new DataTable();
        public CarryOutForm(List<string> barcodeList)
        {
            InitializeComponent();
            DataColumn dc = null;
            dc = dt.Columns.Add("BarcodeNo", Type.GetType("System.String"));
            dc = dt.Columns.Add("ProductModel", Type.GetType("System.String"));
            dc = dt.Columns.Add("Status", Type.GetType("System.String"));
            dc = dt.Columns.Add("Location*", Type.GetType("System.String"));
            dc = dt.Columns.Add("CarryOutReason", Type.GetType("System.String"));   
            dc = dt.Columns.Add("Recipient", Type.GetType("System.String"));
            dc = dt.Columns.Add("Tel", Type.GetType("System.String"));
            PassRecord(barcodeList);
            MetalMaskGlobal.port.DataReceived += new SerialDataReceivedEventHandler(serialPortCarryOut_DataReceived);
        }

        private void PassRecord(List<string> barcodeList)
        {
            foreach (string barcode in barcodeList)
            {
                DataTable dtData = GetData(barcode);
                DataRow dr = dt.NewRow();
                dr["BarcodeNo"] = dtData.Rows[0]["BarcodeNo"];
                dr["ProductModel"] = dtData.Rows[0]["ProductModel"];
                dr["Status"] = dtData.Rows[0]["Status"];
                dr["Location*"] = dtData.Rows[0]["Location"];
                dr["CarryOutReason"] = dtData.Rows[0]["CarryOutReason"];
                dr["Recipient"] = dtData.Rows[0]["Recipient"];
                dr["Tel"] = dtData.Rows[0]["Tel"];

                dt.Rows.Add(dr);
            }

            gridControl1.DataSource = dt;

            gridView1.Columns[3].AppearanceCell.BackColor = PaCSGlobal.MustColor;
            gridView1.Columns[4].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
            gridView1.Columns[5].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
            gridView1.Columns[6].AppearanceCell.BackColor = PaCSGlobal.OptionColor;

            gridView1.Columns[0].Width = 100;
            gridView1.Columns[0].OptionsColumn.AllowEdit = false;
            gridView1.Columns[1].Width = 120;
            gridView1.Columns[1].OptionsColumn.AllowEdit = false;
            gridView1.Columns[2].Width = 120;
            gridView1.Columns[2].OptionsColumn.AllowEdit = false;
            gridView1.Columns[3].Width = 120;
            gridView1.Columns[3].ColumnEdit = new MetalMaskGlobal().cmbVendor();
            gridView1.Columns[4].Width = 150;
            gridView1.Columns[5].Width = 120;
            gridView1.Columns[6].Width = 120;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void serialPortCarryOut_DataReceived(object sender, SerialDataReceivedEventArgs e)
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

              string currentStatus = dtData.Rows[0]["status_code"].ToString();
              if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBCOT"))
              {
                  XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBCOT"), "提示");
                  return;
              }

              DataRow dr = dt.NewRow();
              dr["BarcodeNo"] = dtData.Rows[0]["BarcodeNo"];
              dr["ProductModel"] = dtData.Rows[0]["ProductModel"];
              dr["Status"] = dtData.Rows[0]["Status"];
              dr["Location*"] = dtData.Rows[0]["Location"];
              dr["CarryOutReason"] = dtData.Rows[0]["CarryOutReason"];
              dr["Recipient"] = dtData.Rows[0]["Recipient"];
              dr["Tel"] = dtData.Rows[0]["Tel"];

              dt.Rows.Add(dr);

              gridControl1.DataSource = dt;

              gridView1.Columns[3].AppearanceCell.BackColor = PaCSGlobal.MustColor;
              gridView1.Columns[4].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
              gridView1.Columns[5].AppearanceCell.BackColor = PaCSGlobal.OptionColor;
              gridView1.Columns[6].AppearanceCell.BackColor = PaCSGlobal.OptionColor;

              gridView1.Columns[0].Width = 100;
              gridView1.Columns[0].OptionsColumn.AllowEdit = false;
              gridView1.Columns[1].Width = 120;
              gridView1.Columns[1].OptionsColumn.AllowEdit = false;
              gridView1.Columns[2].Width = 120;
              gridView1.Columns[2].OptionsColumn.AllowEdit = false;
              gridView1.Columns[3].Width = 120;
              gridView1.Columns[3].ColumnEdit = new MetalMaskGlobal().cmbVendor();
              gridView1.Columns[4].Width = 150;
              gridView1.Columns[5].Width = 120;
              gridView1.Columns[6].Width = 120;
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
                            " (select rprs_model_code from pacsm_md_tool_equip b where b.fct_code = 'C660A' and b.tool_gubun_code = 'M' and tool_code = a.tool_code) ProductModel,"+
                            " 'Carry-out' Status,"+
                            "  status_code," +
                            " caryot_rsn CarryOutReason,"+
                            " (SELECT VEND_NM_CN FROM PACSM_MD_VEND B WHERE A.VEND_LOC_CODE = B.VEND_CODE) Location," +
                            " CARYOT_RECEIVER Recipient," +
                            " CARYOT_RECEIVER_CONTCT Tel" +
                            " from pacsm_rm_tool a "+
                            " where tool_gubun_code = 'MM'" +
                            " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' " +
                            " and vend_loc_code = '" + PaCSGlobal.LoginUserInfo.Venderid + "' " +
                            " and tool_id = '" + barcode + "'";

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
                    string vendLocNm = gridView1.GetRowCellValue(i, gridView1.Columns["Location*"]).ToString();
                    //不能搬到自己厂家
                    DataTable dtData = GetData(BarcodeNo);
                    if (vendLocNm.Split(':')[0].Equals(dtData.Rows[0]["Location"]))
                    {
                        XtraMessageBox.Show("不能搬出到自己厂家", "提示");
                        return;
                    }

                    string vendLocCode = vendLocNm.Split(':')[1];

                    string CarryOutReason = gridView1.GetRowCellValue(i, gridView1.Columns["CarryOutReason"]).ToString();
                    string Recipient = gridView1.GetRowCellValue(i, gridView1.Columns["Recipient"]).ToString();
                    string Tel = gridView1.GetRowCellValue(i, gridView1.Columns["Tel"]).ToString();

                    sql = "update pacsm_rm_tool set VEND_LOC_CODE = '" + vendLocCode + "' ,status_code = 'MBCOT',caryot_rsn = '" + CarryOutReason + "',CARYOT_RECEIVER='" + Recipient + "',CARYOT_RECEIVER_CONTCT='" + Tel + "',update_dt = to_char(sysdate,'yyyyMMddhh24miss'),update_user ='" + PaCSGlobal.LoginUserInfo.Id + "' "+
                        " where  tool_gubun_code ='MM' "+
                        " and tool_id = '" + BarcodeNo + "' "+
                        " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
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

        private void CarryOutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MetalMaskGlobal.port.DataReceived -= new SerialDataReceivedEventHandler(serialPortCarryOut_DataReceived);
        }
    }
}
