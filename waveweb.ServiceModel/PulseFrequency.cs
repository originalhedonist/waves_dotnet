namespace waveweb.ServiceModel
{
    public class PulseFrequency
    {
        public double Quiescent { get; set; }
        public double Low { get;set; }
        public double High { get; set; }
        public double ChanceOfHigh { get; set; }
        public Variance Variation { get; set; }
    }
}
