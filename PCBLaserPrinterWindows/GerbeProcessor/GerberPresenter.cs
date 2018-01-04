using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Threading;
using Gerber;

namespace Gerber
{
    public class GerberPresenter
    {
        private readonly IGerberViewer viewer;
        private GerberMetaInfo metaInfo;

        public GerberPresenter(IGerberViewer viewer)
        {
            this.viewer = viewer;
        }

        public void processGerber(string file)
        {
            try
            {
                viewer.startParse();
                var parser = new GerberFileParser(file);
                metaInfo = new GerberMetaInfo(parser.Header, parser.Draws);
                parser.ClassifyRows()
                    .Concat(parser.GenerateDataDraw())
                    .Concat(
                        metaInfo.GenerateMetaInfo(254)
                        .Select(i => new StatusProcessDTO()
                        {
                            ProcessName = "MetaInfo",
                            Percent = i
                        }))                        
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
            catch (Exception)
            {
                viewer.parseError(new Exception(ConstantMessage.UnexpectedError));
            }
        }

        public void startDrawCanvas()
        {
            var drawer = Observable.Start(() => new GerberDrawer().Draw(metaInfo.MetaInfo));
            drawer.Subscribe((image) => viewer.refreshCanvas(image));
        }
    }
}
