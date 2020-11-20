using System;
using System.Net;
using System.Net.Sockets;

namespace VlcDummy
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener server = new TcpListener(localAddr, 13001);
            Console.WriteLine($"vlc is listening on {localAddr} : 13001");

            // Start listening for client requests.
            server.Start();

            while (true)
            {
                Console.Write("-");
                using (TcpClient client = server.AcceptTcpClient())
                {
                    Console.Write(">");

                    byte[] dataBuffer = new byte[256];
                    Int32 byteCount;

                    // Get a stream object for reading and writing
                    using (NetworkStream stream = client.GetStream())
                    {
                        // Read the first batch of the TcpServer response bytes.
                        byteCount = stream.Read(dataBuffer, 0, dataBuffer.Length);
                        stream.Close();
                    }

                    // String to store the response ASCII representation.
                    string responseData = System.Text.Encoding.ASCII.GetString(dataBuffer, 0, byteCount);
                    Console.WriteLine("{0}", responseData);

                    if (responseData.StartsWith("shutdown"))
                    {
                        client.Close();
                        return;
                    }
                }
            }
        }
    }
}
