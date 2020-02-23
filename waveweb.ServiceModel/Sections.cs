namespace waveweb.ServiceModel
{
    public class Sections
    {
        public int SectionLengthSeconds { get; set; }
        public int[] FeatureLengthRange { get; set; }
        public Variance FeatureLengthVariation { get; set; }
        public int[] RampLengthRange { get; set; }
        public Variance RampLengthVariation { get; set; }
    }
}
