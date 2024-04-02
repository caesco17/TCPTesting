using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TCPTesting.Services
{
    public class TCPService : ITCPService
    {
        private IPEndPoint? endPoint { get; set; }
        public Socket GetSocket(string ipAdress, int port)
        {

            IPAddress ipAddress = System.Net.IPAddress.Parse(ipAdress);
            endPoint = new(ipAddress, port);

            Socket _socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            return _socket;
        }

        public IPEndPoint? getEndPoint() { return endPoint; }
    }
}
