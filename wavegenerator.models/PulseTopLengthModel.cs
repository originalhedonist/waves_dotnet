using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class PulseTopLengthModel
    {
        public bool LinkToFeature { get; set; }

        [Range(double.Epsilon, 1)]
        public double Amplitude { get; set; }

        [Range(1, double.MaxValue)]
        public double WavelengthFactor { get; set; }

        public VariationModel Variation { get; set; }
    }
}
