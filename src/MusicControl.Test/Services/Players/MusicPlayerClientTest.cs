using NUnit.Framework;
using PeKaRaSa.MusicControl.Services.Players;

namespace PeKaRaSa.MusicControl.Test.Services.Players
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
