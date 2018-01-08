using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerber
{
    public class GerberMetaDataDTO
    {
        public int TrailingDigits { get; set; }
        public int UnitInMicroMeters { get; set; }
        public int DPI { get; set; }
        public int Scale { get; set; }
        public Rectangle Bounds { get; set; }
        public List<PlarityLayerDTO> PolarityLayers { get; set; } = new List<PlarityLayerDTO>();
    }
}
