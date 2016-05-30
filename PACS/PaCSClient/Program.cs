using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoUpdate;
using DevExpress.XtraBars;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using DevExpress.XtraEditors;

namespace PACSClient
{
    static class Program
    {
        private static System.Threading.Mutex mutex;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //=====创建互斥体法：=====
            bool blnIsRunning;
            Mutex mutexApp = new Mutex(false, Assembly.GetExecutingAssembly().FullName, out   blnIsRunning);
            if (!blnIsRunning)
            {
                XtraMessageBox.Show("程序已经运行！", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }   

            InitUpdate initUp = new InitUpdate();
            int status = initUp.CheckUpdate();

            switch (status)
            {
                case -1:
                    XtraMessageBox.Show("配置文件出错!");
                    break;
                case -2:
                    XtraMessageBox.Show("与服务器连接失败,操作超时!");
                    break;
                case 0:
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                          p.StartInfo.FileName = "PaCSClientMain.exe";//需要启动的程序名       
                          p.StartInfo.WorkingDirectory = Application.StartupPath;    
                          p.StartInfo.Arguments = "pacs"+" "+"client";//启动参数       
                          p.Start();//启动       
                    break;
                default:
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FrmUpdate());
                    break;
            }
        }
    }
}
