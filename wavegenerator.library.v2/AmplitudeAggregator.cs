using System.Threading.Tasks;

namespace wavegenerator.library
{
    public class AmplitudeAggregator : IAmplitude
    {
        private readonly IAmplitude[] components;

        public AmplitudeAggregator(IAmplitude[] components)
        {
            this.components = components;
        }
        public async Task<double> Amplitude(double t, int n, int channel)
        {
            double a = 1;
            foreach(var c in components)
            {
                a *= await c.Amplitude(t, n, channel);
            }
            return a;
        }
    }
}