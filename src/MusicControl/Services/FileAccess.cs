﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PeKaRaSa.MusicControl.Services
{
    class FileAccess : IFileAccess
    {
        readonly DirectoryInfo _directoryInfo;

        public FileAccess(string playlistPath)
        {
            _directoryInfo = new DirectoryInfo(playlistPath);
        }

        public IEnumerable<string> GetFiles()
        {
            return _directoryInfo.GetFiles().Select(f => f.Name);
        }
    }
}
