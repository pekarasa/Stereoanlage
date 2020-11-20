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
            IAudioUnit result = _sut.GetActiveUnit("radio", null);

            // assert
            result.GetType().Name.Should().Be("RadioUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAndAudioCdIsInserte_ThenAudioCdUnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Returns(MediumType.AudioCd);
            LoopbackMock vlcMockPlayer = new LoopbackMock(13001);
            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", null);

            // assert
            result.GetType().Name.Should().Be("AudioCdUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAnDvdIsInserte_ThenDvdUnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Returns(MediumType.Dvd);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", null);

            // assert
            result.GetType().Name.Should().Be("DvdUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAnMp3CdIsInserte_ThenMp3UnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Returns(MediumType.Mp3);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", null);

            // assert
            result.GetType().Name.Should().Be("Mp3Unit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAnMultipleAlbummsCdIsInserte_ThenMultipleAlbummsUnitIsReturned()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Returns(MediumType.MultipleAlbumms);

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", null);

            // assert
            result.GetType().Name.Should().Be("MultipleAlbummsUnit");
        }

        [Test]
        public void GetActiveUnit_WhenUnitChanges_ThenKillIsCalled()
        {
            // arrange
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Returns(MediumType.MultipleAlbumms);
            Mock<IAudioUnit> audioUnitMock = new Mock<IAudioUnit>();

            // act
            IAudioUnit result = _sut.GetActiveUnit("cd", audioUnitMock.Object);

            // assert
            audioUnitMock.Verify(m => m.Kill(), Times.Once);
        }

        [Test]
        public void GetActiveUnit_WhenCalledForNone_ThenNullIsReturned()
        {
            // act
            IAudioUnit result = _sut.GetActiveUnit("none", null);

            // assert
            result.Should().BeNull();
        }

        [Test]
        public void GetDefaultUnit_WhenCalled_ThenRadioUnitIsReturned()
        {
            // arrange
            _sut.GetActiveUnit("none", null);

            // act
            IAudioUnit result = _sut.GetDefaultUnit();

            // assert
            result.GetType().Name.Should().Be("RadioUnit");
        }

        [Test]
        public void GetActiveUnit_WhenCalledForCdAndThenForRadio_ThenRadioIsReturnedWithoutBlocking()
        {
            // arrage
            _mediumTypeService.Setup(m => m.GetInsertedDiscType()).Callback(() => Thread.SpinWait(5000));
            Mock<IAudioUnit> audioUnitMock = new Mock<IAudioUnit>();

            List<Thread> threads = new List<Thread>();
            IAudioUnit result = null;

            threads.Add(new Thread(() =>
            {
                // make shure "radio" is called after "cd"
                Thread.Sleep(2);
                result = _sut.GetActiveUnit("radio", audioUnitMock.Object);
            }));
            threads.Add(new Thread(() => result = _sut.GetActiveUnit("cd", audioUnitMock.Object)));
            threads.Add(new Thread(() => result = _sut.GetActiveUnit("radio", audioUnitMock.Object)));
            threads.Add(new Thread(() => result = _sut.GetActiveUnit("cd", audioUnitMock.Object)));

            // act
            Stopwatch timer = new Stopwatch();
            timer.Start();

            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());

            // assert
            result.GetType().Name.Should().Be("RadioUnit");
            timer.Stop();
            timer.ElapsedMilliseconds.Should().BeLessThan(4999);
        }
    }
}