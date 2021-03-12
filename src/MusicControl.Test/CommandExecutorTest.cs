using FluentAssertions;
using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Units;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PeKaRaSa.MusicControl.Test
{
    public class CommandExecutorTest
    {
        private CommandExecutor _sut;
        private Mock<IAudioUnitFactory> _factoryMock;
        private Mock<IAudioUnit> _audioUnitMock;

        [SetUp]
        public void Setup()
        {
            _factoryMock = new Mock<IAudioUnitFactory>();
            _audioUnitMock = new Mock<IAudioUnit>();
            _factoryMock.Setup(m => m.GetDefaultUnit()).Returns(_audioUnitMock.Object);

            _sut = new CommandExecutor(_factoryMock.Object);
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
            _factoryMock.Verify(m => m.GetDefaultUnit(), Times.Once);
            _factoryMock.Verify(m => m.GetActiveUnit(It.IsAny<string>(), null, It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void Command_WhenNoArgumentsAreGiven_ThenNoCommandIsExecutedOnAudioUnit()
        {
            // act
            _sut.Command(Array.Empty<string>());

            // assert
            _factoryMock.Verify(m => m.GetDefaultUnit(), Times.Once);
            _factoryMock.Verify(m => m.GetActiveUnit(It.IsAny<string>(), null, It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void Command_WhenCommandIsGiven_ThenThisCommandIsCalled()
        {
            // arrange
            _factoryMock.Setup(m => m.GetDefaultUnit()).Returns(_audioUnitMock.Object);
            string[] arguments = "volume up".Split(' ');

            // act
            _sut.Command(arguments);

            // assert
            _audioUnitMock.Verify(m => m.VolumeUp(), Times.Exactly(2)); // 1x Setup + 1x Command
        }
    }
}