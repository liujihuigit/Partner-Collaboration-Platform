using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO.Ports;
using PaCSTools;
using System.Data.OracleClient;
using DevExpress.XtraGrid.Columns;

namespace Toner
{
    public partial class GREJH : DevExpress.XtraEditors.XtraForm
    {
        PaCSGlobal global = new PaCSGlobal();
        private SerialPort[] ports = new SerialPort[1];

        private DataTable dt = new DataTable();
        private delegate void InvokeDelegate(string data);
        private string docno = "";
        private string currentDocno = "";
        private string lastdocno = "";
        private string receivedata = "";
        private string vendor = "";
        private string item = "";
        private bool flagCancel = false;
        private string operation_window = "";
        private string curentMoveType = "";

        private string vend_to = "";

        public GREJH()
        {
            InitializeComponent();
            global.InitMenu();
            ports[0] = new SerialPort();
        }

        private void GREJH_Load(object sender, EventArgs e)
        {
            Init();
            panelStatus.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        }

        private void Init()
        {
            PaCSGlobal.InitComPort("Toner", "GREJH", ports);

            if(ports[0].IsOpen)
                ports[0].DataReceived += new SerialDataReceivedEventHandler(serialPortGREJH_DataReceived);//重新绑定

            cmbVendor.Properties.BeginUpdate();
            TonerGlobal.LoadCmbVendor(cmbVendor);
            cmbVendor.Properties.EndUpdate();
        }

        private void cmbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbItem.Properties.Items.Clear();
            if (cmbVendor.SelectedIndex != -1)
            {
                TonerGlobal.LoadItemByVendCode(cmbItem, (cmbVendor.SelectedItem as ComboxData).Value);
            }
        }

        private void serialPortGREJH_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                System.Threading.Thread.Sleep(100); //读取速度慢，加Sleep延长读取时间, 不可缺少
                int n = ports[0].BytesToRead;
                byte[] buf = new byte[n];
                ports[0].Read(buf, 0, n);
                ports[0].ReceivedBytesThreshold = 31;
                receivedata = System.Text.Encoding.ASCII.GetString(buf);
                receivedata = receivedata.Replace("\r\n", "");

                try
                {
                    this.Invoke(new InvokeDelegate(DoData), receivedata);
                }
                catch (Exception serialPortGREJH_DataReceived)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        //要委托的代码 
                        lbStatus.Text = "【"+receivedata + "】：操作失败";
                        panelStatus.BackColor = Color.Red;
                        XtraMessageBox.Show(this, "System error[serialPortGREJH_DataReceived]: " + serialPortGREJH_DataReceived.Message);
                    }));
                }
            }
        }

        private void DoData(string data)
        {
            if (cmbVendor.SelectedIndex == -1)
            {
                PaCSGlobal.Speak("请选择 生产厂家");
                lbStatus.Text = "请选择 生产厂家";
                panelStatus.BackColor = Color.Yellow;
                cmbVendor.Focus();
                return;
            }
            else if(PaCSGlobal.LoginUserInfo.Fct_code.Equals("C660A"))
            {
                vendor = (cmbVendor.SelectedItem as ComboxData).Value;//制造厂家
                vend_to = "DRI2";
                if (!vendor.Equals(data.Substring(11, 4)))
                {
                    PaCSGlobal.Speak("扫描碳粉生产厂家与选择厂家不符");
                    lbStatus.Text = "扫描碳粉生产厂家【" + PaCSGlobal.GetVendorNameByCode(data.Substring(11, 4)) + "】与选择厂家【" + (cmbVendor.SelectedItem as ComboxData).Text + "】不符";
                    panelStatus.BackColor = Color.Yellow;
                    return;
                }
            }
            else if (PaCSGlobal.LoginUserInfo.Fct_code.Equals("C6H0A"))
            {
                vendor = (cmbVendor.SelectedItem as ComboxData).Value;//制造厂家
                //vendor = "R100";
                vend_to = "RCAE";
                if (data.Substring(11, 4).Equals("R100"))
                {
                    PaCSGlobal.Speak("请扫描标签下方的条码");
                    lbStatus.Text = "请扫描标签下方的条码";
                    panelStatus.BackColor = Color.Yellow;
                    return;
                }
            }

            if (cmbItem.SelectedIndex == -1)
            {
                PaCSGlobal.Speak("请选择 材料编号");
                lbStatus.Text = "请选择 材料编号";
                panelStatus.BackColor = Color.Yellow;
                cmbItem.Focus();
                return;
            }
            else
            {
                item = cmbItem.SelectedItem.ToString();
                if (!item.Equals(data.Substring(0, 11)))
                {
                    PaCSGlobal.Speak("扫描碳粉材料编号与选择编号不符");
                    lbStatus.Text = "扫描碳粉材料编号与选择编号不符";
                    panelStatus.BackColor = Color.Yellow;
                    return;
                }
            }

            flagCancel = checkEdit2.Checked;//是否是入库取消模式

            //要委托的代码 
            tbBucket.Text = receivedata;
            tbBucket.SelectionStart = receivedata.Length;
             
            DataTable dtThis =TonerGlobal.ScanRecordStatus(data);//获取扫描的记录信息(未入库000，已入库101，入库取消102)
            

            if (flagCancel)
            {
                //入库取消 模式
                if (dtThis.Rows.Count == 0)
                {
                    PaCSGlobal.Speak("库存中不存在");
                    lbStatus.Text = "【" + data + "】：库存中不存在";
                    panelStatus.BackColor = Color.Yellow;
                    return;
                }
         
                currentDocno = dtThis.Rows[0]["currentDocno"].ToString();//获取当前finaldocno
                lastdocno = dtThis.Rows[0]["lastdocno"].ToString();//获取当前lastdocno
                operation_window = dtThis.Rows[0]["operation_window"].ToString();//获取当前operation_window

                if (!operation_window.Equals("GREJH"))
                {
                    PaCSGlobal.Speak("不能在此做取消操作");
                    lbStatus.Text = "【" + data + "】：不能在此做取消操作";
                    panelStatus.BackColor = Color.Yellow;
                    return;
                }

                CancelGR(data);
                tbBucket.Text = "";
            }
            else
            {
                //入库
                switch(radioGroup1.SelectedIndex)
                {
                    case 0://首次入库
                        if (dtThis.Rows.Count > 0)
                        {
                            PaCSGlobal.Speak("已入库，不能再次入库");
                            lbStatus.Text = "【" + data + "】：已入库，不能再次入库";
                            panelStatus.BackColor = Color.Yellow;
                            return;
                        }
                        GR(data);//insert
                        tbBucket.Text = "";
                        break;
                    case 1://返退入库
                        if (dtThis.Rows.Count == 0)
                        {
                            PaCSGlobal.Speak("库存中不存在");
                            lbStatus.Text = "【" + data + "】：库存中不存在！";
                            panelStatus.BackColor = Color.Yellow;
                            return;
                        }
                        else
                        {
                            curentMoveType = dtThis.Rows[0]["movetype"].ToString();//获取当前MOVE_TYPE
                            if (!curentMoveType.Equals("351"))
                            {
                                PaCSGlobal.Speak("不是在途状态，不能入库");
                                lbStatus.Text = "【" + data + "】：不是在途状态，不能入库";
                                panelStatus.BackColor = Color.Yellow;
                                return;
                            }
                        }
                        GRAgain(data);//update
                        tbBucket.Text = "";
                        break;
                }

            }
        }

        /// <summary>
        /// 原材料仓库 入库
        /// </summary>
        private void GR(string data)
        {
            //判断状态
            tbDocno.Text = GetDocNo();

            string lotNo = "";
            string boxNo = "";
            string qty = "";

            if (PaCSGlobal.LoginUserInfo.Fct_code.Equals("C660A"))
            {
                 lotNo = data.Substring(15, 6);
                 boxNo = data.Substring(21, 4);
                 qty = data.Substring(25, 6);
            }
            else if (PaCSGlobal.LoginUserInfo.Fct_code.Equals("C6H0A"))
            {
                //SESC
                 lotNo = data.Substring(15, 10);
                 boxNo = data.Substring(11, 4);
                 qty = data.Substring(25, 6);
            }

            string sql = "insert into pacsd_pm_box(BOX_LABEL,ITEM,MAKE_VEND_CODE,LOT_NO,BOX_NO,QTY," +
                " FINAL_MOVE_CODE,FINAL_MOVE_TYPE,FINAL_VEND_FROM,FINAL_VEND_TO,FINAL_DOC_NO," +
                " CREATE_USER,CREATE_IP,BOX_CASE_STATUS,BOX_STATUS,OPERATION_WINDOW,update_date,update_time,update_user,update_ip,fct_code) " +
                " values(:BOX_LABEL,:ITEM,:MAKE_VEND_CODE,:LOT_NO,:BOX_NO,:QTY," +
                " :FINAL_MOVE_CODE,'101',:FINAL_VEND_FROM,:FINAL_VEND_TO,:FINAL_DOC_NO," +
                " :CREATE_USER,:CREATE_IP,:BOX_CASE_STATUS,:BOX_STATUS,'GREJH',to_char(sysdate,'yyyyMMdd'),to_char(sysdate,'hh24miss'),:update_user,:update_ip,:fct_code) ";
            OracleParameter[] cmdParam = new OracleParameter[] {
                    new OracleParameter(":BOX_LABEL", OracleType.VarChar, 50), 
                    new OracleParameter(":ITEM", OracleType.VarChar, 50),
                    new OracleParameter(":MAKE_VEND_CODE", OracleType.VarChar,50),
                    new OracleParameter(":LOT_NO", OracleType.VarChar,50),
                    new OracleParameter(":BOX_NO", OracleType.VarChar,50),
                    new OracleParameter(":QTY", OracleType.VarChar,50),
                    new OracleParameter(":FINAL_MOVE_CODE", OracleType.VarChar,50),
                    new OracleParameter(":FINAL_VEND_FROM", OracleType.VarChar,50),
                    new OracleParameter(":FINAL_VEND_TO", OracleType.VarChar,50),
                    new OracleParameter(":FINAL_DOC_NO", OracleType.VarChar,50),
                    new OracleParameter(":CREATE_USER", OracleType.VarChar,20),
                    new OracleParameter(":CREATE_IP", OracleType.VarChar,20),
                    new OracleParameter(":BOX_CASE_STATUS", OracleType.VarChar,20),
                    new OracleParameter(":BOX_STATUS", OracleType.VarChar,20),
                    new OracleParameter(":update_user", OracleType.VarChar,20),
                    new OracleParameter(":update_ip", OracleType.VarChar,20),
                    new OracleParameter(":fct_code", OracleType.VarChar,20)
                    };
            cmdParam[0].Value = data;
            cmdParam[1].Value = item;
            cmdParam[2].Value = vendor;
            cmdParam[3].Value = lotNo;
            cmdParam[4].Value = boxNo;
            cmdParam[5].Value = qty;
            cmdParam[6].Value = "MOVE0101";
            cmdParam[7].Value = vendor;
            cmdParam[8].Value = vend_to;
            cmdParam[9].Value = docno;
            cmdParam[10].Value = PaCSGlobal.LoginUserInfo.Id;
            cmdParam[11].Value = PaCSGlobal.GetClientIp();

            DataTable dtStatus = TonerGlobal.GetCommInfoByCode("MOVE0101");
            if (dtStatus.Rows.Count > 0)
            {
                cmdParam[12].Value = dtStatus.Rows[0]["BOX_CASE_STATUS"].ToString();
                cmdParam[13].Value = dtStatus.Rows[0]["BOX_STATUS"].ToString();
            }
            else
            {
                cmdParam[12].Value = "";
                cmdParam[13].Value = "";
            }
            cmdParam[14].Value = PaCSGlobal.LoginUserInfo.Id;
            cmdParam[15].Value = PaCSGlobal.GetClientIp();
            cmdParam[16].Value = PaCSGlobal.LoginUserInfo.Fct_code;

            int i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            //插入prog表
            TonerGlobal.InsertIntoProg(data);
            lbStatus.Text = "【" + data + "】：入库成功";
            panelStatus.BackColor = Color.GreenYellow;
            //提示成功语音
            PaCSGlobal.PlayWavOk();
            //刷新列表
            TonerGlobal.SetGridView(GetData(docno), gridView1, gridControl1);
        }

        /// <summary>
        /// 原材料仓库 返退入库
        /// </summary>
        private void GRAgain(string data)
        {
            tbDocno.Text = GetDocNo();

            string sql = "update pacsd_pm_box set final_move_type = '101',final_move_code = 'MOVE0102',final_stock_to=null,final_buffer_to=null,final_line_to=null," +
                " final_doc_no = :final_doc_no,last_doc_no = final_doc_no,operation_window = 'GREJH',box_case_status = :box_case_status,box_status = :box_status," +
                " update_date = to_char(sysdate,'yyyyMMdd'),update_time = to_char(sysdate,'hh24miss'),update_user = :update_user,update_ip = :update_ip " +
                " where box_label = '" + data + "' "+
                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            OracleParameter[] cmdParam = new OracleParameter[] {
                    new OracleParameter(":final_doc_no", OracleType.VarChar, 50),
                    new OracleParameter(":update_user", OracleType.VarChar, 50), 
                    new OracleParameter(":update_ip", OracleType.VarChar, 50),
                    new OracleParameter(":box_case_status", OracleType.VarChar, 50),
                    new OracleParameter(":box_status", OracleType.VarChar, 50)
                    };
            cmdParam[0].Value = docno;
            cmdParam[1].Value = PaCSGlobal.LoginUserInfo.Id;
            cmdParam[2].Value = PaCSGlobal.GetClientIp();

            DataTable dtStatus = TonerGlobal.GetCommInfoByCode("MOVE0102");
            if (dtStatus.Rows.Count > 0)
            {
                cmdParam[3].Value = dtStatus.Rows[0]["BOX_CASE_STATUS"].ToString();
                cmdParam[4].Value = dtStatus.Rows[0]["BOX_STATUS"].ToString();
            }
            else
            {
                cmdParam[3].Value = "";
                cmdParam[4].Value = "";
            }

            int i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
            //插入prog表
            TonerGlobal.InsertIntoProg(data);
            lbStatus.Text = "【" + data + "】：入库成功";
            panelStatus.BackColor = Color.GreenYellow;
            //提示成功语音
            PaCSGlobal.PlayWavOk();
            //刷新列表
            TonerGlobal.SetGridView(GetData(docno), gridView1, gridControl1);
        }

        /// <summary>
        /// 原材料仓库 入库取消
        /// </summary>
        private void CancelGR(string data)
        {
            TonerGlobal.Cancel(lastdocno,data);

            //插入prog表
            TonerGlobal.UpdateProg(currentDocno, data);
            lbStatus.Text = "【" + data + "】：入库取消成功";
            panelStatus.BackColor = Color.GreenYellow;
            //提示成功语音
            PaCSGlobal.PlayWavOk();
            //刷新列表
            TonerGlobal.SetGridView(GetData(""), gridView1, gridControl1);
        }

        private DataTable GetData(string docno)
        {
            StringBuilder sql = new StringBuilder(" select a.final_doc_no DOCNO,a.box_label \"桶标签\",a.item \"材料编号\",(select t.vend_nm_cn from pacsm_md_vend t where t.vend_code = a.make_vend_code and t.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ) \"生产厂家\", " +
            " a.lot_no LotNo,a.box_no BoxNo,a.qty \"数量/千克\",to_char(to_date(a.create_date,'yyyymmdd'),'yyyy-mm-dd') \"入库日期\",to_char(to_date(a.create_time,'hh24miss'),'hh24:mi:ss') \"入库时间\","+
            "(select u.name  from pacs_user u  where u.id = a.create_user) \"入库人\",a.create_ip \"入库IP\" " +
            " from pacsd_pm_box a " +
            " where   operation_window = 'GREJH' " +
            " and create_date between to_char(sysdate-1,'yyyyMMdd') " +
            " and to_char(sysdate,'yyyyMMdd')"+
            " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");

            if (!string.IsNullOrEmpty(docno))
            {
                sql.Append(" and a.final_doc_no like '%" + docno + "%'");
            }
            sql.Append(" order by a.final_doc_no desc,a.create_date||a.create_time desc nulls last");
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());

            if (dtResult.Rows.Count == 0)
            {
                return null;
            }
            return dtResult;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                docno = tbDocno.Text.Trim();

                TonerGlobal.SetGridView(GetData(docno), gridView1, gridControl1);
            }
            catch (Exception btnApply_Click)
            {
                XtraMessageBox.Show(this, "System error[btnApply_Click]: " + btnApply_Click.Message);
            }
        }

        /// <summary>
        /// 生成DOCNO号
        /// </summary>
        /// <returns></returns>
        private string GetDocNo()
        {
            if (string.IsNullOrEmpty(tbDocno.Text))
            {
                docno = TonerGlobal.GenerateDocNo();
            }
            else
            {
                //if("追加扫描的docno中，有出库的碳粉，不能追加扫描")
                docno = tbDocno.Text.Trim();
            }
            return docno;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            tbDocno.Text = "";
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            PaCSGlobal.ExportGridToFile(gridView1, "Toner_GREJH");
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void GREJH_FormClosing(object sender, FormClosingEventArgs e)
        {
            ports[0].DataReceived -= new SerialDataReceivedEventHandler(serialPortGREJH_DataReceived);//取消绑定

            foreach (SerialPort port in ports)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
            }
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit2.Checked)
            {
                DialogResult dr = XtraMessageBox.Show("您确认切换到【入库取消】工作模式 吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    checkEdit2.BackColor = Color.Yellow;
                    PaCSGlobal.Speak("现在是【入库取消】工作模式");
                    lbStatus.Text = "现在是【入库取消】工作模式";
                    panelStatus.BackColor = Color.Yellow;
                    tbDocno.Text = "";
                }
                else
                    return;
            }
            else
            {
                DialogResult dr = XtraMessageBox.Show("您确认切换到【正常入库】工作模式 吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    checkEdit2.BackColor = Color.Transparent;
                    PaCSGlobal.Speak("现在是【正常入库】工作模式");
                    lbStatus.Text = "现在是【正常入库】工作模式";
                    panelStatus.BackColor = Color.Yellow;
                    tbDocno.Text = "";
                }
                else
                    return;
            }
        }

        private void tbDocno_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                AppendScan frmNew = new AppendScan(this.Name);
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    tbDocno.Text = frmNew.ReturnValue["DOCNO"];
                }
            }
            catch (Exception tbDocno_ButtonClick)
            {
                lbStatus.Text = "DOCNO获取失败：" + tbDocno_ButtonClick.Message;
                panelStatus.BackColor = Color.Red;
            }
        }

        private void btnCom_Click(object sender, EventArgs e)
        {
            SettingForm setcom = new SettingForm("Toner", "GREJH", 1);
            DialogResult dg = setcom.ShowDialog();

            if (dg == DialogResult.OK)
            {
                PaCSGlobal.InitComPort("Toner", "GREJH",ports);

                if (ports[0].IsOpen)
                    ports[0].DataReceived += new SerialDataReceivedEventHandler(serialPortGREJH_DataReceived);//重新绑定
            }
        }

        private void gridView1_MouseUp(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = this.gridView1.CalcHitInfo(e.Location);
            if (hi.InRow && e.Button == MouseButtons.Right)
            {
                global.CallMenu(gridView1).ShowPopup(Control.MousePosition);
            } 
        }

        private void tbBucket_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//ENTER键，回车键：确定操作
            {
                if (tbBucket.Text.Trim().Length<31)
                {
                    XtraMessageBox.Show("非法桶标签！", "提示");
                    return;
                }
                DoData(tbBucket.Text.Trim());
            }
        }

    }
}