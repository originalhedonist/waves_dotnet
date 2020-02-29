using System.Threading.Tasks;
using wavegenerator;
using wavegenerator.library;

namespace waveweb.ServiceInterface
{
    public class WaveformDemoWaveFile : FrequencyFunctionWaveFile
    {
        public WaveformDemoWaveFile() : base(numberOfChannels: 1, phaseShiftChannels: false) { }
        protected override Task<double> Frequency(double t, int n, int channel)
        {
            throw new System.NotImplementedException();
        }
    }

}
