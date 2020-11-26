using FluentAssertions;
using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Services;
using System.Configuration;
using System.Threading;

namespace PeKaRaSa.MusicControl.Test.Services
{
    public class MediumTypeServiceTest
    {
        private IMediumTypeService _sut;
        private CancellationToken _cancellationToken;
        private Mock<IOpticalDiscService> _opticalDiscService;

        [SetUp]
        public void Setup()
        {
            ConfigurationManager.AppSettings["MillisecondsToSleepWhenDriveIsNotReady"] = "0";
            ConfigurationManager.AppSettings["MillisecondsToSleepWhenDriveIsOpen"] = "0";

            CancellationTokenSource cts = new CancellationTokenSource();
            _cancellationToken = cts.Token;

            _opticalDiscService = new Mock<IOpticalDiscService>();
            _sut = new MediumTypeService(_opticalDiscService.Object);
        }

        [Test]
        public void GetType_WhenAudioCdIsInserted_ThenReturnAudioCd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("CD tray is open").Returns("Drive is not ready").Returns("Disc found in drive: audio disc");

            // act
            MediumType resul = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            resul.Should().Be(MediumType.AudioCd);
        }

        [Test]
        public void GetType_WhenMixedTypeIsInserted_ThenReturnAudioCd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("CD tray is open").Returns("Drive is not ready").Returns("mixed type CD (data/audio)");

            // act
            MediumType resul = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            resul.Should().Be(MediumType.AudioCd);
        }

        [Test]
        public void GetType_WhenDvdIsInserted_ThenReturnDvd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("CD tray is open").Returns("Drive is not ready").Returns("data disc type 1");
            _opticalDiscService.Setup(m => m.FindFile("AUDIO_TS")).Returns(true);

            // act
            MediumType resul = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            _opticalDiscService.Verify(m => m.Unmount());
            _opticalDiscService.Verify(m => m.Mount());
            resul.Should().Be(MediumType.Dvd);
        }

        [Test]
        public void GetType_WhenMMultipleAlbumsDiscIsInserted_ThenReturnDvd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("CD tray is open").Returns("Drive is not ready").Returns("data disc type 1");
            _opticalDiscService.Setup(m => m.FindFile("AUDIO_TS")).Returns(false);
            _opticalDiscService.Setup(m => m.FindFile("MultipleAlbums.md")).Returns(true);

            // act
            MediumType resul = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            _opticalDiscService.Verify(m => m.Unmount());
            _opticalDiscService.Verify(m => m.Mount());
            resul.Should().Be(MediumType.MultipleAlbumms);
        }

        [Test]
        public void GetType_WhenMp3DiscIsInserted_ThenReturnDvd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("CD tray is open").Returns("Drive is not ready").Returns("data disc type 1");
            _opticalDiscService.Setup(m => m.FindFile("AUDIO_TS")).Returns(false);
            _opticalDiscService.Setup(m => m.FindFile("MultipleAlbums.md")).Returns(false);

            // act
            MediumType resul = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            _opticalDiscService.Verify(m => m.Unmount());
            _opticalDiscService.Verify(m => m.Mount());
            resul.Should().Be(MediumType.Mp3);
        }
    }
}
