using System;
namespace wavegenerator
{
    public abstract class FrequencyFunctionWaveFile : WaveFile
    {
        private double x;
        private readonly bool phaseShiftChannels = false;
        public FrequencyFunctionWaveFile(int lengthSeconds, short channels, bool phaseShiftChannels) : base(lengthSeconds, channels)
        {
            this.phaseShiftChannels = phaseShiftChannels;
        }

        protected abstract double Frequency(double t, int n, int channel);

        public override double Amplitude(double t, int n, int channel)
        {
            var f = Frequency(t, n, channel);
            var dx = Math.PI * f / SamplingFrequency;
            x += dx;
            return (phaseShiftChannels && channel == 1) ? Math.Cos(x) : Math.Sin(x);
        }
    }
}
