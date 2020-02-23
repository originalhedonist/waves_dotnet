using ServiceStack;

namespace waveweb.ServiceModel
{
    [Route("/testpulsewaveform")]
    public class TestPulseWaveformRequest : IReturn<TestPulseWaveformResponse>
    {
        public string WaveformExpression { get; set; }
        public int SectionLengthSeconds { get; set; }
    }
}
