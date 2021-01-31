using PeKaRaSa.MusicControl.Services.Players;

namespace PeKaRaSa.MusicControl.Units
{
    public abstract class AudioUnitBase : IAudioUnit
    {
        protected int Volume { get; set; }
        private readonly int _volumeInitial;
        private readonly int _volumeIncrement;
        private readonly int _volumeMaximum;
        private readonly int _volumeMinimum;
        private bool _isMuted;
        protected IMusicPlayerClient Mpc { get; set; }

        protected AudioUnitBase(IMusicPlayerClient mpc, int volumeInitial, int volumeIncrement, int volumeMinimum, int volumeMaximum)
        {
            Mpc = mpc;
            _volumeInitial = volumeInitial;
            Volume = _volumeInitial;
            _volumeIncrement = volumeIncrement;
            _volumeMinimum = volumeMinimum;
            _volumeMaximum = volumeMaximum;
        }

        public abstract void Kill();
        
        public abstract void Start();

        /// <summary>
        /// Moves to the next channel
        /// </summary>
        public virtual void ChannelUp()
        {
        }

        /// <summary>
        /// Moves to the previous channel
        /// </summary>
        public virtual void ChannelDown()
        {
        }

        /// <summary>
        /// Increases the volume
        /// </summary>
        public virtual void VolumeUp()
        {
            Volume += _volumeIncrement;
            Volume = Volume > _volumeMaximum ? _volumeMaximum : Volume;

            Mpc.Send($"volume {Volume}");
        }

        /// <summary>
        /// Decreases the volume
        /// </summary>
        public virtual void VolumeDown()
        {
            Volume -= _volumeIncrement;
            Volume = Volume < _volumeMinimum ? _volumeMinimum : Volume;

            Mpc.Send($"volume {Volume}");
        }

        /// <summary>
        /// Mute
        /// </summary>
        public virtual void VolumeMute()
        {
            // After muting, we start again with the initial volume
            Volume = _volumeInitial;

            Mpc.Send($"volume {(_isMuted ? Volume : 0)}");
            _isMuted = !_isMuted;
        }

        /// <summary>
        /// Plays the selected track
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// Pauses track playback
        /// </summary>
        public abstract void Pause();

        /// <summary>
        /// Stops track playback
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Jumps to the next track
        /// </summary>
        public virtual void Next() { }

        /// <summary>
        /// Jumps to the previous track
        /// </summary>
        public virtual void Previous() { }

        /// <summary>
        /// Rewind the track
        /// </summary>
        public virtual void Rewind() { }

        /// <summary>
        /// Increases the playback speed
        /// </summary>
        public virtual void FastForward() { }

        /// <summary>
        /// Ejecting the inserted medium
        /// </summary>
        public virtual void Eject() { }

        /// <summary>
        /// Recording the current audio output
        /// </summary>
        public abstract void Record();

        /// <summary>
        /// If it is a multi-disc system, switch to disc <paramref name="position"/>
        /// </summary>
        /// <param name="position">disc 1 - 9</param>
        public virtual void Disc(string position) { }

        /// <summary>
        /// Switch to track <paramref name="position"/>
        /// </summary>
        /// <param name="position">Track 0 - 29</param>
        public abstract void Track(string position);

        /// <summary>
        /// Stop the Audio Unit to switch to a new Audio Unit or to shut down this machine
        /// </summary>
        public abstract void PowerOff();
    }
}
