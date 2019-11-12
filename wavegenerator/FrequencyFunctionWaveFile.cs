using System;
namespace wavegenerator
{
    public abstract class FrequencyFunctionWaveFile : WaveFile
    {
        private double x;
        public FrequencyFunctionWaveFile(int lengthSeconds, short channels) : base(lengthSeconds, channels)
        {
        }

        protected abstract double Frequency(double t, int n, int channel);

        public override double Amplitude(double t, int n, int channel)
        {
            var f = Frequency(t, n, channel);
            var dx = Math.PI * f / SamplingFrequency;
            x += dx;
            return Math.Sin(x);
        }
    }
}
