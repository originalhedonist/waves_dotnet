﻿using System;
using System.Collections.Concurrent;

namespace wavegenerator
{

    public class PulseGenerator : TabletopGenerator
    {
        public PulseGenerator(ChannelSettingsModel channelSettings) :
            base(channelSettings.PulseFrequency.Quiescent, channelSettings.Sections.TotalLength.TotalSeconds, channelSettings.NumSections())
        {
            this.channelSettings = channelSettings;
        }

        public override double Amplitude(double t, int n, int channel)
        {
            double baseA = base.Amplitude(t, n, channel);// must always calculate it, even if we don't use it - it might (does) increment something important

            //first apply wetness,
            double wetness = Wetness(t, n, channel);
            double apos = (baseA + 1) / 2; //base amplitude, always positive - but with proper curves unlike abs
            double dryness = 1 - wetness;
            double a = 1 - dryness * apos;

            return a;
        }

        protected override TabletopParams CreateFeatureParamsForSection(int section)
        {
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
                    channelSettings.Sections.FeatureLengthVariation.ProportionAlong(progression,
                        channelSettings.Sections.MinFeatureLength.TotalSeconds,
                        channelSettings.Sections.MaxFeatureLength.TotalSeconds);
                double maxRampLength = Math.Min(channelSettings.Sections.MaxRampLength.TotalSeconds, (sectionLengthSeconds - topLength) / 2);
                if (channelSettings.Sections.MinRampLength.TotalSeconds > maxRampLength) throw new InvalidOperationException($"MinRampLength must be <= maxRampLength. MinTabletopLength could be too high.");

                // could feasibly be MinRampLength at the start of the track. Desirable? Yes, because other parameters constrain the dramaticness at the start.
                double rampLength =
                    channelSettings.Sections.RampLengthVariation.ProportionAlong(progression,
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

        private readonly ConcurrentDictionary<int, double> maxWetnessForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ConcurrentDictionary<int, double> maxPeakForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ConcurrentDictionary<int, double> maxTroughForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ChannelSettingsModel channelSettings;

        private double Wetness(double t, int n, int channel)
        {
            // rise in a sin^2 fashion from MinWetness to MaxWetness
            int section = Section(n);
            double ts = t - (section * sectionLengthSeconds); //time through the current section

            double maxWetnessForSection = maxWetnessForSectionCache.GetOrAdd(section, s =>
            {
                double progression = ((float)s + 1) / numSections; // <= 1
                double maxWetness = channelSettings.Wetness.Variation.ProportionAlong(progression, channelSettings.Wetness.Minimum, channelSettings.Wetness.Maximum);
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
                wetness = maxWetnessForSection;
            }
            return wetness;
        }

        private double PeakOrTroughLength(double t, int n, int channel, ConcurrentDictionary<int, double> maxValueCache, PulseTopLengthModel setting)
        {
            int section = Section(n);
            double ts = t - (section * sectionLengthSeconds); //time through the current section
            double length;
            double maxForSection = maxValueCache.GetOrAdd(section, s =>
            {
                double progression = ((float)s + 1) / numSections; // <= 1
                double max = setting.Variation.ProportionAlong(progression, setting.Min.TotalSeconds, setting.Max.TotalSeconds);
                return max;
            });
            if (setting.LinkToFeature)
            {
                var p = GetTabletopParamsBySection(section);
                length = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, setting.Min.TotalSeconds, maxForSection, p);
            }
            else
            {
                length = maxForSection;
            }
            return length;
        }

        protected override double PeakLength(double t, int n, int channel)
        {
            if (channelSettings.PeakLength == null) return 0;
            return PeakOrTroughLength(t, n, channel, maxPeakForSectionCache, channelSettings.PeakLength);
        }

        protected override double TroughLength(double t, int n, int channel)
        {
            if (channelSettings.TroughLength == null) return 0;
            return PeakOrTroughLength(t, n, channel, maxTroughForSectionCache, channelSettings.TroughLength);
        }

    }
}
