using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace wavegenerator
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
