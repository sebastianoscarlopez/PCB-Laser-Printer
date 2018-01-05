using System.Collections.Generic;
using System.Linq;
using Gerber;

namespace GerberMetaData
{
    class MetaDataApertureC : MetaData
    {
        public override void Create(GerberMetaDataDTO MetaData, GerberTraceDTO trace, GerberApertureDTO aperture, int layerIndex, int rowFrom, int rowTill)
        {
            base.Create(MetaData, trace, aperture, layerIndex, rowFrom, rowTill);

            var points = MidpointCircle(
                trace.AbsolutePointEnd.X / MetaData.Scale
                , trace.AbsolutePointEnd.Y / MetaData.Scale
                , aperture.Modifiers[0] / MetaData.Scale / 2,
                topRow,
                bottomRow);

            var layer = MetaData.PolarityLayers[layerIndex];
            ResumePoints(points, layer, trace);
        }
    }
}
