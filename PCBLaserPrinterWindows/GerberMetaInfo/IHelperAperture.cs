using Gerber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerberMetaInfo
{
    interface IHelperAperture
    {
        void UpdateRows(GerberMetaInfoDTO MetaInfo, GerberDrawDTO draw, GerberApertureDTO aperture, int layerIndex, int rowFrom, int rowTill);
    }
}
