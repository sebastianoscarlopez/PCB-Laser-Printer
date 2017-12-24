using System;

namespace PCBLaserPrinterWindows
{
    interface IGerberViewer
    {
        void startParse();
        void parseProgress(StatusProcess status);
        void parseComplete();
        void parseError(Exception exception);
        void error(string errorDescription);
    }
}
