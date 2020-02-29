using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wavegenerator
{
    public class BreakApplier : IPerChannelComponent
    {
        private readonly Settings settings;
        private readonly Randomizer randomizer;
        private readonly Break[] breaks;

        public BreakApplier(Settings settings, ChannelSettingsModel channelSettings, Randomizer randomizer)
        {
            this.settings = settings;
            this.randomizer = randomizer;
            var breakTimes = MakeBreaks(channelSettings.Breaks).ToArray();
            breaks = breakTimes.Select(t => new Break(randomizer, t, channelSettings.Breaks)).ToArray();
        }

        public Task<double> Amplitude(double t, int n, int channel)
        {
            var brk = breaks.FirstOrDefault(b => b.IsWithin(t));
            var att = brk?.Attenuation(t) ?? 1;
            return Task.FromResult(att);
        }

        private IEnumerable<TimeSpan> MakeBreaks(BreaksModel breakModel)
        {
            TimeSpan? lastBreakTime = null;
            if (breakModel != null)
            {
                do
                {
                    var minTime = lastBreakTime + breakModel.MinTimeBetweenBreaks + 2 * breakModel.RampLength ?? breakModel.MinTimeSinceStartOfTrack;
                    var maxTime = minTime + breakModel.MaxTimeBetweenBreaks;
                    lastBreakTime = minTime + (randomizer.GetRandom(0.5) * (maxTime - minTime));
                    yield return lastBreakTime.Value;
                } while (lastBreakTime.Value < settings.TrackLength);
            }
        }
    }
}
