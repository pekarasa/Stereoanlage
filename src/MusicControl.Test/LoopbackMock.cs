using System.Net;
using System.Net.Sockets;

namespace PeKaRaSa.MusicControl.Test
{
    internal class LoopbackMock
    {
        public LoopbackMock(int port)
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, port);

            // Start listening for client requests.
            server.Start();
        }
    }
}