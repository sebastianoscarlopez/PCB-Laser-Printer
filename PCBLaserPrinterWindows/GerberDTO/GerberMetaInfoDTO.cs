using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerber
{
    public class GerberMetaInfoDTO
    {
        public int DPI { get; set; }
        public Rectangle Bounds { get; set; }
        public List<PlarityLayerDTO> PolarityLayers { get; set; }
    }
}
