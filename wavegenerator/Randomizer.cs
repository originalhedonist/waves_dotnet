using System;

namespace wavegenerator
{
    public static class Randomizer
    {
        public static Random random = new Random();
        public static double GetRandom() => 
            Constants.Randomization ? random.NextDouble() : 1.0;
        public static bool Probability(double prob, bool defaultValue) =>  
            Constants.Randomization ? GetRandom() < prob : defaultValue;
    }
}
