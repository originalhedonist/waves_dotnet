namespace waveweb.ServiceModel
{
    public class TestPulseWaveformResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public double[][] Data { get; set; }
    }
}
