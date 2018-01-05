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
                var columns = new List<ColumnDataDTO>();
                if (rowIndex == topRow || rowIndex == bottomRow)
                {
                    columns.Add(new ColumnDataDTO()
                    {
                        Left = leftColumn,
                        Right = rightColumn,
                        TypeColumn = TypeColumn.partial,
                        Draws = new List<GerberDrawDTO> { draw }
                    });                    
                }
                else
                {
                    columns.Add(new ColumnDataDTO()
                    {
                        Left = leftColumn,
                        Right = leftColumn,
                        TypeColumn = TypeColumn.partial,
                        Draws = new List<GerberDrawDTO> { draw }
                    });
                    columns.Add(new ColumnDataDTO()
                    {
                        Left = leftColumn + 1,
                        Right = rightColumn - 1,
                        TypeColumn = TypeColumn.fill,
                        Draws = null
                    });
                    columns.Add(new ColumnDataDTO()
                    {
                        Left = rightColumn,
                        Right = rightColumn,
                        TypeColumn = TypeColumn.partial,
                        Draws = new List<GerberDrawDTO> { draw }
                    });
                }
                if (row == null)
                {
                    row = new RowDataDTO()
                    {
                        RowIndex = rowIndex
                    };
                    row.Columns.AddRange(columns);
                    layer.Rows.Add(row);
                }
                else
                {
                    columns.ForEach(column => OverlapColumn(column, row.Columns));
                }
            }
        }

        private void OverlapColumn(ColumnDataDTO column, List<ColumnDataDTO> columns)
        {
            var overlappedColumns = columns
                .Where(c => c.Left <= column.Right && c.Right >= column.Left);
            var deleted = new List<ColumnDataDTO>();
            var added = new List<ColumnDataDTO>();
            var idxOC = 0;
            bool isOmmited = false;
            while (!isOmmited && idxOC < overlappedColumns.Count())
            {
                var overlappedColumn = overlappedColumns.ElementAt(idxOC++);
                switch (overlappedColumn.TypeColumn)
                {
                    case TypeColumn.fill:
                        switch (column.TypeColumn)
                        {
                            case TypeColumn.fill:
                                if (column.Left != overlappedColumn.Left || column.Right != overlappedColumn.Right)
                                {
                                    added.Add(new ColumnDataDTO()
                                    {
                                        Draws = null,
                                        TypeColumn = TypeColumn.fill,
                                        Left = overlappedColumn.Left < column.Left
                                        ? overlappedColumn.Left
                                        : column.Left,
                                        Right = overlappedColumn.Right > column.Right
                                        ? overlappedColumn.Right
                                        : column.Right
                                    });
                                    deleted.Add(overlappedColumn);
                                }
                                isOmmited = true;
                                break;
                            case TypeColumn.partial:
                                if (overlappedColumn.Left <= column.Left)
                                {
                                    column.Left = overlappedColumn.Right + 1;
                                    if (column.Left <= column.Right)
                                    {
                                        added.Add(column);
                                    }
                                    isOmmited = true;
                                }
                                else if (overlappedColumn.Right >= column.Right)
                                {
                                    column.Right = overlappedColumn.Left - 1;
                                    if (column.Left <= column.Right)
                                    {
                                        added.Add(column);
                                    }
                                    isOmmited = true;
                                }
                                else
                                {
                                    added.Add(new ColumnDataDTO()
                                    {
                                        Draws = column.Draws.Select(d => d).ToList(),
                                        TypeColumn = TypeColumn.partial,
                                        Left = column.Left,
                                        Right = overlappedColumn.Left - 1
                                    });
                                    added.Add(new ColumnDataDTO()
                                    {
                                        Draws = column.Draws.Select(d => d).ToList(),
                                        TypeColumn = TypeColumn.partial,
                                        Left = overlappedColumn.Right + 1,
                                        Right = column.Right
                                    });
                                    isOmmited = true;
                                }
                                break;
                        }
                        break;
                    case TypeColumn.partial:
                        switch (column.TypeColumn)
                        {
                            case TypeColumn.fill:
                                if (column.Left <= overlappedColumn.Left)
                                {
                                    added.Add(column);
                                    overlappedColumn.Left = column.Right + 1;
                                    if(overlappedColumn.Left > overlappedColumn.Right)
                                    {
                                        deleted.Add(overlappedColumn);
                                    }
                                    isOmmited = true;
                                }
                                else if(column.Right >= overlappedColumn.Right)
                                {
                                    added.Add(column);
                                    overlappedColumn.Right = column.Left - 1;
                                    if (overlappedColumn.Left > overlappedColumn.Right)
                                    {
                                        deleted.Add(overlappedColumn);
                                    }
                                    isOmmited = true;
                                }
                                else
                                {
                                    added.Add(new ColumnDataDTO()
                                    {
                                        Draws = overlappedColumn.Draws.Select(d => d).ToList(),
                                        TypeColumn = TypeColumn.partial,
                                        Left = column.Right + 1,
                                        Right = overlappedColumn.Right
                                    });
                                    overlappedColumn.Right = column.Left - 1;
                                }
                                break;
                            case TypeColumn.partial:
                                column.Draws.ForEach(d =>
                                {
                                    if (!overlappedColumn.Draws.Contains(d))
                                    {
                                        overlappedColumn.Draws.Add(d);
                                        isOmmited = true;
                                    }
                                });
                                if (isOmmited || column.Left != overlappedColumn.Left || column.Right != overlappedColumn.Right)
                                {
                                    added.Add(new ColumnDataDTO()
                                    {
                                        Draws = overlappedColumn.Draws,
                                        TypeColumn = TypeColumn.partial,
                                        Left = overlappedColumn.Left < column.Left
                                                                            ? overlappedColumn.Left
                                                                            : column.Left,
                                        Right = overlappedColumn.Right > column.Right
                                                                            ? overlappedColumn.Right
                                                                            : column.Right
                                    });
                                    deleted.Add(overlappedColumn);
                                }
                                isOmmited = true;
                                break;
                        }
                        break;
                }
            }
            if (!isOmmited)
            {
                columns.Add(column);
            }
            columns.RemoveAll(c => deleted.Contains(c));
            added.ForEach(c => OverlapColumn(c, columns));
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
