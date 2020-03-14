using System;
using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class WaveStreamV2 : WaveStreamBase, IWaveStream
    {
        private readonly SettingsV2 settings;

        public WaveStreamV2(SettingsV2 settings, ISamplingFrequencyProvider samplingFrequencyProvider, IProgressReporter progressReporter)
            : base(settings, samplingFrequencyProvider, progressReporter)
        {
            this.settings = settings;
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            return Math.Sin(2 * Math.PI * 0.1 * t);
        }
    }

    public class WaveStreamV2PulseComponent : FrequencyFunctionWaveFile
    {
        public WaveStreamV2PulseComponent(ISamplingFrequencyProvider samplingFrequencyProvider, string frequencyFunction) 
            : base(numberOfChannels: 2, phaseShiftChannels: false, samplingFrequencyProvider.SamplingFrequency)
        {

        }

        protected override Task<double> Frequency(double t, int n, int channel)
        {
            throw new NotImplementedException();
        }
    }
}