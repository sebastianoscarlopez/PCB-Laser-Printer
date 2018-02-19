using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using GerberMetaData;

namespace Gerber
{
    /// <summary>
    /// Process Geber data provided by GerberParser or GerberMetaData
    /// The result is a complex object but very usefull for generate a fast an high quality sketch, it can be drawed on the screen or printer.
    /// </summary>
    public class MetaDataAdmin
    {
        public GerberMetaDataDTO MetaDataDTO;
        private GerberMetaDataDTO MetaDataBaseDTO = null;
        private GerberHeaderDTO Header { get; set; }
        private List<GerberTraceDTO> Traces { get; set; }
        private List<KeyValuePair<char, MetaData>> metadataCreators = new List<KeyValuePair<char, GerberMetaData.MetaData>>()
            {
                new KeyValuePair<char, MetaData>('R', new MetaDataApertureR()),
                new KeyValuePair<char, MetaData>('C', new MetaDataApertureC())
            };

        /// <summary>
        /// Constructor from GerberParser data
        /// </summary>
        /// <param name="header">Header gerated with GerberParser</param>
        /// <param name="traces">Draws gerated with GerberParser</param>
        public MetaDataAdmin(GerberHeaderDTO header, List<GerberTraceDTO> traces)
        {
            Header = header;
            Traces = traces;
        }

        /// <summary>
        /// Process all gerber data obtaining his meta data
        /// For biggers resolutions use an small area by time
        /// </summary>
        /// <returns>Total traces processed</returns>
        /// <param name="dpi">Dots per inch. It's recommendable an initial value between 48 and 192, a higher value is a memory exponencial increment</param>
        public IObservable<int> GenerateMetaData(int dpi)
        {
            return GenerateMetaData(dpi, null);
        }
        public IObservable<int> GenerateMetaData(int dpi, Rectangle? area)
        {
            return Observable.Create((IObserver<int> observer) =>
            {
                MetaDataDTO = new GerberMetaDataDTO
                {
                    UnitInMicroMeters = Header.UnitInMicroMeters,
                    TrailingDigits = Header.TrailingDigits,
                    DPI = dpi,
                    Scale = (int)Math.Pow(10, Header.TrailingDigits) / dpi,
                    Bounds = area ?? CalculateBounds()
                };
                MetaDataDTO.PolarityLayers.Add(new PlarityLayerDTO()
                {
                    IsDarkPolarity = true
                });
                if (MetaDataBaseDTO == null)
                {
                    MetaDataBaseDTO = MetaDataDTO;
                }
                var counter = 0;
                Traces.ToObservable().Subscribe(
                    trace =>
                    {
                        updateMetaData(trace, 0);
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
        /// <param name="trace"></param>
        private void updateMetaData(GerberTraceDTO trace, int layerIndex)
        {
            var aperture = Header.Apertures.Where(a => a.Aperture == trace.Aperture).Single();
            MetaData metadata = metadataCreators.Single(m => m.Key == aperture.Shape).Value;
            metadata.Create(MetaDataBaseDTO, MetaDataDTO, trace, aperture,
                layerIndex,
                MetaDataDTO.Bounds.Top / MetaDataDTO.Scale + 1,
                MetaDataDTO.Bounds.Bottom / MetaDataDTO.Scale - 1);
        }

        /// <summary>
        /// Helper to estimate limits of complete gerber
        /// </summary>
        /// <returns>Bound area of Gerber</returns>
        private Rectangle CalculateBounds()
        {
            var auxPoint = Traces[0].AbsolutePointStart;
            var bounds = new Rectangle(auxPoint.X, auxPoint.Y, 0, 0);
            Traces.ForEach(d =>
            {
                var aperture = Header.Apertures.Single(a => a.Aperture == d.Aperture);
                var modifiers = aperture.Modifiers;
                var apertureWidth = modifiers[0] / 2;
                var apertureHeight = modifiers[aperture.Shape == 'R' && modifiers.Count > 1 ? 1 : 0] / 2;
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
