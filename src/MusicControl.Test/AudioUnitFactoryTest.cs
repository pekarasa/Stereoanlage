using FluentAssertions;
using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Units;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace PeKaRaSa.MusicControl.Test
{
    public class AudioUnitFactoryTest
    {
        private AudioUnitFactory _sut;
        private Mock<IMediumTypeService> _mediumTypeService;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            ConfigurationManager.AppSettings["PathToPlaylists"] = "blo";
            CancellationTokenSource cts = new CancellationTokenSource();
            _cancellationToken = cts.Token;
            _mediumTypeService = new Mock<IMediumTypeService>();

            _sut = new AudioUnitFactory(_mediumTypeService.Object);
        }

        [Test]
        public void GetActiveUnit_WhenCalledForRadio_ThenRadioUnitIsReturned()
        {
            // act
            IAudioUnit result = _sut.GetActiveUnit("radio", null, _cancellationToken);

            // assert
            result.GetType().Name.Should().Be("RadioUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAndAudioCdIsInserte_ThenAudioCdUnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType(It.IsAny<CancellationToken>())).Returns(MediumType.AudioCd);
            LoopbackMock vlcMockPlayer = new LoopbackMock(13001);
            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", null, _cancellationToken);

            // assert
            result.GetType().Name.Should().Be("AudioCdUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAndDvdIsInserte_ThenDvdUnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType(It.IsAny<CancellationToken>())).Returns(MediumType.Dvd);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", null, _cancellationToken);

            // assert
            result.GetType().Name.Should().Be("DvdUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAnMp3CdIsInserte_ThenMp3UnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType(It.IsAny<CancellationToken>())).Returns(MediumType.Mp3);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", null, _cancellationToken);

            // assert
            result.GetType().Name.Should().Be("Mp3Unit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAnMultipleAlbummsCdIsInserte_ThenMultipleAlbummsUnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType(It.IsAny<CancellationToken>())).Returns(MediumType.MultipleAlbumms);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", null, _cancellationToken);

            // assert
            result.GetType().Name.Should().Be("MultipleAlbummsUnit");
        }

        [Test]
        public void GetActiveUnit_WhenUnitChanges_ThenKillIsCalled()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType(It.IsAny<CancellationToken>())).Returns(MediumType.MultipleAlbumms);
            Mock<IAudioUnit> audioUnitMock = new Mock<IAudioUnit>();

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", audioUnitMock.Object, _cancellationToken);

            // assert
            audioUnitMock.Verify(m => m.Kill(), Times.Once);
        }

        [Test]
        public void GetActiveUnit_WhenCalledForNone_ThenNullIsReturned()
        {
            // act
            IAudioUnit result = _sut.GetActiveUnit("none", null, _cancellationToken);

            // assert
            result.Should().BeNull();
        }

        [Test]
        public void GetDefaultUnit_WhenCalled_ThenRadioUnitIsReturned()
        {
            // arrange
            _sut.GetActiveUnit("none", null, _cancellationToken);

            // act
            IAudioUnit result = _sut.GetDefaultUnit();

            // assert
            result.GetType().Name.Should().Be("RadioUnit");
        }
    }
}