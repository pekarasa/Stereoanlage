using PeKaRaSa.MusicControl.Units;
using System.Threading;

namespace PeKaRaSa.MusicControl
{
    /// <summary>
    /// Manages the various Audio Units
    /// </summary>
    public interface IAudioUnitFactory
    {
        public IAudioUnit GetActiveUnit(string unitToActivate, IAudioUnit currentUnit, CancellationToken? token);

        public IAudioUnit GetDefaultUnit();
    }
}