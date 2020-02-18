using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator
{
    public class WaveformExpressionValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var expression = (string)value;
            if (expression != null)
            {
                var script = WaveformExpression.Parse(expression);
                try
                {
                    var val = script.RunAsync(new WaveformExpressionParams()).Result;
                    System.Diagnostics.Debug.WriteLine(val);
                }
                catch(CompilationErrorException ex)
                {
                    return new ValidationResult(ex.Message);
                }
            }
            return ValidationResult.Success;
        }
    }
    public static class WaveformExpression
    {
        public static Script<double> Parse(string expression)
        {
            return CSharpScript.Create<double>(expression,
                ScriptOptions.Default.WithImports("System"),
                typeof(WaveformExpressionParams));
        }
    }
    public class FeatureProbabilityValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var featureProbability = (FeatureProbability)value;
            if (featureProbability != null)
            {
                if (featureProbability.Frequency + featureProbability.PeaksAndTroughs + featureProbability.Wetness > 1)
                {
                    return new ValidationResult("Total FeatureProbability cannot be > 1.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
