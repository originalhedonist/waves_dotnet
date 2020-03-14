using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class PulseGenerator : FrequencyFunctionWaveFile
    {
        private readonly PulseFrequencyModel pulseFrequency;
        private readonly Randomizer randomizer;
        private readonly FeatureProvider featureProvider;
        private readonly IFeatureChooser featureChooser;
        private readonly ISectionsProvider sectionsProvider;

        private readonly ConcurrentDictionary<int, double> topFrequencyCache = new ConcurrentDictionary<int, double>();
        private readonly Expression waveformScript;

        public PulseGenerator(
            PulseFrequencyModel pulseFrequency,
            IWaveformExpressionProvider waveformExpressionProvider,
            IWaveFileMetadata waveFileMetadata, 
            Randomizer randomizer,
            FeatureProvider featureProvider,
            IFeatureChooser featureChooser,
            ISectionsProvider sectionsProvider,
            ISamplingFrequencyProvider samplingFrequencyProvider) :
            base(waveFileMetadata.NumberOfChannels, waveFileMetadata.PhaseShiftPulses, samplingFrequencyProvider.SamplingFrequency)
        {
            this.pulseFrequency = pulseFrequency;
            this.randomizer = randomizer;
            this.featureProvider = featureProvider;
            this.featureChooser = featureChooser;
            this.sectionsProvider = sectionsProvider;

            var waveformExpression = waveformExpressionProvider.WaveformExpression;
            if (waveformExpression != null)
            {
                waveformScript = WaveformExpression.Parse(waveformExpression);
            }
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            var baseA = pulseFrequency == null
                ? -1
                : // if we have no PulseFrequencySection at all - we don't care about frequency (or about incrementing anything)
                await base.Amplitude(t, n, channel); // but if we have a pulse frequency, must always calculate it, even if we don't use it - it might (does) increment something important

            return baseA;
        }

        protected override async Task<double> GetWaveformSample(double[] x, bool phaseShiftChannels, int channel)
        {
            if (waveformScript != null)
            {
                var phaseShift = phaseShiftChannels && channel == 1 ? 0.25 : 0; //hardcode 0.25 seconds
                WaveformExpressionParams p = new WaveformExpressionParams
                {
                    x = x[channel] / (2 * Math.PI) + phaseShift //divide by 2pi here so the frequency matches (and we can model in excel against a 2pift sin graph)
                };
                waveformScript.setArgumentValue(nameof(WaveformExpressionParams.x), p.x);
                var result = waveformScript.calculate();
                if(double.IsNaN(result))
                {
                    throw new InvalidOperationException($"Waveform script returned NaN (not-a-number) for x = {p.x}{Environment.NewLine}{waveformScript.getExpressionString()}");
                }
                return result; // (negative, cos wetness inverts it)
            }
            else
            {
                return await base.GetWaveformSample(x, phaseShiftChannels, channel);
            }
        }

        private double CreateTopFrequency(int section)
        {
            var numSections = sectionsProvider.NumSections();
            double progression = (float) section / numSections; // <= 1
            //20% of being a fall, 80% chance a rise
            var isRise = randomizer.GetRandom() > 1 - pulseFrequency.ChanceOfHigh;
            var frequencyLimit = isRise ? pulseFrequency.High : pulseFrequency.Low;
            var topFrequency = randomizer.ProportionAlong(pulseFrequency.Variation, progression,
                pulseFrequency.Quiescent,
                frequencyLimit);
            if (topFrequency <= 0)
                throw new InvalidOperationException("TopFrequency must be > 0");

            return topFrequency;
        }

        protected override Task<double> Frequency(double t, int n, int channel)
        {
            var isThisFeature = featureChooser.IsFeature(n, nameof(FeatureProbabilityModel.Frequency));
            if (!isThisFeature) return Task.FromResult(pulseFrequency.Quiescent);
            var section = sectionsProvider.Section(n);

            var topFrequency = topFrequencyCache.GetOrAdd(section, CreateTopFrequency);
            var frequency = featureProvider.FeatureValue(t, n, pulseFrequency.Quiescent, topFrequency);
            return Task.FromResult(frequency);
        }
    }
}