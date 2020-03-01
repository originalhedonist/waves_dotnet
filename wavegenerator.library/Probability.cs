using wavegenerator.models;

namespace wavegenerator.library
{
    public class Probability
    {
        private readonly IWaveFileMetadata waveFileMetadata;

        public Probability(IWaveFileMetadata waveFileMetadata)
        {
            this.waveFileMetadata = waveFileMetadata;
        }
        public bool Resolve(double currentValue, double probability, bool defaultValue) =>
            waveFileMetadata.Randomization ? currentValue >= 1 - probability : defaultValue;
    }
}
