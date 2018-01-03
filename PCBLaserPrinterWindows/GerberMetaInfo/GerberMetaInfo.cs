using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerber
{
    /// <summary>
    /// Process Geber data provided by GerberParser or GerberMetaInfo 
    /// The result is a complex object but very usefull for generate a fast an high quality draw, it draw can be in screen or printer.
    /// </summary>
    public class GerberMetaInfo
    {
        private GerberHeaderDTO Header { get; set; }
        private List<GerberDrawDTO> Draws { get; set; }
        private GerberMetaInfoDTO MetaInfoBase = null;
        public GerberMetaInfoDTO MetaInfo;

        /// <summary>
        /// Constructor from GerberParser data
        /// </summary>
        /// <param name="header">Header gerated with GerberParser</param>
        /// <param name="draws">Draws gerated with GerberParser</param>
        public GerberMetaInfo(GerberHeaderDTO header, List<GerberDrawDTO> draws)
        {
            Header = header;
            Draws = draws;
            //aux.metaInfo.Bounds = CalculateBounds();
        }

        /// <summary>
        /// Process all gerber data obtaining his meta data
        /// For biggers resolutions use an small area by time
        /// </summary>
        /// <returns>Total draws processed</returns>
        /// <param name="dpi">Dots per inch. It's recommendable an initial value between 48 and 192, a higher value is a memory exponencial increment</param>
        public IObservable<int> GenerateMetaInfo(int dpi)
        {
            return Observable.Create((IObserver<int> observer) =>
            {
                MetaInfo = new GerberMetaInfoDTO();
                if (MetaInfoBase == null)
                {
                    MetaInfoBase = MetaInfo;
                    MetaInfo.DPI = dpi;
                    MetaInfo.Bounds
                }
                for (var i = 0; i < Draws.Count; i++)
                {
                    observer.OnNext(i);
                }
                observer.OnCompleted();
                return () => { };
            });
        }

        /// <summary>
        /// Helper to estimate limits of complete gerber
        /// </summary>
        /// <returns>Bound area of Gerber</returns>
        private Rectangle CalculateBounds()
        {
            var auxPoint = Draws[0].AbsolutePointStart;
            var bounds = new Rectangle(auxPoint.X, auxPoint.Y, 0, 0);
            Draws.ForEach(d =>
            {
                var modifiers = Header.Apertures.Where(a => a.Aperture == d.Aperture).Single().Modifiers;
                var apertureWidth = modifiers[0] / 2;
                var apertureHeight = (modifiers[modifiers.Count > 1 ? 1 : 0]) / 2;
                if (d.AbsolutePointStart.X - apertureWidth < bounds.Left)
                {
                    bounds.X = d.AbsolutePointStart.X - apertureWidth;
                }
                if (d.AbsolutePointStart.Y - apertureHeight < bounds.Y)
                {
                    bounds.Y = d.AbsolutePointStart.Y - apertureHeight;
                }
                if (d.AbsolutePointStart.X + apertureWidth > bounds.Right)
                {
                    bounds.Width = d.AbsolutePointStart.X + apertureWidth - bounds.X;
                }
                if (d.AbsolutePointStart.Y + apertureHeight > bounds.Top)
                {
                    bounds.Height = d.AbsolutePointStart.Y + apertureHeight - bounds.Y;
                }
            });

            return bounds;
        }
    }
}
