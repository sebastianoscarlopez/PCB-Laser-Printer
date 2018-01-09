using System.Linq;
using System.IO.Ports;

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
                ReadTimeout = 10000,
                StopBits = StopBits.One,
                WriteTimeout = 1000
            };
        }

        public void Send(string message)
        {
            serialPort.WriteLine(message.Insert(0, getBCC(message).ToString()));
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
                        Send("Ready?");
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
            int len = data.Length - 2; // BCC and CHARACTER_END
            if (len < 3 || len > MAX_MESSAGE)
            {
                return dv;
            }
            for (int i = 1; i < len; i++)
            {
                dv ^= data[i];
            }
            return dv;
        }
    }
}
