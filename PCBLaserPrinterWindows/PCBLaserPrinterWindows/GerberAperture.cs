using System.Collections.Generic;

namespace PCBLaserPrinterWindows
{
    public class GerberAperture
    {
        public int Aperture { get; set; }
        public char Shape { get; set; }
        public List<double> Modifiers { get; set; } = new List<double>();
    }
}
