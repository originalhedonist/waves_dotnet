using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class BreaksModel
    {
        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan MinTimeSinceStartOfTrack { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan MinTimeBetweenBreaks { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "13:31:35")]
        public TimeSpan MaxTimeBetweenBreaks { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MinLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan MaxLength { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "13:31:35")]
        public TimeSpan RampLength { get; set; }
    }
}
