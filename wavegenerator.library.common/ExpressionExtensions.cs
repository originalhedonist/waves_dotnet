using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace wavegenerator.library.common
{
    public static class ExpressionExtensions
    {
        public static double calculateAndVerify(this Expression expression)
        {
            var v = expression.calculate();
            if(double.IsNaN(v))
            {
                throw new InvalidOperationException($"Expression returned NaN (not-a-number): {expression.GetDebugString()}");
            }
            return v;
        }

        public static double calculateAndVerify(this Expression expression, double min, double max)
        {
            var v = expression.calculateAndVerify();
            if(v < min || v > max)
            {
                throw new InvalidOperationException($"Expression result was out of range:\n{v}\n{expression.GetDebugString()}");
            }
            return v;
        }

        public static void verifySyntax(this Expression expression)
        {
            if(!expression.checkSyntax())
            {
                throw new InvalidOperationException($"Expression is invalid syntax: {expression.GetDebugString()}");
            }
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
    }
}
