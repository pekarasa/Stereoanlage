using System;
using System.Diagnostics;

namespace PeKaRaSa.MusicControl.Services
{
    public class OpticalDiscService : IOpticalDiscService
    {
        private readonly string _opticalDevice;
        private readonly string _opticalMountpoint;

        public OpticalDiscService()
        {
            _opticalDevice = AppSettings.GetValueOrDefault("OpticalDevice", "/dev/sr0");
            _opticalMountpoint = AppSettings.GetValueOrDefault("OpticalMountpoint", "/home/pi/mpd/music/mnt");
        }

        public bool FindFile(string v)
        {
            throw new NotImplementedException();
        }

        public string GetInfo()
        {
            string output = "Drive is not ready";

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
                Console.WriteLine($"Exception during setcd: {e.Message}");
            }

            Console.WriteLine(output);
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
