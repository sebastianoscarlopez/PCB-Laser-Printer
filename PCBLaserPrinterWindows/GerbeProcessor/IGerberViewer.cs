using Gerber;
using System;
using System.Drawing;

namespace Gerber
{
    public interface IGerberViewer
    {
        void startParse();
        void parseProgress(StatusProcessDTO status);
        void parseComplete();
        void parseError(Exception exception);
        void error(string errorDescription);
        void refreshCanvas(Bitmap bitmap);
    }
}
