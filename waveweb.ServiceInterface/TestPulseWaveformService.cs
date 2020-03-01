﻿using System;
using ServiceStack;
using System.Threading.Tasks;
using wavegenerator;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class TestPulseWaveformService : Service
    {
        private readonly IWaveformTestPulseGeneratorProvider pulseGeneratorProvider;

        public TestPulseWaveformService(IWaveformTestPulseGeneratorProvider pulseGeneratorProvider)
        {
            this.pulseGeneratorProvider = pulseGeneratorProvider;
        }

        public TestPulseWaveformResponse Post(
            TestPulseWaveformRequest testPulseWaveformRequest)
        {
            var pulseGenerator = pulseGeneratorProvider.GetPulseGenerator(testPulseWaveformRequest);
            return new TestPulseWaveformResponse
            {
                Success = true,
                SampleNoFeature = new double[][]
                {
                    new double[]{-1,-1},
                    new double[]{0,0},
                    new double[]{1,1}
                },
                SampleLowFrequency = new double[][]
                {
                    new double[]{-1,1},
                    new double[]{0,0},
                    new double[]{1,-1}
                },
                SampleHighFrequency = new double[][]
                {
                    new double[]{-1,2},
                    new double[]{0,1},
                    new double[]{1,1}
                }
            };
        }
    }
}
