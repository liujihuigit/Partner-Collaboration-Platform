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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetalMask
{
    public partial class DeleteForm : XtraForm
    {
        private string receivedata = "";
        private delegate void InvokeDelegate(string data);
        private DataTable dt = new DataTable();
        public DeleteForm(List<string> barcodeList)
        {
            InitializeComponent();
            DataColumn dc = null;
            dc = dt.Columns.Add("BarcodeNo", Type.GetType("System.String"));
            PassRecord(barcodeList);
            MetalMaskGlobal.port.DataReceived += new SerialDataReceivedEventHandler(serialPortDelete_DataReceived);
        }

        private void PassRecord(List<string> barcodeList)
        {
            foreach (string barcode in barcodeList)
            {
                DataTable dtData = GetData(barcode);
                DataRow dr = dt.NewRow();
                dr["BarcodeNo"] = dtData.Rows[0]["BarcodeNo"];
                dt.Rows.Add(dr);
            }
            gridControl1.DataSource = dt;
            gridView1.Columns[0].Width = 100;
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

        private void serialPortDelete_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
            if (!MetalMaskGlobal.CheckStatus(currentStatus, "MBDEL"))
            {
                XtraMessageBox.Show("状态" + MetalMaskGlobal.GetStatusNmByCode(currentStatus) + "不能" + MetalMaskGlobal.GetStatusNmByCode("MBDEL"), "提示");
                return;
            }

            DataRow dr = dt.NewRow();
            dr["BarcodeNo"] = dtData.Rows[0]["BarcodeNo"];

            dt.Rows.Add(dr);

            gridControl1.DataSource = dt;
            gridView1.Columns[0].Width = 100;
            gridView1.Columns[0].OptionsColumn.AllowEdit = false;
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
            string sql = " select tool_id BarcodeNo ,status_code " +
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

                    sql = "update pacsm_rm_tool set del_yn = 'Y',status_code = 'MBDEL',update_dt = to_char(sysdate,'yyyyMMddhh24miss'),update_user ='" + PaCSGlobal.LoginUserInfo.Id + "' "+
                        " where  tool_gubun_code ='MM' "+
                        " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' " +
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeleteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MetalMaskGlobal.port.DataReceived -= new SerialDataReceivedEventHandler(serialPortDelete_DataReceived);///取消对serialPortDelete_DataReceived方法的绑定
        }
    }
}
