using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PaCSTools
{
    /// <summary>
    /// 打印方法：
    /// </summary>
    public partial class PaCSGlobal
    {

        /// <summary>
        /// 打印数据表格
        /// </summary>
        /// <param name="_PrintHeader">标题</param>
        /// <param name="gridControl">数据表控件</param>
        public static void PrintGridData(string _PrintHeader, IPrintable obj)
        {
            PrintingSystem print = new DevExpress.XtraPrinting.PrintingSystem();
            // print.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            PrintableComponentLink link = new PrintableComponentLink(print);
            print.Links.Add(link);
            link.Component = obj;//这里可以是可打印的部件
            PageHeaderFooter phf = link.PageHeaderFooter as PageHeaderFooter;
            phf.Header.Content.Clear();
            phf.Header.Content.AddRange(new string[] { "", _PrintHeader, "" });
            phf.Header.Font = new System.Drawing.Font("宋体", 14, System.Drawing.FontStyle.Bold);
            phf.Header.LineAlignment = BrickAlignment.Center;
            link.CreateDocument(); //建立文档
            print.PreviewFormEx.PrintingSystem.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            print.PreviewFormEx.Show();//进行预览
        }

        /// <summary>
        /// 打印数据表格
        /// </summary>
        /// <param name="_PrintHeader">标题</param>
        /// <param name="gridControl">数据表控件</param>
        /// <param name="papgerKind">纸张类型</param>
        public static void PrintGridData(string _PrintHeader, IPrintable obj, PaperKind papgerKind)
        {
            PrintingSystem print = new DevExpress.XtraPrinting.PrintingSystem();
            // print.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            PrintableComponentLink link = new PrintableComponentLink(print);
            print.Links.Add(link);
            link.Component = obj;//这里可以是可打印的部件
            PageHeaderFooter phf = link.PageHeaderFooter as PageHeaderFooter;
            phf.Header.Content.Clear();
            phf.Header.Content.AddRange(new string[] { "", _PrintHeader, "" });
            phf.Header.Font = new System.Drawing.Font("宋体", 14, System.Drawing.FontStyle.Bold);
            phf.Header.LineAlignment = BrickAlignment.Center;
            link.CreateDocument(); //建立文档
            print.PreviewFormEx.PrintingSystem.PageSettings.PaperKind = papgerKind;
            print.PreviewFormEx.Show();//进行预览
        }

        /// <summary>
        /// 打印报表
        /// </summary>
        /// <param name="report">报表文件</param>
       public static void PrintReport(XtraReport report)
        {
            ReportPrintTool pts = new ReportPrintTool(report);
            pts.Print();
        }
    }
}
