using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{
    public class ChannelSettingsModel
    {
        public int NumSections() => (int)(Settings.Instance.TrackLength.TotalSeconds / Sections.TotalLength.TotalSeconds);

        [WaveformExpressionValidation]
        public string WaveformExpression { get; set; }

        [SectionModelValidation(nameof(Sections))]
        [ValidateObject]
        public SectionModel Sections { get; set; }

        [FeatureProbabilityValidation]
        [ValidateObject]
        public FeatureProbability FeatureProbability { get; set; }

        [Required]
        [ValidateObject]
        public CarrierFrequencyModel CarrierFrequency { get; set; }

        [ValidateObject]
        public PulseFrequencyModel PulseFrequency { get; set; }

        [ValidateObject]
        public WetnessModel Wetness { get; set; }

        [ValidateObject]
        public BreakModel Breaks { get; set; }

        [ValidateObject]
        public RiserModel Rises { get; set; }

        [ValidateObject]
        public PulseTopLengthModel Peaks { get; set; }

        [ValidateObject]
        public PulseTopLengthModel Troughs { get; set; }
    }
}
