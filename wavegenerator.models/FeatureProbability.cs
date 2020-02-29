using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class FeatureProbability
    {
        [Range(0, 1)]
        public double Wetness { get; set; }

        [Range(0, 1)]
        public double Frequency { get; set; }

        public string Decide(double r)
        {
            double c = 0;
            if (r < (c += Wetness)) return nameof(Wetness);
            else if (r < (c += Frequency)) return nameof(Frequency);
            else return string.Empty;
        }
    }
}
