using PeKaRaSa.MusicControl;
using PeKaRaSa.MusicControl.Services;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;

public class Program
{
    /// <summary>
    /// Program which receives commands via port 13000 and controls the individual components of the music control.
    /// </summary>
    public static void Main()
    {
        TcpListener server = null;

        IOpticalDiscService opticalDiscService = new OpticalDiscService();
        IMediumTypeService _mediumTypeService = new MediumTypeService(opticalDiscService);
        IAudioUnitFactory factory = new AudioUnitFactory(_mediumTypeService);

        CommandExecutor commandExecutor = new CommandExecutor(factory);

        try
        {
            if(!Int32.TryParse(ConfigurationManager.AppSettings["Port"], out Int32 port))
            {
                // Fallback: Set the TcpListener on port 13000.
                port = 13000;
            }
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            bool isServerRunning = true;

            // Enter the listening loop.
            while (isServerRunning)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    // Process the data sent by the client.
                    System.Collections.Generic.IEnumerable<string> arguments = data.Split(' ').Select(a => a.Trim().ToLower());

                    if (data.StartsWith("shutdown"))
                    {
                        isServerRunning = false;
                    }

                    commandExecutor.Command(data.Split(' ').Select(a => a.Trim().ToLowerInvariant()));

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                }

                // Shutdown and end connection
                client.Close();
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }
    }
}