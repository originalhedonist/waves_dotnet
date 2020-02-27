namespace waveweb.ServiceModel
{
    public class TestPulseWaveformResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public double[][] SampleNoFeature { get; set; }
        public double[][] SampleHighFrequency { get; set; }
        public double[][] SampleLowFrequency { get; set; }
    }


}
