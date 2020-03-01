using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class PulseGenerator : FrequencyFunctionWaveFile, IPerChannelComponent
    {
        private readonly ChannelSettingsModel channelSettings;
        private readonly PulseFrequencyModel pulseFrequency;
        private readonly Randomizer randomizer;
        private readonly Probability probability;
        private readonly FeatureProvider featureProvider;
        private readonly FeatureChooser featureChooser;
        private readonly ISectionsProvider sectionsProvider;

        private readonly ConcurrentDictionary<int, double> topFrequencyCache = new ConcurrentDictionary<int, double>();
        private readonly Script<double> waveformScript;

        public PulseGenerator(
            ISettingsSectionProvider<ChannelSettingsModel> channelSettingsProvider,
            ISettingsSectionProvider<PulseFrequencyModel> pulseFrequencyModelProvider, 
            Settings settings, 
            Randomizer randomizer,
            Probability probability, 
            FeatureProvider featureProvider,
            FeatureChooser featureChooser,
            ISectionsProvider sectionsProvider) :
            base(settings.NumberOfChannels, settings.PhaseShiftPulses)
        {
            this.channelSettings = channelSettingsProvider.GetSetting();
            this.pulseFrequency = pulseFrequencyModelProvider.GetSetting();
            this.randomizer = randomizer;
            this.probability = probability;
            this.featureProvider = featureProvider;
            this.featureChooser = featureChooser;
            this.sectionsProvider = sectionsProvider;

            if (channelSettings.WaveformExpression != null)
                waveformScript = WaveformExpression.Parse(channelSettings.WaveformExpression);
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
                var result = await waveformScript.RunAsync(new WaveformExpressionParams
                {
                    x = x[channel] / (2 * Math.PI) + phaseShift
                }); //divide by 2pi here so the frequency matches (and we can model in excel against a 2pift sin graph)
                if (result.Exception != null) throw result.Exception;
                return -result.ReturnValue; // (negative, cos wetness inverts it)
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
            var isRise = probability.Resolve(
                randomizer.GetRandom(),
                pulseFrequency.ChanceOfHigh, true);
            var frequencyLimit = isRise ? pulseFrequency.High : pulseFrequency.Low;
            var topFrequency = randomizer.ProportionAlong(pulseFrequency.Variation, progression,
                pulseFrequency.Quiescent,
                frequencyLimit);
            if (topFrequency <= 0)
                throw new InvalidOperationException("TopFrequency must be > 0");

            return topFrequency;
        }

        private int Section(int n)
        {
            return (int) (n / (channelSettings.Sections.TotalLength.TotalSeconds * Settings.SamplingFrequency));
        }

        protected override Task<double> Frequency(double t, int n, int channel)
        {
            if (channelSettings.Sections == null) return Task.FromResult(pulseFrequency.Quiescent);
            var section = Section(n);
            var isThisFeature = featureChooser.IsFeature(n, nameof(FeatureProbabilityModel.Frequency));
            if (!isThisFeature) return Task.FromResult(pulseFrequency.Quiescent);

            var topFrequency = topFrequencyCache.GetOrAdd(section, CreateTopFrequency);
            var frequency = featureProvider.FeatureValue(t, n, pulseFrequency.Quiescent, topFrequency);
            return Task.FromResult(frequency);
        }
    }
}