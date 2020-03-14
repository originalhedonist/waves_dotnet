namespace wavegenerator.models
{
    public class WaveformExpressionProvider : IWaveformExpressionProvider
    {
        public WaveformExpressionProvider(string waveformExpression)
        {
            WaveformExpression = waveformExpression;
        }

        public string WaveformExpression { get; }
    }
}