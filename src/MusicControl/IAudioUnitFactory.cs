﻿using PeKaRaSa.MusicControl.Units;

namespace PeKaRaSa.MusicControl
{
    /// <summary>
    /// Manages the various Audio Units
    /// </summary>
    public interface IAudioUnitFactory
    {
        public IAudioUnit GetActiveUnit(string unitToActivate);

        public IAudioUnit GetDefaultUnit();
    }
}