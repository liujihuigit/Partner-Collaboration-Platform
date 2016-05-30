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
using System.Data.OracleClient;
using DevExpress.XtraEditors.Controls;
using System.IO.Ports;
using DevExpress.Data;

namespace SecuLabel
{
    public partial class BarcodeGr : DevExpress.XtraEditors.XtraForm
    {
        string is_security_type = "";
        string strUnitType = "";


        string workDate = "";
        int iS353_ITEM = 0, i353L_ITEM = 0, iS353_BOX_NO = 0, i353L_BOX_NO = 0, iS353_ROLL_FROM = 0, i353L_ROLL_FROM = 0, iS353_ROLL_TO = 0, i353L_ROLL_TO = 0;
        int iS353_LOT_NO = 0, i353L_LOT_NO = 0, iS353_QTY = 0, i353L_QTY = 0, iS353_SEC_START = 0, i353L_SEC_START = 0, iS353_SEC_END = 0, i353L_SEC_END = 0;

        int iS354_ITEM = 0, i354L_ITEM = 0, iS354_ROLL_NO = 0, i354L_ROLL_NO = 0;
        int iS354_LOT_NO = 0, i354L_LOT_NO = 0, iS354_QTY = 0, i354L_QTY = 0, iS354_SEC_START = 0, i354L_SEC_START = 0, iS354_SEC_END = 0, i354L_SEC_END = 0;

        int iS355_SEC_START = 0, i355L_SEC_START = 0, iS355_SEC_END = 0, i355L_SEC_END = 0, iS355_SEQ_START = 0, i355L_SEQ_START = 0;
        int iS355_SEQ_END = 0, i355L_SEQ_END = 0;


        string item, box_no, roll_from, roll_to, lot_no, security_start, security_end, qty;
        string roll_no, sec_from, sec_to, scan_data_gene;
        string allQty = "";
        int rollCount = 0;

        string doc_no = "";
        string newStart = "", newEnd = "";



        private delegate void InvokeDelegate(string data);

        private string receivedata = "";
        PaCSGlobal global = new PaCSGlobal();
        private SerialPort[] ports = new SerialPort[1];
        public BarcodeGr()
        {
            InitializeComponent();
            ports[0] = new SerialPort();
        }

        private void BarcodeGr_Load(object sender, EventArgs e)
        {
            radioGroup1_SelectedIndexChanged(sender, e);
            getAssyList();
            Init();
            //SecuGlobal.showOK(panelStatus, lblStatus, "Ready");
        }


        /// <summary>
        /// Com口初始化
        /// </summary>
        private void Init()
        {

            PaCSGlobal.InitComPort("SecuLabel", "SecuGr", ports);

            if (ports[0].IsOpen)
                ports[0].DataReceived += new SerialDataReceivedEventHandler(serialPortSecuGr_DataReceived);//重新绑定
            
        }


        /// <summary>
        /// Com 口数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPortSecuGr_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                System.Threading.Thread.Sleep(100); //读取速度慢，加Sleep延长读取时间, 不可缺少
                //serialPort1.DiscardInBuffer();  //如果不执行上面的代码,serialPort1_DataReceived会执行多次
                int n = ports[0].BytesToRead;
                byte[] buf = new byte[n];
                ports[0].Read(buf, 0, n);
               // ports[0].ReceivedBytesThreshold = 31;

                receivedata = System.Text.Encoding.ASCII.GetString(buf);
                receivedata = receivedata.Replace("\r\n", "");

                try
                {
                    this.Invoke(new InvokeDelegate(DoData), receivedata);
                }
                catch (Exception serialPortSecuGr_DataReceived)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        //lbStatus.Text = receivedata + "：入库失败";
                        XtraMessageBox.Show(this, "System error[serialPortSecuGr_DataReceived]: " + serialPortSecuGr_DataReceived.Message);
                    }));
                }
            }
        }






        /// <summary>
        /// 获取资材编号，添加到ComboBox
        /// </summary>
        private void getAssyList()
        {
            try
            {
                string sql = "select MATERIAL_CODE from " + SecuGlobal.tbMaster  + " where barcode_flag = 'Y' and " +
                             "FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code   + "' order by MATERIAL_CODE ";
               OracleDataReader odr =  OracleHelper.ExecuteReader(sql);

               if (odr.HasRows)
               {

                   cbAssyAdd.Properties.Items.Clear();
                   while(odr.Read())
                   {
                       cbAssyAdd.Properties.Items.Add(odr["MATERIAL_CODE"]);
                   }
               }
               else
               {
                   SecuGlobal.showNG(panelStatus, lblStatus, "没有Master信息，请联系仓库担当");
               }

            }
            catch (Exception getAssyList)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, getAssyList.Message);
            }
            
        }


        /// <summary>
        /// 获取截取字符串的必要条件
        /// </summary>
        /// <param name="type"></param>
        private void getScanSplitValue(string type )
        {
            try
            {
                string sql = "select code 描述,etc1 开始长度,etc2 截取长度 from " + SecuGlobal.tb_code + "  where FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' and lang_key = 'EN'  and prefix = '" + type + "' order by seq_no";
                DataTable  dt = OracleHelper.ExecuteDataTable (sql);
                grdControl2.DataSource = dt;
                grdScanSetting.Columns["描述"].Width = 120;

            }
            catch (Exception getScanSplitValue)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, getScanSplitValue.Message);
            }
            

        }




        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string docno = tbBoxNo.Text;
                if (docno.Equals(""))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请输入 Doc No ..");
                    return ;
                }
                showData(docno);
            }
            catch (Exception btnQuery_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnQuery_Click.Message);
            }

        }


        /// <summary>
        /// 改变DataTable 标题栏及排列顺序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable setDtHeader(DataTable dt)
        {

            string[] colField = { "fct_code", "doc_no", "doc_seq", "item", "qty", "box_no", 
                                    "roll_no","lot_no","security_start","security_end" ,"security_type","unit","gr_date","gr_time","gr_user","gr_ip"};

            string[] colName = { "Fct Code", "Doc No", "Doc Seq", "Item", "Qty", "Box No", 
                                    "Roll No","Lot No","Security Start","Security End" ,"Security Type","Unit","Gr Date","Gr Time","Gr User","Gr Ip"};

            //int[] showIndex = { 0, 1, 2, 3, 4, 5, 6, 7 };

            for (int i = 0; i< colField.Length ; i++)
            {
                dt.Columns[colField[i]].ColumnName = colName[i];
                dt.Columns[colName[i]].SetOrdinal(i);

            }

            return dt;

        }




        /// <summary>
        /// 获取标签类型-全局SAMSUNG / XEROX
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioGroup1.SelectedIndex == 0)    
            {
                is_security_type = "SAMSUNG";
                radioGroup2.Properties.Items[0].Enabled = true ;
                radioGroup2.SelectedIndex = 0;
            }
            else
            {
                is_security_type = "XEROX";
                radioGroup2.SelectedIndex = 1;
                radioGroup2.Properties.Items[0].Enabled = false;  
            }
            doc_no = ""; // 三星和XEROX 的单号分开

            cbAssyAdd.Text = "";
            radioGroup2_SelectedIndexChanged_1(sender, e);
        }






        /// <summary>
        /// 根据选择内容，获取截取字符串
        /// 设置基本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioGroup2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            tbQty.Text = "";
            switch (is_security_type)
            {
                case "SAMSUNG":
                    if (radioGroup2.SelectedIndex == 0) //整箱
                    {
                        strUnitType = "BOX";
                        tbQty.Text = "20000";
                        getScanSplitValue("353");
                    }
                    else if (radioGroup2.SelectedIndex == 1) //整卷
                    {
                        strUnitType = "ROLL";
                        tbQty.Text = "2500";
                        getScanSplitValue("354");
                    }
                    else //单个
                    {
                        strUnitType = "ONE";
                        tbQty.Text = "1";
                        SecuGlobal.GridViewInitial(grdScanSetting, grdControl2);
                    }
                    break;

                case "XEROX":
                    if (radioGroup2.SelectedIndex == 1) //整卷
                    {
                        strUnitType = "ROLL";
                        tbQty.Text = "10000";
                        getScanSplitValue("355");
                    }
                    else if (radioGroup2.SelectedIndex == 2) //单个
                    {
                        strUnitType = "ONE";
                        tbQty.Text = "1";
                    }
                    break;
            }
            clear();

            SecuGlobal.showOK(panelStatus, lblStatus, "投入类型为:" + is_security_type + " < " + strUnitType + " >"  );
        }




        private void btnComSetting_Click(object sender, EventArgs e)
        {
            SettingForm setcom = new SettingForm("SecuLabel", "SecuGr", 1);
            DialogResult dg = setcom.ShowDialog();

            if (dg == DialogResult.OK)
            {
                PaCSGlobal.InitComPort("SecuLabel", "SecuGr", ports);

                if (ports[0].IsOpen)
                    ports[0].DataReceived += new SerialDataReceivedEventHandler(serialPortSecuGr_DataReceived);//重新绑定
            }
        }

   



        /// <summary>
        /// 获取Doc No - 申请单信息
        /// </summary>
        /// <param name="Type"></param>
        /// <returns>Doc No</returns>
        public static string GetDocNo(string workdate)
        {
            string sql = "select '" + workdate + "'||fn_gene_seq('SECU','IN','" + workdate + "','N','N','N',4) from dual";

            OracleDataReader odr = OracleHelper.ExecuteReader(sql);
            if (!odr.HasRows)
                return "";

            if (odr.Read())
            {
                return odr.GetString(0);
            }
            else
            {
                return "";
            }
        }



        /// <summary>
        /// 获取Doc Seq - 序号信息
        /// </summary>
        /// <param name="Type"></param>
        /// <returns>Doc Seq</returns>
        public static string GetDocSeq(string docNo)
        {
            string sql = "select fn_gene_seq('SECU','INSEQ','" + docNo + "','N','N','N',4) from dual";

            OracleDataReader odr = OracleHelper.ExecuteReader(sql);
            if (odr.Read())
            {
                return odr.GetString(0);
            }
            else
                return "";
        }





        /// <summary>
        /// 拆分的ASSY CODE 进行判定，然后判断
        /// 申请单号获取
        /// </summary>
        /// <returns></returns>
        private bool AnalysisCondition(string data)
        {
            try
            {
                string sql = "select count(*) from " + SecuGlobal.tbSecurityInTest  + " where scan_data = '" + data + "' " +
                             "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";

                int a = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql));
                if (a > 0)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "数据已经上传，请在确认！");
                    return false;
                }

                if (string.IsNullOrEmpty(cbAssyAdd.Text))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请选择材料信息");
                    return false;
                }


                workDate = PaCSGlobal.GetServerDateTime(2);
                if (string.IsNullOrEmpty(workDate) || !workDate.Length.Equals(8))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "当前日期获取失败,请联系管理担当");
                    return false;
                }

                //doc_no = GetDocNo(workDate);
                if (string.IsNullOrEmpty(doc_no))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "申请单号获取失败,请联系管理担当");
                    return false;
                }


                if (string.IsNullOrEmpty(strUnitType))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "类型获取失败,BOX/ROLL/ONE?");
                    return false;
                }

                return true;
            }
            catch(Exception )
            {
                return false;
            }
        }






        private DataTable  getDt_353(string scanData)
        {
            try
            {
                    item = scanData.Substring(iS353_ITEM - 1, i353L_ITEM);                           //JC68-01673A 
                    box_no = scanData.Substring(iS353_BOX_NO - 1, i353L_BOX_NO);                      // 3310
                    roll_from = scanData.Substring(iS353_ROLL_FROM - 1, i353L_ROLL_FROM);              // 26473
                    roll_to = scanData.Substring(iS353_ROLL_TO - 1, i353L_ROLL_TO);                   // 26480
                    rollCount = (int.Parse(roll_to) - int.Parse(roll_from)) + 1;

                    lot_no = scanData.Substring(iS353_LOT_NO - 1, i353L_LOT_NO);                    //3C7BECDND5

                    allQty = scanData.Substring(iS353_QTY - 1, i353L_QTY).Replace(",", "");   //20000
                    security_start = scanData.Substring(iS353_SEC_START - 1, i353L_SEC_START);         //SECD4AH66180001
                    security_end = scanData.Substring(iS353_SEC_END - 1, i353L_SEC_END);             //SECD4AH66200000
                    qty = (int.Parse(allQty) / rollCount).ToString();

                    int i = item.IndexOf('-');
                    if (!item.Length.Equals(11) || item.Contains('-') == false || i != 4)
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "资材编号信息错误，请确认截取位数设置信息");
                        return null ;
                    }

                    if (!item.Equals(cbAssyAdd.Text))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "当前型号与扫描型号不一致");
                        return null ;
                    }

                    if (!tbQty.Text.Equals(allQty))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "设定数量与扫描数量不一致");
                        return null ;
                    }

                    if (!security_start.Substring(0,6).Equals(security_end.Substring(0,6)))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "前七位不一致");
                        return null ;
                    }


                    string sql3 = "select count(*) from " + SecuGlobal.tbSecurityInvoice   + " where '" + security_start + "' between sec_start and sec_end " +
                                  "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";

                    int count = System.Convert.ToInt32(OracleHelper.ExecuteScalar(sql3));
                    if (count <= 0)
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "未上载防伪标签明细!请联系采购担当");
                        return null;
                    }


                    string sql = " select '" + roll_from + "' + lv - 1 rollno, " +
                                " substr('" + security_start + "',1,7)||(substr('" + security_start + "',8,8)+ " + allQty + "/" + rollCount + "*(lv-1)) secu_from, " +
                                " substr('" + security_start + "',1,7)||(substr('" + security_start + "',8,8)+ " + allQty + "/" + rollCount + "*(lv)-1) secu_to " +
                                " from  " +
                                " ( " +
                                " select level lv " +
                                " from dual " +
                                " connect by level <= " + rollCount + " " +
                                        " ) ";

                    DataTable dt = OracleHelper.ExecuteDataTable(sql);

                    if (dt != null)
                    {
                        SecuGlobal.showOK(panelStatus, lblStatus, "SN验证OK，正在上传数据..");
                        return dt;
                    }

                    else
                    {
                        return null;
                    }
                        
                    
            }
            catch (Exception)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "请确认类型选择 或者 截取参数设置");
                return null;
            }
        }








        private bool  bDataUpload_353(DataTable dt,string data)
        {

            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string doc_seq = GetDocSeq(doc_no);
                    if (string.IsNullOrEmpty(doc_seq))
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "获取Doc Seq 失败，请联系管理员");
                        return false;
                    }

                    roll_no = dr["rollno"].ToString();
                    sec_from = dr["secu_from"].ToString();
                    sec_to = dr["secu_to"].ToString();

                    string sql2 = " select '" + item + "'||' '||'" + roll_no + "'||' '||'" + lot_no + "'||' '|| " +
                    " to_char(" + allQty + "/" + rollCount + ",'fm9g999g999')||' '||'" + sec_from + "'||' '||'" + sec_to + "'  " +
                    " from dual ";

                    OracleDataReader odr = OracleHelper.ExecuteReader(sql2);
                    if (odr.Read())
                    {
                        scan_data_gene = odr.GetString(0);
                    }
                    else
                    {
                        SecuGlobal.showNG(panelStatus, lblStatus, "获取scan_data_gene 失败，请联系管理员");
                        return false;
                    }
                    insert(doc_seq,data );  
                }

                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                return true;

            }
            catch (Exception bDataUpload_353)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, bDataUpload_353.Message );
                return false;
            }
        }




        /// <summary>
        /// 数据上传，全局数据初始化
        /// </summary>
        /// <param name="doc_seq"></param>
        private bool  insert(string doc_seq,string data)
        {

            try
            {
                string strSql = "insert into  " + SecuGlobal.tbSecurityInTest +
                " (DOC_NO,DOC_SEQ,SCAN_DATA,SECURITY_TYPE,UNIT,ITEM,QTY,BOX_NO,ROLL_NO,LOT_NO, " +
                " SECURITY_START,SECURITY_END,GR_DATE,GR_TIME,GR_USER,GR_IP,SCAN_DATA_GENE,FCT_CODE) " +
                "  values " +
                " (:DOC_NO,:DOC_SEQ,:SCAN_DATA,:SECURITY_TYPE,:UNIT,:ITEM,:QTY,:BOX_NO,:ROLL_NO,:LOT_NO," +
                " :SECURITY_START,:SECURITY_END,to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24miss'),:GR_USER,:GR_IP,:SCAN_DATA_GENE,:FCT_CODE) ";
                OracleParameter[] parameters =  new OracleParameter[] {
                                                new OracleParameter(":DOC_NO", doc_no),
                                                new OracleParameter(":DOC_SEQ", doc_seq), 
                                                new OracleParameter(":SCAN_DATA", data), 
                                                new OracleParameter(":SECURITY_TYPE", "SAMSUNG"), 
                                                new OracleParameter(":UNIT", strUnitType), 
                                                new OracleParameter(":ITEM", item), 
                                                new OracleParameter(":QTY", qty), 
                                                new OracleParameter(":BOX_NO", box_no), 
                                                new OracleParameter(":ROLL_NO", roll_no), 
                                                new OracleParameter(":LOT_NO", lot_no), 
                                                new OracleParameter(":SECURITY_START", sec_from), 
                                                new OracleParameter(":SECURITY_END", sec_to), 
                                                new OracleParameter(":GR_USER", PaCSGlobal.LoginUserInfo.Name  ), 
                                                new OracleParameter(":GR_IP", PaCSGlobal.GetClientIp()),
                                                new OracleParameter(":SCAN_DATA_GENE", scan_data_gene),
                                                new OracleParameter(":FCT_CODE", PaCSGlobal.LoginUserInfo.Fct_code)
                                                };

                OracleHelper.ExecuteNonQuery(strSql, parameters);

                string sql = " insert  into " + SecuGlobal.tbSecurityInSnTest  + "(serial_no,doc_no,doc_seq,status,FCT_CODE) " +
                            " select substr('" + sec_from + "',1,7)||lpad(to_number(substr('" + sec_from + "',8,8))+rownum-1,8,0), " +
                            "          '" + doc_no + "', " +
                            "          '" + doc_seq + "', " +
                            "          'IN','" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                            " from dual " +
                            " connect by rownum <= '" + allQty + "'/'" + rollCount  + "' ";

                OracleHelper.ExecuteNonQuery(sql);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private void clear()
        {
            tbSn.Text = "";
            item = ""; box_no = ""; roll_from = ""; roll_to = ""; lot_no = ""; security_start = ""; security_end=""; qty = "";
            roll_no = ""; sec_from = ""; sec_to = ""; scan_data_gene = "";
            allQty = "";
            rollCount = 0;
            //doc_no = "";
        }




        /// <summary>
        /// 从Com口获取的数据进行相应操作
        /// </summary>
        /// <param name="data"></param>
        private void DoData(string data)
        {

            try
            {
                if (string.IsNullOrEmpty(doc_no))  // 一个批次一个单号即可
                {
                    string workDate = PaCSGlobal.GetServerDateTime(2);
                    doc_no = GetDocNo(workDate);
                }
                MessageBox.Show(doc_no);

                if (!AnalysisCondition(data))
                    return;

                switch (strUnitType)
                {
                    case "BOX":  //box Samsung
                        if (!getSlpitValue("353"))
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "条形码截取基本参数获取错误");
                            return;
                        }
                        DataTable dt = getDt_353(data);
                        if (dt == null)
                        {
                            return;
                        }

                        if (!bDataUpload_353(dt, data))
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "数据上传失败，请联系管理员-353");
                            return;
                        }
                        break;

                    case "ROLL": // roll
                        if (radioGroup1.SelectedIndex == 0)  // samsung roll
                        {
                            //if (getSlpitValue("354"))
                            //{
                            //    SecuGlobal.showNG(panelStatus, lblStatus, "条形码截取基本参数获取错误");
                            //    return;
                            //}
                            MessageBox.Show("请将此类型的条形码反馈给管理员，联系管理员增加此类型的判定逻辑");
                            return;
                        }
                        else
                        {
                            if (!getSlpitValue("355"))
                            {
                                SecuGlobal.showNG(panelStatus, lblStatus, "条形码截取基本参数获取错误");
                                return;
                            }

                            if (!bgetData_355(data))
                                return;

                            if (!insertXerox(data))
                                return;

                        }
                        break;

                    case "ONE": // 单个
                        MessageBox.Show("请将此类型的条形码反馈给管理员，联系管理员增加此类型的判定逻辑");
                        break;
                }
                if (!string.IsNullOrEmpty(doc_no))
                {

                    showData(doc_no);
                    SecuGlobal.showOK(panelStatus, lblStatus, "OK");
                }

            }
            catch (Exception)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "异常错误，请联系管理员-UnitType<Box/Roll/One ??>");
            }
            finally
            {
                clear();  //全局变量初始化
            }
        }





        private bool insertXerox(string data)
        {

            try
            {
                string doc_seq = GetDocSeq(doc_no);
                item = cbAssyAdd.Text.Trim();

                string strSql = "insert into  " + SecuGlobal.tbSecurityInTest +
                " (DOC_NO,DOC_SEQ,SCAN_DATA,SECURITY_TYPE,UNIT,ITEM,QTY,BOX_NO,ROLL_NO,LOT_NO, " +
                " SECURITY_START,SECURITY_END,GR_DATE,GR_TIME,GR_USER,GR_IP,SCAN_DATA_GENE,FCT_CODE) " +
                "  values " +
                " (:DOC_NO,:DOC_SEQ,:SCAN_DATA,:SECURITY_TYPE,:UNIT,:ITEM,:QTY,:BOX_NO,:ROLL_NO,:LOT_NO," +
                " :SECURITY_START,:SECURITY_END,to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24miss'),:GR_USER,:GR_IP,:SCAN_DATA_GENE,:FCT_CODE) ";
                OracleParameter[] parameters = new OracleParameter[] {
                                                new OracleParameter(":DOC_NO", doc_no),
                                                new OracleParameter(":DOC_SEQ", doc_seq), 
                                                new OracleParameter(":SCAN_DATA", data), 
                                                new OracleParameter(":SECURITY_TYPE", "XEROX"), 
                                                new OracleParameter(":UNIT", strUnitType), 
                                                new OracleParameter(":ITEM", item), 
                                                new OracleParameter(":QTY", qty), 
                                                new OracleParameter(":BOX_NO", box_no), 
                                                new OracleParameter(":ROLL_NO", roll_no), 
                                                new OracleParameter(":LOT_NO", lot_no), 
                                                new OracleParameter(":SECURITY_START", newStart), 
                                                new OracleParameter(":SECURITY_END", newEnd), 
                                                new OracleParameter(":GR_USER", PaCSGlobal.LoginUserInfo.Name), 
                                                new OracleParameter(":GR_IP", PaCSGlobal.GetClientIp()),
                                                new OracleParameter(":SCAN_DATA_GENE", scan_data_gene),
                                                new OracleParameter(":FCT_CODE", PaCSGlobal.LoginUserInfo.Fct_code )
                                                };

                OracleHelper.ExecuteNonQuery(strSql, parameters);

                string sql = " insert  into " + SecuGlobal.tbSecurityInSnTest + "(serial_no,doc_no,doc_seq,status,FCT_CODE) " +
                            " select substr('" + newStart + "',1,2 )||lpad(to_number(substr('" + newStart + "',3,7))+rownum-1,7,0), " +
                            "          '" + doc_no + "', " +
                            "          '" + doc_seq + "', " +
                            "          'IN','" + PaCSGlobal.LoginUserInfo.Fct_code  + "' " +
                            " from dual " +
                            " connect by rownum <= '" + qty + "' ";

                OracleHelper.ExecuteNonQuery(sql);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }






        /// <summary>
        /// XEROX 向分离数据并验证
        /// </summary>
        /// <param name="scanData"></param>
        /// <returns></returns>
        private bool  bgetData_355(string scanData)
        {
            try
            {

                string sql = "select distinct item from " + SecuGlobal.tbSecurityInTest  + " where security_type = 'SAMSUNG' " +
                             "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                OracleDataReader dr =  OracleHelper.ExecuteReader(sql);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (cbAssyAdd.Text.Equals(dr["item"].ToString()))
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus,cbAssyAdd.Text + "此型号系统检测为SAMSUNG 类型使用的，请在确认");
                            return false;
                        }
                    }
                }


                security_start = scanData.Substring(iS355_SEC_START - 1, i355L_SEC_START);         //BC2980001
                security_end = scanData.Substring(iS355_SEC_END - 1, i355L_SEC_END);               //BC2990000

                if (security_start.Contains("-") || security_end.Contains("-"))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "请确认截取长度");
                    return false;
                }


                if (security_start.Contains(" "))
                    newStart = security_start.Replace(" ", "");
                else
                    newStart = security_start;


                if (security_end.Contains(" "))
                    newEnd = security_end.Replace(" ", "");
                else
                    newEnd = security_end;

                if (newStart.Length != 9)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "截取内容长度错误，请确认截取参数- security_start");
                    return false;
                }

                if (newEnd.Length != 9)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "截取内容长度错误，请确认截取参数- security_end");
                    return false;
                }

                int startCount = System.Convert.ToInt32(scanData.Substring(iS355_SEQ_START - 1, i355L_SEQ_START));
                int endCount = System.Convert.ToInt32(scanData.Substring(iS355_SEQ_END - 1, i355L_SEQ_END));
                qty = (endCount - startCount + 1).ToString();

                if (!tbQty.Text.Equals(qty))
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "数量与扫描数量不一致");
                    return false;
                }
                return true;

            }
            catch (Exception)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "请确认截取长度");
                return false;
            }
        }






        /// <summary>
        /// 获取截取条形码的基本参数
        /// </summary>
        /// <param name="strType"></param>
        /// <returns>True / False </returns>
        private bool  getSlpitValue(string bufType)
        {
            bool result=false ;
            try
            {
                if (grdScanSetting.RowCount == 0)
                    result = false ;


                for (int i = 0; i < grdScanSetting.RowCount; i++)
                {
                    object val = grdScanSetting.GetRowCellValue(i, grdScanSetting.Columns[0]);
                    object valStart = grdScanSetting.GetRowCellValue(i, grdScanSetting.Columns[1]);
                    object valEnd = grdScanSetting.GetRowCellValue(i, grdScanSetting.Columns[2]);

                    if (string.IsNullOrEmpty(val.ToString().Trim()))
                        result = false; ;

                    if (bufType.Equals("353"))
                        setValue("353", valStart, valEnd, val);

                    if (bufType.Equals("354"))
                        setValue("354", valStart, valEnd, val);

                    if (bufType.Equals("355"))
                        setValue("355", valStart, valEnd, val);
                    
                    if (bufType.Equals("356"))
                        setValue("356", valStart, valEnd, val);
                }

                switch (bufType)
                {
                    case "353":

                        if (iS353_ITEM > 0 && i353L_ITEM > 0 && iS353_BOX_NO > 0 && i353L_BOX_NO > 0 && iS353_ROLL_FROM > 0 && i353L_ROLL_FROM > 0
                                && iS353_ROLL_TO > 0 && i353L_ROLL_TO > 0 && iS353_LOT_NO > 0 && i353L_LOT_NO > 0 && iS353_QTY > 0 && i353L_QTY > 0
                                && iS353_SEC_START > 0 && i353L_SEC_START > 0 && iS353_SEC_END > 0 && i353L_SEC_END > 0)
                            result = true;
                        else
                            result = false;
                        break;

                    case "354":
                        if (iS354_ITEM > 0 && i354L_ITEM > 0 && iS354_ROLL_NO > 0 && i354L_ROLL_NO > 0 && iS354_LOT_NO > 0 && i354L_LOT_NO > 0
                            && iS354_QTY > 0 && i354L_QTY > 0 && iS354_SEC_START > 0 && i354L_SEC_START > 0 && iS354_SEC_END > 0 && i354L_SEC_END > 0)
                            result = true;
                        else
                            result = false;
                        break;

                    case "355":
                        if (iS355_SEC_START > 0 && i355L_SEC_START > 0 && iS355_SEC_END > 0 && i355L_SEC_END > 0 && iS355_SEQ_START > 0 && i355L_SEQ_START > 0
                            && iS355_SEQ_END > 0 && i355L_SEQ_END > 0)
                            result = true;
                        else
                            result = false;
                        break;
                }

                return result;
            }
            catch
            {
                return result;
            }
        }





   
        

        /// <summary>
        /// 设置截取条件
        /// </summary>
        /// <param name="bufType"></param>
        /// <param name="valStart"></param>
        /// <param name="valEnd"></param>
        /// <param name="buf"></param>
        private void setValue(string bufType,object valStart,object valEnd, object buf)
        {

            switch(bufType)
            {
                case "353":
                            switch (buf.ToString().Trim())
                            {
                                case "ITEM":
                                    iS353_ITEM = System.Int32.Parse(valStart.ToString());
                                    i353L_ITEM = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "BOX_NO":
                                    iS353_BOX_NO = System.Int32.Parse(valStart.ToString());
                                    i353L_BOX_NO = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "ROLL_FROM":
                                    iS353_ROLL_FROM = System.Int32.Parse(valStart.ToString());
                                    i353L_ROLL_FROM = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "ROLL_TO":
                                    iS353_ROLL_TO = System.Int32.Parse(valStart.ToString());
                                    i353L_ROLL_TO = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "LOT_NO":
                                    iS353_LOT_NO = System.Int32.Parse(valStart.ToString());
                                    i353L_LOT_NO = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "QTY":
                                    iS353_QTY = System.Int32.Parse(valStart.ToString());
                                    i353L_QTY = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "SEC_START":
                                    iS353_SEC_START = System.Int32.Parse(valStart.ToString());
                                    i353L_SEC_START = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "SEC_END":
                                    iS353_SEC_END = System.Int32.Parse(valStart.ToString());
                                    i353L_SEC_END = System.Int32.Parse(valEnd.ToString());
                                    break;
                            }
                    break ;
                case "354":
                            switch (buf.ToString().Trim())
                            {
                                case "ITEM":
                                    iS354_ITEM = System.Int32.Parse(valStart.ToString());
                                    i354L_ITEM = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "ROLL_NO":
                                    iS354_ROLL_NO = System.Int32.Parse(valStart.ToString());
                                    i354L_ROLL_NO = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "LOT_NO":
                                    iS354_LOT_NO = System.Int32.Parse(valStart.ToString());
                                    i354L_LOT_NO = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "QTY":
                                    iS354_QTY = System.Int32.Parse(valStart.ToString());
                                    i354L_QTY = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "SEC_START":
                                    iS354_SEC_START = System.Int32.Parse(valStart.ToString());
                                    i354L_SEC_START = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "SEC_END":
                                    iS354_SEC_END = System.Int32.Parse(valStart.ToString());
                                    i354L_SEC_END = System.Int32.Parse(valEnd.ToString());
                                    break;
                            }
                    break ;
                case "355":
                            switch (buf.ToString().Trim())
                            {

                                case "SEC_START":
                                    iS355_SEC_START = System.Int32.Parse(valStart.ToString());
                                    i355L_SEC_START = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "SEC_END":
                                    iS355_SEC_END = System.Int32.Parse(valStart.ToString());
                                    i355L_SEC_END = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "SEQ_START":
                                    iS355_SEQ_START = System.Int32.Parse(valStart.ToString());
                                    i355L_SEQ_START = System.Int32.Parse(valEnd.ToString());
                                    break;
                                case "SEQ_END":
                                    iS355_SEQ_END = System.Int32.Parse(valStart.ToString());
                                    i355L_SEQ_END = System.Int32.Parse(valEnd.ToString());
                                    break;

                            }
                    break ;
            }

        }



        /// <summary>
        /// 扫描数据处理
        /// 大写、USB扫描枪考虑
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
                DoData(tbSn.Text.Trim());  // 读取的信息分析处理

            }
        }




        /// <summary>
        /// 显示数据到GRID，申请单号为查询条件
        /// </summary>
        /// <param name="doc_no"></param>
        private void showData(string doc_no)
        {
            try
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "正在查询数据，请稍等...");
                SecuGlobal.GridViewInitial(gridView2, gridControl1);

                string sql = " select fct_code, doc_no, doc_seq, item, qty,  " +
                            "        box_no, roll_no, lot_no,  " +
                            "        security_start, security_end, security_type,  " +
                            "        unit, gr_date, gr_time,  " +
                            "        gr_user, gr_ip " +
                            " from " + SecuGlobal.tbSecurityInTest + " " +
                            " where doc_no = '" + doc_no + "' and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code + "'" +
                            " order by doc_seq desc ";

                DataTable dt = OracleHelper.ExecuteDataTable(sql);
                if (dt == null)
                {
                    return;
                }

                dt = setDtHeader(dt);
                gridControl1.DataSource = dt;
                gridView2.BestFitColumns();

                gridView2.Columns["Doc No"].SummaryItem.SummaryType = SummaryItemType.Count;
                gridView2.Columns["Doc No"].SummaryItem.DisplayFormat = "All： {0:f0} ";

                gridView2.Columns["Qty"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                gridView2.Columns["Qty"].Width = 100;
                gridView2.Columns["Qty"].SummaryItem.SummaryType = SummaryItemType.Sum;
                gridView2.Columns["Qty"].SummaryItem.DisplayFormat = "{0:f0} ";

                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            }
            catch (Exception showData)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, showData.Message );
            }
        }





        /// <summary>
        /// 数据导出EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (gridView2.RowCount <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "No Data ,请查询后导出数据");
                return;
            }

            try
            { 
                SecuGlobal.showOK(panelStatus, lblStatus, "正在导出数据，请稍等...");
                PaCSGlobal.ExportGridToFile(gridView2, "Secu Info");
                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            }
            catch (Exception btnExport_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnExport_Click.Message );
            }


        }

        private void cbAssyAdd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAssyAdd.SelectedIndex != -1)
                SecuGlobal.showOK(panelStatus, lblStatus, "材料选择发生变化，请确认后扫描数据");

        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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



        /// <summary>
        /// 窗体关闭释放COM资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarcodeGr_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ports[0].DataReceived -= new SerialDataReceivedEventHandler(serialPortSecuGr_DataReceived);//取消绑定

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