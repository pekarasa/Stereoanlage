using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Units;
using System.Configuration;

namespace PeKaRaSa.MusicControl
{
    public class AudioUnitFactory : IAudioUnitFactory
    {
        readonly IMediumTypeService _mediumTypeService;

        readonly IMusicPlayerClient _mpc;
        readonly IFileAccess _fileAccess;
        readonly IPlaylistService _playlistService;
        readonly IAudioUnit _radio;

        public AudioUnitFactory(IMediumTypeService mediumTypeService)
        {
            _mediumTypeService = mediumTypeService;

            _mpc = new MusicPlayerClient();
            _fileAccess = new FileAccess(ConfigurationManager.AppSettings["PathToPlaylists"]);
            _playlistService = new PlaylistService(_fileAccess);
            _radio = new RadioUnit(_mpc, _playlistService);
        }

        public IAudioUnit GetActiveUnit(string unitToActivate)
        {
            switch (unitToActivate)
            {
                case "radio":
                    return _radio;
                //"stream" => new StreamUnit(),// ActiveUnit=LocalPlaylists
                case "cd":
                    MediumType type = _mediumTypeService.GetInsertedDiscType();
                    return type switch
                    {
                        MediumType.AudioCd => _radio,
                        MediumType.Dvd => _radio,
                        MediumType.Mp3 => _radio,
                        MediumType.MultipleAlbumms => _radio,
                        _ => null,
                    };
                default:
                    return null; // echo "$ActiveUnit: 1: unknown ActiveUnit" >> $musicCenterLog ;;
            };
        }

        public IAudioUnit GetDefaultUnit()
        {
            return _radio;
        }
    }
}