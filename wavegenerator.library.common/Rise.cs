using System;

namespace wavegenerator.library
{
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