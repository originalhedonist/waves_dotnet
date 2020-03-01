using wavegenerator.models;

namespace wavegenerator.library
{
    public class PulseFrequencyModelProvider : ISettingsSectionProvider<PulseFrequencyModel>
    {
        private readonly ChannelSettingsModel channelSettings;

        public PulseFrequencyModelProvider(ISettingsSectionProvider<ChannelSettingsModel> channelSettingsProvider)
        {
            this.channelSettings = channelSettingsProvider.GetSetting();
        }

        public PulseFrequencyModel GetSetting()
        {
            return channelSettings.PulseFrequency;
        }
    }
}