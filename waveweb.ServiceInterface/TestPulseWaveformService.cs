using System;
using System.Collections.Generic;
using ServiceStack;
using System.Threading.Tasks;
using waveweb.ServiceModel;
using wavegenerator.models;

namespace waveweb.ServiceInterface
{
    public class TestPulseWaveformService : Service
    {
        private readonly IWaveformTestPulseGeneratorProvider pulseGeneratorProvider;

        public TestPulseWaveformService(IWaveformTestPulseGeneratorProvider pulseGeneratorProvider)
        {
            this.pulseGeneratorProvider = pulseGeneratorProvider;
        }

        public async Task<TestPulseWaveformResponse> Post(
            TestPulseWaveformRequest testPulseWaveformRequest)
        {
            if (testPulseWaveformRequest.Sections == null)
            {
                throw new ArgumentException("Sections not supplied");
            }

            const int samplingFrequency = 10;

            testPulseWaveformRequest.PulseFrequency.ChanceOfHigh = 1;
            testPulseWaveformRequest.PulseFrequency.Variation.Progression = 0;
            // progression 0 is so it will always choose to use a feature, for the no feature one we use feature chooser to nullify it.

            var hfPulseGenerator = pulseGeneratorProvider.GetPulseGenerator(testPulseWaveformRequest, 
                new GetPulseGeneratorParams { ChooseFeature = nameof(FeatureProbabilityModel.Frequency), SamplingFrequency = samplingFrequency });

            testPulseWaveformRequest.PulseFrequency.ChanceOfHigh = 0;
            var lfPulseGenerator = pulseGeneratorProvider.GetPulseGenerator(testPulseWaveformRequest,
                new GetPulseGeneratorParams { ChooseFeature = nameof(FeatureProbabilityModel.Frequency), SamplingFrequency = samplingFrequency });

            var nfPulseGenerator = pulseGeneratorProvider.GetPulseGenerator(testPulseWaveformRequest,
                new GetPulseGeneratorParams { ChooseFeature = string.Empty, SamplingFrequency = samplingFrequency }); // never chooses a feature

            int N = testPulseWaveformRequest.Sections.SectionLengthSeconds * samplingFrequency;
            var dataNf = new List<double[]>();
            var dataLf = new List<double[]>();
            var dataHf = new List<double[]>();
            for (int n = 0; n <= N; n++)
            {
                var t = testPulseWaveformRequest.Sections.SectionLengthSeconds * (double)n / N;
                var alf = await lfPulseGenerator.Amplitude(t, n, 0);
                var ahf = await hfPulseGenerator.Amplitude(t, n, 0);
                var anf = await nfPulseGenerator.Amplitude(t, n, 0);
                dataNf.Add(new[] { t, anf });
                dataLf.Add(new[] { t, alf });
                dataHf.Add(new[] { t, ahf });
            }
            return new TestPulseWaveformResponse
            {
                Success = true,
                SampleNoFeature = dataNf.ToArray(),
                SampleHighFrequency = dataHf.ToArray(),
                SampleLowFrequency = dataLf.ToArray()
            };
        }
    }
}
