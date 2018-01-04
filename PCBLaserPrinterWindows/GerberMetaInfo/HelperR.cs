using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gerber;
using System.Drawing;

namespace GerberMetaInfo
{
    /// <summary>
    /// GerberMetaInfo use this class to update metainfo with R aperture
    /// </summary>
    class HelperR : IHelperAperture
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MetaInfo">Meta data to update</param>
        /// <param name="draw">Data of draw</param>
        /// <param name="aperture">Aperture data to draw</param>
        /// <param name="layerIndex">Layer Index</param>
        /// <param name="rowFrom">Top row index </param>
        /// <param name="rowTill">Bottom row index</param>
        public void UpdateRows(GerberMetaInfoDTO MetaInfo, GerberDrawDTO draw, GerberApertureDTO aperture, int layerIndex, int rowFrom, int rowTill)
        {
            // Flash with end coordinate
            var topRow = draw.AbsolutePointEnd.Y + aperture.Modifiers[0] / 2;
            var bottomRow = topRow - aperture.Modifiers[0];
            topRow /= MetaInfo.Scale;
            bottomRow /= MetaInfo.Scale;

            var leftColumn = draw.AbsolutePointEnd.X - aperture.Modifiers[0] / 2;
            var rightColumn = leftColumn + aperture.Modifiers[0];
            leftColumn /= MetaInfo.Scale;
            rightColumn /= MetaInfo.Scale;

            if (rowFrom < topRow)
            {
                topRow = rowFrom;
            }
            if (rowTill > bottomRow)
            {
                bottomRow = rowTill;
            }

            var layer = MetaInfo.PolarityLayers[layerIndex];
            for (var rowIndex = topRow; rowIndex >= bottomRow; rowIndex--)
            {
                var row = layer.Rows.Where(r => r.RowIndex == rowIndex).FirstOrDefault();
                var column = new ColumnDataDTO()
                {
                    Left = leftColumn,
                    Right = rightColumn,
                    TypeColumn = TypeColumn.fill,
                    Draws = null
                };
                if (row == null)
                {
                    row = new RowDataDTO()
                    {
                        RowIndex = rowIndex
                    };
                    row.Columns.Add(column);
                    layer.Rows.Add(row);
                }
                else
                {
                    OverlapColumn(column, row.Columns);
                }
            }
        }

        private void OverlapColumn(ColumnDataDTO column, List<ColumnDataDTO> columns)
        {
            var overlappedColumns = columns
                .Where(c => c.Left <= column.Right && c.Right >= column.Left)
                .OrderBy(c => c.Left).ToList();
            columns.Add(column);
            foreach(var overlappedColumn in overlappedColumns)
            {
                if (overlappedColumn.Left < column.Left)
                {
                    column.Left = overlappedColumn.Left;
                }
                if (overlappedColumn.Right > column.Right)
                {
                    column.Right = overlappedColumn.Right;
                }
            }
            columns.RemoveAll(c => overlappedColumns.Contains(c));
        }
        /*
        private Rectangle GetBoundsWithAperture(CoordinateDTO coordinateStart, CoordinateDTO coordinateEnd, GerberApertureDTO aperture)
        {
            var x1 = coordinateStart.X;
            var x2 = coordinateEnd.X;
            if (coordinateStart.X < coordinateEnd.X)
            {
                var aux = x1;
                x1 = x2;
                x2 = aux;
            }
            x1 -= aperture.Modifiers[0];
            x2 += aperture.Modifiers[0];
            return ;
        }

        private 
        */
    }
}
