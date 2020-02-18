using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace wavegenerator
{
    public static class WaveformExpression
    {
        public static Script<double> Parse(string expression)
        {
            return CSharpScript.Create<double>(expression,
                ScriptOptions.Default.WithImports("System"),
                typeof(WaveformExpressionParams));
        }
    }

    public static class CarrierFrequenyExpression
    {
        public static Script<double> Parse(string expression)
        {
            return CSharpScript.Create<double>(expression,
                ScriptOptions.Default.WithImports("System"),
                typeof(CarrierFrequencyExpressionParams));
        }
    }
}
