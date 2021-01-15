namespace PeKaRaSa.MusicControl.Units
{
    /// <summary>
    /// Interface that each audio unit must offer
    /// </summary>
    public interface IAudioUnit
    {
        /// <summary>
        /// Stops the audio unit before switching to the new audio unit
        /// </summary>
        void Kill();

        /// <summary>
        /// Starts playback of the audio unit after switching from an other audio unit
        /// </summary>
        /// <remarks>
        /// The result must be that the audio unit plays the first track.
        /// </remarks>
        void Start();

        /// <summary>
        /// Stop the Audio Unit to switch to a new Audio Unit or to shut down this machine
        /// </summary>
        void PowerOff();

        /// <summary>
        /// Switch to track <paramref name="position"/>
        /// </summary>
        /// <param name="position">Track 0 - 29</param>
        void Track(string position);

        /// <summary>
        /// If it is a multi-disc system, switch to disc <paramref name="position"/>
        /// </summary>
        /// <param name="position">disc 1 - 9</param>
        void Disc(string position);

        /// <summary>
        /// Recording the current audio output
        /// </summary>
        void Record();

        /// <summary>
        /// Ejecting the inserted medium
        /// </summary>
        void Eject();

        /// <summary>
        /// Increases the playback speed
        /// </summary>
        void FastForward();

        /// <summary>
        /// Rewind the track
        /// </summary>
        void Rewind();

        /// <summary>
        /// Jumps to the next track
        /// </summary>
        void Next();

        /// <summary>
        /// Jumps to the previous track
        /// </summary>
        void Previous();

        /// <summary>
        /// Plays the selected track
        /// </summary>
        void Play();

        /// <summary>
        /// Pauses track playback
        /// </summary>
        void Pause();

        /// <summary>
        /// Stops track playback
        /// </summary>
        void Stop();

        /// <summary>
        /// Increases the volume
        /// </summary>
        void VolumeUp();

        /// <summary>
        /// Decreases the volume
        /// </summary>
        void VolumeDown();

        /// <summary>
        /// Mute
        /// </summary>
        void VolumeMute();

        /// <summary>
        /// Moves to the next channel
        /// </summary>
        void ChannelUp();

        /// <summary>
        /// Moves to the previous channel
        /// </summary>
        void ChannelDown();
    }
}