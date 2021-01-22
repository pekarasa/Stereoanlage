using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Services.Players;

namespace PeKaRaSa.MusicControl.Units
{
    /// <summary>
    /// Abstracts the radio unit to listen to radio stations via internet
    /// </summary>
    public class RadioUnit : AudioUnitBase
    {
        private readonly IPlaylistService _playlistService;

        public RadioUnit(IMusicPlayerClient mpc, IPlaylistService playlistService)
        {
            VolumeDefault = 9;
            VolumeIncrement = 3;
            VolumeMaximum = 100;
            VolumeMinimum = 0;
            Mpc = mpc;
            _playlistService = playlistService;
        }
        public override void Next()
        {
            Mpc.Send("next");
        }

        public override void Previous()
        {
            Mpc.Send("prev");
        }

        public override void Kill()
        {
            Mpc.Send("stop");
        }

        public override void Start()
        {
            Mpc.Send($"volume {VolumeDefault}");
            Play();
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
            Mpc.Send("stop");
            Mpc.Send($"volume {VolumeDefault}");
        }

        public override void Record() { }

        public override void Stop()
        {
            Mpc.Send("stop");
        }

        public override void Track(string position)
        {
            if (!int.TryParse(position, out int index))
            {
                Log.WriteLine("track number '{0}' is not a valid integer", position);
                return;
            }

            string playlist = _playlistService.GetPlayListName(index);

            if (playlist == null)
            {
                return;
            }

            Mpc.Send("clear");
            Mpc.Send($"load \"{playlist}\"");
            Mpc.Send(index == 10 ? "random on" : "random off");
            Mpc.Send("play");
        }
    }
}