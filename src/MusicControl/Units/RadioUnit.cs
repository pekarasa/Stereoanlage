using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Services.Player;
using System;

namespace PeKaRaSa.MusicControl.Units
{
    /// <summary>
    /// Abstracts the radio unit to listen to radio stations via internet
    /// </summary>
    public class RadioUnit : AudioUnitBase
    {
        private readonly IMusicPlayerClient _mpc;
        private readonly IPlaylistService _playlistService;
        private int _volume = 30;
        private bool _muted = false;

        public RadioUnit(IMusicPlayerClient mpc, IPlaylistService playlistService)
        {
            _mpc = mpc;
            _playlistService = playlistService;
        }

        public override void Kill()
        {
            _mpc.Send("stop");
        }

        public override void Start()
        {
            Play();
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
            _mpc.Send("stop");
            _mpc.Send("volume 30");
        }

        public override void Record() { }

        public override void Stop()
        {
            _mpc.Send("stop");
        }

        public override void Track(string position)
        {
            if (!int.TryParse(position, out int index))
            {
                Console.WriteLine("track number '{0}' is not a valid integer", position);
                return;
            }

            string playlist = _playlistService.GetPlayListName(index);

            if (playlist != null)
            {
                _mpc.Send("clear");
                _mpc.Send($"load \"{playlist}\"");
                _mpc.Send("play");
            }
        }

        public override void VolumeDown()
        {
            _volume -= 3;
            _volume = _volume < 0 ? 0 : _volume;

            _mpc.Send($"volume {_volume}");
        }

        public override void VolumeMute()
        {
            _mpc.Send($"volume {(_muted ? _volume : 0)}");
            _muted = !_muted;
        }

        public override void VolumeUp()
        {
            _volume += 3;
            _volume = _volume > 100 ? 100 : _volume;

            _mpc.Send("volume {_volume}");
        }
    }
}