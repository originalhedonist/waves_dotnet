using System;
using System.Collections.Concurrent;

namespace wavegenerator
{
    public class PulseGenerator : TabletopGenerator, IDecideWetness
    {
        private readonly ConcurrentDictionary<int, bool> isBreakCache = new ConcurrentDictionary<int, bool>();


        public PulseGenerator(int sectionLengthSeconds, int numSections, short channels) : base(Constants.BasePulseFrequency, sectionLengthSeconds, numSections, channels)
        {
        }

        private bool IsBreak(int section) => isBreakCache.GetOrAdd(section, s =>
        {
            //no breaks in the first ten minutes
            if (s * sectionLengthSeconds < Constants.MinBreakSeconds) return false;
            return Randomizer.GetRandom() > 0.9; //10% chance of being a break after ten mins
        });

        public override double Amplitude(double t, int n, int channel)
        {
            var section = Section(n);
            double baseA = base.Amplitude(t, n, channel);// must always calculate it, even if we don't use it - it might (does) increment something important

            if (IsBreak(section))
            {
                double ts = t - (section * sectionLengthSeconds); //time through the current section
                var breakLength = Randomizer.GetRandom() * (Constants.MaxBreakLength - Constants.MinBreakLength) + Constants.MinBreakLength;
                var p = new TabletopParams
                {
                    RampLength = Constants.BreakRampLength,
                    TopLength = breakLength,
                    RampsUseSin2 = true
                };
                double a = TabletopAlgorithm.GetY(ts, sectionLengthSeconds, baseA, 0, p);
                return a;
            }
            else
                return baseA;
        }

        protected override TabletopParams CreateTabletopParamsForSection(int section)
        {
            if (IsBreak(section)) return new TabletopParams { RampLength = 0, TopLength = 0, RampsUseSin2 = false };//don't apply a table top if we're on a break

            //first decide if it has a tabletop at all.
            //the chance of it being something at all rises from 0% to 100%.
            double progression = ((float)section + 1) / numSections; // <= 1
            var isTabletop = Randomizer.GetRandom() < progression;
            if (isTabletop)
            {
                //if it's a tabletop:
                double topLength = Randomizer.GetRandom() * progression * (Constants.MaxTabletopLength - Constants.MinTabletopLength) + Constants.MinTabletopLength;
                double maxRampLength = (sectionLengthSeconds - topLength) / 2;
                if (Constants.MinRampLength > maxRampLength) throw new InvalidOperationException($"MinRampLength must be <= maxRampLength. MinTabletopLength could be too high.");

                // TODO: could feasibly be MinRampLength at the start of the track. Desirable?
                double rampLength = Constants.MinRampLength + Randomizer.GetRandom() * (maxRampLength - Constants.MinRampLength);
                var result = new TabletopParams
                {
                    RampLength = rampLength,
                    TopLength = topLength,
                    RampsUseSin2 = true
                };
                return result;
            }
            else
            {
                var result = new TabletopParams();
                return result;
            }
        }

        protected override double CreateTopFrequency(int section)
        {
            double progression = ((float)section + 1) / numSections; // <= 1
            //20% of being a fall, 80% chance a rise
            var isRise = Randomizer.GetRandom() < Constants.ChanceOfRise;
            double frequencyLimit = isRise ? Constants.MaxPulseFrequency : Constants.MinPulseFrequency;
            double frequencyChangeLimit = frequencyLimit - baseFrequency;
            double topFrequency = Randomizer.GetRandom() * frequencyChangeLimit * progression;
            return topFrequency;
        }


        private readonly ConcurrentDictionary<int, double> wetnessCache = new ConcurrentDictionary<int, double>();
        public double Wetness(double t, int n)
        {
            // rise in a sin^2 fashion from MinWetness to MaxWetness
            int section = Section(n);
            double maxWetnessForSection = wetnessCache.GetOrAdd(section, s =>
            {
                double progression = ((float)s + 1) / numSections; // <= 1
                double maxWetness = Constants.MinWetness + progression * Randomizer.GetRandom() * (Constants.MaxWetness - Constants.MinWetness);
                return maxWetness;
            });
            double ts = t - (section * sectionLengthSeconds); //time through the current section
            double ps = ts / sectionLengthSeconds; // progression through section
            double x = ps * Math.PI;// max wetness in the middle (x = pi/2)
            double w = Math.Pow(Math.Sin(x), 2);
            return w;
        }
    }
}
