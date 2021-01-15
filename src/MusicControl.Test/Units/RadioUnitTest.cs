using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Services.Players;
using PeKaRaSa.MusicControl.Units;

namespace PeKaRaSa.MusicControl.Test.Units
{
    public class RadioUnitTest
    {
        private RadioUnit _sut;
        private Mock<IMusicPlayerClient> _mpc;
        private Mock<IPlaylistService> _playlistService;

        [SetUp]
        public void Setup()
        {
            _mpc = new Mock<IMusicPlayerClient>();
            _playlistService = new Mock<IPlaylistService>();
            _sut = new RadioUnit(_mpc.Object, _playlistService.Object);
        }

        [Test]
        public void Kill()
        {
            // act
            _sut.Kill();

            // assert
            _mpc.Verify(m => m.Send("stop"));
        }

        [Test]
        public void Track()
        {
            // arrange
            const string expectedTrackName = "Track 3";
            _playlistService.Setup(m => m.GetPlayListName(3)).Returns(expectedTrackName);

            // act
            _sut.Track("3");

            // arrange
            _mpc.Verify(m => m.Send("clear"));
            _mpc.Verify(m => m.Send($"load \"{expectedTrackName}\""));
            _mpc.Verify(m => m.Send("play"));
        }

        [Test]
        public void Mute_WhenPressedMultipleTime_TheVolumeWillToggle()
        {
            // act
            _sut.VolumeMute();
            _mpc.Verify(m => m.Send($"volume 0"));
            _sut.VolumeMute();
            _mpc.Verify(m => m.Send($"volume 9"));
            _sut.VolumeMute();
            _mpc.Verify(m => m.Send($"volume 0"));
        }

        [Test]
        public void PowerOff()
        {
            // act
            _sut.PowerOff();

            // assert
            _mpc.Verify(m => m.Send("stop"));
            _mpc.Verify(m => m.Send("volume 9"));
        }
    }
}