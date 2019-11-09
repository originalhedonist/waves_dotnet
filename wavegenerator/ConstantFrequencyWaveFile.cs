using System;
namespace wavegenerator
{
    public class ConstantFrequencyWaveFile : WaveFile
    {
        private readonly double frequency;

        public ConstantFrequencyWaveFile(double frequency, int lengthSeconds, short channels) : base(lengthSeconds, channels)
        {
            this.frequency = frequency;
        }

        protected override double Amplitude(double t, int n, int channel)
        {
            return Math.Sin(2 * Math.PI * frequency * t);
        }
    }

}
