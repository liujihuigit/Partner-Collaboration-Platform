using DevExpress.Data;
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
    public partial class HistoryForm : XtraForm
    {
        private string receivedata = "";
        private delegate void InvokeDelegate(string data);
        private DataTable dt = new DataTable();
        MSK_Mgnt report;
        public HistoryForm(MSK_Mgnt report)
        {
            InitializeComponent();
            this.report = report;
            DataColumn dc = null;
            dc = dt.Columns.Add("BarcodeNo", Type.GetType("System.String"));
            dateEditFrom.Text = PaCSGlobal.GetServerDateTime(3);
            dateEditTo.Text = PaCSGlobal.GetServerDateTime(3);
            panelControl1.Enabled = false;

            MetalMaskGlobal.port.DataReceived += new SerialDataReceivedEventHandler(serialPortHis_DataReceived);
        }

        private void serialPortHis_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
            string dateFrom = dateEditFrom.Text.Trim().Replace("-", "");//2014-08-05
            string dateTo = dateEditTo.Text.Trim().Replace("-", "");//2014-08-06

            gridControl1.DataSource = GetData(dateFrom, dateTo, data);
            gridView1.BestFitColumns();
            gridView1.Columns["所属厂家"].Width = 120;
            gridView1.Columns["所属厂家"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView1.Columns["所属厂家"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";
        }

         private void btnApply_Click(object sender, EventArgs e)
        {
            string dateFrom = dateEditFrom.Text.Trim().Replace("-","");//2014-08-05
            string dateTo = dateEditTo.Text.Trim().Replace("-", "");//2014-08-06
            string barcode = tbBarcode.Text.Trim();

            gridControl1.DataSource = GetData(dateFrom, dateTo, barcode);
            gridView1.BestFitColumns();
            gridView1.Columns["所属厂家"].Width = 120;
            gridView1.Columns["所属厂家"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView1.Columns["所属厂家"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";
        }

        private DataTable GetData(string dateFrom1,string dateTo1,string barcode)
        {
            StringBuilder sql = new StringBuilder(" select  " +
                                " (select vend_nm_cn from pacsm_md_vend b where a.vend_code = b.vend_code)  \"所属厂家\", " +
                                " tool_id BarcodeNo,to_char(to_date(hist_tsp,'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss') \"动作时间\",                                                 " +
                                " dml_type_code \"动作类型\"," +
                                " tool_gubun_code, tool_code,                       "    +
                                " (select vend_nm_cn from pacsm_md_vend b where a.vend_loc_code = b.vend_code)  \"使用厂家\", " +
                                " (select comm_code_nm from pacsc_md_comm_code b where type_code = 'tool_status' and a.status_code = b.comm_code) status, "+
                                " (select line_nm from gmes20_line g where g.line_code = a.tool_line_code) line,                         "    +
                                " tool_bin_code Bin, "+
                                " tool_sn \"s/n\", tool_ver \"m/m ver\",                        " +
                                " tool_tens_value tension,tool_use_times_add \"本次使用次数\", tool_use_times \"总使用次数\"," +
                                " (select vend_nm_cn from pacsm_md_vend b where a.make_vend_code = b.vend_code)  \"制造厂家\",               " +
                                " (select comm_code_nm from pacsc_md_comm_code b where type_code = 'make_rsn' and a.make_rsn_code = b.comm_code)  \"制造原因\", " +
                                " make_rsn_cont \"制造原因内容\", caryin_rsn \"搬入原因\",                          " +
                                " caryot_rsn \"搬出原因\" , " +
                                " caryot_receiver \"接收人\" , " +
                                " caryot_receiver_contct tel,              " +
                                " (select comm_code_nm from pacsc_md_comm_code b where type_code = 'dsu_rsn' and a.dsu_rsn_code = b.comm_code)  \"废弃原因\"," +
                                " del_yn,                                               "   +
                                " to_char(to_date(create_dt,'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss')  registeredon, "+
                                " (select u.name  from pacs_user u  where u.id = a.create_user) \"注册人\", " +
                                " to_char(to_date(update_dt,'yyyymmddhh24miss'),'yyyy-mm-dd hh24:mi:ss')  updatedon,                 "    +
                                " (select u.name  from pacs_user u  where u.id = a.update_user)  \"更新人\"                                               " +
                                " from pacsm_rm_tool_h a " +
                                " where tool_gubun_code = 'MM' "+
                                " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' " +
                                " and vend_loc_code = '" + report.GetVendor() + "'          ");

            if (radioGroup1.SelectedIndex == 1)
                sql.Append( " and  hist_tsp between '" + dateFrom1 + "000000' and '" + dateTo1 + "235959'");

            if(!string.IsNullOrEmpty(barcode))
                sql.Append(" and  tool_id like '%" + barcode + "%'");

            sql.Append(" order by hist_tsp desc ");
            Console.WriteLine(sql.ToString());
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());

            if(dtResult.Rows.Count==0)
            {
                //XtraMessageBox.Show("无数据", "提示");
                return null;
            }
            return dtResult;
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioGroup1.SelectedIndex == 0)
            {
                panelControl1.Enabled = false;
            }
            else
                panelControl1.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            PaCSGlobal.ExportGridToFile(gridView1, "MetalMask_History");
        }

        private void HistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MetalMaskGlobal.port.DataReceived -= new SerialDataReceivedEventHandler(serialPortHis_DataReceived);
        }
       
    }
}
