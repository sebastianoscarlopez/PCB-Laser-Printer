using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerber
{
    public enum TypeColumn
    {
        fill,
        partial
    }

    public class ColumnDataDTO
    {
        public TypeColumn TypeColumn { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public List<GerberTraceDTO> Traces { get; set; } = new List<GerberTraceDTO>();
    }
}
