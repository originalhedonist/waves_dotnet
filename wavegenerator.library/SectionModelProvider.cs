using wavegenerator.models;

namespace wavegenerator.library
{
    public class SectionModelProvider : ISettingsSectionProvider<SectionsModel>
    {
        private static int nextid = 0;
        private int id = nextid++;
        private readonly ChannelSettingsModel channelSettingsModel;

        public SectionModelProvider(ISettingsSectionProvider<ChannelSettingsModel> channelSettingsModelProvider)
        {
            this.channelSettingsModel = channelSettingsModelProvider.GetSetting();
        }
        
        public SectionsModel GetSetting()
        {
            return channelSettingsModel.Sections;
        }
    }
}