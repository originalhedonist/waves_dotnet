using System;
using System.Collections.Generic;
using System.Linq;

namespace wavegenerator
{
    public class BreakApplier : WaveFile
    {
        private readonly WaveFile pattern;
        private readonly Break[] breaks;

        public BreakApplier(BreakModel breakModel, WaveFile pattern)
        {
            this.pattern = pattern;
            var breakTimes = MakeBreaks(breakModel).ToArray();
            breaks = breakTimes.Select(t => new Break(t, breakModel)).ToArray();
        }

        public override double Amplitude(double t, int n, int channel)
        {
            var brk = breaks.FirstOrDefault(b => b.IsWithin(t));
            var att = brk?.Attenuation(t) ?? 1;
            var a = att * pattern.Amplitude(t, n, channel);
            return a;
        }

        private static IEnumerable<TimeSpan> MakeBreaks(BreakModel breakModel)
        {
            TimeSpan? lastBreakTime = null;
            if (breakModel != null)
            {
                do
                {
                    var minTime = lastBreakTime ?? breakModel.MinTimeSinceStartOfTrack;
                    var maxTime = minTime + breakModel.MaxTimeBetweenBreaks;
                    lastBreakTime = minTime + (Randomizer.GetRandom(0.5) * (maxTime - minTime));
                    yield return lastBreakTime.Value;
                } while (lastBreakTime.Value < Settings.Instance.TrackLength);
            }
        }
    }
}
