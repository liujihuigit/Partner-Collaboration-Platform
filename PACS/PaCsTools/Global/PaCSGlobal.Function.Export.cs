using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCSTools
{
    public partial class PaCSGlobal
    {

        /// <summary>
        /// 导出文件，多种格式选择
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="reportName"></param>
        /// <param name="fullFileName"></param>
        public static void ExportGridToFile(DevExpress.XtraGrid.Views.Base.BaseView gridView, string reportName, string fullFileName = null)
        {
            string fileName = string.Empty;
            if (string.IsNullOrEmpty(fullFileName))
            {
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.DefaultExt = ".xls";
                dlg.FileName = reportName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss");
                dlg.AddExtension = true;
                dlg.Filter = "Excel2000-2003(*.xls)|*.xls|Excel2007以上(*.xlsx)|*.xlsx|PDF文件(*.pdf)|*.pdf|网页文件(*.html)|*.html|RTF文件(*.rtf)|*.rtf|文本文件(*.txt)|*.txt";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            if (fileName == string.Empty)
            {
                return;
            }
            string extFileName = System.IO.Path.GetExtension(fileName).ToUpper();
            DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();

            switch (extFileName)
            {
                case ".XLSX":
                    gridView.ExportToXlsx(fileName);
                    break;
                case ".PDF":
                    gridView.ExportToPdf(fileName);
                    break;
                case ".HTML":
                    gridView.ExportToHtml(fileName);
                    break;
                case ".RTF":
                    gridView.ExportToRtf(fileName);
                    break;
                case ".TXT":
                    gridView.ExportToText(fileName);
                    break;
                default:
                    gridView.ExportToXls(fileName);
                    break;
            }
            OpenFile(fileName);
        }


        private static void OpenFile(string fileName)
        {
            if (XtraMessageBox.Show("你想打开这个文件吗？?", "文件导出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = fileName;
                    process.StartInfo.Verb = "Open";
                    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    process.Start();
                }
                catch
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("您的系统上无法找到适合打开导出的数据文件的应用程序。");
                }
            }
            //progressBarControl1.Position = 0;
        }



        /// <summary>
        /// 状态栏-OK 显示状态 - Zhao :20150126
        /// </summary>
        /// <param name="panelControl"></param>
        /// <param name="lblStatus"></param>
        /// <param name="str"></param>
        public static void showOK(DevExpress.XtraEditors.PanelControl panelControl, DevExpress.XtraEditors.LabelControl lblStatus, string str)
        {
            lblStatus.Text = str;
            panelControl.BackColor = Color.Yellow;
            panelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            //lblStatus.ForeColor = System.Drawing.Color.White;
        }


        /// <summary>
        /// 状态栏-NG 显示状态  - Zhao :20150126
        /// </summary>
        /// <param name="panelControl"></param>
        /// <param name="lblStatus"></param>
        /// <param name="str"></param>
        public static void showNG(DevExpress.XtraEditors.PanelControl panelControl, DevExpress.XtraEditors.LabelControl lblStatus, string str)
        {
            lblStatus.Text = str;
            panelControl.BackColor = Color.Red;
            panelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            //lblStatus.ForeColor = System.Drawing.Color.Red;
        }



        /// <summary>
        /// 清除Grid 信息 - Zhao : 20150126
        /// </summary>
        /// <param name="panelControl"></param>
        /// <param name="lblStatus"></param>
        /// <param name="str"></param>
        public static void GridViewInitial(GridView gridView, GridControl gridControl)
        {
            gridView.ClearSelection();
            gridView.ClearColumnsFilter();
            gridView.ClearDocument();
            gridView.Columns.Clear();

            gridControl.DataSource = null;
            gridView.RefreshData();
        }




        /// <summary>
        /// Record 数据加载到Combobox - Zhao : 20150126
        /// </summary>
        /// <param name="panelControl"></param>
        /// <param name="lblStatus"></param>
        /// <param name="str"></param>
        public static void RecordToComboBox(string Sql, DevExpress.XtraEditors.ComboBoxEdit cbComBox)
        {
            try
            {
                OracleDataReader odr = OracleHelper.ExecuteReader(Sql);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        cbComBox.Properties.Items.Add(odr[0]);
                    }
                }
            }
            catch (Exception RecordToComboBox)
            {
                XtraMessageBox.Show("RecordToComboBox-" + RecordToComboBox.Message);
            }
        }

       
    }
}
