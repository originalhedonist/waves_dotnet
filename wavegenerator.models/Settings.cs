using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using wavegenerator.models;

namespace wavegenerator
{
    public class Settings
    {
        public static Settings Default = new Settings();
        public static Settings Instance { get; set; }

        [Description("Whether to use randomization")]
#if DEBUG
        public bool Randomization { get; set; } = false;
#else
        public bool Randomization {get; set;} = true;
#endif

        public bool ConvertToMp3 { get; set; } = true;

        public int NumFiles { get; set; } = 1;

        public NamingConvention Naming { get; set; } = NamingConvention.RandomFemaleName;

        [Range(typeof(TimeSpan), "00:00:30", "13:31:35")]
#if DEBUG
        public TimeSpan TrackLength { get; set; } = TimeSpan.FromSeconds(30);
#else
        public TimeSpan TrackLength {get; set;} = TimeSpan.FromMinutes(5);
#endif
        [Range(1,2)]
        public short NumberOfChannels { get; set; }

        [Description("Whether the right channel's carrier signal will be phase shifted from the left's")]
        public bool PhaseShiftCarrier { get; set; } = true;

        [Description("Whether the pulsing of the right channel will be phase-shifted from the left")]
        public bool PhaseShiftPulses { get; set; }

        [ValidateObject]
        [ValidateChannelSettings]
        public ChannelSettingsModel[] ChannelSettings { get; set; }

        public ChannelSettingsModel Channel(int channel) => ChannelSettings.Length > 1 ? ChannelSettings[channel] : ChannelSettings.Single();
        public int NumSections(int channel) => (int)(TrackLength / Channel(channel).Sections.TotalLength);
    }

    public class ValidateChannelSettings : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var channelSettings = (ChannelSettingsModel[])value;
            var settingsModel = (Settings)validationContext.ObjectInstance;
            if(channelSettings.Length != 1 && channelSettings.Length != settingsModel.NumberOfChannels)
            {
                return new ValidationResult($"There must be either 1 or {settingsModel.NumberOfChannels} ChannelSettings");
            }
            return ValidationResult.Success;
        }
    }

    public class ChannelSettingsModel
    {
        public int NumSections() => (int)(Settings.Instance.TrackLength / Sections.TotalLength) ;
        [Required]
        [SectionModelValidation(nameof(Sections))]
        [ValidateObject]
        public SectionModel Sections { get; set; } = SectionModel.Default;

        [ValidateObject]
        public CarrierFrequencyModel CarrierFrequency { get; set; } = CarrierFrequencyModel.Default;

        [ValidateObject]
        public PulseFrequencyModel PulseFrequency { get; set; } = PulseFrequencyModel.Default;

        [ValidateObject]
        public WetnessModel Wetness { get; set; } = WetnessModel.Default;

        [ValidateObject]
        public BreakModel Breaks { get; set; } = BreakModel.Default;
    }
}
