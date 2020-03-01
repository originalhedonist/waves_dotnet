using wavegenerator.library;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public interface IWaveformTestPulseGeneratorProvider
    {
        PulseGenerator GetPulseGenerator(TestPulseWaveformRequest testPulseWaveformRequest);
    }
}