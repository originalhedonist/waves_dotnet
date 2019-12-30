using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class FeatureProbability
    {
        [Range(double.Epsilon, 1)]
        public double Wetness { get; set; }

        [Range(double.Epsilon, 1)]
        public double Frequency { get; set; }

        [Range(double.Epsilon, 1)]
        public double PeaksAndTroughs { get; set; }

        public string Decide(double r)
        {
            double c = 0;
            if (r < (c += Wetness)) return nameof(Wetness);
            else if (r < (c += Frequency)) return nameof(Frequency);
            else if (r < (c += PeaksAndTroughs)) return nameof(PeaksAndTroughs);
            else return string.Empty;
        }
    }
}
