using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class BreaksModel
    {
        [Range(typeof(TimeSpan), "00:00:30", "13:31:35")]
        public TimeSpan MinTimeSinceStartOfTrack { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan MinTimeBetweenBreaks { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan MaxTimeBetweenBreaks { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MinLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MaxLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan RampLength { get; set; }
    }

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
