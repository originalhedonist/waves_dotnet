using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class PulseTopLengthModel
    {
        public bool LinkToFeature { get; set; }

        [Range(double.Epsilon, 1)]
        public double Amplitude { get; set; }

        public double GetLimit() => 1 - (Amplitude * 2) ;

        [Range(double.Epsilon, double.MaxValue)]
        public double MinWavelengthFactor { get; set; }

        [Range(double.Epsilon, double.MaxValue)]
        public double MaxWavelengthFactor { get; set; }

        public VarianceModel Variation { get; set; }
    }
}
