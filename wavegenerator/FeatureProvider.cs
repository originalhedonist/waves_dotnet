using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace wavegenerator
{
    public static class FeatureProvider
    {
        private static int Section(ChannelSettingsModel channel, int n) => (int)(n / (channel.Sections.TotalLength.TotalSeconds * Settings.SamplingFrequency));
        private static readonly ConcurrentDictionary<int, TabletopParams> paramsCache = new ConcurrentDictionary<int, TabletopParams>();

        // get a feature-dependent amplitude based on the time through the track
        public static double FeatureValue(ChannelSettingsModel channelSettings, double t, int n, double min, double max)
        {
            if (channelSettings == null) return 0;

            double sectionLengthSeconds = channelSettings.Sections.TotalLength.TotalSeconds;
            int section = Section(channelSettings, n);
            double ts = t - (section * channelSettings.Sections.TotalLength.TotalSeconds); //time through the current section
            var p = GetTabletopParamsBySection(channelSettings, section, nameof(FeatureProbability.Wetness));
            var a = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, min, max, p);
            return a;
        }

        private static TabletopParams GetTabletopParamsBySection(ChannelSettingsModel channelSettings, int section, string feature)
        {
            return paramsCache.GetOrAdd(section, s =>
            {
                var p = CreateFeatureParamsForSection(channelSettings, section);
                ValidateParams(channelSettings, p);
                return p;
            });
        }

        private static void ValidateParams(ChannelSettingsModel channelSettings, TabletopParams p)
        {
            double sectionLengthSeconds = channelSettings.Sections.TotalLength.TotalSeconds;
            if (p.TopLength < 0) throw new InvalidOperationException("TopLength must be >= 0");
            if (p.RampLength < 0) throw new InvalidOperationException("RampLength must be >= 0");
            if (p.TopLength + 2 * p.RampLength > sectionLengthSeconds) throw new InvalidOperationException("TopLength + 2*RampLength must be <= sectionLengthSeconds");
        }

        private static TabletopParams CreateFeatureParamsForSection(ChannelSettingsModel channelSettings, int section)
        {
            int numSections = channelSettings.NumSections();
            double sectionLengthSeconds = channelSettings.Sections.TotalLength.TotalSeconds;

            //first decide if it has a tabletop at all.
            //the chance of it being something at all rises from 0% to 100%.
            double progression = ((float)section + 1) / numSections; // <= 1

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

    }
}
