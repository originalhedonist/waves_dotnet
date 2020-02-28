using System.Linq;
using System.Threading.Tasks;
using wavegenerator.library;

namespace wavegenerator
{
    public class ChannelSplitter : IAmplitude
    {
        private readonly ChannelComponentStack[] channelStacks;

        public ChannelSplitter(Settings settings, IParameterizedResolver parameterizedResolver)
        {
            channelStacks = settings.ChannelSettings.Select(channelSettings =>
            {
                var componentStack = parameterizedResolver.GetRequiredService<ChannelComponentStack>(i => i.Inject(channelSettings));
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
