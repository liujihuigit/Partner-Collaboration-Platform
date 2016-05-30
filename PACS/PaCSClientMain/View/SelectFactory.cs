using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using PaCSTools;

namespace PaCSClientMain.View
{
    public partial class SelectFactory : DevExpress.XtraEditors.XtraForm
    {
        public Dictionary<string, string> ReturnValue = new Dictionary<string, string>();//用这个公开属性传值
        private List<string> fctList = null;
        public SelectFactory(List<string> fctList)
        {
            InitializeComponent();
            this.fctList = fctList;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            switch(cmbFct.SelectedIndex)
            {
                case 0:
                    ReturnValue.Add("fct_code","C660A");
                    break;
                case 1:
                    ReturnValue.Add("fct_code", "C6H0A");
                    break;
                default:
                     ReturnValue.Add("fct_code","C660A");
                    break;
            }
            this.Close();
        }
    }
}