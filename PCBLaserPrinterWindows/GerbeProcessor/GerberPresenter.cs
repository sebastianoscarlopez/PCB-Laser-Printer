﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Threading;
using Gerber;

namespace Gerber
{
    public class GerberPresenter
    {
        private readonly IGerberViewer viewer;
        private MetaDataAdmin metaData;

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
                metaData = new MetaDataAdmin(parser.Header, parser.Draws);
                parser.ClassifyRows()
                    .Concat(parser.GenerateDataDraw())
                    .Concat(
                        metaData.GenerateMetaData(480)
                        .Select(i => new StatusProcessDTO()
                        {
                            ProcessName = "MetaData",
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
            var drawer = Observable.Start(() => new GerberDrawer().Draw(metaData.MetaData));
            drawer
                .ObserveOn(Dispatcher.CurrentDispatcher)
                .Subscribe((image) => viewer.refreshCanvas(image, metaData.MetaData.Bounds, metaData.MetaData.Scale));
        }
    }
}
