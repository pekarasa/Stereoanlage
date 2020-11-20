﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Services;
using System.Configuration;

namespace PeKaRaSa.MusicControl.Test.Services
{
    public class MediumTypeServiceTest
    {
        private IMediumTypeService _sut;
        private Mock<IOpticalDiscService> _opticalDiscService;

        [SetUp]
        public void Setup()
        {
            _opticalDiscService = new Mock<IOpticalDiscService>();
            _sut = new MediumTypeService(_opticalDiscService.Object);

            ConfigurationManager.AppSettings["MillisecondsToSleepWhenDriveIsNotReady"] = "0";
            ConfigurationManager.AppSettings["MillisecondsToSleepWhenDriveIsOpen"] = "0";
        }

        [Test]
        public void GetType_WhenAudioCdIsInserted_ThenReturnAudioCd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("is open").Returns("drive is not ready").Returns("audio disc");

            // act
            MediumType resul = _sut.GetInsertedDiscType();

            // assert
            resul.Should().Be(MediumType.AudioCd);
        }

        [Test]
        public void GetType_WhenMixedTypeIsInserted_ThenReturnAudioCd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("is open").Returns("drive is not ready").Returns("mixed type CD (data/audio)");

            // act
            MediumType resul = _sut.GetInsertedDiscType();

            // assert
            resul.Should().Be(MediumType.AudioCd);
        }

        [Test]
        public void GetType_WhenDvdIsInserted_ThenReturnDvd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("is open").Returns("drive is not ready").Returns("data disc type 1");
            _opticalDiscService.Setup(m => m.FindFile("AUDIO_TS")).Returns(true);

            // act
            MediumType resul = _sut.GetInsertedDiscType();

            // assert
            _opticalDiscService.Verify(m => m.Unmount());
            _opticalDiscService.Verify(m => m.Mount());
            resul.Should().Be(MediumType.Dvd);
        }

        [Test]
        public void GetType_WhenMMultipleAlbumsDiscIsInserted_ThenReturnDvd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("is open").Returns("drive is not ready").Returns("data disc type 1");
            _opticalDiscService.Setup(m => m.FindFile("AUDIO_TS")).Returns(false);
            _opticalDiscService.Setup(m => m.FindFile("MultipleAlbums.md")).Returns(true);

            // act
            MediumType resul = _sut.GetInsertedDiscType();

            // assert
            _opticalDiscService.Verify(m => m.Unmount());
            _opticalDiscService.Verify(m => m.Mount());
            resul.Should().Be(MediumType.MultipleAlbumms);
        }

        [Test]
        public void GetType_WhenMp3DiscIsInserted_ThenReturnDvd()
        {
            // arrange
            _opticalDiscService.SetupSequence(m => m.GetInfo()).Returns("is open").Returns("drive is not ready").Returns("data disc type 1");
            _opticalDiscService.Setup(m => m.FindFile("AUDIO_TS")).Returns(false);
            _opticalDiscService.Setup(m => m.FindFile("MultipleAlbums.md")).Returns(false);

            // act
            MediumType resul = _sut.GetInsertedDiscType();

            // assert
            _opticalDiscService.Verify(m => m.Unmount());
            _opticalDiscService.Verify(m => m.Mount());
            resul.Should().Be(MediumType.Mp3);
        }
    }
}