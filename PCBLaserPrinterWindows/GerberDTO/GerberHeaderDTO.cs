using System.Collections.Generic;

namespace Gerber
{
    public class GerberHeaderDTO
    {
        public bool isLeadingZeroOmission { get; set; }
        public bool isAbsolute { get; set; }
        public int LeadingDigits { get; set; }
        public int TrailingDigits { get; set; }
        public int Unit { get; set; }
        public List<GerberApertureDTO> Apertures { get; set; } = new List<GerberApertureDTO>();
    }
}
