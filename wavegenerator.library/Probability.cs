using wavegenerator.models;

namespace wavegenerator
{
    public class Probability
    {
        private readonly Settings settings;

        public Probability(Settings settings)
        {
            this.settings = settings;
        }
        public bool Resolve(double currentValue, double probability, bool defaultValue) =>
            settings.Randomization ? currentValue >= 1 - probability : defaultValue;
    }
}
