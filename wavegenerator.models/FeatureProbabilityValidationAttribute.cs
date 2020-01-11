using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class FeatureProbabilityValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var featureProbability = (FeatureProbability)value;
            if(featureProbability == null)
            {
                throw new InvalidOperationException("Feature probability section must exist.");
            }
            if(featureProbability.Frequency + featureProbability.PeaksAndTroughs + featureProbability.Wetness > 1)
            {
                return new ValidationResult("Total FeatureProbability cannot be > 1.");
            }
            return ValidationResult.Success;
        }
    }
}
