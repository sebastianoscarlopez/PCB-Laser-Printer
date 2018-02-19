using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Threading;
using System.Drawing;
using PCBLaserPrinterCommunication;
using GerberDTO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Gerber
{
    public class GerberPresenter
    {
        private readonly IGerberViewer viewer;
        private MetaDataAdmin metaDataAdmin;
        private readonly Printer printer = new Printer();

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
                metaDataAdmin = new MetaDataAdmin(parser.Header, parser.Draws);
                parser.ClassifyRows()
                    .Concat(parser.GenerateDataDraw())
                    .Concat(
                        metaDataAdmin.GenerateMetaData(1000)
                            .Select(i => new StatusProcessDTO()
                            {
                                ProcessName = "MetaData",
                                Percent = i
                            })
                    )
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
        object _lock = new object();
        public void Zoom(ZoomDTO z)
        {
            lock (_lock)
            {
                Debug.WriteLine(z.zoom);
                metaDataAdmin.GenerateMetaData(metaDataAdmin.MetaDataDTO.DPI + z.zoom)
                    .Select(i => new StatusProcessDTO()
                    {
                        ProcessName = "Zoom",
                        Percent = i
                    })
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
        }

        public void startDrawCanvas(Size size)
        {
            var drawerCanvas = Observable.Start(() => new GerberDrawer().Draw(metaDataAdmin.MetaDataDTO, size));
            drawerCanvas
                .ObserveOn(Dispatcher.CurrentDispatcher)
                .Subscribe((image) => viewer.refreshCanvas(image, metaDataAdmin.MetaDataDTO.Bounds, metaDataAdmin.MetaDataDTO.Scale));
        }
        
        public void print()
        {
            if (printer.Connect())
            {
                viewer.startPrinter();
                printer.Print(metaDataAdmin.MetaDataDTO)
                    .Subscribe(progress =>
                    {
                        Console.WriteLine(progress);
                    });
            }
            else
            {
                viewer.endPrinter();
            }
        }
    }
}
