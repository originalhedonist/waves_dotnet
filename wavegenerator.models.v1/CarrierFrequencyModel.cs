using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class CarrierFrequencyModel
    {
        [CarrierFrequencyValidation]
        public string Left { get; set; }

        [CarrierFrequencyValidation]
        public string Right { get; set; }
    }
}
