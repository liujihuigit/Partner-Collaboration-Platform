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
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaCSTools
{
    public partial class PaCSGlobal
    {

        /// <summary>
        /// 获取注册表键值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetRegistryValue(string key, string path)
        {
            Microsoft.Win32.RegistryKey registryKey;
            string com = "";
            // HKCU\Software\RegeditStorage
            registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\PaCS\" + path);
            if (registryKey != null)
            {
                com = (string)registryKey.GetValue(key);
                registryKey.Close();
            }
            return com;
        }

        /// <summary>
        /// 设置注册表键值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="com">值</param>
        public static void SetRegistryValue(string key, string com, string path)
        {
            Microsoft.Win32.RegistryKey registryKey;

            // HKCU\Software\RegeditStorage
            registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\PaCS\" + path);
            registryKey.SetValue(key, com);
            registryKey.Close();
        }

        /// <summary>
        /// 判断注册表项是否有指定的注册表值
        /// </summary>
        /// <param name="key">注册表项</param>
        /// <param name="valueName">注册表值</param>
        /// <returns></returns>
        private static bool IsKeyHaveValue(RegistryKey key, string valueName)
        {
            string[] keyNames = key.GetValueNames();
            foreach (string keyName in keyNames)
            {
                if (keyName.Trim() == valueName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 删除注册表项值
        /// </summary>
        /// <param name="key">注册表项</param>
        /// <param name="keyValueName">值名称</param>
        /// <param name="keyValue">值</param>
        /// <returns></returns>
        public static void DeleteRegistryValue(string path)
        {
            RegistryKey key = Registry.CurrentUser;
            if (key.OpenSubKey(@"Software\PaCS\" + path) != null)
                key.DeleteSubKey(@"Software\PaCS\" + path, true); //该方法无返回值，直接调用即可
            key.Close();
        }
    }
}
