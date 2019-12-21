using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class PulseTopLengthModel
    {
        public bool LinkToFeature { get; set; }

        [Range(double.Epsilon, 1)]
        public double Amplitude { get; set; }

        [Range(double.Epsilon, double.MaxValue)]
        public double WavelengthFactor { get; set; }

        public VariationModel Variation { get; set; }
    }
}
