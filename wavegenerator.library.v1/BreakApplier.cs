using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class BreakApplier : IPerChannelComponent
    {
        private readonly IWaveFileMetadata settings;
        private readonly Randomizer randomizer;
        private readonly Break[] breaks;

        public BreakApplier(IWaveFileMetadata settings, BreaksModel breaksSettings, Randomizer randomizer)
        {
            this.settings = settings;
            this.randomizer = randomizer;
            breaks = MakeBreaks(breaksSettings).ToArray();
        }

        public Task<double> Amplitude(double t, int n, int channel)
        {
            var brk = breaks.FirstOrDefault(b => b.IsWithin(t));
            var att = brk?.Attenuation(t) ?? 1;
            return Task.FromResult(att);
        }

        private IEnumerable<Break> MakeBreaks(BreaksModel breakModel)
        {
            Break b = null;
            if (breakModel != null)
            {
                do
                {
                    var minTime = b?.EndTime + breakModel.MinTimeBetweenBreaks ?? breakModel.MinTimeSinceStartOfTrack;
                    var maxTime = minTime + (breakModel.MaxTimeBetweenBreaks - breakModel.MinTimeBetweenBreaks);
                    var breakTime = minTime + (randomizer.GetRandom(0.5) * (maxTime - minTime));
                    var topLength = breakModel.MinLength + (randomizer.GetRandom(0.5) * (breakModel.MaxLength - breakModel.MinLength));
                    b = new Break(breakTime, topLength, breakModel.RampLength);
                    yield return b;
                } while (b.EndTime < settings.TrackLength);
            }
        }
    }
}
