using org.mariuszgromada.math.mxparser;
using System;
using System.IO;

namespace wavegenerator.library.common
{
    public static class ExpressionHelpers
    {
        public static double calculateAndVerify(this Expression expression)
        {
            var v = expression.calculate();
            if(double.IsNaN(v))
            {
                throw new WaveGeneratorException($"Expression returned NaN (not-a-number): {expression.GetDebugString()}");
            }
            return v;
        }

        public static double calculateAndVerify(this Expression expression, double min, double max)
        {
            var v = expression.calculateAndVerify();
            if(v < min || v > max)
            {
                throw new WaveGeneratorException($"Expression result was out of range:\n{v}\n{expression.GetDebugString()}");
            }
            return v;
        }

        public static void verifySyntax(this Expression expression)
        {
            if(!expression.checkSyntax())
            {
                throw new WaveGeneratorException($"Expression is invalid syntax: {expression.GetDebugString()}");
            }
        }

        public static Function verify(this Function function)
        {
            if(!function.checkSyntax())
            {
                throw new WaveGeneratorException($"Function is invalid syntax: {function.GetDebugString()}");
            }
            return function;
        }

        public static string GetDebugString(this Expression expression)
        {
            var sw = new StringWriter();
            sw.WriteLine(expression.getExpressionString());
            for(int i = 0; i < expression.getArgumentsNumber(); i++)
            {
                var arg = expression.getArgument(i);
                sw.WriteLine($"{arg.getArgumentName()} = {arg.getArgumentValue()}");
            }
            return sw.ToString();
        }

        public static string GetDebugString(this Function function)
        {
            var sw = new StringWriter();
            sw.WriteLine(function.getFunctionName());
            for (int i = 0; i < function.getArgumentsNumber(); i++)
            {
                var arg = function.getArgument(i);
                sw.WriteLine($"{arg.getArgumentName()} = {arg.getArgumentValue()}");
            }
            return sw.ToString();
        }

    }
}
