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
                    script.checkSyntax();
                }
                catch(Exception ex)
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
                    script.calculate();
                }
                catch (Exception ex)
                {
                    return new ValidationResult(ex.Message);
                }
            }
            return ValidationResult.Success;
        }
    }



}
