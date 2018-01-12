using Gerber;
using System;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace PCBLaserPrinterCommunication
{
    public class Printer
    {
        IMessenger messenger;

        public Printer()
        {
            messenger = new MessengerUART();
        }

        public bool Connect()
        {
            return messenger.Connect();
        }

        public IObservable<int> Print(GerberMetaDataDTO metaData)
        {
            return Observable.Create((IObserver<int> observer) =>
            {
                try
                {
                    var config = new Config();
                    messenger.Send("printStart");
                    var response = messenger.Receive();
                    var match = new Regex(@"Controller Unit:(\d+)").Match(response);
                    config.ControllerUnit = int.Parse(match.Groups[1].Value);

                    response = messenger.Receive();
                    match = new Regex(@"MotorRevolutionAverage:(\d+)").Match(response);
                    config.MotorRevolutionAverage = int.Parse(match.Groups[1].Value);

                    config.RatioConvert = (metaData.UnitInMicroMeters / Math.Pow(10, metaData.TrailingDigits)) *
                        (config.PrinterAngle / 360.0 * config.MotorRevolutionAverage / config.PrinterWidth) /
                        (config.ControllerUnit * Math.Pow(10, -6));
                    
                    config.Width = (int)(metaData.Bounds.Width * config.RatioConvert);
                    config.Height = (int)(metaData.Bounds.Height * config.RatioConvert) * -1;
                    messenger.Send(config.Width.ToString());
                    messenger.Send(config.Height.ToString());

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
