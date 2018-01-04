using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gerber;

namespace GerberMetaInfo
{
    /// <summary>
    /// GerberMetaInfo use this class to update metainfo with R aperture
    /// </summary>
    class HelperR : IHelperAperture
    {
        public void UpdateRows(GerberMetaInfoDTO MetaInfo, GerberDrawDTO draw, GerberApertureDTO aperture, int layerIndex, int rowFrom, int rowTill)
        {
            
        }
    }
}
