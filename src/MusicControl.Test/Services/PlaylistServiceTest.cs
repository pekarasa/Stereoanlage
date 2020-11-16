using FluentAssertions;
using Moq;
using NUnit.Framework;
using PeKaRaSa.MusicControl.Services;
using System.Collections.Generic;

namespace PeKaRaSa.MusicControl.Test.Services
{
    public class PlaylistServiceTest
    {
        IPlaylistService _sut;
        private Mock<IFileAccess> _fileAccess;

        [SetUp]
        public void Setup()
        {
            _fileAccess = new Mock<IFileAccess>();
            _sut = new PlaylistService(_fileAccess.Object);
        }

        [Test]
        public void GetPlayListName_WhenCalled_ThenReturnIndexedPlaylistName()
        {
            // arrange
            _fileAccess.Setup(m => m.GetFiles()).Returns(new List<string> { "b.m3u", "c.m3u", "a.m3u", "d.m3u" });
            // act
            string result = _sut.GetPlayListName(3);

            // assert
            result.Should().Be("c");
        }

        [Test]
        public void GetPlayListName_WhenIndexIsGreaterThanLargestIndex_ThenReturnNull()
        {
            // arrange
            _fileAccess.Setup(m => m.GetFiles()).Returns(new List<string> { "a.m3u", "b.m3u", "c.m3u", "d.m3u" });
            // act
            string result = _sut.GetPlayListName(5);

            // assert
            result.Should().BeNull();
        }

        [Test]
        public void GetPlayListName_WhenIndexIsSmallerThanSmalestIndex_ThenReturnNull()
        {
            // arrange
            _fileAccess.Setup(m => m.GetFiles()).Returns(new List<string> { "a.m3u", "b.m3u", "c.m3u", "d.m3u" });
            // act
            string result = _sut.GetPlayListName(0);

            // assert
            result.Should().BeNull();
        }
    }
}
