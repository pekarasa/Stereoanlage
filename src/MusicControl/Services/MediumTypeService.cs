using System;
using System.Threading;

namespace PeKaRaSa.MusicControl.Services
{
    public class MediumTypeService : IMediumTypeService
    {
        private readonly IOpticalDiscService _opticalDiscService;
        private readonly int _millisecondsToSleepWhenDriveIsNotReady;
        private readonly int _millisecondsToSleepWhenDriveIsOpen;

        public MediumTypeService(IOpticalDiscService opticalDiscService)
        {
            _opticalDiscService = opticalDiscService;
            _millisecondsToSleepWhenDriveIsNotReady = AppSettings.GetInt32OrDefault("MillisecondsToSleepWhenDriveIsNotReady", 200);
            _millisecondsToSleepWhenDriveIsOpen = AppSettings.GetInt32OrDefault("MillisecondsToSleepWhenDriveIsOpen", 200);
        }

        public MediumType GetInsertedDiscType(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    string info = _opticalDiscService.GetInfo();

                    if (info.Contains("no disc", StringComparison.OrdinalIgnoreCase))
                    {
                        Thread.Sleep(_millisecondsToSleepWhenDriveIsNotReady);
                    }
                    else if (info.Contains("is open", StringComparison.OrdinalIgnoreCase))
                    {
                        Thread.Sleep(_millisecondsToSleepWhenDriveIsOpen);
                    }
                    else if (info.Contains("not ready", StringComparison.OrdinalIgnoreCase))
                    {
                        Thread.Sleep(_millisecondsToSleepWhenDriveIsNotReady);
                    }
                    else if (info.Contains("mixed type CD (data/audio)", StringComparison.OrdinalIgnoreCase) || info.Contains("audio disc"))
                    {
                        return MediumType.AudioCd;
                    }
                    else if (info.Contains("data disc type 1"))
                    {
                        // DataCD or DVD
                        _opticalDiscService.UnMount();
                        _opticalDiscService.Mount();
                        bool isDvd = _opticalDiscService.FindFile("AUDIO_TS");
                        if (isDvd)
                        {
                            return MediumType.Dvd;
                        }

                        bool isMultipleAlbumms = _opticalDiscService.FindFile("MultipleAlbums.md");
                        if (isMultipleAlbumms)
                        {
                            return MediumType.MultipleAlbums;
                        }
                        return MediumType.Mp3;
                    }
                }
                catch 
                {
                    return MediumType.None;
                }

                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
