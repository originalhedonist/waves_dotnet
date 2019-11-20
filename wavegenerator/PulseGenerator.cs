using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace wavegenerator
{
    public class PulseGenerator : TabletopGenerator
    {
        private readonly int[] sectionsThatAreBreaks;
        public PulseGenerator(string compositionName, double sectionLengthSeconds, int numSections, short channels) : base(Settings.Instance.PulseFrequency.Quiescent, sectionLengthSeconds, numSections, channels)
        {
            this.compositionName = compositionName;
            this.sectionsThatAreBreaks = MakeBreaks(numSections).ToArray();
        }

        private static IEnumerable<int> MakeBreaks(int numSections)
        {
            int? lastBreakSection = null;
            do
            {
                var minTime = lastBreakSection == null ?
                    (Settings.Instance.Breaks.MinTimeSinceStartOfTrack) :
                    ((lastBreakSection.Value + 1) * Settings.Instance.Sections.TotalLength) + Settings.Instance.Breaks.MinTimeBetweenBreaks;
                //add 1 to lastBreakSection because we want the time since the END of that section
                var maxTime = minTime + Settings.Instance.Breaks.MaxTimeBetweenBreaks;
                var breakTime = minTime + (Randomizer.GetRandom() * (maxTime - minTime));
                lastBreakSection = (int)(breakTime / Settings.Instance.Sections.TotalLength);
                yield return lastBreakSection.Value;
            } while (lastBreakSection.Value <= numSections);
        }

        private readonly ConcurrentDictionary<int, bool> isBreakCache = new ConcurrentDictionary<int, bool>();
        private readonly ConcurrentDictionary<int, TabletopParams> breakParamsCache = new ConcurrentDictionary<int, TabletopParams>();

        private bool IsBreak(int section) => sectionsThatAreBreaks.Contains(section);

        public TabletopParams GetBreakParams(int section) => breakParamsCache.GetOrAdd(section, s =>
        {
            var breakLength = Settings.Instance.Breaks.MinLength + (Settings.Instance.Breaks.MaxLength - Settings.Instance.Breaks.MinLength) * Randomizer.GetRandom();
            var p = new TabletopParams
            {
                RampLength = Settings.Instance.Breaks.RampLength.TotalSeconds,
                TopLength = breakLength.TotalSeconds,
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

        protected override TabletopParams CreateFeatureParamsForSection(int section)
        {
            if (IsBreak(section)) return new TabletopParams { RampLength = 0, TopLength = 0, RampsUseSin2 = false };//don't apply a table top if we're on a break

            //first decide if it has a tabletop at all.
            //the chance of it being something at all rises from 0% to 100%.
            double progression = ((float)section + 1) / numSections; // <= 1
            var isTabletop = Probability.Resolve(
                Settings.Instance.Sections.ChanceOfFeatureVariance.MakeValue(progression),
                Settings.Instance.Sections.ChanceOfFeature,
                true);
            if (isTabletop)
            {
                //if it's a tabletop:
                double topLength =
                    Settings.Instance.Sections.FeatureLengthVariance.ProportionAlong(progression,
                        Settings.Instance.Sections.MinFeatureLength.TotalSeconds,
                        Settings.Instance.Sections.MaxFeatureLength.TotalSeconds);
                double maxRampLength = (sectionLengthSeconds - topLength) / 2;
                if (Settings.Instance.Sections.MinRampLength.TotalSeconds > maxRampLength) throw new InvalidOperationException($"MinRampLength must be <= maxRampLength. MinTabletopLength could be too high.");

                // could feasibly be MinRampLength at the start of the track. Desirable? Yes, because other parameters constrain the dramaticness at the start.
                double rampLength =
                    Settings.Instance.Sections.RampLengthVariance.ProportionAlong(progression,
                        Settings.Instance.Sections.MaxRampLength.TotalSeconds,
                        Settings.Instance.Sections.MinRampLength.TotalSeconds); // Max is first as shorter ramps are more dramatic (nearer the end of the track)
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
            var isRise = Probability.Resolve(
                Randomizer.GetRandom(),
                Settings.Instance.PulseFrequency.ChanceOfHigh, true);
            double frequencyLimit = isRise ? Settings.Instance.PulseFrequency.High : Settings.Instance.PulseFrequency.Low;
            double topFrequency = Settings.Instance.PulseFrequency.Variation.ProportionAlong(progression,
                Settings.Instance.PulseFrequency.Quiescent,
                frequencyLimit);
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
                double maxWetness = Settings.Instance.MinWetness + Math.Pow(progression, Settings.Instance.WetnessRiseSlownessFactor) * Randomizer.GetRandom() * (Settings.Instance.MaxWetness - Settings.Instance.MinWetness);
                File.AppendAllLines($"{compositionName}.report.txt", new[] { $"The max wetness for section {s} is {maxWetness}" });
                return maxWetness;
            });

            double wetness;
            if (Settings.Instance.LinkWetnessToTabletop)
            {
                var p = GetTabletopParamsBySection(section);
                wetness = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, Settings.Instance.MinWetness, maxWetnessForSection, p);
            }
            else
            {
                double ps = ts / sectionLengthSeconds; // progression through section
                double x = ps * Math.PI;// max wetness in the middle (x = pi/2)
                wetness = Settings.Instance.MinWetness +
                    Math.Pow(Math.Sin(x), 2) * (maxWetnessForSection - Settings.Instance.MinWetness);
            }
            return wetness;
        }
    }
}
