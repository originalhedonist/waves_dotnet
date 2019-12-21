using System;
using System.Collections.Concurrent;
using System.IO;

namespace wavegenerator
{

    public class PulseGenerator : FrequencyFunctionWaveFile
    {
        protected readonly double?[] lastAmplitude;
        protected readonly double?[] lastPeak; // the t of the last peak (either last top, or last bottom)
        protected readonly double?[] lastPeakAmplitude; //whether the last 'peak' was top or bottom
        protected readonly bool[] inPeak;
        protected readonly bool[] inTrough;
        protected readonly double baseFrequency;
        protected readonly double sectionLengthSeconds;
        protected readonly int numSections;


        public PulseGenerator(ChannelSettingsModel channelSettings) : base(phaseShiftChannels: Settings.Instance.PhaseShiftPulses)
        {
            this.channelSettings = channelSettings;

            this.baseFrequency = channelSettings.PulseFrequency.Quiescent;
            this.sectionLengthSeconds = channelSettings.Sections.TotalLength.TotalSeconds;
            this.numSections = channelSettings.NumSections();
            lastAmplitude = new double?[Channels];
            lastPeak = new double?[Channels];
            lastPeakAmplitude = new double?[Channels];
            inPeak = new bool[Channels];
            inTrough = new bool[Channels];
        }

        public override double Amplitude(double t, int n, int channel)
        {
            double baseA = AmplitudeInternal(t, n, channel);// must always calculate it, even if we don't use it - it might (does) increment something important

            //apply wetness
            double wetness = Wetness(t, n);
            double apos = (baseA + 1) / 2; //base amplitude, always positive - but with proper curves unlike abs
            double dryness = 1 - wetness;
            double a = 1 - dryness * apos;

            return a;
        }

        private double AmplitudeInternal(double t, int n, int channel)
        {
            double amplitude;
            var f = Frequency(t, n, channel);

            if (inPeak[channel] && inTrough[channel]) throw new InvalidOperationException($"Sanity check failed.");

            if (inPeak[channel]) f /= PeakWavelengthFactor(t, n);
            if (inTrough[channel]) f /= TroughWavelengthFactor(t, n); //switch peaks and troughs, as they are inverted (by wetness)

            var dx = 2 * Math.PI * f / Settings.SamplingFrequency;
            x[channel] += dx;
            amplitude = (phaseShiftChannels && channel == 1) ? Math.Cos(x[channel]) : Math.Sin(x[channel]);

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
                var justLeftPeak = lastAmplitude[channel].HasValue && amplitude >= channelSettings.Peaks.GetLimit() && lastAmplitude[channel]  < channelSettings.Peaks.GetLimit();
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



        private TabletopParams CreateFeatureParamsForSection(int section)
        {
            //first decide if it has a tabletop at all.
            //the chance of it being something at all rises from 0% to 100%.
            double progression = ((float)section + 1) / numSections; // <= 1
            var isTabletop = Probability.Resolve(
                Randomizer.GetRandom(),
                channelSettings.Sections.ChanceOfFeature,
                true);
            if (isTabletop)
            {
                //if it's a tabletop:
                double topLength =
                    channelSettings.Sections.FeatureLengthVariation.ProportionAlong(progression,
                        channelSettings.Sections.MinFeatureLength.TotalSeconds,
                        channelSettings.Sections.MaxFeatureLength.TotalSeconds);
                double maxRampLength = Math.Min(channelSettings.Sections.MaxRampLength.TotalSeconds, (sectionLengthSeconds - topLength) / 2);
                if (channelSettings.Sections.MinRampLength.TotalSeconds > maxRampLength) throw new InvalidOperationException($"MinRampLength must be <= maxRampLength. MinTabletopLength could be too high.");

                // could feasibly be MinRampLength at the start of the track. Desirable? Yes, because other parameters constrain the dramaticness at the start.
                double rampLength =
                    channelSettings.Sections.RampLengthVariation.ProportionAlong(progression,
                        maxRampLength,
                        channelSettings.Sections.MinRampLength.TotalSeconds); // Max is first as shorter ramps are more dramatic (nearer the end of the track)
                var result = new TabletopParams
                {
                    RampLength = rampLength,
                    TopLength = topLength,
                    RampsUseSin2 = true
                };
                return result;
            }
            else
            {
                var result = new TabletopParams();
                return result;
            }
        }

        private double CreateTopFrequency(int section)
        {
            double progression = ((float)section) / numSections; // <= 1
            //20% of being a fall, 80% chance a rise
            var isRise = Probability.Resolve(
                Randomizer.GetRandom(),
                channelSettings.PulseFrequency.ChanceOfHigh, true);
            double frequencyLimit = isRise ? channelSettings.PulseFrequency.High : channelSettings.PulseFrequency.Low;
            double topFrequency = channelSettings.PulseFrequency.Variation.ProportionAlong(progression,
                baseFrequency,
                frequencyLimit);
            if (topFrequency <= 0)
                throw new InvalidOperationException("TopFrequency must be > 0");

            return topFrequency;
        }

        private readonly ConcurrentDictionary<int, double> maxWetnessForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ConcurrentDictionary<int, double> maxPeakWavelengthFactorForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ConcurrentDictionary<int, double> maxTroughWavelengthFactorForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ChannelSettingsModel channelSettings;

        private double Wetness(double t, int n)
        {
            // rise in a sin^2 fashion from MinWetness to MaxWetness
            int section = Section(n);
            double ts = t - (section * sectionLengthSeconds); //time through the current section

            double maxForSection = maxWetnessForSectionCache.GetOrAdd(section, s =>
            {
                double progression = ((double)s) / Math.Max(1, numSections-1); // <= 1
                double max = channelSettings.Wetness.Variation.ProportionAlong(progression, channelSettings.Wetness.Minimum, channelSettings.Wetness.Maximum);
                return max;
            });

            double value;
            if (channelSettings.Wetness.LinkToFeature)
            {
                var p = GetTabletopParamsBySection(section);
                value = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, channelSettings.Wetness.Minimum, maxForSection, p);
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
            if (model == null) return 1;
            int section = Section(n);
            double ts = t - (section * sectionLengthSeconds); //time through the current section

            double maxForSection = cache.GetOrAdd(section, s =>
            {
                double progression = ((double)s) / Math.Max(1, numSections-1); // <= 1
                double max = model.Variation.ProportionAlong(progression, model.MinWavelengthFactor, model.MaxWavelengthFactor);
                return max;
            });

            double value;
            if (model.LinkToFeature)
            {
                var p = GetTabletopParamsBySection(section);
                value = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, 1, maxForSection, p);
            }
            else
            {
                value = maxForSection;
            }
            return value;
        }

        protected TabletopParams GetTabletopParamsBySection(int section) => paramsCache.GetOrAdd(section, s =>
        {
            var p = CreateFeatureParamsForSection(section);
            ValidateParams(p);
            return p;
        });

        //should only be called once, and cached.
        // It might (and very probably will) do 'Random' operations, so want the same one for the whole segment!

        private readonly ConcurrentDictionary<int, TabletopParams> paramsCache = new ConcurrentDictionary<int, TabletopParams>();
        private readonly ConcurrentDictionary<int, double> topFrequencyCache = new ConcurrentDictionary<int, double>();
        protected int Section(int n) => (int)(n / (sectionLengthSeconds * Settings.SamplingFrequency));
        protected override double Frequency(double t, int n, int channel)
        {
            int section = Section(n);
            var p = GetTabletopParamsBySection(section);

            var topFrequency = topFrequencyCache.GetOrAdd(section, CreateTopFrequency);
            if (topFrequency <= 0) throw new InvalidOperationException("TopFrequency must be >= 0");

            double ts = t - (section * sectionLengthSeconds); //time through the current section

            double frequency = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, baseFrequency, topFrequency, p);

            return frequency;
        }

        private void ValidateParams(TabletopParams p)
        {
            if (p.TopLength < 0) throw new InvalidOperationException("TopLength must be >= 0");
            if (p.RampLength < 0) throw new InvalidOperationException("RampLength must be >= 0");
            if (p.TopLength + 2 * p.RampLength > sectionLengthSeconds) throw new InvalidOperationException("TopLength + 2*RampLength must be <= sectionLengthSeconds");
        }
    }
}
