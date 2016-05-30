using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCS
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginForm frmlogin = new LoginForm();
            DialogResult dg = frmlogin.ShowDialog();
            if (dg == DialogResult.OK)
            {
                MainForm adminMain = new MainForm();
                Application.Run(adminMain);
            }
            else
                frmlogin.Close();
        }
    }
}
