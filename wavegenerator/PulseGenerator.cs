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
        public PulseGenerator(ChannelSettingsModel channelSettings) :
            base(channelSettings.PulseFrequency.Quiescent, channelSettings.Sections.TotalLength.TotalSeconds, channelSettings.NumSections())
        {
            this.sectionsThatAreBreaks = MakeBreaks(channelSettings).ToArray();
            this.channelSettings = channelSettings;
        }

        private static IEnumerable<int> MakeBreaks(ChannelSettingsModel channelSettings)
        {
            int? lastBreakSection = null;
            var numSections = channelSettings.NumSections();
            do
            {
                var minTime = lastBreakSection == null ?
                    (channelSettings.Breaks.MinTimeSinceStartOfTrack) :
                    ((lastBreakSection.Value + 1) * channelSettings.Sections.TotalLength) + channelSettings.Breaks.MinTimeBetweenBreaks;
                //add 1 to lastBreakSection because we want the time since the END of that section
                var maxTime = minTime + channelSettings.Breaks.MaxTimeBetweenBreaks;
                var breakTime = minTime + (Randomizer.GetRandom() * (maxTime - minTime));
                lastBreakSection = (int)(breakTime / channelSettings.Sections.TotalLength);
                yield return lastBreakSection.Value;
            } while (lastBreakSection.Value <= numSections);
        }

        private readonly ConcurrentDictionary<int, TabletopParams> breakParamsCache = new ConcurrentDictionary<int, TabletopParams>();

        private bool IsBreak(int section) => sectionsThatAreBreaks.Contains(section);

        public TabletopParams GetBreakParams(int section) => breakParamsCache.GetOrAdd(section, s =>
        {
            var breakLength = channelSettings.Breaks.MinLength + (channelSettings.Breaks.MaxLength - channelSettings.Breaks.MinLength) * Randomizer.GetRandom();
            var p = new TabletopParams
            {
                RampLength = channelSettings.Breaks.RampLength.TotalSeconds,
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
            double wetness = Wetness(t, n, channel);
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
                Randomizer.GetRandom(),
                channelSettings.Sections.ChanceOfFeature,
                true);
            if (isTabletop)
            {
                //if it's a tabletop:
                double topLength =
                    channelSettings.Sections.FeatureLengthVariance.ProportionAlong(progression,
                        channelSettings.Sections.MinFeatureLength.TotalSeconds,
                        channelSettings.Sections.MaxFeatureLength.TotalSeconds);
                double maxRampLength = Math.Min(channelSettings.Sections.MaxRampLength.TotalSeconds, (sectionLengthSeconds - topLength) / 2);
                if (channelSettings.Sections.MinRampLength.TotalSeconds > maxRampLength) throw new InvalidOperationException($"MinRampLength must be <= maxRampLength. MinTabletopLength could be too high.");

                // could feasibly be MinRampLength at the start of the track. Desirable? Yes, because other parameters constrain the dramaticness at the start.
                double rampLength =
                    channelSettings.Sections.RampLengthVariance.ProportionAlong(progression,
                        maxRampLength,
                        channelSettings.Sections.MinRampLength.TotalSeconds); // Max is first as shorter ramps are more dramatic (nearer the end of the track)
                var result = new TabletopParams
                {
                    RampLength = rampLength,
                    TopLength = topLength,
                    RampsUseSin2 = true
                };
                return result;
            }
            else
            {
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
                channelSettings.PulseFrequency.ChanceOfHigh, true);
            double frequencyLimit = isRise ? channelSettings.PulseFrequency.High : channelSettings.PulseFrequency.Low;
            double topFrequency = channelSettings.PulseFrequency.Variation.ProportionAlong(progression,
                baseFrequency,
                frequencyLimit);
            if (topFrequency <= 0)
                throw new InvalidOperationException("TopFrequency must be > 0");

            return topFrequency;
        }

        private readonly ConcurrentDictionary<int, double> wetnessCache = new ConcurrentDictionary<int, double>();
        private readonly ChannelSettingsModel channelSettings;

        private double Wetness(double t, int n, int channel)
        {
            // rise in a sin^2 fashion from MinWetness to MaxWetness
            int section = Section(n);
            double ts = t - (section * sectionLengthSeconds); //time through the current section

            double maxWetnessForSection = wetnessCache.GetOrAdd(section, s =>
            {
                double progression = ((float)s + 1) / numSections; // <= 1
                double maxWetness = channelSettings.Wetness.Variation.ProportionAlong(progression,
                    channelSettings.Wetness.Minimum,
                    channelSettings.Wetness.Maximum);
                return maxWetness;
            });

            double wetness;
            if (channelSettings.Wetness.LinkToFeature)
            {
                var p = GetTabletopParamsBySection(section);
                wetness = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, channelSettings.Wetness.Minimum, maxWetnessForSection, p);
            }
            else
            {
                double ps = ts / sectionLengthSeconds; // progression through section
                double x = ps * Math.PI;// max wetness in the middle (x = pi/2)
                wetness = channelSettings.Wetness.Minimum +
                    Math.Pow(Math.Sin(x), 2) * (maxWetnessForSection - channelSettings.Wetness.Minimum);
            }
            return wetness;
        }
    }
}
