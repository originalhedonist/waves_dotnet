using wavegenerator.library;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public interface IWaveformTestPulseGeneratorProvider
    {
        PulseGenerator GetPulseGenerator(TestPulseWaveformRequest testPulseWaveformRequest, GetPulseGeneratorParams parameters);
    }

    public class GetPulseGeneratorParams
    {
        public int SamplingFrequency { get; set; }
        public string ChooseFeature { get; set; }
    }

}