using System;

namespace wavegenerator
{
    public class CarrierFrequencyApplier : WaveFile
    {
        private readonly WaveFile pattern;
        private readonly double carrierFrequency;

        public CarrierFrequencyApplier(WaveFile pattern, double carrierFrequency) : base(pattern.LengthSeconds, pattern.Channels)
        {
            this.pattern = pattern;
            this.carrierFrequency = carrierFrequency;
        }

        public override double Amplitude(double t, int n, int channel)
        {
            double x = 2 * Math.PI * carrierFrequency * t;
            double carrierAmplitude = channel == 0 ? Math.Sin(x) : Math.Cos(x); //this is the amplitude if it were constant.
            double patternAmplitude = Math.Abs(pattern.Amplitude(t, n, channel));
            //return carrierAmplitude * patternAmplitude;

            //the nearer wetness is to 1, the more we disregard the pattern amplitude
            double wetness = pattern.Wetness(t, n);
            double a_maxdry = patternAmplitude * carrierAmplitude;
            double a_maxwet = carrierAmplitude;
            double a = a_maxdry + (a_maxwet - a_maxdry) * wetness;
            return a;
        }
    }
}
