using PeKaRaSa.MusicControl.Units;
using System.Threading;

namespace PeKaRaSa.MusicControl
{
    /// <summary>
    /// Manages the various Audio Units
    /// </summary>
    public interface IAudioUnitFactory
    {
        /// <summary>
        /// Returns an instance of the <paramref name="unitToActivate"/> or null if the requested unit does not exist.
        /// </summary>
        /// <param name="unitToActivate">requested unit to activate</param>
        /// <param name="currentUnit">unit to stop</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public IAudioUnit GetActiveUnit(string unitToActivate, IAudioUnit currentUnit, CancellationToken token);

        public IAudioUnit GetDefaultUnit();
    }
}