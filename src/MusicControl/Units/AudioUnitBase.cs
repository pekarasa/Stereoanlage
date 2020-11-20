using System;
using System.Collections.Generic;
using System.Linq;

namespace PeKaRaSa.MusicControl.Units
{
    public abstract class AudioUnitBase : IAudioUnit
    {
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
        public abstract void VolumeUp();

        /// <summary>
        /// Decreases the volume
        /// </summary>
        public abstract void VolumeDown();

        /// <summary>
        /// Mute
        /// </summary>
        public abstract void VolumeMute();

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
