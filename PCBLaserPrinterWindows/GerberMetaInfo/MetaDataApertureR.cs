using System;
using System.Collections.Generic;
using System.Linq;
using Gerber;

namespace GerberMetaData
{
    /// <summary>
    /// GerberMetaData use this class to update metadata with R aperture
    /// </summary>
    class MetaDataApertureR : MetaData
    {
        public override void Create(GerberMetaDataDTO MetaData, GerberTraceDTO trace, GerberApertureDTO aperture, int layerIndex, int rowFrom, int rowTill)
        {
            base.Create(MetaData, trace, aperture, layerIndex, rowFrom, rowTill);

            var leftColumn = trace.AbsolutePointEnd.X - aperture.Modifiers[0] / 2;
            var rightColumn = leftColumn + aperture.Modifiers[0];
            leftColumn /= MetaData.Scale;
            rightColumn /= MetaData.Scale;

            var layer = new PlarityLayerDTO();
            for (var rowIndex = topRow; rowIndex >= bottomRow; rowIndex--)
            {
                var row = GetRow(layer, rowIndex);
                var columns = new List<ColumnDataDTO>();
                if (rowIndex == topRow || rowIndex == bottomRow)
                {
                    columns.Add(
                        CreateColumn(leftColumn, rightColumn, trace, TypeColumn.partial)
                        );
                }
                else
                {
                    columns.Add(
                        CreateColumn(leftColumn, leftColumn, trace, TypeColumn.partial)
                        );
                    columns.Add(
                        CreateColumn(leftColumn + 1, rightColumn - 1, null, TypeColumn.fill)
                        );
                    columns.Add(
                        CreateColumn(rightColumn, rightColumn, trace, TypeColumn.partial)
                        );
                }
                AddColumns(layer, rowIndex, columns);
            }

            if (aperture.Modifiers.Count == 3)
            {
                List<CoordinateDTO> hole = MidpointCircle(trace.AbsolutePointEnd.X, trace.AbsolutePointEnd.Y, aperture.Modifiers[2] / 2, topRow, bottomRow);
                var layerHole = new PlarityLayerDTO();
                layerHole.Polarity = 'C';
                ResumePoints(hole, layerHole, trace);
                MergeLayers(layer, layerHole);
            }
            MergeLayers(MetaData.PolarityLayers[layerIndex], layer);
        }
    }
}
