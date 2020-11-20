﻿using System;
using System.Configuration;
using System.Net.Sockets;

namespace PeKaRaSa.MusicControl.Services.Players
{
    /// <summary>
    /// Communicates via mpc with the music player deamon (mpd)
    /// </summary>
    /// <remarks>
    /// Alternatively it would be possible to communicate directly with the mpd.
    /// <seealso cref="https://www.musicpd.org/doc/html/protocol.html"/>
    /// </remarks>
    public class VlcMediaPlayer : IMusicPlayerClient
    {
        private int _port;

        public VlcMediaPlayer()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["vlcPort"], out _port))
            {
                // Fallback: Set the port 13001.
                _port = 13001;
            }
        }

        public void Send(string command)
        {
            using var tcpClient = new TcpClient("localhost", _port);
            using var stream = tcpClient.GetStream();

            byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
            stream.Write(data, 0, data.Length);

            stream.Close();
            tcpClient.Close();
        }
    }
}