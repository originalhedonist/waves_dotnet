using org.mariuszgromada.math.mxparser;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wavegenerator.library.common;
using wavegenerator.models;

namespace wavegenerator.library
{

    public class WaveStreamV2 : WaveStreamBase, IWaveStream
    {
#nullable enable
        private readonly PulseV2WaveFile? phase;
#nullable disable
        private readonly IAmplitude[] channelComponents;

        public WaveStreamV2(SettingsV2 settings, ISamplingFrequencyProvider samplingFrequencyProvider, IProgressReporter progressReporter, IGetRandom randomizer)
            : base(settings, samplingFrequencyProvider, progressReporter)
        {
            var functions = new List<Function>();
            var constants = new List<Constant> { new Constant("N", N) };
            if (settings.Phase != null)
            {
                phase = new PulseV2WaveFile(samplingFrequencyProvider, numberOfChannels: settings.Channels.Count, settings.Phase, constants.ToArray(), new Function[] { });
                functions.Add(new Function("phase_amp_l", "(1-p)/2", "p").verify());
                functions.Add(new Function("phase_amp_r", "(p+1)/2", "p").verify());
                functions.Add(new Function("phase_shift", "p*pi", "p").verify());
                functions.Add(new Function("phase", new FunctionEvaluator(phase)).verify());
            }

            IAmplitude MakeChannelComponent(FrequencyPulse component) => new PulseV2WaveFile(samplingFrequencyProvider, numberOfChannels: settings.Channels.Count, component, constants.ToArray(), functions.ToArray());

            var breakApplier = new BreakApplier(settings, settings.Breaks, randomizer); // one for all channels
            var riseApplier = new RiseApplier(settings, settings.Rises, randomizer);
            channelComponents = settings.Channels.Values.Select(channel => 
            {
                var preWetnessComponents = new AmplitudeAggregator(channel.Components.Select(MakeChannelComponent).ToArray());
                var wetnessApplier = new WetnessApplierV2(preWetnessComponents, channel.Wetness, functions.ToArray(), constants.ToArray());
                var carrierApplier = MakeChannelComponent(channel.Carrier);
                var postWetnessComponents = new AmplitudeAggregator(new[] { carrierApplier, wetnessApplier, breakApplier, riseApplier });
                return postWetnessComponents;
            }).ToArray();
        }

        public override Task<double> Amplitude(double t, int n, int channel) => channelComponents[channel].Amplitude(t, n, channel);
    }
}