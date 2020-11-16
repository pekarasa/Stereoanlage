using NUnit.Framework;
using PeKaRaSa.MusicControl.Services;

namespace PeKaRaSa.MusicControl.Test.Services
{
    public class MusicPlayerClientTest
    {
        IMusicPlayerClient _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new MusicPlayerClient();
        }

        [Test]
        public void GetPlayListName_WhenCalled_ThenReturnIndexedPlaylistName()
        {
            // act
            _sut.Send("hallo welt");
        }
    }
}
