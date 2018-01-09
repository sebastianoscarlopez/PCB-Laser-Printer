using System.Linq;
using System.IO.Ports;
using System.Globalization;

namespace PCBLaserPrinterCommunication
{
    public class MessengerUART : IMessenger
    {
        const int MAX_MESSAGE = 200;
        readonly SerialPort serialPort;

        public MessengerUART()
        {
            serialPort = new SerialPort()
            {
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                ReadTimeout = 100,
                StopBits = StopBits.One,
                WriteTimeout = 100
            };
        }

        public void Send(string message)
        {
            message += "\n";
            var msg = string.Format("{0}{1}", getBCC(message).ToString(CultureInfo.InvariantCulture), message);
            serialPort.Write(msg);
        }

        public string Receive()
        {
            return serialPort.ReadLine();
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
                        Send("printStart");
                        while (!connected)
                        {
                            var response = Receive();
                            connected = response.StartsWith("Yes");
                        }
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
