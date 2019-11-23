using System;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{
    public class SectionModel
    {
        public static SectionModel Default() => new SectionModel
        {
            TotalLength = TimeSpan.FromSeconds(30),

            MinFeatureLength = TimeSpan.FromSeconds(5),
            MaxFeatureLength = TimeSpan.FromSeconds(20),
            FeatureLengthVariance = new VarianceModel
            {
                Progression = 0.5,
                Randomness = 1
            },

            MinRampLength = TimeSpan.FromSeconds(1),
            MaxRampLength = TimeSpan.FromSeconds(5),
            RampLengthVariance = new VarianceModel
            {
                Progression = 0.8,
                Randomness = 0.2
            },

            ChanceOfFeature = 0.8
        };

        [Range(typeof(TimeSpan), "00:00:05", "13:31:35")]
        public TimeSpan TotalLength { get; set;}

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MinFeatureLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MaxFeatureLength { get; set; }

        [Required]
        [ValidateObject]
        public VarianceModel FeatureLengthVariance { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MinRampLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MaxRampLength { get; set; }

        [Required]
        [ValidateObject]
        public VarianceModel RampLengthVariance { get; set; }

        [Range(0.0, 1.0)]
        public double ChanceOfFeature { get; set; }
    }

    public class SectionModelValidation : ValidationAttribute
    {
        private readonly string prefix;

        public SectionModelValidation(string prefix)
        {
            this.prefix = prefix;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var sectionModel = (SectionModel)value;
            if (sectionModel.MinFeatureLength > sectionModel.MaxFeatureLength) return new ValidationResult($"{prefix}: {nameof(SectionModel.MinFeatureLength)} must be <= {nameof(SectionModel.MaxFeatureLength)}");
            if (sectionModel.MinRampLength > sectionModel.MaxRampLength) return new ValidationResult($"{prefix}: {nameof(SectionModel.MinRampLength)} must be <= {nameof(SectionModel.MaxRampLength)}");
            if (sectionModel.MinRampLength * 2 + sectionModel.MaxFeatureLength > sectionModel.TotalLength) return new ValidationResult($"{prefix}: {nameof(SectionModel.MaxRampLength)}x2 + {nameof(SectionModel.MaxFeatureLength)} must be <= {nameof(SectionModel.TotalLength)}");
            if (sectionModel.MinRampLength.TotalSeconds < 1) return new ValidationResult($"{prefix}: {nameof(SectionModel.MinRampLength)} must be at least 1 second");
            return ValidationResult.Success;
        }

        public override bool RequiresValidationContext => true;
    }
}
