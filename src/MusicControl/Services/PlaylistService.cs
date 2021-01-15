using System.IO;
using System.Linq;

namespace PeKaRaSa.MusicControl.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IFileAccess _directoryInfo;

        public PlaylistService(IFileAccess directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }


        /// <summary>
        /// Returns the playlist name referenced by index.
        /// </summary>
        /// <remarks>
        /// If the index is too large or small, null is returned.
        /// </remarks>
        public string GetPlayListName(int index)
        {
            string[] sortedPlaylistNames = _directoryInfo.GetFiles().OrderBy(f => f).ToArray();
            string fileName = null;

            if (index < 1)
            {
                return null;
            }

            if (index > sortedPlaylistNames.Count())
            {
                return null;
            }

            fileName = sortedPlaylistNames[index - 1];

            return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
