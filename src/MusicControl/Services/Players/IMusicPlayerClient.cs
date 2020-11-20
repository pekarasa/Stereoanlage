namespace PeKaRaSa.MusicControl.Services.Players
{
    public interface IMusicPlayerClient
    {
        /// <summary>
        /// Send the <paramref name="command"/> to the music player client
        /// </summary>
        /// <param name="command"></param>
        void Send(string command);
    }
}