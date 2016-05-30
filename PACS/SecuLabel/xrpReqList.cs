using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
using PaCSTools;

namespace SecuLabel
{
    public partial class xrpReqList : DevExpress.XtraReports.UI.XtraReport
    {
        public xrpReqList()
        {
            InitializeComponent();
        }

        public xrpReqList(DataSet ds)
        {
            InitializeComponent();
            loadIni();
            SetReportData(ds);
        }


        private void loadIni()
        {
           // xrHeader.Text = "三星电子(山东)数码打印机有限公司";
        }


        private void SetReportData(DataSet ds)
        {
            //表头部分
            DataSource = ds;

            DataTable dt = ds.Tables[1];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sReqDate = dt.Rows[i]["REQ_DATE"].ToString();   
                string sProdPlanDate = dt.Rows[i]["Prod_Plan_Date"].ToString();
                if (string.IsNullOrEmpty(sReqDate))
                    this.xrReqDate.Text = "";
                else
                    this.xrReqDate.Text = sReqDate.Substring(0, 4) + "/" + sReqDate.Substring(4, 2) + "/" + sReqDate.Substring(6, 2);

                if (string.IsNullOrEmpty(sProdPlanDate))
                    this.xrPlanDate.Text = "";
                else
                    this.xrPlanDate.Text = sProdPlanDate.Substring(0, 4) + "/" + sProdPlanDate.Substring(4, 2) + "/" + sProdPlanDate.Substring(6, 2);

                this.xrPlant.Text = dt.Rows[i]["plant"].ToString();
                this.xrReqDoc.Text = dt.Rows[i]["Req_Doc"].ToString();
                this.xrBarCode1.Text = dt.Rows[i]["req_doc_barcode"].ToString();
                this.xrVendor.Text = dt.Rows[i]["req_vendor_name"].ToString();
                this.xrUser.Text = dt.Rows[i]["req_user"].ToString();
                this.xrRemark.Text = dt.Rows[i]["remark"].ToString();
            }

            ////明细部分
            this.xrID.DataBindings.Add("Text", DataSource, "req_seq");
            this.xrAssy.DataBindings.Add("Text", DataSource, "material_code");
            this.xrAssyDesc.DataBindings.Add("Text", DataSource, "material_desc");
            this.xrReqQty.DataBindings.Add("Text", DataSource, "req_qty");
            this.xrFactQty.DataBindings.Add("Text", DataSource, "actual_send_qty");
            this.xrBorad.DataBindings.Add("Text", DataSource, "board_count");
            this.xrGRQty.DataBindings.Add("Text", DataSource, "gr_qty");
            this.xrPlanQty.DataBindings.Add("Text", DataSource, "prod_plan_qty");
            this.xrStock.DataBindings.Add("Text", DataSource, "stock_qty");

            this.xrIP.Text  = PaCSGlobal.GetClientIp();
        }


    }
}
