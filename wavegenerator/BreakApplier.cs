using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wavegenerator
{
    public class BreakApplier : IPerChannelComponent
    {
        private readonly Settings settings;
        private readonly WaveStream pattern;
        private readonly Randomizer randomizer;
        private readonly Break[] breaks;

        public BreakApplier(Settings settings, BreakModel breakModel, WaveStream pattern, Randomizer randomizer)
        {
            this.settings = settings;
            this.pattern = pattern;
            this.randomizer = randomizer;
            var breakTimes = MakeBreaks(breakModel).ToArray();
            breaks = breakTimes.Select(t => new Break(randomizer, t, breakModel)).ToArray();
        }

        public async Task<double> Amplitude(double t, int n, int channel)
        {
            var brk = breaks.FirstOrDefault(b => b.IsWithin(t));
            var att = brk?.Attenuation(t) ?? 1;
            var a = att * await pattern.Amplitude(t, n, channel);
            return a;
        }

        private IEnumerable<TimeSpan> MakeBreaks(BreakModel breakModel)
        {
            TimeSpan? lastBreakTime = null;
            if (breakModel != null)
            {
                do
                {
                    var minTime = (lastBreakTime + breakModel.MinTimeBetweenBreaks + 2 * breakModel.RampLength) ?? breakModel.MinTimeSinceStartOfTrack;
                    var maxTime = minTime + breakModel.MaxTimeBetweenBreaks;
                    lastBreakTime = minTime + (randomizer.GetRandom(0.5) * (maxTime - minTime));
                    yield return lastBreakTime.Value;
                } while (lastBreakTime.Value < settings.TrackLength);
            }
        }
    }
}
