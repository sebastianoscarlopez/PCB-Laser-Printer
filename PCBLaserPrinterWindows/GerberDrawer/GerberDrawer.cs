using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

        /// <summary>
        /// Draw metada
        /// </summary>
        /// <param name="metaData"></param>
        /// <returns>Return bitmap with image</returns>
        public Bitmap Draw(GerberMetaDataDTO metaData)
        {
            return Draw(metaData, new Size());
        }

        /// <summary>
        /// Draw metada in size passed
        /// </summary>
        /// <param name="metaData"></param>
        /// <param name="size">Maximun width and height to scale the image</param>
        /// <returns>Return bitmap with image</returns>
        public Bitmap Draw(GerberMetaDataDTO metaData, Size size)
        {
            var width = metaData.Bounds.Width / metaData.Scale;
            var height = Math.Abs(metaData.Bounds.Height) / metaData.Scale;
            var image = new Bitmap(width + 1, height + 1);
            image.SetResolution(metaData.DPI, metaData.DPI);
            var offsetX = metaData.Bounds.X / metaData.Scale * -1;
            var offsetY = (metaData.Bounds.Y + metaData.Bounds.Height) / metaData.Scale * -1;
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Pen penAux = new Pen(Color.Gray, 1);
                for (var h = 0; h < image.Height; h++)
                {
                    graphics.DrawLine(penAux, 0, h, image.Width, h);
                }
                var color = Color.Green;
                Pen pen = new Pen(color, 1);
                metaData.PolarityLayers.ForEach(
                    p => p.Rows.ForEach(r =>
                            r.Columns.ForEach(c =>
                            {
                                if (c.Left == c.Right)
                                {
                                    if (c.Left + offsetX < image.Width && r.RowIndex + offsetY < image.Height)
                                    {
                                        image.SetPixel(c.Left + offsetX, r.RowIndex + offsetY, color);
                                    }
                                }
                                else
                                {
                                    graphics.DrawLine(pen, c.Left + offsetX, r.RowIndex + offsetY, c.Right + offsetX, r.RowIndex + offsetY);
                                }
                            })
                        ));
            }

            if (!size.IsEmpty)
            {
                var scale = 1f;
                scale = (float)size.Width / width;
                var scaleSY = (float)size.Height / height;
                if (scaleSY < scale)
                {
                    scale = scaleSY;
                }
                width = (int)(width * scale);
                height = (int)(height * scale);
                var imageScale = new Bitmap(width + 1, height + 1);
                imageScale.SetResolution(metaData.DPI, metaData.DPI);

                var destRect = new Rectangle(0, 0, width, height);
                using (Graphics graphics = Graphics.FromImage(imageScale))
                {
                    graphics.DrawImage(image, destRect);
                }
                return imageScale;
            }
            else
            {
                return image;
            }
        }
    }
}
