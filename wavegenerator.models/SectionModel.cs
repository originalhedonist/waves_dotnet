using System;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{
    public class SectionModel
    {
        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan TotalLength { get; set;}

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MinFeatureLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MaxFeatureLength { get; set; }

        [Required]
        [ValidateObject]
        public VariationModel FeatureLengthVariation { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan MinRampLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan MaxRampLength { get; set; }

        [Required]
        [ValidateObject]
        public VariationModel RampLengthVariation { get; set; }
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
            if (sectionModel != null)
            {
                if (sectionModel.MinFeatureLength > sectionModel.MaxFeatureLength) return new ValidationResult($"{prefix}: {nameof(SectionModel.MinFeatureLength)} must be <= {nameof(SectionModel.MaxFeatureLength)}");
                if (sectionModel.MinRampLength > sectionModel.MaxRampLength) return new ValidationResult($"{prefix}: {nameof(SectionModel.MinRampLength)} must be <= {nameof(SectionModel.MaxRampLength)}");
                if (sectionModel.MinRampLength * 2 + sectionModel.MaxFeatureLength > sectionModel.TotalLength) return new ValidationResult($"{prefix}: {nameof(SectionModel.MaxRampLength)}x2 + {nameof(SectionModel.MaxFeatureLength)} must be <= {nameof(SectionModel.TotalLength)}");
            }
            return ValidationResult.Success;
        }

        public override bool RequiresValidationContext => true;
    }
}
