using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawerHelper
{
    public class Dot
    {
        static public void Draw(int x, int y, Graphics drawArea)
        {
            Pen mypen = new Pen(Color.Green);

            drawArea.DrawLine(mypen, 0, 0, 200, 150);
        }
    }
}
