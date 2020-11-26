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
            // arrange
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
            // arrange
            _factoryMock.Setup(m => m.GetDefaultUnit()).Returns(_audioUnitMock.Object);
            string[] arguments = "volume up".Split(' ');

            // act
            _sut.Command(arguments);

            // assert
            _audioUnitMock.Verify(m => m.VolumeUp(), Times.Once);
        }

        [Test]
        public void Command_WhenTheFirstThreadSwitchesToCdAndTheSecondThreadSwitchesToRadio_ThenCdIsCanceledAndRadioIsReturnedWithoutBlocking()
        {
            // arrange
            IAudioUnit radio = new RadioUnit(null, null);
            Mock<IAudioUnitFactory> factoryMock = new Mock<IAudioUnitFactory>();
            factoryMock.Setup(m => m.GetDefaultUnit()).Returns(radio);

            factoryMock.Setup(m => m.GetActiveUnit("cd", It.IsAny<IAudioUnit>(), It.IsAny<CancellationToken>()))
                .Callback((string unitToActivate, IAudioUnit u, CancellationToken token) =>
            {
                Thread.Sleep(2000);
                // assert
                unitToActivate.Should().Be("cd");
                token.IsCancellationRequested.Should().Be(true);
                token.ThrowIfCancellationRequested();
            });

            factoryMock.Setup(m => m.GetActiveUnit("radio", It.IsAny<IAudioUnit>(), It.IsAny<CancellationToken>())).Returns(radio);

            CommandExecutor sut = new CommandExecutor(factoryMock.Object);

            string[] cdArguments = "changeUnit cd".Split(' ');
            string[] radioArguments = "changeUnit radio".Split(' ');
            List<Thread> threads = new List<Thread>();

            threads.Add(new Thread(() =>
            {
                // make shure "radio" is called after "cd"
                Thread.Sleep(50);
                sut.Command(radioArguments); ;
            }));
            threads.Add(new Thread(() => sut.Command(cdArguments)));

            // act
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());

            // assert
            factoryMock.Verify(m => m.GetActiveUnit("cd", It.IsAny<IAudioUnit>(), It.IsAny<CancellationToken>()), Times.Once);
            factoryMock.Verify(m => m.GetActiveUnit("radio", It.IsAny<IAudioUnit>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}