using System;

namespace PeKaRaSa.MusicControl.Services
{
    /// <summary>
    /// Class to log messages
    /// </summary>
    public static class Log
    {
        public static void Write(string format, params string[] arg)
        {
            Console.Write(format, arg);
        }
        public static void WriteLine(string format, params string[] arg)
        {
            Console.WriteLine(format, arg);
        }
    }
}
