using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class RiserModel
    {
        public static RiserModel Default() => new RiserModel
        {
            Count = 2,
            LengthEach = TimeSpan.FromSeconds(10),
            EarliestTime = TimeSpan.FromMinutes(10),
            Amount = 0.15
        };

        public int Count { get; set; }

        public TimeSpan EarliestTime { get; set; }

        public TimeSpan LengthEach { get; set; }

        [Range(0.0001, 1)]
        public double Amount { get; set; }
    }
}
