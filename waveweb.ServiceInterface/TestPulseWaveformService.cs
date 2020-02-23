using ServiceStack;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class TestPulseWaveformService : Service
    {
        public TestPulseWaveformResponse Post(TestPulseWaveformRequest testPulseWaveformRequest)
        {
            return new TestPulseWaveformResponse();
        }
    }
}
