using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Services.Players;
using PeKaRaSa.MusicControl.Units;
using System.Threading;

namespace PeKaRaSa.MusicControl
{
    public class AudioUnitFactory : IAudioUnitFactory
    {
        private readonly IMediumTypeService _mediumTypeService;

        private readonly IAudioUnit _radio;
        private readonly IAudioUnit _audioCd;

        public AudioUnitFactory(IMediumTypeService mediumTypeService)
        {
            _mediumTypeService = mediumTypeService;

            _radio = new RadioUnit(new MusicPlayerClient(), new PlaylistService(new FileAccess(AppSettings.GetValueOrDefault("PathToPlaylists", "/home/pi/mpd/playlists"))));
            _audioCd = new AudioCdUnit(new VlcMediaPlayer());
        }

        public IAudioUnit GetActiveUnit(string unitToActivate, IAudioUnit currentUnit, CancellationToken token)
        {
            IAudioUnit newUnit;
            switch (unitToActivate)
            {
                case "radio":
                    newUnit = _radio;
                    break;
                case "cd":
                    MediumType type = MediumType.None;
                    type = _mediumTypeService.GetInsertedDiscType(token);
                    newUnit = type switch
                    {
                        MediumType.AudioCd => _audioCd,
                        MediumType.Dvd => _radio,
                        MediumType.Mp3 => _radio,
                        MediumType.MultipleAlbumms => _radio,
                        _ => null,
                    };
                    break;
                default:
                    return null; // echo "$ActiveUnit: 1: unknown ActiveUnit" >> $musicCenterLog ;;
            };

            // when switching to a new unit, the old unit is deactivated and the new unit is started
            if (newUnit != null && currentUnit != newUnit)
            {
                currentUnit?.Kill();
                newUnit?.Start();
            }

            return newUnit;
        }

        public IAudioUnit GetDefaultUnit()
        {
            return _radio;
        }
    }
}