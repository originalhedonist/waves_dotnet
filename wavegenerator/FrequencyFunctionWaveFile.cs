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

        protected virtual double PeakTopLength(double t, int n, int channel) => 0.0; //TODO: default should be zero when not testing

        public override double Amplitude(double t, int n, int channel)
        {
            double amplitude;
            var peakTopLength = PeakTopLength(t, n, channel);
            if (lastPeak[channel] != null && t - lastPeak[channel] <= peakTopLength)
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

                bool justReachedTop = lastAmplitudeGradient[channel].HasValue && amplitudeGradient <= 0 && lastAmplitudeGradient[channel].Value > 0;
                bool justReachedBottom = lastAmplitudeGradient[channel].HasValue && amplitudeGradient >= 0 && lastAmplitudeGradient[channel] < 0;
                if (justReachedTop)
                {
                    //lastPeak[channel] = t;
                    //lastPeakAmplitude[channel] = 1;
                }
                else if (justReachedBottom)
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
