namespace waveweb.ServiceModel
{
    public class ChannelSettings
    {
        public bool UseCustomWaveformExpression { get; set; }
        public string WaveformExpression { get; set; }
        public Sections Sections { get; set; }
        public FeatureProbability FeatureProbability { get; set; }
        public CarrierFrequency CarrierFrequency { get; set; }
        public PulseFrequency PulseFrequency { get; set; }
        public Wetness Wetness { get; set; }
        public Breaks Breaks { get; set; }
        public Rises Rises { get; set; }
    }
}
