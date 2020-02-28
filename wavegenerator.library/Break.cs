using System;

namespace wavegenerator
{
    public class Break
    {
        private readonly TimeSpan midTime;
        private readonly TimeSpan length;
        private readonly TimeSpan rampLength;
        private readonly double startTime;
        private readonly double endTime;
        private readonly TabletopParams tabletopParams;

        public Break(Randomizer randomizer, TimeSpan midTime, BreakModel breakModel)
        {
            this.midTime = midTime;
            rampLength = breakModel.RampLength;
            length = randomizer.GetRandom(0.5) * (breakModel.MaxLength - breakModel.MinLength) + breakModel.MinLength;
            startTime = (this.midTime - (0.5 * length) - rampLength).TotalSeconds;
            endTime = startTime + (2 * rampLength + length).TotalSeconds;
            tabletopParams = new TabletopParams
            {
                TopLength = length.TotalSeconds,
                RampLength = rampLength.TotalSeconds,
                RampsUseSin2 = true
            };
        }

        public bool IsWithin(double t) => t >= startTime && t < endTime;

        //it'll never be 'before the first ramp' or 'after the last ramp', but that's ok
        public double Attenuation(double t) => TabletopAlgorithm.GetY(t - startTime, endTime - startTime, 1, 0, tabletopParams);
    }
}
