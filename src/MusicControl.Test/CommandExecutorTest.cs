using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Units;
using System;

namespace PeKaRaSa.MusicControl.Test
{
    public class CommandExecutorTest
    {
        private CommandExecutor _sut;
        private Mock<IAudioUnit> _audioUnitMock;

        [SetUp]
        public void Setup()
        {
            _audioUnitMock = new Mock<IAudioUnit>();
            _sut = new CommandExecutor(_audioUnitMock.Object);
        }

        [Test]
        public void Constructor_WhenCalled_ThenStartPlaying()
        {
            // act is done in constructor

            // assert
            _audioUnitMock.Verify(m => m.Disc("1"), Times.Once);
            _audioUnitMock.Verify(m => m.Track("4"), Times.Once);
            _audioUnitMock.Verify(m => m.VolumeUp(), Times.Once);
            _audioUnitMock.Verify(m => m.Play(), Times.Once);
        }

        [Test]
        public void Command_WhenArgumentNull_ThenNoCommandIsExecutedOnAudioUnit()
        {
            // act
            _sut.Command(null);

            // assert
            _audioUnitMock.VerifyAll();
        }

        [Test]
        public void Command_WhenNoArgumentsAreGiven_ThenNoCommandIsExecutedOnAudioUnit()
        {
            // act
            _sut.Command(Array.Empty<string>());

            // assert
            _audioUnitMock.VerifyAll();
        }

        [Test]
        public void Command_WhenCommandIsGiven_ThenThisCommandIsCalled()
        {
            // arrange
            string[] arguments = "volume up".Split(' ');

            // act
            _sut.Command(arguments);

            // assert
            _audioUnitMock.Verify(m => m.VolumeUp(), Times.Exactly(2)); // 1x Setup + 1x Command
        }
    }
}