using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerber
{
    public class GerberDrawer
    {
        public GerberDrawer()
        {
        }

        public Bitmap Draw(GerberMetaInfoDTO metaInfo)
        {
            var image = new Bitmap(metaInfo.Bounds.Width / metaInfo.Scale + 1, Math.Abs(metaInfo.Bounds.Height) / metaInfo.Scale + 1);
            image.SetResolution(metaInfo.DPI, metaInfo.DPI);
            var offsetX = metaInfo.Bounds.X / metaInfo.Scale * -1;
            var offsetY = (metaInfo.Bounds.Y + metaInfo.Bounds.Height) / metaInfo.Scale * -1;
            Graphics g = Graphics.FromImage(image);
            Pen penAux = new Pen(Color.LightGray, 1);
            for (var h = 0; h < image.Height; h++)
            {
                g.DrawLine(penAux, 0, h, image.Width, h);
            }
            var color = Color.Green;
            Pen pen = new Pen(color, 1);
            metaInfo.PolarityLayers.ForEach(
                p => p.Rows.ForEach(r =>
                        r.Columns.ForEach(c =>
                        {
                            if (c.Left == c.Right)
                            {
                                image.SetPixel(c.Left + offsetX, r.RowIndex + offsetY, color);
                            }
                            else
                            {
                                g.DrawLine(pen, c.Left + offsetX, r.RowIndex + offsetY, c.Right + offsetX, r.RowIndex + offsetY);
                            }
                        })
                    ));
            g.ScaleTransform(1, -1);
            g.Dispose();
            return image;
        }
    }
}
