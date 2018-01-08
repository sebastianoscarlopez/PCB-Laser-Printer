using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBLaserPrinterCommunication
{
    public interface IMessenger
    {
        void Send(string message);
        string Receive();
        bool Connect();
    }
}
