using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using PaCSTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCSTools
{
    public partial class SettingForm : XtraForm
    {
        private DataTable dt = new DataTable();
        private string module_name;
        private string form_name;
        private int count;
        private string regPath;

        public SettingForm(string module_name,string form_name,int count)
        {
            InitializeComponent();

            this.module_name = module_name;
            this.form_name = form_name;
            this.count = count;
            this.regPath = module_name + "\\" + form_name; 

            Init();
        }

        private void Init()
        {
            DataColumn dc = null;
            dc = dt.Columns.Add("Com Port", Type.GetType("System.String"));
            dc = dt.Columns.Add("Baud Rate", Type.GetType("System.String"));
            dc = dt.Columns.Add("Suffix", Type.GetType("System.String"));

            for (int i = 1; i < 17; i++)
            {
                if (!string.IsNullOrEmpty(PaCSGlobal.GetRegistryValue("COM" + i, regPath)))
                {
                    DataRow dr = dt.NewRow();
                    dr["Com Port"] = "COM" + i;
                    dr["Baud Rate"] = PaCSGlobal.GetRegistryValue("COM" + i, regPath).Split(',')[0];
                    dr["Suffix"] = PaCSGlobal.GetRegistryValue("COM" + i, regPath).Split(',')[1];
            

                    dt.Rows.Add(dr);
                }
            }

            RepositoryItemComboBox cmbCom = new RepositoryItemComboBox();
            cmbCom.TextEditStyle = TextEditStyles.DisableTextEditor;
            RepositoryItemComboBox cmbRate = new RepositoryItemComboBox();
            cmbRate.TextEditStyle = TextEditStyles.DisableTextEditor;
            RepositoryItemComboBox cmbSfx = new RepositoryItemComboBox();
            cmbSfx.TextEditStyle = TextEditStyles.DisableTextEditor;

            for (int i = 1; i < 17; i++)
            {
                cmbCom.Items.Add("COM" + i);
            }

            cmbRate.Items.Add("2400");
            cmbRate.Items.Add("4800");
            cmbRate.Items.Add("9600");
            cmbRate.Items.Add("14400");
            cmbRate.Items.Add("19200");
            cmbRate.Items.Add("38400");
            cmbRate.Items.Add("57600");
            cmbRate.Items.Add("115200");
            cmbRate.Items.Add("128000");

            cmbSfx.Items.Add("ETX");
            cmbSfx.Items.Add("LF");
            cmbSfx.Items.Add("CR");
            cmbSfx.Items.Add("CRLF");
            cmbSfx.Items.Add("CRFF");
            cmbSfx.Items.Add("NONE");

            gridControl1.DataSource = dt;
            gridView1.Columns[0].Width = 120;
            gridView1.Columns[1].Width = 120;
            gridView1.Columns[0].ColumnEdit = cmbCom;
            gridView1.Columns[1].ColumnEdit = cmbRate;
            gridView1.Columns[2].ColumnEdit = cmbSfx;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridView1.RowCount > count)
                {
                    XtraMessageBox.Show("com port数不能多于"+count+"个!", "提示");
                    return;
                }

                //删除Toner原注册表值
                PaCSGlobal.DeleteRegistryValue(regPath);

                //保存目前的注册表值
                for (int i = 0; i < gridView1.RowCount; i++)
                {                  
                    string com = gridView1.GetRowCellValue(i, gridView1.Columns["Com Port"]).ToString();
                    string rate = gridView1.GetRowCellValue(i, gridView1.Columns["Baud Rate"]).ToString() +","+ gridView1.GetRowCellValue(i, gridView1.Columns["Suffix"]).ToString();
                // string sfx = 

                    //成功打开后，记录到注册表中
                    PaCSGlobal.SetRegistryValue(com, rate, regPath);
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;

                XtraMessageBox.Show("保存成功", "提示");
            }
            catch (Exception btnSave_Click)
            {
                XtraMessageBox.Show(this, "System error[btnSave_Click]: " + btnSave_Click.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            gridView1.AddNewRow();
            int i = gridView1.FocusedRowHandle;

            gridView1.SetRowCellValue(i,gridView1.Columns["Baud Rate"], "9600");
            gridView1.SetRowCellValue(i, gridView1.Columns["Suffix"], "CRLF");
         //   gridView1.SetRowCellValue(i, gridView1.Columns["ship_year"], dr["ship_year"]);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int i = gridView1.FocusedRowHandle;
            gridView1.DeleteRow(i);
        }
    }
}
