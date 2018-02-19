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
        public override void Create(GerberMetaDataDTO metaDataBase, GerberMetaDataDTO metaData, GerberTraceDTO trace, GerberApertureDTO aperture, int layerIndex, int rowFrom, int rowTo)
        {
            base.Create(metaDataBase, metaData, trace, aperture, layerIndex, rowFrom, rowTo);
            var leftColumn = trace.AbsolutePointEnd.X - aperture.Modifiers[0] / 2;
            var rightColumn = leftColumn + aperture.Modifiers[0];
            leftColumn /= metaData.Scale;
            rightColumn /= metaData.Scale;

            var layer = new PlarityLayerDTO()
            {
                IsDarkPolarity = metaData.PolarityLayers[layerIndex].IsDarkPolarity
            };
            for (var rowIndex = topRow; rowIndex >= bottomRow; rowIndex--)
            {
                var row = GetRow(layer, rowIndex);
                var columns = new List<ColumnDataDTO>();
                if (rowIndex == topRow || rowIndex == bottomRow)
                {
                    columns.Add(
                        CreateColumn(leftColumn, rightColumn, new List<GerberTraceDTO> { trace }, TypeColumn.partial)
                        );
                }
                else
                {
                    columns.Add(
                        CreateColumn(leftColumn, leftColumn, new List<GerberTraceDTO> { trace }, TypeColumn.partial)
                        );
                    columns.Add(
                        CreateColumn(leftColumn + 1, rightColumn - 1, null, TypeColumn.fill)
                        );
                    columns.Add(
                        CreateColumn(rightColumn, rightColumn, new List<GerberTraceDTO> { trace }, TypeColumn.partial)
                        );
                }
                AddColumns(layer, rowIndex, columns);
            }

            if (aperture.Modifiers.Count == 3)
            {
                MakeHole(metaData, trace, aperture, layer);
            }
            MergeLayers(metaData.PolarityLayers[layerIndex], layer);
        }
    }
}
