using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class CarrierFrequencyApplier : FrequencyFunctionWaveFile, IPerChannelComponentTranscendsWetness
    {
        private readonly Settings settings;
        private readonly ChannelSettingsModel channelSettings;
        private readonly FeatureProvider featureProvider;

        public CarrierFrequencyApplier(
            Settings settings, 
            ISettingsSectionProvider<ChannelSettingsModel> channelSettingsModelProvider, 
            FeatureProvider featureProvider) : 
            base(numberOfChannels: settings.NumberOfChannels, phaseShiftChannels: settings.PhaseShiftCarrier)
        {
            this.settings = settings;
            this.channelSettings = channelSettingsModelProvider.GetSetting();
            this.featureProvider = featureProvider;
        }

        protected override async Task<double> Frequency(double t, int n, int channel)
        {
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
                T = settings.TrackLength.TotalSeconds,
                v = featureProvider.FeatureValue(t, n, 0, 1)
            };
            var result = await script.RunAsync(carrierFrequencyExpressionParams);
            if (result.Exception != null) throw result.Exception;
            if (result.ReturnValue <= 0 || result.ReturnValue > Settings.SamplingFrequency)
                throw new InvalidOperationException($"Carrier frequency function returned an out of range result of {result.ReturnValue} for t = {t}");
            return result.ReturnValue;
        }
    }
}
