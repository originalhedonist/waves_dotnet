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
        public CreateFileRequestChannelSettings Channel0 { get; set; }
        public CreateFileRequestChannelSettings Channel1 { get; set; }
    }

    public class CreateFileRequestChannelSettings
    {
        public string WaveformExpression { get; set; }
        public int SectionLengthSeconds { get; set; }
        public int MinFeatureLengthSeconds { get; set; }
        public int MaxFeatureLengthSeconds { get; set; }
        public CreateFileRequestVariance FeatureLengthVariation { get; set; }

    }

    public class CreateFileRequestVariance
    {
        public double Randomness { get; set; }
        public double Progression { get; set; }
    }

    public class CreateFileResponse
    {

    }
}
