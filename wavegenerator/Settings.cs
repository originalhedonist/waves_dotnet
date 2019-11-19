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
        public bool Randomization = false;
#else
        public bool Randomization = true;
#endif

        [RegularExpression("abc"), Required]
        public string SomeString { get; set; }

        [Description("Whether to try to use lame (if it is in the PATH) to convert to mp3. If so, and it succeeds, the wav will be deleted, otherwise, it will be left as a wav.")]
        public bool ConvertToMp3 = true;

        [Description("The number of files to create (there is only any point in creating more than 1 if using Randomization, otherwise they will be identical)")]
        public int NumFiles = 1;

        [Description("Naming strategy (1 = random female name, 2 = random male name, 3 = random any name)")]
        [Range(1, 3)]
        public int Naming = 3;

        [Description("The total length of the track (must be in h:mm:ss format, even if h is zero)")]
#if DEBUG
        public TimeSpan TrackLength = TimeSpan.FromSeconds(30);
#else
        public TimeSpan TrackLength = TimeSpan.FromMinutes(5);
#endif
        public void TrackLengthValidation()
        {
            if (TrackLength.TotalSeconds * WaveFile.SamplingFrequency > int.MaxValue) throw new InvalidOperationException($"Don't be silly. (Max track length is {TimeSpan.FromSeconds(int.MaxValue / WaveFile.SamplingFrequency)}. Which is a long time.)");
        }

        [Description("The length of each section of the track, in seconds. (There will only be a whole number of sections - so a 40s track with 30s sections will only be 30s long)")]
        public int SectionLength = 30;
        public void SectionLengthValidation()
        {
            if (SectionLength <= 0) throw new InvalidOperationException($"{nameof(SectionLength)} must be > 0");
            if (SectionLength > TrackLength.TotalSeconds) throw new InvalidOperationException($"{nameof(SectionLength)} must be greater than {nameof(TrackLength)} ({TrackLength})");
        }

        public int NumSections => (int)(TrackLength.TotalSeconds / SectionLength);// number of sections in the track

        [Description("Whether the right channel's carrier signal will be phase shifted from the left's")]
        public bool PhaseShiftCarrier = true;

        [Description("The carrier frequency of the LEFT channel at the START of the track. Does not have to be an integer, so for instance you can have 600.0 left and 600.1 right")]
        public double CarrierFrequencyLeftStart = 600;

        [Description("The carrier frequency of the LEFT channel at the END of the track (if different from start, it rises linearly)")]
        public double CarrierFrequencyLeftEnd = 600;

        [Description("The carrier frequency of the RIGHT channel at the START of the track")]
        public double CarrierFrequencyRightStart = 600;

        [Description("The carrier frequency of the RIGHT channel at the END of the track (if different from start, it rises linearly)")]
        public double CarrierFrequencyRightEnd = 600;

        public void CarrierFrequencyLeftStartValidation() {if(CarrierFrequencyLeftStart <= 0) throw new InvalidOperationException($"{nameof(CarrierFrequencyLeftStart)} must be > 0"); }
        public void CarrierFrequencyRightStartValidation() {if(CarrierFrequencyRightStart <= 0) throw new InvalidOperationException($"{nameof(CarrierFrequencyRightStart)} must be > 0"); }
        public void CarrierFrequencyLeftEndValidation() {if(CarrierFrequencyLeftEnd <= 0) throw new InvalidOperationException($"{nameof(CarrierFrequencyLeftEnd)} must be > 0"); }
        public void CarrierFrequencyRightEndValidation() {if(CarrierFrequencyRightEnd <= 0) throw new InvalidOperationException($"{nameof(CarrierFrequencyRightEnd)} must be > 0"); }

        [Description("Minimum length of each 'tabletop' section in seconds")]
        public double MinTabletopLength => SectionLength / 2;
        // min length of the 'top' part of the table top frequency
        // for now, make this non-optional (it is a property rather than a field)

        [Description(
            @"Maximum length of each 'tabletop' section in seconds.
              The maximum length of the tabletop on any given section will be anything up to MaxTabletopLength, depending on the progression through the track.
              The actual length of the tabletop on any given section will be anything up to the maximum for the section.")]
        public double MaxTabletopLength => SectionLength / 2;// max length of the 'top' part of the table top frequency
        // for now, make this non-optional (it is a property rather than a field)

        public void MaxTabletopLengthValidate()
        {
            if (MaxTabletopLength < MinTabletopLength) throw new InvalidOperationException($"{nameof(MaxTabletopLength)} must be greater than MinTabletoplength ({MinTabletopLength})");
            if (MaxTabletopLength + 2 * MinRampLength > SectionLength) throw new InvalidOperationException($"{nameof(MaxTabletopLength)} must be less than {nameof(SectionLength)} - 2 x {nameof(MinRampLength)}");
        }

        [Description("Whether the pulsing of the right channel will be phase-shifted from the left")]
        public bool PhaseShiftPulses;

        [Description("The 'normal', quiescent frequency that it normally pulses at")]
        public double BasePulseFrequency = 0.5;

        [Description("The lowest frequency the pulsing in a section can fall to")]
        public double MinPulseFrequency = 0.2;

        [Description("The highest frequency the pulsing in a section can rise to")]
        public double MaxPulseFrequency = 1.5;

        [Description(@"
            Tabletop length rise factor. How fast the maximum tabletop length for a section approaches MaxTabletopLength as the track progresses.
            =0      : Rises to MaxTabletopLength instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to half MaxTabletopLength halfway through the track
            >1 =>   : Rises quicker at the end")]
        public double TabletopLengthRiseSlownessFactor = 0.7; // see below (similar to ~ChanceRiseSlownessFactor)

        [Description(@"
            Tabletop chance rise factor. How fast the probability of a tabletop for a section rises as the track progresses.
            =0      : Rises to 'certain' instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to a 50% chance halfway through the track
            >1 =>   : Rises quicker at the end")]
        public double TabletopChanceRiseSlownessFactor = 0.5;

        [Description(@"
            Tabletop frequency rise factor. How fast the variation in frequency of a tabletop for a section rises as the track progresses.
            =0      : Rises to 'MaxPulseFrequency' instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to half way between BasePulseFrequency and MaxPulseFrequency/MinPulseFrequency halfway through the track
            >1 =>   : Rises quicker at the end")]
        public double TabletopFrequencyRiseSlownessFactor = 0.5;

        [Description(@"
            Wetness rise factor. How fast the max wetness (in the middle of the tabletop) for a section rises as the track progresses.
            =0      : Rises to 'MaxWetness' instantly.
            >0, <1  : Rises quicker at the start
            =1 =>   : Rises linearly, to half way between MinWetness and MaxWetness halfway through the track
            >1 =>   : Rises quicker at the end")]
        public double WetnessRiseSlownessFactor = 0.5;

        [Description("Minimum length of the 'ramp' part of the table top")]
        public double MinRampLength = 1; //

        [Description("Minimum wetness. Wetness stays near MinWetness near the start of the track.")]
        public double MinWetness = 0.5; 

        [Description("Maximum wetness. Wetness rises from MinWetness to anything up to MaxWetness by the end of the track.")]
        public double MaxWetness = 0.9; 

        [Description("Whether the wetness rises on the same timeframe as the tabletop (but it's still independent of the scale of the frequency variation)")]
        public bool LinkWetnessToTabletop = true; 

        [Description("The chance of the pulse frequency speeding up to MaxPulseFrequency as opposed to slowing down to MinPulseFrequency")]
        public double ChanceOfRise = 0.7; // the chance of the frequency rising as opposed to falling

        [Description("The soonest in the track there can be a 'break' (seconds)")]
        public double MinTimeBeforeBreak = 600;

        [Description("The chance of any section after the above time being a break")]
        public double ChanceOfBreak = 0.1;

        [Description("Minimum break length, in seconds")]
        public double MinBreakLength = 2;

        [Description("The length of the 'fadeout' before, and 'fade in' after a break")]
        public double BreakRampLength = 5;

        [Description("Maximum break length, in seconds")]
        public double MaxBreakLength = 10;
        public void MaxBreakLengthValidation()
        {
            if (MaxBreakLength + 2 * BreakRampLength > SectionLength) throw new InvalidOperationException($"{nameof(MaxBreakLength)} must be < {nameof(SectionLength)} - 2 x {nameof(BreakRampLength)}");
        }
    }

    public class VarietyModel
    {
        public double Maximum;
        public double Minimum;
        public double Randomness;
        public double Progression;
    }
}
