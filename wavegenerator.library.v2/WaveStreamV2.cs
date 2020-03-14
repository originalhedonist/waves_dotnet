using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wavegenerator.library.common;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class WaveStreamV2 : WaveStreamBase, IWaveStream
    {
        private readonly SettingsV2 settings;
        private readonly PulseV2WaveFile[] channels;

        public WaveStreamV2(SettingsV2 settings, ISamplingFrequencyProvider samplingFrequencyProvider, IProgressReporter progressReporter)
            : base(settings, samplingFrequencyProvider, progressReporter)
        {
            this.settings = settings;

            channels = new PulseV2WaveFile[]
            {
                new PulseV2WaveFile(samplingFrequencyProvider, settings.Right.Frequency, settings.Right.Pulse, new Constant("N", N)),
                new PulseV2WaveFile(samplingFrequencyProvider, settings.Left.Frequency, settings.Left.Pulse, new Constant("N", N))
            };
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            return await channels[channel].Amplitude(t, n, channel);
        }
    }

    public class PulseV2WaveFile : FrequencyFunctionWaveFile
    {
        private readonly Expression frequencyExpression;
        private readonly Expression pulseExpression;

        public PulseV2WaveFile(ISamplingFrequencyProvider samplingFrequencyProvider, 
            string frequencyExpression, 
            string pulseExpression,
            params Constant[] constants) 
            : base(numberOfChannels: 2, phaseShiftChannels: false, samplingFrequencyProvider.SamplingFrequency)
        {
            this.frequencyExpression = new Expression(frequencyExpression);
            this.frequencyExpression.addConstants(constants);
            this.frequencyExpression.addArguments(new Argument("t"), new Argument("n"));
            this.frequencyExpression.verifySyntax();

            this.pulseExpression = new Expression(pulseExpression);
            this.pulseExpression.addConstants(constants);
            this.pulseExpression.addArguments(new Argument("x"));
            this.pulseExpression.verifySyntax();
        }

        protected override Task<double> GetWaveformSample(double[] x, bool phaseShiftChannels, int channel)
        {
            double xparam = x[channel] / (2 * Math.PI);
            this.pulseExpression.setArgumentValue("x", xparam);
            double val = pulseExpression.calculateAndVerify();
            return Task.FromResult(val);

        }

        protected override Task<double> Frequency(double t, int n, int channel)
        {
            frequencyExpression.setArgumentValue("t", t);
            frequencyExpression.setArgumentValue("n", n);
            var val = frequencyExpression.calculateAndVerify();
            return Task.FromResult(val);
        }
    }
}