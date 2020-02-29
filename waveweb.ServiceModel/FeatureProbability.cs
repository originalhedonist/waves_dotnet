namespace waveweb.ServiceModel
{
    public class FeatureProbability
    {
        public double FrequencyWeighting { get; set; }
        public double WetnessWeighting { get; set; }
        public double NothingWeighting { get; set; }

        public double Total() => 
            FrequencyWeighting + 
            WetnessWeighting + 
            NothingWeighting
        ;
    }
}
