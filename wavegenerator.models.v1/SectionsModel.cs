using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator.models
{
    public class SectionsModel
    {
        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan TotalLength { get; set;}

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MinFeatureLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MaxFeatureLength { get; set; }

        [Required]
        [ValidateObject]
        public VarianceModel FeatureLengthVariation { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan MinRampLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan MaxRampLength { get; set; }

        [Required]
        [ValidateObject]
        public VarianceModel RampLengthVariation { get; set; }
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
            var sectionModel = (SectionsModel)value;
            if (sectionModel != null)
            {
                if (sectionModel.MinFeatureLength > sectionModel.MaxFeatureLength) return new ValidationResult($"{prefix}: {nameof(SectionsModel.MinFeatureLength)} must be <= {nameof(SectionsModel.MaxFeatureLength)}");
                if (sectionModel.MinRampLength > sectionModel.MaxRampLength) return new ValidationResult($"{prefix}: {nameof(SectionsModel.MinRampLength)} must be <= {nameof(SectionsModel.MaxRampLength)}");
                if (sectionModel.MinRampLength.TotalSeconds * 2 + sectionModel.MaxFeatureLength.TotalSeconds > sectionModel.TotalLength.TotalSeconds) return new ValidationResult($"{prefix}: {nameof(SectionsModel.MaxRampLength)}x2 + {nameof(SectionsModel.MaxFeatureLength)} must be <= {nameof(SectionsModel.TotalLength)}");
            }
            return ValidationResult.Success;
        }

        public override bool RequiresValidationContext => true;
    }
}
