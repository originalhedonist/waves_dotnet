using System;

namespace wavegenerator
{
    public static class Randomizer
    {

        //public static double GetRandom() => 1.0; //not random
        public static Random random = new Random();
        public static double GetRandom() => Randomizer.GetRandom();
    }
}
