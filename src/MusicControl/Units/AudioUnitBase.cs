using PeKaRaSa.MusicControl.Services.Players;

namespace PeKaRaSa.MusicControl.Units
{
    public abstract class AudioUnitBase : IAudioUnit
    {
        protected int VolumeDefault { get; set; }
        protected int VolumeIncrement { get; set; }
        protected int VolumeMaximum { get; set; }
        protected int VolumeMinimum { get; set; }
        protected bool IsMuted { get; set; } = false;
        protected IMusicPlayerClient Mpc { get; set; }

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
            VolumeDefault += VolumeIncrement;
            VolumeDefault = VolumeDefault > VolumeMaximum ? VolumeMaximum : VolumeDefault;

            Mpc.Send($"volume {VolumeDefault}");
        }

        /// <summary>
        /// Decreases the volume
        /// </summary>
        public virtual void VolumeDown()
        {
            VolumeDefault -= VolumeIncrement;
            VolumeDefault = VolumeDefault < VolumeMinimum ? VolumeMinimum : VolumeDefault;

            Mpc.Send($"volume {VolumeDefault}");
        }

        /// <summary>
        /// Mute
        /// </summary>
        public virtual void VolumeMute()
        {
            Mpc.Send($"volume {(IsMuted ? VolumeDefault : 0)}");
            IsMuted = !IsMuted;
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
