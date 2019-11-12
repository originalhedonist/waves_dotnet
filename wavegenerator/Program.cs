using System;
using System.IO;

namespace wavegenerator
{
    public class Program
    {
        private static string compositionName;

        public static void Main()
        {
            ConstantsParameterization.ParameterizeConstants();

            var pulseGenerator = new PulseGenerator(
                sectionLengthSeconds: Constants.SectionLength,
                numSections: Constants.NumSections,
                channels: 2);
            var carrierFrequencyApplier = new CarrierFrequencyApplier(pulseGenerator,
                carrierFrequencyRight: 600,
                carrierFrequencyLeft: 600);
            compositionName = $"composition_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}"; // think of something more imaginative...
            carrierFrequencyApplier.Write($"{compositionName}.wav");
        }

        public static void WriteLine(string line)
        {
            Console.WriteLine(line);
            File.AppendAllLines($"{compositionName}.txt", new[] { line });
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
