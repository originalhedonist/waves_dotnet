using System;
using System.Collections.Concurrent;

namespace wavegenerator
{
    public abstract class TabletopGenerator : FrequencyFunctionWaveFile
    {
        public TabletopGenerator(double baseFrequency, int sectionLengthSeconds, int numSections, short channels) : base(sectionLengthSeconds * numSections, channels)
        {
            this.baseFrequency = baseFrequency;
            this.sectionLengthSeconds = sectionLengthSeconds;
            this.numSections = numSections;
        }

        private readonly double baseFrequency;
        private readonly int sectionLengthSeconds;
        private readonly int numSections;

        protected struct Params
        {
            public double TopLength;
            public double RampLength;
            public double TopFrequency;
            public bool RampsUseSin2;
        }
        /*            TopLength    RampLength
         *          <--------------><->
         * f|       ________________
         *  |      /TopFrequency    \   PrefixLength (same both sides)
         *  |     /                  \ <-->
         *  |____/                    \____
         *  |baseFrequency
         * ________________________________ts
         *
         * the TopFrequency doesn't necessarily have to be greater than baseFrequency - but it must be >0.
         */

        protected abstract Params CreateParams(int section);
        //should only be called once, and cached.
        // It might (and very probably will) do 'Random' operations, so want the same one for the whole segment!

        private readonly ConcurrentDictionary<int, Params> paramsCache = new ConcurrentDictionary<int, Params>();
        protected override double Frequency(double t, int n, int channel)
        {
            int section = (int)(((float)n / N) * numSections);
            var p = paramsCache.GetOrAdd(section, CreateParams);
            ValidateParams(p);
            double ts = t - (section * sectionLengthSeconds); //time through the current section
            double prefixLength = (sectionLengthSeconds - p.TopLength - 2 * p.RampLength) / 2; //length of the bit at base frequency before the first ramp
            double df = p.TopFrequency - baseFrequency;
            if (ts < prefixLength)
            {
                //before the first ramp
                return baseFrequency;
            }
            else if (ts < prefixLength + p.RampLength)
            {
                // on the first ('up') ramp
                double timeAlongRamp = ts - prefixLength;
                double proportionAlongRamp = timeAlongRamp / p.RampLength;
                double proportionUpRamp = p.RampsUseSin2 ? Math.Pow(Math.Sin(proportionAlongRamp * Math.PI / 2), 2) : proportionAlongRamp;
                return baseFrequency + proportionUpRamp * df;
            }
            else if (ts <= prefixLength + p.RampLength + p.TopLength)
            {
                // on the tabletop
                return p.TopFrequency;
            }
            else if (ts <= prefixLength + 2 * p.RampLength + p.TopLength)
            {
                //on the second ('down') ramp
                double timeAlongRamp = ts - prefixLength - p.RampLength - p.TopLength;
                double proportionAlongRamp = timeAlongRamp / p.RampLength;
                double proportionUpRamp = p.RampsUseSin2 ? Math.Pow(Math.Sin(proportionAlongRamp * Math.PI / 2), 2) : proportionAlongRamp;
                return baseFrequency + (1 - proportionUpRamp) * df;
            }
            else
            {
                //after the second ramp
                return baseFrequency;
            }
        }

        private void ValidateParams(Params p)
        {
            if (p.TopFrequency <= 0) throw new InvalidOperationException("TopFrequency must be > 0");
            if (p.TopLength < 0) throw new InvalidOperationException("TopLength must be >= 0");
            if (p.RampLength < 0) throw new InvalidOperationException("RampLength must be >= 0");
            if (p.TopLength + 2 * p.RampLength > sectionLengthSeconds) throw new InvalidOperationException("TopLength + 2*RampLength must be <= sectionLengthSeconds");
        }
    }
}
