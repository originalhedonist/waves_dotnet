using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class VarianceModel
    {
        [Range(0.0, 1.0)]
        public double Randomness { get; set; }

        [Range(0.0, double.PositiveInfinity)]
        public double Progression { get; set; }

        public double MakeValue(double progress)
        {
            return Math.Pow(Randomizer.GetRandom(), Randomness) * Math.Pow(progress, Progression);
        }

        public double ProportionAlong(double progress, double minValue, double maxValue)
        {
            return minValue + MakeValue(progress) * (maxValue - minValue);
        }
    }
}
