using Autofac;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace wavegenerator
{
    public class ChannelSplitter : IAmplitude
    {
        private readonly ChannelComponentStack[] channelStacks;
        private readonly IComponentContext componentContext;

        public ChannelSplitter(Settings settings, IComponentContext componentContext)
        {
            channelStacks = settings.ChannelSettings.Select(channelSettings =>
            {
                var componentStack = componentContext.Resolve<ChannelComponentStack>(new TypedParameter(typeof(ChannelSettingsModel), channelSettings));
                return componentStack;
            }).ToArray();
            this.componentContext = componentContext;
        }

        public Task<double> Amplitude(double t, int n, int channel)
        {
            return channelStacks[channel].Amplitude(t, n, channel);
        }
    }
}
