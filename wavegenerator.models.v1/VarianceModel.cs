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
    }
}
