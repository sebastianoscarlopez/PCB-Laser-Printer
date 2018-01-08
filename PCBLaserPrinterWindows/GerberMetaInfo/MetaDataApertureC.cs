using System.Collections.Generic;
using System.Linq;
using Gerber;

namespace GerberMetaData
{
    class MetaDataApertureC : MetaData
    {
        public override void Create(GerberMetaDataDTO metaData, GerberTraceDTO trace, GerberApertureDTO aperture, int layerIndex, int rowFrom, int rowTo)
        {
            base.Create(metaData, trace, aperture, layerIndex, rowFrom, rowTo);

            var points = MidpointCircle(
                trace.AbsolutePointEnd.X / metaData.Scale
                , trace.AbsolutePointEnd.Y / metaData.Scale
                , aperture.Modifiers[0] / metaData.Scale / 2,
                topRow,
                bottomRow);

            var layer = new PlarityLayerDTO()
            {
                IsDarkPolarity = metaData.PolarityLayers[layerIndex].IsDarkPolarity
            };

            ResumePoints(points, layer, trace);

            if(aperture.Modifiers.Count() == 2)
            {
                MakeHole(metaData, trace, aperture, layer);
            }
            MergeLayers(metaData.PolarityLayers[layerIndex], layer);
        }
    }
}
