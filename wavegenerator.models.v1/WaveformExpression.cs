using org.mariuszgromada.math.mxparser;

namespace wavegenerator
{
    public static class WaveformExpression
    {
        public static Expression Parse(string expressionString)
        {
            var expression = new Expression(expressionString);
            expression.addArguments(new Argument(nameof(WaveformExpressionParams.x)));
            return expression;
        }
    }

    public static class CarrierFrequenyExpression
    {
        public static Expression Parse(string expressionString)
        {
            var expression = new Expression(expressionString);
            expression.addArguments(new Argument(nameof(CarrierFrequencyExpressionParams.t)));
            expression.addArguments(new Argument(nameof(CarrierFrequencyExpressionParams.T)));
            expression.addArguments(new Argument(nameof(CarrierFrequencyExpressionParams.v)));
            return expression;
        }
    }
}
