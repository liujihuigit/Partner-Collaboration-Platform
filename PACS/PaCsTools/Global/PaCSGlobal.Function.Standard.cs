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

        /// <summary>
        /// 标准化GridView
        /// </summary>
        /// <param name="view">gridView控件</param>
        public static void InitGridViewInPaCS(GridView view)
        {
            view.BestFitColumns();
            view.IndicatorWidth = 45;
            view.Appearance.SelectedRow.BackColor = Color.PeachPuff;

            view.OptionsCustomization.AllowColumnMoving = false;
            view.OptionsPrint.AutoWidth = false;
            view.OptionsBehavior.Editable = false;
            //OptionsSelection
            view.OptionsSelection.MultiSelect = true;
            view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            //OptionsView
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.ColumnAutoWidth = false;
            view.OptionsView.ShowFooter = true;
            view.OptionsView.EnableAppearanceEvenRow = true;
            view.OptionsView.EnableAppearanceOddRow = true;

            //Event
            view.CustomDrawRowIndicator += new RowIndicatorCustomDrawEventHandler(view_CustomDrawRowIndicator);
        }

        private static void view_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

    }
}
