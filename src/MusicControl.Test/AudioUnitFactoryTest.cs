using FluentAssertions;
using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Services;
using PeKaRaSa.MusicControl.Units;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace PeKaRaSa.MusicControl.Test
{
    public class AudioUnitFactoryTest
    {
        private AudioUnitFactory _sut;
        private Mock<IMediumTypeService> _mediumTypeService;

        [SetUp]
        public void Setup()
        {
            ConfigurationManager.AppSettings["PathToPlaylists"] = "blo";
            _mediumTypeService = new Mock<IMediumTypeService>();

            _sut = new AudioUnitFactory(_mediumTypeService.Object);
        }

        [Test]
        public void GetActiveUnit_WhenCalledForRadio_ThenRadioUnitIsReturned()
        {
            // act
            IAudioUnit result = _sut.GetActiveUnit("radio");

            // assert
            result.GetType().Name.Should().Be("RadioUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAndAudioCdIsInserte_ThenAudioCdUnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Returns(MediumType.AudioCd);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd");

            // assert
            result.GetType().Name.Should().Be("AudioCdUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAnDvdIsInserte_ThenDvdUnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Returns(MediumType.Dvd);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd");

            // assert
            result.GetType().Name.Should().Be("DvdUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAnMp3CdIsInserte_ThenMp3UnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Returns(MediumType.Mp3);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd");

            // assert
            result.GetType().Name.Should().Be("Mp3Unit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAnMultipleAlbummsCdIsInserte_ThenMultipleAlbummsUnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Returns(MediumType.MultipleAlbumms);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd");

            // assert
            result.GetType().Name.Should().Be("MultipleAlbummsUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForNone_ThenNullIsReturned()
        {
            // act
            IAudioUnit result = _sut.GetActiveUnit("none");

            // assert
            result.Should().BeNull();
        }

        [Test]
        public void GetDefaultUnit_WhenCalled_ThenRadioUnitIsReturned()
        {
            // arrange
            _sut.GetActiveUnit("none");

            // act
            IAudioUnit result = _sut.GetDefaultUnit();

            // assert
            result.GetType().Name.Should().Be("RadioUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAndThenForRadioThenRadioIsReturnedWithoutBlocking()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Callback(() => Thread.Sleep(100));
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // act
            _sut.GetActiveUnit("cd");
            IAudioUnit result = _sut.GetActiveUnit("radio");

            //
            timer.Stop();
            result.GetType().Name.Should().Be("RadioUnit");
            timer.ElapsedMilliseconds.Should().BeLessThan(99);
        }
    }
}