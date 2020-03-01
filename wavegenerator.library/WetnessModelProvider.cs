using wavegenerator.models;

namespace wavegenerator.library
{
    public class WetnessModelProvider : ISettingsSectionProvider<WetnessModel>
    {
        private readonly ChannelSettingsModel channelSettingsModel;

        public WetnessModelProvider(ISettingsSectionProvider<ChannelSettingsModel> channelSettingsProvider)
        {
            this.channelSettingsModel = channelSettingsProvider.GetSetting();
        }
        
        public WetnessModel GetSetting()
        {
            return channelSettingsModel.Wetness;
        }
    }
}