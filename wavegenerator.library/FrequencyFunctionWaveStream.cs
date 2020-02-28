using System;
using System.IO;
using System.Threading.Tasks;

namespace wavegenerator
{
    public abstract class FrequencyFunctionWaveFile : IAmplitude
    {
        protected readonly double[] x;
        protected readonly bool phaseShiftChannels = false;
        public FrequencyFunctionWaveFile(int numberOfChannels, bool phaseShiftChannels)
        {
            x = new double[numberOfChannels];
            this.phaseShiftChannels = phaseShiftChannels;

        }

        protected abstract Task<double> Frequency(double t, int n, int channel);

        protected virtual double PeakLength(double t, int n, int channel) => 0.0;
        protected virtual double TroughLength(double t, int n, int channel) => 0.0;

        protected virtual Task<double> GetWaveformSample(double[] x, bool phaseShiftChannels, int channel) =>
            Task.FromResult((phaseShiftChannels && channel == 1) ? Math.Cos(x[channel]) : Math.Sin(x[channel]));

        public virtual async Task<double> Amplitude(double t, int n, int channel)
        {
            double amplitude;
            var f = await Frequency(t, n, channel);

            var dx = 2 * Math.PI * f / Settings.SamplingFrequency;
            x[channel] += dx;
            amplitude = await GetWaveformSample(x, phaseShiftChannels, channel);
            return amplitude;
        }

    }
}
