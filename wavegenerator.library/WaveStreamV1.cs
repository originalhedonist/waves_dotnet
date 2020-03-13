using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class WaveStreamV1 : WaveStreamBase
    {
        private readonly ChannelSplitter channelSplitter;

        public WaveStreamV1(Settings settings, ChannelSplitter channelSplitter, ISamplingFrequencyProvider samplingFrequencyProvider,
            IProgressReporter progressReporter) : base(settings, samplingFrequencyProvider, progressReporter)
        {
            this.channelSplitter = channelSplitter;
        }

        public override Task<double> Amplitude(double t, int n, int channel) => channelSplitter.Amplitude(t, n, channel);
    }
}