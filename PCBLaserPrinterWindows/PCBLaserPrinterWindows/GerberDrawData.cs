using System.Drawing;

namespace PCBLaserPrinterWindows
{
    public struct GerberDrawData
    {
        public string GCode { get; set; }
        public bool isLPDark { get; set; }
        public int Aperture { get; set; }
        public Point AbsolutePointStart { get; set; }
        public Point AbsolutePointEnd { get; set; }
    }
}
