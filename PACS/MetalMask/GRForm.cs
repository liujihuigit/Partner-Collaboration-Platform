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
    public partial class GRForm : XtraForm
    {
        private string receivedata = "";
        private delegate void InvokeDelegate(string data);
        private DataTable dt = new DataTable();
        public GRForm(List<string> barcodeList)
        {
            InitializeComponent();

            DataColumn dc = null;
            dc = dt.Columns.Add("BarcodeNo", Type.GetType("System.String"));
            dc = dt.Columns.Add("MaskVer", Type.GetType("System.String"));
            dc = dt.Columns.Add("Status", Type.GetType("System.String"));
            dc = dt.Columns.Add("StoreLoc", Type.GetType("System.String"));
            dc = dt.Columns.Add("Tension", Type.GetType("System.String"));

            PassRecord(barcodeList);
            MetalMaskGlobal.port.DataReceived += new SerialDataReceivedEventHandler(serialPortGR_DataReceived);
        }

        private void PassRecord(List<string> barcodeList)
        {
            foreach (string barcode in barcodeList)
            {
                DataTable dtData = GetData(barcode);
                DataRow dr = dt.NewRow();
                dr["BarcodeNo"] = dtData.Rows[0]["BarcodeNo"];
                dr["MaskVer"] = dtData.Rows[0]["MaskVer"];
                dr["Status"] = dtData.Rows[0]["Status"];
                dr["StoreLoc"] = dtData.Rows[0]["StoreLoc"];
                dr["Tension"] = dtData.Rows[0]["Tension"];

                dt.Rows.Add(dr);
            }

            gridControl1.DataSource = dt;

            //gridView1.Columns[3].AppearanceCell.BackColor = PaCSGlobal.MustColor;

            gridView1.Columns[0].Width = 150;
            gridView1.Columns[0].OptionsColumn.AllowEdit = false;
            gridView1.Columns[1].Width = 150;
            gridView1.Columns[1].OptionsColumn.AllowEdit = false;
            gridView1.Columns[2].Width = 150;
            gridView1.Columns[2].OptionsColumn.AllowEdit = false;
            gridView1.Columns[3].Width = 150;
            gridView1.Columns[3].OptionsColumn.AllowEdit = false;
            gridView1.Columns[4].Width = 150;
            gridView1.Columns[4].OptionsColumn.AllowEdit = false;
        }

        private void GRForm_Load(object sender, EventArgs e)
        {
           
        }

        private void serialPortGR_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
            if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBBIN"))
            {
                XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBBIN"), "提示");
                return;
            }

            DataRow dr = dt.NewRow();
            dr["BarcodeNo"] = dtData.Rows[0]["BarcodeNo"];
            dr["MaskVer"] = dtData.Rows[0]["MaskVer"];
            dr["Status"] = dtData.Rows[0]["Status"];
            dr["StoreLoc"] = dtData.Rows[0]["StoreLoc"]; ;
            dr["Tension"] = dtData.Rows[0]["Tension"];

            dt.Rows.Add(dr);

            gridControl1.DataSource = dt;

            //gridView1.Columns[3].AppearanceCell.BackColor = PaCSGlobal.MustColor;

            gridView1.Columns[0].Width = 150;
            gridView1.Columns[0].OptionsColumn.AllowEdit = false;
            gridView1.Columns[1].Width = 150;
            gridView1.Columns[1].OptionsColumn.AllowEdit = false;
            gridView1.Columns[2].Width = 150;
            gridView1.Columns[2].OptionsColumn.AllowEdit = false;
            gridView1.Columns[3].Width = 150;
            gridView1.Columns[3].OptionsColumn.AllowEdit = false;
            gridView1.Columns[4].Width = 150;
            gridView1.Columns[4].OptionsColumn.AllowEdit = false;
        }


        private void tbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                receivedata = tbBarcode.Text.Trim().ToUpper();
                try
                {
                    DoData(receivedata);
                }
                catch (Exception tbBarcode_KeyDown)
                {
                    XtraMessageBox.Show(this, "System error[tbBarcode_KeyDown]: " + tbBarcode_KeyDown.Message);
                }
            }
        }

        void repositoryItemComboBox_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            e.Value = e.Value.ToString();
            e.Handled = true;
        }

        private bool IsRecordExisted(string barcode)
        {
            DataColumn[] myPrimaryKey = new DataColumn[1];
            myPrimaryKey[0] = dt.Columns["BarcodeNo"];
            dt.PrimaryKey = myPrimaryKey;
            DataRow myRemoveRow = dt.Rows.Find(new string[1] { barcode });//remark$id  id funcid
            if (myRemoveRow != null)
                return true;
            else
                return false;
        }

        private DataTable GetData(string barcode)
        {
            string sql = "select tool_id BarcodeNo,tool_ver MaskVer,status_code,'RACK' Status,TOOL_BIN_CODE StoreLoc,tool_tens_value Tension "+
                " from pacsm_rm_tool "+
                " where  tool_gubun_code ='MM'  "+
                " and vend_loc_code = '" + PaCSGlobal.LoginUserInfo.Venderid + "'  "+
                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' "+
                " and tool_id = '" + barcode + "'";
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql);

            if (dtResult.Rows.Count == 0)
            {
                XtraMessageBox.Show("barcode不存在", "提示");
                return null;
            }

            return dtResult;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    string BarcodeNo = gridView1.GetRowCellValue(i, gridView1.Columns["BarcodeNo"]).ToString();

                    sql = "update pacsm_rm_tool set status_code = 'MBBIN' ,update_dt = to_char(sysdate,'yyyyMMddhh24miss'),update_user ='" + PaCSGlobal.LoginUserInfo.Id + "'  "+
                        " where  tool_gubun_code ='MM' "+
                        " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' "+
                        " and tool_id = '" + BarcodeNo + "'";
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

        private void btnDelRow_Click(object sender, EventArgs e)
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

        private void cmbRack_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbBarcode.Focus();
        }

        private void GRForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MetalMaskGlobal.port.DataReceived -= new SerialDataReceivedEventHandler(serialPortGR_DataReceived);
        }

    }
}
