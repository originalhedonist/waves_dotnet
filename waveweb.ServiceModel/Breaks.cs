namespace waveweb.ServiceModel
{
    public class Breaks
    {
        public int MinTimeSinceStartOfTrackMinutes { get; set; }
        public int MinTimeBetweenBreaksMinutes { get; set; }
        public int MaxTimeBetweenBreaksMinutes { get; set; }
        public int MinLengthSeconds { get; set; }
        public int MaxLengthSeconds { get; set; }
        public int RampLengthSeconds { get; set; }
    }
}
