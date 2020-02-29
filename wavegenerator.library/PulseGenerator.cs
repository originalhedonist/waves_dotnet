﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class PulseGenerator : FrequencyFunctionWaveFile, IPerChannelComponent
    {
        private readonly ConcurrentDictionary<int, double> maxWetnessForSectionCache =
            new ConcurrentDictionary<int, double>();

        private readonly ChannelSettingsModel channelSettings;
        private readonly Settings settings;
        private readonly Randomizer randomizer;
        private readonly Probability probability;
        private readonly FeatureProvider featureProvider;

        private readonly ConcurrentDictionary<(int Section, FeatureProbabilityModel FeatureProbability), string>
            featureTypeCache = new ConcurrentDictionary<(int, FeatureProbabilityModel), string>();

        private readonly ConcurrentDictionary<int, double> topFrequencyCache = new ConcurrentDictionary<int, double>();
        private readonly Script<double> waveformScript;

        public PulseGenerator(ChannelSettingsModel channelSettings, Settings settings, Randomizer randomizer,
            Probability probability, FeatureProvider featureProvider) :
            base(settings.NumberOfChannels, settings.PhaseShiftPulses)
        {
            this.channelSettings = channelSettings;
            this.settings = settings;
            this.randomizer = randomizer;
            this.probability = probability;
            this.featureProvider = featureProvider;

            var numberOfChannels = settings.NumberOfChannels;
            if (channelSettings.WaveformExpression != null)
                waveformScript = WaveformExpression.Parse(channelSettings.WaveformExpression);
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            var baseA = channelSettings.PulseFrequency == null
                ? -1
                : // if we have no PulseFrequencySection at all - we don't care about frequency (or about incrementing anything)
                await base.Amplitude(t, n,
                    channel); // but if we have a pulse frequency, must always calculate it, even if we don't use it - it might (does) increment something important

            //apply wetness
            var wetness = Wetness(t, n);
            var apos = (baseA + 1) / 2; //base amplitude, always positive - but with proper curves unlike abs
            var dryness = 1 - wetness;
            var a = 1 - dryness * apos;

            return a;
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
            var numSections = channelSettings.NumSections(settings);
            double progression = (float) section / numSections; // <= 1
            //20% of being a fall, 80% chance a rise
            var isRise = probability.Resolve(
                randomizer.GetRandom(),
                channelSettings.PulseFrequency.ChanceOfHigh, true);
            var frequencyLimit = isRise ? channelSettings.PulseFrequency.High : channelSettings.PulseFrequency.Low;
            var topFrequency = randomizer.ProportionAlong(channelSettings.PulseFrequency.Variation, progression,
                channelSettings.PulseFrequency.Quiescent,
                frequencyLimit);
            if (topFrequency <= 0)
                throw new InvalidOperationException("TopFrequency must be > 0");

            return topFrequency;
        }

        private int Section(int n)
        {
            return (int) (n / (channelSettings.Sections.TotalLength.TotalSeconds * Settings.SamplingFrequency));
        }

        private double Wetness(double t, int n)
        {
            if (channelSettings.Wetness == null) return 0;

            if (channelSettings.Sections == null) return channelSettings.Wetness.Maximum;

            // rise in a sin^2 fashion from MinWetness to MaxWetness
            var section = Section(n);
            var ts = t - section * channelSettings.Sections.TotalLength.TotalSeconds; //time through the current section

            var maxForSection = maxWetnessForSectionCache.GetOrAdd(section, s =>
            {
                var numSections = channelSettings.NumSections(settings);
                var progression = (double) s / Math.Max(1, numSections - 1); // <= 1
                var max = randomizer.ProportionAlong(channelSettings.Wetness.Variation, progression,
                    channelSettings.Wetness.Minimum, channelSettings.Wetness.Maximum);
                return max;
            });

            double value;
            if (channelSettings.Wetness.LinkToFeature)
            {
                var isThisFeature = nameof(FeatureProbabilityModel.Wetness) == featureTypeCache.GetOrAdd(
                    (section, channelSettings.FeatureProbability), k =>
                    {
                        var v = k.FeatureProbability.Decide(randomizer.GetRandom(0.5));
                        return v;
                    });

                value = featureProvider.FeatureValue(t, n, channelSettings.Wetness.Minimum, maxForSection);
            }
            else
            {
                value = maxForSection;
            }

            return value;
        }

        protected override async Task<double> Frequency(double t, int n, int channel)
        {
            if (channelSettings.Sections == null) return channelSettings.PulseFrequency.Quiescent;
            var section = Section(n);
            var isThisFeature = nameof(FeatureProbabilityModel.Frequency) == featureTypeCache.GetOrAdd(
                (section, channelSettings.FeatureProbability), k =>
                {
                    var v = k.FeatureProbability.Decide(randomizer.GetRandom(0.5));
                    return v;
                });
            if (!isThisFeature) return channelSettings.PulseFrequency.Quiescent;

            var topFrequency = topFrequencyCache.GetOrAdd(section, CreateTopFrequency);
            var frequency = featureProvider.FeatureValue(t, n, channelSettings.PulseFrequency.Quiescent, topFrequency);
            return frequency;
        }
    }
}