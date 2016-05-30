using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace SecuLabel
{
    public partial class rptSecu : DevExpress.XtraReports.UI.XtraReport
    {
        public rptSecu()
        {
            InitializeComponent();

        }


        public rptSecu(DataSet ds)//构造函数重载 
        { 
            InitializeComponent();
            loadIni();
            SetReportData(ds );
        } 


        private void loadIni()
        {
            xrHeader.Text = "三星电子(山东)数码打印机有限公司";
        }

        private void SetReportData(DataSet ds)
        {
            //表头部分
            DataSource = ds;

            DataTable dt = ds.Tables[1];
            for(int i = 0 ; i < dt.Rows.Count ; i++)     
            {
                string sDate = dt.Rows[i]["UPDATE_DATE"].ToString();   //UPDATE_DATE 时间为空
                string sTime = dt.Rows[i]["UPDATE_TIME"].ToString();

                  this.xrReqDoc.Text = dt.Rows[i]["Req_Doc"].ToString();

                  if (string.IsNullOrEmpty(sDate))
                      this.xrReqDate.Text = "";
                  else
                      this.xrReqDate.Text = sDate.Substring(0, 4) + "-" + sDate.Substring(4, 2) + "-" + sDate.Substring(6, 2);

                  if (string.IsNullOrEmpty(sTime))
                      this.xrReqTime.Text = "";
                  else
                      this.xrReqTime.Text = sTime.Substring(0, 2) + ":" + sTime.Substring(2, 2) + ":" + sTime.Substring(4, 2);
                 
                  

                  this.xrUser.Text = dt.Rows[i]["REQ_USER"].ToString();
                  this.xrDept.Text = dt.Rows[i]["REQ_VENDOR"].ToString();
                  this.xrRemark.Text = dt.Rows[i]["REMARK"].ToString();
            }

            ////明细部分
            this.xrID.DataBindings.Add("Text", DataSource, "REQ_SEQ");
            this.xrAssy.DataBindings.Add("Text", DataSource, "material_code");
            this.xrQty.DataBindings.Add("Text", DataSource, "actual_send_qty");
            this.xrBarcode.DataBindings.Add("Text", DataSource, "barcode_flag");
            this.xrBoxNo.DataBindings.Add("Text", DataSource, "box_no");
            this.xrRollNo.DataBindings.Add("Text", DataSource, "roll_no");
            this.xrSnFrom.DataBindings.Add("Text", DataSource, "security_start");
            this.xrSnTo.DataBindings.Add("Text", DataSource, "security_end");
            this.xrPackQty.DataBindings.Add("Text", DataSource, "qty");


        }

    }
}
