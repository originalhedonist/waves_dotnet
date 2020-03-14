using System.IO;
using System.Threading.Tasks;

namespace wavegenerator.library
{
    public interface IWaveStream
    {
        Task<double> Amplitude(double t, int n, int channel);
        Task Write(Stream stream);
        Task Write(string filePath);
    }
}