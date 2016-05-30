using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toner
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(SplashScreen1));
            //System.Threading.Thread.Sleep(2000);
            //DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GREJH());
        }
    }
}
