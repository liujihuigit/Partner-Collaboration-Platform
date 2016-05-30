using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon;
using PaCSClientMain.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PaCSTools;
using DevExpress.XtraEditors;

namespace PaCSClientMain.View
{
    public partial class UserInfoForm : Form
    {
        public UserInfoForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if(!tbOldPwd.Text.Equals(PaCSTools.PaCSGlobal.LoginUserInfo.Password))
                {
                    XtraMessageBox.Show("原密码错误", "提示");
                    return;
                }

                if(!tbNewPwd.Text.Trim().Equals(tbNewPwd2.Text.Trim()))
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "两次密码输入不一致";
                    tbNewPwd2.Text = "";
                    tbNewPwd2.Focus();
                    return;
                }
                string sql = "update pacs_user set password = :password," +
                            " mail=:mail,phone=:phone,remark=:remark," +
                            " UPDATEDATE=to_char(sysdate,'yyyyMMdd'),UPDATETIME=to_char(sysdate,'hh24miss')," +
                            " UPDATEUSER=:UPDATEUSER  where id=:id";
                OracleParameter[] cmdParam = new OracleParameter[] {
                new OracleParameter(":password", OracleType.VarChar, 50),
                new OracleParameter(":mail", OracleType.VarChar,50),
                new OracleParameter(":phone", OracleType.VarChar,30),
                new OracleParameter(":remark", OracleType.VarChar,100),
                new OracleParameter(":UPDATEUSER", OracleType.VarChar,20),
                new OracleParameter(":id", OracleType.VarChar,20)
                };
                cmdParam[0].Value = tbNewPwd.Text.Trim();
                cmdParam[1].Value = tbMail.Text.Trim();
                cmdParam[2].Value = tbPhone.Text.Trim();
                cmdParam[3].Value = tbRemarks.Text.Trim();
                cmdParam[4].Value = PaCSTools.PaCSGlobal.LoginUserInfo.Id;
                cmdParam[5].Value = PaCSTools.PaCSGlobal.LoginUserInfo.Id;

                int i = OracleHelper.ExecuteNonQuery(sql, cmdParam);
                if (i > 0)
                {
                    lbStatus.ForeColor = Color.Green;
                    lbStatus.Text = "保存成功！";
                }
                else
                {
                    lbStatus.ForeColor = Color.Red;
                    lbStatus.Text = "保存失败！";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("system error[btnSave_Click]: "+ex.Message, "提示");
            }
        }

        private void tbOldPwd_Leave(object sender, EventArgs e)
        {
            if (!tbOldPwd.Text.Trim().Equals(PaCSTools.PaCSGlobal.LoginUserInfo.Password))
            {
                lbStatus.ForeColor = Color.Red;
                lbStatus.Text = "原密码错误";
                return;
            }
        }
    }
}
