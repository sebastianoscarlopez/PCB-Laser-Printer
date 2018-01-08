using System.Linq;
using System.IO.Ports;

namespace PCBLaserPrinterCommunication
{
    public class MessengerUART : IMessenger
    {
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
            serialPort.WriteLine(message);
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
    }
}
