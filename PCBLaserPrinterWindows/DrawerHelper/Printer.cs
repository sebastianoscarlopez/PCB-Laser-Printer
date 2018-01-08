using Gerber;
using System;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace PCBLaserPrinterCommunication
{
    public class Printer
    {
        IMessenger messenger;
        int unit;

        public Printer()
        {
            messenger = new MessengerUART();
        }

        public bool Test()
        {
            return messenger.Connect();
        }

        public IObservable<int> Print(GerberMetaDataDTO metaData)
        {
            return Observable.Create((IObserver<int> observer) =>
            {
                try
                {
                    messenger.Send("startPrinter");
                    var response = messenger.Receive();
                    var match = new Regex(@"Unit:(\d+)").Match(response);
                    unit = int.Parse(match.Groups[0].Value); // Controller unit in nanometers
                    var unitRelation = metaData.UnitInMicroMeters * unit / Math.Pow(10, metaData.TrailingDigits + 3);

                    // Width in controller unit
                    var width = metaData.Bounds.Width * unitRelation;
                    var height = metaData.Bounds.Height * unitRelation;
                    messenger.Send(string.Format("size:{0}x{1}", width, height));

                    response = messenger.Receive();
                }
                catch
                {
                    observer.OnError(new Exception("Print unexpected error"));
                }
                return () => { };
            });
        }
    }
}
