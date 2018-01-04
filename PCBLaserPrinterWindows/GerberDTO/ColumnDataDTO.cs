using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerber
{
    public enum TypeColumn
    {
        empty,
        fill,
        partial
    }

    public class ColumnDataDTO
    {
        public TypeColumn TypeColumn { get; set; }
        public int Length { get; set; }
        public List<GerberDrawDTO> Draws { get; set; } = new List<GerberDrawDTO>();
    }
}
