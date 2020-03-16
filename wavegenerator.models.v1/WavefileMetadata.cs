using System;

namespace wavegenerator.models
{
    public class WavefileMetadata : IWaveFileMetadata
    {
        public WavefileMetadata(short numberOfChannels, bool phaseShiftCarrier, bool phaseShiftPulses, bool randomization, TimeSpan trackLength)
        {
            NumberOfChannels = numberOfChannels;
            PhaseShiftCarrier = phaseShiftCarrier;
            PhaseShiftPulses = phaseShiftPulses;
            Randomization = randomization;
            TrackLength = trackLength;
        }

        public short NumberOfChannels { get; }
        public bool PhaseShiftCarrier { get; }
        public bool PhaseShiftPulses { get; }
        public bool Randomization { get; }
        public TimeSpan TrackLength { get; }
        public double TrackLengthSeconds() => TrackLength.TotalSeconds;
    }
}