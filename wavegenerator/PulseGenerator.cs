using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;

namespace wavegenerator
{

    public class PulseGenerator : FrequencyFunctionWaveFile
    {
        protected readonly double?[] lastAmplitude;
        protected readonly double?[] lastPeak; // the t of the last peak (either last top, or last bottom)
        protected readonly double?[] lastPeakAmplitude; //whether the last 'peak' was top or bottom
        protected readonly bool[] inPeak;
        protected readonly bool[] inTrough;
        
        private readonly ConcurrentDictionary<(int Section, FeatureProbability FeatureProbability), string> featureTypeCache = new ConcurrentDictionary<(int, FeatureProbability), string>();
        private readonly ConcurrentDictionary<int, double> topFrequencyCache = new ConcurrentDictionary<int, double>();
        private readonly Script<double> waveformScript;

        public PulseGenerator(ChannelSettingsModel channelSettings) : base(phaseShiftChannels: Settings.Instance.PhaseShiftPulses)
        {
            this.channelSettings = channelSettings;
            lastAmplitude = new double?[Channels];
            lastPeak = new double?[Channels];
            lastPeakAmplitude = new double?[Channels];
            inPeak = new bool[Channels];
            inTrough = new bool[Channels];
            if (channelSettings.WaveformExpression != null)
            {
                waveformScript = WaveformExpression.Parse(channelSettings.WaveformExpression);
            }
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            double baseA = channelSettings.PulseFrequency == null ? -1 : // if we have no PulseFrequencySection at all - we don't care about frequency (or about incrementing anything)
                await AmplitudeInternal(t, n, channel);// but if we have a pulse frequency, must always calculate it, even if we don't use it - it might (does) increment something important

            //apply wetness
            double wetness = Wetness(t, n);
            double apos = (baseA + 1) / 2; //base amplitude, always positive - but with proper curves unlike abs
            double dryness = 1 - wetness;
            double a = 1 - dryness * apos;

            return a;
        }

        protected override async Task<double> GetWaveformSample(double[] x, bool phaseShiftChannels, int channel)
        {
            if(waveformScript != null)
            {
                double phaseShift = phaseShiftChannels && channel == 1 ? 0.25 : 0; //hardcode 0.25 seconds
                var result = await waveformScript.RunAsync(new WaveformExpressionParams { x = (x[channel]/(2*Math.PI)) + phaseShift }); //divide by 2pi here so the frequency matches (and we can model in excel against a 2pift sin graph)
                if (result.Exception != null) throw result.Exception;
                return -result.ReturnValue; // (negative, cos wetness inverts it)
            }
            else return await base.GetWaveformSample(x, phaseShiftChannels, channel);
        }

        private async Task<double> AmplitudeInternal(double t, int n, int channel)
        {
            double amplitude;
            var f = await Frequency(t, n, channel);

            if (inPeak[channel] && inTrough[channel]) throw new InvalidOperationException($"Sanity check failed.");

            if (inPeak[channel]) f /= PeakWavelengthFactor(t, n);
            if (inTrough[channel]) f /= TroughWavelengthFactor(t, n); //switch peaks and troughs, as they are inverted (by wetness)

            var dx = 2 * Math.PI * f / Settings.SamplingFrequency;
            x[channel] += dx;
            amplitude = await GetWaveformSample(x, phaseShiftChannels, channel);

            //peak detection looks like trough detection... but use 'peaks' settings
            bool justReachedPeak = channelSettings.Peaks != null && lastAmplitude[channel].HasValue && amplitude <= channelSettings.Peaks.GetLimit() && lastAmplitude[channel].Value > channelSettings.Peaks.GetLimit();
            bool justReachedTrough = channelSettings.Troughs != null && lastAmplitude[channel].HasValue && amplitude >= channelSettings.Troughs.GetLimit() && lastAmplitude[channel] < channelSettings.Troughs.GetLimit();
            if (justReachedPeak && justReachedTrough) throw new InvalidOperationException($"Sanity check failed.");

            if (justReachedPeak)
            {
                if (inPeak[channel]) throw new InvalidOperationException($"Just reached a peak when already in one");
                inPeak[channel] = true;
            }

            if (justReachedTrough)
            {
                if (inTrough[channel]) throw new InvalidOperationException($"Just reached a trough when already in one");
                inTrough[channel] = true;
            }

            //peak detection looks like trough detection... but use 'peaks' settings
            if (inPeak[channel])
            {
                var justLeftPeak = lastAmplitude[channel].HasValue && amplitude >= channelSettings.Peaks.GetLimit() && lastAmplitude[channel] < channelSettings.Peaks.GetLimit();
                if (justLeftPeak) inPeak[channel] = false;
            }

            if (inTrough[channel])
            {
                var justLeftTrough = lastAmplitude[channel].HasValue && amplitude <= channelSettings.Troughs.GetLimit() && lastAmplitude[channel] > channelSettings.Troughs.GetLimit();
                if (justLeftTrough) inTrough[channel] = false;
            }

            lastAmplitude[channel] = amplitude;

            return amplitude;
        }


        private double CreateTopFrequency(int section)
        {
            var numSections = channelSettings.NumSections();
            double progression = ((float)section) / numSections; // <= 1
            //20% of being a fall, 80% chance a rise
            var isRise = Probability.Resolve(
                Randomizer.GetRandom(),
                channelSettings.PulseFrequency.ChanceOfHigh, true);
            double frequencyLimit = isRise ? channelSettings.PulseFrequency.High : channelSettings.PulseFrequency.Low;
            double topFrequency = channelSettings.PulseFrequency.Variation.ProportionAlong(progression,
                channelSettings.PulseFrequency.Quiescent,
                frequencyLimit);
            if (topFrequency <= 0)
                throw new InvalidOperationException("TopFrequency must be > 0");

            return topFrequency;
        }

        private readonly ConcurrentDictionary<int, double> maxWetnessForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ConcurrentDictionary<int, double> maxPeakWavelengthFactorForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ConcurrentDictionary<int, double> maxTroughWavelengthFactorForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ChannelSettingsModel channelSettings;

        private int Section(int n) => (int)(n / (channelSettings.Sections.TotalLength.TotalSeconds * Settings.SamplingFrequency));

        private double Wetness(double t, int n)
        {
            if (channelSettings.Wetness == null) return 0;

            if (channelSettings.Sections == null) return channelSettings.Wetness.Maximum;

            // rise in a sin^2 fashion from MinWetness to MaxWetness
            int section = Section(n);
            double ts = t - (section * channelSettings.Sections.TotalLength.TotalSeconds); //time through the current section

            double maxForSection = maxWetnessForSectionCache.GetOrAdd(section, s =>
            {
                var numSections = channelSettings.NumSections();
                double progression = ((double)s) / Math.Max(1, numSections - 1); // <= 1
                double max = channelSettings.Wetness.Variation.ProportionAlong(progression, channelSettings.Wetness.Minimum, channelSettings.Wetness.Maximum);
                return max;
            });

            double value;
            if (channelSettings.Wetness.LinkToFeature)
            {
                var isThisFeature = nameof(FeatureProbability.Wetness) == featureTypeCache.GetOrAdd((section, channelSettings.FeatureProbability), k =>
                {
                    string v = k.FeatureProbability.Decide(Randomizer.GetRandom(defaultValue: 0.5));
                    return v;
                });

                value = FeatureProvider.FeatureValue(channelSettings, t, n, channelSettings.Wetness.Minimum, maxForSection);
            }
            else
            {
                value = maxForSection;
            }
            return value;
        }

        private double PeakWavelengthFactor(double t, int n) => PeakOrTroughWavelengthFactor(channelSettings.Peaks, maxPeakWavelengthFactorForSectionCache, t, n);
        private double TroughWavelengthFactor(double t, int n) => PeakOrTroughWavelengthFactor(channelSettings.Troughs, maxTroughWavelengthFactorForSectionCache, t, n);

        private double PeakOrTroughWavelengthFactor(PulseTopLengthModel model, ConcurrentDictionary<int, double> cache, double t, int n)
        {
            if (model == null || channelSettings.Sections == null) return 1;
            int section = Section(n);
            var isThisFeature = nameof(FeatureProbability.PeaksAndTroughs) == featureTypeCache.GetOrAdd((section, channelSettings.FeatureProbability), k =>
            {
                string v = k.FeatureProbability.Decide(Randomizer.GetRandom(defaultValue: 0.5));
                return v;
            });
            if (!isThisFeature) return 1;

            double ts = t - (section * channelSettings.Sections.TotalLength.TotalSeconds); //time through the current section

            double maxForSection = cache.GetOrAdd(section, s =>
            {
                var numSections = channelSettings.NumSections();
                double progression = ((double)s) / Math.Max(1, numSections - 1); // <= 1
                double max = model.Variation.ProportionAlong(progression, model.MinWavelengthFactor, model.MaxWavelengthFactor);
                return max;
            });

            double value;
            if (model.LinkToFeature)
            {
                value = FeatureProvider.FeatureValue(channelSettings, t, n, model.MinWavelengthFactor, maxForSection);
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
            int section = Section(n);
            var isThisFeature = nameof(FeatureProbability.Frequency) == featureTypeCache.GetOrAdd((section, channelSettings.FeatureProbability), k =>
            {
                string v = k.FeatureProbability.Decide(Randomizer.GetRandom(defaultValue: 0.5));
                return v;
            });
            if (!isThisFeature) return channelSettings.PulseFrequency.Quiescent;

            var topFrequency = topFrequencyCache.GetOrAdd(section, CreateTopFrequency);
            double frequency = FeatureProvider.FeatureValue(channelSettings, t, n, channelSettings.PulseFrequency.Quiescent, topFrequency);
            return frequency;
        }
    }
}
