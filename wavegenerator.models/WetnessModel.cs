using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class WetnessModel
    {
        public static WetnessModel Default = new WetnessModel
        {
            LinkToFeature = true,
            Minimum = 0.4,
            Maximum = 0.9,
            Variation = new VarianceModel
            {
                Progression = 1.5,
                Randomness = 0.2
            }
        };

        public bool LinkToFeature { get; set; } = true;

        [Range(0.0, 1.0)]
        public double Minimum { get; set; }

        [Range(0.0, 1.0)]
        public double Maximum { get; set; }

        [Required]
        public VarianceModel Variation { get; set; }
    }
}
