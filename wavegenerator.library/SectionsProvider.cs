namespace wavegenerator.library
{
    public class SectionsProvider : ISectionsProvider
    {
        private readonly Settings settings;
        private readonly SectionsModel sections;

        public SectionsProvider(Settings settings, SectionsModel sections)
        {
            this.settings = settings;
            this.sections = sections;
        }

        public int NumSections()
        {
            return (int)(settings.TrackLength.TotalSeconds / sections.TotalLength.TotalSeconds);
        }

        public int Section(int n)
        {
            return (int) (n / (sections.TotalLength.TotalSeconds * Settings.SamplingFrequency));
        }
    }
}