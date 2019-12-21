using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class ValidateChannelCount : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var channelSettings = (ChannelSettingsModel[])value;
            var settingsModel = (Settings)validationContext.ObjectInstance;
            if (channelSettings.Length != 1 && channelSettings.Length != settingsModel.NumberOfChannels)
            {
                return new ValidationResult($"There must be either 1 or {settingsModel.NumberOfChannels} ChannelSettings");
            }
            return ValidationResult.Success;
        }
    }
}
