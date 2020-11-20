using System;
using System.IO;
using System.Linq;

namespace PeKaRaSa.SetcdDummy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(File.ReadLines("setcd.dummy").First());
        }
    }
}
