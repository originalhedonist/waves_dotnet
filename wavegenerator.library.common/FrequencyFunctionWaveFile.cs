using System;
using System.Threading.Tasks;

namespace wavegenerator.library
{
    public abstract class FrequencyFunctionWaveFile : IAmplitude
    {
        protected readonly double[] X;
        protected readonly bool PhaseShiftChannels;
        private readonly int samplingFrequency;
        private readonly int?[] lastNCheck;
        private readonly double[] lastValue; //allow it to 'cache' the last value. we don't need to cache all values, it would take too much memory
        protected FrequencyFunctionWaveFile(int numberOfChannels, bool phaseShiftChannels, int samplingFrequency)
        {
            X = new double[numberOfChannels];
            lastValue = new double[numberOfChannels];
            lastNCheck = new int?[numberOfChannels];
            PhaseShiftChannels = phaseShiftChannels;
            this.samplingFrequency = samplingFrequency;
        }

        protected abstract Task<double> Frequency(double t, int n, int channel);

        protected virtual Task<double> GetWaveformSample(double[] x, bool phaseShiftChannels, double t, int n, int channel)
        {
            return Task.FromResult(phaseShiftChannels && channel == 1 ? Math.Cos(x[channel]) : Math.Sin(x[channel]));
        }

        public virtual async Task<double> Amplitude(double t, int n, int channel)
        {
            if (lastNCheck[channel] == n) return lastValue[channel]; // cache hit (don't increment x)

            if(lastNCheck[channel] != null && lastNCheck[channel] !=  n-1)
            {
                throw new InvalidOperationException($"Frequency function wave file is stateful and has been calculated in the wrong order");
            }

            lastNCheck[channel] = n;

            var f = await Frequency(t, n, channel);

            var dx = 2 * Math.PI * f / samplingFrequency;
            X[channel] += dx;
            var amplitude = await GetWaveformSample(X, PhaseShiftChannels, t, n, channel);
            lastValue[channel] = amplitude;
            return amplitude;
        }
    }
}