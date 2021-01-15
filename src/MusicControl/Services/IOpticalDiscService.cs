namespace PeKaRaSa.MusicControl.Services
{
    /// <summary>
    /// Simplifies the handling of optical data media
    /// </summary>
    public interface IOpticalDiscService
    {
        /// <summary>
        /// Give information on the cd rom drive. The status of the drive is  checked,  possible
        /// outcome is (a) no disc inserted, (b) tray is open, (c) drive is not ready, (d) disc
        /// is found. In the last case, an attempt is made to determine the type of disc(audio
        /// or one of 4 types of data disc), and for both "audio" and "data disc type 1|2" some
        /// additional information is given. Currently for data discs this is the volume name,
        /// publisher and  data preparer.For audio discs the extra information is very terse,
        /// you may enjoy a full-fledged audio cd player program better.
        /// </summary>
        /// <returns></returns>
        string GetInfo();

        /// <summary>
        /// If a media is mounted, it is unmounted
        /// </summary>
        void UnMount();

        /// <summary>
        /// The media is mounted
        /// </summary>
        void Mount();

        /// <summary>
        /// Checks if a file with the <paramref name="name"/> exists in the root directory
        /// of the mounted media
        /// </summary>
        /// <param name="name">This file name is searched for in the root directory</param>
        /// <returns><c>true</c> if a file was found, otherwise <c>false</c></returns>
        bool FindFile(string name);
    }
}