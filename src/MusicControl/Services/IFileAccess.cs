﻿using System.Collections.Generic;
using System.IO;

namespace PeKaRaSa.MusicControl.Services
{
    /// <summary>
    /// Abstracts access to the file system
    /// </summary>
    public interface IFileAccess
    {
        IEnumerable<string> GetFiles();
    }
}