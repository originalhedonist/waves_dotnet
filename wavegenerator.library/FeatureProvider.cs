using System;
using System.Collections.Concurrent;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class FeatureProvider
    {
        public FeatureProvider(
            SectionsModel sectionsModel, 
            Randomizer randomizer,
            ISectionsProvider sectionsProvider)
        {
            this.sectionModel = sectionsModel;
            this.randomizer = randomizer;
            this.sectionsProvider = sectionsProvider;
        }
        private int Section(int n) => (int)(n / (sectionModel.TotalLength.TotalSeconds * Settings.SamplingFrequency));
        private static readonly ConcurrentDictionary<int, TabletopParams> ParamsCache = new ConcurrentDictionary<int, TabletopParams>();
        private readonly Randomizer randomizer;
        private readonly ISectionsProvider sectionsProvider;
        private readonly SectionsModel sectionModel;

        // get a feature-dependent amplitude based on the time through the track
        public double FeatureValue(double t, int n, double min, double max)
        {
            if (sectionModel == null) return 0;

            double sectionLengthSeconds = sectionModel.TotalLength.TotalSeconds;
            int section = Section(n);
            double ts = t - (section * sectionModel.TotalLength.TotalSeconds); //time through the current section
            var p = GetTabletopParamsBySection(section, nameof(FeatureProbabilityModel.Wetness));
            var a = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, min, max, p);
            return a;
        }

        private TabletopParams GetTabletopParamsBySection(int section, string feature)
        {
            return ParamsCache.GetOrAdd(section, s =>
            {
                var p = CreateFeatureParamsForSection(section);
                ValidateParams(p);
                return p;
            });
        }

        private void ValidateParams(TabletopParams p)
        {
            double sectionLengthSeconds = sectionModel.TotalLength.TotalSeconds;
            if (p.TopLength < 0) throw new InvalidOperationException("TopLength must be >= 0");
            if (p.RampLength < 0) throw new InvalidOperationException("RampLength must be >= 0");
            if (p.TopLength + 2 * p.RampLength > sectionLengthSeconds) throw new InvalidOperationException("TopLength + 2*RampLength must be <= sectionLengthSeconds");
        }

        private TabletopParams CreateFeatureParamsForSection(int section)
        {
            int numSections = sectionsProvider.NumSections();
            double sectionLengthSeconds = sectionModel.TotalLength.TotalSeconds;

            //first decide if it has a tabletop at all.
            //the chance of it being something at all rises from 0% to 100%.
            double progression = ((float)section + 1) / numSections; // <= 1

            //if it's a tabletop:
            double topLength =
                randomizer.ProportionAlong(sectionModel.FeatureLengthVariation, progression,
                    sectionModel.MinFeatureLength.TotalSeconds,
                    sectionModel.MaxFeatureLength.TotalSeconds);
            double maxRampLength = Math.Min(sectionModel.MaxRampLength.TotalSeconds, (sectionLengthSeconds - topLength) / 2);
            if (sectionModel.MinRampLength.TotalSeconds > maxRampLength) throw new InvalidOperationException($"MinRampLength must be <= maxRampLength. MinTabletopLength could be too high.");

            // could feasibly be MinRampLength at the start of the track. Desirable? Yes, because other parameters constrain the dramaticness at the start.
            double rampLength =
                randomizer.ProportionAlong(sectionModel.RampLengthVariation, progression,
                    maxRampLength,
                    sectionModel.MinRampLength.TotalSeconds); // Max is first as shorter ramps are more dramatic (nearer the end of the track)
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
