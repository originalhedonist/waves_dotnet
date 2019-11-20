using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class Settings
    {
        public static Settings Instance = new Settings();

        [Description("Whether to use randomization")]
#if DEBUG
        public bool Randomization {get; set;} = false;
#else
        public bool Randomization {get; set;} = true;
#endif

        public bool ConvertToMp3 {get; set;} = true;

        public int NumFiles {get; set;} = 1;

        public NamingConvention Naming { get; set; } = NamingConvention.RandomFemaleName;

        [Range(typeof(TimeSpan), "00:00:30", "13:31:35")]
#if DEBUG
        public TimeSpan TrackLength {get; set;} = TimeSpan.FromSeconds(30);
#else
        public TimeSpan TrackLength {get; set;} = TimeSpan.FromMinutes(5);
#endif

        [Required]
        [SectionModelValidation(nameof(Sections))]
        public SectionModel Sections { get; set; } = SectionModel.Default;

        [JsonIgnore]
        public int NumSections => (int)(TrackLength.TotalSeconds / Sections.TotalLength.TotalSeconds);// number of sections in the track

        [Description("Whether the right channel's carrier signal will be phase shifted from the left's")]
        public bool PhaseShiftCarrier {get; set;} = true;

        public CarrierFrequencyModel CarrierFrequency { get; set; } = CarrierFrequencyModel.Default;

        [Description("Whether the pulsing of the right channel will be phase-shifted from the left")]
        public bool PhaseShiftPulses {get; set;}

        public PulseFrequencyModel PulseFrequency { get; set; } = PulseFrequencyModel.Default;

        [Description(@"
            Wetness rise factor. How fast the max wetness (in the middle of the tabletop) for a section rises as the track progresses.
            =0      : Rises to 'MaxWetness' instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to half way between MinWetness and MaxWetness halfway through the track
            >1 =>   : Rises quicker at the end")]
        public double WetnessRiseSlownessFactor {get; set;} = 0.5;

        [Description("Minimum wetness. Wetness stays near MinWetness near the start of the track.")]
        public double MinWetness {get; set;} = 0.5; 

        [Description("Maximum wetness. Wetness rises from MinWetness to anything up to MaxWetness by the end of the track.")]
        public double MaxWetness {get; set;} = 0.9; 

        [Description("Whether the wetness rises on the same timeframe as the tabletop (but it's still independent of the scale of the frequency variation)")]
        public bool LinkWetnessToTabletop {get; set;} = true; 

        public BreakModel Breaks { get; set; }
    }


}
