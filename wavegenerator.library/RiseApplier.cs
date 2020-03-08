using System;
using System.Linq;
using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class RiseApplier : IPerChannelComponent
    {
        private readonly Settings settings;
        private readonly Randomizer randomizer;
        private readonly RisesModel riseModel;
        private readonly Rise[] rises;

        public RiseApplier(Settings settings, Randomizer randomizer, RisesModel riseModel)
        {
            this.settings = settings;
            this.randomizer = randomizer;
            this.riseModel = riseModel;
            rises = MakeRises(riseModel).ToArray();
        }

        private Rise[] MakeRises(RisesModel riserModel)
        {
            if (riserModel == null) return new Rise[] { };

            var riseIndexes = Enumerable.Range(0, riserModel.Count);
            var totalAllowedTime = settings.TrackLength - riserModel.EarliestTime;
            var rises = riseIndexes.Select(s =>
            {
                var earliestStartTime = riserModel.EarliestTime + totalAllowedTime * s / riserModel.Count;
                var latestEndTime =
                    riserModel.EarliestTime +
                    totalAllowedTime * (double) (s + 1) /
                    riserModel.Count; //the total time window the rise can occur in.
                var latestStartTime =
                    latestEndTime -
                    riserModel
                        .LengthEach; // but we don't want rises to overlap (calculation too complicated if nothing else), so limit the latest start time
                if (latestStartTime < earliestStartTime)
                    latestStartTime = earliestStartTime;
                var startTime = earliestStartTime + randomizer.GetRandom(0.5) * (latestStartTime - earliestStartTime);
                var endTime = startTime + riseModel.LengthEach;
                var minAttenuation = Math.Pow(1 - riserModel.Amount, riserModel.Count - s);
                var maxAttenuation = Math.Pow(1 - riserModel.Amount, riserModel.Count - s - 1);
                return new Rise(startTime, endTime, 1 - riserModel.Amount, 1);
            }).ToArray();
            return rises;
        }

        public Task<double> Amplitude(double t, int n, int channel)
        {
            //Inclusive at start, exclusive at end, always have 't' at LHS for consistency.
            double amplitude = 1;
            var ts = TimeSpan.FromSeconds(t);
            foreach(var rise in rises)
            {
                double attenuation;
                if (ts > rise.EndTime) attenuation = rise.MaxAttenuation;
                else if (ts < rise.StartTime) attenuation = rise.MinAttenuation;
                else
                {
                    var proportionAlongRise = (ts - rise.StartTime) / rise.Length;
                    var proportionUpRise = Math.Pow(Math.Sin(proportionAlongRise * Math.PI / 2), 2);
                    // sin^2 from 0 to 1 to give a smooth rise
                    attenuation = rise.MinAttenuation + (rise.MaxAttenuation - rise.MinAttenuation) * proportionUpRise;
                }

                amplitude *= attenuation;
            }
            return Task.FromResult(amplitude);
        }
    }

    public class Rise
    {
        public Rise(TimeSpan startTime, TimeSpan endTime, double minAttenuation, double maxAttenuation)
        {
            StartTime = startTime;
            EndTime = endTime;
            MinAttenuation = minAttenuation;
            MaxAttenuation = maxAttenuation;
        }

        public TimeSpan StartTime { get;  }
        public TimeSpan EndTime { get; }
        public TimeSpan Length  => EndTime - StartTime;
        public double MinAttenuation { get; }
        public double MaxAttenuation { get; }
    }
}