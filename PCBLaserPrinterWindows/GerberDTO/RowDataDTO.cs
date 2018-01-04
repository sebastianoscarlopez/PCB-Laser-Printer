using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerber
{
    public class RowDataDTO
    {
        public int RowIndex { get; set; }
        public List<ColumnDataDTO> Columns { get; set; } = new List<ColumnDataDTO>();
    }
}
