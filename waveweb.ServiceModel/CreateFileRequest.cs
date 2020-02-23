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
}
