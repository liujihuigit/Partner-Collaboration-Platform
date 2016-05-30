using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCSClientMain.View
{
    public partial class AttachForm : Form
    {
        public DataTable dt = null;
        public AttachForm()
        {
            InitializeComponent();
            dt = new DataTable();
            dt.Columns.Add("fileName");
            dt.Columns.Add("fileSize");
            dt.PrimaryKey = new DataColumn[] { dt.Columns["fileName"] };
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string fileNm = "";
                OpenFileDialog op = new OpenFileDialog();
                //op.Filter = "word Files(*.doc)|*.doc|All Files(*.*)|*.*";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fi = new FileInfo(op.FileName);

                    long Size = fi.Length;
                    if (Size >= 10 * 1024 * 1024)
                    {
                        XtraMessageBox.Show("单个文件不允许超过 10M !", "提示");
                        return;
                    }
                    //fileNm = op.FileName;
                    fileNm = op.SafeFileName;
                    InsertSuccess(fileNm, (Size/(1024)).ToString());
                }
                //fileNm = fileNm.Substring(fileNm.LastIndexOf('\\') + 1);  //xx.txt 带扩展名
            }
            catch (Exception btnUpload_Click)
            {            
                 XtraMessageBox.Show("system error [btnUpload_Click]: "+btnUpload_Click.Message, "提示");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        public void InsertSuccess(string fileName,string fileSize)
        {
            try
            {
                object[] key = new object[]{fileName};

                if (dt.Rows.Count > 0 && dt.Rows.Find(key)!=null)
                {
                    XtraMessageBox.Show("文件重复上传 !", "提示");
                    return;
                }

                DataRow dr = dt.NewRow();
                dr["fileName"] = fileName;
                dr["fileSize"] = fileSize;
                dt.Rows.Add(dr);
                dataGridView1.DataSource = dt;
          
            }
            catch (Exception ee)
            {
                XtraMessageBox.Show(ee.Message.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string fname = this.dataGridView1["fileName", e.RowIndex].Value.ToString();
                //saveFileDialog1.Filter = "PDF文件(*.pdf)|*.pdf";
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    XtraMessageBox.Show(fname);
                }
            }
        }  
    }
}
