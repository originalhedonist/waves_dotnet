﻿using System;

namespace wavegenerator
{
    public static class Randomizer
    {
        public static Random random = new Random();
        public static double GetRandom() => 
            Settings.Instance.Randomization ? random.NextDouble() : 0.0;

        public static double MakeValue(this VarianceModel variance, double progress)
        {
            int isTopHalf = Randomizer.GetRandom() >= 0.5 ? -1 : 1;
            return Math.Min(1, Math.Pow(Randomizer.GetRandom(), isTopHalf * variance.Randomness) * Math.Pow(progress, variance.Progression));
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
