using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Services.Players;
using System;
using System.Diagnostics;
using System.Threading;

namespace PeKaRaSa.MusicControl.Units
{
    /// <summary>
    /// Abracts the Audio CD unit to listen to audio CDs
    /// </summary>
    public class AudioCdUnit : AudioUnitBase
    {
        private readonly IMusicPlayerClient _mpc;

        //private bool _muted = false;
        
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
            int port = AppSettings.GetInt32OrDefault("vlcPort", 13001);

            Log.WriteLine($"StartProcess(\"cvlc\", $\"-I oldrc --rc-host localhost:{port} cdda://");
            StartProcess("cvlc", $"-I oldrc --rc-host localhost:{port} cdda://");
            Log.WriteLine($"Set initial volume to 30 on {_mpc}");
            Thread.Sleep(2000);
            _mpc.Send("volume 60");
            Play();
        }

        private void StartProcess(string fileName, string arguments)
        {
            try
            {
                Process.Start(fileName, arguments);
            }
            catch (Exception e)
            {
                Log.WriteLine($"Exception during {fileName}: {e.Message}");
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
