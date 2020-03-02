using System.Collections.Concurrent;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class CarrierFrequencyApplier : FrequencyFunctionWaveFile, IPerChannelComponent
    {
        private readonly IWaveFileMetadata metadata;
        private readonly CarrierFrequencyModel carrierFrequency;
        private readonly FeatureProvider featureProvider;

        public CarrierFrequencyApplier(
            IWaveFileMetadata metadata, 
            CarrierFrequencyModel carrierFrequency, 
            FeatureProvider featureProvider,
            ISamplingFrequencyProvider samplingFrequencyProvider) : 
            base(numberOfChannels: metadata.NumberOfChannels, phaseShiftChannels: metadata.PhaseShiftCarrier, samplingFrequencyProvider.SamplingFrequency)
        {
            this.metadata = metadata;
            this.carrierFrequency = carrierFrequency;
            this.featureProvider = featureProvider;
        }

        protected override async Task<double> Frequency(double t, int n, int channel)
        {
            var carrierFrequencyString = channel == 0 ?
                carrierFrequency.Left :
                carrierFrequency.Right;
            return await EvaluateCarrierFrequency(t, n, carrierFrequencyString);
        }

        private static readonly ConcurrentDictionary<string, Expression> scripts = new ConcurrentDictionary<string, Expression>();
        private Task<double> EvaluateCarrierFrequency(double t, int n, string carrierFrequencyString)
        {
            var script = scripts.GetOrAdd(carrierFrequencyString, CarrierFrequenyExpression.Parse);
            var p = new CarrierFrequencyExpressionParams
            {
                t = t,
                T = metadata.TrackLengthSeconds,
                v = featureProvider.FeatureValue(t, n, 0, 1)
            };
            script.setArgumentValue(nameof(CarrierFrequencyExpressionParams.t), p.t);
            script.setArgumentValue(nameof(CarrierFrequencyExpressionParams.T), p.T);
            script.setArgumentValue(nameof(CarrierFrequencyExpressionParams.v), p.v);
            var result = script.calculate();
            return Task.FromResult(result);
        }
    }
}
