using System.Linq;
using System.Net;
using System.Net.Sockets;
using PeKaRaSa.MusicControl.Services;

namespace PeKaRaSa.MusicControl
{
    public class Program
    {
        /// <summary>
        /// Program which receives commands via port 13000 and controls the individual components of the music control.
        /// </summary>
        public static void Main()
        {
            TcpListener server = null;

            IOpticalDiscService opticalDiscService = new OpticalDiscService();
            IMediumTypeService mediumTypeService = new MediumTypeService(opticalDiscService);
            IAudioUnitFactory factory = new AudioUnitFactory(mediumTypeService);

            CommandExecutor commandExecutor = new CommandExecutor(factory);

            try
            {
                int port = AppSettings.GetInt32OrDefault("Port", 13000);
                IPAddress localAddress = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddress, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                var bytes = new byte[256];

                var isServerRunning = true;

                // Enter the listening loop.
                while (isServerRunning)
                {
                    Log.Write($"Waiting for a connection on port {port} ... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Log.WriteLine("Connected!");

                    string data;

                    using (NetworkStream stream = client.GetStream())
                    {
                        int i = stream.Read(bytes, 0, bytes.Length);
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Log.WriteLine(data);
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                        stream.Write(msg, 0, msg.Length);
                        client.Close();
                    }

                    if (data.StartsWith("shutdown"))
                    {
                        isServerRunning = false;
                    }
                    else
                    {
                        string[] commands = data.Trim().Split(' ').Select(a => a.Trim().ToLowerInvariant()).ToArray();
                        commandExecutor.Command(commands);
                    }

                }
            }
            catch (SocketException e)
            {
                Log.WriteLine("SocketException: {0}", e.Message);
            }
            finally
            {
                // Stop listening for new clients.
                server?.Stop();
            }
        }
    }
}