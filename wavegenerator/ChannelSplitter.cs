using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace wavegenerator
{
    public class ChannelSplitter : IAmplitude
    {
        private readonly ChannelComponentStack[] channelStacks;

        public ChannelSplitter(Settings settings, IContainer container)
        {
            channelStacks = settings.ChannelSettings.Select(channelSettings =>
            {
                var nestedContainer = container.GetNestedContainer();
                nestedContainer.Inject(channelSettings);
                var componentStack = nestedContainer.GetRequiredService<ChannelComponentStack>();
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
