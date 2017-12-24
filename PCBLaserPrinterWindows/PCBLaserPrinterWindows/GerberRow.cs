using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBLaserPrinterWindows
{
    public class GerberRow
    {
        public int rowIndex { get; set; }
        public string rowText { get; set; }
        public EGerberRowType type { get; set; }
    }
}
