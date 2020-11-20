﻿using PeKaRaSa.MusicControl.Services.Players;
using System;
using System.Diagnostics;

namespace PeKaRaSa.MusicControl.Units
{
    /// <summary>
    /// Abracts the Audio CD unit to listen to audio CDs
    /// </summary>
    public class AudioCdUnit : AudioUnitBase
    {
        private readonly IMusicPlayerClient _mpc;

        private bool _muted = false;
        
        public AudioCdUnit(IMusicPlayerClient mpc)
        {
            _mpc = mpc;
        }

        public override void FastForward()
        {
            _mpc.Send("fastforward");
        }

        public override void Rewind()
        {
            _mpc.Send("rewind");
        }

        public override void Next()
        {
            _mpc.Send("next");
        }

        public override void Previous()
        {
            _mpc.Send("previous");
        }


        public override void Kill()
        {
            Stop();
            _mpc.Send("shutdown");
            StartProcess("kill", "$(sudo ps aux | grep 'vlc' | awk '{print $2}')");
        }

        public override void Start()
        {
            StartProcess("cvlc", "- I rc--rc - host localhost: 12345--volume - step 6 cdda://");
            _mpc.Send("volume 70");
            Track("1");
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
                    process.StartInfo.RedirectStandardOutput = false;
                    process.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception during {fileName}: {e.Message}");
            }
        }

        public override void Pause()
        {
            _mpc.Send("pause");
        }

        public override void Play()
        {
            _mpc.Send("play");
        }

        public override void PowerOff()
        {
            Kill();
        }

        public override void Eject()
        {
            Kill();
            // todo eject the fisc
        }

        public override void Record()
        {
            //echo "ripit AudioCD" >> $musicCenterLog
            //sudo ripit--nointeraction - W--coder = 2 - e--overwrite e -o / home / pi / mpd / music /
        }

        public override void Stop()
        {
            _mpc.Send("stop");
        }

        public override void Track(string position)
        {
            _mpc.Send($"goto {position}");
        }

        public override void VolumeDown()
        {
            _mpc.Send("voldown");
        }

        public override void VolumeMute()
        {
            _mpc.Send("volume 0");
            //_mpc.Send($"volume {(_muted ? _volume : 0)}");
            //_muted = !_muted;
        }

        public override void VolumeUp()
        {
            _mpc.Send("volup");
        }
    }
}