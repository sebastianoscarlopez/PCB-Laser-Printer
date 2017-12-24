using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Threading;

namespace PCBLaserPrinterWindows
{
    class GerberPresenter
    {
        private readonly IGerberViewer viewer;

        public GerberPresenter(IGerberViewer viewer)
        {
            this.viewer = viewer;
        }

        public void processGerber(string file)
        {
            try
            {
                viewer.startParse();
                var gp = new GerberParser(file);
                gp.ClassifyRows()
                    .Concat(gp.GenerateDataDraw())
                    .ObserveOn(Dispatcher.CurrentDispatcher)
                    .SubscribeOn(ThreadPoolScheduler.Instance)
                    .Subscribe(
                        status =>
                        {
                            viewer.parseProgress(status);
                        },
                        ex => viewer.parseError(ex),
                        () => viewer.parseComplete()
                    );
            }
            catch (Exception ex)
            {
                viewer.parseError(new Exception(ConstantMessage.UnexpectedError));
            }
        }
    }
}
