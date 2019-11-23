using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class RiserModel
    {
        public int Count { get; set; }

        public TimeSpan EarliestTime { get; set; }

        public TimeSpan LengthEach { get; set; }

        [Range(0.0001, 1)]
        public double Amount { get; set; }
    }
}
