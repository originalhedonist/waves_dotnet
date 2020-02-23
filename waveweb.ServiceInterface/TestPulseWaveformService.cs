using ServiceStack;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class TestPulseWaveformService : Service
    {
        public TestPulseWaveformResponse Post(TestPulseWaveformRequest testPulseWaveformRequest)
        {
            return new TestPulseWaveformResponse
            {
                Success = false,
                ErrorMessage = "The formula is not deemed acceptable to mine eye"
            };
        }
    }
}
