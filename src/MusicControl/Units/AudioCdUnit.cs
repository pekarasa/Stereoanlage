using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Services.Players;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace PeKaRaSa.MusicControl.Units
{
    /// <summary>
    /// Abracts the Audio CD unit to listen to audio CDs
    /// </summary>
    public class AudioCdUnit : AudioUnitBase
    {
        public AudioCdUnit(IMusicPlayerClient mpc)
        {
            VolumeDefault = 150;
            VolumeIncrement = 15;
            VolumeMaximum = 300;
            VolumeMinimum = 0;
            Mpc = mpc;
        }

        public override void FastForward()
        {
            Mpc.Send("fastforward");
        }

        public override void Rewind()
        {
            Mpc.Send("rewind");
        }

        public override void Next()
        {
            Mpc.Send("next");
        }

        public override void Previous()
        {
            Mpc.Send("previous");
        }


        public override void Kill()
        {
            Stop();
            Mpc.Send("shutdown");
            StartProcess("kill", "$(sudo ps aux | grep 'vlc' | awk '{print $2}')");
        }

        public override void Start()
        {
            int port = AppSettings.GetInt32OrDefault("vlcPort", 13001);

            Log.WriteLine($"StartProcess(\"cvlc\", $\"-I oldrc --rc-host localhost:{port} cdda://");
            //StartProcess("cvlc", $"-I oldrc --rc-host localhost:{port} cdda://");
            Thread.Sleep(2000);
            Mpc.Send($"volume {VolumeDefault}");
        }

        private void StartProcess(string fileName, string arguments)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.FileName = fileName;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.CreateNoWindow = true;
                    foreach(DictionaryEntry a in process.StartInfo.EnvironmentVariables)
                    {
                        Log.WriteLine($"Process {a.Key}: '{a.Value}'");
                    }
                    process.Start();
                }
                Thread.Sleep(500);
            }
            catch (Exception e)
            {
                Log.WriteLine($"Exception during {fileName}: {e.Message}");
            }
        }

        public override void Pause()
        {
            Mpc.Send("pause");
        }

        public override void Play()
        {
            Mpc.Send("play");
        }

        public override void PowerOff()
        {
            Kill();
        }

        public override void Eject()
        {
            Kill();
            // todo eject the disc
        }

        public override void Record()
        {
            //echo "ripit AudioCD" >> $musicCenterLog
            //sudo ripit--nointeraction - W--coder = 2 - e--overwrite e -o / home / pi / mpd / music /
        }

        public override void Stop()
        {
            Mpc.Send("stop");
        }

        public override void Track(string position)
        {
            Mpc.Send($"goto {position}");
        }
    }
}
