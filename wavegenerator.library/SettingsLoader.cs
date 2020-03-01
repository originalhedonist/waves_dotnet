using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json;
using wavegenerator.models;

namespace wavegenerator.library
{
    public static class SettingsLoader
    {
        public static Settings LoadAndValidateSettings(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var newSettings = JsonConvert.DeserializeObject<Settings>(json);
            Validator.ValidateObject(newSettings, new ValidationContext(newSettings), true);
            return newSettings;
        }
    }
}
