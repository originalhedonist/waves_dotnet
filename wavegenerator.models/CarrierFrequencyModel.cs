using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class CarrierFrequencyModel
    {
        public static CarrierFrequencyModel Default() => new CarrierFrequencyModel { Left = 600, Right = 600 };

        [Range(1,44100)]
        public double Left { get; set; }

        [Range(1, 44100)]
        public double Right { get; set; }
    }
}
