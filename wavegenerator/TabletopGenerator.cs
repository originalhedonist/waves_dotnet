using System;
using System.Collections.Concurrent;

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
        public TabletopGenerator(double baseFrequency, double sectionLengthSeconds, int numSections, short channels) : 
            base(sectionLengthSeconds * numSections, channels, phaseShiftChannels: Settings.Instance.PhaseShiftPulses)
        {
            this.baseFrequency = baseFrequency;
            this.sectionLengthSeconds = sectionLengthSeconds;
            this.numSections = numSections;
        }

        protected readonly double baseFrequency;
        protected readonly double sectionLengthSeconds;
        protected readonly int numSections;

        protected abstract double CreateTopFrequency(SectionId section);
        protected abstract TabletopParams CreateFeatureParamsForSection(SectionId section);
        protected TabletopParams GetTabletopParamsBySection(SectionId section) => paramsCache.GetOrAdd(section, s =>
        {
            var p = CreateFeatureParamsForSection(section);
            ValidateParams(p);
            return p;
        });

        //should only be called once, and cached.
        // It might (and very probably will) do 'Random' operations, so want the same one for the whole segment!

        private readonly ConcurrentDictionary<SectionId, TabletopParams> paramsCache = new ConcurrentDictionary<SectionId, TabletopParams>();
        private readonly ConcurrentDictionary<SectionId, double> topFrequencyCache = new ConcurrentDictionary<SectionId, double>();
        protected int Section(int n) => (int)(((float)n / N) * numSections);
        protected override double Frequency(double t, int n, int channel)
        {
            int section = Section(n);
            var sectionId = new SectionId(section, channel);
            var p = GetTabletopParamsBySection(sectionId);
            
            var topFrequency = topFrequencyCache.GetOrAdd(sectionId, CreateTopFrequency);
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
    }
}
