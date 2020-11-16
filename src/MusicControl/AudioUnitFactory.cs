using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Units;
using System.Configuration;
using System.Threading;

namespace PeKaRaSa.MusicControl
{
    public class AudioUnitFactory : IAudioUnitFactory
    {
        private readonly IMediumTypeService _mediumTypeService;
        private Thread _thread;

        private readonly IMusicPlayerClient _mpc;
        private readonly IFileAccess _fileAccess;
        private readonly IPlaylistService _playlistService;
        private readonly IAudioUnit _radio;

        public AudioUnitFactory(IMediumTypeService mediumTypeService)
        {
            _mediumTypeService = mediumTypeService;

            _mpc = new MusicPlayerClient();
            _fileAccess = new FileAccess(ConfigurationManager.AppSettings["PathToPlaylists"]);
            _playlistService = new PlaylistService(_fileAccess);
            _radio = new RadioUnit(_mpc, _playlistService);
        }

        public IAudioUnit GetActiveUnit(string unitToActivate, IAudioUnit currentUnit)
        {
            switch (unitToActivate)
            {
                case "radio":
                    if (_thread != null)
                    {
                        _thread.Interrupt();
                        do
                        {
                            Thread.Sleep(10);
                        } while (_thread != null);
                    }

                    return _radio;
                case "cd":
                    MediumType type = MediumType.None;
                    try
                    {
                        _thread = new Thread(() => type = _mediumTypeService.GetInsertedDiscType())
                        {
                            Priority = ThreadPriority.Lowest
                        };
                        _thread.Start();
                        _thread.Join();
                    }
                    catch (ThreadInterruptedException)
                    {
                    }
                    finally
                    {
                        _thread = null;
                    }
                    IAudioUnit newUnit = type switch
                    {
                        MediumType.AudioCd => _radio,
                        MediumType.Dvd => _radio,
                        MediumType.Mp3 => _radio,
                        MediumType.MultipleAlbumms => _radio,
                        _ => currentUnit,
                    };

                    // kill deactivated unit
                    if (currentUnit != newUnit)
                    {
                        currentUnit?.Kill();
                    }
                    return newUnit;

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