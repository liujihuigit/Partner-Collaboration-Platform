using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using PaCSTools;
using System.Globalization;

namespace Toner
{
    public partial class Trend : DevExpress.XtraEditors.XtraForm
    {
        public Trend()
        {
            InitializeComponent();
            tbFrom.Text = InitLast6Week();
            tbTo.Text = InitWeek();
        }

        private void Init()
        {
            string weekFrom = tbFrom.Text.Trim();
            string weekTo = tbTo.Text.Trim();

            ChartTitle title = new ChartTitle();
            title.Text = "G/I to Line Trend";
            chartControl1.Titles.Clear();
            chartControl1.Titles.Add(title);
            chartControl1.Series.Clear();

            DataTable dt = GetData(weekFrom, weekTo);

            Series series1 = new Series("SSDP", ViewType.Line);
            series1.ArgumentScaleType = ScaleType.Qualitative;

            foreach (DataRow dr in dt.Select("vendName='SSDP'"))
            {
                series1.Points.Add(new SeriesPoint(dr["week"].ToString(), new double[] { Convert.ToDouble(dr["qty"]) }));
            }

            ((PointSeriesView)series1.View).PointMarkerOptions.Kind = MarkerKind.Circle;
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            ((LineSeriesView)series1.View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;



            Series series2 = new Series("成宇硒鼓", ViewType.Line);
            series2.ArgumentScaleType = ScaleType.Qualitative;

            foreach (DataRow dr in dt.Select("vendName='成宇硒鼓'"))
            {
                series2.Points.Add(new SeriesPoint(dr["week"].ToString(), new double[] { Convert.ToDouble(dr["qty"]) }));
            }

            ((PointSeriesView)series2.View).PointMarkerOptions.Kind = MarkerKind.Triangle;
            series2.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            ((LineSeriesView)series2.View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;



            Series series3 = new Series("阿科帝斯", ViewType.Line);
            series3.ArgumentScaleType = ScaleType.Qualitative;

            foreach (DataRow dr in dt.Select("vendName='阿科帝斯'"))
            {
                series3.Points.Add(new SeriesPoint(dr["week"].ToString(), new double[] { Convert.ToDouble(dr["qty"]) }));
            }

            ((PointSeriesView)series3.View).PointMarkerOptions.Kind = MarkerKind.Diamond;
            series3.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            ((LineSeriesView)series3.View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;

            chartControl1.Series.Add(series1);
            chartControl1.Series.Add(series2);
            chartControl1.Series.Add(series3);
            chartControl1.Legend.Visible = true;
        }

        private DataTable GetData(string weekFrom,string weekTo)
        {
            StringBuilder sql = new StringBuilder("select vend_nm_cn vendName,"+
            " substr(to_char(to_date(a.update_date,'yyyymmdd'),'iyyyiw'),0,4)||'-'||substr(to_char(to_date(a.update_date,'yyyymmdd'),'iyyyiw'),5,2)||'W' week,sum(qty) qty " +
            " from pacsp_pm_box_prgs a, " +
            "  pacsm_md_vend b" +
            " where a.move_type = '261'" +
            " and a.vend_to = b.vend_code "+
            " and a.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' "+
            " and b.fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ");

            DataTable dt = Get(weekFrom,weekTo);
            sql.Append(" and a.update_date between '" + dt.Rows[0][0].ToString() + "' and '" + dt.Rows[0][1].ToString() + "'");

            sql.Append(" group by vend_nm_cn,to_char(to_date(a.update_date,'yyyymmdd'),'iyyyiw')");
            sql.Append(" order by week");
            
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());
            return dtResult;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string weekFrom = tbFrom.Text.Trim();
            string weekTo = tbTo.Text.Trim();
            if (string.IsNullOrEmpty(weekFrom) || string.IsNullOrEmpty(weekTo))
            {
                XtraMessageBox.Show("区间不能为空!", "提示");
                return;
            }

            DataTable dt = GetData(weekFrom,weekTo);
            Init();
        }

         private DataTable Get(string weekFrom, string weekTo)
         {
             StringBuilder sql = new StringBuilder("select min(from_day) weekFrom,max(to_day) weekTo from tb_week where 1=1");

             sql.Append(" and week between '" + weekFrom + "' and '" + weekTo + "'");

             DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());
             return dtResult;
         }

        private string InitWeek()
         {
             string sql = "select to_char(sysdate,'iyyyiw') thisweek from dual";
             DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());
             return dtResult.Rows[0][0].ToString();
         }

        private string InitLast6Week()
        {
            string sql = "select to_char(sysdate-42,'iyyyiw') thisweek from dual";
            DataTable dtResult = OracleHelper.ExecuteDataTable(sql.ToString());
            return dtResult.Rows[0][0].ToString();
        }

    }
}