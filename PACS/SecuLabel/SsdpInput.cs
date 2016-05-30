using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PaCSTools;
using System.IO.Ports;
using DevExpress.Data;

namespace SecuLabel
{
    public partial class SsdpInput : DevExpress.XtraEditors.XtraForm
    {
        private SerialPort[] ports = new SerialPort[1];
        private delegate void InvokeDelegate(string data);
        private string receivedata = "";

        string vendorCode = "";  // 4位Vendor - D5G9
        string secuType = "";    // SAMSUNG / XEROX
        string inputType = "";  // ROLL / ONE

        string ls_item = ""; // 当前材料
        int li_sec_cnt = 0;  // 投入数量
        string ls_line = ""; // LINE 信息
        string li_sec_sn;    // 数据序号信息
        string ls_sn_from = "", ls_sn_to = "";
        string secBuyer = "SEC";
        public SsdpInput()
        {
            ports[0] = new SerialPort();
            InitializeComponent();
        }



        /// <summary>
        /// Com 口数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPortSecuSsdpInput_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                System.Threading.Thread.Sleep(100); //读取速度慢，加Sleep延长读取时间, 不可缺少
                //serialPort1.DiscardInBuffer();  //如果不执行上面的代码,serialPort1_DataReceived会执行多次

                int n = ports[0].BytesToRead;
                byte[] buf = new byte[n];
                ports[0].Read(buf, 0, n);
               // ports[0].ReceivedBytesThreshold = 15; // 在检讨~~

                receivedata = System.Text.Encoding.ASCII.GetString(buf);
                receivedata = receivedata.Replace("\r\n", "");

                try
                {
                    this.Invoke(new InvokeDelegate(DoData), receivedata);
                }
                catch (Exception serialPortSecuSsdpInput_DataReceived)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        ////要委托的代码 
                        //lbStatus.Text = "【" + receivedata + "】：出库失败";
                        XtraMessageBox.Show(this, "System error[serialPortSecuSsdpInput_DataReceived]: " + serialPortSecuSsdpInput_DataReceived.Message);
                    }));
                }
            }
        }


        /// <summary>
        /// 扫描枪设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnComSeting_Click(object sender, EventArgs e)
        {
            SettingForm setcom = new SettingForm("SecuLabel", "SsdpInput", 1);
            DialogResult dg = setcom.ShowDialog();

            if (dg == DialogResult.OK)
            {
                PaCSGlobal.InitComPort("SecuLabel", "SsdpInput", ports);

                if (ports[0].IsOpen)
                    ports[0].DataReceived += new SerialDataReceivedEventHandler(serialPortSecuSsdpInput_DataReceived);//重新绑定
            }
        }



        /// <summary>
        /// 回车事件-手输入或者USB扫描
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122)
            {
                e.KeyChar = (char)((int)e.KeyChar - 32);
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                doWork(tbSn.Text.Trim());  // 读取的信息分析处理
            }
        }

        private void tbSnFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122)
            {
                e.KeyChar = (char)((int)e.KeyChar - 32);
            }
        }

        private void tbSnTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 122)
            {
                e.KeyChar = (char)((int)e.KeyChar - 32);
            }
        }



        /// <summary>
        /// 从Com口获取的数据进行相应操作
        /// </summary>
        /// <param name="data"></param>
        private void DoData(string data)
        {
           

            tbSn.Text = data;
            
            doWork(tbSn.Text.Trim());  // 读取的信息分析处理
        }



        /// <summary>
        /// COM口接收的数据进行处理
        /// </summary>
        /// <param name="data"></param>
        private void doWork(string data)
        {
            try
            {
                receivedata = "";
                if (!bAnalysisData_Comm(data))
                    return;

                SecuGlobal.showOK(panelStatus, lblStatus, "读取条码OK，正在分析上传数据,请稍等...");
                switch (inputType)
                {
                    case "ONE": //单个投入

                        if (!QtyInputCheck_One(data))    //分析 基本条件 满足返回TRUE 
                            return;

                        if (!bInsertData_One(data))
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "数据Insert 异常，请联系管理员");
                            return;
                        }

                        if (!bCheckData_One(data))
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "数据Check 异常-ONE，请联系管理员");
                            return;
                        }
                        SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                        break;

                    case "ROLL":  //按卷投入
                        if (string.IsNullOrEmpty(tbSnFrom.Text.Trim()))
                        {
                            SecuGlobal.showOK(panelStatus, lblStatus, "起始序列号读取成功");
                            tbSnFrom.Text = data;
                            ls_sn_from = tbSnFrom.Text.Trim();
                        }
                        else
                        {
                            if ((ls_sn_from.Length.Equals(15) && ls_sn_from.Substring(0, 7).Equals(data.Substring(0, 7))) ||
                                (ls_sn_from.Length.Equals(9)) && ls_sn_from.Substring(0, 2).Equals(data.Substring(0, 2)))
                            {
                                tbSnTo.Text = data;
                                ls_sn_to = tbSnTo.Text.Trim();
                                if (bProcessingSnRange(data))
                                {
                                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                                }
                                tbSnFrom.Text = "";
                            }
                            else
                            {
                                SecuGlobal.showNG(panelStatus, lblStatus, "序列号前几位不一致，请重新扫描!");
                            }
                        }
                        break;
                }
                showData(vendorCode, li_sec_sn); //显示数据
            }
            catch (Exception err)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, err.Message);
                return;
            }
            finally
            {
                tbSn.Text = ""; tbSnTo.Text = ""; ls_item = ""; ls_line = "";   //相关变量初始化
                li_sec_cnt = 0;
                li_sec_sn = "";
                ls_sn_to = "";
            }
        }



        private void showData(string vendCode,string sec_sn)
        {
            SecuGlobal.GridViewInitial(gridView1, grdControl1);
            string sql = " select serial_NO ,vendor ,sec_sn ,add_datetime from " + SecuGlobal.tbSnSecuHist  + "   " +
                         " where vendor = '" + vendCode + "' and sec_sn = '" + sec_sn + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' order by serial_no ";
            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            grdControl1.DataSource = dt;


            gridView1.Columns["SERIAL_NO"].SummaryItem.SummaryType = SummaryItemType.Count;
            gridView1.Columns["SERIAL_NO"].SummaryItem.DisplayFormat = "All： {0:f0} ";
        }




        /// <summary>
        /// 按卷投入时，数据操作
        /// </summary>
        /// <param name="scandata"></param>
        private bool  bProcessingSnRange(string scandata)
        {
            if (!QtyInputCheck_Roll())
                return false ;

            if (!bInsertData_Roll())
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "数据Insert 异常，请联系管理员");
                return false ;
            }

            if (!bCheckData_Roll(scandata))
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "数据Check 异常-Roll，请联系管理员");
                return false ;
            }
            return true;
        }



        /// <summary>
        /// 分析基本条件
        /// </summary>
        /// <param name="scandata"></param>
        /// <returns></returns>
        private bool bAnalysisData_Comm(string scandata)
        {

            if (cbLine.SelectedIndex == -1 || cbMaterialCode.SelectedIndex == -1 )
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "请选择基本信息- line / 材料");
                return false;
            }

            ls_item = cbMaterialCode.Text.Trim(); // 投入材料
            li_sec_cnt = System.Convert.ToInt32(tbQty.Text.Trim()); // 投入数量
            if (li_sec_cnt == 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "投入数量不能为零");
                return false;
            }

            ls_line = cbLine.Text.Trim().Substring(0, 4);
            if (string.IsNullOrEmpty(ls_line) || (string.IsNullOrEmpty(ls_item)) || ls_item.Equals("ALL"))
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "请选择LINE 信息 或者 材料CODE 信息");
                return false ;
            }

            string sql = "select count(*) from " + SecuGlobal.tbSnSecuHist + " where serial_no = '" + scandata + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
            int li_cnt = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql));
            if (li_cnt > 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "序列号重复扫描!");
                return false ;
            }

            string sql1 = "select count(*) from " + SecuGlobal.tbSecurityInSnTest  + " where serial_no = '" + scandata + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'";
            int li_cnt1 = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql1));
            if (li_cnt1 <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有发料的序列号!");
                return false;
            }

            if (secuType.Equals("SAMSUNG") && !scandata.Length.Equals(15))
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "错误的序列号:" + scandata);
                return false ;
            }

            if (secuType.Equals("XEROX") && !scandata.Length.Equals(9))
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "错误的序列号:" + scandata);
                return false ;
            }
            return true;
        }




        /// <summary>
        /// 验证扫描的数量和计算的数量是否一致
        /// </summary>
        /// <param name="scandata"></param>
        /// <returns></returns>
        private bool QtyInputCheck_One(string scandata)
        {
            int li_sec_calc_sum = 0;
            if (inputType.Equals("ONE"))
            {
                if (secuType.Equals("SAMSUNG"))
                {
                    string sql1 = "select to_number(substr('" + scandata + "',8,8)) - to_number(substr('" + scandata + "',8,8)) + 1 from dual";
                    li_sec_calc_sum = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql1));   //根据SN计算数量
                }
                else if (secuType.Equals("XEROX"))
                {
                    string sql2 = "select to_number(substr('" + scandata + "',3,7)) - to_number(substr('" + scandata + "',3,7)) + 1 from dual";
                    li_sec_calc_sum = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql2));
                }

                if (!li_sec_calc_sum.Equals(li_sec_cnt))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "计算数量与设定数量不一致，不允许保存！");
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }





       /// <summary>
       /// 验证扫描的SN范围计算的数量和设定的数量是否一致
       /// </summary>
       /// <returns></returns>
        private bool QtyInputCheck_Roll()
        {
            int li_sec_calc_sum = 0;
            if (inputType.Equals("ROLL"))
            {
                if (secuType.Equals("SAMSUNG"))
                {
                    string sql1 = "select to_number(substr('" + ls_sn_to + "',8,8)) - to_number(substr('" + ls_sn_from + "',8,8)) + 1 from dual";
                    li_sec_calc_sum = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql1));
                }
                else if (secuType.Equals("XEROX"))
                {
                    string sql2 = "select to_number(substr('" + ls_sn_to + "',3,7)) - to_number(substr('" + ls_sn_from + "',3,7)) + 1 from dual";
                    li_sec_calc_sum = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql2));
                }

                if (!li_sec_calc_sum.Equals(li_sec_cnt))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "计算数量与设定数量不一致，不允许保存！");
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }






        /// <summary>
        /// 数据UPLOAD
        /// </summary>
        /// <param name="scanData"></param>
        /// <returns></returns>
        private bool bInsertData_One(string scanData)
        {
            try
            {
                string sqlSeq = "select seq_secu_id.nextval from dual";
                li_sec_sn = OracleHelper.ExecuteScalar(sqlSeq).ToString();   // Sequece 信息

                string sqlInsert = "";
                if (secuType.Equals("SAMSUNG"))
                {
                    sqlInsert = " insert into " + SecuGlobal.tbSnSecuHist  + "(serial_no,vendor,sec_sn,add_datetime,FCT_CODE) " +
                                        " select substr('" + scanData + "',1,7)||lpad(to_number(substr('" + scanData + "',8,8))+rownum-1,8,0), " +
                                        "          '" + vendorCode + "', " +
                                        "          '" + li_sec_sn + "', " +
                                        "          to_char(sysdate,'yyyymmddhh24miss'),'" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                                        "   from dual " +
                                        " connect by rownum <= '" + li_sec_cnt + "' ";

                }
                else if (secuType.Equals("XEROX"))
                {
                    sqlInsert = " insert into " + SecuGlobal.tbSnSecuHist  + "(serial_no,vendor,sec_sn,add_datetime,FCT_CODE) " +
                                        " select substr('" + scanData + "',1,2)||lpad(to_number(substr('" + scanData + "',3,7))+rownum-1,7,0), " +
                                        "          '" + vendorCode + "', " +
                                        "          '" + li_sec_sn + "', " +
                                        "          to_char(sysdate,'yyyymmddhh24miss'),'" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                                        "   from dual " +
                                        " connect by rownum <= '" + li_sec_cnt + "' ";
                }

                OracleHelper.ExecuteNonQuery(sqlInsert);
                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }




        private bool bCheckData_One(string data )
        {
            try
            {
                ls_sn_from = data;
                ls_sn_to = data;
                string sqlCheck = " select count(*) " +
                    " from " + SecuGlobal.tbSecurityInSnTest + " " +
                    " where serial_no in (select serial_no from " + SecuGlobal.tbSnSecuHist + " " +
                             "where vendor = '" + vendorCode + "' and sec_sn = '" + li_sec_sn + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') " +
                    " and status = 'OUT'    ";
                int li_check_qty = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sqlCheck));
                if (!li_check_qty.Equals(li_sec_cnt))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "数量检验错误，请联系管理员!");
                    return false ;
                }


                // sec_cnt序列号数量   sec_calc_sum//计算数量   store2_in// 都写入相同的数值 ？
                string sqlInset = " insert into " + SecuGlobal.tbPopSecuIn  + "  " +
                                " (vendor,sec_sn,sec_cdate,sec_ctime,sec_itemcd,sec_cnt,sec_start,sec_end,sec_sign,sec_calc_sum,store2_in,sec_buyer,line,FCT_CODE) " +
                                " values " +
                                " ( " +
                                " '" + vendorCode + "','" + li_sec_sn + "',to_char(sysdate,'yyyymmdd'),to_char(sysdate,'HH24Miss'), " +
                                " '" + ls_item + "','" + li_sec_cnt + "','" + ls_sn_from + "','" + ls_sn_to + "', " +
                                " '" + PaCSGlobal.LoginUserInfo.Name + "','" + li_sec_cnt + "','" + li_sec_cnt + "','" + secBuyer + "','" + ls_line + "','" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                                " ) ";
                OracleHelper.ExecuteNonQuery(sqlInset);

                string sqlUpdate = " update " + SecuGlobal.tbSecurityInSnTest + " " +
                                    " set status = 'INPUT', " +
                                    "   shop = '" + ls_line + "' " +
                                    " where serial_no in (select serial_no from " + SecuGlobal.tbSnSecuHist + " " +
                                    " where vendor = '" + vendorCode + "' and sec_sn = '" + li_sec_sn + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
                OracleHelper.ExecuteNonQuery(sqlUpdate);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }








        /// <summary>
        /// 分析数据类型
        /// </summary>
        /// <returns></returns>
        private bool bGetSnRange(string data)
        {
            try
            {
                if (inputType.Equals("ROLL"))
                {
                    if (string.IsNullOrEmpty(tbSnFrom.Text.Trim()))
                    {
                        SecuGlobal.showOK(panelStatus, lblStatus, "起始序列号读取成功");
                        tbSnFrom.Text = data;
                        ls_sn_from = tbSnFrom.Text.Trim();
                    }
                    else
                    {
                        if ((ls_sn_from.Length.Equals(15) && ls_sn_from.Substring(0, 7).Equals(data.Substring(0, 7))) ||
                            (ls_sn_from.Length.Equals(9)) && ls_sn_from.Substring(0, 2).Equals(data.Substring(0, 2)))
                        {
                            tbSnTo.Text = data;
                            ls_sn_to = tbSnTo.Text.Trim();
                            SecuGlobal.showOK(panelStatus, lblStatus, "SN范围读取成功，数据处理中,请稍等...");
                        }
                        else
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "序列号前几位不一致，请重新扫描!");
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }






        /// <summary>
        /// 分析数据类型
        /// </summary>
        /// <returns></returns>
        private bool bInsertData_Roll()
        {
            try
            {
                string sqlSeq = "select seq_secu_id.nextval from dual";
                li_sec_sn = OracleHelper.ExecuteScalar(sqlSeq).ToString();   // Sequece 信息

                string sqlInsert = "";
                if (secuType.Equals("SAMSUNG"))
                {
                    sqlInsert = " insert into " + SecuGlobal.tbSnSecuHist + "(serial_no,vendor,sec_sn,add_datetime,FCT_CODE) " +
                                        " select substr('" + ls_sn_from + "',1,7)||lpad(to_number(substr('" + ls_sn_from + "',8,8))+rownum-1,8,0), " +
                                        "          '" + vendorCode + "', " +
                                        "          '" + li_sec_sn + "', " +
                                        "          to_char(sysdate,'yyyymmddhh24miss'),'" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                                        "   from dual " +
                                        " connect by rownum <= '" + li_sec_cnt + "' ";

                }
                else if (secuType.Equals("XEROX"))
                {
                    sqlInsert = " insert into " + SecuGlobal.tbSnSecuHist  + "(serial_no,vendor,sec_sn,add_datetime,FCT_CODE) " +
                                        " select substr('" + ls_sn_from + "',1,2)||lpad(to_number(substr('" + ls_sn_from + "',3,7))+rownum-1,7,0), " +
                                        "          '" + vendorCode + "', " +
                                        "          '" + li_sec_sn + "', " +
                                        "          to_char(sysdate,'yyyymmddhh24miss'),'" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                                        "   from dual " +
                                        " connect by rownum <= '" + li_sec_cnt + "' ";
                }

                OracleHelper.ExecuteNonQuery(sqlInsert);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }






        /// <summary>
        /// 分析数据类型
        /// </summary>
        /// <returns></returns>
        private bool bCheckData_Roll(string data)
        {
            try
            {
                string sqlCheck = " select count(*) " +
                    " from " + SecuGlobal.tbSecurityInSnTest + " " +
                    " where serial_no in (select serial_no from " + SecuGlobal.tbSnSecuHist + " " +
                             "where vendor = '" + vendorCode + "' and sec_sn = '" + li_sec_sn + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') " +
                    " and status = 'OUT'  and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'  ";
                int li_check_qty = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sqlCheck));
                if (!li_check_qty.Equals(li_sec_cnt))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "数量检验错误，请联系管理员!");
                    return false;
                }


                // sec_cnt序列号数量   sec_calc_sum//计算数量   store2_in// 都写入相同的数值 ？
                string sqlInset = " insert into " + SecuGlobal.tbPopSecuIn + "  " +
                                " (vendor,sec_sn,sec_cdate,sec_ctime,sec_itemcd,sec_cnt,sec_start,sec_end,sec_sign,sec_calc_sum,store2_in,sec_buyer,line,FCT_CODE) " +
                                " values " +
                                " ( " +
                                " '" + vendorCode + "','" + li_sec_sn + "',to_char(sysdate,'yyyymmdd'),to_char(sysdate,'HH24Miss'), " +
                                " '" + ls_item + "','" + li_sec_cnt + "','" + ls_sn_from + "','" + ls_sn_to + "', " +
                                " '" + PaCSGlobal.LoginUserInfo.Name + "','" + li_sec_cnt + "','" + li_sec_cnt + "','" + secBuyer + "','" + ls_line + "','" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                                " ) ";
                OracleHelper.ExecuteNonQuery(sqlInset);

                string sqlUpdate = " update " + SecuGlobal.tbSecurityInSnTest + " " +
                                    " set status = 'INPUT', " +
                                    "   shop = '" + ls_line + "' " +
                                    " where serial_no in (select serial_no from " + SecuGlobal.tbSnSecuHist + " " +
                                    " where vendor = '" + vendorCode + "' and sec_sn = '" + li_sec_sn + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "') and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' ";
                OracleHelper.ExecuteNonQuery(sqlUpdate);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }




        /// <summary>
        /// LOAD 事件-材料、LINE等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SsdpInput_Load(object sender, EventArgs e)
        {
            PaCSGlobal.InitComPort("SecuLabel", "SsdpInput", ports);

            if (ports[0].IsOpen)
                ports[0].DataReceived += new SerialDataReceivedEventHandler(serialPortSecuSsdpInput_DataReceived);//重新绑定

            if (PaCSGlobal.LoginUserInfo.Fct_code.Equals("C660A"))
                cbPlant.Text = "SSDP";
            else
                cbPlant.Text = "SESC";


            string bufVend = SecuGlobal.getPopVendorInfo(PaCSGlobal.LoginUserInfo.Venderid, PaCSGlobal.LoginUserInfo.Fct_code);
            if (!bufVend.Equals(""))
            {
                cbVendor.Text = bufVend;
            }
            else
            {
                cbVendor.Text = PaCSGlobal.LoginUserInfo.Venderid + ":" + PaCSGlobal.LoginUserInfo.Vendername;  //苏州法人
            }


            rdInput.SelectedIndex = 1; //默认是按卷投入
            getVendorCode4();
            SecuGlobal.getMeterialCode(cbMaterialCode);  // 获取材料编号信息tb_security_master
            SecuGlobal.getLineInfo(cbLine, vendorCode);
            rdType_SelectedIndexChanged(sender, e);
            rdInput_SelectedIndexChanged(sender, e);
            SecuGlobal.showOK(panelStatus, lblStatus, "Ready ! 当前类型：" + secuType + "投入类型：" + inputType);

        }



        /// <summary>
        /// 获取四位VENDOR CODE - EXP:　BP3A
        /// </summary>
        private void getVendorCode4()
        {
            vendorCode = cbVendor.Text.Trim();
            if (!string.IsNullOrEmpty(vendorCode))
            {
                if (!vendorCode.Equals("ALL"))
                {
                    string[] split = vendorCode.Split(new Char[] { ':' });
                    vendorCode = split[0].Trim();
                }
            }
        }





        /// <summary>
        /// 类型选择-三星向/XEROX
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            inputType = "";
            tbQty.Text = "";
            if (rdInput.SelectedIndex == 0)
            {
                inputType = "ONE";
                tbQty.Text = "1";
            }
            else if (rdInput.SelectedIndex == 1)
            {
                inputType = "ROLL";
                if(secuType.Equals("SAMSUNG"))
                {
                    tbQty.Text = "2500";
                }
                else if(secuType.Equals("XEROX"))
                {
                    tbQty.Text = "10000";
                } 
            }
            else
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "获取类型异常错误");
            }
            SecuGlobal.showOK(panelStatus, lblStatus, "Ready ! 当前类型：" + secuType + "投入类型：" + inputType);
        }




        /// <summary>
        /// 成卷/ONE 选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdType_SelectedIndexChanged(object sender, EventArgs e)
        {
            secuType = "";
            tbQty.Text = "";
            if (rdType.SelectedIndex == 0)
            {
                secuType = "SAMSUNG";
                if (rdInput.SelectedIndex == 1)
                {
                    tbQty.Text = "2500";
                }
                else
                {
                    tbQty.Text = "1";
                }
            }
            else if (rdType.SelectedIndex == 1)
            {
                secuType = "XEROX";
                if (rdInput.SelectedIndex == 1)
                {
                    tbQty.Text = "10000";
                }
                else
                {
                    tbQty.Text = "1";
                }
            }
            else
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "获取类型异常错误");
            }

            SecuGlobal.showOK(panelStatus, lblStatus, "Ready ! 当前类型：" + secuType + "投入类型：" + inputType);

        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            if (e.Info.IsRowIndicator)
            {
                if (e.RowHandle >= 0)
                {
                    e.Info.DisplayText = (e.RowHandle + 1).ToString();
                }
                else if (e.RowHandle < 0 && e.RowHandle > -1000)
                {
                    e.Info.Appearance.BackColor = System.Drawing.Color.AntiqueWhite;
                    e.Info.DisplayText = "G" + e.RowHandle.ToString();
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //showData("C660", "75619"); //显示数据

            if(gridView1.RowCount <=0 )
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有数据");
                return;
            }
            PaCSGlobal.ExportGridToFile(gridView1, "Input Sn");
        }

        private void SsdpInput_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ports[0].DataReceived -= new SerialDataReceivedEventHandler(serialPortSecuSsdpInput_DataReceived);//取消绑定

                foreach (SerialPort port in ports)
                {
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                }
            }
            catch (Exception IssueAssy_FormClosing)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, IssueAssy_FormClosing.ToString());
            }
        }





    }
}