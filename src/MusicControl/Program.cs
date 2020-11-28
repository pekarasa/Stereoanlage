using PeKaRaSa.MusicControl;
using PeKaRaSa.MusicControl.Services;
using System;
using System.Collections.Generic;
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
            int port = AppSettings.GetInt32OrDefault("Port", 13000);
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
                Log.Write($"Waiting for a connection on port {port} ... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Log.WriteLine("Connected!");

                data = null;

                using (NetworkStream stream = client.GetStream())
                {
                    int i = stream.Read(bytes, 0, bytes.Length);
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Log.WriteLine(data);
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                    stream.Write(msg, 0, msg.Length);
                    client.Close();
                }

                IEnumerable<string> arguments = data.Split(' ').Select(a => a.Trim().ToLower());

                if (data.StartsWith("shutdown"))
                {
                    isServerRunning = false;
                    return;
                }
                else
                {
                    List<string> commands = data.Trim().Split(' ').Select(a => a.Trim().ToLowerInvariant()).ToList();
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
            server.Stop();
        }
    }
}