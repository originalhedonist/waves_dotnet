using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class BreakModel
    {
        public static BreakModel Default = new BreakModel
        {
            MinTimeSinceStartOfTrack = TimeSpan.FromMinutes(10),
            MinLength = TimeSpan.FromSeconds(2),
            MaxLength = TimeSpan.FromSeconds(25),
            MinTimeBetweenBreaks = TimeSpan.FromMinutes(5),
            MaxTimeBetweenBreaks = TimeSpan.FromMinutes(30),
            RampLength = TimeSpan.FromSeconds(5)
        };

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
            var breakModel = (BreakModel)value;
            var settings = (Settings)validationContext.ObjectInstance;

            if (breakModel.MaxTimeBetweenBreaks < breakModel.MinTimeBetweenBreaks) return new ValidationResult($"{nameof(BreakModel.MaxTimeBetweenBreaks)} must be >= {nameof(BreakModel.MinTimeBetweenBreaks)}");
            if (breakModel.MaxLength < breakModel.MinLength) return new ValidationResult($"{nameof(BreakModel.MaxLength)} must be >= {nameof(BreakModel.MinLength)}");
            if (breakModel.MaxLength + 2 * breakModel.RampLength > settings.Sections.TotalLength) return new ValidationResult($"{nameof(BreakModel.MaxLength)} + 2x{nameof(BreakModel.RampLength)} must be <= {nameof(Settings.Sections)}.{nameof(Settings.Sections.TotalLength)}");
            return ValidationResult.Success;
        }
        public override bool RequiresValidationContext => true;
    }

}
