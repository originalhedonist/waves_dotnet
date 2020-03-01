using System;
using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{
    public abstract class FrequencyFunctionWaveFile : IAmplitude
    {
        protected readonly double[] X;
        protected readonly bool PhaseShiftChannels;
        private readonly int samplingFrequency;

        protected FrequencyFunctionWaveFile(int numberOfChannels, bool phaseShiftChannels, int samplingFrequency)
        {
            X = new double[numberOfChannels];
            PhaseShiftChannels = phaseShiftChannels;
            this.samplingFrequency = samplingFrequency;
        }

        protected abstract Task<double> Frequency(double t, int n, int channel);

        protected virtual Task<double> GetWaveformSample(double[] x, bool phaseShiftChannels, int channel)
        {
            return Task.FromResult(phaseShiftChannels && channel == 1 ? Math.Cos(x[channel]) : Math.Sin(x[channel]));
        }

        public virtual async Task<double> Amplitude(double t, int n, int channel)
        {
            var f = await Frequency(t, n, channel);

            var dx = 2 * Math.PI * f / samplingFrequency;
            X[channel] += dx;
            var amplitude = await GetWaveformSample(X, PhaseShiftChannels, channel);
            return amplitude;
        }
    }
}