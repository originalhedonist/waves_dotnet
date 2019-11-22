using System;

namespace wavegenerator
{
    public static class Randomizer
    {
        public static Random random = new Random();
        public static double GetRandom() => 
            Settings.Instance.Randomization ? random.NextDouble() : 1.0;

        public static double MakeValue(this VarianceModel variance, double progress)
        {
            int isTopHalf = GetRandom() >= 0.5 ? -1 : 1;
            double randomnessComponent = Math.Pow(GetRandom(), isTopHalf * variance.Randomness);
            double progressionComponent = Math.Pow(progress, variance.Progression);
            double desiredValue = randomnessComponent * progressionComponent;
            double normalizedValue = Math.Min(1, desiredValue);
            return normalizedValue;
        }

        public static double ProportionAlong(this VarianceModel variance, double progress, double minValue, double maxValue)
        {
            return minValue + MakeValue(variance, progress) * (maxValue - minValue);
        }
    }

    public static class Probability
    {
        public static bool Resolve(double currentValue, double probability, bool defaultValue) =>
            Settings.Instance.Randomization ? currentValue >= 1 - probability : defaultValue;
    }
}
