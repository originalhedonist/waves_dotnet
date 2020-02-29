using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using wavegenerator.models;

namespace wavegenerator
{
    public class FeatureProvider
    {
        public FeatureProvider(ChannelSettingsModel channelSettings, Settings settings, Randomizer randomizer)
        {
            this.channelSettings = channelSettings;
            this.settings = settings;
            this.randomizer = randomizer;
        }
        private int Section(int n) => (int)(n / (channelSettings.Sections.TotalLength.TotalSeconds * Settings.SamplingFrequency));
        private static readonly ConcurrentDictionary<int, TabletopParams> paramsCache = new ConcurrentDictionary<int, TabletopParams>();
        private readonly ChannelSettingsModel channelSettings;
        private readonly Settings settings;
        private readonly Randomizer randomizer;

        // get a feature-dependent amplitude based on the time through the track
        public double FeatureValue(double t, int n, double min, double max)
        {
            if (channelSettings == null) return 0;

            double sectionLengthSeconds = channelSettings.Sections.TotalLength.TotalSeconds;
            int section = Section(n);
            double ts = t - (section * channelSettings.Sections.TotalLength.TotalSeconds); //time through the current section
            var p = GetTabletopParamsBySection(section, nameof(FeatureProbabilityModel.Wetness));
            var a = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, min, max, p);
            return a;
        }

        private TabletopParams GetTabletopParamsBySection(int section, string feature)
        {
            return paramsCache.GetOrAdd(section, s =>
            {
                var p = CreateFeatureParamsForSection(section);
                ValidateParams(p);
                return p;
            });
        }

        private void ValidateParams(TabletopParams p)
        {
            double sectionLengthSeconds = channelSettings.Sections.TotalLength.TotalSeconds;
            if (p.TopLength < 0) throw new InvalidOperationException("TopLength must be >= 0");
            if (p.RampLength < 0) throw new InvalidOperationException("RampLength must be >= 0");
            if (p.TopLength + 2 * p.RampLength > sectionLengthSeconds) throw new InvalidOperationException("TopLength + 2*RampLength must be <= sectionLengthSeconds");
        }

        private TabletopParams CreateFeatureParamsForSection(int section)
        {
            int numSections = channelSettings.NumSections(settings);
            double sectionLengthSeconds = channelSettings.Sections.TotalLength.TotalSeconds;

            //first decide if it has a tabletop at all.
            //the chance of it being something at all rises from 0% to 100%.
            double progression = ((float)section + 1) / numSections; // <= 1

            //if it's a tabletop:
            double topLength =
                randomizer.ProportionAlong(channelSettings.Sections.FeatureLengthVariation, progression,
                    channelSettings.Sections.MinFeatureLength.TotalSeconds,
                    channelSettings.Sections.MaxFeatureLength.TotalSeconds);
            double maxRampLength = Math.Min(channelSettings.Sections.MaxRampLength.TotalSeconds, (sectionLengthSeconds - topLength) / 2);
            if (channelSettings.Sections.MinRampLength.TotalSeconds > maxRampLength) throw new InvalidOperationException($"MinRampLength must be <= maxRampLength. MinTabletopLength could be too high.");

            // could feasibly be MinRampLength at the start of the track. Desirable? Yes, because other parameters constrain the dramaticness at the start.
            double rampLength =
                randomizer.ProportionAlong(channelSettings.Sections.RampLengthVariation, progression,
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
