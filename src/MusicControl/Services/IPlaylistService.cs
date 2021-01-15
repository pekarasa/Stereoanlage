namespace PeKaRaSa.MusicControl.Services
{
    /// <summary>
    /// Service to deal with the play lists
    /// </summary>
    public interface IPlaylistService
    {
        /// <summary>
        /// Returns the name of the playlist in position <paramref name="index"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetPlayListName(int index);
    }
}