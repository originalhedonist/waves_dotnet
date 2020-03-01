using wavegenerator.library;
using wavegenerator.models;

namespace wavegenerator
{
    public class FeatureProbabilityModelProvider : ISettingsSectionProvider<FeatureProbabilityModel>
    {
        private readonly ChannelSettingsModel channelSettings;

        public FeatureProbabilityModelProvider(ISettingsSectionProvider<ChannelSettingsModel> channelSettingsModelProvider)
        {
            this.channelSettings = channelSettingsModelProvider.GetSetting();
        }

        public FeatureProbabilityModel GetSetting()
        {
            return channelSettings.FeatureProbability;
        }
    }
}