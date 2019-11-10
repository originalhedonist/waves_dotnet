using System;

namespace wavegenerator
{
    public class Program
    {
        public static void Main()
        {
            var r = new TabletopTest(261.6, 10, 2, 2);
            r.Write("tabletop.wav");

        }
    }

    public class TabletopTest : TabletopGenerator
    {
        public TabletopTest(double baseFrequency, int sectionLengthSeconds, int numSections, short channels) : base(baseFrequency, sectionLengthSeconds, numSections, channels)
        {
        }

        protected override Params CreateParams(int section)
        {
            return new Params
            {
                TopFrequency = section == 0 ? 261.6*2 : 261.6 / 2,
                RampLength = 1,
                TopLength = 4,
                RampsUseSin2 = true
            };
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
