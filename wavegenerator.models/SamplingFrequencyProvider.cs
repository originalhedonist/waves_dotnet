namespace wavegenerator.models
{
    public class SamplingFrequencyProvider : ISamplingFrequencyProvider
    {
        public SamplingFrequencyProvider(int samplingFrequency)
        {
            SamplingFrequency = samplingFrequency;
        }

        public int SamplingFrequency { get; }
    }
}