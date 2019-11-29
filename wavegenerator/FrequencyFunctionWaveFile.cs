﻿using System;

namespace wavegenerator
{
    public abstract class FrequencyFunctionWaveFile : WaveFile
    {
        protected readonly double[] x;
        protected readonly double?[] lastAmplitude;
        protected readonly double?[] lastAmplitudeGradient;
        protected readonly double?[] lastPeak; // the t of the last peak (either last top, or last bottom)
        protected readonly double?[] lastPeakAmplitude; //whether the last 'peak' was top or bottom
        protected readonly double[] dt;
        protected readonly int[] dn;
        protected readonly double[] lastt;
        protected readonly int?[] lastn;
        protected readonly bool phaseShiftChannels = false;
        public FrequencyFunctionWaveFile(bool phaseShiftChannels)
        {
            x = new double[Channels];
            lastAmplitude = new double?[Channels];
            lastAmplitudeGradient = new double?[Channels];
            lastPeak = new double?[Channels];
            lastPeakAmplitude = new double?[Channels];
            dt = new double[Channels];
            lastt = new double[Channels];
            dn = new int[Channels];
            lastn = new int?[Channels];
            this.phaseShiftChannels = phaseShiftChannels;
        }

        protected abstract double Frequency(double t, int n, int channel);

        protected virtual double PeakLength(double t, int n, int channel) => 0.0;
        protected virtual double TroughLength(double t, int n, int channel) => 0.0;
        public override double Amplitude(double t, int n, int channel)
        {
            double amplitude;
            var f = Frequency(t, n, channel);
            var dx = 2 * Math.PI * f / Settings.SamplingFrequency;
            x[channel] += dx;
            amplitude = (phaseShiftChannels && channel == 1) ? Math.Cos(x[channel]) : Math.Sin(x[channel]);
            double? amplitudeGradient = null;
            if (lastAmplitude[channel].HasValue) amplitudeGradient = amplitude - lastAmplitude[channel].Value;

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
            return amplitude;
        }


    }
}
