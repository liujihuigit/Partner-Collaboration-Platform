using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PaCSTools
{
    public class PrintScreen
    {
        //bVk,虚拟键盘码
        //bScan ,该键的硬件扫描码
        //dwFlags,定义函数操作的各个方面的一个标志位集
        //dwExtraInfo,定义与击键相关的附加的32位值
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,
           UIntPtr dwExtraInfo);//该函数合成一次击键事件


        const int KEYEVENTF_KEYUP = 0x2;//若指定该值，该键将被释放；若未指定该值，该键将被按下

        public static void keydown(Keys k)
        {//按下
            keybd_event((byte)k, 0, 0, UIntPtr.Zero);
        }

        public static void keyup(Keys k)
        {//释放
            keybd_event((byte)k, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public static void printScreen()
        {//模拟PrintScreen
            keydown(Keys.PrintScreen);
            Application.DoEvents();
            keyup(Keys.PrintScreen);
            Application.DoEvents();
        }

        public static void altPrintScreen()
        {//模拟Alt+PrintScreen
            keydown(Keys.Menu);
            keydown(Keys.PrintScreen);
            Application.DoEvents();
            keyup(Keys.PrintScreen);
            keyup(Keys.Menu);
            Application.DoEvents();

        }
    }
}
