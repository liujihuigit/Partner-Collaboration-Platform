using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaCSClientMain.Tools
{
    [DesignerCategory("")]
    [Designer("")]
    public class RibbonLogoHelper : Component
    {
        private Image _Image;
        private RibbonControl _RibbonControl;

        public RibbonControl RibbonControl
        {
            get { return _RibbonControl; }
            set
            {
                if (value == _RibbonControl)
                    return;
                RibbonControl prevValue = _RibbonControl;
                _RibbonControl = value;
                OnRibbonChanged(prevValue, _RibbonControl);
            }
        }

        private void OnRibbonChanged(RibbonControl prevValue, RibbonControl ribbonControl)
        {
            if (prevValue != null)
                prevValue.Paint -= ribbonControl_Paint;
            if (ribbonControl != null)
            {
                ribbonControl.Paint += ribbonControl_Paint;
                ribbonControl.Invalidate();
            }

        }

        void ribbonControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DevExpress.XtraBars.Ribbon.ViewInfo.RibbonViewInfo ribbonViewInfo = _RibbonControl.ViewInfo;
            if (ribbonViewInfo == null)
                return;
            DevExpress.XtraBars.Ribbon.ViewInfo.RibbonCaptionViewInfo captionViewInfo = ribbonViewInfo.Caption;
            if (captionViewInfo == null)
                return;

          //  Rectangle bounds = new Rectangle(captionViewInfo.ContentBounds.X + 120, captionViewInfo.ContentBounds.Y,
               // captionViewInfo.ContentBounds.Width - 22, captionViewInfo.ContentBounds.Height);

            Rectangle bounds = new Rectangle(captionViewInfo.ContentBounds.X+5, captionViewInfo.ContentBounds.Y+2,
                captionViewInfo.ContentBounds.Width - 22, captionViewInfo.ContentBounds.Height);

            e.Graphics.DrawImage(Image, bounds.Location);

           // DrawRibbonLogo(e.Graphics);
        }


        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == _Image)
                    return;
                _Image = value;
                OnImageChanged();
            }
        }



        private void OnImageChanged()
        {
            if (RibbonControl != null)
                RibbonControl.Invalidate();
        }

        private void DrawRibbonLogo(Graphics graphics)
        {
            if (Image == null)
                return;
            RibbonViewInfo ribbonViewInfo = RibbonControl.ViewInfo;
            if (ribbonViewInfo == null)
                return;
            RibbonPanelViewInfo panelViewInfo = ribbonViewInfo.Panel;
            if (panelViewInfo == null)
                return;
            Rectangle bounds = panelViewInfo.Bounds;
            int minX = bounds.X;
            RibbonPageGroupViewInfoCollection groups = panelViewInfo.Groups;
            if (groups == null)
                return;
            if (groups.Count > 0)
                minX = groups[groups.Count - 1].Bounds.Right;
            if (bounds.Height < Image.Height)
                return;
            int offset = (bounds.Height - Image.Height) / 2;
            int width = Image.Width + 15;
            bounds.X = bounds.Width - width;
            if (bounds.X < minX)
                return;
            bounds.Width = width;
            bounds.Y += offset;
            bounds.Height = Image.Height;
            graphics.DrawImage(Image, bounds.Location);
        }

    }
}
