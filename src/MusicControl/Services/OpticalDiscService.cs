using System;
using System.Diagnostics;

namespace PeKaRaSa.MusicControl.Services
{
    public class OpticalDiscService : IOpticalDiscService
    {
        private readonly string _opticalDevice;
        private readonly string _opticalMountPoint;

        public OpticalDiscService()
        {
            _opticalDevice = AppSettings.GetValueOrDefault("OpticalDevice", "/dev/sr0");
            _opticalMountPoint = AppSettings.GetValueOrDefault("OpticalMountPoint", "/home/pi/mpd/music/mnt");
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
                using (var process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "setcd";
                    process.StartInfo.Arguments = $"-i \"{_opticalDevice}\"";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    process.WaitForExit();
                    output = process.StandardOutput.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Log.WriteLine($"Exception during setcd: {e.Message}");
            }

            Log.WriteLine(output);
            return output;
        }

        public void Mount()
        {
            try
            {
                using (var process = new Process())
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
                Log.WriteLine($"Exception during mount: {e.Message}");
            }
        }

        public void UnMount()
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "sudo";
                    process.StartInfo.Arguments = $"umount {_opticalDevice} {_opticalMountPoint}";
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                }
            }
            catch (Exception e)
            {
                Log.WriteLine($"Exception during umount: { e.Message}");
            }
        }
    }
}
