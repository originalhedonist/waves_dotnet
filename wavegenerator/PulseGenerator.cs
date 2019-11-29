using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace wavegenerator
{
    public class PulseGenerator : TabletopGenerator
    {
        private readonly int[] sectionsThatAreBreaks;
        public PulseGenerator(ChannelSettingsModel channelSettings) :
            base(channelSettings.PulseFrequency.Quiescent, channelSettings.Sections.TotalLength.TotalSeconds, channelSettings.NumSections())
        {
            this.sectionsThatAreBreaks = MakeBreaks(channelSettings).ToArray();
            this.channelSettings = channelSettings;
        }

        private static IEnumerable<int> MakeBreaks(ChannelSettingsModel channelSettings)
        {
            int? lastBreakSection = null;
            var numSections = channelSettings.NumSections();
            if (channelSettings.Breaks != null)
            {
                do
                {
                    var minTime = lastBreakSection == null ?
                        (channelSettings.Breaks.MinTimeSinceStartOfTrack) :
                        ((lastBreakSection.Value + 1) * channelSettings.Sections.TotalLength) + channelSettings.Breaks.MinTimeBetweenBreaks;
                    //add 1 to lastBreakSection because we want the time since the END of that section
                    var maxTime = minTime + channelSettings.Breaks.MaxTimeBetweenBreaks;
                    var breakTime = minTime + (Randomizer.GetRandom() * (maxTime - minTime));
                    lastBreakSection = (int)(breakTime / channelSettings.Sections.TotalLength);
                    yield return lastBreakSection.Value;
                } while (lastBreakSection.Value <= numSections);
            }
        }

        private readonly ConcurrentDictionary<int, TabletopParams> breakParamsCache = new ConcurrentDictionary<int, TabletopParams>();

        private bool IsBreak(int section) => sectionsThatAreBreaks.Contains(section);

        public TabletopParams GetBreakParams(int section) => breakParamsCache.GetOrAdd(section, s =>
        {
            var breakLength = channelSettings.Breaks.MinLength + (channelSettings.Breaks.MaxLength - channelSettings.Breaks.MinLength) * Randomizer.GetRandom();
            var p = new TabletopParams
            {
                RampLength = channelSettings.Breaks.RampLength.TotalSeconds,
                TopLength = breakLength.TotalSeconds,
                RampsUseSin2 = true
            };
            return p;
        });


        protected double OverrideAmplitude(double t, int n, int channel)
        {
            //Due to the way wetness inverts, 'peaks' come out as 'troughs' and vice versa. Like looking at yourself in a mirror.
            var invertedTroughLength = PeakLength(t, n, channel);
            var invertedPeakLength = TroughLength(t, n, channel);
            if (lastPeakAmplitude[channel] > 0 && lastPeak[channel] != null && t - lastPeak[channel] <= invertedPeakLength)
            {
                dt[channel] += t - lastt[channel]; // the delay gets longer while we're at the top
                dn[channel]++;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("on the way down again");
            }
            
            if (lastPeakAmplitude[channel] < 0 && lastPeak[channel] != null && t - lastPeak[channel] <= invertedTroughLength)
            {
                dt[channel] += t - lastt[channel];
                dn[channel]++;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("on the way up again");
            }
            double amplitude = FfNormalAmplitude(t - dt[channel], n - dn[channel], channel);
            lastt[channel] = t;
            return amplitude;
        }

        private double FfNormalAmplitude(double t, int n, int channel)
        {
            if (n == lastn[channel] && lastAmplitude[channel].HasValue)
                return lastAmplitude[channel].Value;

            double amplitude;
            var f = Frequency(t, n, channel);
            var dx = 2 * Math.PI * f / Settings.SamplingFrequency;
            x[channel] += dx;
            amplitude = (phaseShiftChannels && channel == 1) ? Math.Cos(x[channel]) : Math.Sin(x[channel]);
            double? amplitudeGradient = null;
            if (lastAmplitude[channel].HasValue) amplitudeGradient = amplitude - lastAmplitude[channel].Value;

            bool justReachedPeak = lastAmplitudeGradient[channel].HasValue && amplitudeGradient <= 0 && lastAmplitudeGradient[channel].Value > 0;
            bool justReachedTrough = lastAmplitudeGradient[channel].HasValue && amplitudeGradient >= 0 && lastAmplitudeGradient[channel] < 0;
            if (justReachedPeak)
            {
                lastPeak[channel] = t + dt[channel];
                lastPeakAmplitude[channel] = 1;
            }
            else if (justReachedTrough)
            {
                lastPeak[channel] = t + dt[channel];
                lastPeakAmplitude[channel] = -1;
            }

            lastn[channel] = n;
            lastAmplitude[channel] = amplitude;
            lastAmplitudeGradient[channel] = amplitudeGradient;
            return amplitude;
        }

        public override double Amplitude(double t, int n, int channel)
        {
            var section = Section(n);
            double baseA = OverrideAmplitude(t, n, channel);// must always calculate it, even if we don't use it - it might (does) increment something important

            //first apply wetness,
            double wetness = Wetness(t, n, channel);
            double apos = (baseA + 1) / 2; //base amplitude, always positive - but with proper curves unlike abs
            double dryness = 1 - wetness;
            double a = 1 - dryness * apos;

            //then apply break
            double a_res;
            if (IsBreak(section))
            {
                double ts = t - (section * sectionLengthSeconds); //time through the current section
                var p = GetBreakParams(section);
                a_res = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, a, 0, p);
            }
            else
            {
                a_res = a;
            }
            return a_res;
        }

        protected override TabletopParams CreateFeatureParamsForSection(int section)
        {
            if (IsBreak(section)) return new TabletopParams { RampLength = 0, TopLength = 0, RampsUseSin2 = false };//don't apply a table top if we're on a break

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

        protected override double CreateTopFrequency(int section)
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
        private readonly ConcurrentDictionary<int, double> maxPeakForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ConcurrentDictionary<int, double> maxTroughForSectionCache = new ConcurrentDictionary<int, double>();
        private readonly ChannelSettingsModel channelSettings;

        private double Wetness(double t, int n, int channel)
        {
            // rise in a sin^2 fashion from MinWetness to MaxWetness
            int section = Section(n);
            double ts = t - (section * sectionLengthSeconds); //time through the current section

            double maxWetnessForSection = maxWetnessForSectionCache.GetOrAdd(section, s =>
            {
                double progression = ((float)s + 1) / numSections; // <= 1
                double maxWetness = channelSettings.Wetness.Variation.ProportionAlong(progression, channelSettings.Wetness.Minimum, channelSettings.Wetness.Maximum);
                return maxWetness;
            });

            double wetness;
            if (channelSettings.Wetness.LinkToFeature)
            {
                var p = GetTabletopParamsBySection(section);
                wetness = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, channelSettings.Wetness.Minimum, maxWetnessForSection, p);
            }
            else
            {
                wetness = maxWetnessForSection;
            }
            return wetness;
        }

        private double PeakOrTroughLength(double t, int n, int channel, ConcurrentDictionary<int, double> maxValueCache, PulseTopLengthModel setting)
        {
            int section = Section(n);
            double ts = t - (section * sectionLengthSeconds); //time through the current section
            double length;
            double maxForSection = maxValueCache.GetOrAdd(section, s =>
            {
                double progression = ((float)s + 1) / numSections; // <= 1
                double max = setting.Variation.ProportionAlong(progression, setting.Min.TotalSeconds, setting.Max.TotalSeconds);
                return max;
            });
            if (setting.LinkToFeature)
            {
                var p = GetTabletopParamsBySection(section);
                length = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, setting.Min.TotalSeconds, maxForSection, p);
            }
            else
            {
                length = maxForSection;
            }
            return length;
        }

        protected override double PeakLength(double t, int n, int channel)
        {
            if (channelSettings.PeakLength == null) return 0;
            return PeakOrTroughLength(t, n, channel, maxPeakForSectionCache, channelSettings.PeakLength);
        }

        protected override double TroughLength(double t, int n, int channel)
        {
            if (channelSettings.TroughLength == null) return 0;
            return PeakOrTroughLength(t, n, channel, maxTroughForSectionCache, channelSettings.TroughLength);
        }

    }
}
