using System;

namespace wavegenerator
{
    public class CarrierFrequencyApplier : FrequencyFunctionWaveFile
    {
        private readonly WaveFile pattern;
        private readonly double carrierFrequencyLeftStart;
        private readonly double carrierFrequencyLeftEnd;
        private readonly double carrierFrequencyRightStart;
        private readonly double carrierFrequencyRightEnd;

        public CarrierFrequencyApplier(WaveFile pattern, 
            double carrierFrequencyLeftStart, double carrierFrequencyLeftEnd,
            double carrierFrequencyRightStart, double carrierFrequencyRightEnd) :
            base(pattern.LengthSeconds, pattern.Channels, phaseShiftChannels: true)
        {
            this.pattern = pattern;
            this.carrierFrequencyLeftStart = carrierFrequencyLeftStart;
            this.carrierFrequencyLeftEnd = carrierFrequencyLeftEnd;
            this.carrierFrequencyRightStart = carrierFrequencyRightStart;
            this.carrierFrequencyRightEnd = carrierFrequencyRightEnd;
        }

        public override double Amplitude(double t, int n, int channel)
        {
            double carrierAmplitude = base.Amplitude(t, n, channel);
            double patternAmplitude = Math.Abs(pattern.Amplitude(t, n, channel));
            return carrierAmplitude * patternAmplitude;
        }

        protected override double Frequency(double t, int n, int channel)
        {
            var carrierFrequencies = channel == 0 ?
                new { Start = carrierFrequencyLeftStart, End = carrierFrequencyLeftEnd } :
                new { Start = carrierFrequencyRightStart, End = carrierFrequencyRightEnd };
            double progression = (double)n / N;
            double carrierFrequency = carrierFrequencies.Start + (carrierFrequencies.End - carrierFrequencies.Start) * progression;
            return carrierFrequency;
        }
    }
}
