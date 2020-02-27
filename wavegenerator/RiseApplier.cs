using System;
using System.Linq;
using System.Threading.Tasks;

namespace wavegenerator
{
    public class RiseApplier : WaveStream
    {
        private readonly Settings settings;
        private readonly Randomizer randomizer;
        private readonly RiserModel riseModel;
        private readonly WaveStream pattern;
        private readonly TimeSpan[] riseStartTimes;

        public RiseApplier(Settings settings, Randomizer randomizer, RiserModel riseModel, WaveStream pattern)
        {
            this.settings = settings;
            this.randomizer = randomizer;
            this.riseModel = riseModel;
            this.pattern = pattern;
            riseStartTimes = MakeTimes(riseModel).ToArray();
        }

        private TimeSpan[] MakeTimes(RiserModel riserModel)
        {
            if (riserModel == null) return new TimeSpan[] { };

            var riseIndexes = Enumerable.Range(0, riserModel.Count);
            var totalAllowedTime = settings.TrackLength - riserModel.EarliestTime;
            var times = riseIndexes.Select(s =>
            {
                var earliestStartTime = riserModel.EarliestTime + totalAllowedTime * s / riserModel.Count;
                var latestEndTime = riserModel.EarliestTime + totalAllowedTime * (double)(s + 1) / riserModel.Count; //the total time window the rise can occur in.
                var latestStartTime = latestEndTime - riserModel.LengthEach; // but we don't want rises to overlap (calculation too complicated if nothing else), so limit the latest start time
                if (latestStartTime < earliestStartTime) throw new InvalidOperationException($"Error in rise calculation - latestStartTime was before earliestStartTime"); //sanity check (shouldn't occur if validation is correct)
                var time = earliestStartTime + randomizer.GetRandom(defaultValue: 0.5) * (latestStartTime - earliestStartTime);
                return time;
            }).ToArray();
            return times;
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            //Inclusive at start, exclusive at end, always have 't' at LHS for consistency.
            double proportionOfPattern;
            if (riseStartTimes.Length == 0)
            {
                proportionOfPattern = 1;
            }
            else
            {
                var numRisesAfter = riseStartTimes.Count(rst => t < rst.TotalSeconds); //wholly after, i.e. yet to start
                var maxAmplitude = Math.Pow(1 - riseModel.Amount, numRisesAfter);
                //are we in a rise?
                var spanning = riseStartTimes.Where(rst => t >= rst.TotalSeconds && t < (rst + riseModel.LengthEach).TotalSeconds).ToArray(); //FirstOrDefault doesn't return null as TimeSpan is a struct
                if (spanning.Any())
                {
                    var riseStartTime = spanning.First();
                    double proportionAlongRise = (t - riseStartTime.TotalSeconds) / riseModel.LengthEach.TotalSeconds;
                    if (proportionAlongRise < 0 || proportionAlongRise > 1) throw new InvalidOperationException($"Invalid proportionAlongRise, should be between 0 and 1"); // sanity check
                    double proportionUpRise = Math.Pow(Math.Sin(proportionAlongRise * Math.PI / 2), 2);//sin^2 from 0 to 1 to give a smooth rise
                    double minAmplitude = Math.Pow(1 - riseModel.Amount, numRisesAfter + 1);
                    proportionOfPattern = minAmplitude + (maxAmplitude - minAmplitude) * proportionUpRise;
                }
                else
                {
                    proportionOfPattern = maxAmplitude;
                }
            }

            return proportionOfPattern * await pattern.Amplitude(t, n, channel);
        }
    }
}
