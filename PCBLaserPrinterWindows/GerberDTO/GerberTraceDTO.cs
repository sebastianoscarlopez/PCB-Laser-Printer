using Gerber;

namespace Gerber
{
    public class GerberTraceDTO
    {
        public string GCode { get; set; }
        public bool IsLPDark { get; set; }
        public int ApertureMode { get; set; }
        public int Aperture { get; set; }
        public CoordinateDTO AbsolutePointStart { get; set; }
        public CoordinateDTO AbsolutePointEnd { get; set; }
    }
}
