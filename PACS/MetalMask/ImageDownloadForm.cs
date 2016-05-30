using DevExpress.XtraEditors;
using PaCSTools;
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

namespace MetalMask
{
    public partial class ImageDownloadForm : XtraForm
    {
        private string creatDT = "";
        private string imageID = "";
        FTPClient ftpClient = null;
        public ImageDownloadForm(string creatDT,string imageID)
        {
            InitializeComponent();
            this.creatDT = creatDT;
            this.imageID = imageID;
            Init();
        }

        private void Init()
        {
            //创建下载文件夹
            if (!Directory.Exists("D:\\FtpDownload"))
            {
                Directory.CreateDirectory("D:\\FtpDownload");
            }
            string path = "D:\\FtpDownload\\" + imageID;
         
            if (!File.Exists(path))
            {
                ftpClient = new FTPClient(); //连接FTP服务器
                if (ftpClient.Connected)
                    ftpClient.DisConnect();
                ftpClient.FTPClientPram("109.116.6.5", creatDT, "pacsuser", "pacs2014", 522);
                ftpClient.SetTransferType(FTPClient.TransferType.Binary);

                ftpClient.Get(imageID, "D:\\FtpDownload", "");
            }
            pictureEdit1.Image = Image.FromFile(path, false);
        }

    }
}
