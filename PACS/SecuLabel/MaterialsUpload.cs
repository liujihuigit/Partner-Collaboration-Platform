using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb; 
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PaCSTools;
using DevExpress.XtraSplashScreen;
using System.Data.OracleClient;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Data;

//using Microsoft.Office.Interop.Excel;

namespace SecuLabel
{

    public partial class MaterialUpload : DevExpress.XtraEditors.XtraForm
    {


        string excelFileName = "";   //要打开的EXCEL 文件名
        string excelFirstTable = ""; //第一个Sheet名字
        public MaterialUpload()
        {
            InitializeComponent();
        }



        private void btnQuery_Click(object sender, EventArgs e)
        {

            try
            {
                SecuGlobal.showOK(panelStatus ,lblStatus ,"数据查询中,请稍等...");
                SecuGlobal.GridViewInitial(grdView1, gridControl1);

                backgroundWorker1.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));
                
            }
            catch (Exception ex)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, ex.Message);
            }

        }




        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string dtFrom = dateEditFrom.Text.Trim().Replace("-", "");
            string dtTo = dateEditTo.Text.Trim().Replace("-", "");
            StringBuilder sql = new StringBuilder(" select  FCT_CODE as \"Fct Code\" , " +
                                                    " sec_boxno as \"Sec Boxno\" , sec_itemcd as \"Sec Itemcd\", sec_rollfrom as \"Sec Rollfrom\",  " +
                                                    " sec_rollto as \"Sec Rollto\", sec_start as \"Sec Start\", sec_end  as \"Sec end\",  " +
                                                    " sec_cnt as \"Sec cnt\", delivery_date  as \"Delivery Date\", invoice_no as \"Invoice No\",  " +
                                                    " add_date as \"Add Date\", add_time as \"Add Time\", add_user as \"Add User\",  " +
                                                    " add_ip as \"Add Ip\",  " +
                                                    " case when   " +
                                                    "   (select count(*) from " + SecuGlobal.tbSecurityInTest  + " b where b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "' and b.roll_no between a.sec_rollfrom and a.sec_rollto) >= 1 then  " +
                                                    "     'Y'  " +
                                                    " else  " +
                                                    "     'N'  " +
                                                    " end 是否入库 " +
                                                    " from " + SecuGlobal.tbSecurityInvoice  + " a  " +
                                                    " where 1 = 1  and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'" );

            if (!string.IsNullOrEmpty(dtFrom) && !string.IsNullOrEmpty(dtTo))
            {
                sql.Append(" and add_date between '" + dtFrom + "' and '" + dtTo + "' ");
            }

            if (!string.IsNullOrEmpty(tbBoxNo.Text))
            {
                sql.Append(" and sec_boxno = '" + tbBoxNo.Text.Trim() + "'");
            }

            if (!string.IsNullOrEmpty(tbRollNo.Text))
            {
                sql.Append(" and '"  + tbRollNo.Text.Trim() + "' between sec_rollfrom and sec_rollto");
            }

            if (!string.IsNullOrEmpty(tbSecuLabel.Text))
            {
                sql.Append(" and sec_start = '" + tbSecuLabel.Text.Trim() + "'");
            }

            sql.Append(" order by sec_boxno  ");

            DataTable dt = OracleHelper.ExecuteDataTable(sql.ToString());
            
            this.Invoke((MethodInvoker)delegate
            {
                gridControl1.DataSource = dt;
                grdView1.BestFitColumns();
                grdView1.Columns["Sec Itemcd"].SummaryItem.SummaryType = SummaryItemType.Count;
                grdView1.Columns["Sec Itemcd"].SummaryItem.DisplayFormat = "共 {0:f0} 条记录";
                SecuGlobal.showOK(panelStatus, lblStatus, "OK");
            });



        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();  
        }




        /// <summary>
        ///基本参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaterialUpload_Load(object sender, EventArgs e)
        {
            SecuGlobal.setDate(dateEditFrom, dateEditTo);
            SecuGlobal.showOK(panelStatus, lblStatus, "Ready");

        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            if (grdView1.RowCount <= 0)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, "没有数据，请先查询！");
            }

            SecuGlobal.showOK(panelStatus, lblStatus, "数据导出中,请稍等...");
            PaCSGlobal.ExportGridToFile(grdView1, "Security Invoice Info");
            SecuGlobal.showOK(panelStatus, lblStatus, "OK");
        }



        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (ckClipBorad.Checked)  //从剪切板获取数据 - 某些EXCEL 格式异常，采用此方式进行
                {
                    if (!bGetDataClipboradAndAnalysis())
                    {
                        return;
                    }
                    else
                    {
                        btnSave.Enabled = true;   // 遍历上传的数据，如果都可用，则Save Enabled = TRUE 
                        SecuGlobal.showOK(panelStatus, lblStatus, "数据读取和分析OK ，请按保存按钮上传数据...");
                        return;
                    }
                }

                SecuGlobal.showOK(panelStatus, lblStatus, "文件打开中,请稍等...");
                SecuGlobal.GridViewInitial(grdView1, gridControl1);

                OpenFileDialog fd = new OpenFileDialog(); 
                fd.Filter = "Excel文件(*.xls;*.xlsx)|*.xls;*.xlsx"; //过滤文件类型
                fd.InitialDirectory = Application.StartupPath + "\\Temp\\";
                fd.ShowReadOnly = true; 

                DialogResult r = fd.ShowDialog();
                if (r == DialogResult.OK)
                {
                    
                    excelFileName = fd.FileName;
                    excelFirstTable = GetExcelFirstTableName(excelFileName);
                    DataTable dt = dtImportExcel(excelFileName, excelFirstTable);

                    gridControl1.DataSource = dt;

                    if (!bAnalysisUploadData(dt))
                    {
                        return;
                    }
                        

                    btnSave.Enabled = true;   // 遍历上传的数据，如果都可用，则Save Enabled = TRUE 
                    SecuGlobal.showOK(panelStatus, lblStatus, "数据读取和分析OK ，请按保存按钮上传数据...");
                }

            }
            catch (Exception btnOpenFile_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnOpenFile_Click.Message);
            }

        }



        private bool bGetDataClipboradAndAnalysis()
        {
            try
            {
                var fmt_csv = System.Windows.Forms.DataFormats.CommaSeparatedValue;  //逗号作为分隔符
                //read the CSV    
                //var dataobject = System.Windows.Forms.Clipboard.GetDataObject();
                //var stream = (System.IO.Stream)dataobject.GetData(fmt_csv);
                //var enc = System.Text.Encoding.GetEncoding(1252);
                //var reader = new System.IO.StreamReader(stream, enc);
                //string data_csv = reader.ReadToEnd();
                //this.richTextBox1.AppendText(data_csv);


                string data_string = System.Windows.Forms.Clipboard.GetText(); //read the Unicode String   \t

                DataTable dt = new DataTable();
                dt.Columns.Add("CODE", typeof(string));
                dt.Columns.Add("ROLL No From", typeof(string));
                dt.Columns.Add("ROLL No To", typeof(string));
                dt.Columns.Add("Security SN From", typeof(string));
                dt.Columns.Add("Security SN To", typeof(string));
                dt.Columns.Add("BOX No", typeof(string));
                dt.Columns.Add("Qtty", typeof(string));
                dt.Columns.Add("DELIVERY DATE", typeof(string));
                dt.Columns.Add("INVOICE NO.", typeof(string));

                string[] arrstr = data_string.Split(new char[] { '\r', '\n' });//截取行
                string[] cells;

                for (int i = 0; i < arrstr.Length; i++)//循环行数
                {
                    cells = arrstr[i].ToString().Split('\t');//取每行中的每一列
                    if (cells.Length.Equals(9))
                    {
                        DataRow rows = dt.NewRow();
                        rows["CODE"] = cells[0];
                        if (!cells[0].Contains("-"))
                        {
                            SecuGlobal.showNG(panelStatus, lblStatus, "资材编号错误，请确认!");
                            return false;
                        }

                        rows["ROLL No From"] = cells[1];
                        rows["ROLL No To"] = cells[2];
                        rows["Security SN From"] = cells[3];
                        rows["Security SN To"] = cells[4];
                        rows["BOX No"] = cells[5];
                        rows["Qtty"] = cells[6].Contains(",") ? cells[6].Replace(",", "") : cells[6];
                        rows["DELIVERY DATE"] = cells[7];
                        rows["INVOICE NO."] = cells[8];
                        dt.Rows.Add(rows);
                    }

                }
                gridControl1.DataSource = dt;   //加载数据

                if (grdView1.RowCount <= 0)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "剪贴板没有复制内容或者复制的列数不符合要求，请在确认！");
                    return false;
                }
                    
                if (!bAnalysisUploadData(dt))  //分析数据
                    return false;

                return true;

            }
            catch (Exception err )
            {
                SecuGlobal.showNG(panelStatus, lblStatus, err.Message );
                return false;
            }
        }





        /// <summary>   
        /// EXCEL 导入DataTable 
        /// </summary>   
        /// <param name="file">要导入的文件</param>   
        /// <returns> DataTable </returns>   
        public static DataTable dtImportExcel(string excelFileName, string tabelName)
        {
            DataTable dtExcel = new DataTable();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + excelFileName +
                ";Extended Properties='Excel 12.0; HDR=YES IMEX=1'"))
            {
                conn.Open();

                string strSql = "select * from [" + tabelName + "]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(strSql, conn);
                adapter.Fill(dtExcel);
                return dtExcel;
            }

        }




        /// <summary>
        /// C#中获取Excel文件的第一个表名 
        /// Excel文件中第一个表名的缺省值是Sheet1$, 但有时也会被改变为其他名字
        /// </summary>
        /// <param name="excelFileName"></param>
        /// <returns>Excel 第一个表名</returns>
        public static string GetExcelFirstTableName(string excelFileName)
        {
            string tableName = null;
            if (File.Exists(excelFileName))
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + excelFileName +
                    ";Extended Properties='Excel 12.0; HDR=YES IMEX=1'"))   //HDR=YES 第一行是标题栏    IMEX=1  只是读取
                {
                    conn.Open();
                    DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    tableName = dt.Rows[0][2].ToString().Trim();
                }
            }
            return tableName;
        }



        /// <summary>
        /// grdview 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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



        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                backgroundWorker2.RunWorkerAsync();
                SplashScreenManager.ShowForm(typeof(WaitLoading));

                btnSave.Enabled = false;
            }
            catch (Exception btnSave_Click)
            {
                SecuGlobal.showNG(panelStatus, lblStatus, btnSave_Click.Message);  //是否要做Rollback？
            }
  
        }






        /// <summary>   
        /// 每一行信息插入数据库中   
        /// </summary>   
        /// <param name="dr">要插入的这一行dt-datarow对象</param>   
        /// <returns>sql语句和用out关键字的参数数组对象</returns>   
        public static string GetSqlString(DataRow dr, out OracleParameter[] parameters)
        {
            StringBuilder sql = new StringBuilder("insert into  " + SecuGlobal.tbSecurityInvoice +
                "(sec_boxno,sec_itemcd,sec_rollfrom,sec_rollto,sec_start,sec_end,sec_cnt,delivery_date,invoice_no,add_date,add_time,add_user,add_ip,FCT_CODE) " +
                "values (:sec_boxno,:sec_itemcd,:sec_rollfrom,:sec_rollto,:sec_start,:sec_end,:sec_cnt,:delivery_date,:invoice_no," +
                "to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24miss'),:add_user,:add_ip,:FCT_CODE)");

            parameters = new OracleParameter[] {
                new OracleParameter(":sec_boxno", Convert.ToString(dr[5])),
                new OracleParameter(":sec_itemcd", Convert.ToString(dr[0])), 
                new OracleParameter(":sec_rollfrom", Convert.ToString(dr[1])), 
                new OracleParameter(":sec_rollto", Convert.ToString(dr[2])), 
                new OracleParameter(":sec_start", Convert.ToString(dr[3])), 
                new OracleParameter(":sec_end", Convert.ToString(dr[4])), 
                new OracleParameter(":sec_cnt", Convert.ToString(dr[6])), 
                new OracleParameter(":delivery_date", Convert.ToString(dr[7])), 
                new OracleParameter(":invoice_no", Convert.ToString(dr[8])), 
                //new OracleParameter(":add_date", to_char(sysdate,'yyyymmdd')), 
                //new OracleParameter(":add_time", "to_char(sysdate,'hh24miss')"), 
                new OracleParameter(":add_user", PaCSTools.PaCSGlobal.LoginUserInfo.Name), 
                new OracleParameter(":add_ip", PaCSTools.PaCSGlobal.GetClientIp()),
                new OracleParameter(":FCT_CODE", PaCSTools.PaCSGlobal.LoginUserInfo.Fct_code)
            };
            return sql.ToString();//将sql return出去   
        } 





        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {

            this.Invoke((MethodInvoker)delegate
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "数据上传中，请耐心等待...");
                try
                {
                    //MessageBox.Show(grdView1.RowCount.ToString());
                    for (int i = 0; i < grdView1.RowCount; i++)
                    {
                        StringBuilder sql = new StringBuilder("insert into  " + SecuGlobal.tbSecurityInvoice +
                        "(sec_boxno,sec_itemcd,sec_rollfrom,sec_rollto,sec_start,sec_end,sec_cnt,delivery_date,invoice_no,add_date,add_time,add_user,add_ip,FCT_CODE) " +
                        "values (:sec_boxno,:sec_itemcd,:sec_rollfrom,:sec_rollto,:sec_start,:sec_end,:sec_cnt,:delivery_date,:invoice_no," +
                        "to_char(sysdate,'yyyymmdd'),to_char(sysdate,'hh24miss'),:add_user,:add_ip,:FCT_CODE)");

                        OracleParameter[] parameters = new OracleParameter[] {
                                    new OracleParameter(":sec_boxno", Convert.ToString(grdView1.GetRowCellValue(i, grdView1.Columns[5]))),
                                    new OracleParameter(":sec_itemcd", Convert.ToString(grdView1.GetRowCellValue(i, grdView1.Columns[0]))), 
                                    new OracleParameter(":sec_rollfrom", Convert.ToString(grdView1.GetRowCellValue(i, grdView1.Columns[1]))), 
                                    new OracleParameter(":sec_rollto", Convert.ToString(grdView1.GetRowCellValue(i, grdView1.Columns[2]))), 
                                    new OracleParameter(":sec_start", Convert.ToString(grdView1.GetRowCellValue(i, grdView1.Columns[3]))), 
                                    new OracleParameter(":sec_end", Convert.ToString(grdView1.GetRowCellValue(i, grdView1.Columns[4]))), 
                                    new OracleParameter(":sec_cnt", Convert.ToString(grdView1.GetRowCellValue(i, grdView1.Columns[6]))), 
                                    new OracleParameter(":delivery_date", Convert.ToString(grdView1.GetRowCellValue(i, grdView1.Columns[7]))), 
                                    new OracleParameter(":invoice_no", Convert.ToString(grdView1.GetRowCellValue(i, grdView1.Columns[8]))), 
                                    //new OracleParameter(":add_date", to_char(sysdate,'yyyymmdd')), 
                                    //new OracleParameter(":add_time", "to_char(sysdate,'hh24miss')"), 
                                    new OracleParameter(":add_user", PaCSTools.PaCSGlobal.LoginUserInfo.Name), 
                                    new OracleParameter(":add_ip", PaCSTools.PaCSGlobal.GetClientIp()),
                                    new OracleParameter(":FCT_CODE", PaCSTools.PaCSGlobal.LoginUserInfo.Fct_code)
                                };
                        OracleHelper.ExecuteNonQuery(sql.ToString(), parameters);
                    }
                    SecuGlobal.showOK(panelStatus, lblStatus, "数据保存 OK");
                }
                catch (Exception DoWork)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, DoWork.Message); // rollback ?
                }
                finally
                {
                    if (ckClipBorad.Checked)  //剪贴板模式恢复到EXCEL模式，
                    {
                        //System.Windows.Forms.Clipboard.Clear(); //剪贴板不能清空，否则会导致数据显示异常
                        ckClipBorad.Checked = false;
                    }
                }

            });

        }




        /// <summary>
        /// 遍历EXCEL（DataTable）
        /// 如果数据已经存在那么ERROR 提示
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>True(数据可用) 否则False</returns>
        private bool bAnalysisUploadData(DataTable dt)
        {
            bool b=false ;
            string ls_sec_boxno  = "";
            string ls_sec_start = "";
            string ls_sec_end = "";
            string strSql = "";
            int iCounts;
            
            foreach (DataRow dr in dt.Rows)
            {
                ls_sec_boxno = Convert.ToString(dr[5]);
                ls_sec_start = Convert.ToString(dr[3]);
                ls_sec_end = Convert.ToString(dr[4]);

                strSql = "select count(*) from " + SecuGlobal.tbSecurityInvoice  + " where sec_boxno = '" + ls_sec_boxno + "' " +
                         "and FCT_CODE = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                iCounts = System.Convert.ToInt32(OracleHelper.ExecuteScalar(strSql));

                if (iCounts>=1)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "已经上载的箱号:" + ls_sec_boxno);
                    break;
                }

                strSql = "select * from " + SecuGlobal.tbSecurityInvoice + "  where '" + ls_sec_start + "' between sec_start and sec_end " +
                         "and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                iCounts = System.Convert.ToInt32(OracleHelper.ExecuteScalar(strSql));
                if (iCounts >= 1)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "已上载的防伪标签号:" + ls_sec_start);
                    break;
                }


                strSql = "select * from " + SecuGlobal.tbSecurityInvoice + " where '" + ls_sec_end + "' between sec_start and sec_end " +
                         "and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code  + "'";
                iCounts = System.Convert.ToInt32(OracleHelper.ExecuteScalar(strSql));
                if (iCounts >= 1)
                {
                    SecuGlobal.showNG(panelStatus, lblStatus, "已上载的防伪标签号:" + ls_sec_end);
                    break;
                }

                b = true;
            }

            return b;
        }




        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm();
        }





        private void ckClipBorad_CheckedChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            SecuGlobal.GridViewInitial(grdView1, gridControl1);
            if (ckClipBorad.Checked)
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "剪贴板方式：打开EXCEL-复制要上载的数据<标题栏除外>，然后点击批量上载");
            }
            else
            {
                SecuGlobal.showOK(panelStatus, lblStatus, "Excel方式");
            }
        }


    }
}