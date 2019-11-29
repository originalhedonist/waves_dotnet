using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class CarrierFrequencyModel
    {
        [Range(1, Settings.SamplingFrequency)]
        public double Left { get; set; }

        [Range(1, Settings.SamplingFrequency)]
        public double Right { get; set; }
    }
}
