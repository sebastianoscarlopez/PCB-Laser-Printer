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
            // Flash with end coordinate
            var topRow = trace.AbsolutePointEnd.Y + aperture.Modifiers[0] / 2;
            var bottomRow = topRow - aperture.Modifiers[0];
            topRow /= MetaData.Scale;
            bottomRow /= MetaData.Scale;

            var leftColumn = trace.AbsolutePointEnd.X - aperture.Modifiers[0] / 2;
            var rightColumn = leftColumn + aperture.Modifiers[0];
            leftColumn /= MetaData.Scale;
            rightColumn /= MetaData.Scale;

            if (rowFrom < topRow)
            {
                topRow = rowFrom;
            }
            if (rowTill > bottomRow)
            {
                bottomRow = rowTill;
            }

            var layer = MetaData.PolarityLayers[layerIndex];
            for (var rowIndex = topRow; rowIndex >= bottomRow; rowIndex--)
            {
                var row = layer.Rows.Where(r => r.RowIndex == rowIndex).FirstOrDefault();
                var columns = new List<ColumnDataDTO>();
                if (rowIndex == topRow || rowIndex == bottomRow)
                {
                    columns.Add(new ColumnDataDTO()
                    {
                        Left = leftColumn,
                        Right = rightColumn,
                        TypeColumn = TypeColumn.partial,
                        Traces = new List<GerberTraceDTO> { trace }
                    });                    
                }
                else
                {
                    columns.Add(new ColumnDataDTO()
                    {
                        Left = leftColumn,
                        Right = leftColumn,
                        TypeColumn = TypeColumn.partial,
                        Traces = new List<GerberTraceDTO> { trace }
                    });
                    columns.Add(new ColumnDataDTO()
                    {
                        Left = leftColumn + 1,
                        Right = rightColumn - 1,
                        TypeColumn = TypeColumn.fill,
                        Traces = null
                    });
                    columns.Add(new ColumnDataDTO()
                    {
                        Left = rightColumn,
                        Right = rightColumn,
                        TypeColumn = TypeColumn.partial,
                        Traces = new List<GerberTraceDTO> { trace }
                    });
                }
                AddColumns(layer, rowIndex, columns);
            }
        }
    }
}
