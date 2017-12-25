using System.Drawing;

namespace PCBLaserPrinterWindows
{
    public class GerberDrawInfo
    {
        public string GCode { get; set; }
        public bool IsLPDark { get; set; }
        public int ApertureMode { get; set; }
        public int Aperture { get; set; }
        public Point AbsolutePointStart { get; set; }
        public Point AbsolutePointEnd { get; set; }
    }
}
