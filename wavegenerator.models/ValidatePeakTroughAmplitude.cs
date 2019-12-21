using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace wavegenerator
{
    public class ValidatePeakTroughAmplitude : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var channelSettings = (ChannelSettingsModel[])value;
            var settingsModel = (Settings)validationContext.ObjectInstance;
            if (channelSettings.Any(HasMismatchedPeaksAndTroughs))
            {
                return new ValidationResult($"Peaks Amplitude must be greater than Troughs Amplitude.");
            }
            return ValidationResult.Success;
        }

        private bool HasMismatchedPeaksAndTroughs(ChannelSettingsModel channelSettings)
        {
            return channelSettings.Peaks != null && channelSettings.Troughs != null &&
                channelSettings.Peaks.Amplitude <= channelSettings.Troughs.Amplitude;
        }
    }
}
