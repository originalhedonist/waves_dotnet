using System.Threading.Tasks;

namespace wavegenerator.library
{
    public interface IAmplitude
    {
        Task<double> Amplitude(double t, int n, int channel);
    }
}