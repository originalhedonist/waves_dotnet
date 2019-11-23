using System;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{
    public class PulseFrequencyModel
    {
        public static PulseFrequencyModel Default() => new PulseFrequencyModel
        {
            ChanceOfHigh = 0.8,
            Quiescent = 0.5,
            Low = 0.2,
            High = 1.2,
            Variation = new VarianceModel { Progression = 0.8, Randomness = 0.5 }
        };

        [Range(0.001, 44100)]
        public double Quiescent { get; set; }

        [Range(0.001, 44100)]
        public double Low { get; set; }

        [Range(0.001, 44100)]
        public double High { get; set; }

        [Range(0.0, 1.0)]
        public double ChanceOfHigh { get; set; }

        [ValidateObject]
        public VarianceModel Variation { get; set; }
    }
}
