using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class WetnessApplier : IAmplitude, IPerChannelComponent
    {
        private readonly PulseGenerator pulseGenerator;
        private readonly ISectionsProvider sectionsProvider;
        private readonly WetnessModel wetness;
        private readonly SectionsModel sections;
        private readonly IFeatureChooser featureChooser;
        private readonly Randomizer randomizer;
        private readonly FeatureProvider featureProvider;

        private readonly ConcurrentDictionary<int, double> maxWetnessForSectionCache =
            new ConcurrentDictionary<int, double>();

        public WetnessApplier(
            PulseGenerator pulseGenerator,
            ISectionsProvider sectionsProvider,
            IFeatureChooser featureChooser,
            Randomizer randomizer,
            FeatureProvider featureProvider,
            WetnessModel wetness,
            SectionsModel sections
            )
        {
            this.pulseGenerator = pulseGenerator;
            this.sectionsProvider = sectionsProvider;
            this.wetness = wetness;
            this.sections = sections;
            this.featureChooser = featureChooser;
            this.randomizer = randomizer;
            this.featureProvider = featureProvider;
        }

        public async Task<double> Amplitude(double t, int n, int channel)
        {
            var baseA = await pulseGenerator.Amplitude(t, n, channel); // but if we have a pulse frequency, must always calculate it, even if we don't use it - it might (does) increment something important

            //apply wetness
            var wetnessVal = Wetness(t, n);
            var apos = (1 - baseA) / 2; //base amplitude, always positive - but with proper curves unlike abs
            var dryness = 1 - wetnessVal;
            var a = 1 - dryness * apos;
            return a;
        }

        private double Wetness(double t, int n)
        {
            if (wetness == null) return 0;

            if (sections == null) return wetness.Maximum;

            // rise in a sin^2 fashion from MinWetness to MaxWetness
            var section = sectionsProvider.Section(n);

            var maxForSection = maxWetnessForSectionCache.GetOrAdd(section, s =>
            {
                var numSections = sectionsProvider.NumSections();
                var progression = (double)s / Math.Max(1, numSections - 1); // <= 1
                var max = randomizer.ProportionAlong(wetness.Variation, progression,
                    wetness.Minimum, wetness.Maximum);
                return max;
            });

            double value;
            if (wetness.LinkToFeature)
            {
                var isThisFeature = featureChooser.IsFeature(n, nameof(FeatureProbabilityModel.Wetness));
                value = isThisFeature ?
                    featureProvider.FeatureValue(t, n, wetness.Minimum, maxForSection)
                    : wetness.Minimum;
            }
            else
            {
                value = maxForSection;
            }

            return value;
        }

    }
}