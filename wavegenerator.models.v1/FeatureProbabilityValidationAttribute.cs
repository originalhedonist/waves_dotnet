using System;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{

    public class FeatureProbabilityValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var featureProbability = (FeatureProbabilityModel)value;
            if (featureProbability != null)
            {
                if (featureProbability.Frequency + featureProbability.Wetness > 1)
                {
                    return new ValidationResult("Total FeatureProbability cannot be > 1.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
