using System;
using wavegenerator.library;

namespace wavegenerator
{
    public class Break
    {
        internal TimeSpan StartTime { get; }
        internal TimeSpan EndTime { get; }

        private double StartTimeSeconds => StartTime.TotalSeconds;
        private double EndTimeSeconds => EndTime.TotalSeconds;
        private readonly TabletopParams tabletopParams;

        public Break(TimeSpan startTime, TimeSpan topLength, TimeSpan rampLength)
        {
            StartTime = startTime;
            EndTime = startTime + topLength + 2*rampLength;
            tabletopParams = new TabletopParams
            {
                TopLength = topLength.TotalSeconds,
                RampLength = rampLength.TotalSeconds,
                RampsUseSin2 = true
            };
        }

        public bool IsWithin(double t) => t >= StartTimeSeconds && t < EndTimeSeconds;

        //it'll never be 'before the first ramp' or 'after the last ramp', but that's ok
        public double Attenuation(double t) => TabletopAlgorithm.GetY(t - StartTimeSeconds, EndTimeSeconds - StartTimeSeconds, 1, 0, tabletopParams);
    }
}
