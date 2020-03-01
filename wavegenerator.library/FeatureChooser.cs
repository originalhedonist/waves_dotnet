using System.Collections.Concurrent;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class FeatureChooser
    {
        private readonly ISectionsProvider sectionsProvider;
        private readonly FeatureProbabilityModel featureProbability;
        private readonly Randomizer randomizer;

        private readonly ConcurrentDictionary<(int Section, FeatureProbabilityModel FeatureProbability), string>
            featureTypeCache = new ConcurrentDictionary<(int, FeatureProbabilityModel), string>();

        public FeatureChooser(
            ISectionsProvider sectionsProvider,
            ISettingsSectionProvider<FeatureProbabilityModel> featureProbabilityModelProvider,
            Randomizer randomizer
            )
        {
            this.sectionsProvider = sectionsProvider;
            this.featureProbability = featureProbabilityModelProvider.GetSetting();
            this.randomizer = randomizer;
        }

        public bool IsFeature(int n, string feature)
        {
            var section = sectionsProvider.Section(n);
            return feature == featureTypeCache.GetOrAdd(
                (section, featureProbability), k =>
                {
                    var v = k.FeatureProbability.Decide(randomizer.GetRandom(0.5));
                    return v;
                });
        }
    }
}