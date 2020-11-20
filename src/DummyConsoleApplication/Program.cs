using System;
using System.IO;
using System.Linq;

namespace PeKaRaSa.SetcdDummy
{
    class Program
    {
        /// <summary>
        /// Reads and removes the first line of setcd.dummy, appends it at the end and returns it
        /// </summary>
        /// <param name="args"></param>

        static void Main(string[] args)
        {
            var allLines = File.ReadAllLines("setcd.dummy").ToList();
            string firstLine = allLines.First();
            allLines.Add(firstLine);
            allLines.RemoveAt(0);

            File.WriteAllLines("setcd.dummy", allLines);

            Console.WriteLine(firstLine);
        }
    }
}
