using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{
    public class WetnessModel
    {
        public bool LinkToFeature { get; set; } = true;

        [Range(0.0, 1.0)]
        public double Minimum { get; set; }

        [Range(0.0, 1.0)]
        public double Maximum { get; set; }

        [Required]
        [ValidateObject]
        public VarianceModel Variation { get; set; }
    }
}
