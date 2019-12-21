using System;
using System.Collections.Concurrent;
using System.IO;

namespace wavegenerator
{
    public struct TabletopParams
    {
        public double TopLength;
        public double RampLength;
        public bool RampsUseSin2;
    }

    public abstract class TabletopGenerator : FrequencyFunctionWaveFile
    {
        private readonly BufferedStream debugStream;
        private readonly StreamWriter debugWriter;
        protected readonly double?[] lastAmplitude;
        protected readonly double?[] lastPeak; // the t of the last peak (either last top, or last bottom)
        protected readonly double?[] lastPeakAmplitude; //whether the last 'peak' was top or bottom
        protected readonly bool[] inPeak;
        protected readonly bool[] inTrough;

        public TabletopGenerator(double baseFrequency, double sectionLengthSeconds, int numSections) : base(phaseShiftChannels: Settings.Instance.PhaseShiftPulses)
        {
            this.baseFrequency = baseFrequency;
            this.sectionLengthSeconds = sectionLengthSeconds;
            this.numSections = numSections;
            lastAmplitude = new double?[Channels];
            lastPeak = new double?[Channels];
            lastPeakAmplitude = new double?[Channels];
            inPeak = new bool[Channels];
            inTrough = new bool[Channels];

            debugStream = new BufferedStream(new FileStream("frequencydebug.csv", FileMode.Create, FileAccess.Write));
            debugWriter = new StreamWriter(debugStream);


        }

        protected readonly double baseFrequency;
        protected readonly double sectionLengthSeconds;
        protected readonly int numSections;

        protected abstract double CreateTopFrequency(int section);
        protected abstract TabletopParams CreateFeatureParamsForSection(int section);
        protected TabletopParams GetTabletopParamsBySection(int section) => paramsCache.GetOrAdd(section, s =>
        {
            var p = CreateFeatureParamsForSection(section);
            ValidateParams(p);
            return p;
        });

        //should only be called once, and cached.
        // It might (and very probably will) do 'Random' operations, so want the same one for the whole segment!

        private readonly ConcurrentDictionary<int, TabletopParams> paramsCache = new ConcurrentDictionary<int, TabletopParams>();
        private readonly ConcurrentDictionary<int, double> topFrequencyCache = new ConcurrentDictionary<int, double>();
        protected int Section(int n) => (int)(n / (sectionLengthSeconds * Settings.SamplingFrequency));
        protected override double Frequency(double t, int n, int channel)
        {
            int section = Section(n);
            var p = GetTabletopParamsBySection(section);
            
            var topFrequency = topFrequencyCache.GetOrAdd(section, CreateTopFrequency);
            if (topFrequency <= 0) throw new InvalidOperationException("TopFrequency must be >= 0");

            double ts = t - (section * sectionLengthSeconds); //time through the current section

            double frequency = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, baseFrequency, topFrequency, p);

            return frequency;
        }

        private void ValidateParams(TabletopParams p)
        {
            if (p.TopLength < 0) throw new InvalidOperationException("TopLength must be >= 0");
            if (p.RampLength < 0) throw new InvalidOperationException("RampLength must be >= 0");
            if (p.TopLength + 2 * p.RampLength > sectionLengthSeconds) throw new InvalidOperationException("TopLength + 2*RampLength must be <= sectionLengthSeconds");
        }


        public override double Amplitude(double t, int n, int channel)
        {
            //if (n == lastn[channel] && lastAmplitude[channel].HasValue)
            //    return lastAmplitude[channel].Value;

            double amplitude;
            var f = Frequency(t, n, channel);

            if (inPeak[channel] && inTrough[channel]) throw new InvalidOperationException($"Sanity check failed.");

            //if (inPeak[channel]) f /= 5;
            if (inTrough[channel]) f /= 5;


            var dx = 2 * Math.PI * f / Settings.SamplingFrequency;
            x[channel] += dx;
            amplitude = (phaseShiftChannels && channel == 1) ? Math.Cos(x[channel]) : Math.Sin(x[channel]);

            bool justReachedPeak = lastAmplitude[channel].HasValue && amplitude >= 0.8 && lastAmplitude[channel].Value < 0.8;
            bool justReachedTrough = lastAmplitude[channel].HasValue && amplitude <= -0.8 && lastAmplitude[channel] > -0.8;
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
                var justLeftPeak = lastAmplitude[channel].HasValue && amplitude <= 0.8 && lastAmplitude[channel] > 0.8;
                if (justLeftPeak) inPeak[channel] = false;
            }

            if (inTrough[channel])
            {
                var justLeftTrough = lastAmplitude[channel].HasValue && amplitude >= -0.8 && lastAmplitude[channel] <  -0.8;
                if (justLeftTrough) inTrough[channel] = false;
            }

            lastAmplitude[channel] = amplitude;


            return amplitude;
        }


    }
}
