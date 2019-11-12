﻿using System;
using System.ComponentModel;

namespace wavegenerator
{
    public class Constants
    {
        [Description("Whether to use randomization")]
        public static bool Randomization = true;

        [Description("The number of files to create (there is only any point in creating more than 1 if using Randomization, otherwise they will be identical)")]
        public static int NumFiles = 1;

        [Description("Naming strategy (1 = random female name, 2 = random male name, 3 = random any name)")]
        public static int Naming = 3;
        public static void NamingValidation(int newVal)
        {
            if (!(newVal >= 1 && newVal <= 3)) throw new InvalidOperationException($"Naming must be 1, 2 or 3.");
        }

        [Description("The total length of the track (must be in h:mm:ss format, even if h is zero)")]
        public static TimeSpan TrackLength = TimeSpan.FromMinutes(5);
        public static void TrackLengthValidation(TimeSpan newVal)
        {
            if (newVal.TotalSeconds * WaveFile.SamplingFrequency > int.MaxValue) throw new InvalidOperationException($"Don't be silly. (Max track length is {TimeSpan.FromSeconds(int.MaxValue/WaveFile.SamplingFrequency)}. Which is a long time.)");
        }

        [Description("The length of each section of the track")]
        public static int SectionLength = 30;
        public static void SectionLengthValidate(int newVal)
        {
            if (newVal <= 0) throw new InvalidOperationException($"{nameof(SectionLength)} must be > 0");
        }

        public static int NumSections => (int)(TrackLength.TotalSeconds / SectionLength);// number of sections in the track

        [Description("Minimum length of each 'tabletop' section in seconds")]
        public static double MinTabletopLength = 4;// min length of the 'top' part of the table top frequency

        [Description(
            @"Maximum length of each 'tabletop' section in seconds.
              The maximum length of the tabletop on any given section will be anything up to MaxTabletopLength, depending on the progression through the track.
              The actual length of the tabletop on any given section will be anything up to the maximum for the section.")]
        public static double MaxTabletopLength = 15;// max length of the 'top' part of the table top frequency
        public static void MaxTabletopLengthValidate(double newVal)
        {
            if (newVal < MinTabletopLength) throw new InvalidOperationException($"{nameof(MaxTabletopLength)} must be greater than MinTabletoplength ({MinTabletopLength})");
        }

        [Description(@"
            Tabletop length rise factor. How fast the maximum tabletop length for a section approaches MaxTabletopLength as the track progresses.
            =0      : Rises to MaxTabletopLength instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to half MaxTabletopLength halfway through the track
            >1 =>   : Rises quicker at the end")]
        public static double TabletopLengthRiseSlownessFactor = 0.7; // see below (similar to ~ChanceRiseSlownessFactor)

        [Description("Minimum length of the 'ramp' part of the table top")]
        public static double MinRampLength = 1; //

        [Description("Minimum wetness. Wetness stays near MinWetness near the start of the track.")]
        public static double MinWetness = 0.5; 

        [Description("Maximum wetness. Wetness rises from MinWetness to anything up to MaxWetness by the end of the track.")]
        public static double MaxWetness = 0.9; 

        [Description("Whether the wetness rises in line with the frequency change")]
        public static bool LinkWetnessToFrequency = true; 

        [Description("The 'normal', quiescent frequency that it normally pulses at")]
        public static double BasePulseFrequency = 0.5;

        [Description("The lowest frequency the pulsing in a section can fall to")]
        public static double MinPulseFrequency = 0.2;

        [Description("The highest frequency the pulsing in a section can rise to")]
        public static double MaxPulseFrequency = 1.5;

        [Description("The chance of the pulse frequency speeding up to MaxPulseFrequency as opposed to slowing down to MinPulseFrequency")]
        public static double ChanceOfRise = 0.7; // the chance of the frequency rising as opposed to falling

        [Description(@"
            Tabletop chance rise factor. How fast the probability of a tabletop for a section rises as the track progresses.
            =0      : Rises to 'certain' instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to a 50% chance halfway through the track
            >1 =>   : Rises quicker at the end")]
        public static double TabletopChanceRiseSlownessFactor = 0.5;

        [Description("The soonest in the track there can be a 'break' (seconds)")]
        public static double MinTimeBeforeBreak = 600;

        [Description("The chance of a break")]
        public static double ChanceOfBreak = 0.1;

        [Description("Minimum break length, in seconds")]
        public static double MinBreakLength = 2;

        [Description("Maximum break length, in seconds")]
        public static double MaxBreakLength = 10;

        [Description("The length of the 'fadeout' before, and 'fade in' after a break")]
        public static double BreakRampLength = 5;
    }
}
