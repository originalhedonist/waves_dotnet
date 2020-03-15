using org.mariuszgromada.math.mxparser;
using System;
using System.Threading.Tasks;
using wavegenerator.library.common;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class PulseV2WaveFile : FrequencyFunctionWaveFile
    {
        private readonly Expression frequencyExpression;
        private readonly Expression pulseExpression;

        public PulseV2WaveFile(ISamplingFrequencyProvider samplingFrequencyProvider, 
            int numberOfChannels,
            PulseComponent component,
            Constant[] constants,
            Function[] functions) 
            : base(numberOfChannels: numberOfChannels, phaseShiftChannels: false, samplingFrequencyProvider.SamplingFrequency)
        {
            this.frequencyExpression = new Expression(component.Frequency);
            this.frequencyExpression.addConstants(constants);
            this.frequencyExpression.addFunctions(functions);
            this.frequencyExpression.addArguments(new Argument("t"), new Argument("n"), new Argument("channel"));
            this.frequencyExpression.verifySyntax();

            this.pulseExpression = new Expression(component.Pulse);
            this.pulseExpression.addConstants(constants);
            this.pulseExpression.addFunctions(functions);
            this.pulseExpression.addArguments(new Argument("x"), new Argument("t"), new Argument("n"), new Argument("channel"));
            this.pulseExpression.verifySyntax();
        }

        protected override Task<double> GetWaveformSample(double[] x, bool phaseShiftChannels, double t, int n, int channel)
        {
            double xparam = x[channel] / (2 * Math.PI);
            this.pulseExpression.setArgumentValue("x", xparam);
            this.pulseExpression.setArgumentValue("t", t);
            this.pulseExpression.setArgumentValue("n", n);
            this.pulseExpression.setArgumentValue("channel", channel);
            double val = pulseExpression.calculateAndVerify(-1, 1);
            return Task.FromResult(val);

        }

        protected override Task<double> Frequency(double t, int n, int channel)
        {
            frequencyExpression.setArgumentValue("t", t);
            frequencyExpression.setArgumentValue("n", n);
            frequencyExpression.setArgumentValue("channel", channel);
            var val = frequencyExpression.calculateAndVerify();
            return Task.FromResult(val);
        }
    }
}