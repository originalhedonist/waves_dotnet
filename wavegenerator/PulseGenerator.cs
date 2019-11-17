using System;
using System.IO;
using System.Collections.Concurrent;

namespace wavegenerator
{
    public class PulseGenerator : TabletopGenerator
    { 
        public PulseGenerator(string compositionName, int sectionLengthSeconds, int numSections, short channels) : base(Constants.BasePulseFrequency, sectionLengthSeconds, numSections, channels)
        {
            this.compositionName = compositionName;
        }

        private readonly ConcurrentDictionary<int, bool> isBreakCache = new ConcurrentDictionary<int, bool>();
        private readonly ConcurrentDictionary<int, TabletopParams> breakParamsCache = new ConcurrentDictionary<int, TabletopParams>();

        private bool IsBreak(int section) => isBreakCache.GetOrAdd(section, s =>
        {
            if (s * sectionLengthSeconds < Constants.MinTimeBeforeBreak) return false;
            var retval = Randomizer.Probability(Constants.ChanceOfBreak, false); //10% chance of being a break after ten mins
            if (retval) File.AppendAllLines($"{compositionName}.report.txt", new[] { $"Section {s} is a break" });
            return retval;
        });

        public TabletopParams GetBreakParams(int section) => breakParamsCache.GetOrAdd(section, s =>
        {
            var breakLength = Randomizer.GetRandom() * (Constants.MaxBreakLength - Constants.MinBreakLength) + Constants.MinBreakLength;
            var p = new TabletopParams
            {
                RampLength = Constants.BreakRampLength,
                TopLength = breakLength,
                RampsUseSin2 = true
            };
            return p;
        });

        public override double Amplitude(double t, int n, int channel)
        {
            var section = Section(n);
            double baseA = base.Amplitude(t, n, channel);// must always calculate it, even if we don't use it - it might (does) increment something important

            //first apply wetness,
            double wetness = Wetness(t, n);
            double apos = (baseA + 1) / 2; //base amplitude, always positive - but with proper curves unlike abs
            double dryness = 1 - wetness;
            double a = 1 - dryness * apos;

            //then apply break
            double a_res;
            if (IsBreak(section))
            {
                double ts = t - (section * sectionLengthSeconds); //time through the current section
                var p = GetBreakParams(section);
                a_res = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, a, 0, p);
            }
            else
            {
                a_res = a;
            }
            return a_res;
        }

        protected override TabletopParams CreateTabletopParamsForSection(int section)
        {
            if (IsBreak(section)) return new TabletopParams { RampLength = 0, TopLength = 0, RampsUseSin2 = false };//don't apply a table top if we're on a break

            //first decide if it has a tabletop at all.
            //the chance of it being something at all rises from 0% to 100%.
            double progression = ((float)section + 1) / numSections; // <= 1
            var isTabletop = Randomizer.Probability(Math.Pow(progression, Constants.TabletopChanceRiseSlownessFactor), true);
            if (isTabletop)
            {
                //if it's a tabletop:
                double topLength = 
                    Randomizer.GetRandom() * 
                    Math.Pow(progression, Constants.TabletopLengthRiseSlownessFactor) *
                    (Constants.MaxTabletopLength - Constants.MinTabletopLength) + Constants.MinTabletopLength;
                double maxRampLength = (sectionLengthSeconds - topLength) / 2;
                if (Constants.MinRampLength > maxRampLength) throw new InvalidOperationException($"MinRampLength must be <= maxRampLength. MinTabletopLength could be too high.");

                // could feasibly be MinRampLength at the start of the track. Desirable? Yes, because other parameters constrain the dramaticness at the start.
                double rampLength = Constants.MinRampLength + Randomizer.GetRandom() * (maxRampLength - Constants.MinRampLength);
                var result = new TabletopParams
                {
                    RampLength = rampLength,
                    TopLength = topLength,
                    RampsUseSin2 = true
                };
                File.AppendAllLines($"{compositionName}.report.txt", new[] { $"Section {section} is a tabletop with length {result.TopLength}, rampLength = {result.RampLength}" });
                return result;
            }
            else
            {
                File.AppendAllLines($"{compositionName}.report.txt", new[] { $"Section {section} is not a tabletop" });
                var result = new TabletopParams();
                return result;
            }
        }

        protected override double CreateTopFrequency(int section)
        {
            double progression = ((float)section) / numSections; // <= 1
            //20% of being a fall, 80% chance a rise
            var isRise = Randomizer.Probability(Constants.ChanceOfRise, true);
            double frequencyLimit = isRise ? Constants.MaxPulseFrequency : Constants.MinPulseFrequency;
            double frequencyChangeLimit = frequencyLimit - baseFrequency;
            double topFrequency = baseFrequency + Randomizer.GetRandom() * frequencyChangeLimit * Math.Pow(progression, Constants.TabletopFrequencyRiseSlownessFactor);
            if (topFrequency <= 0)
                throw new InvalidOperationException("TopFrequency must be > 0");

            File.AppendAllLines($"{compositionName}.report.txt", new[] { $"Section {section} (at {TimeSpan.FromSeconds(section * sectionLengthSeconds)} using top frequency of {topFrequency}"});
            return topFrequency;
        }

        private readonly ConcurrentDictionary<int, double> wetnessCache = new ConcurrentDictionary<int, double>();
        private readonly string compositionName;

        private double Wetness(double t, int n)
        {
            // rise in a sin^2 fashion from MinWetness to MaxWetness
            int section = Section(n);
            double ts = t - (section * sectionLengthSeconds); //time through the current section

            double maxWetnessForSection = wetnessCache.GetOrAdd(section, s =>
            {
                double progression = ((float)s + 1) / numSections; // <= 1
                double maxWetness = Constants.MinWetness + Math.Pow(progression, Constants.WetnessRiseSlownessFactor) * Randomizer.GetRandom() * (Constants.MaxWetness - Constants.MinWetness);
                File.AppendAllLines($"{compositionName}.report.txt", new[] { $"The max wetness for section {s} is {maxWetness}" });
                return maxWetness;
            });

            double wetness;
            if (Constants.LinkWetnessToTabletop)
            {
                var p = GetTabletopParamsBySection(section);
                wetness = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, Constants.MinWetness, maxWetnessForSection, p);
            }
            else
            {
                double ps = ts / sectionLengthSeconds; // progression through section
                double x = ps * Math.PI;// max wetness in the middle (x = pi/2)
                wetness = Constants.MinWetness +
                    Math.Pow(Math.Sin(x), 2) * (maxWetnessForSection - Constants.MinWetness);
            }
            return wetness;
        }
    }
}
