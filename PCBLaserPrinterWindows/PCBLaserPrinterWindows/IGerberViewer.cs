using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBLaserPrinterWindows
{
    interface IGerberViewer
    {
        void startParse();
        void parseProgress(int progress);
        void parseProgress(int progress, int total);
        void parseComplete();
        void parseError(Exception exception);
        void error(string errorDescription);
    }
}
