using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace PaCS.Tools
{
    public partial class MyTreeView : TreeView
    {
        private Brush b = null;//节点字体颜色
        private Point p;//画CheckBox的位置
        public MyTreeView()
        {
            this.DrawMode = TreeViewDrawMode.OwnerDrawText;//自己画文本
        }
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            b = Brushes.Black;//默认字体为黑色
            if (!IsParent(e.Node))
            {
                p = e.Bounds.Location;//获取节点的位置
                p.X = p.X - 12;//覆盖到默认画CheckBox的位置
                CheckBoxRenderer.DrawCheckBox(e.Graphics, p, CheckBoxState.CheckedDisabled);//画一个禁用的选中的CheckBox
                b = Brushes.Gray;//当前节点字体为灰色
            }
            if ((e.State & TreeNodeStates.Focused) != 0)
                b = Brushes.White;//点击某节点时节点字体颜色为白色
            e.Graphics.DrawString(e.Node.Text, this.Font, b, e.Bounds.Location);//画文本
        }

        protected bool IsParent(TreeNode tn)
        {
            return tn.Nodes.Count > 0;
        }
    }
}
