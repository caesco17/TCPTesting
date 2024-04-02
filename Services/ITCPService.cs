using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPTesting.Services
{
    public interface ITCPService
    {
        public Socket GetSocket(string ipAdress, int port);
        public IPEndPoint? getEndPoint();
    }
}
