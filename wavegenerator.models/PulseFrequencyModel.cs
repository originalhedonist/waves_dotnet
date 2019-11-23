using System;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{
    public class PulseFrequencyModel
    {
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
