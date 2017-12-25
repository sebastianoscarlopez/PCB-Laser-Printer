using System.Drawing;

namespace PCBLaserPrinterWindows
{
    public class GerberDrawData
    {
        public string GCode { get; set; }
        public bool IsLPDark { get; set; } = true;
        public int Aperture { get; set; }
        public Point AbsolutePointStart { get; set; }
        public Point AbsolutePointEnd { get; set; }
    }
}
