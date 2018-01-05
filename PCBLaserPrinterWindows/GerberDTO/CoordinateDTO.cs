using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerber
{
    public class CoordinateDTO
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public CoordinateDTO(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1}", X, Y);
        }
    }
}
