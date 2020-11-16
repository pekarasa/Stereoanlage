using System;
using System.Configuration;
using System.Diagnostics;

namespace PeKaRaSa.MusicControl.Services
{
    public class OpticalDiscService : IOpticalDiscService
    {
        private readonly string _opticalDevice;
        private readonly string _opticalMountpoint;

        public OpticalDiscService()
        {
            _opticalDevice = ConfigurationManager.AppSettings["OpticalDevice"];
            _opticalMountpoint = ConfigurationManager.AppSettings["OpticalMountpoint"];
        }

        public bool FindFile(string v)
        {
            throw new NotImplementedException();
        }

        public string GetInfo()
        {
            // todo: remove the default value and replace it with "not ready"
            string output = "audio disc";

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "setcd";
                    process.StartInfo.Arguments = $"-i \"{_opticalDevice}\"";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return output;
        }

        public void Mount()
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "sudo";
                    process.StartInfo.Arguments = $"mount {_opticalDevice} ";
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Unmount()
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "sudo";
                    process.StartInfo.Arguments = $"umount {_opticalDevice} {_opticalMountpoint}";
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
