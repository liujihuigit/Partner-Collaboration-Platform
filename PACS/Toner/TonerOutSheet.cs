using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using PaCSTools;
using System.Data;

namespace Toner
{
    public partial class TonerOutSheet : DevExpress.XtraReports.UI.XtraReport
    {
        public TonerOutSheet(string docnos,string vendto,string vendtoCode)
        {
            InitializeComponent();
            this.cellDateTime.Text = PaCSGlobal.GetServerDateTime(6);
            this.cellConfirmVendor.Text = vendto + "确认者";

            if (PaCSGlobal.LoginUserInfo.Fct_code.Equals("C660A"))
            {
                lblTitle.Text = "SSDP TONER 出库 S/N 管控表";
                this.cellConfirmerSamsung.Text = "SSDP确认者";
            }
            else if (PaCSGlobal.LoginUserInfo.Fct_code.Equals("C6H0A"))
            {
                lblTitle.Text = "SESC Copier TONER 出库 S/N 管控表";
                this.cellConfirmerSamsung.Text = "SESC确认者";
            }

            SetDataBind(GetDataSource(docnos, vendtoCode));
        }

        private DataTable GetDataSource(string docnos, string vendtoCode)
        {
            string sql = "  select rownum rn,box_label,lot_no||'-'||box_no sn,item,qty from pacsd_pm_box w where w.fct_code = '"+PaCSGlobal.LoginUserInfo.Fct_code+"' " +
                " and w.final_vend_to = '" + vendtoCode + "'" +
                " and w.final_doc_no in(" + docnos + ") ";//201410140020,201410100028

            DataTable dt = OracleHelper.ExecuteDataTable(sql);
            return dt;
        }

        private void SetDataBind(DataTable dt)//绑定数据源
        {
            this.DataSource = dt;

            this.cellNo.DataBindings.Add("Text", DataSource, "rn");
            this.cellBarcode.DataBindings.Add("Text", DataSource, "box_label");
            this.cellSN.DataBindings.Add("Text", DataSource, "sn");
            this.cellCode.DataBindings.Add("Text", DataSource, "item");
            this.cellQty.DataBindings.Add("Text", DataSource, "qty");
        }

    }
}
