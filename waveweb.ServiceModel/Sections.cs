namespace waveweb.ServiceModel
{
    public class Sections
    {
        public int SectionLengthSeconds { get; set; }
        public int[] FeatureLengthRangeSeconds { get; set; }
        public Variance FeatureLengthVariation { get; set; }
        public int[] RampLengthRangeSeconds { get; set; }
        public Variance RampLengthVariation { get; set; }
    }
}
