using System;
namespace wavegenerator
{
    public class Constants
    {
        public const int SectionLength = 30;// length of each 'section' (a 'section' containing one 'tabletop' of either rising, or falling, pulse frequency in the middle)
        public const int NumSections = 10;// number of sections in the track
        public const double MinTabletopLength = 1;// min length of the 'top' part of the table top frequency
        public const double MaxTabletopLength = 20;// max length of the 'top' part of the table top frequency
        public const double MinRampLength = 1; //minimum length of the 'ramp' part of the table top
        public const double MinWetness = 0.5; // minimum wetness. Wetness stays near MinWetness near the start of the track.
        public const double MaxWetness = 1;// maximum wetness. Wetness rises from MinWetness to anything up to MaxWetness by the end of the track.
        public const bool LinkWetnessToFrequency = true; //whether the wetness rises in line with the frequency change

        public const double BasePulseFrequency = 0.5; // the 'normal', quiescent frequency that it normally pulses at
        public const double MinPulseFrequency = 0.2; // the lowest frequency the pulsing in a section can fall to
        public const double MaxPulseFrequency = 1.5; //the highest frequency the pulsing in a section can rise to

        public const double MinTimeBeforeBreak = 600; // the soonest in the track there can be a 'break' (seconds)
        public const double ChanceOfBreak = 0.1;
        public const double MinBreakLength = 1;
        public const double MaxBreakLength = 10;
        public const double BreakRampLength = 2; //BreakRampLength * 2 + MaxBreakLength must be <= SectionLength

        public const double ChanceOfRise = 0.8; // the chance of the frequency rising as opposed to falling
    }
}
