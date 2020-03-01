using wavegenerator.models;

namespace wavegenerator.library
{
    public class RiseSectionModelProvider : ISettingsSectionProvider<RisesModel>
    {
        private readonly ChannelSettingsModel channelSettings;

        public RiseSectionModelProvider(ISettingsSectionProvider<ChannelSettingsModel> channelSettingsModelProvider)
        {
            this.channelSettings = channelSettingsModelProvider.GetSetting();
        }

        public RisesModel GetSetting() => channelSettings.Rises;
    }
}