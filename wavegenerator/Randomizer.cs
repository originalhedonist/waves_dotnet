using System;

namespace wavegenerator
{
    public static class Randomizer
    {
        public static Random random = new Random();
        public static double GetRandom() => 
            Settings.Instance.Randomization ? random.NextDouble() : 1.0;
        public static bool Probability(double prob, bool defaultValue) =>  
            Settings.Instance.Randomization ? GetRandom() < prob : defaultValue;
    }
}
