using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace wavegenerator.models
{
    public class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);

            if(value is IEnumerable e)
            {
                foreach(var obj in e)
                {
                    Validator.TryValidateObject(obj, new ValidationContext(obj), results, true);
                }
            }
            else
            {
                Validator.TryValidateObject(value, context, results, true);
            }
            

            if (results.Count != 0)
            {
                var msgs = results.Select(r => $"{validationContext.DisplayName}: {r.ErrorMessage}");
                return new ValidationResult(string.Join(Environment.NewLine, msgs));
            }

            return ValidationResult.Success;
        }
    }
}

