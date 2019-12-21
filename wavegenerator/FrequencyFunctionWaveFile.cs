using System;
using System.IO;

namespace wavegenerator
{
    public abstract class FrequencyFunctionWaveFile : WaveFile
    {
        protected readonly double[] x;
        protected readonly double?[] lastAmplitude;
        protected readonly double?[] lastAmplitudeGradient;
        protected readonly double?[] lastPeak; // the t of the last peak (either last top, or last bottom)
        protected readonly double?[] lastPeakAmplitude; //whether the last 'peak' was top or bottom
        protected readonly bool phaseShiftChannels = false;
        protected readonly bool[] inPeak;
        protected readonly bool[] inTrough;
        public FrequencyFunctionWaveFile(bool phaseShiftChannels)
        {
            x = new double[Channels];
            lastAmplitude = new double?[Channels];
            lastAmplitudeGradient = new double?[Channels];
            lastPeak = new double?[Channels];
            lastPeakAmplitude = new double?[Channels];
            inPeak = new bool[Channels];
            inTrough = new bool[Channels];

            this.phaseShiftChannels = phaseShiftChannels;

        }

        protected abstract double Frequency(double t, int n, int channel);

        protected virtual double PeakLength(double t, int n, int channel) => 0.0;
        protected virtual double TroughLength(double t, int n, int channel) => 0.0;



        public override double Amplitude(double t, int n, int channel)
        {
            //if (n == lastn[channel] && lastAmplitude[channel].HasValue)
            //    return lastAmplitude[channel].Value;

            double amplitude;
            var f = Frequency(t, n, channel);

            if (inPeak[channel] && inTrough[channel]) throw new InvalidOperationException($"Sanity check failed.");

            //if (inPeak[channel]) f /= 5;
            //if (inTrough[channel]) f /= 2;


            var dx = 2 * Math.PI * f / Settings.SamplingFrequency;
            x[channel] += dx;
            amplitude = (phaseShiftChannels && channel == 1) ? Math.Cos(x[channel]) : Math.Sin(x[channel]);
            double? amplitudeGradient = null;
            if (lastAmplitude[channel].HasValue) amplitudeGradient = (amplitude - lastAmplitude[channel].Value) * Settings.SamplingFrequency;

            bool justReachedPeak = lastAmplitudeGradient[channel].HasValue && amplitudeGradient <= 0.5 && lastAmplitudeGradient[channel].Value > 0.5;
            bool justReachedTrough = lastAmplitudeGradient[channel].HasValue && amplitudeGradient >= -0.5 && lastAmplitudeGradient[channel] < -0.5;
            if (justReachedPeak && justReachedTrough) throw new InvalidOperationException($"Sanity check failed.");

            if (justReachedPeak)
            {
                if (inPeak[channel]) throw new InvalidOperationException($"Just reached a peak when already in one");
                inPeak[channel] = true;
            }

            if (justReachedTrough)
            {
                if (inTrough[channel]) throw new InvalidOperationException($"Just reached a trough when already in one");
                inTrough[channel] = true;
            }

            if (inPeak[channel])
            {
                var justLeftPeak = lastAmplitudeGradient[channel].HasValue && amplitudeGradient <= -0.5 && lastAmplitudeGradient[channel] > -0.5;
                if (justLeftPeak) inPeak[channel] = false;
            }

            if (inTrough[channel])
            {
                var justLeftTrough = lastAmplitudeGradient[channel].HasValue && amplitudeGradient >= 0.5 && lastAmplitudeGradient[channel] < 0.5;
                if (justLeftTrough) inTrough[channel] = false;
            }

            lastAmplitude[channel] = amplitude;
            lastAmplitudeGradient[channel] = amplitudeGradient;


            return amplitude;
        }

    }
}
