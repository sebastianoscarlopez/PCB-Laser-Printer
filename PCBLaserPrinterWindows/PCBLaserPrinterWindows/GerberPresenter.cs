using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBLaserPrinterWindows
{
    class GerberPresenter
    {
        List<GerberRow> commands;
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
                viewer.parseProgress(gp.totalHeaders(), gp.totalCommands() + gp.totalHeaders());
                commands = new List<GerberRow>();
                var gc = gp.GetCommands();
                gc
                    .SubscribeOn(Scheduler.NewThread)
                    .ObserveOn(DispatcherScheduler.Instance)
                    .Subscribe(
                    c =>
                    {
                        commands.Add(c);
                        viewer.parseProgress(commands.Count + gp.totalHeaders());
                    },
                    ex => viewer.parseError(ex),
                    () => viewer.parseComplete()
                    );
            }catch(Exception ex)
            {
                viewer.parseError(new Exception(ConstantMessage.UnexpectedError));
            }
        }
    }
}
