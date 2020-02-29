using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{
    public class ChannelSettingsModel
    {
        public int NumSections(Settings settings) => (int)(settings.TrackLength.TotalSeconds / Sections.TotalLength.TotalSeconds);

        [WaveformExpressionValidation]
        public string WaveformExpression { get; set; }

        [SectionModelValidation(nameof(Sections))]
        [ValidateObject]
        public SectionsModel Sections { get; set; }

        [FeatureProbabilityValidation]
        [ValidateObject]
        public FeatureProbabilityModel FeatureProbability { get; set; }

        [Required]
        [ValidateObject]
        public CarrierFrequencyModel CarrierFrequency { get; set; }

        [ValidateObject]
        public PulseFrequencyModel PulseFrequency { get; set; }

        [ValidateObject]
        public WetnessModel Wetness { get; set; }

        [ValidateObject]
        public BreaksModel Breaks { get; set; }

        [ValidateObject]
        public RisesModel Rises { get; set; }
    }
}
