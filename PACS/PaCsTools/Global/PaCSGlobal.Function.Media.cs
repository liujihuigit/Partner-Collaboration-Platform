using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCSTools
{
    public partial class PaCSGlobal
    {
        public static SoundPlayer _msg = new SoundPlayer(Properties.Resources.msg);
        public static SoundPlayer _shake = new SoundPlayer(Properties.Resources.nudge);

        /// <summary>
        /// 朗读文本
        /// </summary>
        /// <param name="contents"></param>
        public static void Speak(string contents)
        {
            if (string.IsNullOrWhiteSpace(contents))
            {
                return;
            }
            // 一new一speech就搞定
            SpeechSynthesizer sp = new SpeechSynthesizer(); 
            sp.Volume = 100;
            sp.Rate = 0;

            // 开始读啦
            sp.SpeakAsync(contents);
        }

        /// <summary>
        /// 播放提示音
        /// </summary>
        public static void PlayWavOk()
        {
            SoundPlayer p = new SoundPlayer();
            //p.SoundLocation = Application.StartupPath + "//folder.wav";
            p.Stream = Properties.Resources.ok;//只能播放.wav文件.
            p.Load();
            p.Play();  
        }


        /// <summary>
        /// 产生闪屏振动效果
        /// </summary>
        private void Nudge(XtraForm form)
        {
            NotifyIcon notifyIcon1 = new NotifyIcon();
            if (notifyIcon1.Visible == true)
            {
                return;
            }
            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
            }
            int i = 0;
            Point _old = form.Location;
            Point _new1 = new Point(_old.X + 2, _old.Y + 2);
            Point _new2 = new Point(_old.X - 2, _old.Y - 2);
            _shake.Play();
            while (i < 4)
            {
                form.Location = _new1;
                Thread.Sleep(60);
                form.Location = _new2;
                Thread.Sleep(60);
                i++;
            }
            form.Location = _old;
        }
       


    }
}
