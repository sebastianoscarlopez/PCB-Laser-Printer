using System.Linq;
using System.IO.Ports;
using System.Globalization;
using System.Threading;

namespace PCBLaserPrinterCommunication
{
    public class MessengerUART : IMessenger
    {
        const int MAX_MESSAGE = 200;
        const char CHARACTER_END = '\n';
        readonly SerialPort serialPort;

        public MessengerUART()
        {
            serialPort = new SerialPort()
            {
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                ReadTimeout = 1000,
                StopBits = StopBits.One,
                WriteTimeout = 100
            };
        }

        public bool Send(string message)
        {
            Thread.Sleep(100);
            message += CHARACTER_END;
            var msg = string.Format("{0}{1}", getBCC(message).ToString(CultureInfo.InvariantCulture), message);

            bool receiveOk = false;
            int maxRetries = 5;
            do
            {
                try
                {
                    serialPort.Write(msg);
                    var data = serialPort.ReadLine();
                    receiveOk = data == "OK";
                }
                catch
                {
                    receiveOk = false;
                }
            } while (!receiveOk && --maxRetries > 0);
            return receiveOk;
        }

        public string Receive()
        {
            Thread.Sleep(10);
            string data = null;
            bool receiveOk = false;
            int maxRetries = 5;
            do
            {
                try
                {
                    data = serialPort.ReadLine();
                    var bcc = getBCC(data.Substring(1) + CHARACTER_END);
                    receiveOk = bcc == data[0];
                    Thread.Sleep(100);
                    serialPort.WriteLine(receiveOk ? "OK" : "FAILED");
                }
                catch
                {
                    receiveOk = false;
                }
            } while (!receiveOk && --maxRetries > 0);
            return data.Substring(1);
        }

        public bool Connect()
        {
            var ports = SerialPort.GetPortNames().Reverse().ToArray();
            var connected = false;
            for (var idx = 0; idx < ports.Length && !connected; idx++)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.PortName = ports[idx];
                try
                {
                    serialPort.Open();
                    if (serialPort.IsOpen)
                    {
                        Send("...");
                        connected = Send("Ready?");
                    }
                } catch
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }
                }
            }
            if(serialPort.IsOpen && !connected)
            {
                serialPort.Close();
            }
            return connected;
        }

        private char getBCC(string data)
        {
            char dv = '\0';
            int len = data.Length;
            if (len < 3 || len > MAX_MESSAGE)
            {
                return dv;
            }
            for (int i = 0; i < len; i++)
            {
                dv ^= data[i];
                
            }
            return dv;
        }
    }
}
