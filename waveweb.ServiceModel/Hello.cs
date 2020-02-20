using ServiceStack;
using System.ComponentModel;

namespace waveweb.ServiceModel
{
    [Route("/hello")]
    [Route("/hello/{Name}")]
    public class Hello : IReturn<HelloResponse>
    {
        public int TheNumber { get; set; }
        public string Name { get; set; }
    }

    public class HelloResponse
    {
        public string Result { get; set; }
    }

    [Route("/createfile")]
    public class CreateFileRequest : IReturn<CreateFileRequest>
    {
        public bool Randomization { get; set; }
        public int TrackLengthMinutes { get; set; }
        public bool DualChannel { get; set; }
        public bool PhaseShiftCarrier { get; set; }
        public bool PhaseShiftPulses { get; set; }
        public CreateFileRequestChannelSettings Left { get; set; }
        public CreateFileRequestChannelSettings Right { get; set; }
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
