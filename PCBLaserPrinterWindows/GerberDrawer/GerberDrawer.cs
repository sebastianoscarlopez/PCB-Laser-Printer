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

        public Bitmap Draw(GerberMetaDataDTO metaData)
        {
            var image = new Bitmap(metaData.Bounds.Width / metaData.Scale + 1, Math.Abs(metaData.Bounds.Height) / metaData.Scale + 1);
            image.SetResolution(metaData.DPI, metaData.DPI);
            var offsetX = metaData.Bounds.X / metaData.Scale * -1;
            var offsetY = (metaData.Bounds.Y + metaData.Bounds.Height) / metaData.Scale * -1;
            Graphics g = Graphics.FromImage(image);
            Pen penAux = new Pen(Color.LightGray, 1);
            for (var h = 0; h < image.Height; h++)
            {
                g.DrawLine(penAux, 0, h, image.Width, h);
            }
            var color = Color.Green;
            Pen pen = new Pen(color, 1);
            metaData.PolarityLayers.ForEach(
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
