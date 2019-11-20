using System;

namespace wavegenerator
{
    public static class Randomizer
    {
        public static Random random = new Random();
        public static double GetRandom() => 
            Settings.Instance.Randomization ? random.NextDouble() : 0.0;
    }

    public static class Probability
    {
        public static bool Resolve(double currentValue, double probability, bool defaultValue) =>
            Settings.Instance.Randomization ? currentValue >= 1 - probability : defaultValue;
    }
}
