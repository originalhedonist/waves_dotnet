using System;
namespace wavegenerator
{
    public abstract class FrequencyFunctionWaveFile : WaveFile
    {
        private readonly double[] x;
        private readonly bool phaseShiftChannels = false;
        public FrequencyFunctionWaveFile(bool phaseShiftChannels)
        {
            x = new double[Channels];
            this.phaseShiftChannels = phaseShiftChannels;
        }

        protected abstract double Frequency(double t, int n, int channel);

        public override double Amplitude(double t, int n, int channel)
        {
            var f = Frequency(t, n, channel);
            var dx = 2 * Math.PI * f / SamplingFrequency;
            x[channel] += dx;
            return (phaseShiftChannels && channel == 1) ? Math.Cos(x[channel]) : Math.Sin(x[channel]);
        }
    }
}
