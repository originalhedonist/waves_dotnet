namespace wavegenerator.models
{
    public interface IWaveFileMetadata
    {
        short NumberOfChannels { get; }
        bool PhaseShiftCarrier { get; }
        bool PhaseShiftPulses { get; }
        bool Randomization { get; set; }
    }
}