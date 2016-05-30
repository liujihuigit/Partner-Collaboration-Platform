using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PaCSClientMain.View;
using DevExpress.XtraBars;
using System.Threading;
using System.Reflection;
using DevExpress.XtraEditors;
using PaCSTools;
using System.IO.Ports;
using System.Diagnostics;

namespace PACSClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            Application.EnableVisualStyles();
            DevExpress.Skins.SkinManager.EnableFormSkins();

            //=====创建互斥体法：=====
            bool blnIsRunning;
            Mutex mutexApp = new Mutex(false, Assembly.GetExecutingAssembly().FullName, out   blnIsRunning);
            if (!blnIsRunning)
            {
                XtraMessageBox.Show("程序已经运行！", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }   

            String[] CmdArgs = System.Environment.GetCommandLineArgs();
            if (CmdArgs.Length > 1)//正常启动，先更新程序，再主程序
            //if(2 > 1)//测试代码
            {
                //参数0是它本身的路径
                //String arg0 = CmdArgs[0].ToString();
                //String arg1 = CmdArgs[1].ToString();
                //String arg2 = CmdArgs[2].ToString();

                // Application.SetCompatibleTextRenderingDefault(false)的作用
                //如题：
                //1.作用:在应用程序范围内设置控件显示文本的默认方式(可以设为使用新的GDI+,还是旧的GDI)，true使用GDI+方式显示文本, false使用GDI方式显示文本。
                //2.只能在单独运行窗体的程序中调用该方法；不能在插件式的程序中调用该方法。
                //3.只能在程序创建任何窗体前调用该方法，否则会引发InvalidOperationException异常。
                ClientMainForm clientMain = new ClientMainForm();
                //sw.Stop();
                //XtraMessageBox.Show("TimeStap" + sw.ElapsedMilliseconds, "提示");
               

                LoginForm frmlogin = new LoginForm();
                DialogResult dg = frmlogin.ShowDialog();
                if (dg != DialogResult.OK)
                {
                    return;             
                }

                clientMain.Init();
                Application.Run(clientMain);
            }
            else
            {
                XtraMessageBox.Show("不能启动此程序，请启动PaCS程序(PaCSClient.exe)", "提示");
                return;
            }

        }
    }
}
