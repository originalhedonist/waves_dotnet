using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class BreakModelValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var breakModel = (BreaksModel)value;

            if (breakModel.MaxTimeBetweenBreaks < breakModel.MinTimeBetweenBreaks) return new ValidationResult($"{nameof(BreaksModel.MaxTimeBetweenBreaks)} must be >= {nameof(BreaksModel.MinTimeBetweenBreaks)}");
            if (breakModel.MaxLength < breakModel.MinLength) return new ValidationResult($"{nameof(BreaksModel.MaxLength)} must be >= {nameof(BreaksModel.MinLength)}");
            return ValidationResult.Success;
        }
        public override bool RequiresValidationContext => true;
    }

}
