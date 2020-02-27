using System;

namespace wavegenerator
{
    public class Randomizer
    {
        public Randomizer(Settings settings)
        {
            this.settings = settings;
        }
        private readonly Random random = new Random();
        private readonly Settings settings;

        public double GetRandom(double defaultValue = 1.0) => 
            settings.Randomization ? random.NextDouble() : defaultValue;

        public double MakeValue(VariationModel variance, double progress)
        {
            int isTopHalf = GetRandom() >= 0.5 ? -1 : 1;
            double randomnessComponent = Math.Pow(GetRandom(), isTopHalf * variance.Randomness);
            double progressionComponent = Math.Pow(progress, variance.Progression);
            double desiredValue = randomnessComponent * progressionComponent;
            double normalizedValue = Math.Min(1, desiredValue);
            return normalizedValue;
        }

        public double ProportionAlong(VariationModel variance, double progress, double minValue, double maxValue)
        {
            return minValue + MakeValue(variance, progress) * (maxValue - minValue);
        }
    }
}
