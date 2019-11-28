using System;
using System.Collections.Concurrent;

namespace wavegenerator
{
    public abstract class FrequencyFunctionWaveFile : WaveFile
    {
        private readonly double[] x;
        private readonly double?[] lastAmplitude;
        private readonly double?[] lastAmplitudeGradient;
        private readonly double?[] lastPeak; // the t of the last peak (either last top, or last bottom)
        private readonly double?[] lastPeakAmplitude; //whether the last 'peak' was top or bottom

        private readonly bool phaseShiftChannels = false;
        public FrequencyFunctionWaveFile(bool phaseShiftChannels)
        {
            x = new double[Channels];
            lastAmplitude = new double?[Channels];
            lastAmplitudeGradient = new double?[Channels];
            lastPeak = new double?[Channels];
            lastPeakAmplitude = new double?[Channels];
            this.phaseShiftChannels = phaseShiftChannels;
        }

        protected abstract double Frequency(double t, int n, int channel);

        protected virtual double PeakLength(double t, int n, int channel) => 0.0;
        protected virtual double TroughLength(double t, int n, int channel) => 0.0;

        public override double Amplitude(double t, int n, int channel)
        {
            double amplitude;
            //Due to the way wetness inverts, 'peaks' come out as 'troughs' and vice versa. Like looking at yourself in a mirror.
            var invertedTroughLength = PeakLength(t, n, channel);
            var invertedPeakLength = TroughLength(t, n, channel);
            if (lastPeakAmplitude[channel] > 0 && lastPeak[channel] != null && t - lastPeak[channel] <= invertedPeakLength)
            {
                amplitude = lastPeakAmplitude[channel].Value; //we don't increment x, because time freezes at the top!
            }
            else if (lastPeakAmplitude[channel] < 0 && lastPeak[channel] != null && t - lastPeak[channel] <= invertedTroughLength)
            {
                amplitude = lastPeakAmplitude[channel].Value; //we don't increment x, because time freezes at the top!
            }
            else
            {
                var f = Frequency(t, n, channel);
                var dx = 2 * Math.PI * f / SamplingFrequency;
                x[channel] += dx;
                amplitude = (phaseShiftChannels && channel == 1) ? Math.Cos(x[channel]) : Math.Sin(x[channel]);
                double? amplitudeGradient = null;
                if (lastAmplitude[channel].HasValue) amplitudeGradient = amplitude - lastAmplitude[channel].Value ;

                bool justReachedPeak = lastAmplitudeGradient[channel].HasValue && amplitudeGradient <= 0 && lastAmplitudeGradient[channel].Value > 0;
                bool justReachedTrough = lastAmplitudeGradient[channel].HasValue && amplitudeGradient >= 0 && lastAmplitudeGradient[channel] < 0;
                if (justReachedPeak)
                {
                    lastPeak[channel] = t;
                    lastPeakAmplitude[channel] = 1;
                }
                else if (justReachedTrough)
                {
                    lastPeak[channel] = t;
                    lastPeakAmplitude[channel] = -1;
                }

                lastAmplitude[channel] = amplitude;
                lastAmplitudeGradient[channel] = amplitudeGradient;
            }
            return amplitude;
        }
    }
}
