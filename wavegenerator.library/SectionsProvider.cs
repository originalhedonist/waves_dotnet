using wavegenerator.models;

namespace wavegenerator.library
{
    public class SectionsProvider : ISectionsProvider
    {
        private readonly Settings settings;
        private readonly SectionsModel sections;
        private readonly ISamplingFrequencyProvider samplingFrequencyProvider;

        public SectionsProvider(Settings settings, SectionsModel sections, ISamplingFrequencyProvider samplingFrequencyProvider)
        {
            this.settings = settings;
            this.sections = sections;
            this.samplingFrequencyProvider = samplingFrequencyProvider;
        }

        public int NumSections()
        {
            return (int)(settings.TrackLength.TotalSeconds / sections.TotalLength.TotalSeconds);
        }

        public int Section(int n)
        {
            return (int) (n / (sections.TotalLength.TotalSeconds * samplingFrequencyProvider.SamplingFrequency));
        }
    }
}