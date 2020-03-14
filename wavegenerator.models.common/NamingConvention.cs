using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using wavegenerator.models;

namespace wavegenerator
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NamingStrategy
    {
        RandomFemaleName = 1, RandomMaleName, RandomAnyName
    }

    public class NamingConvention
    {
        public string Specific { get; set; }
        public NamingStrategy? Strategy { get; set; }
    }

    public class ValidateNamingConvention : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var namingConvention = (NamingConvention)value;
            if (namingConvention != null)
            {
                if (namingConvention.Specific != null && namingConvention.Strategy != null)
                {
                    return new ValidationResult("Cannot specify both 'Specific' and 'Strategy'");
                }

                if (namingConvention.Specific == null && namingConvention.Strategy == null)
                {
                    return new ValidationResult("Must specify either 'Specific' or 'Strategy'");
                }
            }
            return ValidationResult.Success;
        }
    }
}
