using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class PulseTopLengthModel
    {
        public bool LinkToFeature { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan Min { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan Max { get; set; }

        public VariationModel Variation { get; set; }
    }
}
