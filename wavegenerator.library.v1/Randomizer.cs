﻿using System;
using wavegenerator.library.common;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class Randomizer : IGetRandom
    {
        private readonly Random random = new Random();
        private readonly IWaveFileMetadata waveFileMetadata;

        public Randomizer(IWaveFileMetadata waveFileMetadata)
        {
            this.waveFileMetadata = waveFileMetadata;
        }

        public double GetRandom(double defaultValue = 1.0) => waveFileMetadata.Randomization ? random.NextDouble() : defaultValue;

        public double MakeValue(VarianceModel variance, double progress)
        {
            var isTopHalf = GetRandom() >= 0.5 ? -1 : 1;
            var randomnessComponent = Math.Pow(GetRandom(), isTopHalf * variance.Randomness);
            var progressionComponent = Math.Pow(progress, variance.Progression);
            var desiredValue = randomnessComponent * progressionComponent;
            var normalizedValue = Math.Min(1, desiredValue);
            return normalizedValue;
        }

        public double ProportionAlong(VarianceModel variance, double progress, double minValue, double maxValue) => minValue + MakeValue(variance, progress) * (maxValue - minValue);
    }
}