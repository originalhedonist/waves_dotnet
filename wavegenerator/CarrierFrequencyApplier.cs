using System;

namespace wavegenerator
{
    public class CarrierFrequencyApplier : FrequencyFunctionWaveFile
    {
        private readonly WaveFile pattern;
        private readonly double carrierFrequencyLeft;
        private readonly double carrierFrequencyRight;

        public CarrierFrequencyApplier(WaveFile pattern, double carrierFrequencyLeft, double carrierFrequencyRight) :
            base(pattern.LengthSeconds, pattern.Channels, phaseShiftChannels: Settings.Instance.PhaseShiftCarrier)
        {
            this.pattern = pattern;
            this.carrierFrequencyLeft = carrierFrequencyLeft;
            this.carrierFrequencyRight = carrierFrequencyRight;
        }

        public override double Amplitude(double t, int n, int channel)
        {
            double carrierAmplitude = base.Amplitude(t, n, channel);
            double patternAmplitude = Math.Abs(pattern.Amplitude(t, n, channel));
            return carrierAmplitude * patternAmplitude;
        }

        protected override double Frequency(double t, int n, int channel)
        {
            return channel == 0 ? carrierFrequencyLeft : carrierFrequencyRight;
        }
    }
}
