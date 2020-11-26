using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Units;
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
        public void Command_WhenArgumentNull_ThenNoCommandIsExecutedOnAudioUnit()
        {
            // act
            _sut.Command(null);

            // assert
            _factoryMock.Verify(m => m.GetDefaultUnit(), Times.Once);
            _factoryMock.Verify(m => m.GetActiveUnit(It.IsAny<string>(), null, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void Command_WhenNoArgumentsAreGiven_ThenNoCommandIsExecutedOnAudioUnit()
        {
            // act
            _sut.Command(new List<string>());

            // assert
            _factoryMock.Verify(m => m.GetDefaultUnit(), Times.Once);
            _factoryMock.Verify(m => m.GetActiveUnit(It.IsAny<string>(), null, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void Command_WhenChangeUnitFromRadioToRadio_ThenActiveUnitStaysRadio()
        {
            _factoryMock.Setup(m => m.GetDefaultUnit()).Returns(_audioUnitMock.Object);
            _factoryMock.Setup(m => m.GetActiveUnit("radio", _audioUnitMock.Object, It.IsAny<CancellationToken>())).Returns(_audioUnitMock.Object);

            // act
            _sut.Command("changeunit radio".Split(' '));

            // assert
            _factoryMock.Verify(m => m.GetDefaultUnit(), Times.Once);
            _factoryMock.Verify(m => m.GetActiveUnit("radio", _audioUnitMock.Object, It.IsAny<CancellationToken>()), Times.Once);
            _audioUnitMock.Verify(m => m.Kill(), Times.Never);
        }

        [Test]
        public void Command_WhenCommandIsGiven_ThenThisCommandIsCalled()
        {
            _factoryMock.Setup(m => m.GetDefaultUnit()).Returns(_audioUnitMock.Object);

            // act
            string[] arguments = "volume up".Split(' ');
            _sut.Command(arguments);

            // assert
            _audioUnitMock.Verify(m => m.VolumeUp(), Times.Once);
        }

    }
}