using System.Linq;
using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class ChannelSplitter : IAmplitude
    {
        private readonly ChannelComponentStack[] channelStacks;

        public ChannelSplitter(Settings settings, IParameterizedResolver parameterizedResolver)
        {
            channelStacks = settings.ChannelSettings.Select(channelSettings =>
            {
                var componentStack = parameterizedResolver.GetRequiredService<ChannelComponentStack>(i =>
                {
                    i.Inject(channelSettings.Wetness);
                    i.Inject(channelSettings.Sections);
                    i.Inject(channelSettings.FeatureProbability);
                    i.Inject(channelSettings.PulseFrequency);
                    i.Inject(channelSettings.Breaks);
                    i.Inject(channelSettings.Rises);
                    i.Inject(channelSettings.CarrierFrequency);
                    i.Inject<IWaveformExpressionProvider>(channelSettings);
                });
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
