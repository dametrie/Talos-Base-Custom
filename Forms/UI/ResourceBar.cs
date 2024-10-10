using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Talos.Forms.UI
{
    internal class ResourceBar : ProgressBar
    {

        private readonly Bitmap barImage;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x04;
                return cp;
            }
        }

        internal ResourceBar(string name)
        {
            Name = name;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            barImage = Name.Equals("manaBar") ? Properties.Resources.mpBar : Properties.Resources.hpBar;
        }


        protected virtual void OnPaintBackground(PaintEventArgs pevent)
        {
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int num = 1;
            using (Image image = new Bitmap(Width, Height))
            {
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    System.Drawing.Rectangle bounds = new System.Drawing.Rectangle(0, 0, Width, Height);
                    if (ProgressBarRenderer.IsSupported)
                    {
                        ProgressBarRenderer.DrawVerticalBar(graphics, bounds);
                    }
                    num = (100 - Value) * MaximumSize.Height / 100;
                    TextureBrush textureBrush = new TextureBrush(barImage);
                    graphics.FillRectangle(textureBrush, 0, num, bounds.Width, bounds.Height);
                    e.Graphics.DrawImage(image, 0, 0);
                    image.Dispose();
                    textureBrush.Dispose();
                }
            }
        }

        protected virtual void Finalize()
        {
            try
            {
                barImage?.Dispose();
            }
            finally
            {
                Finalize();
            }
        }

    }
}
