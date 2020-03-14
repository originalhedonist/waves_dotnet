namespace wavegenerator.models
{
    public class WavefileMetadata : IWaveFileMetadata
    {
        private readonly double trackLengthSeconds;

        public WavefileMetadata(short numberOfChannels, bool phaseShiftCarrier, bool phaseShiftPulses, bool randomization, double trackLengthSeconds)
        {
            NumberOfChannels = numberOfChannels;
            PhaseShiftCarrier = phaseShiftCarrier;
            PhaseShiftPulses = phaseShiftPulses;
            Randomization = randomization;
            this.trackLengthSeconds = trackLengthSeconds;
        }

        public short NumberOfChannels { get; }
        public bool PhaseShiftCarrier { get; }
        public bool PhaseShiftPulses { get; }
        public bool Randomization { get; }
        public double TrackLengthSeconds() => trackLengthSeconds;
    }
}