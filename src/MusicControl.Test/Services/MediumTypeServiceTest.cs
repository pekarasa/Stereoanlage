using FluentAssertions;
using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Services;
using System;
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
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("No disc is inserted").Returns("CD tray is open").Returns("Drive is not ready").Returns("Disc found in drive: audio disc");

            // act
            MediumType result = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            result.Should().Be(MediumType.AudioCd);
        }

        [Test]
        public void GetType_WhenMixedTypeIsInserted_ThenReturnAudioCd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("CD tray is open").Returns("Drive is not ready").Returns("mixed type CD (data/audio)");

            // act
            MediumType result = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            result.Should().Be(MediumType.AudioCd);
        }

        [Test]
        public void GetType_WhenDvdIsInserted_ThenReturnDvd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("CD tray is open").Returns("Drive is not ready").Returns("data disc type 1");
            _opticalDiscService.Setup(m => m.FindFile("AUDIO_TS")).Returns(true);

            // act
            MediumType result = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            _opticalDiscService.Verify(m => m.UnMount());
            _opticalDiscService.Verify(m => m.Mount());
            result.Should().Be(MediumType.Dvd);
        }

        [Test]
        public void GetType_WhenMultipleAlbumsDiscIsInserted_ThenReturnDvd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("CD tray is open").Returns("Drive is not ready").Returns("data disc type 1");
            _opticalDiscService.Setup(m => m.FindFile("AUDIO_TS")).Returns(false);
            _opticalDiscService.Setup(m => m.FindFile("MultipleAlbums.md")).Returns(true);

            // act
            MediumType result = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            _opticalDiscService.Verify(m => m.UnMount());
            _opticalDiscService.Verify(m => m.Mount());
            result.Should().Be(MediumType.MultipleAlbums);
        }

        [Test]
        public void GetType_WhenMp3DiscIsInserted_ThenReturnDvd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("CD tray is open").Returns("Drive is not ready").Returns("data disc type 1");
            _opticalDiscService.Setup(m => m.FindFile("AUDIO_TS")).Returns(false);
            _opticalDiscService.Setup(m => m.FindFile("MultipleAlbums.md")).Returns(false);

            // act
            MediumType result = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            _opticalDiscService.Verify(m => m.UnMount());
            _opticalDiscService.Verify(m => m.Mount());
            result.Should().Be(MediumType.Mp3);
        }

        [Test]
        public void GetType_WhenAnExceptionOccurs_ThenReturnNone()
        {
            // arrange
            _opticalDiscService.Setup(m => m.GetInfo()).Callback(() => { throw new Exception(); });

            // act
            MediumType result = _sut.GetInsertedDiscType(_cancellationToken);

            // assert
            result.Should().Be(MediumType.None);
        }

        [Test]
        public void GetType_WhenCanceled_ThenRaiseOperationCanceledException()
        {
            // arrange
            _opticalDiscService.Setup(m => m.GetInfo()).Returns("CD tray is open");
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();

            // act
            Action action = () => _sut.GetInsertedDiscType(cts.Token);

            // assert
            action.Should().ThrowExactly<OperationCanceledException>();
        }
    }
}
