using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace wavegenerator
{
    public class CarrierFrequencyApplier : FrequencyFunctionWaveFile
    {
        private readonly WaveStream[] patterns;

        public CarrierFrequencyApplier(WaveStream[] patterns) : 
            base(phaseShiftChannels: Settings.Instance.PhaseShiftCarrier)
        {
            this.patterns = patterns;
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            double carrierAmplitude = await base.Amplitude(t, n, channel);
            var pattern = patterns.ForChannel(channel);
            double patternAmplitude = Math.Abs(await pattern.Amplitude(t, n, channel));
            return carrierAmplitude * patternAmplitude;
        }

        protected override async Task<double> Frequency(double t, int n, int channel)
        {
            ChannelSettingsModel channelSettings = Settings.Instance.ChannelSettings.ForChannel(channel);
            var carrierFrequencyString = channel == 0 ?
                channelSettings.CarrierFrequency.Left :
                channelSettings.CarrierFrequency.Right;
            return await EvaluateCarrierFrequency(t, n, carrierFrequencyString, channelSettings);
        }

        private static readonly ConcurrentDictionary<string, Script<double>> scripts = new ConcurrentDictionary<string, Script<double>>();
        private async Task<double> EvaluateCarrierFrequency(double t, int n, string carrierFrequencyString, ChannelSettingsModel channelSettings)
        {
            var script = scripts.GetOrAdd(carrierFrequencyString, CarrierFrequenyExpression.Parse);
            var carrierFrequencyExpressionParams = new CarrierFrequencyExpressionParams
            {
                t = t,
                T = Settings.Instance.TrackLength.TotalSeconds,
                v = FeatureProvider.FeatureValue(channelSettings, t, n, 0, 1)
            };
            var result = await script.RunAsync(carrierFrequencyExpressionParams);
            if (result.Exception != null) throw result.Exception;
            if (result.ReturnValue <= 0 || result.ReturnValue > Settings.SamplingFrequency)
                throw new InvalidOperationException($"Carrier frequency function returned an out of range result of {result.ReturnValue} for t = {t}");
            return result.ReturnValue;
        }
    }
}
