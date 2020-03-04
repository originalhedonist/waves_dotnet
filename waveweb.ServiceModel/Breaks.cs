namespace waveweb.ServiceModel
{
    public class Breaks
    {
        public int MinTimeSinceStartOfTrackMinutes { get; set; }
        public int[] TimeBetweenBreaksMinutesRange { get; set; }
        public int[] LengthSecondsRange { get; set; }
        public int RampLengthSeconds { get; set; }
    }
}
