using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using PaCSTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetalMask
{
    public partial class DetailForm : XtraForm
    {
        private string selectedRowId = "";
        private string selectedImageId = "";
        public DetailForm(string selectedRowId, string selectedImageId)
        {
            InitializeComponent();
            this.selectedRowId = selectedRowId;
            this.selectedImageId = selectedImageId;
        }

        private void BaseInfo_Load(object sender, EventArgs e)
        {
            DataTable dtData = GetData(selectedRowId);
            if (dtData.Rows.Count == 0)
            {
                XtraMessageBox.Show("barcode不存在", "提示");
                return;
            }

            tbBarcode.Text = dtData.Rows[0]["BarcodeNO"].ToString();
            tbToolType.Text = dtData.Rows[0]["ToolType"].ToString();
            tbMaskCode.Text = dtData.Rows[0]["MaskCode"].ToString();
            tbPCBCode.Text = dtData.Rows[0]["PCB Code"].ToString();

            tbModel.Text = dtData.Rows[0]["ProductModel"].ToString();
            tbVendor.Text = dtData.Rows[0]["所属厂家"].ToString();
            tbVendloc.Text = dtData.Rows[0]["使用厂家"].ToString();
            tbStatus.Text = dtData.Rows[0]["Status"].ToString();

            tbLine.Text = dtData.Rows[0]["Line"].ToString();
            tbBin.Text = dtData.Rows[0]["Bin"].ToString();
            tbsn.Text = dtData.Rows[0]["S/N"].ToString();
            tbMMVer.Text = dtData.Rows[0]["M/M Ver"].ToString();

            tbTens.Text = dtData.Rows[0]["Tension"].ToString();
            tbTimes.Text = dtData.Rows[0]["使用次数"].ToString();
            tbMaker.Text = dtData.Rows[0]["制造厂家"].ToString();
            tbMakRsn.Text = dtData.Rows[0]["制造原因"].ToString();

            tbRsnCont.Text = dtData.Rows[0]["制造原因内容"].ToString();
            tbInRsn.Text = dtData.Rows[0]["搬入原因"].ToString();
            tbOutRsn.Text = dtData.Rows[0]["搬出原因"].ToString();
            tbReceiver.Text = dtData.Rows[0]["接收人"].ToString();

            tbContact.Text = dtData.Rows[0]["联系方式"].ToString();
            tbDispRsn.Text = dtData.Rows[0]["废弃原因"].ToString();

            tbDelYN.Text = dtData.Rows[0]["DEL_YN"].ToString();
            tbUpdateDT.Text = dtData.Rows[0]["UpdatedOn"].ToString();
            tbUpdateUser.Text = dtData.Rows[0]["更新人"].ToString();
            tbCreateDate.Text = dtData.Rows[0]["RegisteredOn"].ToString();
            tbCreateUser.Text = dtData.Rows[0]["注册人"].ToString();

            lbImg.Text = dtData.Rows[0]["IMG_YN"].ToString().Equals("Y")?"已上传图片":"未上传图片";
        }

        private DataTable GetData(string barcode)
        {
            string sql = " select * from vwm_rm_eqp_metalmask " +
                            " where \"BarcodeNO\" = '" + barcode + "' " +
                            " and fct_code = '" + PaCSGlobal.LoginUserInfo.Fct_code + "' ";

            DataTable dtResult = OracleHelper.ExecuteDataTable(sql);
            return dtResult;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            try
            {
                if(selectedImageId.Length<10)
                {
                    XtraMessageBox.Show("没有图片附件", "提示");
                    return;
                }
                string datetimeDictory = DateTime.ParseExact(tbCreateDate.Text.Substring(0, 10), "yyyy-MM-dd", CultureInfo.CurrentCulture).ToString("yyyyMMdd");
                ImageDownloadForm frmNew = new ImageDownloadForm(datetimeDictory, selectedImageId);
                DialogResult dg = frmNew.ShowDialog();
            }
            catch (Exception btnImage_Click)
            {
                XtraMessageBox.Show(this, "System error[btnImage_Click]: " + btnImage_Click.Message);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbBarcode.Text.Equals(""))
                {
                    XtraMessageBox.Show("请输入 Barcode No", "提示");
                    tbBarcode.Focus();
                    return;
                }

                ImageUploadForm frmNew = new ImageUploadForm(tbBarcode.Text.Trim());
                DialogResult dg = frmNew.ShowDialog();
                if (dg == DialogResult.OK)
                {
                    lbImg.Text = "图片上传成功";
                    lbImg.ForeColor = Color.Green;
                }
            }
            catch (Exception btnImage_ButtonClick)
            {
                XtraMessageBox.Show(this, "System error[btnImage_ButtonClick]: " + btnImage_ButtonClick.Message);
            }
        }
    }
}
