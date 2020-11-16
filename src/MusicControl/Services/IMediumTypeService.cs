namespace PeKaRaSa.MusicControl.Services
{
    /// <summary>
    /// Simplifies the handling of optical data media
    /// </summary>
    public interface IMediumTypeService
    {
        /// <summary>
        /// Determines the type of the inserted media
        /// </summary>
        /// <returns>the type of the inserted medium</returns>
        MediumType GetInsertedDiscType();
    }
}