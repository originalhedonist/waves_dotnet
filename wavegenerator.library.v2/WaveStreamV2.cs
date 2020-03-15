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
        private readonly SettingsV2 settings;

#nullable enable
        private readonly PulseV2WaveFile? phase;
#nullable disable
        private readonly PulseV2WaveFile[][] channelComponents;

        public WaveStreamV2(SettingsV2 settings, ISamplingFrequencyProvider samplingFrequencyProvider, IProgressReporter progressReporter)
            : base(settings, samplingFrequencyProvider, progressReporter)
        {
            this.settings = settings;
            var functions = new List<Function>();
            var constants = new List<Constant> { new Constant("N", N) };
            if (settings.Phase != null)
            {
                phase = new PulseV2WaveFile(samplingFrequencyProvider, numberOfChannels: settings.Channels.Count, settings.Phase, constants.ToArray(), new Function[] { });
                functions.Add(new Function("phase_amp_l", "(1-p)/2", "p"));
                functions.Add(new Function("phase_amp_r", "(p+1)/2", "p"));
                functions.Add(new Function("phase_shift", "p*pi", "p"));
                functions.Add(new Function("phase", new FunctionEvaluator(phase)));
            }

            channelComponents = settings.Channels.Values.Select(channel => 
                channel.Components.Select(component => 
                    new PulseV2WaveFile(samplingFrequencyProvider, numberOfChannels:settings.Channels.Count, component, constants.ToArray(), functions.ToArray())
                    ).ToArray()).ToArray();
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            double a = 1;
            foreach(var component in channelComponents[channel])
            {
                a *= await component.Amplitude(t, n, channel);
            }
            return a;
        }
    }
}