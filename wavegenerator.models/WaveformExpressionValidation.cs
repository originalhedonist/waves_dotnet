using Microsoft.CodeAnalysis.Scripting;
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

    public class CarrierFrequencyValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var expression = (string)value;
            if (expression != null)
            {
                var script = CarrierFrequenyExpression.Parse(expression);
                try
                {
                    var val = script.RunAsync(new CarrierFrequencyExpressionParams()).Result;
                    System.Diagnostics.Debug.WriteLine(val);
                }
                catch (CompilationErrorException ex)
                {
                    return new ValidationResult(ex.Message);
                }
            }
            return ValidationResult.Success;
        }
    }



}
