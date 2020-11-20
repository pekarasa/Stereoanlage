using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Services.Player;
using PeKaRaSa.MusicControl.Units;
using System.Configuration;
using System.Threading;

namespace PeKaRaSa.MusicControl
{
    public class AudioUnitFactory : IAudioUnitFactory
    {
        private readonly IMediumTypeService _mediumTypeService;
        private Thread _thread;

        private readonly IAudioUnit _radio;
        private readonly IAudioUnit _audioCd;

        public AudioUnitFactory(IMediumTypeService mediumTypeService)
        {
            _mediumTypeService = mediumTypeService;

            _radio = new RadioUnit(new MusicPlayerClient(), new PlaylistService(new FileAccess(ConfigurationManager.AppSettings["PathToPlaylists"])));
            _audioCd = new AudioCdUnit(new VlcMediaPlayer());
        }

        public IAudioUnit GetActiveUnit(string unitToActivate, IAudioUnit currentUnit)
        {
            IAudioUnit newUnit;
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

                    newUnit = _radio;
                    break;
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
                    newUnit = type switch
                    {
                        MediumType.AudioCd => _audioCd,
                        MediumType.Dvd => _radio,
                        MediumType.Mp3 => _radio,
                        MediumType.MultipleAlbumms => _radio,
                        _ => currentUnit,
                    };
                    break;
                default:
                    return null; // echo "$ActiveUnit: 1: unknown ActiveUnit" >> $musicCenterLog ;;
            };
            // kill deactivated unit and start new unit
            if (currentUnit != newUnit)
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