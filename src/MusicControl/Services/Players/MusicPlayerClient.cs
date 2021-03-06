﻿using System;
using System.Diagnostics;
using System.Threading;

namespace PeKaRaSa.MusicControl.Services.Players
{
    /// <summary>
    /// Communicates via mpc with the music player daemon (mpd)
    /// </summary>
    /// <remarks>
    /// Alternatively it would be possible to communicate directly with the mpd.
    /// <seealso cref="https://www.musicpd.org/doc/html/protocol.html"/>
    /// </remarks>
    public class MusicPlayerClient : IMusicPlayerClient
    {
        public void Send(string command)
        {
            Log.WriteLine("\t\t\t\t***\tmpc {0}", command);

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "mpc";
                    process.StartInfo.Arguments = command;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Log.WriteLine($"Exception during mpc: {e.Message}");
            }
        }
    }
}