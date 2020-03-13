using System;
using System.IO;
using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class WaveStreamV2 : IAmplitude, IWaveStream
    {
        private readonly SettingsV2 settings;

        public WaveStreamV2(SettingsV2 settings)
        {
            this.settings = settings;
        }
        public Task<double> Amplitude(double t, int n, int channel)
        {
            throw new NotImplementedException();
        }

        public Task Write(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task Write(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}