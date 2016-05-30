using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting.BarCode;
using PaCSTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toner
{
    public partial class Form2 : DevExpress.XtraEditors.XtraForm
    {
        public Form2()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {

        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            barCodeControl1.Parent = this;
            //barCodeControl1.AutoModule = true;
            barCodeControl1.Text = textEdit1.Text;

            Code128Generator symb = new Code128Generator();
            barCodeControl1.Symbology= symb;
            symb.CharacterSet = Code128Charset.CharsetAuto;
        }
    }
}
