using System;

namespace wavegenerator
{
    public static class Randomizer
    {
#if RANDOM
        public static Random random = new Random();
        public static double GetRandom() => random.NextDouble();
        public static bool Probability(double prob, bool _) => GetRandom() < prob;
#else
        public static double GetRandom() => 1.0; //not random
        public static bool Probability(double prob, bool defaultValue) => defaultValue;
#endif
    }
}
