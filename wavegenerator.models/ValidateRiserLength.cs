using System.ComponentModel.DataAnnotations;
using System.Linq;
using wavegenerator.models;

namespace wavegenerator
{
    public class ValidateRiserLength : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var channelSettings = (ChannelSettingsModel[])value;
            var settingsModel = (Settings)validationContext.ObjectInstance;
            if (channelSettings.Any(cs => cs.Rises != null && cs.Rises.Count > 0 && cs.Rises.EarliestTime.TotalSeconds + (cs.Rises.Count * cs.Rises.LengthEach.TotalSeconds) > settingsModel.TrackLength.TotalSeconds))
            {
                return new ValidationResult($"Rises: EarliestTime + (Count * LengthEach) must be <= total track length.");
            }
            return ValidationResult.Success;
        }
    }
}
