using org.mariuszgromada.math.mxparser;
using System;

namespace wavegenerator.library
{
    public class FunctionEvaluator : FunctionExtension
    {
        private readonly IAmplitude generator;
        private readonly string[] parameterNames;
        private readonly double[] parameterValues;
        public FunctionEvaluator(IAmplitude generator)
        {
            this.generator = generator;
            parameterNames = new[] { "t", "n", "channel" };
            parameterValues = new double[] { 0, 0, 0 };
        }

        public double calculate() => generator.Amplitude(
                parameterValues[0],
                (int)Math.Round(parameterValues[1], 0),
                (int)Math.Round(parameterValues[2], 0)).Result;

        public FunctionExtension clone() => new FunctionEvaluator(generator);
        public string getParameterName(int parameterIndex) => parameterNames[parameterIndex];
        public int getParametersNumber() => parameterNames.Length;
        public void setParameterValue(int parameterIndex, double parameterValue) => parameterValues[parameterIndex] = parameterValue;
    }
}