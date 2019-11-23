using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class CarrierFrequencyModel
    {
        [Range(1,44100)]
        public double Left { get; set; }

        [Range(1, 44100)]
        public double Right { get; set; }
    }
}
