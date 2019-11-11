using System;

namespace wavegenerator
{
    public class CarrierFrequencyApplier : WaveFile
    {
        private readonly WaveFile pattern;
        private readonly double carrierFrequencyRight;
        private readonly double carrierFrequencyLeft;

        public CarrierFrequencyApplier(WaveFile pattern, double carrierFrequencyRight, double carrierFrequencyLeft) : base(pattern.LengthSeconds, pattern.Channels)
        {
            this.pattern = pattern;
            this.carrierFrequencyRight = carrierFrequencyRight;
            this.carrierFrequencyLeft = carrierFrequencyLeft;
        }

        public override double Amplitude(double t, int n, int channel)
        {
            double carrierFrequency = channel == 0 ? carrierFrequencyLeft : carrierFrequencyRight;
            double x = 2 * Math.PI * carrierFrequency * t;
            double carrierAmplitude = channel == 0 ? Math.Sin(x) : Math.Cos(x); //this is the amplitude if it were constant.
            double patternAmplitude = Math.Abs(pattern.Amplitude(t, n, channel));
            return carrierAmplitude * patternAmplitude;
        }
    }
}
