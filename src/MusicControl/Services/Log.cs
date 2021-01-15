using System;

namespace PeKaRaSa.MusicControl.Services
{
    /// <summary>
    /// Class to log messages
    /// </summary>
    public static class Log
    {
        public static void Write(string format, params object[] arg)
        {
            Console.Write(format, arg);
        }
        public static void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }
    }
}
