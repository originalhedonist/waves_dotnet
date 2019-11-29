﻿using System;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{
    public class PulseFrequencyModel
    {
        [Range(0.001, Settings.SamplingFrequency)]
        public double Quiescent { get; set; }

        [Range(0.001, Settings.SamplingFrequency)]
        public double Low { get; set; }

        [Range(0.001, Settings.SamplingFrequency)]
        public double High { get; set; }

        [Range(0.0, 1.0)]
        public double ChanceOfHigh { get; set; }

        [ValidateObject]
        public VariationModel Variation { get; set; }
    }
}
