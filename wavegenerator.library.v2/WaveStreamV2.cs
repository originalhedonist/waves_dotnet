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

#nullable enable
        private readonly PulseV2WaveFile? phase;
#nullable disable
        private readonly PulseV2WaveFile[] channels;

        public WaveStreamV2(SettingsV2 settings, ISamplingFrequencyProvider samplingFrequencyProvider, IProgressReporter progressReporter)
            : base(settings, samplingFrequencyProvider, progressReporter)
        {
            this.settings = settings;
            var functions = new List<Function>();
            var constants = new List<Constant> { new Constant("N", N) };
            if (settings.Phase != null)
            {
                phase = new PulseV2WaveFile(samplingFrequencyProvider, settings.Phase, constants.ToArray(), new Function[] { });
                functions.Add(new Function("phase_amp_l", "(1-p)/2", "p"));
                functions.Add(new Function("phase_amp_r", "(p+1)/2", "p"));
                functions.Add(new Function("phase_shift", "p*pi", "p"));
                //functions.Add(new Function("phase", ))
            }

            channels = new PulseV2WaveFile[]
            {
                new PulseV2WaveFile(samplingFrequencyProvider, settings.Right, constants.ToArray(), functions.ToArray()),
                new PulseV2WaveFile(samplingFrequencyProvider, settings.Left, constants.ToArray(), functions.ToArray())
            };
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            return await channels[channel].Amplitude(t, n, channel);
        }
    }

    public class FunctionEvaluator : FunctionExtension
    {
        private readonly IAmplitude generator;

        public FunctionEvaluator(IAmplitude generator)
        {
            this.generator = generator;
        }

        public double calculate()
        {
            throw new NotImplementedException();
        }

        public FunctionExtension clone()
        {
            throw new NotImplementedException();
        }

        public string getParameterName(int parameterIndex)
        {
            throw new NotImplementedException();
        }

        public int getParametersNumber()
        {
            throw new NotImplementedException();
        }

        public void setParameterValue(int parameterIndex, double parameterValue)
        {
            throw new NotImplementedException();
        }
    }

    public class PulseV2WaveFile : FrequencyFunctionWaveFile, FunctionExtension
    {
        private readonly Expression frequencyExpression;
        private readonly Expression pulseExpression;

        public PulseV2WaveFile(ISamplingFrequencyProvider samplingFrequencyProvider, 
            PulseSettingsV2 pulseSettings,
            Constant[] constants,
            Function[] functions) 
            : base(numberOfChannels: 2, phaseShiftChannels: false, samplingFrequencyProvider.SamplingFrequency)
        {
            this.frequencyExpression = new Expression(pulseSettings.Frequency);
            this.frequencyExpression.addConstants(constants);
            this.frequencyExpression.addFunctions(functions);
            this.frequencyExpression.addArguments(new Argument("t"), new Argument("n"));
            this.frequencyExpression.verifySyntax();

            this.pulseExpression = new Expression(pulseSettings.Pulse);
            this.pulseExpression.addConstants(constants);
            this.pulseExpression.addFunctions(functions);
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

        public int getParametersNumber()
        {
            throw new NotImplementedException();
        }

        public void setParameterValue(int parameterIndex, double parameterValue)
        {
            throw new NotImplementedException();
        }

        public string getParameterName(int parameterIndex)
        {
            throw new NotImplementedException();
        }

        public double calculate()
        {
            throw new NotImplementedException();
        }

        public FunctionExtension clone()
        {
            throw new NotImplementedException();
        }
    }
}