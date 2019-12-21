using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using wavegenerator.models;

namespace wavegenerator
{
    public class ChannelSettingsModel
    {
        public int NumSections() => (int)(Settings.Instance.TrackLength / Sections.TotalLength);
        [Required]
        [SectionModelValidation(nameof(Sections))]
        [ValidateObject]
        public SectionModel Sections { get; set; }

        [Required]
        [ValidateObject]
        public CarrierFrequencyModel CarrierFrequency { get; set; }

        [Required]
        [ValidateObject]
        public PulseFrequencyModel PulseFrequency { get; set; }

        [Required]
        [ValidateObject]
        public WetnessModel Wetness { get; set; }

        [ValidateObject]
        public BreakModel Breaks { get; set; }

        [ValidateObject]
        public RiserModel Rises { get; set; }

        [ValidateObject]
        public PulseTopLengthModel Peaks { get; set; }

        [ValidateObject]
        public PulseTopLengthModel Troughs { get; set; }
    }

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

    public class ValidateRiserLength: ValidationAttribute
    {
        public override bool RequiresValidationContext => true;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var channelSettings = (ChannelSettingsModel[])value;
            var settingsModel = (Settings)validationContext.ObjectInstance;
            if(channelSettings.Any(cs => cs.Rises != null && cs.Rises.EarliestTime + (cs.Rises.Count * cs.Rises.LengthEach) > settingsModel.TrackLength))
            {
                return new ValidationResult($"Rises: EarliestTime + (Count * LengthEach) must be <= total track length.");
            }
            return ValidationResult.Success;
        }
    }
}
