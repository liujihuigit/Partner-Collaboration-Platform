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
        private static GridView gridView = null;
        private PopupMenu popupMenu = new PopupMenu();
        private BarManager barManager1 = null;

        public void InitMenu()
        {
            barManager1 = new BarManager();
            popupMenu.Manager = barManager1;

            BarButtonItem itemCopyCell = new BarButtonItem();
            itemCopyCell.Caption = "复制单元格";
            itemCopyCell.Glyph = Properties.Resources.copy_16x16;
            itemCopyCell.ItemClick += new ItemClickEventHandler(itemCopyCell_ItemClick);

            BarButtonItem itemCopyRow = new BarButtonItem();
            itemCopyRow.Caption = "复制当前行";
            itemCopyRow.Glyph = Properties.Resources.checkbox_16x16;
            itemCopyRow.ItemClick += new ItemClickEventHandler(itemCopyRow_ItemClick);

            BarButtonItem itemExport = new BarButtonItem();
            itemExport.Caption = "导出到Excel";
            itemExport.Glyph = Properties.Resources.exporttoxlsx_16x16;
            itemExport.ItemClick += new ItemClickEventHandler(itemExport_ItemClick);

            barManager1.Items.Add(itemCopyCell);
            barManager1.Items.Add(itemCopyRow);
            barManager1.Items.Add(itemExport);

            popupMenu.AddItem(itemCopyCell);
            popupMenu.AddItem(itemCopyRow);
            popupMenu.AddItem(itemExport);
        }

        public PopupMenu CallMenu(GridView view)
        {
            gridView = view;
            barManager1.Form = gridView.GridControl;
            return popupMenu;
        }

        private static void itemCopyCell_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Clipboard.SetDataObject(gridView.GetFocusedDisplayText().ToString());
        }

        private static void itemCopyRow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView.SelectedRowsCount == 0) 
                return;

            const string CellDelimiter = "\t";
            const string LineDelimiter = "\r\n";
            string result = "";

            // iterate cells and compose a tab delimited string of cell values
            for (int i = gridView.SelectedRowsCount - 1; i >= 0; i--)
            {
                int row = gridView.GetSelectedRows()[i];
                for (int j = 0; j < gridView.VisibleColumns.Count; j++)
                {
                    result += gridView.GetRowCellDisplayText(row, gridView.VisibleColumns[j]);
                    if (j != gridView.VisibleColumns.Count - 1)
                        result += CellDelimiter;
                }
                if (i != 0)
                    result += LineDelimiter;
            }
            Clipboard.SetDataObject(result);
        }

        private static void itemExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportGridToFile(gridView, gridView.ViewCaption);
        }

    }
}
