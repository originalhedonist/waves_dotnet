using System;

namespace wavegenerator
{
    public class Program
    {

        public static void Main()
        {
            CheckConstants();

            var pulseGenerator = new PulseGenerator(
                sectionLengthSeconds: Constants.SectionLength,
                numSections: Constants.NumSections,
                channels: 2);
            var carrierFrequencyApplier = new CarrierFrequencyApplier<PulseGenerator>(pulseGenerator, 600);
            carrierFrequencyApplier.Write($"composition_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.wav");

        }

        private static void CheckConstants()
        {
            if (Constants.MaxTabletopLength >= Constants.SectionLength - 2 * Constants.MinRampLength)
                throw new InvalidOperationException($"MaxTabletopLength must be < SectionLength - 2*MinRampLength");

            if (Constants.MaxBreakLength >= Constants.SectionLength - 2 * Constants.BreakRampLength)
                throw new InvalidOperationException($"MaxBreakLength must be < SectionLength - 2*BreakRampLength");
        }
    }

    public class TabletopTest : TabletopGenerator
    {
        public TabletopTest(double baseFrequency, int sectionLengthSeconds, int numSections, short channels) : base(baseFrequency, sectionLengthSeconds, numSections, channels)
        {
        }

        protected override TabletopParams CreateTabletopParamsForSection(int section)
        {
            return new TabletopParams
            {
                RampLength = 1,
                TopLength = 4,
                RampsUseSin2 = true
            };
        }

        protected override double CreateTopFrequency(int section)
        {
            return 261.6 * 2;
        }
    }


    public class Jump : FrequencyFunctionWaveFile
    {
        public Jump(int lengthSeconds, short channels) : base(lengthSeconds, channels)
        {
        }

        protected override double Frequency(double t, int n, int channel)
        {
            if (t < 1)
                return 261.6;
            else
                return 261.1 * 2;
        }
    }

    public class Rising : FrequencyFunctionWaveFile
    {
        public Rising(int lengthSeconds, short channels) : base(lengthSeconds, channels)
        {
        }

        protected override double Frequency(double t, int n, int channel)
        {
            return 261.6 * (1 + (float)n / N);
        }
    }
}
