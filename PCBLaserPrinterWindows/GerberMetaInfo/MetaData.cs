using Gerber;
using System.Collections.Generic;
using System.Linq;

namespace GerberMetaData
{
    internal abstract class MetaData
    {
        protected int topRow, bottomRow;

        /// <summary>
        /// It do metadata processing the draw
        /// </summary>
        /// <param name="MetaData">Meta data to update</param>
        /// <param name="trace">Trace data</param>
        /// <param name="aperture">Aperture data to draw</param>
        /// <param name="layerIndex">Layer Index</param>
        /// <param name="rowFrom">Top row index </param>
        /// <param name="rowTill">Bottom row index</param>
        virtual public void Create(GerberMetaDataDTO MetaData, GerberTraceDTO trace, GerberApertureDTO aperture, int layerIndex, int rowFrom, int rowTill)
        {
            topRow = trace.AbsolutePointEnd.Y + aperture.Modifiers[aperture.Shape == 'R' && aperture.Modifiers.Count > 1 ? 1 : 0] / 2;
            bottomRow = topRow - aperture.Modifiers[aperture.Shape == 'R' && aperture.Modifiers.Count > 1 ? 1 : 0];
            topRow /= MetaData.Scale;
            bottomRow /= MetaData.Scale;
            if (rowFrom < topRow)
            {
                topRow = rowFrom;
            }
            if (rowTill > bottomRow)
            {
                bottomRow = rowTill;
            }
        }

        /// <summary>
        /// A simple column
        /// </summary>
        /// <param name="left">X left coordinate</param>
        /// <param name="right">X right coordinate</param>
        /// <param name="trace">Trace, only when is partial</param>
        /// <param name="typeColumn">Type of column</param>
        /// <returns></returns>
        protected ColumnDataDTO CreateColumn(int left, int right, GerberTraceDTO trace, TypeColumn typeColumn)
        {
            return new ColumnDataDTO()
            {
                Left = left,
                Right = right,
                TypeColumn = typeColumn,
                Traces = new List<GerberTraceDTO> { trace }
            };
        }

        /// <summary>
        /// Get row of layer
        /// </summary>
        /// <param name="layer">layer</param>
        /// <param name="rowIndex">Layer index</param>
        /// <returns></returns>
        protected RowDataDTO GetRow(PlarityLayerDTO layer, int rowIndex)
        {
            return layer.Rows.Where(r => r.RowIndex == rowIndex).FirstOrDefault();
        }

        /// <summary>
        /// Add columns to row, row is created whether isn't exist
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columns"></param>
        protected void AddColumns(PlarityLayerDTO layer, int rowIndex, List<ColumnDataDTO> columns)
        {
            var row = layer.Rows.Where(r => r.RowIndex == rowIndex).FirstOrDefault();
            if (row == null)
            {
                row = new RowDataDTO()
                {
                    RowIndex = rowIndex
                };
                layer.Rows.Add(row);
            }
            columns.ForEach(column => OverlapColumn(column, row.Columns));
        }

        /// <summary>
        /// Overlap a column into the row
        /// </summary>
        /// <param name="column">New column to overlap</param>
        /// <param name="columns">Columns in row</param>
        protected void OverlapColumn(ColumnDataDTO column, List<ColumnDataDTO> columns)
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
                                        Traces = null,
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
                                        Traces = column.Traces.Select(d => d).ToList(),
                                        TypeColumn = TypeColumn.partial,
                                        Left = column.Left,
                                        Right = overlappedColumn.Left - 1
                                    });
                                    added.Add(new ColumnDataDTO()
                                    {
                                        Traces = column.Traces.Select(d => d).ToList(),
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
                                    if (overlappedColumn.Left > overlappedColumn.Right)
                                    {
                                        deleted.Add(overlappedColumn);
                                    }
                                    isOmmited = true;
                                }
                                else if (column.Right >= overlappedColumn.Right)
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
                                        Traces = overlappedColumn.Traces.Select(d => d).ToList(),
                                        TypeColumn = TypeColumn.partial,
                                        Left = column.Right + 1,
                                        Right = overlappedColumn.Right
                                    });
                                    overlappedColumn.Right = column.Left - 1;
                                }
                                break;
                            case TypeColumn.partial:
                                column.Traces.ForEach(d =>
                                {
                                    if (!overlappedColumn.Traces.Contains(d))
                                    {
                                        overlappedColumn.Traces.Add(d);
                                        isOmmited = true;
                                    }
                                });
                                if (isOmmited || column.Left != overlappedColumn.Left || column.Right != overlappedColumn.Right)
                                {
                                    added.Add(new ColumnDataDTO()
                                    {
                                        Traces = overlappedColumn.Traces,
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

        /// <summary>
        /// Midpoint circle algorithm adapted
        /// </summary>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="radius">Circle radius</param>
        protected List<CoordinateDTO> MidpointCircle(int x0, int y0, int radius, int topRow, int bottomRow)
        {
            var points = new List<CoordinateDTO>();
            int x = radius - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (radius << 1);

            while (x >= y)
            {
                var auxY = y0 + y;
                if (auxY <= topRow && auxY >= bottomRow)
                {
                    points.Add(new CoordinateDTO(x0 + x, auxY));
                    points.Add(new CoordinateDTO(x0 - x, auxY));
                }
                auxY = y0 + x;
                if (auxY <= topRow && auxY >= bottomRow)
                {
                    points.Add(new CoordinateDTO(x0 + y, auxY));
                    points.Add(new CoordinateDTO(x0 - y, auxY));
                }
                auxY = y0 - y;
                if (auxY <= topRow && auxY >= bottomRow)
                {
                    points.Add(new CoordinateDTO(x0 - x, auxY));
                    points.Add(new CoordinateDTO(x0 + x, auxY));
                }
                auxY = y0 - x;
                if (auxY <= topRow && auxY >= bottomRow)
                {
                    points.Add(new CoordinateDTO(x0 - y, auxY));
                    points.Add(new CoordinateDTO(x0 + y, auxY));
                }

                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }
                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += dx - (radius << 1);
                }
            }
            return points
                .OrderBy(p => p.X)
                .OrderByDescending(p => p.Y)
                .ToList();
        }

        /// <summary>
        /// Resume points into columns in layer
        /// </summary>
        /// <param name="points">Coordinate to resume</param>
        /// <param name="layer">Layer column would be added</param>
        /// <param name="trace">Trace in partial columns</param>
        protected void ResumePoints(List<CoordinateDTO> points, PlarityLayerDTO layer, GerberTraceDTO trace)
        {
            var total = points.Count();
            var previousPoint = points[0];
            var actualPoint = previousPoint;
            var row = GetRow(layer, actualPoint.Y);
            var columns = new List<ColumnDataDTO>();
            var column = CreateColumn(actualPoint.X, actualPoint.X, trace, TypeColumn.partial);
            for (var idx = 1; idx < total; idx++)
            {
                actualPoint = points[idx];
                if (previousPoint.Y == actualPoint.Y)
                {
                    if (previousPoint.X == actualPoint.X || previousPoint.X + 1 == actualPoint.X)
                    {
                        column.Right = actualPoint.X;
                    }
                    else
                    {
                        columns.Add(column);
                        if (previousPoint.X + 1 < actualPoint.X)
                        {
                            columns.Add(
                                CreateColumn(previousPoint.X + 1, actualPoint.X - 1, null, TypeColumn.fill)
                                );
                        }
                        column = CreateColumn(actualPoint.X, actualPoint.X, trace, TypeColumn.partial);
                    }
                }
                else
                {
                    columns.Add(column);
                    AddColumns(layer, previousPoint.Y, columns);
                    columns.Clear();
                    column = CreateColumn(actualPoint.X, actualPoint.X, trace, TypeColumn.partial);
                }
                previousPoint = actualPoint;
            }
            columns.Add(column);
            AddColumns(layer, actualPoint.Y, columns);
        }

        /// <summary>
        /// Merge two layers, result is with bottom polarity
        /// </summary>
        /// <param name="layerBottom">Layer bottom, it's affected with the result</param>
        /// <param name="layerTop">Layer top, it's unnafected</param>
        protected void MergeLayers(PlarityLayerDTO layerBottom, PlarityLayerDTO layerTop)
        {
            var rows = layerBottom.Rows.Union(layerTop.Rows).OrderBy(r => r.RowIndex);
            var layer = new PlarityLayerDTO()
            {
                Polarity = layerBottom.Polarity
            };
            rows.
        }
    }
}
