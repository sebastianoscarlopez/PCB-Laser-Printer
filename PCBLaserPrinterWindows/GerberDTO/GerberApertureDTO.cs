using System.Collections.Generic;

namespace Gerber
{
    public class GerberApertureDTO
    {
        public int Aperture { get; set; }
        public char Shape { get; set; }
        public List<int> Modifiers { get; set; } = new List<int>();
    }
}
