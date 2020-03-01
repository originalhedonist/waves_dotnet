using System.Linq;
using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{

    public class ChannelSettingsProvider : ISettingsSectionProvider<ChannelSettingsModel>
    {
        private readonly ChannelSettingsModel channelSettingsModel;

        public ChannelSettingsProvider(ChannelSettingsModel channelSettingsModel)
        {
            this.channelSettingsModel = channelSettingsModel;
        }

        public ChannelSettingsModel GetSetting()
        {
            return channelSettingsModel;
        }
    }

    public class ChannelSplitter : IAmplitude
    {
        private readonly ChannelComponentStack[] channelStacks;

        public ChannelSplitter(Settings settings, IParameterizedResolver parameterizedResolver)
        {
            channelStacks = settings.ChannelSettings.Select(channelSettings =>
            {
                var channelSettingsModelProvider = new ChannelSettingsProvider(channelSettings);
                var componentStack = parameterizedResolver.GetRequiredService<ChannelComponentStack>(i => i.Inject<ISettingsSectionProvider<ChannelSettingsModel>>(channelSettingsModelProvider));
                return componentStack;
            }).ToArray();
        }

        public Task<double> Amplitude(double t, int n, int channel)
        {
            int channelIndex;
            if(channel > 0 && channelStacks.Length == 1)
            {
                channelIndex = 0;
            }
            else
            {
                channelIndex = channel;
            }
            return channelStacks[channelIndex].Amplitude(t, n, channel);
        }
    }
}
