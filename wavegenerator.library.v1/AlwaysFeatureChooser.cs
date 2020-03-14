namespace wavegenerator.library
{
    public class AlwaysFeatureChooser : IFeatureChooser
    {
        private readonly string requiredFeature;

        public AlwaysFeatureChooser(string requiredFeature)
        {
            this.requiredFeature = requiredFeature;
        }

        public bool IsFeature(int n, string feature) => Equals(requiredFeature, feature);
    }
}