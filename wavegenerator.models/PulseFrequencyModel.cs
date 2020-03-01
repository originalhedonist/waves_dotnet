using System.ComponentModel.DataAnnotations;

namespace wavegenerator.models
{
    public class PulseFrequencyModel
    {
        [Range(0.001, double.MaxValue)]
        public double Quiescent { get; set; }

        [Range(0.001, double.MaxValue)]
        public double Low { get; set; }

        [Range(0.001, double.MaxValue)]
        public double High { get; set; }

        [Range(0.0, 1.0)]
        public double ChanceOfHigh { get; set; }

        [ValidateObject]
        public VarianceModel Variation { get; set; }
    }
}
