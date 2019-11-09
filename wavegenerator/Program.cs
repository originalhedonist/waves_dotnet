using System;
namespace wavegenerator
{
    public class Program
    {
        public static void Main()
        {
            var r = new Rising(2, 2);
            r.Write("rising.wav");

            var j = new Jump(2, 2);
            j.Write("jump.wav");
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
