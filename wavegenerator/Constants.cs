using System;
using System.ComponentModel;

namespace wavegenerator
{
    public class Constants
    {
        [Description("Whether to use randomization")]
#if DEBUG
        public static bool Randomization = false;
#else
        public static bool Randomization = true;
#endif

        [Description("The number of files to create (there is only any point in creating more than 1 if using Randomization, otherwise they will be identical)")]
        public static int NumFiles = 1;

        [Description("Naming strategy (1 = random female name, 2 = random male name, 3 = random any name)")]
        public static int Naming = 3;
        public static void NamingValidation()
        {
            if (!(Naming >= 1 && Naming <= 3)) throw new InvalidOperationException($"Naming must be 1, 2 or 3.");
        }

        [Description("The total length of the track (must be in h:mm:ss format, even if h is zero)")]
#if DEBUG
        public static TimeSpan TrackLength = TimeSpan.FromSeconds(30);
#else
        public static TimeSpan TrackLength = TimeSpan.FromMinutes(5);
#endif
        public static void TrackLengthValidation()
        {
            if (TrackLength.TotalSeconds * WaveFile.SamplingFrequency > int.MaxValue) throw new InvalidOperationException($"Don't be silly. (Max track length is {TimeSpan.FromSeconds(int.MaxValue / WaveFile.SamplingFrequency)}. Which is a long time.)");
        }

        [Description("The length of each section of the track, in seconds. (There will only be a whole number of sections - so a 40s track with 30s sections will only be 30s long)")]
        public static int SectionLength = 30;
        public static void SectionLengthValidation()
        {
            if (SectionLength <= 0) throw new InvalidOperationException($"{nameof(SectionLength)} must be > 0");
            if (SectionLength > TrackLength.TotalSeconds) throw new InvalidOperationException($"{nameof(SectionLength)} must be greater than {nameof(TrackLength)} ({TrackLength})");
        }

        public static int NumSections => (int)(TrackLength.TotalSeconds / SectionLength);// number of sections in the track

        [Description("The carrier frequency of the LEFT channel at the START of the track. Does not have to be an integer, so for instance you can have 600.0 left and 600.1 right")]
        public static double CarrierFrequencyLeftStart = 600;

        [Description("The carrier frequency of the LEFT channel at the END of the track (if different from start, it rises linearly)")]
        public static double CarrierFrequencyLeftEnd = 600;

        [Description("The carrier frequency of the RIGHT channel at the START of the track")]
        public static double CarrierFrequencyRightStart = 600;

        [Description("The carrier frequency of the RIGHT channel at the END of the track (if different from start, it rises linearly)")]
        public static double CarrierFrequencyRightEnd = 600;

        public static void CarrierFrequencyLeftStartValidation() {if(CarrierFrequencyLeftStart <= 0) throw new InvalidOperationException($"{nameof(CarrierFrequencyLeftStart)} must be > 0"); }
        public static void CarrierFrequencyRightStartValidation() {if(CarrierFrequencyRightStart <= 0) throw new InvalidOperationException($"{nameof(CarrierFrequencyRightStart)} must be > 0"); }
        public static void CarrierFrequencyLeftEndValidation() {if(CarrierFrequencyLeftEnd <= 0) throw new InvalidOperationException($"{nameof(CarrierFrequencyLeftEnd)} must be > 0"); }
        public static void CarrierFrequencyRightEndValidation() {if(CarrierFrequencyRightEnd <= 0) throw new InvalidOperationException($"{nameof(CarrierFrequencyRightEnd)} must be > 0"); }

        [Description("Minimum length of each 'tabletop' section in seconds")]
        public static double MinTabletopLength = 4;// min length of the 'top' part of the table top frequency

        [Description(
            @"Maximum length of each 'tabletop' section in seconds.
              The maximum length of the tabletop on any given section will be anything up to MaxTabletopLength, depending on the progression through the track.
              The actual length of the tabletop on any given section will be anything up to the maximum for the section.")]
        public static double MaxTabletopLength = 15;// max length of the 'top' part of the table top frequency
        public static void MaxTabletopLengthValidate()
        {
            if (MaxTabletopLength < MinTabletopLength) throw new InvalidOperationException($"{nameof(MaxTabletopLength)} must be greater than MinTabletoplength ({MinTabletopLength})");
            if (MaxTabletopLength + 2 * MinRampLength > SectionLength) throw new InvalidOperationException($"{nameof(MaxTabletopLength)} must be less than {nameof(SectionLength)} - 2 x {nameof(MinRampLength)}");
        }

        [Description("The 'normal', quiescent frequency that it normally pulses at")]
        public static double BasePulseFrequency = 0.5;

        [Description("The lowest frequency the pulsing in a section can fall to")]
        public static double MinPulseFrequency = 0.2;

        [Description("The highest frequency the pulsing in a section can rise to")]
        public static double MaxPulseFrequency = 1.5;

        [Description(@"
            Tabletop length rise factor. How fast the maximum tabletop length for a section approaches MaxTabletopLength as the track progresses.
            =0      : Rises to MaxTabletopLength instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to half MaxTabletopLength halfway through the track
            >1 =>   : Rises quicker at the end")]
        public static double TabletopLengthRiseSlownessFactor = 0.7; // see below (similar to ~ChanceRiseSlownessFactor)

        [Description(@"
            Tabletop chance rise factor. How fast the probability of a tabletop for a section rises as the track progresses.
            =0      : Rises to 'certain' instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to a 50% chance halfway through the track
            >1 =>   : Rises quicker at the end")]
        public static double TabletopChanceRiseSlownessFactor = 0.5;

        [Description(@"
            Tabletop frequency rise factor. How fast the variation in frequency of a tabletop for a section rises as the track progresses.
            =0      : Rises to 'MaxPulseFrequency' instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to half way between BasePulseFrequency and MaxPulseFrequency/MinPulseFrequency halfway through the track
            >1 =>   : Rises quicker at the end")]
        public static double TabletopFrequencyRiseSlownessFactor = 0.5;

        [Description("Minimum length of the 'ramp' part of the table top")]
        public static double MinRampLength = 1; //

        [Description("Minimum wetness. Wetness stays near MinWetness near the start of the track.")]
        public static double MinWetness = 0.5; 

        [Description("Maximum wetness. Wetness rises from MinWetness to anything up to MaxWetness by the end of the track.")]
        public static double MaxWetness = 0.9; 

        [Description("Whether the wetness rises in line with the frequency change")]
        public static bool LinkWetnessToFrequency = true; 

        [Description("The chance of the pulse frequency speeding up to MaxPulseFrequency as opposed to slowing down to MinPulseFrequency")]
        public static double ChanceOfRise = 0.7; // the chance of the frequency rising as opposed to falling

        [Description("The soonest in the track there can be a 'break' (seconds)")]
        public static double MinTimeBeforeBreak = 600;

        [Description("The chance of any section after the above time being a break")]
        public static double ChanceOfBreak = 0.1;

        [Description("Minimum break length, in seconds")]
        public static double MinBreakLength = 2;

        [Description("Maximum break length, in seconds")]
        public static double MaxBreakLength = 10;
        public static void MaxBreakLengthValidation()
        {
            if (MaxBreakLength + 2 * BreakRampLength > SectionLength) throw new InvalidOperationException($"{nameof(MaxBreakLength)} must be < {nameof(SectionLength)} - 2 x {nameof(BreakRampLength)}");
        }

        [Description("The length of the 'fadeout' before, and 'fade in' after a break")]
        public static double BreakRampLength = 5;
    }
}
