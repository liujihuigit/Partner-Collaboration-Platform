using DevExpress.XtraEditors;
using PaCSTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetalMask
{
    public partial class ImageUploadForm : XtraForm
    {
        public string ReturnValue ="";//用这个公开属性传值
        private string picpath = "";
        private string barcode = "";
        private string filenameExt = "";
        FTPClient ftpClient = null;
        public ImageUploadForm(string barcode)
        {
            InitializeComponent();
            ftpClient = new FTPClient(); //连接FTP服务器
            ftpClient.FTPClientPram("109.116.6.5", "", "pacsuser", "pacs2014", 522);
            ftpClient.SetTransferType(FTPClient.TransferType.Binary);
            this.barcode = barcode;
        }

        private void ImageUploadForm_Load(object sender, EventArgs e)
        {
            //每天在服务器创建一个文件夹
            string DirectoryName = PaCSGlobal.GetServerDateTime(2);
            string[] str = ftpClient.Dir("");
            bool exists = ((IList)str).Contains(DirectoryName + "\r");
            if (!exists)//如果ftp服务器上没有创建，则创建新文件夹
                ftpClient.MkDir(DirectoryName);
            ftpClient.ChDir(DirectoryName);
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "上传图片 (*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp";  
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                picpath = openFileDialog1.FileName;
                filenameExt = openFileDialog1.SafeFileName.Split('.')[1];
                pictureEdit1.Image = Image.FromFile(picpath);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(picpath))
                {
                    string destFilePath = barcode + "_" + PaCSGlobal.GetServerDateTime(1) + "."+filenameExt;
                    File.Copy(picpath, destFilePath);
                    ftpClient.Put(destFilePath);
                    XtraMessageBox.Show("保存成功", "提示");
                    ReturnValue = destFilePath;
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    XtraMessageBox.Show("请选择图片", "提示");
                    return;
                }
            }
            catch (Exception btnSave_Click)
            {
                DialogResult = DialogResult.No;
                XtraMessageBox.Show(this, "System error[btnSave_Click]: " + btnSave_Click.Message);
            }
        }

    }
}
