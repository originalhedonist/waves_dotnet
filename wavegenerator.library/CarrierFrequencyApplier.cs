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
        private readonly CarrierFrequencyModel carrierFrequency;
        private readonly FeatureProvider featureProvider;
        private readonly ISamplingFrequencyProvider samplingFrequencyProvider;

        public CarrierFrequencyApplier(
            Settings settings, 
            CarrierFrequencyModel carrierFrequency, 
            FeatureProvider featureProvider,
            ISamplingFrequencyProvider samplingFrequencyProvider) : 
            base(numberOfChannels: settings.NumberOfChannels, phaseShiftChannels: settings.PhaseShiftCarrier, samplingFrequencyProvider.SamplingFrequency)
        {
            this.settings = settings;
            this.carrierFrequency = carrierFrequency;
            this.featureProvider = featureProvider;
            this.samplingFrequencyProvider = samplingFrequencyProvider;
        }

        protected override async Task<double> Frequency(double t, int n, int channel)
        {
            var carrierFrequencyString = channel == 0 ?
                carrierFrequency.Left :
                carrierFrequency.Right;
            return await EvaluateCarrierFrequency(t, n, carrierFrequencyString);
        }

        private static readonly ConcurrentDictionary<string, Script<double>> scripts = new ConcurrentDictionary<string, Script<double>>();
        private async Task<double> EvaluateCarrierFrequency(double t, int n, string carrierFrequencyString)
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
            if (result.ReturnValue <= 0 || result.ReturnValue > samplingFrequencyProvider.SamplingFrequency)
                throw new InvalidOperationException($"Carrier frequency function returned an out of range result of {result.ReturnValue} for t = {t}");
            return result.ReturnValue;
        }
    }
}
