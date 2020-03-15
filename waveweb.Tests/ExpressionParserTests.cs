using org.mariuszgromada.math.mxparser;
using System;
using Xunit;

namespace waveweb.Tests
{
    public class ExpressionParserTests
    {
        [Fact]
        public void CaseSensitive()
        {
            Expression e = new Expression("x+X");
            e.addArguments(new Argument("x"));
            e.addArguments(new Argument("X"));
            e.setArgumentValue("x", 1);
            e.setArgumentValue("X", 2);
            Assert.Equal(3, e.calculate());
        }

        [Theory]
        [InlineData("phase_amp_l", -1, 1)]
        [InlineData("phase_amp_l", 1, 0)]
        [InlineData("phase_amp_r", -1, 0)]
        [InlineData("phase_amp_r", 1, 1)]
        [InlineData("phase_shift", -1, -Math.PI)]
        [InlineData("phase_shift", 0, 0)]
        [InlineData("phase_shift", 1, Math.PI)]
        public void Function(string function, double input, double output)
        {
            var expr = new Expression($"{function}(input)");
            expr.addArguments(new Argument("input", input));
            expr.addFunctions(new Function("phase_amp_l", "(1-p)/2", "p"));
            expr.addFunctions(new Function("phase_amp_r", "(p+1)/2", "p"));
            expr.addFunctions(new Function("phase_shift", "p*pi", "p"));
            Assert.True(expr.checkSyntax());
            Assert.Equal(output, expr.calculate(), 8);
        }

        [Fact]
        public void ConstantValue()
        {
            var expr = new Expression("0.5");
            expr.addArguments(new Argument("x"));
            Assert.True(expr.checkSyntax());
            Assert.Equal(0.5, expr.calculate());
        }
    }
}
