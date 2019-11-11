using System;
namespace wavegenerator
{
    public class Constants
    {
        public const int SectionLength = 30;// length of each 'section' (a 'section' containing one 'tabletop' of either rising, or falling, pulse frequency in the middle)
        public static readonly TimeSpan TrackLength = TimeSpan.FromMinutes(5);
        public static readonly int NumSections = (int)(TrackLength.TotalSeconds / SectionLength);// number of sections in the track
        public const double MinTabletopLength = 4;// min length of the 'top' part of the table top frequency
        public const double MaxTabletopLength = 15;// max length of the 'top' part of the table top frequency
        public const double TabletopLengthRiseSlownessFactor = 0.7; // see below (similar to ~ChanceRiseSlownessFactor)

        public const double MinRampLength = 1; //minimum length of the 'ramp' part of the table top
        public const double MinWetness = 0.5; // minimum wetness. Wetness stays near MinWetness near the start of the track.
        public const double MaxWetness = 0.9;// maximum wetness. Wetness rises from MinWetness to anything up to MaxWetness by the end of the track.
        public const bool LinkWetnessToFrequency = true; //whether the wetness rises in line with the frequency change

        public const double BasePulseFrequency = 0.5; // the 'normal', quiescent frequency that it normally pulses at
        public const double MinPulseFrequency = 0.2; // the lowest frequency the pulsing in a section can fall to
        public const double MaxPulseFrequency = 1.5; //the highest frequency the pulsing in a section can rise to
        public const double TabletopChanceRiseSlownessFactor = 0.5; // how slowly the chance of there being a tabletop rises through the track.
            // 0: rises to 'certain' instantly - tabletop on every section
            // 0.5: rises quickly, so a >50% chance of one half way through the track
            // 1 : rises linearly, so a 50% chance of one half way through the track
            // >1: rises slowly, so a <50% chance of one half way through the track

        public const double MinTimeBeforeBreak = 600; // the soonest in the track there can be a 'break' (seconds)
        public const double ChanceOfBreak = 0.1;
        public const double MinBreakLength = 2;
        public const double MaxBreakLength = 10;
        public const double BreakRampLength = 5; //BreakRampLength * 2 + MaxBreakLength must be <= SectionLength

        public const double ChanceOfRise = 0.7; // the chance of the frequency rising as opposed to falling
    }
}
