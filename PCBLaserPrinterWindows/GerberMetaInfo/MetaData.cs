using Gerber;
using System.Collections.Generic;
using System.Linq;

namespace GerberMetaData
{
    internal abstract class MetaData
    {
        /// <summary>
        /// It do metadata processing the draw
        /// </summary>
        /// <param name="MetaData">Meta data to update</param>
        /// <param name="trace">Trace data</param>
        /// <param name="aperture">Aperture data to draw</param>
        /// <param name="layerIndex">Layer Index</param>
        /// <param name="rowFrom">Top row index </param>
        /// <param name="rowTill">Bottom row index</param>
        abstract public void Create(GerberMetaDataDTO MetaData, GerberTraceDTO trace, GerberApertureDTO aperture, int layerIndex, int rowFrom, int rowTill);

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
                row.Columns.AddRange(columns);
                layer.Rows.Add(row);
            }
            else
            {
                columns.ForEach(column => OverlapColumn(column, row.Columns));
            }
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
    }
}
