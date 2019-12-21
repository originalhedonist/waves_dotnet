using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class PulseTopLengthModel
    {
        public bool LinkToFeature { get; set; }

        public double Amplitude { get; set; }

        public VariationModel Variation { get; set; }
    }
}
