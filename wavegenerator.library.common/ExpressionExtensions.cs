using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
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
                throw new InvalidOperationException($"Expression returned NaN (not-a-number): {expression.getExpressionString()}");
            }
            return v;
        }

        public static void verifySyntax(this Expression expression)
        {
            if(!expression.checkSyntax())
            {
                throw new InvalidOperationException($"Expression is invalid syntax: {expression.getExpressionString()}");
            }
        }
    }
}
