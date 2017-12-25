using System.Collections.Generic;

namespace PCBLaserPrinterWindows
{
    public class GerberHeader
    {
        public bool isLeadingZeroOmission { get; set; }
        public bool isAbsolute { get; set; }
        public int LeadingDigits { get; set; }
        public int TrailingDigits { get; set; }
        public int Unit { get; set; }
        public List<GerberAperture> Apertures { get; set; } = new List<GerberAperture>();
    }
}
