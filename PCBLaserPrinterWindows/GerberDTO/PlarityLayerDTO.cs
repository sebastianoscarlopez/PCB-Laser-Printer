using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerber
{
    public class PlarityLayerDTO
    {
        public bool IsDarkPolarity { get; set; }
        public List<RowDataDTO> Rows { get; set; } = new List<RowDataDTO>();
    }
}
