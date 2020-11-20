using System;
using System.Configuration;
using System.Threading;

namespace PeKaRaSa.MusicControl.Services
{
    public class MediumTypeService : IMediumTypeService
    {
        private readonly IOpticalDiscService _opticalDiscService;
        private int _millisecondsToSleepWhenDriveIsNotReady;
        private int _millisecondsToSleepWhenDriveIsOpen;

        public MediumTypeService(IOpticalDiscService opticalDiscService)
        {
            _opticalDiscService = opticalDiscService;
            if (!int.TryParse(ConfigurationManager.AppSettings["MillisecondsToSleepWhenDriveIsNotReady"], out _millisecondsToSleepWhenDriveIsNotReady))
            {
                _millisecondsToSleepWhenDriveIsNotReady = 1000;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["MillisecondsToSleepWhenDriveIsOpen"], out _millisecondsToSleepWhenDriveIsOpen))
            {
                _millisecondsToSleepWhenDriveIsOpen = 3000;
            }
        }

        public MediumType GetInsertedDiscType()
        {
            while (true)
            {
                try
                {
                    string info = _opticalDiscService.GetInfo();

                    if (info.Contains("no disc inserted", StringComparison.OrdinalIgnoreCase))
                    {
                        Thread.SpinWait(_millisecondsToSleepWhenDriveIsNotReady);
                    }
                    else if (info.Contains("is open", StringComparison.OrdinalIgnoreCase))
                    {
                        Thread.SpinWait(_millisecondsToSleepWhenDriveIsOpen);
                    }
                    else if (info.Contains("not ready", StringComparison.OrdinalIgnoreCase))
                    {
                        Thread.SpinWait(_millisecondsToSleepWhenDriveIsNotReady);
                    }
                    else if (info.Contains("mixed type CD (data/audio)", StringComparison.OrdinalIgnoreCase) || info.Contains("audio disc"))
                    {
                        return MediumType.AudioCd;
                    }
                    else if (info.Contains("data disc type 1"))
                    {
                        // DataCD or DVD
                        _opticalDiscService.Unmount();
                        _opticalDiscService.Mount();
                        bool isDvd = _opticalDiscService.FindFile("AUDIO_TS");
                        if (isDvd)
                        {
                            return MediumType.Dvd;
                        }

                        bool isMultipleAlbumms = _opticalDiscService.FindFile("MultipleAlbums.md");
                        if (isMultipleAlbumms)
                        {
                            return MediumType.MultipleAlbumms;
                        }
                        return MediumType.Mp3;
                    }
                }
                catch 
                {
                    return MediumType.None;
                }
            }
        }
    }
}
