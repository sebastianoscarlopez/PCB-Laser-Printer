using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gerber;
using System.Drawing;

namespace GerberMetaData
{
    class MetaDataApertureC : MetaData
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
            var points = MidpointCircle(
                trace.AbsolutePointEnd.X / MetaData.Scale
                , trace.AbsolutePointEnd.Y / MetaData.Scale
                , aperture.Modifiers[0] / MetaData.Scale,
                topRow,
                bottomRow);
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
                    if(previousPoint.X + 1 == actualPoint.X)
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
                    column = CreateColumn(actualPoint.X, actualPoint.X, trace, TypeColumn.partial);
                }
                previousPoint = actualPoint;
            }
            columns.Add(column);
            AddColumns(layer, actualPoint.Y, columns);
        }

        /// <summary>
        /// Midpoint circle algorithm adapted
        /// </summary>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="radius">Circle radius</param>
        private List<CoordinateDTO> MidpointCircle(int x0, int y0, int radius, int topRow, int bottomRow)
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
//                if (auxY <= topRow && auxY >= bottomRow)
                {
                    points.Add(new CoordinateDTO(x0 + x, auxY));
                    points.Add(new CoordinateDTO(x0 - x, auxY));
                }
  //              auxY = y0 + x;
                if (auxY <= topRow && auxY >= bottomRow)
                {
                    points.Add(new CoordinateDTO(x0 + y, auxY));
                    points.Add(new CoordinateDTO(x0 - y, auxY));
                }
                auxY = y0 - y;
    //            if (auxY <= topRow && auxY >= bottomRow)
                {
                    points.Add(new CoordinateDTO(x0 - x, auxY));
                    points.Add(new CoordinateDTO(x0 + x, auxY));
                }
                auxY = y0 - x;
      //          if (auxY <= topRow && auxY >= bottomRow)
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
    }
}
