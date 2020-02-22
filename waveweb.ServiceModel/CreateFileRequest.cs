using ServiceStack;
using System.ComponentModel;

namespace waveweb.ServiceModel
{
    [Route("/createfile")]
    public class CreateFileRequest : IReturn<CreateFileRequest>
    {
        public bool Randomization { get; set; }
        public int TrackLengthMinutes { get; set; }
        public bool DualChannel { get; set; }
        public bool PhaseShiftCarrier { get; set; }
        public bool PhaseShiftPulses { get; set; }
        public ChannelSettings Channel0 { get; set; }
        public ChannelSettings Channel1 { get; set; }
    }

    public class ChannelSettings
    {
        public bool UseCustomWaveformExpression { get; set; }
        public string WaveformExpression { get; set; }
        public Sections Sections { get; set; }
        public FeatureProbability FeatureProbability { get; set; }
        public CarrierFrequency CarrierFrequency { get; set; }
        public PulseFrequency PulseFrequency { get; set; }
        public Wetness Wetness { get; set; }
        public Breaks Breaks { get; set; }
        public Rises Rises { get; set; }
    }

    public class Sections
    {
        public int SectionLengthSeconds { get; set; }
        public int MinFeatureLengthSeconds { get; set; }
        public int MaxFeatureLengthSeconds { get; set; }
        public Variance FeatureLengthVariation { get; set; }
        public int MinRampLengthSeconds { get; set; }
        public int MaxRampLengthSeconds { get; set; }
        public Variance RampLengthVariation { get; set; }
    }

    public class FeatureProbability
    {
        public double FrequencyWeighting { get; set; }
        public double WetnessWeighting { get; set; }
        public double NothingWeighting { get; set; }
    }

    public class CarrierFrequency
    {
        public string Left { get; set; }
        public string Right { get; set; }
    }

    public class PulseFrequency
    {
        public double Quiescent { get; set; }
        public double Low { get;set; }
        public double High { get; set; }
        public double ChanceOfHigh { get; set; }
        public Variance Variation { get; set; }
    }

    public class Variance
    {
        public double Randomness { get; set; }
        public double Progression { get; set; }
    }

    public class Wetness
    {
        public bool LinkToFeature { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public Variance Variation { get; set; }
    }

    public class Breaks
    {
        public int MinTimeSinceStartOfTrackMinutes { get; set; }
        public int MinTimeBetweenBreaksMinutes { get; set; }
        public int MaxTimeBetweenBreaksMinutes { get; set; }
        public int MinLengthSeconds { get; set; }
        public int MaxLengthSeconds { get; set; }
        public int RampLengthSeconds { get; set; }
    }

    public class Rises
    {
        public int Count { get; set; }
        public int EarliestTimeMinutes { get; set; }
        public int LengthEachSeconds { get; set; }
        public double Amount { get; set; }
    }

    public class CreateFileResponse
    {

    }
}
