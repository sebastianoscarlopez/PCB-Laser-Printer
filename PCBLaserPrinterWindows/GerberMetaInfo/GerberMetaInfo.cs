using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using GerberMetaInfo;
using System.Threading.Tasks;

namespace Gerber
{
    /// <summary>
    /// Process Geber data provided by GerberParser or GerberMetaInfo 
    /// The result is a complex object but very usefull for generate a fast an high quality draw, it draw can be in screen or printer.
    /// </summary>
    public class GerberMetaInfo
    {
        public GerberMetaInfoDTO MetaInfo;
        private GerberMetaInfoDTO MetaInfoBase = null;
        private GerberHeaderDTO Header { get; set; }
        private List<GerberDrawDTO> Draws { get; set; }
        private HelperR helperR = new HelperR();

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
                MetaInfoBase = MetaInfo;
                MetaInfo.DPI = dpi;
                MetaInfo.Bounds = CalculateBounds();
                MetaInfo.PolarityLayers.Add(new PlarityLayerDTO()
                {
                    Polarity = 'D'
                });
                var counter = 0;
                Draws.ToObservable().Subscribe(
                    draw =>
                    {
                        updateMetaInfo(draw, 0);
                        observer.OnNext(++counter);
                    },
                    ex => observer.OnError(ex),
                    () => observer.OnCompleted()
                );
                return () => { };
            });
        }

        /// <summary>
        /// Update rows
        /// </summary>
        /// <param name="draw"></param>
        private void updateMetaInfo(GerberDrawDTO draw, int layerIndex)
        {
            IHelperAperture helper = helperR;
            helper.UpdateRows(MetaInfo, draw, Header.Apertures.Where(a => a.Aperture == draw.Aperture).Single(),
                layerIndex,
                MetaInfo.Bounds.Top,
                MetaInfo.Bounds.Bottom);
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
                    bounds.Width += bounds.X - d.AbsolutePointStart.X + apertureWidth;
                    bounds.X = d.AbsolutePointStart.X - apertureWidth;
                }
                if (d.AbsolutePointStart.Y + apertureHeight > bounds.Y)
                {
                    bounds.Height -= d.AbsolutePointStart.Y + apertureHeight - bounds.Y;
                    bounds.Y = d.AbsolutePointStart.Y + apertureHeight;
                }
                if (d.AbsolutePointStart.X + apertureWidth > bounds.Right)
                {
                    bounds.Width = d.AbsolutePointStart.X + apertureWidth - bounds.X;
                }
                if (d.AbsolutePointStart.Y - apertureHeight < bounds.Bottom)
                {
                    bounds.Height = d.AbsolutePointStart.Y - apertureHeight - bounds.Y;
                }

                if (d.AbsolutePointEnd.X - apertureWidth < bounds.Left)
                {
                    bounds.Width += bounds.X - d.AbsolutePointEnd.X + apertureWidth;
                    bounds.X = d.AbsolutePointEnd.X - apertureWidth;
                }
                if (d.AbsolutePointEnd.Y + apertureHeight > bounds.Y)
                {
                    bounds.Height -= d.AbsolutePointEnd.Y + apertureHeight - bounds.Y;
                    bounds.Y = d.AbsolutePointEnd.Y + apertureHeight;
                }
                if (d.AbsolutePointEnd.X + apertureWidth > bounds.Right)
                {
                    bounds.Width = d.AbsolutePointEnd.X + apertureWidth - bounds.X;
                }
                if (d.AbsolutePointEnd.Y - apertureHeight < bounds.Bottom)
                {
                    bounds.Height = d.AbsolutePointEnd.Y - apertureHeight - bounds.Y;
                }
            });

            return bounds;
        }
    }
}
