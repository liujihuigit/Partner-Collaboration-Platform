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
using DevExpress.XtraSplashScreen;
using DevExpress.XtraEditors.Repository;

namespace Toner
{
    public partial class PrintBarCode : DevExpress.XtraEditors.XtraForm
    {
        private delegate void InvokeDelegate(string data,string count);
        LPTHelper lpt = new LPTHelper();
        PaCSGlobal global = new PaCSGlobal();
        public PrintBarCode()
        {
            InitializeComponent();
            global.InitMenu();
            cmbVendor.Focus();
        }

        private void PrintBarCode_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            cmbVendor.Properties.BeginUpdate();
            TonerGlobal.LoadCmbVendor(cmbVendor);
            cmbVendor.Properties.EndUpdate();

            if(PaCSGlobal.LoginUserInfo.Fct_code.Equals("C660A"))
            {
                cmbDPI.SelectedIndex = 0;
            }
            else if(PaCSGlobal.LoginUserInfo.Fct_code.Equals("C6H0A"))
            {
                cmbDPI.SelectedIndex = 1;
            }
        }

        private void cmbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbItem.Properties.Items.Clear();
            if (cmbVendor.SelectedIndex != -1)
            {
                TonerGlobal.LoadItemByVendCode(cmbItem, (cmbVendor.SelectedItem as ComboxData).Value);
            }
        }

        private void PrintBarCode_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbVendor.SelectedIndex == -1)
                {
                    PaCSGlobal.Speak("请选择 生产厂家");
                    XtraMessageBox.Show("请选择 生产厂家！", "提示");
                    cmbVendor.Focus();
                    return;
                }

                if (cmbItem.SelectedIndex == -1)
                {
                    PaCSGlobal.Speak("请选择 材料编号");
                    XtraMessageBox.Show("请选择 材料编号！", "提示");
                    cmbItem.Focus();
                    return;
                }

                if (cmbUnit.Text.Equals(""))
                {
                    PaCSGlobal.Speak("请选择或输入单位");
                    XtraMessageBox.Show("请选择或输入单位！", "提示");
                    cmbUnit.Focus();
                    return;
                }
                else
                {
                    try
                    {
                        int.Parse(cmbUnit.Text.Trim());
                    }
                    catch (Exception)
                    {
                        XtraMessageBox.Show("单位为正整数！", "提示");
                        cmbUnit.Focus();
                        return;
                    }
                }

                if (tbCount.EditValue == null)
                {
                    PaCSGlobal.Speak("请输入打印张数");
                    XtraMessageBox.Show("请输入打印张数！", "提示");
                    tbCount.Focus();
                    return;
                }
                else
                {
                    try
                    {
                        int.Parse(tbCount.EditValue.ToString());
                    }
                    catch (Exception)
                    {
                        XtraMessageBox.Show("打印张数为正整数！", "提示");
                        tbCount.Focus();
                        return;
                    }
                }

                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitPrinting));
            }
            catch (Exception btnPrint_Click)
            {
                XtraMessageBox.Show(this, "System error[btnPrint_Click]: " + btnPrint_Click.Message);
            }

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string item = cmbItem.SelectedItem.ToString();
            string vendor = (cmbVendor.SelectedItem as ComboxData).Value;//制造厂家
            string lotno = PaCSGlobal.GetServerDateTime(1).Substring(0, 10);
            string qty = "0000" + cmbUnit.Text;

            string data = item + vendor + lotno + qty;
            string count = tbCount.EditValue.ToString();
            this.Invoke((MethodInvoker)delegate
            {
                DoData(data, count);
            });
        }

        private void DoData(string data,string count)
        {
            //如果扫描的是新label则提示错误
            if (!CheckLabel(data))
            {
                XtraMessageBox.Show("此标签为新标签，请扫描原始标签！","提示");
                return;
            }

            GenerateLabel(data, count);
            ShowGrid();
        }

        private bool CheckLabel(string data)
        {
            string sql = "select 1 from pacsd_pm_box_gen " +
              " where box_label_uni = '" + data + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                return false;
            return true;
        }

        private void GenerateLabel(string data, string count)
        {
            string docno = "";
            string sql = "select to_char(sysdate,'yyyymmdd')||fn_gene_seq('TONER_UNI_LABEL_DOC',to_char(sysdate,'yyyymmdd'),'" + PaCSGlobal.LoginUserInfo.Fct_code + "','N','N','N',4) doc_no from dual";
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql);
            if (dtResult.Rows.Count > 0)
                docno = dtResult.Rows[0][0].ToString();

            string sql2 = "insert into pacsd_pm_box_gen(box_label_uni,doc_no,box_label_ori,scan_date,scan_time,scan_user,scan_ip,fct_code) " +
             " select substr(:ls_box_label_ori,1,11)||substr(:ls_box_label_ori,12,4)||to_char(sysdate,'yymmdd')||fn_gene_seq('TONER_UNI_LABEL'," +
             " substr(:ls_box_label_ori,1,11),substr(:ls_box_label_ori,12,4),to_char(sysdate,'yymmdd')," +
             " substr(:ls_box_label_ori,26,6),'" + PaCSGlobal.LoginUserInfo.Fct_code + "',4)||substr(:ls_box_label_ori,26,6) box_label_uni," +
             " :ls_doc_no," +
             " :ls_box_label_ori," +
             " to_char(sysdate,'yyyymmdd')," +
             " to_char(sysdate,'hh24miss')," +
             " :ls_user_id," +
             " sys_context('USERENV','IP_ADDRESS')," +
             " :fct_code "+
             " from dual" +
             " connect by level <= " + count + "";

            OracleParameter[] cmdParam = new OracleParameter[] {
                    new OracleParameter(":ls_box_label_ori", OracleType.VarChar, 50), 
                    new OracleParameter(":ls_doc_no", OracleType.VarChar, 50),
                    new OracleParameter(":ls_user_id", OracleType.VarChar,20),
                     new OracleParameter(":fct_code", OracleType.VarChar,20)
                    };
            cmdParam[0].Value = data;
            cmdParam[1].Value = docno;
            cmdParam[2].Value = PaCSGlobal.LoginUserInfo.Id;
            cmdParam[3].Value = PaCSGlobal.LoginUserInfo.Fct_code;

            int i = OracleHelper.ExecuteNonQuery(sql2,cmdParam);
            //打印标签
            PrintLabel(docno);
        }

        private void ShowGrid()
        {
            string sql = "select box_label_uni \"生成标签\",doc_no DOCNO,box_label_ori \"初始标签\","+
                " to_char(to_date(scan_date,'yyyymmdd'),'yyyy-mm-dd') \"扫描日期\",to_char(to_date(scan_time,'hh24miss'),'hh24:mi:ss') \"扫描时间\", " +
                " (select u.name  from pacs_user u  where u.id = scan_user) \"扫描人\",scan_ip \"扫描人IP\"" +
                " from pacsd_pm_box_gen "+
                " where fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' "+
                " order by doc_no desc,box_label_uni desc";

            DataTable dtResult = OracleHelper.ExecuteDataTable(sql);

            gridControl1.DataSource = dtResult;
            gridView1.BestFitColumns();
            gridView1.OptionsCustomization.AllowColumnMoving = false;//禁止列拖动
            gridView1.OptionsView.AllowCellMerge = true;
            gridView1.OptionsBehavior.Editable = false;
        }


        private string SetCmdText(string uniqueLabel)
        {
            int dpi = cmbDPI.SelectedIndex;
            string temp = "";

            string part_no = uniqueLabel.Substring(0, 11);
            string vendor1 = uniqueLabel.Substring(11, 4);
            string t10 = uniqueLabel.Substring(15, 10);
            string qty1 = uniqueLabel.Substring(25, 6);
            string qty2 = uniqueLabel.Substring(29, 2);

            string sql = "select description from tb_fpp_itemmaster WHERE werks = 'P631' and matnr = '"+part_no+"'";
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql);
            string prod = dtResult.Rows[0][0].ToString();

            string sql2 = "select vend_nm from pacsm_md_vend where vend_code = '" + vendor1 + "' and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";
            DataTable dtResult2 = OracleHelper.ExecuteDataTable(sql2);
            string vendor1Name = dtResult2.Rows[0][0].ToString();

            string fulldate = DateTime.Now.ToString("yyyy.MM.dd");

            if(dpi==0)  //200dpi
            {
                temp = "^XA" +
                            "^PW800" +
                            "^PR2" +
                            "^MD30" +
                            "^MNY" +
                            "^LH20,0^FS" +
                            "^SEE:UHANGUL.DAT^FS" +
                            "^CWQ,E:AAAAA.FNT^FS" +
                            "^FO20,16^BY2^BCN,70,Y,N,N,N^FD" + part_no + vendor1 + t10 + qty1 + "^FS" +
                            "^FO28,110^AR^FDPart No : " + part_no + "^FS" +
                            "^CI14" +
                            "^FO28,150^A0N,20,20^CI13^FR^FD" + prod + "^FS" +
                            "^CI1" +
                            "^FO28,170^AR^FDVendor : ^FS" +
                            "^CI14" +
                            "^FO140,180^A0N,20,20^CI13^FR^FD" + vendor1Name + "^FS" +
                            "^CI1" +
                            "^FO28,200^AR^FDQTY     : " + qty2 + "^FS" +
                            "^FO300,200^AR^FD" + fulldate + "***^FS" +
                            "^PQ1" +
                            "^XZ";
            }
            else if (dpi == 1) //300dpi
            {
                temp = "^XA" +
                            "^PRC                                                                                                                                    " +
                            "^LH0,0^FS                                                                                                                           " +
                            "^LL1225                                                                                                                              " +
                            "^MD0                                                                                                                                  " +
                            "^MNY                                                                                                                                  " +
                            "^LH0,0^FS                                                                                                                           " +
                            "^FO56,382^A0N,55,46^CI13^FR^FDVENDOR:^FS                                                            " +
                            "^FO823,452^A0N,55,46^CI13^FR^FD" + fulldate + "***^FS                                             " +
                            "^BY3,3.0^FO66,70^BCN,120,Y,N,N^FR^FD>:" + part_no + vendor1 + t10 + qty1 + "^FS  " +
                            "^FO56,248^A0N,55,46^CI13^FR^FDPart No : ^FS                                                            " +
                            "^FO268,248^A0N,55,46^CI13^FR^FD" + part_no + "^FS                                                    " +
                            "^FO56,314^A0N,48,39^CI13^FR^FD" + prod + "^FS                                                      " +
                            "^FO262,388^A0N,47,39^CI13^FR^FD"+ vendor1Name +"^FS                                       " +
                            "^FO56,452^A0N,55,46^CI13^FR^FDQTY:^FS                                                                    " +
                            "^FO184,452^A0N,55,46^CI13^FR^FD" + qty2 + "^FS                                                      " +
                            "^PQ1,0,0,N                                                                                                                          " +
                            "^XZ                                                                                                                                      " +
                            "^FX End of job                                                                                                                      " +
                            "^XA                                                                                                                                      " +
                            "^IDR:ID*.*                                                                                                                            " +
                            "^XZ                                                                                                                                      ";
            }

            return temp;
        }


        private void PrintLabel(string docno)
        {
            try
            {
                lpt.Open();
                string sql = "select box_label_uni" +
                    " from pacsd_pm_box_gen " +
                    " where doc_no ='" + docno + "' " +
                    " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' "+
                    " order by box_label_uni desc";
                DataTable dtResult = OracleHelper.ExecuteDataTable(sql);
                DataView dvTree = new DataView(dtResult);
                if (dtResult.Rows.Count > 0)
                {
                    foreach (DataRowView dr in dvTree)
                    {
                        lpt.Write(SetCmdText(dr[0].ToString()));
                    }
                }
                lpt.Close();
            }
            catch (Exception PrintLabel)
            {
                 XtraMessageBox.Show(this, "System error[PrintLabel]: " + PrintLabel.Message);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();  
        }

        private void gridView1_MouseUp(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = this.gridView1.CalcHitInfo(e.Location);
            if (hi.InRow && e.Button == MouseButtons.Right)
            {
                global.CallMenu(gridView1).ShowPopup(Control.MousePosition);
            } 
        }

    }
}